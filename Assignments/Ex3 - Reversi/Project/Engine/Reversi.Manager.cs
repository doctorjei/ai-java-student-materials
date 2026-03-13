namespace Uwu.Games.Reversi.Engine;
using Configuration = Uwu.Data.Configuration;

public class Manager
{
    // Special type to track game state information.
    public enum AppPhase
    {
        Inactive, // The game is not yet started / has ended (inactive).
        UserWait, // Waiting for the user to make a move.
        AiWait,   // Waiting for the computer to make a move.
        Moving,   // A move is in progress but incomplete (e.g., animation playing)
    }

    public record MoveRecord(int MoveNumber, string Player, string Position, Reversi.State Game);

    /***************************************************************************************.
    | Data independent of any game / installation instance (types, static / const elements) |
    `***************************************************************************************/

    // Grid size and labels (can be edited for different board sizes).
    public const int GRID_SIZE = 8;
    public static string Rows { get; private set; } = "12345678";
    public static string Cols { get; private set; } = "ABCDEFGH";
    public static int GetColLabel(string s) => Cols.Contains(s) ? Cols.IndexOf(s[0]) : GRID_SIZE;
    public static int GetRowLabel(string s) => Rows.Contains(s) ? Rows.IndexOf(s[0]) : GRID_SIZE;

    // Static helper Functions (Short, Potentially Inlined)
    public static Player ColorValue(string s) => s == "Black" ? Player.Black : Player.White;
    public static string ColorString(Player c) => c == Player.Black ? "Black" : "White";

    /*********************************************.
    | Persistent data (goes beyond a single game) |
    `*********************************************/
    public Configuration Config { get; protected set; }
    public Data.Statistics GameStats { get; protected set; }
    public Data.Statistics? statisticsBackup;

    // Historical information / backups (for restoring, etc.)
    public int MoveNo { get; private set; }
    private readonly List<MoveRecord> moveHistory;
    public IReadOnlyList<MoveRecord> MoveHistory => moveHistory.AsReadOnly();

    /***************************.
    | Current Game Data & Logic |
    `***************************/
    public AppPhase Phase { get; set; } = AppPhase.Inactive;
    public State Game { get; protected set; } = new State();
    public Agent[] Agents { get; set; } = new Agent[2];
    public bool IsAIPlaySuspended { get; private set; } = false;
    private Thread? workerThread = null;
    private readonly Driver parent;

    public Manager(string settingsFile, Driver _parent)
    {
        // Allocate non-nullable data structures.
        (Config, GameStats, Agents) = InitAppData(settingsFile);
        moveHistory = new(64);
        parent = _parent;

        // See if we need to extend the row / column labels based on grid size.
        while (Rows.Length < GRID_SIZE) Rows += (char)(Rows[^1] + 1);
        while (Cols.Length < GRID_SIZE) Cols += (char)(Cols[^1] + 1);
    }

      /*********************************************************************************\
     /           ======== Callback Routines (Commands & Triggers) ========               \
     \  Commands are typically invoked via UI, occasionally mapped from other commands.  /
      \________________________________________________________________________________*/

    // Starts a new game.
    public void Command_NewGame(object? s, EventArgs? e)
    {
        // Initialize the move history.
        MoveNo = 1;
        moveHistory.Clear();
        IsAIPlaySuspended = false;
        Game.NewSimulation();
    }

    // Undoes the previous move or all moves.
    public void Command_Undo(object? sender, EventArgs? e, bool undoAll)
    {
        KillComputerMoveThread();

        // Save the current game phase so we'll know if we need to perform a restart.
        AppPhase oldGamePhase = Phase;

        // Lock the board to prevent any changes by an active compute move.
        lock (Game)
        {
            // When undoing last move, save current board & player color for later restoration.
            if (moveHistory.Count < MoveNo)
                moveHistory.Add(new MoveRecord(MoveNo, ColorString(Game.Current), "", Game));

            // Undo either the previous move or all moves.
            RestoreGameAt(undoAll ? 0 : MoveNo - 2);

        }

        // If the game was over, restore the statistics and restart it.
        if (oldGamePhase == AppPhase.Inactive)
            GameStats = new Data.Statistics(statisticsBackup!);
    }

    // Redo the next move or all moves.
    public void Command_Redo(object? sender, EventArgs? e, bool redoAll)
    {
        // Redo either the next move or all moves, then start play again.
        RestoreGameAt(redoAll ? moveHistory.Count - 1 : MoveNo);
    }

        // Clear the suspend computer play upon resume.
    public void Command_Resume(object? sender, EventArgs? e) => IsAIPlaySuspended = false;
    public void Command_Resign() => KillComputerMoveThread();

    public void Command_Exit()
    {
        KillComputerMoveThread();
        StoreAppSettings(Config, GameStats, Agents);
    }

    /*************************** General Trigger Order (from New Game) ****************************.
    | [1:StartGame] => [2:StartTurn] => [3:MakeMove] => [4:EndMove] => {Repeat 2-3} => [5:EndGame] |
    `**********************************************************************************************/

    public void Trigger_RollBack() { }  // No-op

    public void Trigger_StartTurn()     // Cascade from StartGame / EndMove triggers, or rollback.
    {
        if (!Game.StartTurn())
            return;

        // Set the game phase according to whether this will be a human or AI move.
        if (IsAIPlayer(Game.Current))
        {
            Phase = AppPhase.AiWait;
            if (!IsAIPlaySuspended)
            {
                // Start a separate thread to perform the computer's move.
                workerThread = new Thread(new ThreadStart(this.GetAIMove))
                {
                    Priority = System.Threading.ThreadPriority.Lowest,
                    Name = "Calculate Computer Move",
                    IsBackground = true
                };
                workerThread.Start();
            }
        }
        else
            Phase = AppPhase.UserWait;
    }

    // Makes a move for the current player.
    public void Trigger_MakeMove(Move move) // Triggered when move execution starts.
    {
        Phase = AppPhase.Moving;            

        // Clean up move history to ensure it contains only moves made prior to this one.
        while (moveHistory.Count > MoveNo - 1)
            moveHistory.RemoveAt(moveHistory.Count - 1);

        string position = Cols[move.Column].ToString() + Rows[move.Row];
        moveHistory.Add(new MoveRecord(MoveNo, ColorString(Game.Current), position, new State(Game)));
        MoveNo++;

        lock (Game)
        {
            Game.MakeMove(move);
        }
    }

    public void Trigger_EndMove() => Game.EndMove(); // Casecade from MakeMove trigger (on completion).

    // Ends the current game, optionally by player resignation.
     // Cascade from EndMove at endgame --> Review
    public void Trigger_EndGame(bool isResignation)
    {
        // If we've already played this rodeo, just exit.
        if (Phase == AppPhase.Inactive)
            return;

        Phase = AppPhase.Inactive;
        Game.EndSimulation();

        // Gather info for calculations - AI player designations and a stastistics backup.
        statisticsBackup = new Data.Statistics(GameStats);

        Player quitter = Player.Empty;
        bool isBlackAI = IsAIPlayer(Player.Black);
        bool isWhiteAI = IsAIPlayer(Player.White);

        if (isResignation)
        {
            if (isBlackAI && !isWhiteAI)
                quitter = Player.White;
            else if (isWhiteAI && !isBlackAI)
                quitter = Player.Black;
            else
                quitter = Game.Current;
        }

        // Update the scores based on whether one player resigned.
        int blackScore = Game.BlackCount + Game.TallyFinalScore(quitter);
        GameStats.Update(blackScore, 64 - blackScore, isBlackAI, isWhiteAI);
    }

    /********************************************************.
    | Major Support Routines (Game Scoring,Undo, Redo, etc.) |
    `********************************************************/

    public void RestoreGameAt(int n) // Restoes game to state before move n was made.
    {
        // Get move record, restore board, reset move number, update display, & reset turn.
        MoveRecord item = moveHistory[n];
        Game = new State(item.Game);
        IsAIPlaySuspended = true;
        MoveNo = n + 1;
    }

    // Get a move from the AI and then post the move to the game state.
    private void GetAIMove() =>
        parent.PostMoveBegin(Agents[Game.Current < 0 ? 0 : 1].DetermineMove(Game));

    // Cancels the computer move thread, if it is active.
    public void KillComputerMoveThread()
    {
        if (workerThread == null || workerThread.ThreadState == ThreadState.Stopped)
            return;

        try
        {
            // workerThread.Abort(); // TODO: Find out why.
            workerThread.Join();
        }
        catch (Exception) { }
        finally { workerThread = null; }
    }

    // Determines if a given color is being played by the computer.
    public bool IsAIPlayer(Player color)
    {
        return color switch
        {
            Player.Black => Agents[0].Variant != Agent.Type.Human,
            Player.White => Agents[1].Variant != Agent.Type.Human,
            _ => false
        };
    }

    public void SetAgentBehaviors()
    {
        foreach (Agent agent in Agents)
            agent.UpdateBehavior();

        // If no moves have been made yet, set the current color to black.
        //        if (LastMoveColor == Player.Empty)
        //            Game.PlayerColor = Player.Black; // TODO: fix before bringing back in.
    }

    /*******************.
    | Callback Routines |
    `*******************/

    // For "dangerous" requests, if game is active, prompt for confirmation first.
    public bool Prompt_Resign(object? sender, EventArgs e) => Phase != AppPhase.Inactive;
    public bool Prompt_Exit(object? sender, EventArgs e) => Phase != AppPhase.Inactive;

    // Not yet connected.
    public void Command_Options(object? sender, EventArgs? e) => Console.WriteLine($"[DBG] Options_Click sent from {sender}: {e}");
    public void Command_Stats(object? sender, EventArgs? e) => Console.WriteLine($"[DBG] Statistics_Click sent from {sender}: {e}");
    public void Command_Help(object? sender, EventArgs? e) => Console.WriteLine($"[DBG] HelpTopics_Click sent from {sender}: {e}");
    public void Command_About(object? sender, EventArgs? e) => Console.WriteLine($"[DBG] About_Click sent from {sender}: {e}");

    /******************************************.
    | Initialization and Data Storage Routines |
    `******************************************/
    public static (Configuration, Data.Statistics, Agent[]) InitAppData(string settingsFile)
    {
        // Load any saved settings from file.
        Configuration settings = new(settingsFile);
        Agent[] agents = new Agent[2];
        Data.Statistics statistics;

        // Process options and statistics, if any, from the settings - one section at a time.
        try { statistics = LoadStats(settings); }
        catch (Exception) { statistics = new(); }

        // Next up: player settings.
        try
        {
            var (types, diffs) = LoadAiSettings(settings);
            agents[0] = new(Player.Black, types[0], diffs[0]);
            agents[1] = new(Player.White, types[1], diffs[1]);
        }
        catch (Exception) { agents[0] = new(); agents[1] = new(); }

        return (settings, statistics, agents);
    }

    public static Data.Statistics LoadStats(Configuration settings) => new()
    {
        // Load saved statistics.
        BlackWins = settings.Get<Int32>("Statistics", "BlackWins"),
        WhiteWins = settings.Get<Int32>("Statistics", "WhiteWins"),
        OverallDraws = settings.Get<Int32>("Statistics", "OverallDraws"),
        BlackTotalScore = settings.Get<Int32>("Statistics", "BlackTotalScore"),
        WhiteTotalScore = settings.Get<Int32>("Statistics", "WhiteTotalScore"),
        ComputerWins = settings.Get<Int32>("Statistics", "ComputerWins"),
        UserWins = settings.Get<Int32>("Statistics", "UserWins"),
        VsComputerDraws = settings.Get<Int32>("Statistics", "VsComputerDraws"),
        ComputerTotalScore = settings.Get<Int32>("Statistics", "ComputerTotalScore"),
        UserTotalScore = settings.Get<Int32>("Statistics", "UserTotalScore")
    };

    public static (Agent.Type[], Agent.AiLevel[]) LoadAiSettings(Configuration settings) =>
        ([settings.Get("Options", "AiType_0", Agent.P1_TYPE),
          settings.Get("Options", "AiType_1", Agent.P2_TYPE)],
         [settings.Get("Options", "AiLevel_0", Agent.P1_DIFFICULTY),
          settings.Get("Options", "AiLevel_1", Agent.P2_DIFFICULTY)]);

    public static void StoreAiSettings(Configuration settings) => StoreAiSettings(settings,
        [Agent.P1_TYPE, Agent.P2_TYPE], [Agent.P1_DIFFICULTY, Agent.P2_DIFFICULTY]);

    public static void StoreAiSettings(Configuration conf, Agent.Type[] types, Agent.AiLevel[] diffs)
    {
        conf.Set("Options", "AiType_0", types[0]);
        conf.Set("Options", "AiType_1", types[1]);
        conf.Set("Options", "AiLevel_0", diffs[0]);
        conf.Set("Options", "AiLevel_1", diffs[1]);
        conf.Save();
    }

    public static void StoreStatistics(Configuration settings, Data.Statistics stats)
    {
        settings.Set("Statistics", "BlackWins", stats.BlackWins);
        settings.Set("Statistics", "WhiteWins", stats.WhiteWins);
        settings.Set("Statistics", "OverallDraws", stats.OverallDraws);
        settings.Set("Statistics", "BlackTotalScore", stats.BlackTotalScore);
        settings.Set("Statistics", "WhiteTotalScore", stats.WhiteTotalScore);
        settings.Set("Statistics", "ComputerWins", stats.ComputerWins);
        settings.Set("Statistics", "UserWins", stats.UserWins);
        settings.Set("Statistics", "VsComputerDraws", stats.VsComputerDraws);
        settings.Set("Statistics", "ComputerTotalScore", stats.ComputerTotalScore);
        settings.Set("Statistics", "UserTotalScore", stats.UserTotalScore);
        settings.Save();
    }

    public static void StoreAppSettings(Configuration settings, Data.Statistics stats, Agent[] agents)
    {
        StoreStatistics(settings, stats);
        StoreAiSettings(settings, [agents[0].Variant, agents[1].Variant],
                                        [agents[0].Level, agents[1].Level]);
    }
}
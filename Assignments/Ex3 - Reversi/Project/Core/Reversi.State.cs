// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

// TODO:
// - Incorporate current player as part of state, including checks and updates, to avoid invalid states
// - Identify and other elements in the "ReversiGame" class that should be moved to the State class.
namespace Uwu.Games.Reversi;

// Supporting classes (player, move)
public enum Player { Black = -1, Empty = 0, White = 1 } // Player representation

public class Move(GamePlaying.IndexKey<State> key) : GamePlaying.State.Move(key)
{
    public int Rank { get; set; } = 0;
    public int Row { get => this[0]; }
    public int Column { get => this[1]; }
}

/// <summary>Class representing the reversi game board.</summary>
public class State : GamePlaying.State<Player, Move, Player>, GamePlaying.LoopingExecutionState<Move>
{
    // Special type to track game phase / step.
    public enum SimPhase
    {
        Terminal, // The game is not yet started / has ended (inactive).
        Waiting,  // Waiting for the agent to enter a move.
        Moving,   // A move is in progress (currently changing state).
    }

    public SimPhase Phase { get; set; } = SimPhase.Terminal;

    public const int DIMENSION_COUNT = 2; // Default number of dimensions
    public const int GRID_SIZE = 8;       // Default size in each dimension

    // Get the next player in typical order.
    public override Player Next { get => (Player)(-(int)Current); }
    new public static GamePlaying.IndexKey<State>.KeyProvider KeyAt { get; } = new();

    // These counts reflect the current board situation.
    public int BlackCount { get; private set; }
    public int WhiteCount { get; private set; }
    public int EmptyCount { get; private set; }
    public int Score { get => WhiteCount - BlackCount; }

    // Intentionally separate so it's hard to accidentally modify by mistake.
    public void setCurrent(Player p) { Current = p; }

    // Creates a new, cleared Board object; updates the counts.
    static State()
    {
        Dimensions = new int[DIMENSION_COUNT];
        
        for (int i = 0; i < DIMENSION_COUNT; i++)
            State.Dimensions[i] = GRID_SIZE;
    }

    public State() : base() { ClearBoard(); UpdateCounts(); }
    public State(State board) : base(board) { } // Copy constructor
    public static Move NewMove(params int[] idx) => new(KeyAt[idx]);
 
    /***************************.
    | Primary Game Flow Methods |
    `--------------------------*/

    // Sets a board with the initial game set-up.
    public void NewSimulation()
    {
        // Clear the board, set two black & two white discs in center, & update the counts.
        ClearBoard();
        this[3, 3] = Player.White;
        this[3, 4] = Player.Black;
        this[4, 3] = Player.Black;
        this[4, 4] = Player.White;
        UpdateCounts();

        Current = Player.Black;
        Phase = SimPhase.Waiting;
    }

    // Starts a new game or optionally, rolls back an ended game. (It's a no-op in Reversi.)
    public void StartSimulation() { }

    public bool StartTurn() // Returns true is the game is active, or false if over.
//    public bool StartTurn_bool() // Returns true is the game is active, or false if over.
    {
        // If the current player cannot make a valid move, forfeit the turn.
        if (!HasAnyValidMove(Current))
        {
            Current = Next;

            // If it happens again, the game is over.
            if (!HasAnyValidMove(Current))
                return false;
        }
        return true;
    }

    // Place disc & flips outflanked pieces. NOTE: Method does **NOT** check move validity!
    public override void MakeMove(Move move) // TODO: Add check to manager...
    {
        // Set the disc on the square.
        this[move.Row, move.Column] = Current;

        // Flip any flanked opponents.
        for (int dRow = -1; dRow <= 1; dRow++)
        {
            for (int dCol = -1; dCol <= 1; dCol++)
            {
                // Are there any outflanked opponents?
                if (!(dRow == 0 && dCol == 0) && IsOutFlanking(Current, move.Row, move.Column, dRow, dCol))
                {
                    int tRow = move.Row + dRow;
                    int tCol = move.Column + dCol;

                    // Flip 'em.
                    while (this[tRow, tCol] != Current)
                    {
                        this[tRow, tCol] = Current;
                        tRow += dRow;
                        tCol += dCol;
                    }
                }
            }
        }
        UpdateCounts();  // Update the counts.
    }

    public void EndMove() => Current = Next; // Next player

    // Ends the current game, optionally by player resignation.
    public int TallyFinalScore(Player quitter)
    {
        // Gather info for calculations - AI player designations and a stastistics backup.
        if (quitter == Player.Empty)
            return 0;
        else if (quitter == Player.Black)
            return 0 - BlackCount;
        else
            return 64 - BlackCount;
    }

    public void EndSimulation() { } // No-op at the moment.

    /**********************.
    |  Supporting Methods  |
    `---------------------*/

    // Determines if the player can make any valid move on the board.
    public bool HasAnyValidMove(Player color)
    {
        // Check all board positions for a valid move.
        for (int _row = 0; _row < 8; _row++)
            for (int _col = 0; _col < 8; _col++)
                if (IsValidMove(color, NewMove(_row, _col)))
                    return true; // Found one!

        return false; // None found.
    }

    // Determines if the game is over (i.e., no valid moves for either player)
    public override bool IsTerminalState() =>
        !HasAnyValidMove(Player.Black) && !HasAnyValidMove(Player.White);

    // Determines if a given move is valid for the player.
    public override bool IsValidMove(Player color, Move move)
    {
        // The square must be empty.
        if (this[move.Row, move.Column] != Player.Empty)
            return false;

        // Must be able to flip at least one opponent disc.
        for (int dRow = -1; dRow <= 1; dRow++)
            for (int dCol = -1; dCol <= 1; dCol++)
                if (!(dRow == 0 && dCol == 0))
                    if (IsOutFlanking(color, move.Row, move.Column, dRow, dCol))
                        return true;

        // No opponents could be flipped.
        return false;
    }

    // Returns number of valid moves player can make on current board.
    public int GetValidMoveCount(Player color)
    {
        int n = 0;

        // Check all board positions.
        for (int _row = 0; _row < GRID_SIZE; _row++)
            for (int _col = 0; _col < GRID_SIZE; _col++)
                if (IsValidMove(color, NewMove(_row, _col)))
                    n++;
        return n;
    }

    private void ClearBoard()
    {
        for (int _row = 0; _row < GRID_SIZE; _row++)
            for (int _col = 0; _col < GRID_SIZE; _col++)
                this[_row, _col] = Player.Empty;
    }

    // Given a move & direction, determines if any opponent discs will be outflanked. NOTE: invalid
    // directions are not excluded! dr & dc may be -1, 0, or 1, but not both zero - i.e., (0,0).
    private bool IsOutFlanking(Player color, int row, int col, int dRow, int dCol)
    {
        // Move in given direction while remaining on board; land on disc of opposite color.
        int _r = row + dRow;
        int _c = col + dCol;
        while (_r >= 0 && _r < GRID_SIZE && _c >= 0 && _c < GRID_SIZE && this[_r, _c] == Next)
        {
            _r += dRow;
            _c += dCol;
        }

        // If past boundary, moved only one space or landed on other-color disc, return false.
        if (_r < 0 || _r > (GRID_SIZE - 1) || _c < 0 || _c > (GRID_SIZE - 1) ||
            (_r - dRow == row && _c - dCol == col) || this[_r, _c] != color)
            return false;

        // Otherwise, return true.
        return true;
    }

    // Updates board counts and safe disc map. MUST be called after any changes to contents.
    private void UpdateCounts()
    {
        // Reset and then tally all counts.
        BlackCount = WhiteCount = EmptyCount = 0;

        for (int _row = 0; _row < GRID_SIZE; _row++)
        {
            for (int _col = 0; _col < GRID_SIZE; _col++)
            {
                switch (this[_row, _col])
                {
                    case Player.Black: BlackCount++; break;
                    case Player.White: WhiteCount++; break;
                    case Player.Empty: EmptyCount++; break;
                }
            }
        }
    }
}

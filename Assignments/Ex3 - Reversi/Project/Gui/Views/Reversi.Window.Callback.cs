using Avalonia.Controls;
using Avalonia.Input;
using Uwu.Games.Reversi.Engine;

namespace Uwu.Games.Reversi;
using WindowClosingEventArgs = WindowClosingEventArgs;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

public partial class MainWindow : Avalonia.Controls.Window
{
    public bool loaded = false;

    #region Initialiation & Setup
    private void ConnectUiComponents()
    {

        // Set up move history view & button / menu options.
        MoveView.ItemsSource = moveRows;
        SetCmdStatus(true, false, false, false, false);

        // Callbacks: GUI window routines.
        Opened += (s, e) => loaded = true;         // Window opened
        SizeChanged += Window_Adjust;              // Window resizing
        PositionChanged += Window_Adjust;          // Window movement
        ScalingChanged += Window_Adjust;           // Scaling changes (e.g., new monitor)
        Closed += (s, e) => parent.Command_Exit(); // "Hard" exit: termination is imminent.

        // Callbacks: Primary game functions.
        NewGameItem.Click += (s, e) => parent.Command_NewGame(s, e, null);
        NewGameButton.Click += (s, e) => parent.Command_NewGame(s, e, null);
        ResumeItem.Click += (s, e) => parent.Command_Resume(s, e, null);
        ResumeButton.Click += (s, e) => parent.Command_Resume(s, e, null);
        UndoItem.Click += (s, e) => parent.Command_Undo(s, e, null, false);
        UndoButton.Click += (s, e) => parent.Command_Undo(s, e, null, false);
        UndoAllItem.Click += (s, e) => parent.Command_Undo(s, e, null, true);
        UndoAllButton.Click += (s, e) => parent.Command_Undo(s, e, null, true);
        RedoItem.Click += (s, e) => parent.Command_Redo(s, e, null, false);
        RedoButton.Click += (s, e) => parent.Command_Redo(s, e, null, false);
        RedoAllItem.Click += (s, e) => parent.Command_Redo(s, e, null, true);
        RedoAllButton.Click += (s, e) => parent.Command_Redo(s, e, null, true);

        // Callbacks: Dialogs (options, statistics, help, etc.)
        OptionsItem.Click += (s, e) => parent.Command_Options(s, e, null);
        StatsItem.Click += (s, e) => parent.Command_Stats(s, e, null);
        HelpTopicsItem.Click += (s, e) => parent.Command_Help(s, e, null);
        AboutItem.Click += (s, e) => parent.Command_About(s, e, null);

        // For resignation/exit, attempt "soft close"; for in-progress game, check with user.
        Closing += ExitRequest;
        ExitItem.Click += ExitRequest;
        ResignItem.Click += ResignRequest;
        ResignButton.Click += ResignRequest;

        void ExitRequest(object? s, EventArgs e) // Prompt the user if needed (see above)
        {
            if (e.GetType() == typeof(WindowClosingEventArgs))
                ((WindowClosingEventArgs)e).Cancel = !closing;

            if (!closing)
                parent.Prompt_Exit(s, e, Uwu.Gui.MsgBox.YesNo("Exit?", "Leave Reversi?", 2));
        }

        void ResignRequest(object? s, RoutedEventArgs e) => // Same: user prompt as needed.
            parent.Prompt_Resign(s, e, Uwu.Gui.MsgBox.YesNo("Resign?", "Resign this game?", 2));
    }
    #endregion

    #region Event Listener Methods (Input Devices)
     /***********************************.
    |   Event Listeners (Input Devices)   |
     \._________________________________*/

    // Keyboard input (row / column entry)
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Check the game phase to ensure it is the user's turn.
        if (manager.Phase != Manager.AppPhase.UserWait)
            return;

        // Convert the key character to column/row number.
        string s = e.KeySymbol ?? "".ToString().ToUpper();

        // Id rhwew'a no input, or more than one character, there's nothing to do; return.
        if (s.Length <= 0 || s.Length > 1)
            return;

        int col = Manager.GetColLabel(s);
        int row = Manager.GetRowLabel(s);

        
        if (col >= 0 && col < Manager.GRID_SIZE) // Valid column; store, clear row, return.
        {
            keyedColNumber = col;
            keyedRowNumber = -1;
            return;
        }
        else if (keyedColNumber < 0 || keyedColNumber > 7) // Current column invalid; return.
            return;
        else if (row < 0 || row > 7) // Invalid row entry from user; reset row/col & return.
        {
            keyedColNumber = keyedRowNumber = -1;
            return;
        }
        else
            keyedRowNumber = row; // Valid row; store.

        // If the keyed row & column are a valid move, make the move on the board.
        Move move = State.NewMove(keyedRowNumber, keyedColNumber);
        if (manager.Game.IsValidMove(manager.Game.Current, move))
            parent.PostMoveBegin(move);
    }

    // Handles a mouse move on a board square.
    protected void Square_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Check game phase to ensure it's user's turn and that source is not null.
        if (manager.Phase != Manager.AppPhase.UserWait || e.Source == null)
            return;

        SquareControl current = (SquareControl)e.Source;
        Move move = State.NewMove(current.Row, current.Col);

        // If the square is a valid move for the current player, indicate it.
        if (manager.Game.IsValidMove(manager.Game.Current, move))
        {
            if (!current.IsActive && current.PreviewContents == (int)Player.Empty)
            {
                // If ShowValidMoves is active, mark square. If preview is inactive, update visual.
                if (ShowValidMoves)
                {
                    current.IsActive = true;
                    if (!PreviewMoves)
                        current.InvalidateVisual();
                }

                // If the preview moves option is active, mark the appropriate squares.
                if (PreviewMoves)
                {
                    State board = new(manager.Game);
                    board.MakeMove(move);

                    // Set up the move preview.
                    for (int i = 0; i < Manager.GRID_SIZE; i++)
                        for (int j = 0; j < Manager.GRID_SIZE; j++)
                            if (board[i, j] != manager.Game[i, j])
                                 squareControls[i, j].SetPreview(board[i, j]);
                }
            }
            current.Cursor = handCursor; // Change to "hand" cursor.
        }
    }

    // Handles a click on a board square. Input → raise preserved events
    protected void Square_OnPointerPressed(object? sender, PointerEventArgs e)
    {
        if (e.GetCurrentPoint((Avalonia.Visual?)e.Source).Properties.IsLeftButtonPressed)
            pressed = (SquareControl?)e.Source;
    }

    // Handles a click on a board square. Input → raise preserved events
    protected void Square_OnPointerReleased(object? sender, PointerEventArgs e)
    {
        var source = (SquareControl?)e.Source;

        if (((PointerReleasedEventArgs)e).InitialPressMouseButton == MouseButton.Left)
        {
            if (pressed == null || pressed != source)
            {
                pressed = null;
                return;
            }

            // Check the game phase to ensure it's the user's turn.
            if (manager.Phase != Manager.AppPhase.UserWait)
                return;

            Move move = State.NewMove(source.Row, source.Col);

            // If the move is valid, restore the cursor and make the move.
            if (manager.Game.IsValidMove(manager.Game.Current, move))
            {
                source.Cursor = defaultCursor;
                parent.PostMoveBegin(move);
            }
        }
    }

    // Handles mouse leving a board square.
    protected void Square_OnPointerExited(object? sender, PointerEventArgs e)
    {
        var source = (SquareControl?)e.Source;

        // If the square is currently active, deactivate it.
        if (source != null && source.IsActive)
        {
            source.IsActive = false;
            source.InvalidateVisual();
        }

        // If the move is being previewed, clear all affected squares; then restore the cursor.
        if (source!.PreviewContents != (int)Player.Empty)
        {
            for (int i = 0; i < Manager.GRID_SIZE; i++)
                for (int j = 0; j < Manager.GRID_SIZE; j++)
                    squareControls[i, j].SetPreview(Player.Empty);

            source.Cursor = defaultCursor;
        }
    }
    #endregion

    #region UI Callback Methods: Primary Functions
     /*******************************************.
    |   [UI Callback Methods] Primary Functions   |
     \._________________________________________*/

    // Window settings handler
    private void Window_Adjust(object? s, EventArgs e) { if (loaded) StoreWindowOptions(); }
    public bool Prompt_Resign(object? s, EventArgs e) => false; // No prompt needed before resigning
    public bool Prompt_Exit(object? s, EventArgs e) => false;   // No prompt needed before exiting
    public void Command_Exit() { closing = true; StoreGuiOptions(); } // Close *now.*

    // Starts a new game.
    public void Command_NewGame(object? sender, EventArgs e)
    {
        // Initialize the information displays.
        ColorLabel.Text = "Current: ";
        ColorPanel.IsVisible = true;

        SetCmdStatus(false, true, false, false, false);

        StatusLabel.Text = "";
        moveRows.Clear();
        StatusPanel.InvalidateVisual();
        //moveListView.InvalidateVisual(); // Unneeded supposedly?

        // Once other GUI elements are ready, get the board ready and update its display.
        UpdateBoardDisplay();        
    } 

    // Resign Game: stop any active animation and reset the board display.
    public void Command_Resign()
    {
        StopMoveAnimation();
        UnHighlightSquares();
        UpdateBoardDisplay();
    }

    public void Command_Resume(object? s, EventArgs e) => SetCmdStatus(null, null, null, false, false);
    public void Command_Undo(object? sender, EventArgs e, bool undoAll) => RestoreGame();
    public void Command_Redo(object? sender, EventArgs e, bool redoAll) => RestoreGame();
    #endregion

    #region UI Callback Methods: Dialogs
    /********************************************.
   |   [UI Callback Methods] Dialogs / Windows    |
    \.__________________________________________*/

    // "Options" must run async, so we avoid blocking via a "fire & forget" async kickoff.
    public void Command_Options(object? s, EventArgs e) => ShowOptionsDlg().ContinueWith(r =>
    {
        if (r.IsCompletedSuccessfully && r.Result != null)
            parent.UiLaunchAndWait(() => ProcessOptions((OptionsDialog.MiniOptions)r.Result));
    });

    public async Task<OptionsDialog.MiniOptions?> ShowOptionsDlg()
    {
        // Prepare an options set for the user to work with.
        var (flags, colors) = MainWindow.LoadBoardOptions(manager.Config);
        var (types, diffs) = Manager.LoadAiSettings(manager.Config);

        OptionsDialog.MiniOptions? options = new()
        {
            flags = flags,
            colors = colors,
            types = types,
            levels = diffs
        };

        // Show the options dialog. If the "OK" button was pressed, update everything accordingly.
        return await new OptionsDialog(options).ShowDialog<OptionsDialog.MiniOptions?>(this);
    }
    
    private void ProcessOptions(OptionsDialog.MiniOptions opts)
    {
        // Update settings to start; we'll clean up after that.
        ShowValidMoves = opts.flags[0];
        PreviewMoves = opts.flags[1];
        AnimateMoves = opts.flags[2];
        SquareControl.SetColors(opts.colors[0], opts.colors[1], opts.colors[2], opts.colors[3]);
        manager.Agents[0].Variant = opts.types[0];
        manager.Agents[1].Variant = opts.types[1];
        manager.Agents[0].Level = opts.levels[0];
        manager.Agents[1].Level = opts.levels[1];
        Manager.StoreAiSettings(manager.Config, opts.types, opts.levels);
        MainWindow.StoreBoardOptions(manager.Config, opts.flags, opts.colors);

        // If game is currently in progress, take extra care handling player options.
        if (manager.Phase != Manager.AppPhase.Inactive)
        {
            // Lock the board to prevent race conditions. If an animation is playing. stop it.
            lock (manager.Game)
            {
                if (manager.Phase == Manager.AppPhase.Moving)
                {
                    StopMoveAnimation();
                    UpdateBoardDisplay();
                    parent.PostMoveEnd();
                }
                UnHighlightSquares();
            }

            // If agent(s) have changed, update them & restart turn. Otherwise, highlight moves.
            Agent[] agents = manager.Agents;
            lock (agents)
            {
                if (opts.types[0] != agents[0].Variant || opts.levels[0] != agents[0].Level ||
                    opts.types[1] != agents[1].Variant || opts.levels[1] != manager.Agents[1].Level)
                    parent.RequestRestartAgents(opts);

                else if (ShowValidMoves && !manager.IsAIPlayer(manager.Game.Current))
                    HighlightValidMoves();
            }
        }
        SquaresPanel.InvalidateVisual(); // Update the board display in all cases.    
    }        

    // Recreate and show the statistics dialog.
    public void Command_Stats(object? sender, EventArgs e) =>
        new StatisticsDialog(manager.GameStats).ShowDialog(this);

    // Help Topics
    public void Command_Help(object? sender, EventArgs e)
    {
        // If the help file exists, show it. Otherwise, display an error message.

        if (new FileInfo(HELP_FILE).Exists)
            Uwu.Gui.Helpers.OpenHelpFile(HELP_FILE);
        else
            Uwu.Gui.MsgBox.Alert("File Not Found", $"The help file {HELP_FILE} could not be found.");
    }

    // About
    public void Command_About(object? sender, EventArgs e) => new AboutBox().ShowDialog(this);    
    #endregion

    #region Animation Timer
    // Updates animation of the move.
    private void Update_Animation(object? sender, EventArgs e)
    {
        // Lock the board to prevent race conditions.
        lock (manager.Game)
        {
            // If a move is being animated, advance the animation counters on the square controls.
            if (manager.Phase == Manager.AppPhase.Moving)
            {
                bool isComplete = true;

                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        isComplete = squareControls[i, j].AdvanceAnimation() ?? isComplete;

                // Refresh the display.
                SquaresPanel.InvalidateVisual();

                // If the animation is complete, end the move.
                if (isComplete)
                {
                    StopMoveAnimation();
                    UpdateBoardDisplay();
                    parent.PostMoveEnd();
                }
            }
        }
    }
    #endregion
}
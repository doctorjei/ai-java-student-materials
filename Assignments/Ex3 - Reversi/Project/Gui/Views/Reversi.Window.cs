using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.ObjectModel;
using Uwu.Games.Reversi.Engine;

namespace Uwu.Games.Reversi;
using Configuration = Uwu.Data.Configuration;
using StandardCursorType = Avalonia.Input.StandardCursorType;

/// <summary>/// It's Reversi.</summary>
public partial class MainWindow : Window
{
    /****************************************
        | Hypothetically modifiable constants. |
        ****************************************/

    // Data files & settings
    public const string HELP_FILE = "Reversi.chm";

    // Drawing constants
    public const int FRAME_LENGTH_MS = 16; // Approx. 60 FPS
    public static readonly Pen GridPen = new(new SolidColorBrush(Color.Parse("#FF204020")), 1);
    public static readonly SolidColorBrush HEADER_BG = new(Color.Parse("#FFD4D0C8"));
    public static readonly SolidColorBrush HEADER_FG = new(Color.Parse("#FF333333"));
    public static readonly SolidColorBrush DARK_BRUSH = new(Color.Parse("#FF166D2D"));
    public static readonly SolidColorBrush LIGHT_BRUSH = new(Color.Parse("#FF1F8F3E"));
    public static readonly SolidColorBrush BLACK_BRUSH = new(Colors.Black);
    public static readonly SolidColorBrush WHITE_BRUSH = new(Colors.White);

    // Game state, move data, board display.
    public Label cornerLabel = null;
    public Label[] rowLabels = new Label[Engine.Manager.GRID_SIZE];
    public Label[] colLabels = new Label[Engine.Manager.GRID_SIZE];
    public SquareControl[,] squareControls =
        new SquareControl[Engine.Manager.GRID_SIZE, Engine.Manager.GRID_SIZE];

    // Structural / component references.
    private Application parent;
    private Manager manager;
    private readonly ObservableCollection<Manager.MoveRecord> moveRows = [];

    // For Input-related structures & data.
    private int keyedRowNumber; // Stores row for moves via keyboard
    private int keyedColNumber; // Stores column for moves via keyboard
    private SquareControl? pressed = null; // Stores square where pointer was pressed.
    private readonly Avalonia.Input.Cursor handCursor = new(StandardCursorType.Hand);
    private readonly Avalonia.Input.Cursor defaultCursor = new(StandardCursorType.Arrow);

    // Board visualization settings.
    private bool ShowValidMoves { get; set; } = true;
    private bool PreviewMoves { get; set; } = false;
    private bool AnimateMoves { get; set; } = true;

    // Asynchronous elements (timers / threads)
    private readonly Avalonia.Threading.DispatcherTimer animationTimer = new();
    private static readonly TimeSpan timerInterval = TimeSpan.FromMilliseconds(FRAME_LENGTH_MS);                                     
    private bool closing = false;

    public MainWindow(Manager _manager, Application _parent)
    {
        manager = _manager;
        parent = _parent;

        // Initialize window & component.
        this.AttachDevTools();
        InitializeComponent();
        ConnectUiComponents();

        // Load & apply GUI settings.
        LoadGuiOptions();
        InitializeBoardDisplay();

        // Initialize animation timer.
        animationTimer.Interval = timerInterval;
        animationTimer.Tick += new EventHandler(Update_Animation);
    }

    private void InitializeBoardDisplay()
    {
        // Populate the row and column definitions for the squaresPanel gameplay area.
        for (int index = 0; index < Engine.Manager.GRID_SIZE; index++)
        {
            SquaresPanel.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            SquaresPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }
        SquaresPanel.Children.Clear(); // Clear out any existing children, just in case.

        // Label settings (Along top and left sides), starting the the corner label.
        cornerLabel = this.FindControl<Label>("cornerLabel") ?? new();
        cornerLabel.Content = "";
        Grid.SetRow(cornerLabel, 0);
        Grid.SetColumn(cornerLabel, 0);
        cornerLabel.Classes.Add("HeaderLabel");
        SquaresPanel.Children.Add(cornerLabel);

        // For each label, assign its content, set its location, and add to the panel.
        for (int i = 0; i < Engine.Manager.GRID_SIZE; i++)
        {
            // Rows: "1..N" in column 0, and...
            rowLabels[i] = this.FindControl<Label>($"rowLabel{i}") ?? new();
            rowLabels[i].Content = Engine.Manager.Rows[i];
            Grid.SetRow(rowLabels[i], i + 1);
            Grid.SetColumn(rowLabels[i], 0);
            rowLabels[i].Classes.Add("HeaderLabel");
            SquaresPanel.Children.Add(rowLabels[i]);

            // Columns: - "alpha[0]..[N-1]" in row 0 (A, B, C, ...)
            colLabels[i] = this.FindControl<Label>($"colLabel{i}") ?? new();
            colLabels[i].Content = Engine.Manager.Cols[i];
            Grid.SetRow(colLabels[i], 0);
            Grid.SetColumn(colLabels[i], i + 1);
            colLabels[i].Classes.Add("HeaderLabel");
            SquaresPanel.Children.Add(colLabels[i]);
        }

        // Assign all of the cells.
        for (int _r = 0; _r < Engine.Manager.GRID_SIZE; _r++)
        {
            for (int _c = 0; _c < Engine.Manager.GRID_SIZE; _c++)
            {
                // Create the controls, add the callbacks (for clicks), then add them to grid.
                var cell = new SquareControl { Margin = new Thickness(0), Row = _r, Col = _c };
                cell.PointerPressed += Square_OnPointerPressed;
                cell.PointerReleased += Square_OnPointerReleased;
                cell.PointerMoved += Square_OnPointerMoved;
                cell.PointerExited += Square_OnPointerExited;

                Grid.SetRow(cell, _r + 1);
                Grid.SetColumn(cell, _c + 1);
                squareControls[_r, _c] = cell;
                SquaresPanel.Children.Add(cell);
            }
        }
    }

    //============================
    // Primary game flow methods.
    //============================

    // Rollback an ended game (so it can be replayed / explored).
    public void Trigger_RollBack()
    {
        // Fix the color information and buttons; everything else should be good to go.
        ColorLabel.Text = "Current: ";
        ColorPanel.IsVisible = true;
        SetCmdStatus(false, true, null, null, null);
    }

    public void Trigger_StartTurn()
    {
        if (manager!.Phase == Engine.Manager.AppPhase.Inactive)
            return;

        // Set the player text for the status display.
        String playerText = Engine.Manager.ColorString(manager.Game.Current);
        playerText = String.Format("{0}'s", playerText);

        if ((manager!.Agents[0].Variant != Engine.Agent.Type.Human &&
            manager!.Agents[1].Variant == Engine.Agent.Type.Human) ||
            (manager!.Agents[1].Variant != Engine.Agent.Type.Human &&
            manager!.Agents[0].Variant == Engine.Agent.Type.Human))
            playerText = manager!.IsAIPlayer(manager.Game.Current) ? "My" : "Your";

        // Update the turn display panel.
        if (manager.Game.Current == Player.Black)
            ColorPanel.Background = BLACK_BRUSH;
        else
            ColorPanel.Background = WHITE_BRUSH;

        ColorPanel.InvalidateVisual();

        // If it's a computer player's turn, set up the worker thread run the AI.
        if (manager.Phase == Engine.Manager.AppPhase.AiWait)
        {
            // Check if computer play is currently suspended.
            if (manager.IsAIPlaySuspended)
            {
                // Enable "Resume Play" menu item & tool bar button so user can resume game.
                SetCmdStatus(null, null, null, true, null);

                // Update the status display to reflect AI pause.
                string shortCutText = ResumeItem.HotKey!.ToString() ?? "[OOPS]";
                StatusLabel.Text = String.Format("{0} move... (Suspended, press {1} to resume play.)", playerText, shortCutText);
                StatusBar.IsVisible = false;
            }
            else
            {
                // Set the status display.
                StatusLabel.Text = String.Format("{0} move, thinking... ", playerText);

                // Set up progress bar; put it to the right of the test.
                StatusBar.Value = 0;
                StatusBar.Minimum = 0;
                StatusBar.Maximum = manager.Game.GetValidMoveCount(manager.Game.Current);
                //statusLabel.Left + statusLabel.Width; // TODO: Check it this looks right.
                StatusBar.IsVisible = true;
            }
        }
        // Otherwise, set up for a user move.
        else
        {
            // Reset the keyed column and row numbers.
            keyedColNumber = -1;
            keyedRowNumber = -1;

            // Show valid moves, if that option is active.
            if (ShowValidMoves)
            {
                HighlightValidMoves();
                SquaresPanel.InvalidateVisual();
            }

            // Update the status display.
            StatusLabel.Text = String.Format("{0} move...", playerText);
            StatusBar.IsVisible = false;

            // Set focus on the form so it will receive key presses.
            Focus();
        }

        // Update the status display.
        StatusPanel.InvalidateVisual();
        PlayToolBar.InvalidateVisual();
    }

    // Makes a move for the current player.
    public void Trigger_MakeMove(Move move)
    {
        // Make a copy of the old board for comparison post-animation.
        State oldBoard = new(manager.MoveHistory[^1].Game);

        // Post the move to the game state; then, update the move list.
        moveRows.Add(manager.MoveHistory[^1]);
        MoveView.InvalidateVisual();
        MoveView.ScrollIntoView(manager.MoveHistory[^1], null);

        // Enable/disable move-related menu items & tool bar buttons as appropriate.
        SetCmdStatus(null, null, true, null, false);

        // Update the status display and game display.
        StatusLabel.Text = "";
        StatusBar.IsVisible = false;
        StatusPanel.InvalidateVisual();
        UnHighlightSquares();

        // If the animate move option is active, set up animation for the affected discs.
        if (AnimateMoves)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // Mark the newly added disc.
                    if (i == move.Row && j == move.Column)
                        squareControls[i, j].IsNew = true;
                    // Initialize animation for the discs that were flipped.
                    else if (manager.Game[i, j] != oldBoard[i, j])
                        squareControls[i, j].AnimationCounter = SquareControl.AnimationStart;
                }
            }
        }

        // Update the display to reflect the board changes.
        this.UpdateBoardDisplay();

        // If the animate moves option is active, start the animation.
        if (AnimateMoves)
        {
            manager.Phase = Engine.Manager.AppPhase.Moving; // TODO: should be turned on my manager regardless (to protect board edit), but then turned off if not useful here.
            animationTimer.Start();
        }

        // Otherwise, end the move.
        else
            parent.PostMoveEnd(); 
    }

    // Called when move is completed (including animation) to start next turn.
    public void Trigger_EndMove() { }

    // Ends the current game, optionally by player resignation.
    public void Trigger_EndGame(bool isResignation)
    {
        // Stop the game timer.
        animationTimer.Stop();

        // Enable/disable the menu items and tool bar buttons as appropriate.
        SetCmdStatus(true, false, false, false, false);

        // Clear the current player indicator display.
        ColorLabel.Text = "";
        ColorPanel.Background = null;
        ColorPanel.IsVisible = false;

        // Hide the status progress bar.
        StatusBar.IsVisible = false;
        StatusPanel.InvalidateVisual();

        // For a computer vs. user game, determine who played what color.
        bool isBlackComputer = manager.IsAIPlayer(Player.Black);
        bool isWhiteComputer = manager.IsAIPlayer(Player.White);

        // Handle a resignation.            
        if (isResignation)
        {
            Player quitter = Player.Empty;

            if (isBlackComputer && !isWhiteComputer)
                quitter = Player.White;
            else if (isWhiteComputer && !isBlackComputer)
                quitter = Player.Black;
            else if (!isBlackComputer && !isWhiteComputer)
                quitter = manager.Game.Current;

            if (quitter == Player.Empty)
                StatusLabel.Text = "Game Aborted.";
            else
                StatusLabel.Text = Engine.Manager.ColorString(quitter) + " resigns.";
        }
        // Handle an end game; update the status message accordingly.
        else
        {
            if (manager.Game.BlackCount > manager.Game.WhiteCount)
                StatusLabel.Text = "Black wins.";
            else if (manager.Game.WhiteCount > manager.Game.BlackCount)
                StatusLabel.Text = "White wins.";
            else
                StatusLabel.Text = "Draw.";
        }

        // Re-enable the undo move-related menu items and tool bar buttons.
        SetCmdStatus(null, null, manager.MoveNo > 1, false,
                        manager.MoveNo < manager.MoveHistory.Count);

        // Update the status display.
        StatusPanel.InvalidateVisual();
    }

    /**********************************.
   |   Board-Specific Display Updates   |
    \.________________________________*/

    // Stops animation of a move and resets the squares.
    private void StopMoveAnimation()
    {
        // Stop the animation timer.
        animationTimer.Stop();

        // Reset the animation counters and new disc flag on all squares.
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                squareControls[i, j].StopAnimation();
    }

    // Updates the display to reflect the current game board.
    private void UpdateBoardDisplay()
    {
        // Map the current game board to the square controls.
        for (int i = 0; i < Engine.Manager.GRID_SIZE; i++)
            for (int j = 0; j < Engine.Manager.GRID_SIZE; j++)
                squareControls[i, j].ResetContents(manager.Game[i, j]);

        // Set counts and redraw the board.
        BlackCountLabel.Text = manager.Game.BlackCount.ToString();
        WhiteCountLabel.Text = manager.Game.WhiteCount.ToString();
        BlackCountLabel.InvalidateVisual();
        WhiteCountLabel.InvalidateVisual();
        SquaresPanel.InvalidateVisual();
    }

    // Highlights the board squares that represent valid moves for the current player.
    private void HighlightValidMoves()
    {
        for (int i = 0; i < Engine.Manager.GRID_SIZE; i++)
            for (int j = 0; j < Engine.Manager.GRID_SIZE; j++)
                squareControls[i, j].IsValid =
                    manager.Game.IsValidMove(manager.Game.Current, State.NewMove(i, j));
    }

    // Removes any highlighting from all the board squares.
    private void UnHighlightSquares()
    {
        // Clear the flags on each square.
        for (int i = 0; i < Engine.Manager.GRID_SIZE; i++)
            for (int j = 0; j < Engine.Manager.GRID_SIZE; j++)
                squareControls[i, j].ClearFlags();
    }


    /*******************************.
   |   General GUI Display Updates   |
    \._____________________________*/

    private void RestoreGame()
    {
        // Stop animations, clear square highlighting, restore board, update display, and reset turn.
        StopMoveAnimation();
        UnHighlightSquares();
        UpdateBoardDisplay();

        // Restore the move list.
        moveRows.Clear();
        for (int i = 0; i < manager.MoveNo - 1; i++)
            moveRows.Add(manager.MoveHistory[i]);

        if (moveRows.Count > 0)
            MoveView.ScrollIntoView(manager.MoveHistory[^1], null);
        else
            MoveView.InvalidateVisual();

        // Enable/disable the move-related menu items and tool bar buttons as appropriate.
        SetCmdStatus(null, null,
            manager.MoveNo > 1, false, manager.MoveNo < manager.MoveHistory.Count);
    }

    private void SetCmdStatus(bool? newGame, bool? resign, bool? undo, bool? resume, bool? redo)
    {
        NewGameItem.IsEnabled = NewGamePanel.IsEnabled = newGame ?? NewGameItem.IsEnabled;
        ResignItem.IsEnabled = ResignPanel.IsEnabled = resign ?? ResignItem.IsEnabled;
        ResumeItem.IsEnabled = ResumePanel.IsEnabled = resume ?? ResumeItem.IsEnabled;

        UndoItem.IsEnabled = UndoPanel.IsEnabled = undo ?? UndoItem.IsEnabled;
        UndoAllItem.IsEnabled = UndoAllPanel.IsEnabled = undo ?? UndoAllItem.IsEnabled;
        RedoItem.IsEnabled = RedoPanel.IsEnabled = redo ?? RedoItem.IsEnabled;
        RedoAllItem.IsEnabled = RedoAllPanel.IsEnabled = redo ?? RedoAllItem.IsEnabled;
    }

    // Updates the status progress bar. Note: Called from the worker thread.
    private void UpdateStatusProgress() // TODO: Never called??? Maybe we cut this?...
    {
        // Increase the progress bar value by one.
        if (StatusBar.Value < StatusBar.Maximum)
        {
            StatusBar.Value++;
            StatusBar.InvalidateVisual();
        }
    }

    /**********************************************
     | Loading & Saving of Gui Settings / Options |
     **********************************************/
    public static Rect LoadWindowOptions(Configuration settings, Rect? _bounds = null)
    {
        // See if the location was default.
        Rect bounds = _bounds ?? new Rect(100, 100, 576, 480);

        // Load saved GUI game flags and GUI options.
        return new(settings.Get("Window", "Left", bounds.Position.X),
            settings.Get("Window", "Top", bounds.Position.Y),
            settings.Get("Window", "Width", bounds.Size.Width),
            settings.Get("Window", "Height", bounds.Size.Height));
    }

    public static (bool[], Color[]) LoadBoardOptions(Configuration settings,
        bool show = true, bool preview = false, bool animate = true,
        Color? normal = null, Color? valid = null, Color? active = null, Color? move = null)
    {
        // Prep default values if they came in empty.
        normal ??= SquareControl.NormalColorDefault;
        valid ??= SquareControl.ValidColorDefault;
        active ??= SquareControl.ActiveColorDefault;
        move ??= SquareControl.MoveColorDefault;

        return ([settings.Get("Options", "ShowValidMoves", show),
            settings.Get("Options", "PreviewMoves", preview),
            settings.Get("Options", "AnimateMoves", animate)],
           [settings.Get("Options", "BoardColor", (Color)normal),
            settings.Get("Options", "ValidMoveColor", (Color)valid),
            settings.Get("Options", "ActiveSquareColor", (Color)active),
            settings.Get("Options", "MoveIndicatorColor", (Color)move)]);
    }

    public static void StoreWindowOptions(Configuration settings, Rect data)
    {
        // Save window settings.
        settings.Set("Window", "Left", Convert.ToInt32(data.Position.X));
        settings.Set("Window", "Top", Convert.ToInt32(data.Position.Y));
        settings.Set("Window", "Width", Convert.ToInt32(data.Width));
        settings.Set("Window", "Height", Convert.ToInt32(data.Height));
        settings.Save();
    }

    public static void StoreBoardOptions(Configuration settings, bool[] flags, Color[] colors)
    {
        // Save visualization options.
        settings.Set("Options", "ShowValidMoves", flags[0]);
        settings.Set("Options", "PreviewMoves", flags[1]);
        settings.Set("Options", "AnimateMoves", flags[2]);
        settings.Set("Options", "BoardColor", colors[0]);
        settings.Set("Options", "ValidMoveColor", colors[1]);
        settings.Set("Options", "ActiveSquareColor", colors[2]);
        settings.Set("Options", "MoveIndicatorColor", colors[3]);
        settings.Save();
    }

    private void LoadWindowOptions() => Bounds = LoadWindowOptions(manager.Config, Bounds);

    private void LoadBoardOptions()
    {

        var (flags, colors) = LoadBoardOptions(manager.Config, ShowValidMoves, PreviewMoves,
            AnimateMoves, SquareControl.NormalColor, SquareControl.ValidColor,
            SquareControl.ActiveColor, SquareControl.MoveColor);

        (ShowValidMoves, PreviewMoves, AnimateMoves) = (flags[0], flags[1], flags[2]);
        SquareControl.SetColors(colors[0], colors[1], colors[2], colors[3]);
    }

    // Loads any saved program settings.
    private void LoadGuiOptions()
    {
        LoadWindowOptions();
        LoadBoardOptions();
    }

    private void StoreWindowOptions() =>
        StoreWindowOptions(manager.Config, new(Bounds.Position, Bounds.Size));

    private void StoreBoardOptions() => StoreBoardOptions(manager.Config,
        [ShowValidMoves, PreviewMoves, AnimateMoves], [SquareControl.NormalColor,
        SquareControl.ValidColor, SquareControl.ActiveColor, SquareControl.MoveColor]);

    // Loads any saved program settings.
    private void StoreGuiOptions()
    {
        StoreWindowOptions();
        StoreBoardOptions();
    }
}
public static class AppConfigExtensions
{
    public static void Set(this Configuration cfg, string sec, string name,
        Color color) => cfg.Set(sec, name, color, x => $"#{x.ToUInt32():X8}");
}

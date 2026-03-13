namespace Uwu.Games.Reversi;

public interface Driver
{
    // REQUIRED: Mechanism for subsystems to begin moves & indicate completion (animations, etc).
    public void PostMoveBegin(Move move);
    public void PostMoveEnd();

    // REQUIRED: Commands; broadcast to subsystems by user interaction / input.
    public void Command_NewGame(object? s, EventArgs e, Action<object?, EventArgs>? c);
    public void Command_Undo(object? s, EventArgs e, Action<object?, EventArgs, bool>? c, bool a);
    public void Command_Redo(object? s, EventArgs e, Action<object?, EventArgs, bool>? c, bool a);
    public void Command_Resume(object? s, EventArgs e, Action<object?, EventArgs>? c);
    public void Command_Options(object? s, EventArgs e, Action<object?, EventArgs>? c);
    public void Command_Stats(object? s, EventArgs e, Action<object?, EventArgs>? c);
    public void Command_Help(object? s, EventArgs e, Action<object?, EventArgs>? c);
    public void Command_About(object? s, EventArgs e, Action<object?, EventArgs>? c);

    // REQUIRED: Special cases; dangerous actions are "guarded" by confirmation prompts.
    public void Prompt_Resign(object? s, EventArgs e, Task<bool> conf);
    public void Prompt_Exit(object? s, EventArgs e, Task<bool> conf);
    public void Command_Resign();
    public void Command_Exit();

    // SUGGESTED: Triggers; broadcast to subsystems by events. (Not "official" to avoid public req.)
    // protected void Trigger_StartGame(bool rollback);
    // protected void Trigger_StartTurn();
    // protected void Trigger_MakeMove(ReversiMove move);
    // protected void Trigger_EndMove();
    // protected void Trigger_EndGame(bool resigned);
}
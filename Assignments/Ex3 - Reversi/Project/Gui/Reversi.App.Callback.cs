// comment this to turn off student output
#define ShowStudentOutput

using Avalonia.Threading;
using Uwu.Games.Reversi.Engine;
using Uwu.Threads;

namespace Uwu.Games.Reversi;

using Callback = Action<object?, EventArgs>;
using Callback_bool = Action<object?, EventArgs, bool>;
using AppPhase = Manager.AppPhase;

public partial class Application : Avalonia.Application, Driver
{
            /************************************************************************.
            |        Subsystems & Threading (See Primary File for Definitions)       |
            --------------------------------------------------------------------------
            |            private ReversiManager?     manager;                        |
            |            private ReversiForm?        mainForm;                       |
            |            private List<Thread>        workerThreads = [];             |
            `-----------------------------------------------------------------------*/

    object key1 = new(), key2 = new();

        /*********************************************************************************.
        |      Dispatcher Callback Routing - Always invoked by subsystems (GUI / manager)  |
        `--------------------------------------------------------------------------------*/
    // Allows subsystems (GUI / manager) to begin moves & indicate completion (animations, etc).
    public void PostMoveBegin(Move mv) => UiLaunchAndWait(() => Trigger_MakeMove(mv));
    public void PostMoveEnd() => UiLaunchAndWait(Trigger_EndMove);
    //private void UiFireAndForget(Action action) { lock (key1) { Dispatcher.UIThread.Post(action); } }

    // Often, we need the UI thread to perform an action and wait for it to complete.
    public void UiLaunchAndWait(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action.Invoke();
            return;
        }
        
        lock (key1)
        {
            ManualResetEvent flare = new(false);
            Dispatcher.UIThread.Post(() => UiCatchAndConfirm(action, flare));
            flare.WaitOne();
        }
    }

    public void UiCatchAndConfirm(Action action, ManualResetEvent flare)
    {
        lock (key2) { action.Invoke(); }
        flare.Set();
    }


        /*********************************************************************************.
        |  Command Callback Routing - Always invoked by user (sometimes routed together)  |
        `--------------------------------------------------------------------------------*/

    /***************. ,*********************.**************************.**********************.
    | [Cmd] NewGame | | [Via] Menu / Button | [Desc] Starts a New Game | [Triggers] StartTurn |
    `---------------' `---------------------'--------------------------'---------------------*/
    public void Command_NewGame(object? s, EventArgs e, Callback? a) => Run.Write(s, e,
        manager!.Command_NewGame, mainForm!.Command_NewGame, (s, e) => Trigger_StartTurn(), a);

    /**************. ,*********************.***************************.**********************.
    | [Cmd] Resume | | [Via] Menu / Button | [Desc] Restarts Paused AI | [Triggers] StartTurn |
    `--------------' `---------------------'---------------------------'---------------------*/
    public void Command_Resume(object? s, EventArgs e, Callback? c) => Run.Write(s, e,
        manager!.Command_Resume, mainForm!.Command_Resume, (_, _) => Trigger_StartTurn(), c);

    /***************. ,************.***********************.*******************************.
    | [Cmd] Options | | [Via] Menu | [Desc] Options Dialog | [Triggers] EndMove, StartTurn |
    `---------------' `------------'-----------------------'------------------------------*/
    // TODO: Add EndMove, StartTurn (probably opening and closing respectively); fix it.
    public void Command_Options(object? s, EventArgs e, Callback? c) =>
        Run.Write<object?, EventArgs>(s, e, manager!.Command_Options, mainForm!.Command_Options, c);

    /******************. ,************.**************************.*********************.
    | [Cmd] Statistics | | [Via] Menu | [Desc] Statistics Dialog | [Triggers] None?... |
    `------------------' `------------'--------------------------'--------------------*/
    // TODO: Statistics dialog (menu item)
    public void Command_Stats(object? s, EventArgs e, Callback? c) =>
        Run.Write(s, e, manager!.Command_Stats, mainForm!.Command_Stats, c);

    /***************. ,************.***********************.********************.
    |  [Cmd] About  | | [Via] Menu |  [Desc] About Dialog  |  [Triggers] None   |
    `---------------' `------------'-----------------------'-------------------*/
    public void Command_About(object? s, EventArgs e, Callback? c) =>
        Run.Write(s, e, manager!.Command_About, mainForm!.Command_About, c);

    /**************. ,************.**********************.*******************************.
    |  [Cmd] Help  | | [Via] Menu |  [Desc] Help Dialog  |  [Triggers] None?             |
    `--------------' `------------'----------------------'------------------------------*/
    // TODO: Help dialog (menu item)
    public void Command_Help(object? s, EventArgs e, Callback? c) =>
        Run.Write(s, e, manager!.Command_Help, mainForm!.Command_Help, c);

    /************. ,******************.****************************.*****************************.
    | [Cmd] Undo | | [Via] Menu / Btn | [Desc] Trace Back 1+ Moves | [Trig.] RollBack, StartTurn |
    `------------' `------------------'----------------------------'----------------------------*/
    public void Command_Undo(object? s, EventArgs e, Callback_bool? c, bool all)
    {
        Run.Write(s, e, all, manager!.Command_Undo, mainForm!.Command_Undo, c);
        if (manager!.Phase == AppPhase.Inactive)
            Trigger_RollBack();
        else
            Trigger_StartTurn();     // Active game; start turn normally.
    }

    /**************. ,*********************.*******************************.**********************.
    |  [Cmd] Redo  | | [Via] Menu / Button | [Desc] Trace Forward 1+ Moves | [Triggers] StartTurn |
    `--------------' `---------------------'-------------------------------'---------------------*/
    public void Command_Redo(object? s, EventArgs e, Callback_bool? c, bool all) => Run.Write(s, e,
        all, manager!.Command_Redo, mainForm!.Command_Redo, c, (e, s, all) => Trigger_StartTurn());

    /**************. ,****************.*************************************.*****************.
    | [Req] Resign | | [Via] Menu/Btn | [Desc] If systems req, show prompt. | [Triggers] None |
    `--------------' `----------------'-------------------------------------'----------------*/
    public void Prompt_Resign(object? s, EventArgs e, Task<bool> c) =>
        Run.Poll(c, async (r) => await Confirm(r, c, Command_Resign), (_) =>
            manager!.Prompt_Resign(s, e), (_) => mainForm!.Prompt_Resign(s, e));

    /**************. ,****************.*************************************.*****************.
    |  [Req] Exit  | | [Via] Menu/Btn | [Desc] If systems req, show prompt. | [Triggers] None |
    `--------------' `----------------'-------------------------------------'----------------*/
    public void Prompt_Exit(object? s, EventArgs e, Task<bool> c) => // TODO: Fix close error...
        Run.Poll(c, async (r) => await Confirm(r, c, Command_Exit), (_) =>
            manager!.Prompt_Exit(s, e), (_) => mainForm!.Prompt_Exit(s, e));

    /*************************. ,*****************************************************************.
    | [Commands] Resign, Exit | | Immediate response; terminate (game or app) ASAP; cleanup only. |
    `-------------------------' `----------------------------------------------------------------*/
    public void Command_Exit() => Run.RunAll(manager!.Command_Exit, mainForm!.Command_Exit);
    public void Command_Resign() => Run.RunAll(
        manager!.Command_Resign, mainForm!.Command_Resign, () => Trigger_EndGame(true));


            /****************************************************************.
           |   Trigger Callback Routing - Invoked by events and/or commands   |
            \.______________________________________________________________*/
        
    /*___________   _____________________________ ____________________________ ___________________
     |   [CMD]   | |       [INV.COMMANDS]        |   [INV. TRIGGERS]          |   [CASCADES TO]   |
     | StartTurn | | Undo, Redo, Resume, Options | StartGame,RollBack,EndMove | EndGame (if term) |
     `-----------' `-----------------------------'----------------------------'------------------*/
    protected void Trigger_StartTurn()
    {
        Run.RunAll(manager!.Trigger_StartTurn, mainForm!.Trigger_StartTurn, () =>
        {
            if (manager!.Phase == AppPhase.Inactive)
                Trigger_EndGame(false); // TODO: Is this right?...
        });
    }

    /*__________   _________________________ ______________________________________________________
     |  [CMD]   | |      [INVOKED BY]       |                     [CASCADES TO]                    |
     | MakeMove | |  =={ UI / AI Only }==   |  Update_Anim (timer), EndMove (direct, after anim.)  |
     `----------' `-------------------------'-----------------------------------------------------*/
    protected void Trigger_MakeMove(Move move) =>
        Run.Write(move, manager!.Trigger_MakeMove, mainForm!.Trigger_MakeMove);

    /*_________   ________________ _________________________________________ ______________________
     |  [CMD]  | | [INV.COMMANDS] |         [INV. TRIGGERS, MISC.]          |    [CASCADES TO]     |
     | EndMove | |     Options    |  MakeMove (direct); Animation (direct)  |  StartTurn (always)  |
     `---------' `----------------'-----------------------------------------'----------------------*/
    protected void Trigger_EndMove() => Run.RunAll(manager!.Trigger_EndMove,
                            mainForm!.Trigger_EndMove, () => Trigger_StartTurn());

    /*__________  ________________ ___________________ _________________
     |  [CMD]  | | [INV.COMMANDS] |  [INV. TRIGGERS]  |  [CASCADES TO]  |
     | EndGame | |     Resign     |      StartTurn    |     (None)      |
     `---------' `----------------'-------------------'----------------*/
    protected void Trigger_EndGame(bool resigned) => Run.Write(resigned,
                        manager!.Trigger_EndGame, mainForm!.Trigger_EndGame);

    /*____________   ______________________ __________________ _________________
     |    [CMD]   | |    [INV.COMMANDS]    |  [INV.TRIGGERS]  |  [CASCADES TO]  |
     |  RollBack  | |  Undo (if inactive)  |      (None)      |    StartTurn    |
     `------------' `----------------------'------------------'----------------*/
    protected void Trigger_RollBack() => Run.RunAll(manager!.Trigger_RollBack,
                        mainForm!.Trigger_RollBack, Trigger_StartTurn);

    public void RequestRestartAgents(OptionsDialog.MiniOptions opts)
    {
        if (manager != null)
        {
            manager.KillComputerMoveThread();
            manager.SetAgentBehaviors();
            Trigger_StartTurn();
        }
    }
    /*********************************************************************************************
      Generalized subsystem polling and user-confirmation prompting routine (for exit / resign).
    **********************************************************************************************/
    private static async Task Confirm(bool[] responses, Task<bool> confirm, Action command)
    {
        bool prompt = false;
        foreach (var item in responses)
            if (prompt = prompt || item)
                break;

        if (!prompt || await confirm)
            command();
    }
}

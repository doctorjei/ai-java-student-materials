#define ShowStudentOutput // comment this to turn off student output
using Avalonia;

namespace Uwu.Games.Reversi;
using StyleInclude = Avalonia.Markup.Xaml.Styling.StyleInclude;
using Lifetime = Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

public partial class Application() : Avalonia.Application, Driver
{
    /*******************************************.
    | Console Stream Setup (for student output) |
    `********************************************/
#if ShowStudentOutput
    static readonly Stream consoleStream = Console.OpenStandardOutput();
#else
    static readonly Stream consoleStream = System.IO.Stream.Null;
#endif

    /// <summary>Set up the debug console stream for use in the student behavior.
    private static void SetupConsoleStream() =>
        GamePlaying.Behavior<State, Player, Move, Player, int>
            .DebugConsole = new(consoleStream) { AutoFlush = true };

    /******************************************.
    | Application Initialization and Main Loop |
    `*******************************************/
    public static readonly string SETTINGS_FILE = "Reversi.xml";
    private Engine.Manager? manager;
    private MainWindow? mainForm;
    private List<Thread> workerThreads = [];

    public override void Initialize()
    {
        Styles.Add(new Avalonia.Themes.Simple.SimpleTheme());
        Styles.Add(new StyleInclude(new Uri("avares://Avalonia.Controls.DataGrid/"))
        { Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Simple.xaml") });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is Lifetime desktop)
        {
            manager = new(SETTINGS_FILE, this);
            desktop.MainWindow = mainForm = new MainWindow(manager, this);
        }
        base.OnFrameworkInitializationCompleted();
    }

    [STAThread]
    public static void Main(string[] args)
    {
        SetupConsoleStream();
        var builder = AppBuilder.Configure<Application>().UsePlatformDetect();
        builder.WithInterFont().LogToTrace().StartWithClassicDesktopLifetime(args);
    }
}

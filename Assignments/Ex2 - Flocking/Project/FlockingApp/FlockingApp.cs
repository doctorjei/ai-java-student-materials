using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;

using Uwu.Simulation.Steering.Flocking.Views;

namespace Uwu.Simulation.Steering.Flocking
{
    public class FlockingApp : Application
    {
        public override void Initialize() =>
            Styles.Add(new Avalonia.Themes.Simple.SimpleTheme()); // FluentTheme also OK perhaps.

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();

            base.OnFrameworkInitializationCompleted();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var builder = AppBuilder.Configure<FlockingApp>().UsePlatformDetect();
            builder.WithInterFont().LogToTrace().StartWithClassicDesktopLifetime(args);
        }
    }
}

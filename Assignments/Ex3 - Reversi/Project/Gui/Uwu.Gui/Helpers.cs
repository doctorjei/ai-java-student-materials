using System.Diagnostics;

namespace Uwu.Gui;

public static class Helpers
{
    public static void OpenHelpFile(string helpFilePath)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = helpFilePath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            // optional fallback or logging
            Debug.WriteLine($"Failed to open help file: {ex.Message}");
        }
    }
}

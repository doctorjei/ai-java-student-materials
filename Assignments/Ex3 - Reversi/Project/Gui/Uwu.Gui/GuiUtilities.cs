using Avalonia.Media.Imaging;
using CustomMessageBox.Avalonia;

using BtnRole = CustomMessageBox.Avalonia.SpecialButtonRole;
using Icon = CustomMessageBox.Avalonia.MessageBoxIcon;

namespace Uwu.Gui;

public static class MsgBox
{
    /*****************************************
     | MessageBox-specific helper functions. |
     *****************************************/

    // Convert log level into icon enumeration.
    public static Icon LogToIcon(int lvl) => lvl switch
    { >= 0 and <= 3 => Icon.Error, 4 => Icon.Warning, 5 or 6 => Icon.Information, _ => Icon.Question };

    // Simple function to convert button index into a bool value (max 3 buttons).
    public static bool? BtnBool(int btn, int def) =>
        (btn <= 0 ? def : btn) switch { <= 1 => true, 2 => false, _ => null };

    /***********************************************
     | Common MessageBox dialog routines (various) |
     ***********************************************/

    // Alert box; takes in alert message, title, alert level (icon), button text.
    public static void Alert(MessageBox box, string bt = "OK") => AnyButtons(box, 0, bt);

    // Icon enumeration variant.
    public static void Alert(string m, string t, Icon icn = Icon.Information, string bt = "OK") =>
        Alert(new MessageBox(m, t, icn), bt);

    // Bitmap icon variant.
    public static void Alert(string message, string title, Bitmap icon, string buttonText = "OK") =>
        Alert(new MessageBox(message, title, icon), buttonText);

    // Alert box, but using unix log levels for convenience.
    public static void Alert(string message, string title, int lvl, string bt = "OK") =>
        Alert(new MessageBox(message, title, LogToIcon(lvl)), bt);

    // Icon enumeration variant; no MessageBox object needed.
    public static async Task<bool> YesNo(string message, string title, int def = -1,
      Icon icon = Icon.Question, string yes = "Yes", string no = "No") =>
        await YesNo(new MessageBox(message, title, icon), def, yes, no);

    // Bitmap variant; no MessageBox object needed.
    public static async Task<bool> YesNo(string message, string title, int def,
      Bitmap? icon, string yes = "Yes", string no = "No") =>
        await YesNo(new MessageBox(message, title, icon), def, yes, no);

    // Simple yes/no dialog box / alert.
    public static async Task<bool> YesNo(MessageBox box, int def = -1, string y = "Yes", string n = "No") =>
        BtnBool(await AnyButtons(box, def, [y, n]), def) ?? default;

    // Yes, No, Cancel (3B). Return values - "Yes": true; "No": false; "Cancel": null.
    public static async Task<bool?> YesNoCancel(MessageBox box,
      int def = -1, string y = "Yes", string n = "No", string c = "Cancel") =>
        BtnBool(await AnyButtons(box, def, [y, n, c]), def);

    // Icon enumeration variant; no MessageBox object needed.
    public static Task<bool?> YesNoCancel(string msg, string title, int def = -1,
      Icon icn = Icon.Question, string y = "Yes", string n = "No", string c = "Cancel") =>
        YesNoCancel(new MessageBox(msg, title, icn), def, y, n, c);

    // Bitmap variant; no MessageBox object needed.
    public static Task<bool?> YesNoCancel(string message, string title, int def,
      Bitmap? icon, string y = "Yes", string n = "No", string c = "Cancel") =>
        YesNoCancel(new MessageBox(message, title, icon), def, y, n, c);

    // Parent button routine (for us); handles any number of buttons, icon enum or bitmap, defaults.
    public static Task<int> AnyButtons(MessageBox box, int def = -1, params string[] btn)
    {
        if (btn.Length == 0)
            btn = ["OK"];

        def = def < 0 ? 0 : def > btn.Length - 1 ? btn.Length - 1 : def;
        List<MessageBoxButton<int>> buttons = [];

        for (int idx = 0; idx < btn.Length; idx++)
            buttons.Add(new(btn[idx], idx + 1, idx + 1 == def ? BtnRole.IsDefault : BtnRole.None));

        return box.Show(buttons.ToArray());
    }

    // Bitmap variant; no MessageBox object needed.
    public static Task<int> AnyButtons(string message,
      string title, Bitmap icon, int def = -1, params string[] btn) =>
        AnyButtons(new MessageBox(title, message, icon), def, btn);

    // Icon enumeration variant; no MessageBox object needed.
    public static Task<int> AnyButtons(string message, string title,
      Icon icon = Icon.Question, int def = -1, params string[] btn) =>
        AnyButtons(new MessageBox(title, message, icon), def, btn);
}
using Avalonia.Controls;
using Avalonia.Media;

namespace Uwu.Games.Reversi;
using RangeBaseValueChangedEventArgs = Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

using Agent = Engine.Agent;
using Type = Engine.Agent.Type;
using AiLevel = Engine.Agent.AiLevel;

public partial class AboutBox : Window
{
    public AboutBox()
    {
        InitializeComponent();
        okButton.Click += (_, __) => Close(true);
    }
}

public partial class StatisticsDialog : Window
{
    private readonly Data.Statistics _statistics;

    public StatisticsDialog(Data.Statistics statistics)
    {
        _statistics = statistics;
        InitializeComponent();
        MapStatisticsToControls();
    }

    /// <summary>Sets the form controls based on the current game statistics.</summary>
    public void MapStatisticsToControls()
    {
        // Get and display the game statistics
        if (BlackWinsLabel != null)
            BlackWinsLabel.Text = _statistics.BlackWins.ToString();
        
        if (WhiteWinsLabel != null)
            WhiteWinsLabel.Text = _statistics.WhiteWins.ToString();
        
        if (OverallDrawsLabel != null)
            OverallDrawsLabel.Text = _statistics.OverallDraws.ToString();
        
        if (BlackTotalScoreLabel != null)
            BlackTotalScoreLabel.Text = _statistics.BlackTotalScore.ToString();
        
        if (WhiteTotalScoreLabel != null)
            WhiteTotalScoreLabel.Text = _statistics.WhiteTotalScore.ToString();
        
        if (ComputerWinsLabel != null)
            ComputerWinsLabel.Text = _statistics.ComputerWins.ToString();
        
        if (UserWinsLabel != null)
            UserWinsLabel.Text = _statistics.UserWins.ToString();
        
        if (VsComputerDrawsLabel != null)
            VsComputerDrawsLabel.Text = _statistics.VsComputerDraws.ToString();
        
        if (ComputerTotalScoreLabel != null)
            ComputerTotalScoreLabel.Text = _statistics.ComputerTotalScore.ToString();
        
        if (UserTotalScoreLabel != null)
            UserTotalScoreLabel.Text = _statistics.UserTotalScore.ToString();
    }

    /// <summary>Resets the game statistics when the "Reset" button is clicked.</summary>
    private async void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        // Prompt for confirmation        
        if (await Uwu.Gui.MsgBox.YesNo("Reset statistics?", "Confirm Reset", 2))
        {
            _statistics.Reset();
            MapStatisticsToControls();
        }
    }

    /// <summary>Closes the dialog when the "Close" button is clicked.</summary>
    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();
}

public partial class OptionsDialog : Window
{
    public struct MiniOptions
    {
        public bool[] flags;
        public Color[] colors;
        public Type[] types;
        public AiLevel[] levels;
    }

    MiniOptions opts;

    public OptionsDialog(MiniOptions? _opts)
    {
        opts = _opts ?? new MiniOptions()
        {
            flags = [true, false, true],
            colors = [SquareControl.NormalColorDefault, SquareControl.ValidColorDefault,
                      SquareControl.ActiveColorDefault, SquareControl.MoveColorDefault],
            types = [Agent.P1_TYPE, Agent.P2_TYPE],
            levels = [Agent.P1_DIFFICULTY, Agent.P2_DIFFICULTY]
        };
        
        InitializeComponent();
        MapConfigToControls();
    }

    ///<summary>Sets the form controls based on the current game options.</summary>
    private void MapConfigToControls()
    {
        if (ShowValid != null)
            ShowValid.IsChecked = opts.flags[0];
        if (Preview != null)
            Preview.IsChecked = opts.flags[1];
        if (Animate != null)
            Animate.IsChecked = opts.flags[2];

        if (BoardColorPanel != null)
            BoardColorPanel.Background = new SolidColorBrush(opts.colors[0]);
        if (ValidColorPanel != null)
            ValidColorPanel.Background = new SolidColorBrush(opts.colors[1]);
        if (ActiveColorPanel != null)
            ActiveColorPanel.Background = new SolidColorBrush(opts.colors[2]);
        if (MoveColorPanel != null)
            MoveColorPanel.Background = new SolidColorBrush(opts.colors[3]);

        if (P1_StudentAiAgent != null)
            P1_StudentAiAgent.IsChecked = opts.types[0] == Type.StudentAI;
        if (P1_HumanAgent != null)
            P1_HumanAgent.IsChecked = opts.types[0] == Type.Human;
        if (P1_ExampleAiAgent != null)
            P1_ExampleAiAgent.IsChecked = opts.types[0] == Type.ExampleAI;

        if (P2_StudentAiAgent != null)
            P2_StudentAiAgent.IsChecked = opts.types[1] == Type.StudentAI;
        if (P2_HumanAgent != null)
            P2_HumanAgent.IsChecked = opts.types[1] == Type.Human;
        if (P2_ExampleAiAgent != null)
            P2_ExampleAiAgent.IsChecked = opts.types[1] == Type.ExampleAI;

        // Set difficulty sliders
        SetLevelSlider(P1_LevelSlider, P1_LevelLabel, opts.levels[0]);
        SetLevelSlider(P2_LevelSlider, P2_LevelLabel, opts.levels[1]);
    }

    private void SetLevelSlider(Slider? slider, TextBlock? label, AiLevel level)
    {
        if (slider == null || label == null) return;
        
        slider.Value = level switch
            { AiLevel.Intermediate => 1, AiLevel.Advanced => 2, AiLevel.Expert => 2, _ => 0 };

        label.Text = (int)slider.Value switch { <1 => "Look Ahead Depth: 0 - Beginner",
            1 => "Look Ahead Depth: 1 - Intermediate", >1 => "Look Ahead Depth: 3 - Advanced" };
    }

    /// <summary>Sets the game options based on the current state of the form controls.</summary>
    private void MapControlsToOptions()
    {
        opts.flags = [ShowValid?.IsChecked ?? false,
                      Preview?.IsChecked ?? false,
                      Animate?.IsChecked ?? false];

        opts.colors = [
            (BoardColorPanel?.Background is SolidColorBrush boardBrush) ?
                        boardBrush.Color : SquareControl.NormalColorDefault,
            (ValidColorPanel?.Background is SolidColorBrush validBrush) ?
                        validBrush.Color : SquareControl.NormalColorDefault,
            (ActiveColorPanel?.Background is SolidColorBrush activeBrush) ?
                        activeBrush.Color : SquareControl.NormalColorDefault,
            (MoveColorPanel?.Background is SolidColorBrush indicatorBrush) ?
                        indicatorBrush.Color : SquareControl.NormalColorDefault];

        opts.types = [
            P1_StudentAiAgent?.IsChecked ?? false ?
            Agent.Type.StudentAI : P1_ExampleAiAgent?.IsChecked ?? false ?
            Agent.Type.ExampleAI : Agent.Type.Human,
            P2_StudentAiAgent?.IsChecked ?? false ?
            Agent.Type.StudentAI : P2_ExampleAiAgent?.IsChecked ?? true ?
            Agent.Type.ExampleAI : Agent.Type.Human];

        AiLevel SliderToLevel(int slider) => slider switch
            { <1 => AiLevel.Beginner, 1 => AiLevel.Intermediate, > 1 => AiLevel.Advanced };

        opts.levels = [ SliderToLevel((int)(P1_LevelSlider?.Value ?? 1)!),
                        SliderToLevel((int)(P2_LevelSlider?.Value ?? 1)!)];
    }

    private async void BoardColor_Click(object? sender, RoutedEventArgs e)
    {
        if (BoardColorPanel != null)
            await ShowColorPicker(BoardColorPanel);
    }

    private async void ValidColor_Click(object? sender, RoutedEventArgs e)
    {
        if (ValidColorPanel != null)
            await ShowColorPicker(ValidColorPanel);
    }

    private async void ActiveColor_Click(object? sender, RoutedEventArgs e)
    {
        if (ActiveColorPanel != null)
            await ShowColorPicker(ActiveColorPanel);
    }

    private async void MoveColor_Click(object? sender, RoutedEventArgs e)
    {
        if (MoveColorPanel != null)
            await ShowColorPicker(MoveColorPanel);
    }

    private async Task ShowColorPicker(Border colorPanel)
    {
        // Create a simple color picker dialog
        var dialog = new ColorPickerDialog();
        if (colorPanel.Background is SolidColorBrush brush)
            dialog.SetColor(brush.Color);
        
        var result = await dialog.ShowDialog<Color?>(this);
        if (result.HasValue)
            colorPanel.Background = new SolidColorBrush(result.Value);
    }

    private void P1_LevelSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (P1_LevelLabel == null) return;

        P1_LevelLabel.Text = (int)e.NewValue switch
        {
            0 => "Look Ahead Depth: 0 - Beginner",
            1 => "Look Ahead Depth: 1 - Intermediate",
            2 => "Look Ahead Depth: 3 - Advanced",
            _ => "Look Ahead Depth: 0 - Beginner"
        };
    }

    private void P2_LevelSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (P2_LevelLabel == null) return;

        P2_LevelLabel.Text = (int)e.NewValue switch
        {
            0 => "Look Ahead Depth: 0 - Beginner",
            1 => "Look Ahead Depth: 1 - Intermediate",
            2 => "Look Ahead Depth: 3 - Advanced",
            _ => "Look Ahead Depth: 0 - Beginner"
        };
    }

    private async void Defaults_Click(object? sender, RoutedEventArgs e)
    {
        // Prompt for confirmation        
        if (await Uwu.Gui.MsgBox.YesNo(
            "Restoring defaults will erase your settings. Continue?", "Confirm Default Reset", 2))
        {
            opts.types =        [Agent.P1_TYPE, Agent.P2_TYPE];
            opts.levels =       [Agent.P1_DIFFICULTY, Agent.P2_DIFFICULTY];
            opts.flags =        [true, false, true];
            opts.colors =       [SquareControl.NormalColorDefault, SquareControl.ValidColorDefault,
                                 SquareControl.ActiveColorDefault, SquareControl.MoveColorDefault];

            MapConfigToControls();
        }
    }

    private void Ok_Click(object? sender, RoutedEventArgs e) {  MapControlsToOptions(); Close(opts); }
    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(null);
}

public partial class ColorPickerDialog : Window
{
    public Color SelectedColor { get; private set; }
    private Color previousColor;

    private bool updatingSlider = false;
    private bool updatingHex = false;

    public ColorPickerDialog()
    {
        InitializeComponent();
        SetColor(Colors.White);
    }

    public void SetColor(Color color)
    {
        Color temp = SelectedColor;

        SelectedColor = color;
        UpdateHexFromSelected();
        UpdateSlidersFromSelected();
        UpdatePreviewFromSelected();
        previousColor = temp;
    }

    private void UpdateSlidersFromSelected()
    {
        updatingSlider = true;
        if (RedSlider != null) RedSlider.Value = SelectedColor.R;
        if (GreenSlider != null) GreenSlider.Value = SelectedColor.G;
        if (BlueSlider != null) BlueSlider.Value = SelectedColor.B;
        updatingSlider = false;
    }

    private void UpdateHexFromSelected()
    {
        updatingHex = true;
        if (HexTextBox != null)
            HexTextBox!.Text = $"{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        updatingHex = false;
    }

    private void UpdatePreviewFromSelected()
    {
        if (PreviewBorder == null) return;

        PreviewBorder.Background = new SolidColorBrush(SelectedColor);
        if (RedValue != null) RedValue.Text = SelectedColor.R.ToString();
        if (GreenValue != null) GreenValue.Text = SelectedColor.G.ToString();
        if (BlueValue != null) BlueValue.Text = SelectedColor.B.ToString();
    }

    private void ColorSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (!updatingSlider && RedSlider != null && GreenSlider != null && BlueSlider != null)
        {
            byte r = (byte)(RedSlider?.Value ?? 0);
            byte g = (byte)(GreenSlider?.Value ?? 0);
            byte b = (byte)(BlueSlider?.Value ?? 0);
            SetColor(Color.FromRgb(r, g, b));

            UpdateHexFromSelected();
            UpdatePreviewFromSelected();
        }
    }

    private void HexTextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (HexTextBox == null || updatingHex)
            return;

        string hex = (HexTextBox.Text ?? "").Trim();
        if (hex.Length == 6 && IsValidHex(hex))
        {            
            try
            {
                byte r = Convert.ToByte(hex[0..2], 16);
                byte g = Convert.ToByte(hex[2..4], 16);
                byte b = Convert.ToByte(hex[4..6], 16);

                SetColor(Color.FromRgb(r, g, b));
                UpdateSlidersFromSelected();
                UpdatePreviewFromSelected();
            }
            catch { /* Invalid hex, ignore */ }
        }
    }

    private static bool IsValidHex(string hex)
    {
        foreach (char c in hex)
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
                return false;

        return true;
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close(SelectedColor);
    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close(SelectedColor);
}

/*
		private System.Windows.Forms.TabPage displayTabPage; ***********
		private System.Windows.Forms.Label boardColorLabel; ***********
		private System.Windows.Forms.Label validColorLabel; ******
		private System.Windows.Forms.Label activeColorLabel;******
		private System.Windows.Forms.Label moveIndicatorColorLabel;*******
		private System.Windows.Forms.TabPage playersTabPage;*******
		private System.Windows.Forms.Panel blackPlayerPanel;*********
		private System.Windows.Forms.Panel whitePlayerPanel;**********
		private System.Windows.Forms.Label blackPlayerLabel;*******
		private System.Windows.Forms.Label whitePlayerLabel;******
		private System.Windows.Forms.TabPage difficultyTabPage;*****
		private System.ComponentModel.Container components = ?????????????
		private GroupBox groupBox2; ?????????
		private GroupBox groupBox1; ?????????
		private TableLayoutPanel tableLayoutPanel1; ????????
        */
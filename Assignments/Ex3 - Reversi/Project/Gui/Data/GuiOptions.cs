using Avalonia;
using Avalonia.Media;

// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)

namespace Uwu.Games.Reversi.Gui
{
	/// <summary>Summary description for Options.</summary>
	public class GuiOptions()
	{
		// Display settings.
		public bool ShowValidMoves { get; set; } = true;
		public bool PreviewMoves { get; set; } = false;
		public bool AnimateMoves { get; set; } = true;
		public Color ActiveColor { get; set; } = SquareControl.ActiveColorDefault;
		public Color BoardColor { get; set; } = SquareControl.NormalColorDefault;
		public Color MoveColor { get; set; } = SquareControl.MoveColorDefault;
		public Color ValidColor { get; set; } = SquareControl.ValidColorDefault;
		public Point Location { get; set; } = new Point(100, 100);
		public Size WindowSize { get; set; } = new Size(820, 640);

		// Creates a new Options object by copying an existing one.
		public GuiOptions(GuiOptions options) : this()
		{
			ShowValidMoves     = options.ShowValidMoves;
			PreviewMoves       = options.PreviewMoves;
			AnimateMoves       = options.AnimateMoves;
			BoardColor         = options.BoardColor;
			ValidColor         = options.ValidColor;
			ActiveColor        = options.ActiveColor;
			MoveColor          = options.MoveColor;
			Location           = options.Location;
			WindowSize         = options.WindowSize;
		}
	}
}

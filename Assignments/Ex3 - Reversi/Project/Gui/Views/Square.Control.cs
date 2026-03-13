using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Uwu.Games.Reversi
{
    /// <summary>SquareControl: Custom Control for tiles on the Reversi board</summary>
    public class SquareControl : Control
    {
        // Constants to address some compiler limitations
        public const string ACTIVE_ARGB = "#ff00e000";
        public const string NORMAL_ARGB = "#ff008000";
        public const string VALID_ARGB  = "#ff00b000";
        public const string MOVE_ARGB   = "#ffff0000";

        // Default color values
        public static readonly Color ActiveColorDefault = Color.Parse(ACTIVE_ARGB);
        public static readonly Color NormalColorDefault = Color.Parse(NORMAL_ARGB);
        public static readonly Color ValidColorDefault = Color.Parse(VALID_ARGB);
        public static readonly Color MoveColorDefault = Color.Parse(MOVE_ARGB);

        public static void SetColors(Color board, Color valid, Color active, Color indicator)
        {
            NormalColor = board;
            ValidColor = valid;
            ActiveColor = active;
            MoveColor = indicator;
        }

        public static Color[] GetColors() => [NormalColor, ValidColor, ActiveColor, MoveColor];

        // Colors / brushes / pens used in rendering the control
        // Public colors (with brush caching in the setters)
        private static Color _activeColor = ActiveColorDefault;
        private static Color _normalColor = NormalColorDefault;
        private static Color _validColor = ValidColorDefault;
        private static Color _moveColor = MoveColorDefault;

        private static SolidColorBrush _activeBrush = new(_activeColor);
        private static SolidColorBrush _normalBrush = new(_normalColor);
        private static SolidColorBrush _validBrush = new(_validColor);
        private static Pen _movePen = new(new SolidColorBrush(_moveColor), 2);

        // Property accessors for colors
        public static Color ActiveColor
        {
            get => _activeColor;
            set { _activeColor = value; _activeBrush = new SolidColorBrush(value); }
        }
        public static Color MoveColor
        {
            get => _moveColor;
            set { _moveColor = value; _movePen = new Pen(new SolidColorBrush(value), 2); }
        }
        public static Color NormalColor
        {
            get => _normalColor;
            set { _normalColor = value; _normalBrush = new SolidColorBrush(value); }
        }
        public static Color ValidColor
        {
            get => _validColor;
            set { _validColor = value; _validBrush = new SolidColorBrush(value); }
        }

        // Contents of the square (see value definitions in Board class).
        public Player Contents { get; set; }
        public Player PreviewContents;

        public void ResetContents(Player newContents)
        {
            Contents = newContents;
            PreviewContents = Player.Empty;
            base.InvalidateVisual();
        }

        public void SetPreview(Player newPreview)
        {
            PreviewContents = newPreview;
            base.InvalidateVisual();
        }

        // Flags used for highlighting
        public bool IsValid = false;
        public bool IsActive = false;
        public bool IsNew = false;
        public void ClearFlags() => IsValid = IsActive = IsNew = false;

        // Animation variables
        public static readonly int AnimationStart = 6;
        public static readonly int AnimationStop = -AnimationStart;
        public int AnimationCounter = AnimationStop;

        public bool? AdvanceAnimation()
        {
            if (AnimationCounter > AnimationStop)
            {
                AnimationCounter--;
                InvalidateVisual();
                return false;
            }
            return null;
        }
                        
        public void StopAnimation()
        {
            AnimationCounter = AnimationStop;
            IsNew = false;
        }

        // Row/Col as styled properties so ReversiForm can set them during grid construction
        public static readonly StyledProperty<int> RowProperty =
            AvaloniaProperty.Register<SquareControl, int>(nameof(Row));
        public static readonly StyledProperty<int> ColProperty =
            AvaloniaProperty.Register<SquareControl, int>(nameof(Col));

        // Position of the square on the board
        public int Row { get => GetValue(RowProperty); set => SetValue(RowProperty, value); }
        public int Col { get => GetValue(ColProperty); set => SetValue(ColProperty, value); }

        public SquareControl()
        {
            // Ensure default brushes/pens are initialized
            SetColors(_normalColor, _validColor, _activeColor, _moveColor);
            Focusable = false;
        }

        public override void Render(DrawingContext ctx)
        {
            // Prepare the renderer; grab the background color and brush.
            base.Render(ctx);
            Color back = _normalColor;
            SolidColorBrush backBrush = _normalBrush;

            // Clear the square, filling with the appropriate background color
            if (IsValid)
            {
                back = _validColor;
                backBrush = _validBrush;
            }
            if (IsActive)
            {
                back = _activeColor;
                backBrush = _activeBrush;
            }

            // Fill background.
            var rect = new Rect(Bounds.Size);
            ctx.FillRectangle(backBrush, rect);

            // Setup the pens for beveled borders, then draw them.
            var penDark = BorderPenDark(back);
            var penLight = BorderPenLight(back);
            ctx.DrawLine(penLight, rect.TopLeft, rect.TopRight);       // top
            ctx.DrawLine(penLight, rect.TopLeft, rect.BottomLeft);     // left
            ctx.DrawLine(penDark, rect.BottomLeft, rect.BottomRight); // bottom
            ctx.DrawLine(penDark, rect.TopRight, rect.BottomRight);   // right

            // Draw the disc, if any
            if (Contents != Player.Empty || PreviewContents != Player.Empty)
            {
                // Get size and position parameters based on control size and animation state
                int size = (int)(rect.Width * (AnimationCounter > AnimationStop ? 0.85 : 0.80));
                int offset = (int)((rect.Width - size) / 2);
                int thickness = (int)(size * 0.08);
                int width = size;
                int height = Math.Max(thickness, (int)Math.Round(size * Math.Abs((double)AnimationCounter / AnimationStart)));
                int left = offset;
                int top = offset + (int)Math.Round((double)(size - height) / 2.0);

                var discRect = new Rect(left, top, width, height);

                // // Draw the shadow
                var shadowAlpha = (PreviewContents == 0) ? 48 : 24;
                var shadowBrush = new SolidColorBrush(Color.FromArgb((byte)shadowAlpha, 0, 0, 0));
                if (AnimationCounter <= AnimationStop)
                    ctx.DrawEllipse(shadowBrush, null, new Rect(left + thickness, top + thickness, width, height));
                else
                    ctx.DrawEllipse(shadowBrush, null, new Rect(left + thickness, top + thickness, width, Math.Max(height, size - top + thickness)));

                // Draw the disc edge, if animating
                if (AnimationCounter > AnimationStop)
                {
                    double pct = 1.0 - Math.Abs((double)AnimationCounter / AnimationStart);
                    int thick = (int)Math.Ceiling(1.5 * thickness * pct);
                    var edge = new SolidColorBrush(Colors.Gray);

                    if (AnimationCounter == 0)
                    {
                        ctx.FillRectangle(edge, new Rect(left, top - (int)(thick / 2.0), width, thick));
                    }
                    else
                    {
                        var edgeRect = new Rect(left, top - (AnimationCounter < 0 ? thick : 0), width, height + thick);
                        ctx.DrawGeometry(edge, null, new EllipseGeometry(edgeRect));
                    }
                }

                // Draw the disc face, if not on edge
                SolidColorBrush face;
                if (PreviewContents == 0)
                {
                    bool liveBlack = Contents == Player.Black;
                    var black = new SolidColorBrush(Colors.Black);
                    var white = new SolidColorBrush(Color.FromArgb(0xFF, 0xEB, 0xEB, 0xEB)); // slightly dimmed
                    face = (AnimationCounter > 0) ? (liveBlack ? white : black)
                                                  : (liveBlack ? black : white);
                }
                else
                {
                    // If the disc is being flipped, switch the color.
                    bool previewBlack = PreviewContents < 0;
                    face = previewBlack
                        ? new SolidColorBrush(Color.FromArgb(96, 0, 0, 0))
                        : new SolidColorBrush(Color.FromArgb(160, 255, 255, 255));
                }
                ctx.DrawEllipse(face, null, discRect);

                // Highlight the disc face.
                var center = new RelativePoint(1.0 / 3.0, 1.0 / 3.0, RelativeUnit.Relative);
                var radial = new RadialGradientBrush
                {
                    Center = center,
                    GradientOrigin = center,
                    RadiusX = new RelativeScalar(.085, RelativeUnit.Relative),
                    RadiusY = new RelativeScalar(.085, RelativeUnit.Relative),
                };

                if (PreviewContents == 0)
                {
                    bool faceIsBlack = face.Color.ToUInt32() == Colors.Black.ToUInt32();
                    var hi = faceIsBlack ? Color.FromArgb(128, 169, 169, 169) : Colors.White;
                    radial.GradientStops.Add(new GradientStop(hi, 0));
                    radial.GradientStops.Add(new GradientStop(face.Color, 1));
                }
                else
                {
                    bool previewBlack = PreviewContents < 0;
                    var hi = previewBlack ? Color.FromArgb(48, 169, 169, 169)
                                          : Color.FromArgb(96, 255, 255, 255);
                    radial.GradientStops.Add(new GradientStop(hi, 0));
                    radial.GradientStops.Add(new GradientStop(face.Color, 1));
                }

                ctx.DrawEllipse(radial, null, discRect);

                // Draw a circle around the disc if it has been newly added.
                if (IsNew)
                    ctx.DrawEllipse(null, _movePen, discRect);
            }
        }
        
        // Border pens are recomputed each frame from the current background (to keep the bevel faithful)
        private static Pen BorderPenLight(Color back) => new(new SolidColorBrush(AdjustBrightness(back, 1.5)), 1);
        private static Pen BorderPenDark(Color back) => new(new SolidColorBrush(AdjustBrightness(back, 0.6)), 1);

        // Returns a lighter or darker version of the given color.
        private static Color AdjustBrightness(Color c, double m)
        {
            static byte Clamp(double v) => (byte)Math.Max(0, Math.Min(255, Math.Round(v)));
            return Color.FromArgb(c.A, Clamp(c.R * m), Clamp(c.G * m), Clamp(c.B * m));
        }
    }
}

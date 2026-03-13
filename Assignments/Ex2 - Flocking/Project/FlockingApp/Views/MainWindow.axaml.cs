using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SkiaSharp;

using Uwu.Simulation;
using Uwu.Simulation.Steering;
using Uwu.Simulation.Steering.ExampleAI;
using Uwu.Simulation.Steering.StudentAI;

namespace Uwu.Simulation.Steering.Flocking.Views
{
    public partial class MainWindow : Window
    {
        // State information
        private readonly List<Flocker> _flocks = new();
        private bool _useExample = true;
        private int _frameCount;
        private int _totalBoids;

        // Timing variables
        private const double UPDATE_TIMESTEP = 0.04;
        private readonly DispatcherTimer _updateTimer;
        private double _accumulatedSeconds;

        // Assets
        private readonly Dictionary<uint, Bitmap> _sprites = new(); // ARGB -> Bitmap
        private readonly Dictionary<uint, Brush> _brushes = new(); // ARGB -> Brush
        private Bitmap? _background;
        private SKBitmap? _boidImg;

        // Misc.
        private Random rng = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // Try to get the icon and the resources. On failure, silently continue (?).
            this.Icon = LoadFromStream<WindowIcon>("Assets/Flocking.ico");
            _background = LoadFromStream<Bitmap>("Assets/background.png");
            _boidImg = LoadFromStream<SKBitmap>("Assets/boid.png", stream =>
            {
                using var memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Position = 0;
                return SKBitmap.Decode(memStream);
            });

            // set RenderHost callback to our Render() (mirrors GDI+ pipeline)
            RenderHost.RenderRequested += OnRenderRequested;

            // Set up the timer.
            _updateTimer = new DispatcherTimer { Interval=TimeSpan.FromSeconds(UPDATE_TIMESTEP) };
            _updateTimer.Tick += updateTimer_Tick;
            _updateTimer.Start();

            // Initialize default values. UI Redundant?...
            radioExample.IsChecked = true;
            boidCount.Value = 3;
            flockRadius.Value = 100;

            // nothing selected, disable controls
//            numBoidsSelector.Enabled = false;
//            flockRadiusSelector.Enabled = false;
//            colorButton.Enabled = false;
//            alignmentWeightSelector.Enabled = false;
//            cohesionWeightSelector.Enabled = false;
//            separationWeightSelector.Enabled = false;
        }

        public override void Render(DrawingContext context)
        {
            _frameCount++; // FPS accounting
            IBrush bgBrush;

            if (_background != null)
                bgBrush = new ImageBrush(_background) { Stretch = Stretch.None };
            else
                bgBrush = Brushes.Black;

            context.DrawRectangle(bgBrush, null, RenderHost.Bounds);

            foreach (var flock in _flocks)
            {
                if (_boidImg == null)
                {
                    if (!_brushes.TryGetValue((uint)flock.GroupColor.ToArgb(), out var brush))
                    {
                        brush = new SolidColorBrush((uint)flock.GroupColor.ToArgb());
                        _brushes[(uint)flock.GroupColor.ToArgb()] = brush;
                    }

                    foreach (var boid in flock.Boids)
                        context.DrawEllipse(brush, null,
                            new Point(boid.Position.X, boid.Position.Y), 3, 3);

                    continue;
                }

                uint argbColor = unchecked((uint)flock.GroupColor.ToArgb());

                foreach (var boid in flock.Boids)
                {
                    var _width = _boidImg.Width;
                    var _height = _boidImg.Height;

                    var tf = Matrix.CreateTranslation(_width / -2.0, _height / -2.0);
                    tf *= Matrix.CreateRotation(boid.Heading);
                    tf *= Matrix.CreateTranslation(boid.Position.X, boid.Position.Y);

                    using (context.PushTransform(tf))
                        DrawTintedImage(context, new Rect(0, 0, _width, _height), argbColor);
                }
            }
        }

        // Event Handlers
        private void updateTimer_Tick(object? from, EventArgs e)
        {
            // Time update; flock update.
            _accumulatedSeconds += UPDATE_TIMESTEP;
            UpdateFlocks();

            if (_accumulatedSeconds >= 1.0) // Hmm... really here?...
            {
                labelFps.Text = $"FPS: {_frameCount:D4}";
                _frameCount = 0;
                _accumulatedSeconds = 0.0;
            }

            // trigger redraw
            RenderHost.InvalidateVisual();
        }

        private void btnAddFlock_Click(object? from, RoutedEventArgs e)
        {
            // Construct a new flock.
            float x = Convert.ToSingle(rng.NextDouble() * RenderHost.Bounds.Width);
            float y = Convert.ToSingle(rng.NextDouble() * RenderHost.Bounds.Height);
            Type type = (_useExample ? typeof(ExampleFlock) : typeof(Flock));
            Flocker flock = (_useExample ? new ExampleFlock() : new Flock());
            flock.AveragePosition = new Vector2(x, y);
            flock.GroupRadius = (double)flockRadius.Value!;
            flock.SetCount((int)(boidCount.Value!));

            // initial strengths from sliders
            flock.AlignmentStrength  = sliderAlignment.Value;
            flock.CohesionStrength   = sliderCohesion.Value;
            flock.SeparationStrength = sliderSeparation.Value;

            _flocks.Add(flock);
            listFlocks.ItemsSource = null; // simple refresh TODO: WHAT??
            listFlocks.ItemsSource = _flocks;
            listFlocks.SelectedItem = flock;
        }

        private void btnRemoveFlock_Click(object? from, RoutedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
            {
                flock.Clear();
                _flocks.Remove(flock);
                listFlocks.ItemsSource = null;
                listFlocks.ItemsSource = _flocks;
            }
        }

        private void listFlocks_SelectionChanged(object? from, SelectionChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is not Flocker flock) return;

            // push selection to controls
            _totalBoids = flock.Boids.Count;
            flockRadius.Value = Convert.ToDecimal(flock.GroupRadius);
            sliderAlignment.Value = flock.AlignmentStrength;
            sliderCohesion.Value = flock.CohesionStrength;
            sliderSeparation.Value = flock.SeparationStrength;
            // TODO: What about ColorButton???
        }

        private void boidCount_ValueChanged(object? from, NumericUpDownValueChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
                flock.SetCount((int)e.NewValue!);
        }

        private void flockRadius_ValueChanged(object? from, NumericUpDownValueChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
                flock.GroupRadius = (double)e.NewValue!;
        }

        private void sliderAlignment_ValueChanged(object? from, RangeBaseValueChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
                flock.AlignmentStrength = e.NewValue;
        }
        private void sliderCohesion_ValueChanged(object? from, RangeBaseValueChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
                flock.CohesionStrength = e.NewValue;
        }
        private void sliderSeparation_ValueChanged(object? from, RangeBaseValueChangedEventArgs e)
        {
            if (listFlocks.SelectedItem is Flocker flock)
                flock.SeparationStrength = e.NewValue;
        }

        private void radioStudent_Checked(object? s, RoutedEventArgs e) => _useExample = false;
        private void radioExample_Checked(object? s, RoutedEventArgs e) => _useExample = true;

        private void btnColor_Click(object? from, RoutedEventArgs e)
        {
            if (listFlocks.SelectedItem is not Flocker flock)
                return;

            int r = rng.Next(40,256), g = rng.Next(40,256), b = rng.Next(40,256);
            flock.GroupColor = System.Drawing.Color.FromArgb(255, r, g, b);
        }

        // input if you used it before
        private void RenderHost_PointerPressed(object? from, PointerPressedEventArgs e)
        {
            // e.g., focus or future interactions
        }

        // RenderHost calls this during drawing
        private void OnRenderRequested(DrawingContext ctx) => Render(ctx);

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            foreach (var kv in _sprites) kv.Value.Dispose();
            _sprites.Clear();
            _brushes.Clear();
            _boidImg?.Dispose();
        }

        private void UpdateFlocks()
        {
            var bounds = RenderHost.Bounds;
            var width = (float)bounds.Width;
            var height = (float)bounds.Height;
            _totalBoids = 0;

            foreach (Flocker flock in _flocks)
            {
                _totalBoids += flock.Boids.Count;
                flock.Update((float)UPDATE_TIMESTEP);

                foreach (MovingObject boid in flock.Boids)
                {
                    double radius = boid.CollisionRadius;
                    double x = boid.Position.X, y = boid.Position.Y;

                    if (x + radius < 0)
                        x = width + radius;

                    else if (x - radius > width)
                        x = -radius;

                    if (y + radius < 0)
                        y = height + radius;

                    else if (y - radius > height)
                        y = -radius;

                    boid.Position = new Vector2((float)x, (float)y);
                }
            }
            labelTotalBoids.Text = $"Total Boids: {boidCount:D5}";
        }

        private T? LoadFromStream<T>(string path, Func<Stream, T>? handler=null) where T: class
        {
            try
            {
                var assembly = GetType().Assembly.GetName().Name!;
                var uri = new Uri($"avares://{assembly}/{path}");
                using var stream = AssetLoader.Open(uri);

                if (handler != null)
                    return handler(stream);

                var streamBuilder = typeof(T).GetConstructor(new Type[] {typeof(Stream)});
                if (streamBuilder != null)
                    return (T)streamBuilder.Invoke(new object[] {stream});
                else
                    return null;
            }
            catch { return null; }
        }

        private void DrawTintedImage(DrawingContext ctx, Rect dest, uint argb)
        {
            if (!_sprites.TryGetValue(argb, out var sprite))
            {
                // Prepare tinting cheme and image.
                var filter = SKColorFilter.CreateBlendMode(new SKColor(argb), SKBlendMode.Modulate);
                var paint = new SKPaint { ColorFilter = filter };
                var image = SKImage.FromBitmap(_boidImg);

                // Draw the tinted image to a surface.
                var surface = SKSurface.Create(new SKImageInfo(_boidImg!.Width, _boidImg!.Height));
                surface.Canvas.Clear(SKColors.Transparent);
                surface.Canvas.DrawImage(image, 0, 0, paint);
                surface.Canvas.Flush();

                // Save the surface as an Avalonia bitmap.
                var memStream = new MemoryStream();
                surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(memStream);
                memStream.Position = 0;
                sprite = _sprites[argb] = new Bitmap(memStream);
            }

            // Draw the tinted Avalonia bitmap
            ctx.DrawImage(sprite, new Rect(sprite.Size), dest);
        }
    }
}


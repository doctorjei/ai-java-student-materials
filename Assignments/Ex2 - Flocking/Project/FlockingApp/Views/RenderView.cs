using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Uwu.Simulation.Steering.Flocking.Views
{
   // === a minimal RenderControl equivalent ===
    public sealed class RenderView : Control
    {
        public event Action<DrawingContext>? RenderRequested;

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            RenderRequested?.Invoke(context);
        }
    }
}


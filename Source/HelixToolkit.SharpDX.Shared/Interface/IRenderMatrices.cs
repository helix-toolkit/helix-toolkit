using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX
{
    public interface IRenderMatrices
    {
        Matrix ViewMatrix { get; }

        Matrix ProjectionMatrix { get; }

        Matrix WorldMatrix { get; }

        Matrix ViewportMatrix { get; }

        Matrix ScreenViewProjectionMatrix { get; }

        double ActualWidth { get; }
        double ActualHeight { get; }
    }
}

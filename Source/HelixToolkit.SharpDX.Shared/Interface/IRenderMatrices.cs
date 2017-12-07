using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    public interface IRenderMatrices
    {
        ICamera Camera { get; }
        Matrix ViewMatrix { get; }

        Matrix ProjectionMatrix { get; }

        Matrix WorldMatrix { get; }

        Matrix ViewportMatrix { get; }

        Matrix ScreenViewProjectionMatrix { get; }

        double ActualWidth { get; }
        double ActualHeight { get; }

        DeviceContext DeviceContext { get; }

        TimeSpan TimeStamp { get; }
        Light3DSceneShared LightScene { get; }

        void UpdatePerFrameData();
    }
}

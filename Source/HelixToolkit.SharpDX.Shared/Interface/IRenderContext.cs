/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    public interface IRenderContext : IDisposable
    {
        ICamera Camera { set; get; }
        Matrix ViewMatrix { get; }

        Matrix ProjectionMatrix { get; }

        Matrix WorldMatrix { set; get; }

        Matrix ViewportMatrix { get; }

        Matrix ScreenViewProjectionMatrix { get; }

        double ActualWidth { get; }
        double ActualHeight { get; }

        DeviceContext DeviceContext { get; }

        TimeSpan TimeStamp { get; }
        Light3DSceneShared LightScene { get; }
        GlobalTransformStruct GlobalTransform { get; }
        void UpdatePerFrameData();

        bool IsShadowPass { set; get; }
        bool EnableBoundingFrustum { set; get; }
        BoundingFrustum BoundingFrustum { set; get; }
        IRenderHost RenderHost { get; }

        IContextSharedResource SharedResource { get; }
    }

    public interface IContextSharedResource : IDisposable
    {
        ShaderResourceView ShadowView { set; get; }
    }
}

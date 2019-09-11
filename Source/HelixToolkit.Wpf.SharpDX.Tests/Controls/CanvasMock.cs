// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasMock.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Render;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX.Tests.Controls
{
    using SharpDX.Utilities;

    class CanvasMock : IRenderCanvas
    {
        public IRenderHost RenderHost { private set; get; } = new DefaultRenderHost();

        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;

        public CanvasMock()
        {
            RenderHost.EffectsManager = new DefaultEffectsManager();
        }
    }
}

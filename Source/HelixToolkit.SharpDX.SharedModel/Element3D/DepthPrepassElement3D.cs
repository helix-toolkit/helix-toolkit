// <copyright file="CrossSectionMeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2018 Helix Toolkit contributors
// </copyright>

using System.Collections.Generic;
using SharpDX;
#if NETFX_CORE
namespace HelixToolkit.UWP
#elif WINUI
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// Do a depth prepass before rendering.
    /// <para>Must customize the DefaultEffectsManager and set DepthStencilState to DefaultDepthStencilDescriptions.DSSDepthEqualNoWrite in default ShaderPass from EffectsManager to achieve best performance.</para>
    /// </summary>
    public sealed class DepthPrepassElement3D : Element3D
    {
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new DepthPrepassNode();
        }
        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        public override bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}

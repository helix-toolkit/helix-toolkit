/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Linq;

#if DX11_1
#else
using Device = SharpDX.Direct3D11.Device;
#endif

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Render
    {
        using Core;
        using Model.Scene;
        using Model.Scene2D;

        public partial class DefaultRenderHost
        {
            #region Per frame render list
            protected readonly FastList<SceneNode> viewportRenderables = new FastList<SceneNode>();
            /// <summary>
            /// The pending renderables
            /// </summary>
            protected readonly FastList<KeyValuePair<int, SceneNode>> perFrameFlattenedScene = new FastList<KeyValuePair<int, SceneNode>>();
            /// <summary>
            /// The light renderables
            /// </summary>
            protected readonly FastList<SceneNode> lightNodes = new FastList<SceneNode>();
            /// <summary>
            /// The pending render nodes
            /// </summary>
            protected readonly FastList<SceneNode> opaqueNodes = new FastList<SceneNode>();
            /// <summary>
            /// The opaque nodes in frustum
            /// </summary>
            protected readonly FastList<SceneNode> opaqueNodesInFrustum = new FastList<SceneNode>();
            /// <summary>
            /// The transparent nodes
            /// </summary>
            protected readonly FastList<SceneNode> transparentNodes = new FastList<SceneNode>();
            /// <summary>
            /// The transparent nodes in frustum
            /// </summary>
            protected readonly FastList<SceneNode> transparentNodesInFrustum = new FastList<SceneNode>();
            /// <summary>
            /// The particle nodes
            /// </summary>
            protected readonly FastList<SceneNode> particleNodes = new FastList<SceneNode>();
            /// <summary>
            /// The pending render nodes
            /// </summary>
            protected readonly FastList<SceneNode> preProcNodes = new FastList<SceneNode>();
            /// <summary>
            /// The pending render nodes
            /// </summary>
            protected readonly FastList<SceneNode> postProcNodes = new FastList<SceneNode>();
            /// <summary>
            /// The render nodes for post render
            /// </summary>
            protected readonly FastList<SceneNode> nodesForPostRender = new FastList<SceneNode>();
            /// <summary>
            /// The pending render nodes
            /// </summary>
            protected readonly FastList<SceneNode> screenSpacedNodes = new FastList<SceneNode>();

            /// <summary>
            /// The viewport renderable2D
            /// </summary>
            protected readonly FastList<SceneNode2D> viewportRenderable2D = new FastList<SceneNode2D>();
            /// <summary>
            /// The need update cores
            /// </summary>
            private readonly FastList<RenderCore> needUpdateCores = new FastList<RenderCore>();

            /// <summary>
            /// Gets the current frame flattened scene graph. KeyValuePair.Key is the depth of the node.
            /// </summary>
            /// <value>
            /// Gets the current frame flattened scene graph
            /// </value>
            public sealed override FastList<KeyValuePair<int, SceneNode>> PerFrameFlattenedScene { get { return perFrameFlattenedScene; } }
            /// <summary>
            /// Gets the per frame lights.
            /// </summary>
            /// <value>
            /// The per frame lights.
            /// </value>
            public sealed override IEnumerable<LightNode> PerFrameLights
            {
                get { return lightNodes.Select(x => x as LightNode); }
            }
            /// <summary>
            /// Gets the per frame nodes for opaque rendering. <see cref="RenderType.Opaque"/>
            /// <para>This does not include <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>, <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
            /// </summary>
            public sealed override FastList<SceneNode> PerFrameOpaqueNodes { get { return opaqueNodes; } }
            /// <summary>
            /// Gets the per frame opaque nodes in frustum.
            /// </summary>
            /// <value>
            /// The per frame opaque nodes in frustum.
            /// </value>
            public sealed override FastList<SceneNode> PerFrameOpaqueNodesInFrustum { get { return opaqueNodesInFrustum; } }

            /// <summary>
            /// Gets the per frame transparent nodes in frustum.
            /// </summary>
            /// <value>
            /// The per frame transparent nodes in frustum.
            /// </value>
            public sealed override FastList<SceneNode> PerFrameTransparentNodesInFrustum { get { return transparentNodesInFrustum; } }
            /// <summary>
            /// Gets the per frame transparent nodes. , <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>
            /// <para>This does not include <see cref="RenderType.Opaque"/>, <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
            /// </summary>
            /// <value>
            /// The per frame transparent nodes.
            /// </value>
            public sealed override FastList<SceneNode> PerFrameTransparentNodes { get { return transparentNodes; } }
            /// <summary>
            /// Gets the per frame transparent nodes.
            /// </summary>
            /// <value>
            /// The per frame transparent nodes.
            /// </value>
            public sealed override FastList<SceneNode> PerFrameParticleNodes { get { return particleNodes; } }
            /// <summary>
            /// Gets the per frame post effects cores. It is the subset of <see cref="PerFrameOpaqueNodes"/>
            /// </summary>
            /// <value>
            /// The per frame post effects cores.
            /// </value>
            public sealed override FastList<SceneNode> PerFrameNodesWithPostEffect
            {
                get { return nodesForPostRender; }
            }

            public const int FrustumPartitionSize = 500;
            #endregion
        }
    }
}

/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    /// <summary>
    ///
    /// </summary>
    public class MeshNode : MaterialGeometryNode
    {
        #region Properties
        private bool frontCCW = true;
        /// <summary>
        /// Gets or sets a value indicating whether [front CCW].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front CCW]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontCCW
        {
            get { return frontCCW; }
            set
            {
                if (Set(ref frontCCW, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private CullMode cullMode = CullMode.None;
        /// <summary>
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
        public CullMode CullMode
        {
            get { return cullMode; }
            set
            {
                if (Set(ref cullMode, value))
                {
                    OnRasterStateChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [invert normal].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invert normal]; otherwise, <c>false</c>.
        /// </value>
        public bool InvertNormal
        {
            get { return (RenderCore as MeshRenderCore).InvertNormal; }
            set
            {
                (RenderCore as MeshRenderCore).InvertNormal = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable tessellation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable tessellation]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTessellation
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.EnableTessellation;
                }
                else { return false; }
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.EnableTessellation = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation factor.
        /// </summary>
        /// <value>
        /// The maximum tessellation factor.
        /// </value>
        public float MaxTessellationFactor
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.MaxTessellationFactor;
                }
                return 0;
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.MaxTessellationFactor = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation factor.
        /// </summary>
        /// <value>
        /// The minimum tessellation factor.
        /// </value>
        public float MinTessellationFactor
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.MinTessellationFactor;
                }
                return 0;
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.MinTessellationFactor = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation distance.
        /// </summary>
        /// <value>
        /// The maximum tessellation distance.
        /// </value>
        public float MaxTessellationDistance
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.MaxTessellationDistance;
                }
                return 0;
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.MaxTessellationDistance = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation distance.
        /// </summary>
        /// <value>
        /// The minimum tessellation distance.
        /// </value>
        public float MinTessellationDistance
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.MinTessellationDistance;
                }
                return 0;
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.MinTessellationDistance = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the type of the mesh.
        /// </summary>
        /// <value>
        /// The type of the mesh.
        /// </value>
        public MeshTopologyEnum MeshType
        {
            get
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    return core.MeshType;
                }
                return 0;
            }
            set
            {
                if (RenderCore is IPatchRenderParams core)
                {
                    core.MeshType = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color4 WireframeColor
        {
            get
            {
                return (RenderCore as IMeshRenderParams).WireframeColor;
            }
            set
            {
                (RenderCore as IMeshRenderParams).WireframeColor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [render wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get { return (RenderCore as IMeshRenderParams).RenderWireframe; }
            set { (RenderCore as IMeshRenderParams).RenderWireframe = value; }
        } 
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IGeometryBufferProxy OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return EffectsManager.GeometryBufferManager.Register<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Create raster state description.
        /// </summary>
        /// <returns></returns>
        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCCW,
                IsMultisampleEnabled = IsMSAAEnabled,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
        }

        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return base.OnCheckGeometry(geometry) && geometry is MeshGeometry3D;
        }

        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context) && MeshType == MeshTopologyEnum.PNTriangles;
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
        {
            return (Geometry as MeshGeometry3D).HitTest(context, totalModelMatrix, ref rayWS, ref hits, this.WrapperSource);
        }
    }
}
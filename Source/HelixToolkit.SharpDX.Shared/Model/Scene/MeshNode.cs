/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using Matrix = System.Numerics.Matrix4x4;

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
    public class MeshNode : MaterialGeometryNode, IDynamicReflectable
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

        /// <summary>
        /// Gets or sets the dynamic reflector.
        /// </summary>
        /// <value>
        /// The dynamic reflector.
        /// </value>
        public IDynamicReflector DynamicReflector
        {
            set
            {
                (RenderCore as IDynamicReflectable).DynamicReflector = value;
            }
            get
            {
                return (RenderCore as IDynamicReflectable).DynamicReflector;
            }
        }
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshRenderCore();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicMeshGeometryBufferModel>(modelGuid, geometry)
                : EffectsManager.GeometryBufferManager.Register<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
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
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
        {
            return (Geometry as MeshGeometry3D).HitTest(context, totalModelMatrix, ref rayWS, ref hits, this.WrapperSource);
        }
    }
}
/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

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
    public class BillboardNode : GeometryNode
    {
        /// <summary>
        /// Gets or sets a value indicating whether [fixed size].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedSize
        {
            set
            {
                (RenderCore as IBillboardRenderParams).FixedSize = value;
            }
            get { return (RenderCore as IBillboardRenderParams).FixedSize; }
        }

        private bool isTransparent = false;
        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return isTransparent; }
            set
            {
                if (Set(ref isTransparent, value))
                {
                    if (RenderCore.RenderType == RenderType.Opaque || RenderCore.RenderType == RenderType.Transparent)
                    {
                        RenderCore.RenderType = value ? RenderType.Transparent : RenderType.Opaque;
                    }
                }
            }
        }

        public BillboardNode()
        {
            HasBound = false;
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new BillboardRenderCore();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IGeometryBufferProxy OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return EffectsManager.GeometryBufferManager.Register<DefaultBillboardBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardText];
        }
        /// <summary>
        /// Checks the bounding frustum.
        /// </summary>
        /// <param name="viewFrustum">The view frustum.</param>
        /// <returns></returns>
        protected override bool CheckBoundingFrustum(BoundingFrustum viewFrustum)
        {
            if (!HasBound)
            {
                return true;
            }
            var sphere = this.BoundsSphereWithTransform;
            return viewFrustum.Intersects(ref sphere);
        }

        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return geometry is IBillboardText;
        }
        /// <summary>
        /// Create raster state description.
        /// </summary>
        /// <returns></returns>
        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = false,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (Geometry as BillboardBase).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, FixedSize);
        }
    }
}

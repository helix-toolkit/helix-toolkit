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
    public class LineNode : GeometryNode
    {
        #region Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            get { return (RenderCore as ILineRenderParams).LineColor; }
            set { (RenderCore as ILineRenderParams).LineColor = value; }
        }
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public float Thickness
        {
            get { return (RenderCore as ILineRenderParams).Thickness; }
            set { (RenderCore as ILineRenderParams).Thickness = value; }
        }

        /// <summary>
        /// Gets or sets the smoothness.
        /// </summary>
        /// <value>
        /// The smoothness.
        /// </value>
        public float Smoothness
        {
            get { return (RenderCore as ILineRenderParams).Smoothness; }
            set { (RenderCore as ILineRenderParams).Smoothness = value; }
        }

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            set; get;
        } = 1.0; 
        #endregion

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicLineGeometryBufferModel>(modelGuid, geometry) 
                : EffectsManager.GeometryBufferManager.Register<DefaultLineGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new LineRenderCore();
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
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = IsMSAAEnabled,
                //IsAntialiasedLineEnabled = true, // Intel HD 3000 doesn't like this (#10051) and it's not needed
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        ///<para>If<see cref="SceneNode.OnSetRenderTechnique" /> is set, then<see cref="SceneNode.OnSetRenderTechnique" /> instead of<see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Lines];
        }
        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context))
            {
                return !RenderHost.IsDeferredLighting;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return base.OnCheckGeometry(geometry) && geometry is LineGeometry3D;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (Geometry as LineGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, (float)HitTestThickness);
        }
    }
}

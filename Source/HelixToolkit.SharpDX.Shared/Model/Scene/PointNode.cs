/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using global::SharpDX;

    /// <summary>
    /// 
    /// </summary>
    public class PointNode : GeometryNode
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
            get { return (RenderCore as IPointRenderParams).PointColor; }
            set { (RenderCore as IPointRenderParams).PointColor = value; }
        }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size2F Size
        {
            get { return new Size2F((RenderCore as IPointRenderParams).Width, (RenderCore as IPointRenderParams).Height); }
            set
            {
                (RenderCore as IPointRenderParams).Width = value.Width;
                (RenderCore as IPointRenderParams).Height = value.Height;
            }
        }
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            get { return (RenderCore as IPointRenderParams).Figure; }
            set { (RenderCore as IPointRenderParams).Figure = value; }
        }
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public float FigureRatio
        {
            get { return (RenderCore as IPointRenderParams).FigureRatio; }
            set { (RenderCore as IPointRenderParams).FigureRatio = value; }
        }

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            set; get;
        } = 4; 
        #endregion

        /// <summary>
        /// Distances the ray to point.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static double DistanceRayToPoint(Ray r, Vector3 p)
        {
            Vector3 v = r.Direction;
            Vector3 w = p - r.Position;

            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;

            Vector3 pb = r.Position + v * b;
            return (p - pb).Length();
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return geometry != null && geometry.IsDynamic ? EffectsManager.GeometryBufferManager.Register<DynamicPointGeometryBufferModel>(modelGuid, geometry) 
                : EffectsManager.GeometryBufferManager.Register<DefaultPointGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PointRenderCore();
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
                CullMode = CullMode.Back,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };
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
            return host.EffectsManager[DefaultRenderTechniqueNames.Points];
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
            return base.OnCheckGeometry(geometry) && geometry is PointGeometry3D;
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
            return (Geometry as PointGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this.WrapperSource, (float)HitTestThickness);
        }
    }
}
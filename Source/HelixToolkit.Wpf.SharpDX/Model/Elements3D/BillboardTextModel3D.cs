using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.GeometryModel3D" />
    public class BillboardTextModel3D : GeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(BillboardTextModel3D),
            new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3D).RenderCore as IBillboardRenderParams).FixedSize = (bool)e.NewValue;
                }));

        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public bool FixedSize
        {
            set
            {
                SetValue(FixedSizeProperty, value);
            }
            get
            {
                return (bool)GetValue(FixedSizeProperty);
            }
        }
        #endregion

        #region Overridable Methods        
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new BillboardRenderCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as IBillboardRenderParams).FixedSize = FixedSize;
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IGeometryBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            var buffer = EffectsManager.GeometryBufferManager.Register<DefaultBillboardBufferModel>(modelGuid, geometry);
            return buffer;
        }
        /// <summary>
        /// Called when [unregister buffer model].
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        protected override void OnUnregisterBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            EffectsManager.GeometryBufferManager.Unregister<DefaultBillboardBufferModel>(modelGuid, geometry);
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="Element3DCore.OnSetRenderTechnique" /> is set, then <see cref="Element3DCore.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardText];
        }
        ///// <summary>
        ///// Checks the bounding frustum.
        ///// </summary>
        ///// <param name="viewFrustum">The view frustum.</param>
        ///// <returns></returns>
        //protected override bool CheckBoundingFrustum(BoundingFrustum viewFrustum)
        //{
        //    var sphere = Geometry.BoundingSphere;
        //    return viewFrustum.Intersects(ref sphere);
        //}
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
            return (Geometry as BillboardBase).HitTest(context, totalModelMatrix, ref ray, ref hits, this, FixedSize);
        }

        #endregion
    }
}

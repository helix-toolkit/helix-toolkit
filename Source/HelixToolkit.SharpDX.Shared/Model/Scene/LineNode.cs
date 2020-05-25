/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

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
    namespace Model.Scene
    {
        using Core;
        /// <summary>
        /// 
        /// </summary>
        public class LineNode : MaterialGeometryNode
        {
            private double hitTestThickness = 1;
            /// <summary>
            /// Used only for point/line hit test
            /// </summary>
            public double HitTestThickness
            {
                set => Set(ref hitTestThickness, value);
                get => hitTestThickness;
            }
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
                return new PointLineRenderCore();
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
                    IsFrontCounterClockwise = true,

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

            protected override bool PreHitTestOnBounds(ref Ray ray)
            {
                return BoundsSphereWithTransform.Intersects(ref ray);
            }
        }
    }

}

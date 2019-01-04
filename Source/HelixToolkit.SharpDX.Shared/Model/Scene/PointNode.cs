/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

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
        public class PointNode : MaterialGeometryNode
        {
            #region Properties
            private double hitTestThickness = 4;
            /// <summary>
            /// Used only for point/line hit test
            /// </summary>
            public double HitTestThickness
            {
                set => Set(ref hitTestThickness, value);
                get => hitTestThickness;
            }
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

}
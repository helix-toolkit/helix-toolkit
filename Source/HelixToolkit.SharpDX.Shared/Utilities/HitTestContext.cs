/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
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
    public sealed class HitTestContext
    {
        /// <summary>
        /// Gets or sets the render matrices.
        /// </summary>
        /// <value>
        /// The render matrices.
        /// </value>
        public IRenderMatrices RenderMatrices
        {
            set; get;
        } = null;
        /// <summary>
        /// Gets or sets the ray in world space.
        /// </summary>
        /// <value>
        /// The ray.
        /// </value>
        public Ray RayWS
        {
            set; get;
        }
        /// <summary>
        /// Gets or sets the hit point on screen space. This is the hit point on viewport region without DpiScaled coordinate.
        /// </summary>
        /// <value>
        /// The screen hit point.
        /// </value>
        public Vector2 HitPointSP
        {
            set; get;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HitTestContext"/> class.
        /// </summary>
        /// <param name="metrices">The render metrices.</param>
        /// <param name="rayWS">The ray in world space.</param>
        /// <param name="hitSP">The hit point on screen space. Pass in the hit point on viewport region directly. 
        /// <para>Do not scale with DpiScale factor.</para></param>
        public HitTestContext(IRenderMatrices metrices, ref Ray rayWS, ref Vector2 hitSP)
        {
            RenderMatrices = metrices;
            RayWS = rayWS;
            HitPointSP = hitSP;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HitTestContext"/> class.
        /// </summary>
        /// <param name="metrices">The render metrices.</param>
        /// <param name="rayWS">The ray in world space.</param>
        /// <param name="hitSP">The hit point on screen space. Pass in the hit point on viewport region directly.
        /// <para>Do not scale with DpiScale factor.</para></param>
        public HitTestContext(IRenderMatrices metrices, Ray rayWS, Vector2 hitSP)
            : this(metrices, ref rayWS, ref hitSP)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitTestContext"/> class.
        /// This calculates screen hit point automatically from metrices and world space ray.
        /// </summary>
        /// <param name="metrices">The render metrices.</param>
        /// <param name="rayWS">The ray in world space.</param>
        public HitTestContext(IRenderMatrices metrices, ref Ray rayWS)
        {
            RenderMatrices = metrices;
            RayWS = rayWS;
            if (metrices != null)
            {
                HitPointSP = metrices.Project(rayWS.Position);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitTestContext"/> class.
        /// This calculates ray in world space automatically from metrices and hit point.
        /// </summary>
        /// <param name="metrices">The render metrices.</param>
        /// <param name="hitSP">Screen hit point. Pass in the hit point on viewport region directly.
        /// <para>Do not scale with DpiScale factor.</para></param>
        public HitTestContext(IRenderMatrices metrices, ref Vector2 hitSP)
        {
            RenderMatrices = metrices;
            HitPointSP = hitSP;
            if (metrices != null)
            {
                metrices.UnProject(hitSP, out var ray);
                RayWS = ray;
            }
        }
    }
}

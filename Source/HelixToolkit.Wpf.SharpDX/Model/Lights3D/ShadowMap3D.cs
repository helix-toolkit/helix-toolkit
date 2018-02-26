// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShadowMap3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.ComponentModel;
    using System.Windows;
    using System.Linq;
    using global::SharpDX;

    using Utilities;
    using Core;
    using HelixToolkit.Wpf.SharpDX.Cameras;
    using System.Collections.Generic;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Element3D" />
    public class ShadowMap3D : Element3D
    {
        /// <summary>
        /// The resolution property
        /// </summary>
        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Size), typeof(ShadowMap3D), new PropertyMetadata(new Size(1024, 1024), (d, e) =>
            {
                var resolution = (Size)e.NewValue;
                ((d as IRenderable).RenderCore as ShadowMapCore).Width = (int)resolution.Width;
                ((d as IRenderable).RenderCore as ShadowMapCore).Height = (int)resolution.Height;
            }));


        /// <summary>
        /// The bias property
        /// </summary>
        public static readonly DependencyProperty BiasProperty =
                DependencyProperty.Register("Bias", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.0015, (d, e)=>
                {
                    ((d as IRenderable).RenderCore as ShadowMapCore).Bias = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The intensity property
        /// </summary>
        public static readonly DependencyProperty IntensityProperty =
                DependencyProperty.Register("Intensity", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.5, (d, e)=>
                {
                    ((d as IRenderable).RenderCore as ShadowMapCore).Intensity = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The light camera property
        /// </summary>
        public static readonly DependencyProperty LightCameraProperty =
                DependencyProperty.Register("LightCamera", typeof(ProjectionCamera), typeof(ShadowMap3D), new PropertyMetadata(null, (d, e) =>
                {
                    (d as ShadowMap3D).lightCamera = (ProjectionCamera)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public Size Resolution
        {
            get { return (Size)this.GetValue(ResolutionProperty); }
            set { this.SetValue(ResolutionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Bias
        {
            get { return (double)this.GetValue(BiasProperty); }
            set { this.SetValue(BiasProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Intensity
        {
            get { return (double)this.GetValue(IntensityProperty); }
            set { this.SetValue(IntensityProperty, value); }
        }
        /// <summary>
        /// Distance of the directional light from origin
        /// </summary>
        public ProjectionCamera LightCamera
        {
            get { return (ProjectionCamera)this.GetValue(LightCameraProperty); }
            set { this.SetValue(LightCameraProperty, value); }
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new ShadowMapCore();
        }

        private ShadowMapCore shadowCore;

        private readonly OrthographicCameraCore orthoCamera = new OrthographicCameraCore() { NearPlaneDistance = 1, FarPlaneDistance = 500 };
        private readonly PerspectiveCameraCore persCamera = new PerspectiveCameraCore() { NearPlaneDistance = 1, FarPlaneDistance = 500 };
        private ProjectionCamera lightCamera;
        private readonly Stack<IEnumerator<IRenderable>> stackCache = new Stack<IEnumerator<IRenderable>>();
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            var c = core as ShadowMapCore;
            //c.FactorPCF = (float)FactorPCF;
            c.Intensity = (float)Intensity;
            c.Bias = (float)Bias;
            c.Width = (int)(Resolution.Width);
            c.Height = (int)(Resolution.Height);
        }
        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return true if attached
        /// </returns>
        protected override bool OnAttach(IRenderHost host)
        {
            base.OnAttach(host);
            shadowCore = RenderCore as ShadowMapCore;            
            return true;
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// <para>Default returns <see cref="Element3DCore.IsAttached" /> &amp;&amp; <see cref="Visibility" /> == <see cref="Visibility.Visible" /></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            if(base.CanRender(context) && RenderHost.IsShadowMapEnabled && !context.IsShadowPass)
            {
                CameraCore camera = lightCamera == null ? null : lightCamera;
                if (lightCamera == null)
                {
                    var root = context.RenderHost.Viewport.Renderables.Take(Constants.MaxLights)
                        .PreorderDFT(x => x is ILight3D && x.IsRenderable 
                        && (((ILight3D)x).LightType == LightType.Directional || ((ILight3D)x).LightType == LightType.Spot), stackCache)
                        .Take(1).Select(x=>x as ILight3D);
                    foreach (var light in root)
                    {
                        if (light.LightType == LightType.Directional)
                        {
                            var dlight = ((IRenderable)light).RenderCore as DirectionalLightCore;
                            var dir = Vector4.Transform(dlight.Direction.ToVector4(0), dlight.ModelMatrix).Normalized();
                            var pos = -100 * dir;
                            orthoCamera.LookDirection = new Vector3(dir.X, dir.Y, dir.Z);
                            orthoCamera.Position = new Vector3(pos.X, pos.Y, pos.Z);
                            orthoCamera.UpDirection = Vector3.UnitZ;
                            orthoCamera.Width = 50;
                            camera = orthoCamera;
                        }
                        else if (light.LightType == LightType.Spot)
                        {
                            var splight = ((IRenderable)light).RenderCore as SpotLightCore;
                            persCamera.Position = (splight.Position + splight.ModelMatrix.Row4.ToVector3());
                            var look = Vector4.Transform(splight.Direction.ToVector4(0), splight.ModelMatrix);
                            persCamera.LookDirection = new Vector3(look.X, look.Y, look.Z);
                            persCamera.FarPlaneDistance = (float)splight.Range;
                            persCamera.FieldOfView = (float)splight.OuterAngle;
                            persCamera.UpDirection = Vector3.UnitZ;
                            camera = persCamera;
                        }
                    }
                }
                if (camera == null)
                {
                    shadowCore.FoundLightSource = false;
                }
                else
                {
                    shadowCore.FoundLightSource = true;
                    shadowCore.LightViewProjectMatrix = camera.GetViewMatrix() * camera.GetProjectionMatrix(shadowCore.Width / shadowCore.Height);                    
                }
                return true;
            }
            else { return false; }
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
            return false;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            throw new System.NotImplementedException();
        }
    }
}

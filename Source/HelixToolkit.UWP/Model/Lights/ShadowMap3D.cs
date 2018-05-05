/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using Cameras;
    using global::SharpDX;
    using Model;
    using Model.Scene;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    public class ShadowMap3D : Element3D
    {
        
        /// <summary>
        /// The resolution property
        /// </summary>
        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Size), typeof(ShadowMap3D), new PropertyMetadata(new Size(1024, 1024), (d, e) =>
            {
                var resolution = (Size)e.NewValue;
                ((d as Element3DCore).SceneNode as ShadowMapNode).Resolution = new Size2((int)resolution.Width, (int)resolution.Height);
            }));


        /// <summary>
        /// The bias property
        /// </summary>
        public static readonly DependencyProperty BiasProperty =
                DependencyProperty.Register("Bias", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.0015, (d, e)=>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).Bias = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The intensity property
        /// </summary>
        public static readonly DependencyProperty IntensityProperty =
                DependencyProperty.Register("Intensity", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.5, (d, e)=>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).Intensity = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The light camera property
        /// </summary>
        public static readonly DependencyProperty LightCameraProperty =
                DependencyProperty.Register("LightCamera", typeof(ProjectionCamera), typeof(ShadowMap3D), new PropertyMetadata(null, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).LightCamera = (e.NewValue as ProjectionCamera).CameraInternal as ProjectionCameraCore;
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

        protected override SceneNode OnCreateSceneNode()
        {
            return new ShadowMapNode();
        }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is ShadowMapNode n)
            {
                n.Intensity = (float)Intensity;
                n.Bias = (float)Bias;
                n.Resolution = new Size2((int)(Resolution.Width), (int)(Resolution.Height));
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}

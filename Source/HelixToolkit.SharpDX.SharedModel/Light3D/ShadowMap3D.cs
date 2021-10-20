/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
#if NETFX_CORE
using  Windows.UI.Xaml;
using Windows.Foundation;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Windows.Foundation;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Cameras;
    using Model.Scene;
#endif
    /// <summary>
    /// 
    /// </summary>
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
                DependencyProperty.Register("Bias", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.0015, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).Bias = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The intensity property
        /// </summary>
        public static readonly DependencyProperty IntensityProperty =
                DependencyProperty.Register("Intensity", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(0.5, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).Intensity = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// The light camera property
        /// </summary>
        public static readonly DependencyProperty LightCameraProperty =
                DependencyProperty.Register("LightCamera", typeof(IProjectionCameraModel), typeof(ShadowMap3D), new PropertyMetadata(null, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ShadowMapNode).LightCamera = (e.NewValue as Camera)?.CameraInternal as ProjectionCameraCore;
                }));

        /// <summary>
        /// The distance property
        /// </summary>
        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(200.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ShadowMapNode).Distance = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// The ortho width property
        /// </summary>
        public static readonly DependencyProperty OrthoWidthProperty =
            DependencyProperty.Register("OrthoWidth", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(100.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ShadowMapNode).OrthoWidth = (float)(double)e.NewValue;
            }));


        /// <summary>
        /// The far field distance property
        /// </summary>
        public static readonly DependencyProperty FarFieldDistanceProperty =
            DependencyProperty.Register("FarFieldDistance", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(500.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ShadowMapNode).FarField = (float)(double)e.NewValue;
            }));


        /// <summary>
        /// The near field distance property
        /// </summary>
        public static readonly DependencyProperty NearFieldDistanceProperty =
            DependencyProperty.Register("NearFieldDistance", typeof(double), typeof(ShadowMap3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ShadowMapNode).NearField = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty AutoCoverCompleteSceneProperty =
            DependencyProperty.Register("AutoCoverCompleteScene", typeof(bool), typeof(ShadowMap3D), new PropertyMetadata(false, (d, e) =>
            {

            }));




        // Using a DependencyProperty as the backing store for IsSceneDynamic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSceneDynamicProperty =
            DependencyProperty.Register("IsSceneDynamic", typeof(bool), typeof(ShadowMap3D), new PropertyMetadata(false, (d, e) =>
            {

            }));



        /// <summary>
        /// Gets or sets the distance for shadow caster.
        /// </summary>
        /// <value>
        /// The distance of the shadow caster
        /// </value>
        public double Distance
        {
            get
            {
                return (double)GetValue(DistanceProperty);
            }
            set
            {
                SetValue(DistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the orthographic camera used in shadow caster.
        /// </summary>
        /// <value>
        /// The width of the orthographic matrix.
        /// </value>
        public double OrthoWidth
        {
            get
            {
                return (double)GetValue(OrthoWidthProperty);
            }
            set
            {
                SetValue(OrthoWidthProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public Size Resolution
        {
            get
            {
                return (Size)this.GetValue(ResolutionProperty);
            }
            set
            {
                this.SetValue(ResolutionProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Bias
        {
            get
            {
                return (double)this.GetValue(BiasProperty);
            }
            set
            {
                this.SetValue(BiasProperty, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Intensity
        {
            get
            {
                return (double)this.GetValue(IntensityProperty);
            }
            set
            {
                this.SetValue(IntensityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the near field distance.
        /// </summary>
        /// <value>
        /// The near field distance.
        /// </value>
        public double NearFieldDistance
        {
            get
            {
                return (double)GetValue(NearFieldDistanceProperty);
            }
            set
            {
                SetValue(NearFieldDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the far field distance.
        /// </summary>
        /// <value>
        /// The far field distance.
        /// </value>
        public double FarFieldDistance
        {
            get
            {
                return (double)GetValue(FarFieldDistanceProperty);
            }
            set
            {
                SetValue(FarFieldDistanceProperty, value);
            }
        }
        /// <summary>
        /// Distance of the directional light from origin
        /// </summary>
        public IProjectionCameraModel LightCamera
        {
            get
            {
                return (IProjectionCameraModel)this.GetValue(LightCameraProperty);
            }
            set
            {
                this.SetValue(LightCameraProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shadow map should automatically cover complete scene. Only effective with directional light.
        /// <para>Limitation: Currently unable to properly cover BoneSkinned model animation.</para>
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automaticcally cover complete scene]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCoverCompleteScene
        {
            get
            {
                return (bool)GetValue(AutoCoverCompleteSceneProperty);
            }
            set
            {
                SetValue(AutoCoverCompleteSceneProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the scene is dynamic. Only effective if <see cref="AutoCoverCompleteScene"/> is true.
        /// <para>Setting to true will force shadow map to update the shadow camera for each frame. May impact the performance.</para>
        /// </summary>
        /// <value>
        ///   <c>true</c> if scene is dynamic; otherwise, <c>false</c>.
        /// </value>
        public bool IsSceneDynamic
        {
            get
            {
                return (bool)GetValue(IsSceneDynamicProperty);
            }
            set
            {
                SetValue(IsSceneDynamicProperty, value);
            }
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
                n.Distance = (float)Distance;
                n.OrthoWidth = (float)OrthoWidth;
                n.FarField = (float)FarFieldDistance;
                n.NearField = (float)NearFieldDistance;
                n.AutoCoverCompleteScene = AutoCoverCompleteScene;
                n.IsSceneDynamic = IsSceneDynamic;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}

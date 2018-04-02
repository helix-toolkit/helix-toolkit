using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    public partial class Viewport3DX
    {
        /// <summary>
        /// The is deferred shading enabled propery
        /// </summary>
        public static readonly DependencyProperty IsShadowMappingEnabledProperty = DependencyProperty.Register(
            "IsShadowMappingEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false,
                (s, e) =>
                {
                    if (((Viewport3DX)s).renderHostInternal != null)
                        ((Viewport3DX)s).renderHostInternal.IsShadowMapEnabled = (bool)e.NewValue;
                }));


        public bool IsShadowMappingEnabled
        {
            set
            {
                SetValue(IsShadowMappingEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsShadowMappingEnabledProperty);
            }
        }

        /// <summary>
        /// The Render Technique property
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(IRenderTechnique), typeof(Viewport3DX), new PropertyMetadata(null,
                (s, e) => 
                {
                    if (((Viewport3DX)s).renderHostInternal != null)
                        ((Viewport3DX)s).renderHostInternal.RenderTechnique = e.NewValue as IRenderTechnique;
                }));

        public IRenderTechnique RenderTechnique
        {
            set
            {
                SetValue(RenderTechniqueProperty, value);
            }
            get
            {
                return (IRenderTechnique)GetValue(RenderTechniqueProperty);
            }
        }

        /// <summary>
        /// The EffectsManager property.
        /// </summary>
        public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
            "EffectsManager", typeof(IEffectsManager), typeof(Viewport3DX), new PropertyMetadata(
                null,
                (s, e) => {
                    if (((Viewport3DX)s).renderHostInternal != null)
                        ((Viewport3DX)s).renderHostInternal.EffectsManager = e.NewValue as IEffectsManager;
                }));


        public IEffectsManager EffectsManager
        {
            set
            {
                SetValue(EffectsManagerProperty, value);
            }
            get
            {
                return (IEffectsManager)GetValue(EffectsManagerProperty);
            }
        }

        /// <summary>
        /// The camera property
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera",
            typeof(Camera),
            typeof(Viewport3DX),
            new PropertyMetadata(null));


        public Camera Camera
        {
            set
            {
                SetValue(CameraProperty, value);
            }
            get
            {
                return (Camera)GetValue(CameraProperty);
            }
        }

        /// <summary>
        /// Gets or sets the render host internal.
        /// </summary>
        /// <value>
        /// The render host internal.
        /// </value>
        protected IRenderHost renderHostInternal;
    }
}

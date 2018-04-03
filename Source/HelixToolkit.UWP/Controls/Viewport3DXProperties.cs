/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Windows.UI;
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
                    var viewport = s as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                        viewport.renderHostInternal.IsShadowMapEnabled = (bool)e.NewValue;
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
                    var viewport = s as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                        viewport.renderHostInternal.RenderTechnique = e.NewValue as IRenderTechnique;
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
                    var viewport = s as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                        viewport.renderHostInternal.EffectsManager = e.NewValue as IEffectsManager;
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
        /// Background Color property.this.RenderHost
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color), typeof(Viewport3DX),
            new PropertyMetadata(Colors.White, (s, e) =>
            {
                var viewport = s as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.ClearColor = ((Color)e.NewValue).ToColor4();
                }
            }));

        public Color BackgroundColor
        {
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
            get
            {
                return (Color)GetValue(BackgroundColorProperty);
            }
        }


        /// <summary>
        /// The message text property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(
            "MessageText", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        /// <value>
        /// The message text.
        /// </value>
        public string MessageText
        {
            set
            {
                SetValue(MessageTextProperty, value);
            }
            get
            {
                return (string)GetValue(MessageTextProperty);
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

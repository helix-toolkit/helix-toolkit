/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using Windows.UI;
using Windows.UI.Xaml;
using Vector3 = SharpDX.Vector3;

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
                (s, e) =>
                {
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
        /// The model up direction property
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty = DependencyProperty.Register(
            "ModelUpDirection", typeof(Vector3), typeof(Viewport3DX), new PropertyMetadata(new Vector3(0, 1, 0)));

        /// <summary>
        /// Gets or sets the model up direction.
        /// </summary>
        /// <value>
        /// The model up direction.
        /// </value>
        public Vector3 ModelUpDirection
        {
            get
            {
                return (Vector3)this.GetValue(ModelUpDirectionProperty);
            }

            set
            {
                this.SetValue(ModelUpDirectionProperty, value);
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

        #region Coordinate System

        /// <summary>
        /// The show coordinate system property.
        /// </summary>
        public static readonly DependencyProperty ShowCoordinateSystemProperty = DependencyProperty.Register(
            "ShowCoordinateSystem", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The coordinate system horizontal position property. Relative to viewport center
        /// <para>Default: -0.8</para>
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemHorizontalPositionProperty = DependencyProperty.Register(
                "CoordinateSystemHorizontalPosition",
                typeof(double),
                typeof(Viewport3DX),
                new PropertyMetadata(-0.8));

        /// <summary>
        /// The coordinate system label foreground property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelForegroundProperty = DependencyProperty.Register(
                "CoordinateSystemLabelForeground",
                typeof(Color),
                typeof(Viewport3DX),
                new PropertyMetadata(Colors.DarkGray));

        /// <summary>
        /// The coordinate system label X property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelXProperty = DependencyProperty.Register(
                "CoordinateSystemLabelX", typeof(string), typeof(Viewport3DX), new PropertyMetadata("X"));

        /// <summary>
        /// The coordinate system label Y property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelYProperty = DependencyProperty.Register(
                "CoordinateSystemLabelY", typeof(string), typeof(Viewport3DX), new PropertyMetadata("Y"));

        /// <summary>
        /// The coordinate system label Z property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelZProperty = DependencyProperty.Register(
                "CoordinateSystemLabelZ", typeof(string), typeof(Viewport3DX), new PropertyMetadata("Z"));

        /// <summary>
        /// The coordinate system vertical position property. Relative to viewport center.
        /// <para>Default: -0.8</para>
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemVerticalPositionProperty = DependencyProperty.Register(
                "CoordinateSystemVerticalPosition",
                typeof(double),
                typeof(Viewport3DX),
                new PropertyMetadata(-0.8));

        /// <summary>
        /// The coordinate system size property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemSizeProperty = DependencyProperty.Register(
                "CoordinateSystemSize", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the horizontal position of the coordinate system viewport. Relative to the viewport center.
        /// <para>Default: -0.8</para>
        /// </summary>
        /// <value>
        /// The horizontal position.
        /// </value>
        public double CoordinateSystemHorizontalPosition
        {
            get
            {
                return (double)this.GetValue(CoordinateSystemHorizontalPositionProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemHorizontalPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the coordinate system label.
        /// </summary>
        /// <value>
        /// The color of the coordinate system label.
        /// </value>
        public Color CoordinateSystemLabelForeground
        {
            get
            {
                return (Color)this.GetValue(CoordinateSystemLabelForegroundProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemLabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system label X.
        /// </summary>
        /// <value>
        /// The coordinate system label X.
        /// </value>
        public string CoordinateSystemLabelX
        {
            get
            {
                return (string)this.GetValue(CoordinateSystemLabelXProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemLabelXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system label Y.
        /// </summary>
        /// <value>
        /// The coordinate system label Y.
        /// </value>
        public string CoordinateSystemLabelY
        {
            get
            {
                return (string)this.GetValue(CoordinateSystemLabelYProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemLabelYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the coordinate system label Z.
        /// </summary>
        /// <value>
        /// The coordinate system label Z.
        /// </value>
        public string CoordinateSystemLabelZ
        {
            get
            {
                return (string)this.GetValue(CoordinateSystemLabelZProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemLabelZProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical position of the coordinate system viewport. Relative to the viewport center
        /// <para>Default: -0.8</para>
        /// </summary>
        /// <value>
        /// The vertical position.
        /// </value>
        public double CoordinateSystemVerticalPosition
        {
            get
            {
                return (double)this.GetValue(CoordinateSystemVerticalPositionProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemVerticalPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the coordinate system viewport.
        /// </summary>
        /// <value>
        /// The width of the coordinate system viewport.
        /// </value>
        public double CoordinateSystemSize
        {
            get
            {
                return (double)this.GetValue(CoordinateSystemSizeProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the coordinate system.
        /// </summary>
        /// <value>
        /// <c>true</c> if coordinate system should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowCoordinateSystem
        {
            get
            {
                return (bool)this.GetValue(ShowCoordinateSystemProperty);
            }

            set
            {
                this.SetValue(ShowCoordinateSystemProperty, value);
            }
        }

        #endregion Coordinate System

        #region ViewCube

        /// <summary>
        /// The show view cube property.
        /// </summary>
        public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
            "ShowViewCube", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// The view cube texture. It must be a 6x1 (ex: 600x100) ratio image. You can also use BitmapExtension.CreateViewBoxBitmapSource to create
        /// </summary>
        public static readonly DependencyProperty ViewCubeTextureProperty = DependencyProperty.Register(
                "ViewCubeTexture", typeof(System.IO.Stream), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The view cube horizontal position property. Relative to viewport center.
        /// <para>Default: 0.8</para>
        /// </summary>
        public static readonly DependencyProperty ViewCubeHorizontalPositionProperty = DependencyProperty.Register(
                "ViewCubeHorizontalPosition",
                typeof(double),
                typeof(Viewport3DX),
                new PropertyMetadata(0.8));

        /// <summary>
        /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsViewCubeEdgeClicksEnabledProperty =
            DependencyProperty.Register("IsViewCubeEdgeClicksEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The view cube vertical position property. Relative to viewport center.
        /// <para>Default: -0.8</para>
        /// </summary>
        public static readonly DependencyProperty ViewCubeVerticalPositionProperty = DependencyProperty.Register(
                "ViewCubeVerticalPosition",
                typeof(double),
                typeof(Viewport3DX),
                new PropertyMetadata(-0.8));

        /// <summary>
        /// The view cube size property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeSizeProperty = DependencyProperty.Register(
            "ViewCubeSize", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets a value indicating whether to show the view cube.
        /// </summary>
        /// <value>
        /// <c>true</c> if the view cube should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowViewCube
        {
            get
            {
                return (bool)this.GetValue(ShowViewCubeProperty);
            }

            set
            {
                this.SetValue(ShowViewCubeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube texture;
        /// The view cube texture. It must be a 6x1 (ex: 600x100) ratio image. You can also use BitmapExtension.CreateViewBoxBitmapSource to create
        /// </summary>
        /// <value>
        /// The view cube texture.
        /// </value>
        public System.IO.Stream ViewCubeTexture
        {
            get
            {
                return (System.IO.Stream)this.GetValue(ViewCubeTextureProperty);
            }

            set
            {
                this.SetValue(ViewCubeTextureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the view cube viewport. Relative to viewport center
        /// <para>Default: 0.8</para>
        /// </summary>
        /// <value>
        /// The horizontal position.
        /// </value>
        public double ViewCubeHorizontalPosition
        {
            get
            {
                return (double)this.GetValue(ViewCubeHorizontalPositionProperty);
            }

            set
            {
                this.SetValue(ViewCubeHorizontalPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets if the view cube edge clickable.
        /// </summary>
        /// <value>
        /// Boolean for enable or disable.
        /// </value>
        public bool IsViewCubeEdgeClicksEnabled
        {
            get { return (bool)GetValue(IsViewCubeEdgeClicksEnabledProperty); }
            set { SetValue(IsViewCubeEdgeClicksEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical position of view cube viewport. Relative to viewport center
        /// <para>Default: -0.8</para>
        /// </summary>
        /// <value>
        /// The vertical position.
        /// </value>
        public double ViewCubeVerticalPosition
        {
            get
            {
                return (double)this.GetValue(ViewCubeVerticalPositionProperty);
            }

            set
            {
                this.SetValue(ViewCubeVerticalPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the view cube viewport.
        /// </summary>
        /// <value>
        /// The width of the view cube viewport.
        /// </value>
        public double ViewCubeSize
        {
            get
            {
                return (double)this.GetValue(ViewCubeSizeProperty);
            }

            set
            {
                this.SetValue(ViewCubeSizeProperty, value);
            }
        }

        #endregion ViewCube
    }
}
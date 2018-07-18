/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using System.Numerics;

namespace HelixToolkit.UWP
{
    using Utilities;
    public partial class Viewport3DX
    {
        #region Events

        public event EventHandler<MouseDown3DEventArgs> OnMouse3DDown;

        public event EventHandler<MouseUp3DEventArgs> OnMouse3DUp;

        public event EventHandler<MouseMove3DEventArgs> OnMouse3DMove;

        /// <summary>
        /// Fired whenever an exception occurred at rendering subsystem.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> RenderExceptionOccurred;

        #endregion Events

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
            new PropertyMetadata(null, (d, e) =>
            {
                var m = d as Viewport3DX;
                if (e.OldValue != null)
                {
                    (e.OldValue as Camera).CameraInternal.PropertyChanged -= m.CameraInternal_PropertyChanged;
                }
                if (e.NewValue != null)
                {
                    (e.NewValue as Camera).CameraInternal.PropertyChanged += m.CameraInternal_PropertyChanged;
                }
            }));

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
        /// The default camera property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the default camera.
        /// </summary>
        /// <value>
        /// The default camera.
        /// </value>
        public ProjectionCamera DefaultCamera
        {
            get
            {
                return (ProjectionCamera)this.GetValue(DefaultCameraProperty);
            }

            set
            {
                this.SetValue(DefaultCameraProperty, value);
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
            "ModelUpDirection", typeof(Vector3), typeof(Viewport3DX), new PropertyMetadata(new Vector3(0, 1, 0), (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ModelUpDirection = (Vector3)e.NewValue;
            }));

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

        /// <summary>
        /// The camera mode property
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(Viewport3DX), new PropertyMetadata(CameraMode.Inspect, (d, e) =>
            {
                (d as Viewport3DX).CameraController.CameraMode = (CameraMode)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the camera mode.
        /// </summary>
        /// <value>
        /// The camera mode.
        /// </value>
        public CameraMode CameraMode
        {
            get
            {
                return (CameraMode)this.GetValue(CameraModeProperty);
            }

            set
            {
                this.SetValue(CameraModeProperty, value);
            }
        }

        /// <summary>
        /// The camera rotation mode property
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(Viewport3DX),
                new PropertyMetadata(CameraRotationMode.Turntable, (d, e) =>
                {
                    (d as Viewport3DX).CameraController.CameraRotationMode = (CameraRotationMode)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the camera rotation mode.
        /// </summary>
        /// <value>
        /// The camera rotation mode.
        /// </value>
        public CameraRotationMode CameraRotationMode
        {
            get
            {
                return (CameraRotationMode)this.GetValue(CameraRotationModeProperty);
            }

            set
            {
                this.SetValue(CameraRotationModeProperty, value);
            }
        }

        /// <summary>
        /// The left right rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightRotationSensitivityProperty = DependencyProperty.Register(
            "LeftRightRotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.LeftRightRotationSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the left and right keys.
        /// </summary>
        /// <value>
        /// The rotation sensitivity.
        /// </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double LeftRightRotationSensitivity
        {
            get
            {
                return (double)this.GetValue(LeftRightRotationSensitivityProperty);
            }

            set
            {
                this.SetValue(LeftRightRotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The left right pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightPanSensitivityProperty = DependencyProperty.Register(
            "LeftRightPanSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.LeftRightPanSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the sensitivity for pan by the left and right keys.
        /// </summary>
        /// <value>
        /// The pan sensitivity.
        /// </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double LeftRightPanSensitivity
        {
            get
            {
                return (double)this.GetValue(LeftRightPanSensitivityProperty);
            }

            set
            {
                this.SetValue(LeftRightPanSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The rotation sensitivity property
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.Register(
            "RotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.RotationSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the rotation sensitivity.
        /// </summary>
        /// <value>
        /// The rotation sensitivity.
        /// </value>
        public double RotationSensitivity
        {
            get
            {
                return (double)this.GetValue(RotationSensitivityProperty);
            }

            set
            {
                this.SetValue(RotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The up down Pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownPanSensitivityProperty = DependencyProperty.Register(
                "UpDownPanSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    (d as Viewport3DX).CameraController.UpDownPanSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sensitivity for pan by the up and down keys.
        /// </summary>
        /// <value>
        /// The pan sensitivity.
        /// </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double UpDownPanSensitivity
        {
            get
            {
                return (double)this.GetValue(UpDownPanSensitivityProperty);
            }

            set
            {
                this.SetValue(UpDownPanSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The up down rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownRotationSensitivityProperty = DependencyProperty.Register(
                "UpDownRotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    (d as Viewport3DX).CameraController.UpDownRotationSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the up and down keys.
        /// </summary>
        /// <value>
        /// The rotation sensitivity.
        /// </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double UpDownRotationSensitivity
        {
            get
            {
                return (double)this.GetValue(UpDownRotationSensitivityProperty);
            }

            set
            {
                this.SetValue(UpDownRotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The zoom sensitivity property
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty = DependencyProperty.Register(
            "ZoomSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the zoom sensitivity.
        /// </summary>
        /// <value>
        /// The zoom sensitivity.
        /// </value>
        public double ZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(ZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(ZoomSensitivityProperty, value);
            }
        }

        /// <summary>
        /// The spin release time property
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty = DependencyProperty.Register(
            "SpinReleaseTime", typeof(int), typeof(Viewport3DX), new PropertyMetadata(200, (d, e) =>
            {
                (d as Viewport3DX).CameraController.SpinReleaseTime = (int)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the spin release time in milliseconds (maximum allowed time to start a spin).
        /// </summary>
        /// <value>
        /// The spin release time (in milliseconds).
        /// </value>
        public int SpinReleaseTime
        {
            get
            {
                return (int)this.GetValue(SpinReleaseTimeProperty);
            }

            set
            {
                this.SetValue(SpinReleaseTimeProperty, value);
            }
        }

        /// <summary>
        /// Rotate around this fixed rotation point only.<see cref="FixedRotationPointEnabledProperty"/>
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointProperty = DependencyProperty.Register(
            "FixedRotationPoint", typeof(Vector3), typeof(Viewport3DX), new PropertyMetadata(new Vector3(), (d, e) =>
            {
                (d as Viewport3DX).CameraController.FixedRotationPoint = (Vector3)e.NewValue;
            }));

        /// <summary>
        /// Rotate around this fixed rotation point only.<see cref="FixedRotationPointEnabled"/>
        /// </summary>
        public Vector3 FixedRotationPoint
        {
            set
            {
                SetValue(FixedRotationPointProperty, value);
            }
            get
            {
                return (Vector3)GetValue(FixedRotationPointProperty);
            }
        }

        /// <summary>
        /// Enable fixed rotation mode and use FixedRotationPoint for rotation. Only works under CameraMode = Inspect
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointEnabledProperty = DependencyProperty.Register(
            "FixedRotationPointEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                (d as Viewport3DX).CameraController.FixedRotationPointEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Enable fixed rotation mode and use <see cref="FixedRotationPoint"/>  for rotation. Only works under <see cref="CameraMode"/> = Inspect
        /// </summary>
        public bool FixedRotationPointEnabled
        {
            set
            {
                SetValue(FixedRotationPointEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(FixedRotationPointEnabledProperty);
            }
        }

        /// <summary>
        /// The is pan enabled property
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsPanEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if pan is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }

            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty =
            DependencyProperty.Register(
                "IsInertiaEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsInertiaEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
        /// </summary>
        /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
        public bool IsInertiaEnabled
        {
            get
            {
                return (bool)this.GetValue(IsInertiaEnabledProperty);
            }

            set
            {
                this.SetValue(IsInertiaEnabledProperty, value);
            }
        }

        /// <summary>
        /// The is rotation enabled property
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty = DependencyProperty.Register(
            "IsRotationEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsRotationEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether rotation is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if rotation is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsRotationEnabled
        {
            get
            {
                return (bool)this.GetValue(IsRotationEnabledProperty);
            }

            set
            {
                this.SetValue(IsRotationEnabledProperty, value);
            }
        }

        /// <summary>
        /// The enable touch rotate property
        /// </summary>
        public static readonly DependencyProperty IsTouchRotateEnabledProperty =
            DependencyProperty.Register("IsTouchRotateEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.EnableTouchRotate = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [enable one finger touch rotate].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable touch rotate]; otherwise, <c>false</c>.
        /// </value>
        public bool IsTouchRotateEnabled
        {
            get { return (bool)GetValue(IsTouchRotateEnabledProperty); }
            set { SetValue(IsTouchRotateEnabledProperty, value); }
        }

        /// <summary>
        /// The IsTouchZoomEnabled property.
        /// </summary>
        public static readonly DependencyProperty IsPinchZoomEnabledProperty = DependencyProperty.Register(
            "IsPinchZoomEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.EnablePinchZoom = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether two finger pinch zoom is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if pinch zoom is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool IsPinchZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPinchZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsPinchZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// The enable touch rotate property
        /// </summary>
        public static readonly DependencyProperty IsThreeFingerPanningEnabledProperty =
            DependencyProperty.Register("IsThreeFingerPanningEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.EnableThreeFingerPan = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [enable three finger panning].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable three finger panning]; otherwise, <c>false</c>.
        /// </value>
        public bool IsThreeFingerPanningEnabled
        {
            get { return (bool)GetValue(IsThreeFingerPanningEnabledProperty); }
            set { SetValue(IsThreeFingerPanningEnabledProperty, value); }
        }

        /// <summary>
        /// The is zoom enabled property
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsZoomEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether zoom is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if zoom is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// The pan cursor property
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(CoreCursorType), typeof(Viewport3DX), new PropertyMetadata(CoreCursorType.Hand, (d, e) =>
            {
                (d as Viewport3DX).CameraController.PanCursor = (CoreCursorType)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the pan cursor.
        /// </summary>
        /// <value>
        /// The pan cursor.
        /// </value>
        public CoreCursorType PanCursor
        {
            get
            {
                return (CoreCursorType)this.GetValue(PanCursorProperty);
            }

            set
            {
                this.SetValue(PanCursorProperty, value);
            }
        }

        /// <summary>
        /// The rotate cursor property
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(CoreCursorType), typeof(Viewport3DX), new PropertyMetadata(CoreCursorType.SizeAll, (d, e) =>
            {
                (d as Viewport3DX).CameraController.RotateCursor = (CoreCursorType)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the rotate cursor.
        /// </summary>
        /// <value>
        /// The rotate cursor.
        /// </value>
        public CoreCursorType RotateCursor
        {
            get
            {
                return (CoreCursorType)this.GetValue(RotateCursorProperty);
            }

            set
            {
                this.SetValue(RotateCursorProperty, value);
            }
        }

        /// <summary>
        /// The rotate around mouse down point property
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty = DependencyProperty.Register(
            "RotateAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                (d as Viewport3DX).CameraController.RotateAroundMouseDownPoint = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value>
        /// <c>true</c> if rotating around mouse down point; otherwise, <c>false</c>.
        /// </value>
        public bool RotateAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(RotateAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(RotateAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        /// The zoom around mouse down point property
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty = DependencyProperty.Register(
            "ZoomAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomAroundMouseDownPoint = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around the mouse down point.
        /// </summary>
        /// <value>
        /// <c>true</c> if zooming around the mouse down point; otherwise, <c>false</c>.
        /// </value>
        public bool ZoomAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(ZoomAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(ZoomAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        /// The is change field of view enabled property
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty = DependencyProperty.Register(
            "IsChangeFieldOfViewEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsChangeFieldOfViewEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether change field of view is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if change field of view is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsChangeFieldOfViewEnabled
        {
            get
            {
                return (bool)this.GetValue(IsChangeFieldOfViewEnabledProperty);
            }

            set
            {
                this.SetValue(IsChangeFieldOfViewEnabledProperty, value);
            }
        }

        /// <summary>
        /// The maximum field of view property
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty = DependencyProperty.Register(
            "MaximumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(120.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.MaximumFieldOfView = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the maximum field of view.
        /// </summary>
        /// <value>
        /// The maximum field of view.
        /// </value>
        public double MaximumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MaximumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MaximumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        /// The minimum field of view property
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty = DependencyProperty.Register(
            "MinimumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(10.0, (d, e) =>
            {
                (d as Viewport3DX).CameraController.MinimumFieldOfView = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the minimum field of view.
        /// </summary>
        /// <value>
        /// The minimum field of view.
        /// </value>
        public double MinimumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MinimumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MinimumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        /// The zoom cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(CoreCursorType), typeof(Viewport3DX), new PropertyMetadata(CoreCursorType.SizeNorthSouth, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomCursor = (CoreCursorType)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the zoom cursor.
        /// </summary>
        /// <value>
        /// The zoom cursor.
        /// </value>
        public CoreCursorType ZoomCursor
        {
            get
            {
                return (CoreCursorType)this.GetValue(ZoomCursorProperty);
            }

            set
            {
                this.SetValue(ZoomCursorProperty, value);
            }
        }

        /// <summary>
        /// The far zoom distance limit property.
        /// </summary>
        public static readonly DependencyProperty ZoomDistanceLimitFarProperty = DependencyProperty.Register(
            "ZoomDistanceLimitFar", typeof(double), typeof(Viewport3DX), new PropertyMetadata(double.PositiveInfinity, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomDistanceLimitFar = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating the far distance limit for zoom.
        /// </summary>
        public double ZoomDistanceLimitFar
        {
            get
            {
                return (double)this.GetValue(ZoomDistanceLimitFarProperty);
            }

            set
            {
                this.SetValue(ZoomDistanceLimitFarProperty, value);
            }
        }

        /// <summary>
        /// The near zoom distance limit property.
        /// </summary>
        public static readonly DependencyProperty ZoomDistanceLimitNearProperty = DependencyProperty.Register(
            "ZoomDistanceLimitNear", typeof(double), typeof(Viewport3DX), new PropertyMetadata(0.001, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomDistanceLimitNear = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating the near distance limit for zoom.
        /// </summary>
        public double ZoomDistanceLimitNear
        {
            get
            {
                return (double)this.GetValue(ZoomDistanceLimitNearProperty);
            }

            set
            {
                this.SetValue(ZoomDistanceLimitNearProperty, value);
            }
        }

        /// <summary>
        /// The zoom rectangle cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty = DependencyProperty.Register(
            "ZoomRectangleCursor", typeof(CoreCursorType), typeof(Viewport3DX), new PropertyMetadata(CoreCursorType.SizeNorthwestSoutheast, (d, e) =>
            {
                (d as Viewport3DX).CameraController.ZoomRectangleCursor = (CoreCursorType)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the zoom rectangle cursor.
        /// </summary>
        /// <value>
        /// The zoom rectangle cursor.
        /// </value>
        public CoreCursorType ZoomRectangleCursor
        {
            get
            {
                return (CoreCursorType)this.GetValue(ZoomRectangleCursorProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleCursorProperty, value);
            }
        }

        /// <summary>
        ///   The is move enabled property.
        /// </summary>
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
            "IsMoveEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).CameraController.IsMoveEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        ///   Gets or sets a value indicating whether move is enabled.
        /// </summary>
        /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
        public bool IsMoveEnabled
        {
            get
            {
                return (bool)this.GetValue(IsMoveEnabledProperty);
            }

            set
            {
                this.SetValue(IsMoveEnabledProperty, value);
            }
        }

        /// <summary>
        /// The camera inertia factor property.
        /// </summary>
        public static readonly DependencyProperty CameraInertiaFactorProperty = DependencyProperty.Register(
            "CameraInertiaFactor", typeof(double), typeof(Viewport3DX), new PropertyMetadata(0.93, (d, e) =>
            {
                (d as Viewport3DX).CameraController.InertiaFactor = (double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the camera inertia factor.
        /// </summary>
        /// <value>
        /// The camera inertia factor.
        /// </value>
        public double CameraInertiaFactor
        {
            get
            {
                return (double)this.GetValue(CameraInertiaFactorProperty);
            }

            set
            {
                this.SetValue(CameraInertiaFactorProperty, value);
            }
        }

        /// <summary>
        /// The infinite spin property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                (d as Viewport3DX).CameraController.InfiniteSpin = (bool)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets a value indicating whether infinite spin is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if infinite spin is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool InfiniteSpin
        {
            get
            {
                return (bool)this.GetValue(InfiniteSpinProperty);
            }

            set
            {
                this.SetValue(InfiniteSpinProperty, value);
            }
        }

        /// <summary>
        /// The mouse input controller property
        /// </summary>
        public static readonly DependencyProperty InputControllerProperty = DependencyProperty.Register(
            "InputController", typeof(InputController), typeof(Viewport3DX), new PropertyMetadata(null, (d, e) =>
            {
                (d as Viewport3DX).CameraController.InputController = e.NewValue == null ? new InputController() : e.NewValue as InputController;
            }));

        /// <summary>
        /// Gets or sets the mouse input controller.
        /// </summary>
        /// <value>
        /// The mouse input controller.
        /// </value>
        public InputController InputController
        {
            set
            {
                SetValue(InputControllerProperty, value);
            }
            get
            {
                return (InputController)GetValue(InputControllerProperty);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PageUpDownZoomSensitivityProperty =
            DependencyProperty.Register(
                "PageUpDownZoomSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    (d as Viewport3DX).CameraController.PageUpDownZoomSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sensitivity for zoom by the page up and page down keys.
        /// </summary>
        /// <value> The zoom sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the zoom direction.
        /// </remarks>
        public double PageUpDownZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(PageUpDownZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(PageUpDownZoomSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MoveSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveSensitivityProperty =
            DependencyProperty.Register(
                "MoveSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    (d as Viewport3DX).CameraController.MoveSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the move sensitivity.
        /// </summary>
        /// <value> The move sensitivity. </value>
        public double MoveSensitivity
        {
            get
            {
                return (double)this.GetValue(MoveSensitivityProperty);
            }

            set
            {
                this.SetValue(MoveSensitivityProperty, value);
            }
        }

#if MSAA

        /// <summary>
        /// Set MSAA Level
        /// </summary>
        public static readonly DependencyProperty MSAAProperty = DependencyProperty.Register("MSAA", typeof(MSAALevel), typeof(Viewport3DX),
            new PropertyMetadata(MSAALevel.Disable, (s, e) =>
            {
                var viewport = s as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.MSAA = (MSAALevel)e.NewValue;
                }
            }));

        /// <summary>
        /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
        /// </summary>
        public MSAALevel MSAA
        {
            get
            {
                return (MSAALevel)this.GetValue(MSAAProperty);
            }
            set
            {
                this.SetValue(MSAAProperty, value);
            }
        }

#endif

        /// <summary>
        /// Enable mouse button hit test
        /// </summary>
        public static readonly DependencyProperty EnableMouseButtonHitTestProperty = DependencyProperty.Register(
            "EnableMouseButtonHitTest", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                (d as Viewport3DX).enableMouseButtonHitTest = (bool)e.NewValue;
            }));

        /// <summary>
        /// Enable mouse button hit test
        /// </summary>
        public bool EnableMouseButtonHitTest
        {
            set
            {
                SetValue(EnableMouseButtonHitTestProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableMouseButtonHitTestProperty);
            }
        }

        /// <summary>
        /// Manually move camera to look at a point in 3D space
        /// </summary>
        public static readonly DependencyProperty ManualLookAtPointProperty = DependencyProperty.Register(
            "ManualLookAtPoint", typeof(Vector3), typeof(Viewport3DX), new PropertyMetadata(new Vector3(), (d, e) =>
                {
                    (d as Viewport3DX).LookAt((Vector3)e.NewValue);
                }));

        /// <summary>
        /// Manually move camera to look at a point in 3D space. (Same as calling Viewport3DX.LookAt() function)
        /// Since camera may have been moved by mouse, the value gets does not reflect the actual point camera currently looking at.
        /// </summary>
        public Vector3 ManualLookAtPoint
        {
            set
            {
                SetValue(ManualLookAtPointProperty, value);
            }
            get
            {
                return (Vector3)GetValue(ManualLookAtPointProperty);
            }
        }

        /// <summary>
        /// Enable render frustum to avoid rendering model if it is out of view frustum
        /// </summary>
        public static readonly DependencyProperty EnableRenderFrustumProperty
            = DependencyProperty.Register("EnableRenderFrustumProperty", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true,
                (s, e) =>
                {
                    var viewport = s as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                    {
                        viewport.renderHostInternal.EnableRenderFrustum = (bool)e.NewValue;
                    }
                }));

        /// <summary>
        /// Enable render frustum to skip rendering model if model is out of the camera bounding frustum
        /// </summary>
        public bool EnableRenderFrustum
        {
            set
            {
                SetValue(EnableRenderFrustumProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableRenderFrustumProperty);
            }
        }

        /// <summary>
        /// <para>Enable deferred rendering. Use multithreading to call rendering procedure using different Deferred Context.</para>
        /// <para>Deferred Rendering: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476892.aspx</para>
        /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
        /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
        /// </summary>
        public static readonly DependencyProperty EnableDeferredRenderingProperty
            = DependencyProperty.Register("EnableDeferredRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// <para>Enable deferred rendering. Use multithreading to call rendering procedure using different Deferred Context.</para>
        /// <para>Deferred Rendering: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476892.aspx</para>
        /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
        /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
        /// </summary>
        public bool EnableDeferredRendering
        {
            set
            {
                SetValue(EnableDeferredRenderingProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableDeferredRenderingProperty);
            }
        }

        /// <summary>
        /// The enable automatic octree update property
        /// </summary>
        public static readonly DependencyProperty EnableAutoOctreeUpdateProperty =
            DependencyProperty.Register("EnableAutoOctreeUpdate", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.AutoUpdateOctree = (bool)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [enable automatic update octree for geometry models].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable automatic octree update]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableAutoOctreeUpdate
        {
            get { return (bool)GetValue(EnableAutoOctreeUpdateProperty); }
            set { SetValue(EnableAutoOctreeUpdateProperty, value); }
        }

        /// <summary>
        /// The render exception property.
        /// </summary>
        public static DependencyProperty RenderExceptionProperty = DependencyProperty.Register(
            "RenderException", typeof(Exception), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Exception"/> that occured at rendering subsystem.
        /// </summary>
        public Exception RenderException
        {
            get { return (Exception)this.GetValue(RenderExceptionProperty); }
            set { this.SetValue(RenderExceptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>
        /// The frame rate.
        /// </value>
        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        /// <summary>
        /// The frame rate property
        /// </summary>
        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(double), typeof(Viewport3DX), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets a value indicating whether [enable order independent transparent rendering] for Transparent objects.
        /// <see cref="MaterialGeometryModel3D.IsTransparent"/>, <see cref="BillboardTextModel3D.IsTransparent"/>
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOITRendering
        {
            get { return (bool)GetValue(EnableOITRenderingProperty); }
            set { SetValue(EnableOITRenderingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable order independent transparent rendering] for Transparent objects.
        /// <see cref="MaterialGeometryModel3D.IsTransparent"/>, <see cref="BillboardTextModel3D.IsTransparent"/>
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
        /// </value>
        public static readonly DependencyProperty EnableOITRenderingProperty =
            DependencyProperty.Register("EnableOITRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.EnableOITRendering = (bool)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// Gets or sets the Order independent transparent rendering color weight power. 
        /// Used for color weight calculation. 
        /// <para>Different near field/far field settings may need different power value for z value based weight calculation.</para>
        /// </summary>
        /// <value>
        /// The oit weight power.
        /// </value>
        public double OITWeightPower
        {
            get { return (double)GetValue(OITWeightPowerProperty); }
            set { SetValue(OITWeightPowerProperty, value); }
        }

        /// <summary>
        /// The Order independent transparent rendering color weight power property
        /// </summary>
        public static readonly DependencyProperty OITWeightPowerProperty =
            DependencyProperty.Register("OITWeightPower", typeof(double), typeof(Viewport3DX), new PropertyMetadata(3.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.OITWeightPower = (float)(double)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// Gets or sets the oit weight depth slope. Used to increase resolution for particular range of depth values. 
        /// <para>If value = 2, the depth range from 0-0.5 expands to 0-1 to increase resolution. However, values from 0.5 - 1 will be pushed to 1</para>
        /// </summary>
        /// <value>
        /// The oit weight depth slope.
        /// </value>
        public double OITWeightDepthSlope
        {
            get { return (double)GetValue(OITWeightDepthSlopeProperty); }
            set { SetValue(OITWeightDepthSlopeProperty, value); }
        }
        /// <summary>
        /// The oit weight depth slope property
        /// </summary>
        public static readonly DependencyProperty OITWeightDepthSlopeProperty =
            DependencyProperty.Register("OITWeightDepthSlope", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.OITWeightDepthSlope = (float)(double)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// Gets or sets the oit weight mode.
        /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
        /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
        /// </summary>
        /// <value>
        /// The oit weight mode.
        /// </value>
        public OITWeightMode OITWeightMode
        {
            get { return (OITWeightMode)GetValue(OITWeightModeProperty); }
            set { SetValue(OITWeightModeProperty, value); }
        }

        /// <summary>
        /// The oit weight mode property
        /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
        /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
        /// </summary>
        public static readonly DependencyProperty OITWeightModeProperty =
            DependencyProperty.Register("OITWeightMode", typeof(OITWeightMode), typeof(Viewport3DX), new PropertyMetadata(OITWeightMode.Linear1, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.OITWeightMode = (OITWeightMode)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));
        /// <summary>
        /// Gets or sets the fxaa. If MSAA is set, FXAA will be disabled automatically
        /// </summary>
        /// <value>
        /// The enable fxaa.
        /// </value>
        public FXAALevel FXAALevel
        {
            get { return (FXAALevel)GetValue(FXAALevelProperty); }
            set { SetValue(FXAALevelProperty, value); }
        }

        /// <summary>
        /// The fxaa level property
        /// </summary>
        public static readonly DependencyProperty FXAALevelProperty =
            DependencyProperty.Register("FXAALevel", typeof(FXAALevel), typeof(Viewport3DX), new PropertyMetadata(FXAALevel.None, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.FXAALevel = (FXAALevel)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [enable design mode rendering].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable design mode rendering]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDesignModeRendering
        {
            get { return (bool)GetValue(EnableDesignModeRenderingProperty); }
            set { SetValue(EnableDesignModeRenderingProperty, value); }
        }

        /// <summary>
        /// The enable design mode rendering property
        /// </summary>
        public static readonly DependencyProperty EnableDesignModeRenderingProperty =
            DependencyProperty.Register("EnableDesignModeRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// Used to create multiple viewport with shared models.
        /// </summary>
        public static readonly DependencyProperty EnableSharedModelModeProperty
            = DependencyProperty.Register("EnableSharedModelMode", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (s, e) =>
            {
                var viewport = s as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.EnableSharingModelMode = (bool)e.NewValue;
                }
            }));

        /// <summary>
        /// Used to create multiple viewport with shared models.
        /// </summary>
        public bool EnableSharedModelMode
        {
            set
            {
                SetValue(EnableSharedModelModeProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableSharedModelModeProperty);
            }
        }

        /// <summary>
        /// Binding to the element inherit with <see cref="IModelContainer"/>
        /// </summary>
        public static readonly DependencyProperty SharedModelContainerProperty
            = DependencyProperty.Register("SharedModelContainer", typeof(IModelContainer), typeof(Viewport3DX), new PropertyMetadata(null,
                (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    if (e.OldValue is IModelContainer o)
                    {
                        o.DettachViewport3DX(viewport);
                    }
                    if (e.NewValue is IModelContainer n)
                    {
                        n.AttachViewport3DX(viewport);
                    }
                    viewport.SharedModelContainerInternal = (IModelContainer)e.NewValue;
                    if (viewport.renderHostInternal != null)
                    {
                        viewport.renderHostInternal.SharedModelContainer = (IModelContainer)e.NewValue;
                    }
                }));

        /// <summary>
        /// Binding to the element inherit with <see cref="IModelContainer"/>
        /// </summary>
        public IModelContainer SharedModelContainer
        {
            set
            {
                SetValue(SharedModelContainerProperty, value);
            }
            get
            {
                return (IModelContainer)GetValue(SharedModelContainerProperty);
            }
        }

        /// <summary>
        /// Gets or sets the shared model container internal.
        /// </summary>
        /// <value>
        /// The shared model container internal.
        /// </value>
        protected IModelContainer SharedModelContainerInternal { private set; get; } = null;

        /// <summary>
        /// The show camera info property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraInfoProperty = DependencyProperty.Register(
            "ShowCameraInfo",
            typeof(bool),
            typeof(Viewport3DX),
            new PropertyMetadata(false, (d, e) =>
            {
                if ((d as Viewport3DX).renderHostInternal != null)
                {
                    if (((bool)e.NewValue))
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail |= RenderDetail.Camera;
                    }
                    else
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail &= ~RenderDetail.Camera;
                    }
                }
            }));
        /// <summary>
        /// Gets or sets a value indicating whether to show camera info.
        /// </summary>
        /// <value>
        /// <c>true</c> if camera info should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowCameraInfo
        {
            get
            {
                return (bool)this.GetValue(ShowCameraInfoProperty);
            }

            set
            {
                this.SetValue(ShowCameraInfoProperty, value);
            }
        }

        /// <summary>
        /// The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
            "ShowFrameRate", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                if ((d as Viewport3DX).renderHostInternal != null)
                {
                    if (((bool)e.NewValue))
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail |= RenderDetail.FPS;
                    }
                    else
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail &= ~RenderDetail.FPS;
                    }
                }
            }));
        /// <summary>
        /// Gets or sets a value indicating whether to show frame rate.
        /// </summary>
        /// <value>
        /// <c>true</c> if frame rate should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowFrameRate
        {
            get
            {
                return (bool)this.GetValue(ShowFrameRateProperty);
            }

            set
            {
                this.SetValue(ShowFrameRateProperty, value);
            }
        }


        /// <summary>
        /// The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameDetailsProperty = DependencyProperty.Register(
            "ShowFrameDetails", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                if ((d as Viewport3DX).renderHostInternal != null)
                {
                    if (((bool)e.NewValue))
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail |= RenderDetail.Statistics;
                    }
                    else
                    {
                        (d as Viewport3DX).renderHostInternal.ShowRenderDetail &= ~RenderDetail.Statistics;
                    }
                }
            }));
        /// <summary>
        /// Gets or sets a value indicating whether [show frame details].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show frame details]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFrameDetails
        {
            set
            {
                SetValue(ShowFrameDetailsProperty, value);
            }
            get
            {
                return (bool)GetValue(ShowFrameDetailsProperty);
            }
        }
        /// <summary>
        /// The show triangle count info property.
        /// </summary>
        public static readonly DependencyProperty ShowTriangleCountInfoProperty = DependencyProperty.Register(
             "ShowTriangleCountInfo", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
             {
                 if ((d as Viewport3DX).renderHostInternal != null)
                 {
                     if (((bool)e.NewValue))
                     {
                         (d as Viewport3DX).renderHostInternal.ShowRenderDetail |= RenderDetail.TriangleInfo;
                     }
                     else
                     {
                         (d as Viewport3DX).renderHostInternal.ShowRenderDetail &= ~RenderDetail.TriangleInfo;
                     }
                 }
             }));

        /// <summary>
        /// Gets or sets a value indicating whether to show the total number of triangles in the scene.
        /// </summary>
        public bool ShowTriangleCountInfo
        {
            get
            {
                return (bool)this.GetValue(ShowTriangleCountInfoProperty);
            }

            set
            {
                this.SetValue(ShowTriangleCountInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the render detail output.
        /// </summary>
        /// <value>
        /// The render detail output.
        /// </value>
        public string RenderDetailOutput
        {
            get { return (string)GetValue(RenderDetailOutputProperty); }
            set { SetValue(RenderDetailOutputProperty, value); }
        }

        /// <summary>
        /// The render detail output property
        /// </summary>
        public static readonly DependencyProperty RenderDetailOutputProperty =
            DependencyProperty.Register("RenderDetailOutput", typeof(string), typeof(Viewport3DX), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets a value indicating whether [enable render order]. 
        /// Specify render order in <see cref="Element3D.RenderOrder"/>. 
        /// Scene node will be sorted by the <see cref="Element3D.RenderOrder"/> during rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable manual render order]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderOrder
        {
            get { return (bool)GetValue(EnableManualRenderOrderProperty); }
            set { SetValue(EnableManualRenderOrderProperty, value); }
        }
        /// <summary>
        /// The enable manual render order property
        /// </summary>
        public static readonly DependencyProperty EnableManualRenderOrderProperty =
            DependencyProperty.Register("EnableRenderOrder", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false,
                (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                    {
                        viewport.renderHostInternal.RenderConfiguration.EnableRenderOrder = (bool)e.NewValue;
                        viewport.renderHostInternal.InvalidatePerFrameRenderables();
                    }
                }));
    }
}
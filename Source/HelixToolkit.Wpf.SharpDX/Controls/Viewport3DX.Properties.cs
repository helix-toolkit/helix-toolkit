// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DX.Properties.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides the dependency properties for Viewport3DX.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using Color4 = global::SharpDX.Color4;

    /// <summary>
    /// Provides the dependency properties for Viewport3DX.
    /// </summary>
    public partial class Viewport3DX
    {
        /// <summary>
        /// Background Color property.this.RenderHost
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color4), typeof(Viewport3DX),
            new UIPropertyMetadata(new Color4(1, 1, 1, 1), (s, e) => ((Viewport3DX)s).ReAttach()));

        /// <summary>
        /// The camera changed event.
        /// </summary>
        public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
            "CameraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Viewport3DX));

        /// <summary>
        /// The camera inertia factor property.
        /// </summary>
        public static readonly DependencyProperty CameraInertiaFactorProperty = DependencyProperty.Register(
            "CameraInertiaFactor", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(0.93));

        /// <summary>
        /// The camera info property.
        /// </summary>
        public static readonly DependencyProperty CameraInfoProperty = DependencyProperty.Register(
            "CameraInfo", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The camera mode property
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(Viewport3DX), new PropertyMetadata(CameraMode.Inspect));

        /// <summary>
        /// The camera property
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera", 
            typeof(Camera), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata((s, e) => ((Viewport3DX)s).CameraPropertyChanged()));

        /// <summary>
        /// The camera rotation mode property
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
                "CameraRotationMode", 
                typeof(CameraRotationMode), 
                typeof(Viewport3DX), 
                new PropertyMetadata(CameraRotationMode.Turntable));

        /// <summary>
        /// The change fov cursor property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty = DependencyProperty.Register(
                "ChangeFieldOfViewCursor", typeof(Cursor), typeof(Viewport3DX), new UIPropertyMetadata(Cursors.ScrollNS));

        /// <summary>
        /// The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewGestureProperty = DependencyProperty.Register(
                "ChangeFieldOfViewGesture", 
                typeof(MouseGesture), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));

        /// <summary>
        /// The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeLookAtGestureProperty = DependencyProperty.Register(
                "ChangeLookAtGesture", 
                typeof(MouseGesture), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightDoubleClick)));

        /// <summary>
        /// The children property
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register(
            "Children", typeof(Element3DCollection), typeof(Viewport3DX));

        /// <summary>
        /// The coordinate system height property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemHeightProperty = DependencyProperty.Register(
                "CoordinateSystemHeight", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(80.0));

        /// <summary>
        /// The coordinate system horizontal position property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemHorizontalPositionProperty = DependencyProperty.Register(
                "CoordinateSystemHorizontalPosition", 
                typeof(HorizontalAlignment), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// The coordinate system label foreground property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelForegroundProperty = DependencyProperty.Register(
                "CoordinateSystemLabelForeground", 
                typeof(Brush), 
                typeof(Viewport3DX), 
                new PropertyMetadata(Brushes.Black));

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
        /// The coordinate system vertical position property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemVerticalPositionProperty = DependencyProperty.Register(
                "CoordinateSystemVerticalPosition", 
                typeof(VerticalAlignment), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// The coordinate system width property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemWidthProperty = DependencyProperty.Register(
                "CoordinateSystemWidth", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(80.0));

        /// <summary>
        /// The current position property.
        /// </summary>
        public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
                "CurrentPosition", 
                typeof(Point3D), 
                typeof(Viewport3DX), 
                new FrameworkPropertyMetadata(
                    new Point3D(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The debug info property.
        /// </summary>
        public static readonly DependencyProperty DebugInfoProperty = DependencyProperty.Register(
            "DebugInfo", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// Deferred Render accessor
        /// </summary>
        public static readonly DependencyProperty DeferredRendererProperty = DependencyProperty.Register(
            "DeferredRenderer", typeof(DeferredRenderer), typeof(Viewport3DX), new PropertyMetadata(null));
        
        /// <summary>
        /// The default camera property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The EnableCurrentPosition property.
        /// </summary>
        public static readonly DependencyProperty EnableCurrentPositionProperty = DependencyProperty.Register(
                "EnableCurrentPosition", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The field of view text property.
        /// </summary>
        public static readonly DependencyProperty FieldOfViewTextProperty = DependencyProperty.Register(
                "FieldOfViewText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The FPS counter property
        /// </summary>
        public static readonly DependencyProperty FpsCounterProperty = DependencyProperty.Register(
            "FpsCounter", typeof(FpsCounter), typeof(Viewport3DX), new PropertyMetadata(new FpsCounter()));

        /// <summary>
        /// The frame rate property.
        /// </summary>
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(int), typeof(Viewport3DX));

        /// <summary>
        /// The frame rate text property.
        /// </summary>
        public static readonly DependencyProperty FrameRateTextProperty = DependencyProperty.Register(
            "FrameRateText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The front view gesture property.
        /// </summary>
        public static readonly DependencyProperty FrontViewGestureProperty = DependencyProperty.Register(
                "FrontViewGesture", 
                typeof(InputGesture), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));

        /// <summary>
        /// The infinite spin property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The info background property.
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
            "InfoBackground", 
            typeof(Brush), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))));

        /// <summary>
        /// The info foreground property.
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
            "InfoForeground", typeof(Brush), typeof(Viewport3DX), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// The message text property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(
            "MessageText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The render exception property.
        /// </summary>
        public static DependencyProperty RenderExceptionProperty = DependencyProperty.Register(
            "RenderException", typeof(Exception), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The render host property.
        /// </summary>
        public static DependencyProperty RenderHostProperty = DependencyProperty.Register(
            "RenderHost", typeof(DPFCanvas), typeof(Viewport3DX), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The is deferred shading enabled propery
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(RenderTechnique), typeof(Viewport3DX), new PropertyMetadata(Techniques.RenderPhong, (s, e) => ((Viewport3DX)s).RenderTechniquePropertyChanged()));

        /// <summary>
        /// The is deferred shading enabled propery
        /// </summary>
        //public static readonly DependencyProperty IsDeferredShadingEnabledProperty = DependencyProperty.Register(
        //    "IsDeferredShadingEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (s, e) => ((Viewport3DX)s).ReAttach()));
        
        /// <summary>
        /// The is deferred shading enabled propery
        /// </summary>
        public static readonly DependencyProperty IsShadowMappingEnabledProperty = DependencyProperty.Register(
            "IsShadowMappingEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (s, e) => ((Viewport3DX)s).ReAttach()));

        /// <summary>
        /// The is change field of view enabled property
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty = DependencyProperty.Register(
            "IsChangeFieldOfViewEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty =
            DependencyProperty.Register(
                "IsInertiaEnabled", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(true));

        /// <summary>
        /// The is pan enabled property
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// The is rotation enabled property
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty = DependencyProperty.Register(
            "IsRotationEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// The IsTouchZoomEnabled property.
        /// </summary>
        public static readonly DependencyProperty IsTouchZoomEnabledProperty = DependencyProperty.Register(
            "IsTouchZoomEnabled", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(true));

        /// <summary>
        /// The is zoom enabled property
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// The left right pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightPanSensitivityProperty = DependencyProperty.Register(
            "LeftRightPanSensitivity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The left right rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightRotationSensitivityProperty = DependencyProperty.Register(
            "LeftRightRotationSensitivity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The maximum field of view property
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty = DependencyProperty.Register(
            "MaximumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(120.0));

        /// <summary>
        /// The minimum field of view property
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty = DependencyProperty.Register(
            "MinimumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(10.0));

        /// <summary>
        /// The model up direction property
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty = DependencyProperty.Register(
            "ModelUpDirection", typeof(Vector3D), typeof(Viewport3DX), new PropertyMetadata(new Vector3D(0, 1, 0)));

        /// <summary>
        /// The orthographic property.
        /// </summary>
        public static readonly DependencyProperty OrthographicProperty = DependencyProperty.Register(
            "Orthographic", 
            typeof(bool), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(false, (s, e) => ((Viewport3DX)s).OrthographicChanged()));

        /// <summary>
        /// The orthographic toggle gesture property.
        /// </summary>
        public static readonly DependencyProperty OrthographicToggleGestureProperty = DependencyProperty.Register(
            "OrthographicToggleGesture", 
            typeof(InputGesture), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// The page up down zoom sensitivity property.
        /// </summary>
        public static readonly DependencyProperty PageUpDownZoomSensitivityProperty = DependencyProperty.Register(
            "PageUpDownZoomSensitivity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The pan cursor property
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.Hand));

        /// <summary>
        /// The rotate around mouse down point property
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty = DependencyProperty.Register(
            "RotateAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The rotate cursor property
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeAll));

        /// <summary>
        /// The rotation sensitivity property
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.Register(
            "RotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0));

        /// <summary>
        /// The show camera info property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraInfoProperty = DependencyProperty.Register(
            "ShowCameraInfo", 
            typeof(bool), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(false, (s, e) => ((Viewport3DX)s).UpdateCameraInfo()));

        /// <summary>
        /// The show camera target property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty = DependencyProperty.Register(
            "ShowCameraTarget", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(true));

        /// <summary>
        /// The show coordinate system property.
        /// </summary>
        public static readonly DependencyProperty ShowCoordinateSystemProperty = DependencyProperty.Register(
            "ShowCoordinateSystem", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The show field of view property.
        /// </summary>
        public static readonly DependencyProperty ShowFieldOfViewProperty = DependencyProperty.Register(
            "ShowFieldOfView", 
            typeof(bool), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(false, (s, e) => ((Viewport3DX)s).UpdateFieldOfViewInfo()));

        /// <summary>
        /// The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
            "ShowFrameRate", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The show triangle count info property.
        /// </summary>
        public static readonly DependencyProperty ShowTriangleCountInfoProperty = DependencyProperty.Register(
             "ShowTriangleCountInfo", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The show view cube property.
        /// </summary>
        public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
            "ShowViewCube", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(true));

        /// <summary>
        /// The spin release time property
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty = DependencyProperty.Register(
            "SpinReleaseTime", typeof(int), typeof(Viewport3DX), new PropertyMetadata(200));

        /// <summary>
        /// The status property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The sub title property.
        /// </summary>
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The sub title size property.
        /// </summary>
        public static readonly DependencyProperty SubTitleSizeProperty = DependencyProperty.Register(
            "SubTitleSize", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(12.0));

        /// <summary>
        /// The text brush property.
        /// </summary>
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
            "TextBrush", typeof(Brush), typeof(Viewport3DX), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// The title background property.
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register(
                "TitleBackground", typeof(Brush), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The title font family property.
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty = DependencyProperty.Register(
                "TitleFontFamily", typeof(FontFamily), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The title size property.
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register(
            "TitleSize", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(12.0));

        /// <summary>
        /// The touch mode property.
        /// </summary>
        public static readonly DependencyProperty TouchModeProperty = DependencyProperty.Register(
            "TouchMode", typeof(TouchMode), typeof(Viewport3DX), new UIPropertyMetadata(TouchMode.Panning));

        /// <summary>
        /// The triangle count info property.
        /// </summary>
        public static readonly DependencyProperty TriangleCountInfoProperty = DependencyProperty.Register(
                "TriangleCountInfo", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata(null));

        /// <summary>
        /// The up down Pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownPanSensitivityProperty = DependencyProperty.Register(
                "UpDownPanSensitivity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The up down rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownRotationSensitivityProperty = DependencyProperty.Register(
                "UpDownRotationSensitivity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The use default gestures property
        /// </summary>
        public static readonly DependencyProperty UseDefaultGesturesProperty = DependencyProperty.Register(
                "UseDefaultGestures", 
                typeof(bool), 
                typeof(Viewport3DX), 
                new PropertyMetadata(true, (s, e) => ((Viewport3DX)s).UseDefaultGesturesChanged()));

        /// <summary>
        /// The view cube back text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBackTextProperty = DependencyProperty.Register(
                "ViewCubeBackText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("R"));

        /// <summary>
        /// The view cube bottom text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBottomTextProperty = DependencyProperty.Register(
                "ViewCubeBottomText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("D"));

        /// <summary>
        /// The view cube front text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeFrontTextProperty = DependencyProperty.Register(
                "ViewCubeFrontText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("L"));

        /// <summary>
        /// The view cube height property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeHeightProperty = DependencyProperty.Register(
            "ViewCubeHeight", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(80.0));

        /// <summary>
        /// The view cube horizontal position property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeHorizontalPositionProperty = DependencyProperty.Register(
                "ViewCubeHorizontalPosition", 
                typeof(HorizontalAlignment), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(HorizontalAlignment.Right));

        /// <summary>
        /// The view cube left text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeLeftTextProperty = DependencyProperty.Register(
                "ViewCubeLeftText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("F"));

        /// <summary>
        /// The view cube opacity property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeOpacityProperty =  DependencyProperty.Register(
                "ViewCubeOpacity", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(0.5));

        /// <summary>
        /// The view cube right text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeRightTextProperty = DependencyProperty.Register(
                "ViewCubeRightText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("B"));

        /// <summary>
        /// The view cube top text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeTopTextProperty = DependencyProperty.Register(
                "ViewCubeTopText", typeof(string), typeof(Viewport3DX), new UIPropertyMetadata("U"));

        /// <summary>
        /// The view cube vertical position property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeVerticalPositionProperty = DependencyProperty.Register(
                "ViewCubeVerticalPosition", 
                typeof(VerticalAlignment), 
                typeof(Viewport3DX), 
                new UIPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// The view cube width property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeWidthProperty = DependencyProperty.Register(
            "ViewCubeWidth", typeof(double), typeof(Viewport3DX), new UIPropertyMetadata(80.0));

        /// <summary>
        /// The zoom around mouse down point property
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty = DependencyProperty.Register(
            "ZoomAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The zoom cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeNS));

        /// <summary>
        /// The zoom extents when loaded property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty = DependencyProperty.Register(
            "ZoomExtentsWhenLoaded", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(false));

        /// <summary>
        /// The zoom rectangle cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty = DependencyProperty.Register(
            "ZoomRectangleCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeNWSE));

        /// <summary>
        /// The zoom rectangle gesture property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleGestureProperty = DependencyProperty.Register(
            "ZoomRectangleGesture", 
            typeof(MouseGesture), 
            typeof(Viewport3DX), 
            new UIPropertyMetadata(
                new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// The zoom sensitivity property
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty = DependencyProperty.Register(
            "ZoomSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0));


        /// <summary>
        /// Background Color
        /// </summary>
        public Color4 BackgroundColor
        {
            get { return (Color4)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        /// Event when a property has been changed
        /// </summary>
        public event RoutedEventHandler CameraChanged
        {
            add
            {
                this.AddHandler(CameraChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(CameraChangedEvent, value);
            }
        }

        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public Camera Camera
        {
            get
            {
                return (Camera)this.GetValue(CameraProperty);
            }

            set
            {
                this.SetValue(CameraProperty, value);
            }
        }

        /// <summary>
        /// Gets the camera controller
        /// </summary>
        public CameraController CameraController
        {
            get
            {
                return this.cameraController;
            }
        }

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
        /// Gets or sets the camera info.
        /// </summary>
        /// <value>
        /// The camera info.
        /// </value>
        public string CameraInfo
        {
            get
            {
                return (string)this.GetValue(CameraInfoProperty);
            }

            set
            {
                this.SetValue(CameraInfoProperty, value);
            }
        }

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
        /// Gets or sets the change field of view cursor.
        /// </summary>
        /// <value>
        /// The change field of view cursor.
        /// </value>
        public Cursor ChangeFieldOfViewCursor
        {
            get
            {
                return (Cursor)this.GetValue(ChangeFieldOfViewCursorProperty);
            }

            set
            {
                this.SetValue(ChangeFieldOfViewCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[Bindable(true)]
        //public Element3DCollection Children
        //{
        //    get
        //    {
        //        return (Element3DCollection)this.GetValue(ChildrenProperty);
        //    }

        //    private set
        //    {
        //        this.SetValue(ChildrenProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the height of the coordinate system viewport.
        /// </summary>
        /// <value>
        /// The height of the coordinate system viewport.
        /// </value>
        public double CoordinateSystemHeight
        {
            get
            {
                return (double)this.GetValue(CoordinateSystemHeightProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the coordinate system viewport.
        /// </summary>
        /// <value>
        /// The horizontal position.
        /// </value>
        public HorizontalAlignment CoordinateSystemHorizontalPosition
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(CoordinateSystemHorizontalPositionProperty);
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
        public Brush CoordinateSystemLabelForeground
        {
            get
            {
                return (Brush)this.GetValue(CoordinateSystemLabelForegroundProperty);
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
        /// Gets or sets the vertical position of the coordinate system viewport.
        /// </summary>
        /// <value>
        /// The vertical position.
        /// </value>
        public VerticalAlignment CoordinateSystemVerticalPosition
        {
            get
            {
                return (VerticalAlignment)this.GetValue(CoordinateSystemVerticalPositionProperty);
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
        public double CoordinateSystemWidth
        {
            get
            {
                return (double)this.GetValue(CoordinateSystemWidthProperty);
            }

            set
            {
                this.SetValue(CoordinateSystemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        /// <remarks>
        /// The <see cref="EnableCurrentPosition"/> property must be set to true to enable updating of this property.
        /// </remarks>
        public Point3D CurrentPosition
        {
            get
            {
                return (Point3D)this.GetValue(CurrentPositionProperty);
            }

            set
            {
                this.SetValue(CurrentPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the debug info text.
        /// </summary>
        /// <value>
        /// The debug info text.
        /// </value>
        public string DebugInfo
        {
            get
            {
                return (string)this.GetValue(DebugInfoProperty);
            }

            set
            {
                this.SetValue(DebugInfoProperty, value);
            }
        }

        /// <summary>
        /// Read-Only DP for the deferred renderes (little bit hacky...)
        /// </summary>
        public DeferredRenderer DeferredRenderer
        {
            get { return (DeferredRenderer)this.GetValue(DeferredRendererProperty); }
            set { this.SetValue(DeferredRendererProperty, value); }
        }
             

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
        /// Gets or sets a value indicating whether calculation of the <see cref="CurrentPosition"/> property is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool EnableCurrentPosition
        {
            get
            {
                return (bool)this.GetValue(EnableCurrentPositionProperty);
            }

            set
            {
                this.SetValue(EnableCurrentPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the field of view text.
        /// </summary>
        /// <value>
        /// The field of view text.
        /// </value>
        public string FieldOfViewText
        {
            get
            {
                return (string)this.GetValue(FieldOfViewTextProperty);
            }

            set
            {
                this.SetValue(FieldOfViewTextProperty, value);
            }
        }

        /// <summary>
        /// Gets the FPS counter.
        /// </summary>
        /// <value>
        /// The FPS counter.
        /// </value>
        public FpsCounter FpsCounter
        {
            get
            {
                return (FpsCounter)this.GetValue(FpsCounterProperty);
            }

            private set
            {
                this.SetValue(FpsCounterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>
        /// The frame rate.
        /// </value>
        public int FrameRate
        {
            get
            {
                return (int)this.GetValue(FrameRateProperty);
            }

            set
            {
                this.SetValue(FrameRateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the frame rate text.
        /// </summary>
        /// <value>
        /// The frame rate text.
        /// </value>
        public string FrameRateText
        {
            get
            {
                return (string)this.GetValue(FrameRateTextProperty);
            }

            set
            {
                this.SetValue(FrameRateTextProperty, value);
            }
        }

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
        /// Gets or sets the background brush for the CameraInfo and TriangleCount fields.
        /// </summary>
        /// <value>
        /// The info background.
        /// </value>
        public Brush InfoBackground
        {
            get
            {
                return (Brush)this.GetValue(InfoBackgroundProperty);
            }

            set
            {
                this.SetValue(InfoBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground brush for informational text.
        /// </summary>
        /// <value>
        /// The foreground brush.
        /// </value>
        public Brush InfoForeground
        {
            get
            {
                return (Brush)this.GetValue(InfoForegroundProperty);
            }

            set
            {
                this.SetValue(InfoForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        /// <value>
        /// The message text.
        /// </value>
        public string MessageText
        {
            get
            {
                return (string)this.GetValue(MessageTextProperty);
            }

            set
            {
                this.SetValue(MessageTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Exception"/> that occured at rendering subsystem.
        /// </summary>
        public Exception RenderException
        {
            get { return (Exception)this.GetValue(RenderExceptionProperty); }
            set { this.SetValue(RenderExceptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DPFCanvas"/>.
        /// </summary>
        public DPFCanvas RenderHost
        {
            get { return (DPFCanvas)this.GetValue(RenderHostProperty); }
            set { this.SetValue(RenderHostProperty, value); }
        }

        /// <summary>
        /// Gets or sets value for the shading model shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public RenderTechnique RenderTechnique
        {
            get { return (RenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether deferred shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        //public bool IsDeferredShadingEnabled
        //{
        //    get { return (bool)this.GetValue(IsDeferredShadingEnabledProperty); }
        //    set { this.SetValue(IsDeferredShadingEnabledProperty, value); }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether shadow mapping is enabled
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowMappingEnabled
        {
            get { return (bool)this.GetValue(IsShadowMappingEnabledProperty); }
            set { this.SetValue(IsShadowMappingEnabledProperty, value); }
        }

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
        /// Gets or sets a value indicating whether touch zoom (pinch gesture) is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if touch zoom is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool IsTouchZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsTouchZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsTouchZoomEnabledProperty, value);
            }
        }

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
        /// Gets or sets the model up direction.
        /// </summary>
        /// <value>
        /// The model up direction.
        /// </value>
        public Vector3D ModelUpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(ModelUpDirectionProperty);
            }

            set
            {
                this.SetValue(ModelUpDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an orthographic camera should be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if orthographic; otherwise, <c>false</c> .
        /// </value>
        public bool Orthographic
        {
            get
            {
                return (bool)this.GetValue(OrthographicProperty);
            }

            set
            {
                this.SetValue(OrthographicProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for zoom by the page up and page down keys.
        /// </summary>
        /// <value>
        /// The zoom sensitivity.
        /// </value>
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
        /// Gets or sets the pan cursor.
        /// </summary>
        /// <value>
        /// The pan cursor.
        /// </value>
        public Cursor PanCursor
        {
            get
            {
                return (Cursor)this.GetValue(PanCursorProperty);
            }

            set
            {
                this.SetValue(PanCursorProperty, value);
            }
        }

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
        /// Gets or sets the rotate cursor.
        /// </summary>
        /// <value>
        /// The rotate cursor.
        /// </value>
        public Cursor RotateCursor
        {
            get
            {
                return (Cursor)this.GetValue(RotateCursorProperty);
            }

            set
            {
                this.SetValue(RotateCursorProperty, value);
            }
        }

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
        /// Gets or sets a value indicating whether to show the camera target adorner.
        /// </summary>
        /// <value>
        /// <c>true</c> if camera target should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowCameraTarget
        {
            get
            {
                return (bool)this.GetValue(ShowCameraTargetProperty);
            }

            set
            {
                this.SetValue(ShowCameraTargetProperty, value);
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

        /// <summary>
        /// Gets or sets a value indicating whether to show field of view.
        /// </summary>
        /// <value>
        /// <c>true</c> if field of view should be shown; otherwise, <c>false</c> .
        /// </value>
        public bool ShowFieldOfView
        {
            get
            {
                return (bool)this.GetValue(ShowFieldOfViewProperty);
            }

            set
            {
                this.SetValue(ShowFieldOfViewProperty, value);
            }
        }

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
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status
        {
            get
            {
                return (string)this.GetValue(StatusProperty);
            }

            set
            {
                this.SetValue(StatusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sub title.
        /// </summary>
        /// <value>
        /// The sub title.
        /// </value>
        public string SubTitle
        {
            get
            {
                return (string)this.GetValue(SubTitleProperty);
            }

            set
            {
                this.SetValue(SubTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the sub title.
        /// </summary>
        /// <value>
        /// The size of the sub title.
        /// </value>
        public double SubTitleSize
        {
            get
            {
                return (double)this.GetValue(SubTitleSizeProperty);
            }

            set
            {
                this.SetValue(SubTitleSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
        public Brush TextBrush
        {
            get
            {
                return (Brush)this.GetValue(TextBrushProperty);
            }

            set
            {
                this.SetValue(TextBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }

            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title background brush.
        /// </summary>
        /// <value>
        /// The title background.
        /// </value>
        public Brush TitleBackground
        {
            get
            {
                return (Brush)this.GetValue(TitleBackgroundProperty);
            }

            set
            {
                this.SetValue(TitleBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title font family.
        /// </summary>
        /// <value>
        /// The title font family.
        /// </value>
        public FontFamily TitleFontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(TitleFontFamilyProperty);
            }

            set
            {
                this.SetValue(TitleFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the title.
        /// </summary>
        /// <value>
        /// The size of the title.
        /// </value>
        public double TitleSize
        {
            get
            {
                return (double)this.GetValue(TitleSizeProperty);
            }

            set
            {
                this.SetValue(TitleSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the touch mode.
        /// </summary>
        /// <value>
        /// The touch mode.
        /// </value>
        public TouchMode TouchMode
        {
            get
            {
                return (TouchMode)this.GetValue(TouchModeProperty);
            }

            set
            {
                this.SetValue(TouchModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets TriangleCountInfo.
        /// </summary>
        public string TriangleCountInfo
        {
            get
            {
                return (string)this.GetValue(TriangleCountInfoProperty);
            }

            set
            {
                this.SetValue(TriangleCountInfoProperty, value);
            }
        }

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
        /// Gets or sets a value indicating whether to use default mouse/keyboard gestures.
        /// </summary>
        /// <value>
        ///   <c>true</c> if default gestures should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseDefaultGestures
        {
            get
            {
                return (bool)this.GetValue(UseDefaultGesturesProperty);
            }

            set
            {
                this.SetValue(UseDefaultGesturesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube back text.
        /// </summary>
        /// <value>
        /// The view cube back text.
        /// </value>
        public string ViewCubeBackText
        {
            get
            {
                return (string)this.GetValue(ViewCubeBackTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeBackTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube bottom text.
        /// </summary>
        /// <value>
        /// The view cube bottom text.
        /// </value>
        public string ViewCubeBottomText
        {
            get
            {
                return (string)this.GetValue(ViewCubeBottomTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeBottomTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube front text.
        /// </summary>
        /// <value>
        /// The view cube front text.
        /// </value>
        public string ViewCubeFrontText
        {
            get
            {
                return (string)this.GetValue(ViewCubeFrontTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeFrontTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the view cube viewport.
        /// </summary>
        /// <value>
        /// The height of the view cube viewport.
        /// </value>
        public double ViewCubeHeight
        {
            get
            {
                return (double)this.GetValue(ViewCubeHeightProperty);
            }

            set
            {
                this.SetValue(ViewCubeHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position of the view cube viewport.
        /// </summary>
        /// <value>
        /// The horizontal position.
        /// </value>
        public HorizontalAlignment ViewCubeHorizontalPosition
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(ViewCubeHorizontalPositionProperty);
            }

            set
            {
                this.SetValue(ViewCubeHorizontalPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube left text.
        /// </summary>
        /// <value>
        /// The view cube left text.
        /// </value>
        public string ViewCubeLeftText
        {
            get
            {
                return (string)this.GetValue(ViewCubeLeftTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeLeftTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the opacity of the ViewCube when inactive.
        /// </summary>
        public double ViewCubeOpacity
        {
            get
            {
                return (double)this.GetValue(ViewCubeOpacityProperty);
            }

            set
            {
                this.SetValue(ViewCubeOpacityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube right text.
        /// </summary>
        /// <value>
        /// The view cube right text.
        /// </value>
        public string ViewCubeRightText
        {
            get
            {
                return (string)this.GetValue(ViewCubeRightTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeRightTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view cube top text.
        /// </summary>
        /// <value>
        /// The view cube top text.
        /// </value>
        public string ViewCubeTopText
        {
            get
            {
                return (string)this.GetValue(ViewCubeTopTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeTopTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical position of view cube viewport.
        /// </summary>
        /// <value>
        /// The vertical position.
        /// </value>
        public VerticalAlignment ViewCubeVerticalPosition
        {
            get
            {
                return (VerticalAlignment)this.GetValue(ViewCubeVerticalPositionProperty);
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
        public double ViewCubeWidth
        {
            get
            {
                return (double)this.GetValue(ViewCubeWidthProperty);
            }

            set
            {
                this.SetValue(ViewCubeWidthProperty, value);
            }
        }

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
        /// Gets or sets the zoom cursor.
        /// </summary>
        /// <value>
        /// The zoom cursor.
        /// </value>
        public Cursor ZoomCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomCursorProperty);
            }

            set
            {
                this.SetValue(ZoomCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Zoom extents when the control has loaded.
        /// </summary>
        public bool ZoomExtentsWhenLoaded
        {
            get
            {
                return (bool)this.GetValue(ZoomExtentsWhenLoadedProperty);
            }

            set
            {
                this.SetValue(ZoomExtentsWhenLoadedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom rectangle cursor.
        /// </summary>
        /// <value>
        /// The zoom rectangle cursor.
        /// </value>
        public Cursor ZoomRectangleCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomRectangleCursorProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleCursorProperty, value);
            }
        }

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
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DX.Properties.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides the dependency properties for Viewport3DX.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Matrix = System.Numerics.Matrix4x4;
namespace HelixToolkit.Wpf.SharpDX
{
    using Controls;
    using Elements2D;


    /// <summary>
    /// Provides the dependency properties for Viewport3DX.
    /// </summary>
    public partial class Viewport3DX
    {
        /// <summary>
        /// Background Color property.this.RenderHost
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color), typeof(Viewport3DX),
            new PropertyMetadata(Colors.White, (s, e) =>
            {
                if (((Viewport3DX)s).renderHostInternal != null)
                {
                    ((Viewport3DX)s).renderHostInternal.ClearColor = ((Color)e.NewValue).ToColor4();
                }
            }));

        /// <summary>
        /// The camera changed event.
        /// </summary>
        public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
            "CameraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Viewport3DX));

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseDown3D
        {
            add { this.AddHandler(GeometryModel3D.MouseDown3DEvent, value); }
            remove { this.RemoveHandler(GeometryModel3D.MouseDown3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseUp3D
        {
            add { this.AddHandler(GeometryModel3D.MouseUp3DEvent, value); }
            remove { this.RemoveHandler(GeometryModel3D.MouseUp3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseMove3D
        {
            add { this.AddHandler(GeometryModel3D.MouseMove3DEvent, value); }
            remove { this.RemoveHandler(GeometryModel3D.MouseMove3DEvent, value); }
        }
        /// <summary>
        /// Occurs when [form mouse move].
        /// </summary>
        public event WinformHostExtend.FormMouseMoveEventHandler FormMouseMove
        {
            add { this.AddHandler(WinformHostExtend.FormMouseMoveEvent, value); }
            remove { this.RemoveHandler(WinformHostExtend.FormMouseMoveEvent, value); }
        }
        /// <summary>
        /// Occurs when [form mouse wheel].
        /// </summary>
        public event WinformHostExtend.FormMouseWheelEventHandler FormMouseWheel
        {
            add { this.AddHandler(WinformHostExtend.FormMouseWheelEvent, value); }
            remove { this.RemoveHandler(WinformHostExtend.FormMouseWheelEvent, value); }
        }
        /// <summary>
        /// The camera inertia factor property.
        /// </summary>
        public static readonly DependencyProperty CameraInertiaFactorProperty = DependencyProperty.Register(
            "CameraInertiaFactor", typeof(double), typeof(Viewport3DX), new PropertyMetadata(0.93, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.InertiaFactor = (double)e.NewValue;            
            }));

        /// <summary>
        /// The camera mode property
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(Viewport3DX), new PropertyMetadata(CameraMode.Inspect, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.CameraMode = (CameraMode)e.NewValue;
            }));

        /// <summary>
        /// The camera property
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera",
            typeof(Camera),
            typeof(Viewport3DX),
            new PropertyMetadata(null, (s, e) => 
            {
                (s as Viewport3DX).CameraPropertyChanged(e);
            }));

        /// <summary>
        /// The camera rotation mode property
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty = DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(Viewport3DX),
                new PropertyMetadata(CameraRotationMode.Turntable, (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    viewport.CameraController.CameraRotationMode = (CameraRotationMode)e.NewValue;
                }));

        /// <summary>
        /// The change fov cursor property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty = DependencyProperty.Register(
                "ChangeFieldOfViewCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.ScrollNS, (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    viewport.CameraController.ChangeFieldOfViewCursor = (Cursor)e.NewValue;
                }));

        /// <summary>
        /// The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewGestureProperty = DependencyProperty.Register(
                "ChangeFieldOfViewGesture",
                typeof(MouseGesture),
                typeof(Viewport3DX),
                new PropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));

        /// <summary>
        /// The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeLookAtGestureProperty = DependencyProperty.Register(
                "ChangeLookAtGesture",
                typeof(MouseGesture),
                typeof(Viewport3DX),
                new PropertyMetadata(new MouseGesture(MouseAction.RightDoubleClick)));

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
        /// The is coordinate system mover enabled property
        /// </summary>
        public static readonly DependencyProperty IsCoordinateSystemMoverEnabledProperty =
            DependencyProperty.Register("IsCoordinateSystemMoverEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));


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
        /// The current position property.
        /// </summary>
        public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
                "CurrentPosition",
                typeof(Point3D),
                typeof(Viewport3DX),
                new FrameworkPropertyMetadata(
                    new Point3D(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The default camera property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(Viewport3DX), new PropertyMetadata(null, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.DefaultCamera = e.NewValue as ProjectionCamera;
            }));

        /// <summary>
        /// The EnableCurrentPosition property.
        /// </summary>
        public static readonly DependencyProperty EnableCurrentPositionProperty = DependencyProperty.Register(
                "EnableCurrentPosition", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The EffectsManager property.
        /// </summary>
        public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
            "EffectsManager", typeof(IEffectsManager), typeof(Viewport3DX), new PropertyMetadata(
                null,
                (s, e) => ((Viewport3DX)s).EffectsManagerPropertyChanged()));

        /// <summary>
        /// The field of view text property.
        /// </summary>
        public static readonly DependencyProperty FieldOfViewTextProperty = DependencyProperty.Register(
                "FieldOfViewText", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The frame rate property.
        /// </summary>
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(Viewport3DX));

        /// <summary>
        /// The frame rate text property.
        /// </summary>
        public static readonly DependencyProperty FrameRateTextProperty = DependencyProperty.Register(
            "FrameRateText", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The front view gesture property.
        /// </summary>
        public static readonly DependencyProperty FrontViewGestureProperty = DependencyProperty.Register(
                "FrontViewGesture",
                typeof(InputGesture),
                typeof(Viewport3DX),
                new PropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));

        /// <summary>
        /// The infinite spin property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.InfiniteSpin = (bool)e.NewValue;
            }));

        /// <summary>
        /// The info background property.
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
            "InfoBackground",
            typeof(Brush),
            typeof(Viewport3DX),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x80, 0x8f, 0x8f, 0x8f))));

        /// <summary>
        /// The info foreground property.
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
            "InfoForeground", typeof(Brush), typeof(Viewport3DX), new PropertyMetadata(Brushes.Blue));

        /// <summary>
        /// The message text property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(
            "MessageText", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The render exception property.
        /// </summary>
        public static DependencyProperty RenderExceptionProperty = DependencyProperty.Register(
            "RenderException", typeof(Exception), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The Render Technique property
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(IRenderTechnique), typeof(Viewport3DX), new PropertyMetadata(null,
                (s, e) => ((Viewport3DX)s).RenderTechniquePropertyChanged(e.NewValue as IRenderTechnique)));

        ///// <summary>
        ///// The is deferred shading enabled propery
        ///// </summary>
        //public static readonly DependencyProperty IsDeferredShadingEnabledProperty = DependencyProperty.Register(
        //    "IsDeferredShadingEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (s, e) => ((Viewport3DX)s).ReAttach()));

        /// <summary>
        /// The is deferred shading enabled propery
        /// </summary>
        public static readonly DependencyProperty IsShadowMappingEnabledProperty = DependencyProperty.Register(
            "IsShadowMappingEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, 
                (s, e) =>
                {
                    if(((Viewport3DX)s).renderHostInternal!=null)
                        ((Viewport3DX)s).renderHostInternal.IsShadowMapEnabled = (bool)e.NewValue;
                }));

        /// <summary>
        /// The is change field of view enabled property
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty = DependencyProperty.Register(
            "IsChangeFieldOfViewEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.IsChangeFieldOfViewEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty =
            DependencyProperty.Register(
                "IsInertiaEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    viewport.CameraController.IsInertiaEnabled = (bool)e.NewValue;
                }));

        /// <summary>
        /// The is pan enabled property
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.IsPanEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is rotation enabled property
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty = DependencyProperty.Register(
            "IsRotationEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.IsRotationEnabled = (bool)e.NewValue;
            }));

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
        /// The IsTouchZoomEnabled property.
        /// </summary>
        public static readonly DependencyProperty IsPinchZoomEnabledProperty = DependencyProperty.Register(
            "IsPinchZoomEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.EnablePinchZoom = (bool)e.NewValue;
            }));

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
        /// The is zoom enabled property
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.IsZoomEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The left right pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightPanSensitivityProperty = DependencyProperty.Register(
            "LeftRightPanSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.LeftRightPanSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// The left right rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty LeftRightRotationSensitivityProperty = DependencyProperty.Register(
            "LeftRightRotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.LeftRightRotationSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// The maximum field of view property
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty = DependencyProperty.Register(
            "MaximumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(120.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.MaximumFieldOfView = (double)e.NewValue;
            }));

        /// <summary>
        /// The minimum field of view property
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty = DependencyProperty.Register(
            "MinimumFieldOfView", typeof(double), typeof(Viewport3DX), new PropertyMetadata(10.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.MinimumFieldOfView = (double)e.NewValue;
            }));

        /// <summary>
        /// The model up direction property
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty = DependencyProperty.Register(
            "ModelUpDirection", typeof(Vector3D), typeof(Viewport3DX), new PropertyMetadata(new Vector3D(0, 1, 0), (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ModelUpDirection = ((Vector3D)e.NewValue).ToVector3();
            }));

        /// <summary>
        /// The orthographic property.
        /// </summary>
        public static readonly DependencyProperty OrthographicProperty = DependencyProperty.Register(
            "Orthographic",
            typeof(bool),
            typeof(Viewport3DX),
            new PropertyMetadata(false, (s, e) => ((Viewport3DX)s).OrthographicChanged()));

        /// <summary>
        /// The orthographic toggle gesture property.
        /// </summary>
        public static readonly DependencyProperty OrthographicToggleGestureProperty = DependencyProperty.Register(
            "OrthographicToggleGesture",
            typeof(InputGesture),
            typeof(Viewport3DX),
            new PropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// The page up down zoom sensitivity property.
        /// </summary>
        public static readonly DependencyProperty PageUpDownZoomSensitivityProperty = DependencyProperty.Register(
            "PageUpDownZoomSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.PageUpDownZoomSensitivity = (double)e.NewValue;
            }));

        /// <summary>
        /// The pan cursor property
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.Hand, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.PanCursor = (Cursor)e.NewValue;
            }));

        /// <summary>
        /// The rotate around mouse down point property
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty = DependencyProperty.Register(
            "RotateAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.RotateAroundMouseDownPoint = (bool)e.NewValue;
            }));

        /// <summary>
        /// The rotate cursor property
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeAll, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.RotateCursor = (Cursor)e.NewValue;
            }));

        /// <summary>
        /// The rotation sensitivity property
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.Register(
            "RotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.RotationSensitivity = (double)e.NewValue;
            }));

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
        /// The show camera target property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty = DependencyProperty.Register(
            "ShowCameraTarget", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ShowCameraTarget = (bool)e.NewValue;
            }));

        /// <summary>
        /// The show coordinate system property.
        /// </summary>
        public static readonly DependencyProperty ShowCoordinateSystemProperty = DependencyProperty.Register(
            "ShowCoordinateSystem", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
            "ShowFrameRate", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d,e)=> 
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
        /// The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameDetailsProperty = DependencyProperty.Register(
            "ShowFrameDetails", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d,e)=> 
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
        /// The show triangle count info property.
        /// </summary>
        public static readonly DependencyProperty ShowTriangleCountInfoProperty = DependencyProperty.Register(
             "ShowTriangleCountInfo", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e)=> 
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
        /// The show view cube property.
        /// </summary>
        public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
            "ShowViewCube", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

        /// <summary>
        /// The spin release time property
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty = DependencyProperty.Register(
            "SpinReleaseTime", typeof(int), typeof(Viewport3DX), new PropertyMetadata(200, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.SpinReleaseTime = (int)e.NewValue;
            }));

        /// <summary>
        /// The sub title property.
        /// </summary>
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The sub title size property.
        /// </summary>
        public static readonly DependencyProperty SubTitleSizeProperty = DependencyProperty.Register(
            "SubTitleSize", typeof(double), typeof(Viewport3DX), new PropertyMetadata(12.0));

        /// <summary>
        /// The text brush property.
        /// </summary>
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
            "TextBrush", typeof(Brush), typeof(Viewport3DX), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// The title background property.
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty = DependencyProperty.Register(
                "TitleBackground", typeof(Brush), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The title font family property.
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty = DependencyProperty.Register(
                "TitleFontFamily", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Viewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// The title size property.
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register(
            "TitleSize", typeof(double), typeof(Viewport3DX), new PropertyMetadata(12.0));



        /// <summary>
        /// The up down Pan sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownPanSensitivityProperty = DependencyProperty.Register(
                "UpDownPanSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    viewport.CameraController.UpDownPanSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// The up down rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty UpDownRotationSensitivityProperty = DependencyProperty.Register(
                "UpDownRotationSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
                {
                    var viewport = d as Viewport3DX;
                    viewport.CameraController.UpDownRotationSensitivity = (double)e.NewValue;
                }));

        /// <summary>
        /// The use default gestures property
        /// </summary>
        public static readonly DependencyProperty UseDefaultGesturesProperty = DependencyProperty.Register(
                "UseDefaultGestures",
                typeof(bool),
                typeof(Viewport3DX),
                new PropertyMetadata(true, (s, e) => ((Viewport3DX)s).UseDefaultGesturesChanged()));

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
        /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsViewCubeMoverEnabledProperty =
            DependencyProperty.Register("IsViewCubeMoverEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true));

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
        /// The zoom around mouse down point property
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty = DependencyProperty.Register(
            "ZoomAroundMouseDownPoint", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomAroundMouseDownPoint = (bool)e.NewValue;
            }));

        /// <summary>
        /// The zoom cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeNS, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomCursor = (Cursor)e.NewValue;
            }));

        /// <summary>
        /// The far zoom distance limit property.
        /// </summary>
        public static readonly DependencyProperty ZoomDistanceLimitFarProperty = DependencyProperty.Register(
            "ZoomDistanceLimitFar", typeof(double), typeof(Viewport3DX), new PropertyMetadata(double.PositiveInfinity, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomDistanceLimitFar = (double)e.NewValue;
            }));

        /// <summary>
        /// The near zoom distance limit property.
        /// </summary>
        public static readonly DependencyProperty ZoomDistanceLimitNearProperty = DependencyProperty.Register(
            "ZoomDistanceLimitNear", typeof(double), typeof(Viewport3DX), new PropertyMetadata(0.001, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomDistanceLimitNear = (double)e.NewValue;
            }));

        /// <summary>
        /// The zoom extents when loaded property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty = DependencyProperty.Register(
            "ZoomExtentsWhenLoaded", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The zoom rectangle cursor property
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty = DependencyProperty.Register(
            "ZoomRectangleCursor", typeof(Cursor), typeof(Viewport3DX), new PropertyMetadata(Cursors.SizeNWSE, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomRectangleCursor = (Cursor)e.NewValue;
            }));

        /// <summary>
        /// The zoom rectangle gesture property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleGestureProperty = DependencyProperty.Register(
            "ZoomRectangleGesture",
            typeof(MouseGesture),
            typeof(Viewport3DX),
            new PropertyMetadata(
                new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// The zoom sensitivity property
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty = DependencyProperty.Register(
            "ZoomSensitivity", typeof(double), typeof(Viewport3DX), new PropertyMetadata(1.0, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.ZoomSensitivity = (double)e.NewValue;
            }));

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
#endif

        /// <summary>
        ///   The is move enabled property.
        /// </summary>
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
            "IsMoveEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e) =>
            {
                var viewport = d as Viewport3DX;
                viewport.CameraController.IsMoveEnabled = (bool)e.NewValue;
            }));


        /// <summary>
        /// Rotate around this fixed rotation point only.<see cref="FixedRotationPointEnabledProperty"/> 
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointProperty = DependencyProperty.Register(
            "FixedRotationPoint", typeof(Point3D), typeof(Viewport3DX), new PropertyMetadata(new Point3D(), (d,e)=>
            {
                (d as Viewport3DX).CameraController.FixedRotationPoint = ((Point3D)e.NewValue).ToVector3();
            }));

        /// <summary>
        /// Enable fixed rotation mode and use FixedRotationPoint for rotation. Only works under CameraMode = Inspect
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointEnabledProperty = DependencyProperty.Register(
            "FixedRotationPointEnabled", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d,e)=> 
            {
                (d as Viewport3DX).CameraController.FixedRotationPointEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// Enable mouse button hit test
        /// </summary>
        public static readonly DependencyProperty EnableMouseButtonHitTestProperty = DependencyProperty.Register(
            "EnableMouseButtonHitTest", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d, e)=> {
                (d as Viewport3DX).enableMouseButtonHitTest = (bool)e.NewValue;
            }));

        /// <summary>
        /// Manually move camera to look at a point in 3D space
        /// </summary>
        public static readonly DependencyProperty ManualLookAtPointProperty = DependencyProperty.Register(
            "ManualLookAtPoint", typeof(Point3D), typeof(Viewport3DX), new FrameworkPropertyMetadata(new Point3D(), (d, e) => { },
                (d, e) =>
                {
                    (d as Viewport3DX).LookAt((Point3D)e);
                    return e;
                })
            { BindsTwoWayByDefault = false });

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
                    viewport.EnableRenderFrustum = (bool)e.NewValue;
                }
            }));

        /// <summary>
        /// <para>Enable deferred rendering. Use multithreading to call rendering procedure using different Deferred Context.</para> 
        /// <para>Deferred Rendering: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476892.aspx</para>
        /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
        /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
        /// </summary>
        public static readonly DependencyProperty EnableDeferredRenderingProperty
            = DependencyProperty.Register("EnableDeferredRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

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
        /// The enable swap chain rendering property
        /// </summary>
        public static readonly DependencyProperty EnableSwapChainRenderingProperty
            = DependencyProperty.Register("EnableSwapChainRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));

        /// <summary>
        /// The content2 d property
        /// </summary>
        public static readonly DependencyProperty Content2DProperty
            = DependencyProperty.Register("Content2D", typeof(Element2D), typeof(Viewport3DX), new PropertyMetadata(null, (d, e)=> 
            {
                if (e.OldValue is Element2D elementOld)
                {
                    (d as Viewport3DX).Overlay2D.Children.Remove(elementOld);                   
                }
                if (e.NewValue is Element2D elementNew)
                {
                    (d as Viewport3DX).Overlay2D.Children.Add(elementNew);                   
                }
            }));

        /// <summary>
        /// The enable d2 d rendering property
        /// </summary>
        public static readonly DependencyProperty EnableD2DRenderingProperty =
            DependencyProperty.Register("EnableD2DRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d,e)=>
            {
                var viewport = d as Viewport3DX;
                if(viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.RenderD2D = (bool)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// The enable automatic octree update property
        /// </summary>
        public static readonly DependencyProperty EnableAutoOctreeUpdateProperty =
            DependencyProperty.Register("EnableAutoOctreeUpdate", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false, (d,e)=>
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.AutoUpdateOctree = (bool)e.NewValue;
                }
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [enable order independent transparent rendering] for Transparent objects.
        /// <see cref="MaterialGeometryModel3D.IsTransparent"/>, <see cref="BillboardTextModel3D.IsTransparent"/>
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
        /// </value>
        public static readonly DependencyProperty EnableOITRenderingProperty =
            DependencyProperty.Register("EnableOITRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(true, (d,e)=> 
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.EnableOITRendering = (bool)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));

        /// <summary>
        /// The Order independent transparent rendering color weight power property
        /// </summary>
        public static readonly DependencyProperty OITWeightPowerProperty =
            DependencyProperty.Register("OITWeightPower", typeof(double), typeof(Viewport3DX), new PropertyMetadata(3.0, (d,e)=> 
            {
                var viewport = d as Viewport3DX;
                if (viewport.renderHostInternal != null)
                {
                    viewport.renderHostInternal.RenderConfiguration.OITWeightPower = (float)(double)e.NewValue;
                    viewport.InvalidateRender();
                }
            }));


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
        /// The enable design time rendering property
        /// </summary>
        public static readonly DependencyProperty EnableDesignModeRenderingProperty =
            DependencyProperty.Register("EnableDesignModeRendering", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false));


        /// <summary>
        /// The enable render order property. <see cref="EnableRenderOrder"/>
        /// </summary>
        public static readonly DependencyProperty EnableRenderOrderProperty =
            DependencyProperty.Register("EnableRenderOrder", typeof(bool), typeof(Viewport3DX), new PropertyMetadata(false,
                (d,e)=>
                {
                    var viewport = d as Viewport3DX;
                    if (viewport.renderHostInternal != null)
                    {
                        viewport.renderHostInternal.RenderConfiguration.EnableRenderOrder = (bool)e.NewValue;
                        viewport.renderHostInternal.InvalidatePerFrameRenderables();
                    }
                }));


        /// <summary>
        /// Background Color
        /// </summary>
        public Color BackgroundColor
        {
            get { return (Color)this.GetValue(BackgroundColorProperty); }
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
        internal CameraController CameraController
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


        //public DeferredRenderer DeferredRenderer
        //{
        //    get { return (DeferredRenderer)this.GetValue(DeferredRendererProperty); }
        //    set { this.SetValue(DeferredRendererProperty, value); }
        //}


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
        /// Gets or sets the <see cref="IEffectsManager"/>.
        /// </summary>
        public IEffectsManager EffectsManager
        {
            get { return (IEffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
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
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>
        /// The frame rate.
        /// </value>
        public double FrameRate
        {
            get
            {
                return (double)this.GetValue(FrameRateProperty);
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
        /// Gets or sets the render host internal.
        /// </summary>
        /// <value>
        /// The render host internal.
        /// </value>
        protected IRenderHost renderHostInternal;
        /// <summary>
        /// Gets or sets value for the shading model shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            get { return (IRenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether deferred shading is used
        ///// </summary>
        ///// <value>
        ///// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        ///// </value>
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
        public string TitleFontFamily
        {
            get
            {
                return (string)this.GetValue(TitleFontFamilyProperty);
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
        /// Gets or sets a value indicating whether this instance is view cube mover enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is view cube mover enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewCubeMoverEnabled
        {
            get { return (bool)GetValue(IsViewCubeMoverEnabledProperty); }
            set { SetValue(IsViewCubeMoverEnabledProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether coordinate system mover enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if coordinate system mover enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCoordinateSystemMoverEnabled
        {
            get { return (bool)GetValue(IsCoordinateSystemMoverEnabledProperty); }
            set { SetValue(IsCoordinateSystemMoverEnabledProperty, value); }
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

#if MSAA
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
        /// Rotate around this fixed rotation point only.<see cref="FixedRotationPointEnabled"/> 
        /// </summary>
        public Point3D FixedRotationPoint
        {
            set
            {
                SetValue(FixedRotationPointProperty, value);
            }
            get
            {
                return (Point3D)GetValue(FixedRotationPointProperty);
            }
        }

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
        /// Manually move camera to look at a point in 3D space. (Same as calling Viewport3DX.LookAt() function)
        /// Since camera may have been moved by mouse, the value gets does not reflect the actual point camera currently looking at.
        /// </summary>
        public Point3D ManualLookAtPoint
        {
            set
            {
                SetValue(ManualLookAtPointProperty, value);
            }
            get
            {
                return (Point3D)GetValue(ManualLookAtPointProperty);
            }
        }

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
        /// <para>Use HwndHost as rendering surface, swapchain for rendering. Much faster than using D3DImage.</para> 
        /// <para>Drawbacks: The rendering surface will cover all WPF controls in the same Viewport region. Move controls out of viewport region to solve this problem.</para>
        /// <para>For displaying ViewCube and CoordinateSystem, separate Model needs to create to render along with the other models. WPF viewport will not be visibled.</para>
        /// <para>Note: Enable deferred rendering will use seperate rendering thread or rendering.</para>
        /// </summary>
        public bool EnableSwapChainRendering
        {
            set
            {
                SetValue(EnableSwapChainRenderingProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableSwapChainRenderingProperty);
            }
        }

        /// <summary>
        /// Gets or sets the content2d.
        /// </summary>
        /// <value>
        /// The content2 d.
        /// </value>
        public Element2D Content2D
        {
            get
            {
                return (Element2D)GetValue(Content2DProperty);
            }
            set
            {
                SetValue(Content2DProperty, value);
            }
        }
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
        /// Gets or sets a value indicating whether [enable direct2D rendering]. Default is On
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render d2d]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableD2DRendering
        {
            get { return (bool)GetValue(EnableD2DRenderingProperty); }
            set { SetValue(EnableD2DRenderingProperty, value); }
        }

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
        /// Gets or sets a value indicating whether [enable design time rendering].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable design time rendering]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDesignModeRendering
        {
            get { return (bool)GetValue(EnableDesignModeRenderingProperty); }
            set { SetValue(EnableDesignModeRenderingProperty, value); }
        }

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
            get { return (bool)GetValue(EnableRenderOrderProperty); }
            set { SetValue(EnableRenderOrderProperty, value); }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelixViewport3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A control that contains a Viewport3D and a CameraController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A control that contains a <see cref="Viewport3D" /> and a <see cref="CameraController" />.
    /// </summary>
    [ContentProperty("Children")]
    [TemplatePart(Name = "PART_CameraController", Type = typeof(CameraController))]
    [TemplatePart(Name = "PART_ViewportGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_ViewCubeViewport", Type = typeof(Viewport3D))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class HelixViewport3D : ItemsControl, IHelixViewport3D
    {
        /// <summary>
        /// Identifies the <see cref="BackViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackViewGestureProperty =
            DependencyProperty.Register(
                "BackViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.B, ModifierKeys.Control)));

        /// <summary>
        /// Identifies the <see cref="BottomViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomViewGestureProperty =
            DependencyProperty.Register(
                "BottomViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.D, ModifierKeys.Control)));

        /// <summary>
        /// The camera changed event.
        /// </summary>
        public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
            "CameraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HelixViewport3D));

        /// <summary>
        /// Identifies the <see cref="CameraInertiaFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraInertiaFactorProperty =
            DependencyProperty.Register(
                "CameraInertiaFactor", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(0.93));

        /// <summary>
        /// Identifies the <see cref="CameraInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraInfoProperty = DependencyProperty.Register(
            "CameraInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CameraMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(HelixViewport3D), new UIPropertyMetadata(CameraMode.Inspect));

        /// <summary>
        /// Identifies the <see cref="CameraRotationMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(CameraRotationMode.Turntable, (s, e) => ((HelixViewport3D)s).OnCameraRotationModeChanged()));

        /// <summary>
        /// Identifies the <see cref="ChangeFieldOfViewCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewCursor",
                typeof(Cursor),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(Cursors.ScrollNS));

        /// <summary>
        /// Identifies the <see cref="ChangeFieldOfViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewGestureProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));

        /// <summary>
        /// Identifies the <see cref="ChangeLookAtGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeLookAtGestureProperty =
            DependencyProperty.Register(
                "ChangeLookAtGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightDoubleClick)));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemHeightProperty =
            DependencyProperty.Register(
                "CoordinateSystemHeight", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(80.0));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemHorizontalPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemHorizontalPositionProperty =
            DependencyProperty.Register(
                "CoordinateSystemHorizontalPosition",
                typeof(HorizontalAlignment),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemLabelForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelForegroundProperty =
            DependencyProperty.Register(
                "CoordinateSystemLabelForeground",
                typeof(Brush),
                typeof(HelixViewport3D),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemLabelX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelXProperty =
            DependencyProperty.Register(
                "CoordinateSystemLabelX", typeof(string), typeof(HelixViewport3D), new PropertyMetadata("X"));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemLabelY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelYProperty =
            DependencyProperty.Register(
                "CoordinateSystemLabelY", typeof(string), typeof(HelixViewport3D), new PropertyMetadata("Y"));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemLabelZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelZProperty =
            DependencyProperty.Register(
                "CoordinateSystemLabelZ", typeof(string), typeof(HelixViewport3D), new PropertyMetadata("Z"));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemVerticalPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemVerticalPositionProperty =
            DependencyProperty.Register(
                "CoordinateSystemVerticalPosition",
                typeof(VerticalAlignment),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// Identifies the <see cref="CoordinateSystemWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemWidthProperty =
            DependencyProperty.Register(
                "CoordinateSystemWidth", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(80.0));

        /// <summary>
        /// Identifies the CurrentPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(
                "CurrentPosition",
                typeof(Point3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(new Point3D(0, 0, 0)));

        /// <summary>
        /// Identifies the EnableCurrentPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableCurrentPositionProperty =
            DependencyProperty.Register(
                "EnableCurrentPosition", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CalculateCursorPosition"/> dependency property.
        /// It enables (true) or disables (false) the calculation of the cursor position in the 3D Viewport
        /// </summary>
        public static readonly DependencyProperty CalculateCursorPositionProperty =
            DependencyProperty.Register(
                "CalculateCursorPosition", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CursorPosition"/> dependency property.
        /// </summary>
        /// <remarks>
        /// The return value equals ConstructionPlanePosition or CursorModelSnapPosition if CursorSnapToModels is not null.
        /// </remarks>
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register(
                "CursorPosition",
                typeof(Point3D?),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="CursorOnElementPosition"/> dependency property.
        /// </summary>
        /// <remarks>
        /// This property returns the position of the nearest model.
        /// </remarks>
        public static readonly DependencyProperty CursorOnElementPositionProperty =
            DependencyProperty.Register(
                "CursorOnElementPosition",
                typeof(Point3D?),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="CursorOnConstructionPlanePosition"/> dependency property.
        /// </summary>
        /// <remarks>
        /// This property returns the point on the cursor plane..
        /// </remarks>
        public static readonly DependencyProperty CursorOnConstructionPlanePositionProperty =
            DependencyProperty.Register(
                "CursorOnConstructionPlanePosition",
                typeof(Point3D?),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="ConstructionPlane"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConstructionPlaneProperty =
            DependencyProperty.Register(
                "ConstructionPlane",
                typeof(Plane3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="CursorRay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorRayProperty =
            DependencyProperty.Register(
                "CursorRay",
                typeof(Ray3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new Ray3D(new Point3D(0, 0, 0), new Vector3D(0, 0, -1)), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="DebugInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DebugInfoProperty = DependencyProperty.Register(
            "DebugInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DefaultCamera"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FieldOfViewText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldOfViewTextProperty =
            DependencyProperty.Register(
                "FieldOfViewText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FrameRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(int), typeof(HelixViewport3D));

        /// <summary>
        /// Identifies the <see cref="FrameRateText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameRateTextProperty = DependencyProperty.Register(
            "FrameRateText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FrontViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontViewGestureProperty =
            DependencyProperty.Register(
                "FrontViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));

        /// <summary>
        /// Identifies the <see cref="InfiniteSpin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="InfoBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
            "InfoBackground",
            typeof(Brush),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))));

        /// <summary>
        /// Identifies the <see cref="InfoForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
            "InfoForeground", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="IsChangeFieldOfViewEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty =
            DependencyProperty.Register(
                "IsChangeFieldOfViewEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsHeadLightEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHeadlightEnabledProperty =
            DependencyProperty.Register(
                "IsHeadLightEnabled",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).OnHeadlightChanged()));

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty =
            DependencyProperty.Register(
                "IsInertiaEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsPanEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsMoveEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
            "IsMoveEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsViewCubeEdgeClicksEnabledProperty =
            DependencyProperty.Register("IsViewCubeEdgeClicksEnabled", typeof(bool), typeof(HelixViewport3D), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsRotationEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty =
            DependencyProperty.Register(
                "IsRotationEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsTouchZoomEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTouchZoomEnabledProperty =
            DependencyProperty.Register(
                "IsTouchZoomEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsZoomEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="LeftRightPanSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftRightPanSensitivityProperty =
            DependencyProperty.Register(
                "LeftRightPanSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="LeftRightRotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftRightRotationSensitivityProperty =
            DependencyProperty.Register(
                "LeftRightRotationSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="LeftViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftViewGestureProperty =
            DependencyProperty.Register(
                "LeftViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.L, ModifierKeys.Control)));

        /// <summary>
        /// The look at (target) point changed event
        /// </summary>
        public static readonly RoutedEvent LookAtChangedEvent = EventManager.RegisterRoutedEvent(
            "LookAtChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HelixViewport3D));

        /// <summary>
        /// Identifies the <see cref="MaximumFieldOfView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty =
            DependencyProperty.Register(
                "MaximumFieldOfView", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(140.0));

        /// <summary>
        /// Identifies the <see cref="MinimumFieldOfView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty =
            DependencyProperty.Register(
                "MinimumFieldOfView", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(5.0));

        /// <summary>
        /// Identifies the <see cref="ModelUpDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty =
            DependencyProperty.Register(
                "ModelUpDirection",
                typeof(Vector3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new Vector3D(0, 0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="Orthographic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrthographicProperty = DependencyProperty.Register(
            "Orthographic", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).OnOrthographicChanged()));

        /// <summary>
        /// Identifies the <see cref="OrthographicToggleGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrthographicToggleGestureProperty =
            DependencyProperty.Register(
                "OrthographicToggleGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PageUpDownZoomSensitivityProperty =
            DependencyProperty.Register(
                "PageUpDownZoomSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="PanCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.Hand));

        /// <summary>
        /// Identifies the <see cref="PanGesture2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanGesture2Property = DependencyProperty.Register(
            "PanGesture2",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.MiddleClick)));

        /// <summary>
        /// Identifies the <see cref="PanGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanGestureProperty = DependencyProperty.Register(
            "PanGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Shift)));

        /// <summary>
        /// Identifies the <see cref="ResetCameraGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResetCameraGestureProperty =
            DependencyProperty.Register(
                "ResetCameraGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.MiddleDoubleClick)));

        /// <summary>
        /// Identifies the <see cref="ResetCameraKeyGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResetCameraKeyGestureProperty =
            DependencyProperty.Register(
                "ResetCameraKeyGesture",
                typeof(KeyGesture),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new KeyGesture(Key.Home), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="RightViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightViewGestureProperty =
            DependencyProperty.Register(
                "RightViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.R, ModifierKeys.Control)));

        /// <summary>
        /// Identifies the <see cref="RotateAroundMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "RotateAroundMouseDownPoint", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FixedRotationPointEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointEnabledProperty =
            DependencyProperty.Register(
                "FixedRotationPointEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FixedRotationPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointProperty =
            DependencyProperty.Register(
                "FixedRotationPoint", typeof(Point3D), typeof(HelixViewport3D), new UIPropertyMetadata(default(Point3D)));

        /// <summary>
        /// Identifies the <see cref="RotateCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.SizeAll));

        /// <summary>
        /// Identifies the <see cref="RotateGesture2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateGesture2Property = DependencyProperty.Register(
            "RotateGesture2", typeof(MouseGesture), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RotateGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateGestureProperty = DependencyProperty.Register(
            "RotateGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick)));

        /// <summary>
        /// Identifies the <see cref="RotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register(
                "RotationSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ShowCameraInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraInfoProperty = DependencyProperty.Register(
            "ShowCameraInfo",
            typeof(bool),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).UpdateCameraInfo()));

        /// <summary>
        /// Identifies the <see cref="ShowCameraTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register(
                "ShowCameraTarget", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowCoordinateSystem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCoordinateSystemProperty =
            DependencyProperty.Register(
                "ShowCoordinateSystem", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="ShowFieldOfView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowFieldOfViewProperty =
            DependencyProperty.Register(
                "ShowFieldOfView",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).UpdateFieldOfViewInfo()));

        /// <summary>
        /// Identifies the <see cref="ShowFrameRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
            "ShowFrameRate",
            typeof(bool),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).OnShowFrameRateChanged()));

        /// <summary>
        /// Identifies the <see cref="ShowTriangleCountInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTriangleCountInfoProperty =
            DependencyProperty.Register(
                "ShowTriangleCountInfo",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, (s, e) => ((HelixViewport3D)s).OnShowTriangleCountInfoChanged()));

        /// <summary>
        /// Identifies the <see cref="ShowViewCube"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
            "ShowViewCube", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Status"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SubTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SubTitleSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubTitleSizeProperty = DependencyProperty.Register(
            "SubTitleSize", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(12.0));

        /// <summary>
        /// Identifies the <see cref="TextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
            "TextBrush", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="TitleBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register(
                "TitleBackground", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TitleFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register(
                "TitleFontFamily", typeof(FontFamily), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TitleSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register(
            "TitleSize", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(12.0));

        /// <summary>
        /// Identifies the <see cref="TopViewGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopViewGestureProperty = DependencyProperty.Register(
            "TopViewGesture",
            typeof(InputGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new KeyGesture(Key.U, ModifierKeys.Control)));

        /// <summary>
        /// Identifies the <see cref="TriangleCountInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TriangleCountInfoProperty =
            DependencyProperty.Register(
                "TriangleCountInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="UpDownPanSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDownPanSensitivityProperty =
            DependencyProperty.Register(
                "UpDownPanSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="UpDownRotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDownRotationSensitivityProperty =
            DependencyProperty.Register(
                "UpDownRotationSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ViewCubeBackText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBackTextProperty =
            DependencyProperty.Register(
                "ViewCubeBackText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("B"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeBottomText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBottomTextProperty =
            DependencyProperty.Register(
                "ViewCubeBottomText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("D"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeFrontText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeFrontTextProperty =
            DependencyProperty.Register(
                "ViewCubeFrontText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("F"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeHeightProperty = DependencyProperty.Register(
            "ViewCubeHeight", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(80.0));

        /// <summary>
        /// Identifies the <see cref="ViewCubeHorizontalPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeHorizontalPositionProperty =
            DependencyProperty.Register(
                "ViewCubeHorizontalPosition",
                typeof(HorizontalAlignment),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(HorizontalAlignment.Right));

        /// <summary>
        /// Identifies the <see cref="ViewCubeLeftText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeLeftTextProperty =
            DependencyProperty.Register(
                "ViewCubeLeftText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("L"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeOpacityProperty =
            DependencyProperty.Register(
                "ViewCubeOpacity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(0.5));

        /// <summary>
        /// Identifies the <see cref="ViewCubeRightText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeRightTextProperty =
            DependencyProperty.Register(
                "ViewCubeRightText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("R"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeTopText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeTopTextProperty =
            DependencyProperty.Register(
                "ViewCubeTopText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("U"));

        /// <summary>
        /// Identifies the <see cref="ViewCubeVerticalPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeVerticalPositionProperty =
            DependencyProperty.Register(
                "ViewCubeVerticalPosition",
                typeof(VerticalAlignment),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// Identifies the <see cref="ViewCubeWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeWidthProperty = DependencyProperty.Register(
            "ViewCubeWidth", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(80.0));

        /// <summary>
        /// Identifies the <see cref="ZoomAroundMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "ZoomAroundMouseDownPoint", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="SnapMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SnapMouseDownPointProperty =
            DependencyProperty.Register(
                "SnapMouseDownPoint", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ZoomCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.SizeNS));

        /// <summary>
        /// Identifies the <see cref="ZoomExtentsGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsGestureProperty =
            DependencyProperty.Register(
                "ZoomExtentsGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// Identifies the <see cref="ZoomExtentsWhenLoaded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty =
            DependencyProperty.Register(
                "ZoomExtentsWhenLoaded", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="ZoomGesture2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomGesture2Property = DependencyProperty.Register(
            "ZoomGesture2", typeof(MouseGesture), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ZoomGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomGestureProperty = DependencyProperty.Register(
            "ZoomGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Control)));

        /// <summary>
        /// Identifies the <see cref="ZoomRectangleCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty =
            DependencyProperty.Register(
                "ZoomRectangleCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.ScrollSE));

        /// <summary>
        /// Identifies the <see cref="ZoomRectangleGesture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleGestureProperty =
            DependencyProperty.Register(
                "ZoomRectangleGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(
                    new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        /// Identifies the <see cref="ZoomSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(
                "ZoomSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ZoomedByRectangle"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ZoomedByRectangleEvent = EventManager.RegisterRoutedEvent(
            "ZoomedByRectangle", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HelixViewport3D));

        /// <summary>
        /// The limit FPS property
        /// </summary>
        public static readonly DependencyProperty LimitFPSProperty =
            DependencyProperty.Register("LimitFPS", typeof(bool), typeof(HelixViewport3D), new PropertyMetadata(true, (d,e)=> 
            {
                (d as HelixViewport3D).limitFPS = (bool)e.NewValue;
            }));

        /// <summary>
        /// The adorner layer name.
        /// </summary>
        private const string PartAdornerLayer = "PART_AdornerLayer";

        /// <summary>
        /// The viewport grid name.
        /// </summary>
        private const string PartViewportGrid = "PART_ViewportGrid";

        /// <summary>
        /// The camera controller name.
        /// </summary>
        private const string PartCameraController = "PART_CameraController";

        /// <summary>
        /// The coordinate view name.
        /// </summary>
        private const string PartCoordinateView = "PART_CoordinateView";

        /// <summary>
        /// The view cube name.
        /// </summary>
        private const string PartViewCube = "PART_ViewCube";

        /// <summary>
        /// The view cube viewport name.
        /// </summary>
        private const string PartViewCubeViewport = "PART_ViewCubeViewport";

        /// <summary>
        /// The frame rate stopwatch.
        /// </summary>
        private readonly Stopwatch fpsWatch = new Stopwatch();

        /// <summary>
        /// The headlight.
        /// </summary>
        private readonly DirectionalLight headLight = new DirectionalLight { Color = Colors.White };

        /// <summary>
        /// The lights.
        /// </summary>
        private readonly Model3DGroup lights;

        /// <summary>
        /// The orthographic camera.
        /// </summary>
        private readonly OrthographicCamera orthographicCamera;

        /// <summary>
        /// The perspective camera.
        /// </summary>
        private readonly PerspectiveCamera perspectiveCamera;

        /// <summary>
        /// The rendering event listener.
        /// </summary>
        private readonly RenderingEventListener renderingEventListener;

        /// <summary>
        /// The viewport.
        /// </summary>
        private readonly Viewport3D viewport;

        /// <summary>
        /// The adorner layer.
        /// </summary>
        private AdornerDecorator adornerLayer;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// The coordinate system lights.
        /// </summary>
        private Model3DGroup coordinateSystemLights;

        /// <summary>
        /// The coordinate view.
        /// </summary>
        private Viewport3D coordinateView;

        /// <summary>
        /// The current camera.
        /// </summary>
        private Camera currentCamera;

        /// <summary>
        /// The frame counter.
        /// </summary>
        private int frameCounter;

        /// <summary>
        /// The "control has been loaded before" flag.
        /// </summary>
        private bool hasBeenLoadedBefore;

        /// <summary>
        /// The frame counter for info field updates.
        /// </summary>
        private int infoFrameCounter;

        /// <summary>
        /// The is subscribed to rendering event.
        /// </summary>
        private bool isSubscribedToRenderingEvent;

        /// <summary>
        /// The view cube.
        /// </summary>
        private ViewCubeVisual3D viewCube;

        /// <summary>
        /// The view cube lights.
        /// </summary>
        private Model3DGroup viewCubeLights;

        /// <summary>
        /// The view cube view.
        /// </summary>
        private Viewport3D viewCubeViewport;

        /// <summary>
        /// Initializes static members of the <see cref="HelixViewport3D"/> class.
        /// </summary>
        static HelixViewport3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(HelixViewport3D), new FrameworkPropertyMetadata(typeof(HelixViewport3D)));
            ClipToBoundsProperty.OverrideMetadata(typeof(HelixViewport3D), new FrameworkPropertyMetadata(true));
            OrthographicToggleCommand = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelixViewport3D"/> class.
        /// </summary>
        public HelixViewport3D()
        {
            // The Viewport3D must be created here since the Children collection is attached directly
            this.viewport = new Viewport3D();

            // viewport.SetBinding(UIElement.IsHitTestVisibleProperty, new Binding("IsViewportHitTestVisible") { Source = this });
            // viewport.SetBinding(UIElement.ClipToBoundsProperty, new Binding("ClipToBounds") { Source = this });

            // headlight
            this.lights = new Model3DGroup();
            this.viewport.Children.Add(new ModelVisual3D { Content = this.lights });

            this.perspectiveCamera = new PerspectiveCamera();
            this.orthographicCamera = new OrthographicCamera();
            this.perspectiveCamera.Reset();
            this.orthographicCamera.Reset();

            this.Camera = this.Orthographic ? (ProjectionCamera)this.orthographicCamera : this.perspectiveCamera;

            // http://blogs.msdn.com/wpfsdk/archive/2007/01/15/maximizing-wpf-3d-performance-on-tier-2-hardware.aspx
            // RenderOptions.EdgeMode?

            // start a watch for FPS calculations
            this.fpsWatch.Start();

            this.Loaded += this.OnControlLoaded;
            this.Unloaded += this.OnControlUnloaded;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, this.CopyHandler));
            this.CommandBindings.Add(new CommandBinding(OrthographicToggleCommand, this.OrthographicToggle));
            this.renderingEventListener = new RenderingEventListener(this.CompositionTargetRendering);
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
        /// Occurs when the look at/target point changed.
        /// </summary>
        public event RoutedEventHandler LookAtChanged
        {
            add
            {
                this.AddHandler(LookAtChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(LookAtChangedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when the view is zoomed by rectangle.
        /// </summary>
        public event RoutedEventHandler ZoomedByRectangle
        {
            add
            {
                this.AddHandler(ZoomedByRectangleEvent, value);
            }

            remove
            {
                this.RemoveHandler(ZoomedByRectangleEvent, value);
            }
        }

        /// <summary>
        /// Gets the command that toggles between orthographic and perspective camera.
        /// </summary>
        public static RoutedCommand OrthographicToggleCommand { get; private set; }

        /// <summary>
        /// Gets or sets the back view gesture.
        /// </summary>
        /// <value>
        /// The back view gesture.
        /// </value>
        public InputGesture BackViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(BackViewGestureProperty);
            }

            set
            {
                this.SetValue(BackViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom view gesture.
        /// </summary>
        /// <value>
        /// The bottom view gesture.
        /// </value>
        public InputGesture BottomViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(BottomViewGestureProperty);
            }

            set
            {
                this.SetValue(BottomViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public ProjectionCamera Camera
        {
            get
            {
                return this.Viewport.Camera as ProjectionCamera;
            }

            set
            {
                if (this.currentCamera != null)
                {
                    this.currentCamera.Changed -= this.CameraPropertyChanged;
                }

                this.Viewport.Camera = value;

                this.currentCamera = this.Viewport.Camera;
                this.currentCamera.Changed += this.CameraPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the camera controller.
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
        /// Gets the camera info.
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

            private set
            {
                this.SetValue(CameraInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CameraMode" />
        /// </summary>
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
        /// Gets or sets the cursor used when changing field of view.
        /// </summary>
        /// <value>
        /// A <see cref="Cursor"/>.
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
        /// Gets or sets the change field of view gesture.
        /// </summary>
        /// <value>
        /// The change field of view gesture.
        /// </value>
        public MouseGesture ChangeFieldOfViewGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ChangeFieldOfViewGestureProperty);
            }

            set
            {
                this.SetValue(ChangeFieldOfViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the change look-at gesture.
        /// </summary>
        /// <value>
        /// The change look-at gesture.
        /// </value>
        public MouseGesture ChangeLookAtGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ChangeLookAtGestureProperty);
            }

            set
            {
                this.SetValue(ChangeLookAtGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public Visual3DCollection Children
        {
            get
            {
                return this.viewport.Children;
            }
        }

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
        /// Gets or sets the coordinate system X label.
        /// </summary>
        /// <value>
        /// The coordinate system X label.
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
        /// Gets or sets the coordinate system Y label.
        /// </summary>
        /// <value>
        /// The coordinate system Y label.
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
        /// Gets or sets the coordinate system Z label.
        /// </summary>
        /// <value>
        /// The coordinate system Z label.
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
        /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
        /// </remarks>
        [Obsolete("CurrentPosition is now obsolete, please use CursorPosition instead", false)]
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
        /// Gets or sets a value indicating whether calculation of the <see cref="CurrentPosition" /> property is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
        /// </value>
        [Obsolete("EnableCurrentPosition is now obsolete, please use CalculateCursorPosition instead", false)]
        public bool EnableCurrentPosition
        {
            get
            {
                return this.CalculateCursorPosition;
            }

            set
            {
                this.CalculateCursorPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether calculation of the <see cref="CursorPosition" /> properties is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool CalculateCursorPosition
        {
            get
            {
                return (bool)this.GetValue(CalculateCursorPositionProperty);
            }

            set
            {
                this.SetValue(CalculateCursorPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets the current cursor position.
        /// </summary>
        /// <value>
        /// The current cursor position.
        /// </value>
        /// <remarks>
        /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
        /// </remarks>
        public Point3D? CursorPosition
        {
            get
            {
                return (Point3D?)this.GetValue(CursorPositionProperty);
            }

            private set
            {
                this.SetValue(CursorPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets the current cursor position on the cursor plane.
        /// </summary>
        /// <value>
        /// The cursor plane position.
        /// </value>
        /// <remarks>
        /// The <see cref="CalculateCursorPosition" /> property must be set to true to enable updating of this property.
        /// </remarks>
        public Point3D? CursorOnConstructionPlanePosition
        {
            get
            {
                return (Point3D?)this.GetValue(CursorOnConstructionPlanePositionProperty);
            }

            private set
            {
                this.SetValue(CursorOnConstructionPlanePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the plane that defines the <see cref="CursorOnConstructionPlanePosition" />.
        /// </summary>
        /// <value>
        /// The plane used to calculate the <see cref="CursorOnConstructionPlanePosition" />.
        /// </value>
        public Plane3D ConstructionPlane
        {
            get
            {
                return (Plane3D)this.GetValue(ConstructionPlaneProperty);
            }

            set
            {
                this.SetValue(ConstructionPlaneProperty, value);
            }
        }

        /// <summary>
        /// Gets the cursor ray.
        /// </summary>
        /// <value>
        /// The cursor ray.
        /// </value>
        public Ray3D CursorRay
        {
            get
            {
                return (Ray3D)this.GetValue(CursorRayProperty);
            }

            private set
            {
                this.SetValue(CursorRayProperty, value);
            }
        }

        /// <summary>
        /// Gets the current cursor position on the nearest model. If the model is not hit, the position is <c>null</c>.
        /// </summary>
        /// <value>
        /// The position of the model intersection.
        /// </value>
        /// <remarks>
        /// The <see cref="CalculateCursorPosition" /> property must be set to <c>true</c> to enable updating of this property.
        /// </remarks>
        public Point3D? CursorOnElementPosition
        {
            get
            {
                return (Point3D?)this.GetValue(CursorOnElementPositionProperty);
            }

            private set
            {
                this.SetValue(CursorOnElementPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets the field of view text.
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

            private set
            {
                this.SetValue(FieldOfViewTextProperty, value);
            }
        }

        /// <summary>
        /// Gets the frame rate.
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

            private set
            {
                this.SetValue(FrameRateProperty, value);
            }
        }

        /// <summary>
        /// Gets the frame rate text.
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

            private set
            {
                this.SetValue(FrameRateTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front view gesture.
        /// </summary>
        /// <value>
        /// The front view gesture.
        /// </value>
        public InputGesture FrontViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(FrontViewGestureProperty);
            }

            set
            {
                this.SetValue(FrontViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable infinite spin.
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
        /// Gets or sets a value indicating whether change of field-of-view is enabled.
        /// </summary>
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
        /// Gets or sets a value indicating whether the head light is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the head light is enabled; otherwise, <c>false</c> .
        /// </value>
        public bool IsHeadLightEnabled
        {
            get
            {
                return (bool)this.GetValue(IsHeadlightEnabledProperty);
            }

            set
            {
                this.SetValue(IsHeadlightEnabledProperty, value);
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
        /// Gets or sets a value indicating whether move (by AWSD keys) is enabled.
        /// </summary>
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
        /// Gets or sets the view cube edge clickable.
        /// </summary>
        public bool IsViewCubeEdgeClicksEnabled
        {
            get { return (bool)GetValue(IsViewCubeEdgeClicksEnabledProperty); }
            set { SetValue(IsViewCubeEdgeClicksEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether rotation is enabled.
        /// </summary>
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
        ///     <c>true</c> if touch zoom is enabled; otherwise, <c>false</c> .
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
        /// Gets or sets the left view gesture.
        /// </summary>
        /// <value>
        /// The left view gesture.
        /// </value>
        public InputGesture LeftViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(LeftViewGestureProperty);
            }

            set
            {
                this.SetValue(LeftViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets the lights.
        /// </summary>
        /// <value>
        /// The lights.
        /// </value>
        public Model3DGroup Lights
        {
            get
            {
                return this.lights;
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
        /// Gets or sets the up direction of the model. This is used by the view cube.
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
        /// Gets or sets a value indicating whether this <see cref="HelixViewport3D" /> should use an orthographic camera.
        /// </summary>
        /// <value>
        ///     <c>true</c> if an orthographic camera should be used; otherwise, <c>false</c> .
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
        /// Gets or sets the orthographic toggle gesture.
        /// </summary>
        /// <value>
        /// The orthographic toggle gesture.
        /// </value>
        public InputGesture OrthographicToggleGesture
        {
            get
            {
                return (InputGesture)this.GetValue(OrthographicToggleGestureProperty);
            }

            set
            {
                this.SetValue(OrthographicToggleGestureProperty, value);
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
        /// Gets or sets the pan gesture.
        /// </summary>
        /// <value>
        /// The pan gesture.
        /// </value>
        public MouseGesture PanGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(PanGestureProperty);
            }

            set
            {
                this.SetValue(PanGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alternative pan gesture.
        /// </summary>
        /// <value>
        /// The alternative pan gesture.
        /// </value>
        public MouseGesture PanGesture2
        {
            get
            {
                return (MouseGesture)this.GetValue(PanGesture2Property);
            }

            set
            {
                this.SetValue(PanGesture2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the reset camera gesture.
        /// </summary>
        public InputGesture ResetCameraGesture
        {
            get
            {
                return (InputGesture)this.GetValue(ResetCameraGestureProperty);
            }

            set
            {
                this.SetValue(ResetCameraGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the reset camera key gesture.
        /// </summary>
        /// <value>
        /// The reset camera key gesture.
        /// </value>
        public KeyGesture ResetCameraKeyGesture
        {
            get
            {
                return (KeyGesture)this.GetValue(ResetCameraKeyGestureProperty);
            }

            set
            {
                this.SetValue(ResetCameraKeyGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right view gesture.
        /// </summary>
        /// <value>
        /// The right view gesture.
        /// </value>
        public InputGesture RightViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(RightViewGestureProperty);
            }

            set
            {
                this.SetValue(RightViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value>
        ///     <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> .
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
        /// Gets or sets a value indicating whether to rotate around a fixed point.
        /// </summary>
        /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
        public bool FixedRotationPointEnabled
        {
            get
            {
                return (bool)this.GetValue(FixedRotationPointEnabledProperty);
            }

            set
            {
                this.SetValue(FixedRotationPointEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the center of rotation.
        /// </summary>
        /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
        public Point3D FixedRotationPoint
        {
            get
            {
                return (Point3D)this.GetValue(FixedRotationPointProperty);
            }

            set
            {
                this.SetValue(FixedRotationPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation cursor.
        /// </summary>
        /// <value>
        /// The rotation cursor.
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
        /// Gets or sets the rotation gesture.
        /// </summary>
        /// <value>
        /// The rotation gesture.
        /// </value>
        public MouseGesture RotateGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(RotateGestureProperty);
            }

            set
            {
                this.SetValue(RotateGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alternative rotation gesture.
        /// </summary>
        /// <value>
        /// The alternative rotation gesture.
        /// </value>
        public MouseGesture RotateGesture2
        {
            get
            {
                return (MouseGesture)this.GetValue(RotateGesture2Property);
            }

            set
            {
                this.SetValue(RotateGesture2Property, value);
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
        ///     <c>true</c> if the camera info should be shown; otherwise, <c>false</c> .
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
        ///     <c>true</c> if the camera target adorner should be shown; otherwise, <c>false</c> .
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
        ///     <c>true</c> if the coordinate system should be shown; otherwise, <c>false</c> .
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
        /// Gets or sets a value indicating whether to show the current field of view.
        /// </summary>
        /// <value>
        ///     <c>true</c> if field of view should be shown; otherwise, <c>false</c> .
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
        /// Gets or sets a value indicating whether to show the frame rate.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the frame rate should be shown; otherwise, <c>false</c> .
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
        ///     <c>true</c> if the view cube should be shown; otherwise, <c>false</c> .
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
        /// Gets or sets the top view gesture.
        /// </summary>
        /// <value>
        /// The top view gesture.
        /// </value>
        public InputGesture TopViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(TopViewGestureProperty);
            }

            set
            {
                this.SetValue(TopViewGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets information about the triangle count.
        /// </summary>
        public string TriangleCountInfo
        {
            get
            {
                return (string)this.GetValue(TriangleCountInfoProperty);
            }

            private set
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
        /// Gets or sets the opacity of the view cube when inactive.
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
        /// Gets the viewport.
        /// </summary>
        /// <value>
        /// The viewport.
        /// </value>
        public Viewport3D Viewport
        {
            get
            {
                return this.viewport;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around the mouse down point.
        /// </summary>
        /// <value>
        ///     <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> .
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
        /// Gets or sets a value indicating whether to snap the mouse down point to a model.
        /// </summary>
        /// <value> <c>true</c> if snapping the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool SnapMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(SnapMouseDownPointProperty);
            }

            set
            {
                this.SetValue(SnapMouseDownPointProperty, value);
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
        /// Gets or sets the zoom extents gesture.
        /// </summary>
        public InputGesture ZoomExtentsGesture
        {
            get
            {
                return (InputGesture)this.GetValue(ZoomExtentsGestureProperty);
            }

            set
            {
                this.SetValue(ZoomExtentsGestureProperty, value);
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
        /// Gets or sets the zoom gesture.
        /// </summary>
        /// <value>
        /// The zoom gesture.
        /// </value>
        public MouseGesture ZoomGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ZoomGestureProperty);
            }

            set
            {
                this.SetValue(ZoomGestureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alternative zoom gesture.
        /// </summary>
        /// <value>
        /// The alternative zoom gesture.
        /// </value>
        public MouseGesture ZoomGesture2
        {
            get
            {
                return (MouseGesture)this.GetValue(ZoomGesture2Property);
            }

            set
            {
                this.SetValue(ZoomGesture2Property, value);
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
        /// Gets or sets the zoom rectangle gesture.
        /// </summary>
        /// <value>
        /// The zoom rectangle gesture.
        /// </value>
        public MouseGesture ZoomRectangleGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ZoomRectangleGestureProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleGestureProperty, value);
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

        /// <summary>
        /// Gets or sets a value indicating whether [limit FPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [limit FPS]; otherwise, <c>false</c>.
        /// </value>
        public bool LimitFPS
        {
            get { return (bool)GetValue(LimitFPSProperty); }
            set { SetValue(LimitFPSProperty, value); }
        }

        #region Private Variables
        private bool limitFPS = true;
        private TimeSpan prevTime;
        #endregion
        /// <summary>
        /// Changes the camera direction.
        /// </summary>
        /// <param name="newDirection">
        /// The new direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ChangeCameraDirection(Vector3D newDirection, double animationTime = 0)
        {
            if (this.cameraController != null)
            {
                this.cameraController.ChangeDirection(newDirection, animationTime);
            }
        }

        /// <summary>
        /// Copies the view to the clipboard.
        /// </summary>
        public void Copy()
        {
            this.Viewport.Copy(this.Viewport.ActualWidth * 2, this.Viewport.ActualHeight * 2, Brushes.White, 2);
        }

        /// <summary>
        /// Copies the view to the clipboard as <c>xaml</c>.
        /// </summary>
        public void CopyXaml()
        {
            Clipboard.SetText(XamlHelper.GetXaml(this.Viewport.Children));
        }

        /// <summary>
        /// Exports the view to the specified file.
        /// </summary>
        /// <remarks>
        /// Exporters.Filter contains all supported export file types.
        /// </remarks>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        public void Export(string fileName)
        {
            this.Viewport.Export(fileName, this.Background);
        }

        /// <summary>
        /// Exports the view to a stereo image with the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stereoBase">The stereo base.</param>
        public void ExportStereo(string fileName, double stereoBase)
        {
            this.Viewport.ExportStereo(fileName, stereoBase, this.Background);
        }

        /// <summary>
        /// Finds the nearest object.
        /// </summary>
        /// <param name="pt">
        /// The 3D position.
        /// </param>
        /// <param name="pos">
        /// The 2D position.
        /// </param>
        /// <param name="normal">
        /// The normal at the hit point.
        /// </param>
        /// <param name="obj">
        /// The object that was hit.
        /// </param>
        /// <returns>
        /// <c>True</c> if an object was hit.
        /// </returns>
        public bool FindNearest(Point pt, out Point3D pos, out Vector3D normal, out DependencyObject obj)
        {
            return this.Viewport.FindNearest(pt, out pos, out normal, out obj);
        }

        /// <summary>
        /// Finds the nearest point.
        /// </summary>
        /// <param name="pt">
        /// The point.
        /// </param>
        /// <returns>
        /// A point.
        /// </returns>
        public Point3D? FindNearestPoint(Point pt)
        {
            return this.Viewport.FindNearestPoint(pt);
        }

        /// <summary>
        /// Finds the <see cref="Visual3D" /> that is nearest the camera ray through the specified point.
        /// </summary>
        /// <param name="pt">
        /// The point.
        /// </param>
        /// <returns>
        /// The nearest <see cref="Visual3D" /> or <c>null</c> if no visual was hit.
        /// </returns>
        public Visual3D FindNearestVisual(Point pt)
        {
            return this.Viewport.FindNearestVisual(pt);
        }

        /// <summary>
        /// Changes the camera to look at the specified point.
        /// </summary>
        /// <param name="target">
        /// The point.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Point3D target, double animationTime = 0)
        {
            this.Camera.LookAt(target, animationTime);
        }

        /// <summary>
        /// Changes the camera to look at the specified point.
        /// </summary>
        /// <param name="target">
        /// The target point.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Point3D target, double distance, double animationTime)
        {
            this.Camera.LookAt(target, distance, animationTime);
        }

        /// <summary>
        /// Changes the camera to look at the specified point.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public void LookAt(Point3D target, Vector3D direction, double animationTime)
        {
            this.Camera.LookAt(target, direction, animationTime);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call
        /// <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />
        /// .
        /// </summary>
        /// <exception cref="HelixToolkitException">
        /// A part is missing from the template.
        /// </exception>
        public override void OnApplyTemplate()
        {
            if (this.adornerLayer == null)
            {
                this.adornerLayer = this.Template.FindName(PartAdornerLayer, this) as AdornerDecorator;
            }

            var grid = this.Template.FindName(PartViewportGrid, this) as Grid;
            if (grid == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartViewportGrid);
            }

            grid.Children.Add(this.viewport);

            if (this.adornerLayer == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartAdornerLayer);
            }

            if (this.cameraController == null)
            {
                this.cameraController = this.Template.FindName(PartCameraController, this) as CameraController;
                if (this.cameraController != null)
                {
                    this.cameraController.Viewport = this.Viewport;
                    this.cameraController.LimitFPS = this.limitFPS;
                    this.cameraController.LookAtChanged += (s, e) => this.OnLookAtChanged();
                    this.cameraController.ZoomedByRectangle += (s, e) => this.OnZoomedByRectangle();
                }
            }

            if (this.cameraController == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartCameraController);
            }

            if (this.coordinateView == null)
            {
                this.coordinateView = this.Template.FindName(PartCoordinateView, this) as Viewport3D;

                this.coordinateSystemLights = new Model3DGroup();

                // coordinateSystemLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
                // coordinateSystemLights.Children.Add(new AmbientLight(Colors.DarkGray));
                this.coordinateSystemLights.Children.Add(new AmbientLight(Colors.LightGray));

                if (this.coordinateView != null)
                {
                    this.coordinateView.Camera = new PerspectiveCamera();
                    this.coordinateView.Children.Add(new ModelVisual3D { Content = this.coordinateSystemLights });
                }
            }

            if (this.coordinateView == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartCoordinateView);
            }

            if (this.viewCubeViewport == null)
            {
                this.viewCubeViewport = this.Template.FindName(PartViewCubeViewport, this) as Viewport3D;

                this.viewCubeLights = new Model3DGroup();
                this.viewCubeLights.Children.Add(new AmbientLight(Colors.White));
                if (this.viewCubeViewport != null)
                {
                    this.viewCubeViewport.Camera = new PerspectiveCamera();
                    this.viewCubeViewport.Children.Add(new ModelVisual3D { Content = this.viewCubeLights });
                    this.viewCubeViewport.MouseEnter += this.ViewCubeViewportMouseEnter;
                    this.viewCubeViewport.MouseLeave += this.ViewCubeViewportMouseLeave;
                }

                this.viewCube = this.Template.FindName(PartViewCube, this) as ViewCubeVisual3D;
                if (this.viewCube != null)
                {
                    this.viewCube.Viewport = this.Viewport;
                }
            }

            // update the coordinateview camera
            this.OnCameraChanged();

            // add the default headlight
            this.OnHeadlightChanged();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public void ResetCamera()
        {
            if (this.cameraController != null)
            {
                this.cameraController.ResetCamera();
            }
        }

        /// <summary>
        /// Sets the camera position and orientation.
        /// </summary>
        /// <param name="newPosition">
        /// The new camera position.
        /// </param>
        /// <param name="newDirection">
        /// The new camera look direction.
        /// </param>
        /// <param name="newUpDirection">
        /// The new camera up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void SetView(Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime = 0)
        {
            this.Camera.AnimateTo(newPosition, newDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Sets the camera orientation and adjusts the camera position to fit the model into the view.
        /// </summary>
        /// <param name="newDirection">The new camera look direction.</param>
        /// <param name="newUpDirection">The new camera up direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public void FitView(Vector3D newDirection, Vector3D newUpDirection, double animationTime = 0)
        {
            this.Camera.FitView(this.Viewport, newDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the screen.
        /// </summary>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ZoomExtents(double animationTime = 0)
        {
            if (this.cameraController != null)
            {
                this.cameraController.ZoomExtents(animationTime);
            }
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding box.
        /// </summary>
        /// <param name="bounds">
        /// The bounding box.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ZoomExtents(Rect3D bounds, double animationTime = 0)
        {
            this.Camera.ZoomExtents(this.Viewport, bounds, animationTime);
        }

        /// <summary>
        /// Raises the LookAtChanged event.
        /// </summary>
        protected internal virtual void OnLookAtChanged()
        {
            var args = new RoutedEventArgs(LookAtChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Raises the ZoomedByRectangle event.
        /// </summary>
        protected internal virtual void OnZoomedByRectangle()
        {
            var args = new RoutedEventArgs(ZoomedByRectangleEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Handles camera changes.
        /// </summary>
        protected virtual void OnCameraChanged()
        {
            // update the camera of the coordinate system
            if (this.coordinateView != null)
            {
                this.Camera.CopyDirectionOnly(this.coordinateView.Camera as PerspectiveCamera, 30);
            }

            // update the camera of the view cube
            if (this.viewCubeViewport != null)
            {
                this.Camera.CopyDirectionOnly(this.viewCubeViewport.Camera as PerspectiveCamera, 20);
            }

            // update the headlight and coordinate system light
            if (this.Camera != null)
            {
                if (this.headLight != null)
                {
                    this.headLight.Direction = this.Camera.LookDirection;
                }

                if (this.coordinateSystemLights != null)
                {
                    var cshl = this.coordinateSystemLights.Children[0] as DirectionalLight;
                    if (cshl != null)
                    {
                        cshl.Direction = this.Camera.LookDirection;
                    }
                }
            }

            if (this.ShowFieldOfView)
            {
                this.UpdateFieldOfViewInfo();
            }

            if (this.ShowCameraInfo)
            {
                this.UpdateCameraInfo();
            }
        }

        /// <summary>
        /// Handles changes to the <see cref="IsHeadLightEnabled" /> property.
        /// </summary>
        protected void OnHeadlightChanged()
        {
            if (this.lights == null)
            {
                return;
            }

            if (this.IsHeadLightEnabled && !this.lights.Children.Contains(this.headLight))
            {
                this.lights.Children.Add(this.headLight);
            }

            if (!this.IsHeadLightEnabled && this.lights.Children.Contains(this.headLight))
            {
                this.lights.Children.Remove(this.headLight);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items" /> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        /// <exception cref="System.NotImplementedException">
        /// Move operation not implemented.
        /// or
        /// Replace operation not implemented.
        /// </exception>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException("Move operation not implemented.");
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace operation not implemented.");
                case NotifyCollectionChangedAction.Reset:
                    this.Children.Clear();
                    break;
            }
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property changes.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.
        /// </param>
        /// <param name="newValue">
        /// New value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.
        /// </param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.RemoveItems(oldValue);
            this.AddItems(newValue);
        }

        /// <summary>
        /// Invoked when an unhandled MouseMove attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.CalculateCursorPosition)
            {
                var pt = e.GetPosition(this);
                this.UpdateCursorPosition(pt);
            }
        }

        /// <summary>
        /// Raises the camera changed event.
        /// </summary>
        protected virtual void RaiseCameraChangedEvent()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(CameraChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Updates the cursor position.
        /// </summary>
        /// <param name="pt">The position of the cursor (in viewport coordinates).</param>
        private void UpdateCursorPosition(Point pt)
        {
            this.CursorOnElementPosition = this.FindNearestPoint(pt);
            this.CursorPosition = this.Viewport.UnProject(pt);

            // Calculate the cursor ray
            Point3D cursorNearPlanePoint;
            Point3D cursorFarPlanePoint;
            var ok = this.Viewport.Point2DtoPoint3D(pt, out cursorNearPlanePoint, out cursorFarPlanePoint);
            if (ok)
            {
                var ray = new Ray3D(cursorFarPlanePoint, cursorNearPlanePoint);
                this.CursorRay = ray;
            }
            else
            {
                this.CursorOnConstructionPlanePosition = null;
                this.CursorRay = null;
            }

            // Calculate the intersection between the construction plane and the cursor ray
            if (this.CursorRay != null)
            {
                this.CursorOnConstructionPlanePosition = this.ConstructionPlane.LineIntersection(
                    this.CursorRay.Origin,
                    this.CursorRay.Origin + this.CursorRay.Direction);
            }
            else
            {
                this.CursorOnConstructionPlanePosition = null;
            }

            // TODO: remove this code when the CurrentPosition property is removed
#pragma warning disable 618
            if (this.CursorOnElementPosition.HasValue)
            {
                this.CurrentPosition = this.CursorOnElementPosition.Value;
            }
            else
            {
                if (this.CursorPosition.HasValue)
                {
                    this.CurrentPosition = this.CursorPosition.Value;
                }
            }
#pragma warning restore 618
        }

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="newValue">The items to add.</param>
        private void AddItems(IEnumerable newValue)
        {
            if (newValue != null)
            {
                foreach (var element in newValue)
                {
                    var visual = element as Visual3D;
                    if (visual != null)
                    {
                        this.Children.Add(visual);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Changed event of the current camera.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraPropertyChanged(object sender, EventArgs e)
        {
            // Raise notification
            this.RaiseCameraChangedEvent();

            // Update the CoordinateView camera and the headlight direction
            this.OnCameraChanged();
        }

        /// <summary>
        /// Handles the Rendering event of the CompositionTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void CompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            if (limitFPS && prevTime == e.RenderingTime)
            {
                return;
            }
            prevTime = e.RenderingTime;
            this.frameCounter++;
            if (this.ShowFrameRate && this.fpsWatch.ElapsedMilliseconds > 500)
            {
                this.FrameRate = (int)(this.frameCounter / (0.001 * this.fpsWatch.ElapsedMilliseconds));
                this.FrameRateText = this.FrameRate + " FPS";
                this.frameCounter = 0;
                this.fpsWatch.Reset();
                this.fpsWatch.Start();
            }

            // update the info fields every 100 frames
            // (it would be better to update only when the visual model of the Viewport3D changes)
            this.infoFrameCounter++;
            if (this.ShowTriangleCountInfo && this.infoFrameCounter > 100)
            {
                var count = this.viewport.GetTotalNumberOfTriangles();
                this.TriangleCountInfo = string.Format("Triangles: {0}", count);
                this.infoFrameCounter = 0;
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Copy" /> command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CopyHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // var vm = Viewport3DHelper.GetViewMatrix(Camera);
            // double ar = ActualWidth / ActualHeight;
            // var pm = Viewport3DHelper.GetProjectionMatrix(Camera, ar);
            // double w = 2 / pm.M11;
            // pm.OffsetX = -1
            // ;
            // pm.M11 *= 2;
            // pm.M22 *= 2;
            // var mc = new MatrixCamera(vm, pm);
            // viewport.Camera = mc;
            this.Copy();
        }

        /// <summary>
        /// Handles changes to the camera rotation mode.
        /// </summary>
        private void OnCameraRotationModeChanged()
        {
            if (this.CameraRotationMode != CameraRotationMode.Trackball && this.cameraController != null)
            {
                this.cameraController.ResetCameraUpDirection();
            }
        }

        /// <summary>
        /// Handles the Loaded event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.hasBeenLoadedBefore)
            {
                if (this.DefaultCamera != null)
                {
                    this.DefaultCamera.Copy(this.perspectiveCamera);
                    this.DefaultCamera.Copy(this.orthographicCamera);
                }

                this.hasBeenLoadedBefore = true;
            }

            this.UpdateRenderingEventSubscription();
            if (this.ZoomExtentsWhenLoaded)
            {
                this.ZoomExtents();
            }
        }

        /// <summary>
        /// Handles the Unloaded event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeRenderingEvent();
        }

        /// <summary>
        /// Handles changes to the <see cref="Orthographic" /> property.
        /// </summary>
        private void OnOrthographicChanged()
        {
            var oldCamera = this.Camera;
            if (this.Orthographic)
            {
                this.Camera = this.orthographicCamera;
            }
            else
            {
                this.Camera = this.perspectiveCamera;
            }

            oldCamera.Copy(this.Camera, false);
        }

        /// <summary>
        /// Handles changes to the <see cref="ShowFrameRate" /> property.
        /// </summary>
        private void OnShowFrameRateChanged()
        {
            this.UpdateRenderingEventSubscription();
        }

        /// <summary>
        /// Handles changes to the <see cref="ShowTriangleCountInfo" /> property.
        /// </summary>
        private void OnShowTriangleCountInfoChanged()
        {
            this.UpdateRenderingEventSubscription();
        }

        /// <summary>
        /// Handles the <see cref="OrthographicToggleCommand" />.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OrthographicToggle(object sender, ExecutedRoutedEventArgs e)
        {
            this.Orthographic = !this.Orthographic;
        }

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="oldValue">
        /// The items to remove.
        /// </param>
        private void RemoveItems(IEnumerable oldValue)
        {
            if (oldValue != null)
            {
                foreach (var element in oldValue)
                {
                    var visual = element as Visual3D;
                    if (visual != null)
                    {
                        this.Children.Remove(visual);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to the rendering event.
        /// </summary>
        private void SubscribeToRenderingEvent()
        {
            if (!this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.AddListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = true;
            }
        }

        /// <summary>
        /// Unsubscribes the rendering event.
        /// </summary>
        private void UnsubscribeRenderingEvent()
        {
            if (this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.RemoveListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = false;
            }
        }

        /// <summary>
        /// Updates the camera info.
        /// </summary>
        private void UpdateCameraInfo()
        {
            this.CameraInfo = this.Camera.GetInfo();
        }

        /// <summary>
        /// Updates the field of view info.
        /// </summary>
        private void UpdateFieldOfViewInfo()
        {
            var pc = this.Camera as PerspectiveCamera;
            this.FieldOfViewText = pc != null ? string.Format("FoV ∠ {0:0}°", pc.FieldOfView) : null;
        }

        /// <summary>
        /// Updates the rendering event subscription.
        /// </summary>
        private void UpdateRenderingEventSubscription()
        {
            if (this.ShowFrameRate || this.ShowTriangleCountInfo)
            {
                this.SubscribeToRenderingEvent();
            }
            else
            {
                this.UnsubscribeRenderingEvent();
            }
        }

        /// <summary>
        /// Handles the mouse enter events on the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ViewCubeViewportMouseEnter(object sender, MouseEventArgs e)
        {
            this.viewCubeViewport.AnimateOpacity(1.0, 200);
        }

        /// <summary>
        /// Handles the mouse leave events on the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ViewCubeViewportMouseLeave(object sender, MouseEventArgs e)
        {
            this.viewCubeViewport.AnimateOpacity(this.ViewCubeOpacity, 200);
        }
    }
}

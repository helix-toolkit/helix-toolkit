using HelixToolkit.SharpDX;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Elements2D;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Elements2D;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Controls;
using HelixToolkit.Avalonia.SharpDX.Elements2D;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Provides the dependency properties for Viewport3DX.
/// </summary>
public partial class Viewport3DX
{
    // Using a DependencyProperty as the backing store for AllowUpDownRotation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty AllowUpDownRotationProperty =
        HelixProperty.Register<Viewport3DX, bool>("AllowUpDownRotation",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            float allowX = viewport.cameraController.AllowRotateXY.X;
            float allowY = (bool)e.NewValue! ? 1 : 0;
            viewport.CameraController.AllowRotateXY = new Vector2(allowX, allowY);
        });

    // Using a DependencyProperty as the backing store for AllowLeftRightRotation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty AllowLeftRightRotationProperty =
        HelixProperty.Register<Viewport3DX, bool>("AllowLeftRightRotation",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            float allowX = (bool)e.NewValue! ? 1 : 0;
            float allowY = viewport.cameraController.AllowRotateXY.Y;
            viewport.CameraController.AllowRotateXY = new Vector2(allowX, allowY);
        });

    /// <summary>
    /// Background Color property.this.RenderHost
    /// </summary>
    public static readonly DependencyProperty BackgroundColorProperty =
        HelixProperty.Register<Viewport3DX, UIColor>("BackgroundColor",
            UIColors.White, (s, e) =>
        {
            if (s is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal != null)
            {
                viewport.renderHostInternal.ClearColor = ((UIColor)e.NewValue!).ToColor4();
            }
        });

    /// <summary>
    /// The belongs to parent window property
    /// </summary>
    public static readonly DependencyProperty BelongsToParentWindowProperty =
        HelixProperty.Register<Viewport3DX, bool>("BelongsToParentWindow", true);

    /// <summary>
    /// The camera property
    /// </summary>
    public static readonly DependencyProperty CameraProperty =
        HelixProperty.Register<Viewport3DX, Camera?>("Camera",
            null, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraPropertyChanged(e);
        });

    /// <summary>
    /// The camera inertia factor property.
    /// </summary>
    public static readonly DependencyProperty CameraInertiaFactorProperty =
        HelixProperty.Register<Viewport3DX, double>("CameraInertiaFactor",
            0.93, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.InertiaFactor = (double)e.NewValue!;
        });

    /// <summary>
    /// The camera mode property
    /// </summary>
    public static readonly DependencyProperty CameraModeProperty =
        HelixProperty.Register<Viewport3DX, CameraMode>("CameraMode",
            CameraMode.Inspect, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.CameraMode = (CameraMode)e.NewValue!;
        });

    /// <summary>
    /// The camera rotation mode property
    /// </summary>
    public static readonly DependencyProperty CameraRotationModeProperty =
        HelixProperty.Register<Viewport3DX, CameraRotationMode>("CameraRotationMode",
            CameraRotationMode.Turntable, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                viewport.CameraController.CameraRotationMode = (CameraRotationMode)e.NewValue!;
            });

    /// <summary>
    /// The change fov cursor property.
    /// </summary>
    public static readonly DependencyProperty ChangeFieldOfViewCursorProperty =
        HelixProperty.Register<Viewport3DX, UICursor>("ChangeFieldOfViewCursor",
#if false
#elif WINUI || WPF
            Cursors.ScrollNS,
#elif AVALONIA
            new UICursor(StandardCursorType.SizeNorthSouth),
#else
#error Unknown framework
#endif
            (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                viewport.CameraController.ChangeFieldOfViewCursor = (UICursor)e.NewValue!;
            });

    /// <summary>
    /// The content 2d property
    /// </summary>
    public static readonly DependencyProperty Content2DProperty
        = HelixProperty.Register<Viewport3DX, Element2D?>("Content2D",
            null, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (e.OldValue is Element2D elementOld)
            {
                viewport.Overlay2D.Children.Remove(elementOld);
            }
            if (e.NewValue is Element2D elementNew)
            {
                viewport.Overlay2D.Children.Add(elementNew);
            }
        });

    /// <summary>
    /// The coordinate system color X property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemAxisXColorProperty =
        HelixProperty.Register<Viewport3DX, UIColor>("CoordinateSystemAxisXColor", Colors.Red);

    /// <summary>
    /// The coordinate system Color Y property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemAxisYColorProperty =
        HelixProperty.Register<Viewport3DX, UIColor>("CoordinateSystemAxisYColor", Colors.Green);

    /// <summary>
    /// The coordinate system Color Z property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemAxisZColorProperty =
        HelixProperty.Register<Viewport3DX, UIColor>("CoordinateSystemAxisZColor", Colors.Blue);

    /// <summary>
    /// The coordinate system horizontal position property. Relative to viewport center
    /// <para>Default: -0.8</para>
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemHorizontalPositionProperty =
        HelixProperty.Register<Viewport3DX, double>("CoordinateSystemHorizontalPosition",
            -0.8);

    /// <summary>
    /// The coordinate system label foreground property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelForegroundProperty =
        HelixProperty.Register<Viewport3DX, UIColor>("CoordinateSystemLabelForeground",
            UIColors.DarkGray);

    /// <summary>
    /// The coordinate system label X property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelXProperty =
        HelixProperty.Register<Viewport3DX, string>("CoordinateSystemLabelX", "X");

    /// <summary>
    /// The coordinate system label Y property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelYProperty =
        HelixProperty.Register<Viewport3DX, string>("CoordinateSystemLabelY", "Y");

    /// <summary>
    /// The coordinate system label Z property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelZProperty =
        HelixProperty.Register<Viewport3DX, string>("CoordinateSystemLabelZ", "Z");

    /// <summary>
    /// The coordinate system vertical position property. Relative to viewport center.
    /// <para>Default: -0.8</para>
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemVerticalPositionProperty =
        HelixProperty.Register<Viewport3DX, double>("CoordinateSystemVerticalPosition", -0.8);

    /// <summary>
    /// The coordinate system size property.
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemSizeProperty =
        HelixProperty.Register<Viewport3DX, double>("CoordinateSystemSize", 1.0);

    /// <summary>
    /// Identifies the <see cref="CursorPosition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// The return value equals ConstructionPlanePosition or CursorModelSnapPosition if CursorSnapToModels is not null.
    /// </remarks>
    public static readonly DependencyProperty CursorPositionProperty =
        HelixProperty.Register<Viewport3DX, Point3D?>("CursorPosition",
            null, true);

    /// <summary>
    /// Identifies the <see cref="CursorOnElementPosition"/> dependency property.
    /// </summary>
    /// <remarks>
    /// This property returns the position of the nearest model.
    /// </remarks>
    public static readonly DependencyProperty CursorOnElementPositionProperty =
        HelixProperty.Register<Viewport3DX, Point3D?>("CursorOnElementPosition",
            null, true);

    /// <summary>
    /// The default camera property.
    /// </summary>
    public static readonly DependencyProperty DefaultCameraProperty =
        HelixProperty.Register<Viewport3DX, ProjectionCamera?>("DefaultCamera",
            null, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.DefaultCamera = e.NewValue as ProjectionCamera;
        });

    /// <summary>
    /// The dpi scale
    /// </summary>
    public static readonly DependencyProperty DpiScaleProperty =
        HelixProperty.Register<Viewport3DX, double>("DpiScale",
            1.0, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                if (viewport.hostPresenter != null && viewport.hostPresenter.Content is IRenderCanvas canvas)
                {
                    canvas.DpiScale = (float)(double)e.NewValue!;
                }
            });

    /// <summary>
    /// The EffectsManager property.
    /// </summary>
    public static readonly DependencyProperty EffectsManagerProperty =
        HelixProperty.Register<Viewport3DX, IEffectsManager?>("EffectsManager",
            null, (s, e) =>
        {
            if (s is not Viewport3DX viewport)
            {
                return;
            }

            viewport.EffectsManagerPropertyChanged();
        });

    /// <summary>
    /// The enable automatic octree update property
    /// </summary>
    public static readonly DependencyProperty EnableAutoOctreeUpdateProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableAutoOctreeUpdate",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.AutoUpdateOctree = (bool)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// Identifies the <see cref="EnableCursorPosition"/> dependency property.
    /// It enables (true) or disables (false) the calculation of the cursor position in the 3D Viewport
    /// </summary>
    public static readonly DependencyProperty EnableCursorPositionProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableCursorPosition", false);

    /// <summary>
    /// The enable d2d rendering property
    /// </summary>
    public static readonly DependencyProperty EnableD2DRenderingProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableD2DRendering",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal != null)
            {
                viewport.renderHostInternal.RenderConfiguration.RenderD2D = (bool)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// <para>Enable deferred rendering. Use multithreading to call rendering procedure using different Deferred Context.</para> 
    /// <para>Deferred Rendering: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476892.aspx</para>
    /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
    /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
    /// </summary>
    public static readonly DependencyProperty EnableDeferredRenderingProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableDeferredRendering", false);

    /// <summary>
    /// The enable design mode rendering property
    /// </summary>
    public static readonly DependencyProperty EnableDesignModeRenderingProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableDesignModeRendering", false);

    /// <summary>
    /// The enable dpi scale property
    /// </summary>
    public static readonly DependencyProperty EnableDpiScaleProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableDpiScale",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.hostPresenter is not null && viewport.hostPresenter.Content is IRenderCanvas canvas)
            {
                canvas.EnableDpiScale = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// Enable mouse button hit test
    /// </summary>
    public static readonly DependencyProperty EnableMouseButtonHitTestProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableMouseButtonHitTest",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.enableMouseButtonHitTest = (bool)e.NewValue!;
        });

    public static readonly DependencyProperty EnableOITDepthPeelingDynamicIterationProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableOITDepthPeelingDynamicIteration",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.EnableOITDepthPeelingDynamicIteration = (bool)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// Enable render frustum to avoid rendering model if it is out of view frustum
    /// </summary>
    public static readonly DependencyProperty EnableRenderFrustumProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableRenderFrustumProperty",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.EnableRenderFrustum = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// The enable render order property. <see cref="EnableRenderOrder"/>
    /// </summary>
    public static readonly DependencyProperty EnableRenderOrderProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableRenderOrder",
            false,
            (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                if (viewport.renderHostInternal is not null)
                {
                    viewport.renderHostInternal.RenderConfiguration.EnableRenderOrder = (bool)e.NewValue!;
                    viewport.renderHostInternal.InvalidatePerFrameRenderables();
                }
            });

    /// <summary>
    /// Used to create multiple viewport with shared models.
    /// </summary>
    public static readonly DependencyProperty EnableSharedModelModeProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableSharedModelMode",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.EnableSharingModelMode = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// The enable ssao property
    /// </summary>
    public static readonly DependencyProperty EnableSSAOProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableSSAO",
            false,
            (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                if (viewport.renderHostInternal is not null)
                {
                    viewport.renderHostInternal.RenderConfiguration.EnableSSAO = (bool)e.NewValue!;
                    viewport.renderHostInternal.InvalidateRender();
                }
            });

#if false
#elif WINUI
#elif WPF
    /// <summary>
    /// The enable swap chain rendering property
    /// </summary>
    public static readonly DependencyProperty EnableSwapChainRenderingProperty =
        HelixProperty.Register<Viewport3DX, bool>("EnableSwapChainRendering", false);
#elif AVALONIA
#else
#error Unknown framework
#endif

    /// <summary>
    /// The field of view text property.
    /// </summary>
    public static readonly DependencyProperty FieldOfViewTextProperty =
        HelixProperty.Register<Viewport3DX, string?>("FieldOfViewText", null);

    /// <summary>
    /// Rotate around this fixed rotation point only.<see cref="FixedRotationPointEnabledProperty"/> 
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointProperty =
        HelixProperty.Register<Viewport3DX, Point3D>("FixedRotationPoint",
            new Point3D(), (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.FixedRotationPoint = ((Point3D)e.NewValue!).ToVector3();
        });

    /// <summary>
    /// Enable fixed rotation mode and use FixedRotationPoint for rotation. Only works under CameraMode = Inspect
    /// </summary>
    public static readonly DependencyProperty FixedRotationPointEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("FixedRotationPointEnabled",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.FixedRotationPointEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The frame rate property.
    /// </summary>
    public static readonly DependencyProperty FrameRateProperty =
        HelixProperty.Register<Viewport3DX, double>("FrameRate", 0.0);

    /// <summary>
    /// The frame rate text property.
    /// </summary>
    public static readonly DependencyProperty FrameRateTextProperty =
        HelixProperty.Register<Viewport3DX, string?>("FrameRateText", null);

    /// <summary>
    /// The fxaa level property
    /// </summary>
    public static readonly DependencyProperty FXAALevelProperty =
        HelixProperty.Register<Viewport3DX, FXAALevel>("FXAALevel",
            FXAALevel.None, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.FXAALevel = (FXAALevel)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    public static readonly DependencyProperty IncreaseSwapchainFPSProperty =
        HelixProperty.Register<Viewport3DX, bool>("IncreaseSwapchainFPS",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

#if false
#elif WINUI
#elif WPF
            if (viewport.hostPresenter != null && viewport.hostPresenter.Content is DPFSurfaceSwapChain surface)
            {
                surface.IncreaseFPS = (bool)e.NewValue!;
            }
#elif AVALONIA
#else
#error Unknown framework
#endif
        });

    /// <summary>
    /// The infinite spin property.
    /// </summary>
    public static readonly DependencyProperty InfiniteSpinProperty =
        HelixProperty.Register<Viewport3DX, bool>("InfiniteSpin",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.InfiniteSpin = (bool)e.NewValue!;
        });

    /// <summary>
    /// The info background property.
    /// </summary>
    public static readonly DependencyProperty InfoBackgroundProperty =
        HelixProperty.Register<Viewport3DX, Brush>("InfoBackground", new SolidColorBrush(UIColor.FromArgb(0x80, 0x8f, 0x8f, 0x8f)));

    /// <summary>
    /// The info foreground property.
    /// </summary>
    public static readonly DependencyProperty InfoForegroundProperty =
        HelixProperty.Register<Viewport3DX, Brush>("InfoForeground", new SolidColorBrush(UIColors.Blue));

#if false
#elif WINUI
    /// <summary>
    /// The mouse input controller property
    /// </summary>
    public static readonly DependencyProperty InputControllerProperty =
    HelixProperty.Register<Viewport3DX, InputController?>("InputController",
        null, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.InputController = e.NewValue == null ? new InputController() : e.NewValue as InputController;
        });
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif

    /// <summary>
    /// The is change field of view enabled property
    /// </summary>
    public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsChangeFieldOfViewEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.IsChangeFieldOfViewEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The is coordinate system mover enabled property
    /// </summary>
    public static readonly DependencyProperty IsCoordinateSystemMoverEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsCoordinateSystemMoverEnabled", true);

    ///// <summary>
    ///// The is deferred shading enabled propery
    ///// </summary>
    //public static readonly DependencyProperty IsDeferredShadingEnabledProperty =
    //  HelixProperty.Register<Viewport3DX, bool>("IsDeferredShadingEnabled",
    //      false, (s, e) => ((Viewport3DX)s).ReAttach());

    /// <summary>
    /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsInertiaEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsInertiaEnabled",
            true, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                viewport.CameraController.IsInertiaEnabled = (bool)e.NewValue!;
            });

    /// <summary>
    ///   The is move enabled property.
    /// </summary>
    public static readonly DependencyProperty IsMoveEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsMoveEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.IsMoveEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The is pan enabled property
    /// </summary>
    public static readonly DependencyProperty IsPanEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsPanEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.IsPanEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The IsTouchZoomEnabled property.
    /// </summary>
    public static readonly DependencyProperty IsPinchZoomEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsPinchZoomEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.EnablePinchZoom = (bool)e.NewValue!;
        });

    /// <summary>
    /// The is rotation enabled property
    /// </summary>
    public static readonly DependencyProperty IsRotationEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsRotationEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.IsRotationEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The is deferred shading enabled propery
    /// </summary>
    public static readonly DependencyProperty IsShadowMappingEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsShadowMappingEnabled",
            false,
            (s, e) =>
            {
                var host = (s as Viewport3DX)?.renderHostInternal;

                if (host is not null)
                {
                    host.IsShadowMapEnabled = (bool)e.NewValue!;
                }
            });

    /// <summary>
    /// The enable touch rotate property
    /// </summary>
    public static readonly DependencyProperty IsThreeFingerPanningEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsThreeFingerPanningEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.EnableThreeFingerPan = (bool)e.NewValue!;
        });

    /// <summary>
    /// The enable touch rotate property
    /// </summary>
    public static readonly DependencyProperty IsTouchRotateEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsTouchRotateEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.EnableTouchRotate = (bool)e.NewValue!;
        });

    /// <summary>
    /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsViewCubeEdgeClicksEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsViewCubeEdgeClicksEnabled", false);

    /// <summary>
    /// Identifies the <see cref=" IsViewCubeEdgeClicksEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsViewCubeMoverEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsViewCubeMoverEnabled", true);

    /// <summary>
    /// The is zoom enabled property
    /// </summary>
    public static readonly DependencyProperty IsZoomEnabledProperty =
        HelixProperty.Register<Viewport3DX, bool>("IsZoomEnabled",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.IsZoomEnabled = (bool)e.NewValue!;
        });

    /// <summary>
    /// The left right pan sensitivity property.
    /// </summary>
    public static readonly DependencyProperty LeftRightPanSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("LeftRightPanSensitivity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.LeftRightPanSensitivity = (double)e.NewValue!;
        });

    /// <summary>
    /// The left right rotation sensitivity property.
    /// </summary>
    public static readonly DependencyProperty LeftRightRotationSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("LeftRightRotationSensitivity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.LeftRightRotationSensitivity = (double)e.NewValue!;
        });

    /// <summary>
    /// Manually move camera to look at a point in 3D space
    /// </summary>
    public static readonly DependencyProperty ManualLookAtPointProperty =
        HelixProperty.Register<Viewport3DX, Point3D>("ManualLookAtPoint",
            new Point3D(), (d, e) =>
        {
            (d as Viewport3DX)?.LookAt((Point3D)e.NewValue!);
        });

    /// <summary>
    /// The maximum field of view property
    /// </summary>
    public static readonly DependencyProperty MaximumFieldOfViewProperty =
        HelixProperty.Register<Viewport3DX, double>("MaximumFieldOfView",
            120.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.MaximumFieldOfView = (double)e.NewValue!;
        });

    /// <summary>
    /// The message text property.
    /// </summary>
    public static readonly DependencyProperty MessageTextProperty =
        HelixProperty.Register<Viewport3DX, string?>("MessageText", null);

    /// <summary>
    /// The minimum field of view property
    /// </summary>
    public static readonly DependencyProperty MinimumFieldOfViewProperty =
        HelixProperty.Register<Viewport3DX, double>("MinimumFieldOfView",
            10.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.MinimumFieldOfView = (double)e.NewValue!;
        });

    /// <summary>
    /// The minimum update count property
    /// </summary>
    public static readonly DependencyProperty MinimumUpdateCountProperty =
        HelixProperty.Register<Viewport3DX, int>("MinimumUpdateCount",
            6, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.MinimumUpdateCount = (uint)Math.Max(0, (int)e.NewValue!);
            }
        });

    /// <summary>
    /// The model up direction property
    /// </summary>
    public static readonly DependencyProperty ModelUpDirectionProperty =
        HelixProperty.Register<Viewport3DX, Vector3D>("ModelUpDirection",
            new Vector3D(0, 1, 0), (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ModelUpDirection = ((Vector3D)e.NewValue!).ToVector3();
        });

    /// <summary>
    /// Identifies the <see cref="MoveSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MoveSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("MoveSensitivity",
            1.0, (d, e) =>
        {
            if (d is Viewport3DX viewport)
            {
                viewport.CameraController.MoveSensitivity = (double)e.NewValue!;
            }
        });

    /// <summary>
    /// Set MSAA Level
    /// </summary>
    public static readonly DependencyProperty MSAAProperty =
        HelixProperty.Register<Viewport3DX, MSAALevel>("MSAA",
            MSAALevel.Disable, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.MSAA = (MSAALevel)e.NewValue!;
            }
        });

    public static readonly DependencyProperty OITDepthPeelingIterationProperty =
        HelixProperty.Register<Viewport3DX, int>("OITDepthPeelingIteration",
            4, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.OITDepthPeelingIteration = (int)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// Gets or sets a value indicating for Transparent objects render mode.
    /// <see cref="MaterialGeometryModel3D.IsTransparent"/>, <see cref="BillboardTextModel3D.IsTransparent"/>
    /// </summary>
    public static readonly DependencyProperty OITRenderModeProperty =
        HelixProperty.Register<Viewport3DX, OITRenderType>("OITRenderMode",
            OITRenderType.DepthPeeling, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.OITRenderType = (OITRenderType)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// The oit weight depth slope property
    /// </summary>
    public static readonly DependencyProperty OITWeightDepthSlopeProperty =
        HelixProperty.Register<Viewport3DX, double>("OITWeightDepthSlope",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.OITWeightDepthSlope = (float)(double)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// The oit weight mode property
    /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
    /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
    /// </summary>
    public static readonly DependencyProperty OITWeightModeProperty =
        HelixProperty.Register<Viewport3DX, OITWeightMode>("OITWeightMode",
            OITWeightMode.Linear1, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.OITWeightMode = (OITWeightMode)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// The Order independent transparency rendering color weight power property
    /// </summary>
    public static readonly DependencyProperty OITWeightPowerProperty =
        HelixProperty.Register<Viewport3DX, double>("OITWeightPower",
            3.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.OITWeightPower = (float)(double)e.NewValue!;
                viewport.InvalidateRender();
            }
        });

    /// <summary>
    /// The orthographic property.
    /// </summary>
    public static readonly DependencyProperty OrthographicProperty =
        HelixProperty.Register<Viewport3DX, bool>("Orthographic",
            false, (s, e) =>
        {
            if (s is Viewport3DX viewport)
            {
                viewport.OrthographicChanged();
            }
        });

    /// <summary>
    /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PageUpDownZoomSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("PageUpDownZoomSensitivity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.PageUpDownZoomSensitivity = (double)e.NewValue!;
        });

    /// <summary>
    /// The pan cursor property
    /// </summary>
    public static readonly DependencyProperty PanCursorProperty =
        HelixProperty.Register<Viewport3DX, UICursor>("PanCursor",
#if false
#elif WINUI || WPF
            Cursors.Hand,
#elif AVALONIA
            new UICursor(StandardCursorType.Hand),
#else
#error Unknown framework
#endif
            (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.PanCursor = (UICursor)e.NewValue!;
        });

    /// <summary>
    /// The pinch zoom at center property
    /// </summary>
    public static readonly DependencyProperty PinchZoomAtCenterProperty =
        HelixProperty.Register<Viewport3DX, bool>("PinchZoomAtCenter",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.PinchZoomAtCenter = (bool)e.NewValue!;
        });

    /// <summary>
    /// The render detail output property
    /// </summary>
    public static readonly DependencyProperty RenderDetailOutputProperty =
        HelixProperty.Register<Viewport3DX, string>("RenderDetailOutput", string.Empty);

    /// <summary>
    /// The render exception property.
    /// </summary>
    public static readonly DependencyProperty RenderExceptionProperty =
        HelixProperty.Register<Viewport3DX, Exception?>("RenderException", null);

    /// <summary>
    /// The rotate around mouse down point property
    /// </summary>
    public static readonly DependencyProperty RotateAroundMouseDownPointProperty =
        HelixProperty.Register<Viewport3DX, bool>("RotateAroundMouseDownPoint",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.RotateAroundMouseDownPoint = (bool)e.NewValue!;
        });

    /// <summary>
    /// The rotate cursor property
    /// </summary>
    public static readonly DependencyProperty RotateCursorProperty =
        HelixProperty.Register<Viewport3DX, UICursor>("RotateCursor",
#if false
#elif WINUI || WPF
            Cursors.SizeAll,
#elif AVALONIA
            new UICursor(StandardCursorType.SizeAll),
#else
#error Unknown framework
#endif
            (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.RotateCursor = (UICursor)e.NewValue!;
        });

    /// <summary>
    /// The rotation sensitivity property
    /// </summary>
    public static readonly DependencyProperty RotationSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("RotationSensitivity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.RotationSensitivity = (double)e.NewValue!;
        });

    /// <summary>
    /// Binding to the element inherit with <see cref="IModelContainer"/> 
    /// </summary>
    public static readonly DependencyProperty SharedModelContainerProperty =
        HelixProperty.Register<Viewport3DX, IModelContainer?>("SharedModelContainer",
            null, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (e.OldValue is IModelContainer o)
            {
                o.DettachViewport3DX(viewport);
            }

            if (e.NewValue is IModelContainer n)
            {
                n.AttachViewport3DX(viewport);
            }

            viewport.SharedModelContainerInternal = (IModelContainer?)e.NewValue;

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.SharedModelContainer = (IModelContainer?)e.NewValue;
            }
        });

    /// <summary>
    /// The show camera info property.
    /// </summary>
    public static readonly DependencyProperty ShowCameraInfoProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowCameraInfo",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                if ((bool)e.NewValue!)
                {
                    viewport.renderHostInternal.ShowRenderDetail |= RenderDetail.Camera;
                }
                else
                {
                    viewport.renderHostInternal.ShowRenderDetail &= ~RenderDetail.Camera;
                }
            }
        });

    /// <summary>
    /// The show camera target property.
    /// </summary>
    public static readonly DependencyProperty ShowCameraTargetProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowCameraTarget",
            true, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ShowCameraTarget = (bool)e.NewValue!;
        });

    /// <summary>
    /// The show coordinate system property.
    /// </summary>
    public static readonly DependencyProperty ShowCoordinateSystemProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowCoordinateSystem", false);

    /// <summary>
    /// The show frame rate property.
    /// </summary>
    public static readonly DependencyProperty ShowFrameDetailsProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowFrameDetails",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                if ((bool)e.NewValue!)
                {
                    viewport.renderHostInternal.ShowRenderDetail |= RenderDetail.Statistics;
                }
                else
                {
                    viewport.renderHostInternal.ShowRenderDetail &= ~RenderDetail.Statistics;
                }
            }
        });

    /// <summary>
    /// The show frame rate property.
    /// </summary>
    public static readonly DependencyProperty ShowFrameRateProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowFrameRate",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                if ((bool)e.NewValue!)
                {
                    viewport.renderHostInternal.ShowRenderDetail |= RenderDetail.FPS;
                }
                else
                {
                    viewport.renderHostInternal.ShowRenderDetail &= ~RenderDetail.FPS;
                }
            }
        });

    /// <summary>
    /// The show triangle count info property.
    /// </summary>
    public static readonly DependencyProperty ShowTriangleCountInfoProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowTriangleCountInfo",
            false, (d, e) =>
         {
             if (d is not Viewport3DX viewport)
             {
                 return;
             }

             if (viewport.renderHostInternal is not null)
             {
                 if ((bool)e.NewValue!)
                 {
                     viewport.renderHostInternal.ShowRenderDetail |= RenderDetail.TriangleInfo;
                 }
                 else
                 {
                     viewport.renderHostInternal.ShowRenderDetail &= ~RenderDetail.TriangleInfo;
                 }
             }
         });

    /// <summary>
    /// The show view cube property.
    /// </summary>
    public static readonly DependencyProperty ShowViewCubeProperty =
        HelixProperty.Register<Viewport3DX, bool>("ShowViewCube", true);

    /// <summary>
    /// The spin release time property
    /// </summary>
    public static readonly DependencyProperty SpinReleaseTimeProperty =
        HelixProperty.Register<Viewport3DX, int>("SpinReleaseTime",
            200, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.SpinReleaseTime = (int)e.NewValue!;
        });

    /// <summary>
    /// The ssao intensity property
    /// </summary>
    public static readonly DependencyProperty SSAOIntensityProperty =
        HelixProperty.Register<Viewport3DX, double>("SSAOIntensity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.SSAOIntensity = (float)(double)e.NewValue!;
                viewport.renderHostInternal.InvalidateRender();
            }
        });

    /// <summary>
    /// The ssao quality property
    /// </summary>
    public static readonly DependencyProperty SSAOQualityProperty =
        HelixProperty.Register<Viewport3DX, SSAOQuality>("SSAOQuality",
            SSAOQuality.Low, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            if (viewport.renderHostInternal is not null)
            {
                viewport.renderHostInternal.RenderConfiguration.SSAOQuality = (SSAOQuality)e.NewValue!;
                viewport.renderHostInternal.InvalidateRender();
            }
        });

    /// <summary>
    /// The ssao sampling radius property
    /// </summary>
    public static readonly DependencyProperty SSAOSamplingRadiusProperty =
        HelixProperty.Register<Viewport3DX, double>("SSAOSamplingRadius",
            0.5, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                if (viewport.renderHostInternal is not null)
                {
                    viewport.renderHostInternal.RenderConfiguration.SSAORadius = (float)(double)e.NewValue!;
                    viewport.renderHostInternal.InvalidateRender();
                }
            });

    /// <summary>
    /// The sub title property.
    /// </summary>
    public static readonly DependencyProperty SubTitleProperty =
        HelixProperty.Register<Viewport3DX, string?>("SubTitle", null);

    /// <summary>
    /// The sub title size property.
    /// </summary>
    public static readonly DependencyProperty SubTitleSizeProperty =
        HelixProperty.Register<Viewport3DX, double>("SubTitleSize", 12.0);

    /// <summary>
    /// The text brush property.
    /// </summary>
    public static readonly DependencyProperty TextBrushProperty =
        HelixProperty.Register<Viewport3DX, Brush>("TextBrush", new SolidColorBrush(UIColors.Black));

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        HelixProperty.Register<Viewport3DX, string?>("Title", null);

    /// <summary>
    /// The title background property.
    /// </summary>
    public static readonly DependencyProperty TitleBackgroundProperty =
        HelixProperty.Register<Viewport3DX, Brush?>("TitleBackground", null);

    /// <summary>
    /// The title font family property.
    /// </summary>
    public static readonly DependencyProperty TitleFontFamilyProperty =
        HelixProperty.Register<Viewport3DX, string?>("TitleFontFamily", null);

    /// <summary>
    /// The title size property.
    /// </summary>
    public static readonly DependencyProperty TitleSizeProperty =
        HelixProperty.Register<Viewport3DX, double>("TitleSize", 12.0);

    /// <summary>
    /// The up down Pan sensitivity property.
    /// </summary>
    public static readonly DependencyProperty UpDownPanSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("UpDownPanSensitivity",
            1.0, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                viewport.CameraController.UpDownPanSensitivity = (double)e.NewValue!;
            });

    /// <summary>
    /// The up down rotation sensitivity property.
    /// </summary>
    public static readonly DependencyProperty UpDownRotationSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("UpDownRotationSensitivity",
            1.0, (d, e) =>
            {
                if (d is not Viewport3DX viewport)
                {
                    return;
                }

                viewport.CameraController.UpDownRotationSensitivity = (double)e.NewValue!;
            });

    /// <summary>
    /// The use default gestures property
    /// </summary>
    public static readonly DependencyProperty UseDefaultGesturesProperty =
        HelixProperty.Register<Viewport3DX, bool>("UseDefaultGestures",
            true, (s, e) => ((Viewport3DX)s).UseDefaultGesturesChanged());

    /// <summary>
    /// The view cube horizontal position property. Relative to viewport center.
    /// <para>Default: 0.8</para>
    /// </summary>
    public static readonly DependencyProperty ViewCubeHorizontalPositionProperty =
        HelixProperty.Register<Viewport3DX, double>("ViewCubeHorizontalPosition", 0.8);

    /// <summary>
    /// The view cube size property.
    /// </summary>
    public static readonly DependencyProperty ViewCubeSizeProperty =
        HelixProperty.Register<Viewport3DX, double>("ViewCubeSize", 1.0);

    /// <summary>
    /// The view cube texture. It must be a 6x1 (ex: 600x100) ratio image. You can also use BitmapExtension.CreateViewBoxBitmapSource to create
    /// </summary>
    public static readonly DependencyProperty ViewCubeTextureProperty =
        HelixProperty.Register<Viewport3DX, TextureModel?>("ViewCubeTexture", null);

    /// <summary>
    /// The view cube vertical position property. Relative to viewport center.
    /// <para>Default: -0.8</para>
    /// </summary>
    public static readonly DependencyProperty ViewCubeVerticalPositionProperty =
        HelixProperty.Register<Viewport3DX, double>("ViewCubeVerticalPosition", -0.8);

    /// <summary>
    /// The zoom around mouse down point property
    /// </summary>
    public static readonly DependencyProperty ZoomAroundMouseDownPointProperty =
        HelixProperty.Register<Viewport3DX, bool>("ZoomAroundMouseDownPoint",
            false, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomAroundMouseDownPoint = (bool)e.NewValue!;
        });

    /// <summary>
    /// The zoom cursor property
    /// </summary>
    public static readonly DependencyProperty ZoomCursorProperty =
        HelixProperty.Register<Viewport3DX, UICursor>("ZoomCursor",
#if false
#elif WINUI || WPF
            Cursors.SizeNS,
#elif AVALONIA
            new UICursor(StandardCursorType.SizeNorthSouth),
#else
#error Unknown framework
#endif
            (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomCursor = (UICursor)e.NewValue!;
        });

    /// <summary>
    /// The far zoom distance limit property.
    /// </summary>
    public static readonly DependencyProperty ZoomDistanceLimitFarProperty =
        HelixProperty.Register<Viewport3DX, double>("ZoomDistanceLimitFar",
            double.PositiveInfinity, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomDistanceLimitFar = (double)e.NewValue!;
        });

    /// <summary>
    /// The near zoom distance limit property.
    /// </summary>
    public static readonly DependencyProperty ZoomDistanceLimitNearProperty =
        HelixProperty.Register<Viewport3DX, double>("ZoomDistanceLimitNear",
            0.001, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomDistanceLimitNear = (double)e.NewValue!;
        });

    /// <summary>
    /// The zoom extents when loaded property.
    /// </summary>
    public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty =
        HelixProperty.Register<Viewport3DX, bool>("ZoomExtentsWhenLoaded", false);

    /// <summary>
    /// The zoom rectangle cursor property
    /// </summary>
    public static readonly DependencyProperty ZoomRectangleCursorProperty =
        HelixProperty.Register<Viewport3DX, UICursor>("ZoomRectangleCursor",
#if false
#elif WINUI || WPF
            Cursors.SizeNWSE,
#elif AVALONIA
            new UICursor(StandardCursorType.BottomRightCorner),
#else
#error Unknown framework
#endif
            (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomRectangleCursor = (UICursor)e.NewValue!;
        });

    /// <summary>
    /// The zoom sensitivity property
    /// </summary>
    public static readonly DependencyProperty ZoomSensitivityProperty =
        HelixProperty.Register<Viewport3DX, double>("ZoomSensitivity",
            1.0, (d, e) =>
        {
            if (d is not Viewport3DX viewport)
            {
                return;
            }

            viewport.CameraController.ZoomSensitivity = (double)e.NewValue!;
        });

    /// <summary>
    /// Gets or sets a value indicating whether globally [allow left right rotation].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [allow left right rotation]; otherwise, <c>false</c>.
    /// </value>
    public bool AllowLeftRightRotation
    {
        get
        {
            return (bool)GetValue(AllowLeftRightRotationProperty)!;
        }
        set
        {
            SetValue(AllowLeftRightRotationProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether globally [allow up down rotation].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [allow up down rotation]; otherwise, <c>false</c>.
    /// </value>
    public bool AllowUpDownRotation
    {
        get
        {
            return (bool)GetValue(AllowUpDownRotationProperty)!;
        }
        set
        {
            SetValue(AllowUpDownRotationProperty, value);
        }
    }

    /// <summary>
    /// Background Color
    /// </summary>
    public UIColor BackgroundColor
    {
        get
        {
            return (UIColor)this.GetValue(BackgroundColorProperty)!;
        }
        set
        {
            this.SetValue(BackgroundColorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if the life cycle of the viewport
    /// depends on the first parent window found in the actual visual tree.
    /// </summary>
    /// <value>
    /// <c>true</c> if the viewport belongs to the first parent window; otherwise, <c>false</c>
    /// </value>
    public bool BelongsToParentWindow
    {
        get
        {
            return (bool)GetValue(BelongsToParentWindowProperty)!;
        }
        set
        {
            SetValue(BelongsToParentWindowProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the camera.
    /// </summary>
    /// <value>
    /// The camera.
    /// </value>
    public Camera? Camera
    {
        get
        {
            return (Camera?)this.GetValue(CameraProperty);
        }

        set
        {
            this.SetValue(CameraProperty, value);
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
            return (double)this.GetValue(CameraInertiaFactorProperty)!;
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
            return (CameraMode)this.GetValue(CameraModeProperty)!;
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
            return (CameraRotationMode)this.GetValue(CameraRotationModeProperty)!;
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
    public UICursor ChangeFieldOfViewCursor
    {
        get
        {
            return (UICursor)this.GetValue(ChangeFieldOfViewCursorProperty)!;
        }

        set
        {
            this.SetValue(ChangeFieldOfViewCursorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the content2d.
    /// </summary>
    /// <value>
    /// The content2 d.
    /// </value>
    public Element2D? Content2D
    {
        get
        {
            return (Element2D?)GetValue(Content2DProperty);
        }
        set
        {
            SetValue(Content2DProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the coordinate system color X.
    /// </summary>
    /// <value>
    /// The coordinate system color X.
    /// </value>
    public UIColor CoordinateSystemAxisXColor
    {
        get
        {
            return (UIColor)this.GetValue(CoordinateSystemAxisXColorProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemAxisXColorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the coordinate system color Y.
    /// </summary>
    /// <value>
    /// The coordinate system color T.
    /// </value>
    public UIColor CoordinateSystemAxisYColor
    {
        get
        {
            return (UIColor)this.GetValue(CoordinateSystemAxisYColorProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemAxisYColorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the coordinate system color Z.
    /// </summary>
    /// <value>
    /// The coordinate system color Z.
    /// </value>
    public UIColor CoordinateSystemAxisZColor
    {
        get
        {
            return (UIColor)this.GetValue(CoordinateSystemAxisZColorProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemAxisZColorProperty, value);
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
            return (double)this.GetValue(CoordinateSystemHorizontalPositionProperty)!;
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
    public UIColor CoordinateSystemLabelForeground
    {
        get
        {
            return (UIColor)this.GetValue(CoordinateSystemLabelForegroundProperty)!;
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
            return (string)this.GetValue(CoordinateSystemLabelXProperty)!;
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
            return (string)this.GetValue(CoordinateSystemLabelYProperty)!;
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
            return (string)this.GetValue(CoordinateSystemLabelZProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemLabelZProperty, value);
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
            return (double)this.GetValue(CoordinateSystemSizeProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemSizeProperty, value);
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
            return (double)this.GetValue(CoordinateSystemVerticalPositionProperty)!;
        }

        set
        {
            this.SetValue(CoordinateSystemVerticalPositionProperty, value);
        }
    }

    /// <summary>
    /// Gets the current cursor position.
    /// </summary>
    /// <value>
    /// The current cursor position.
    /// </value>
    /// <remarks>
    /// The <see cref="EnableCursorPosition" /> property must be set to true to enable updating of this property.
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
    /// Gets the current cursor position on the nearest model. If the model is not hit, the position is <c>null</c>.
    /// </summary>
    /// <value>
    /// The position of the model intersection.
    /// </value>
    /// <remarks>
    /// The <see cref="EnableCursorPosition" /> property must be set to <c>true</c> to enable updating of this property.
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
    /// Gets or sets the default camera.
    /// </summary>
    /// <value>
    /// The default camera.
    /// </value>
    public ProjectionCamera? DefaultCamera
    {
        get
        {
            return (ProjectionCamera?)this.GetValue(DefaultCameraProperty);
        }

        set
        {
            this.SetValue(DefaultCameraProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the dpi scale. For example, if dpi scale is set to 200% in windows, this value must be set to 2.
    /// </summary>
    /// <value>
    /// The dpi scale.
    /// </value>
    public double DpiScale
    {
        set
        {
            SetValue(DpiScaleProperty, value);
        }
        get
        {
            return (double)GetValue(DpiScaleProperty)!;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="IEffectsManager"/>.
    /// </summary>
    public IEffectsManager? EffectsManager
    {
        get
        {
            return (IEffectsManager?)GetValue(EffectsManagerProperty);
        }

        set
        {
            SetValue(EffectsManagerProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable automatic update octree for geometry models].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable automatic octree update]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableAutoOctreeUpdate
    {
        get
        {
            return (bool)GetValue(EnableAutoOctreeUpdateProperty)!;
        }
        set
        {
            SetValue(EnableAutoOctreeUpdateProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether calculation of the <see cref="CursorPosition" /> properties is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if calculation is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool EnableCursorPosition
    {
        get
        {
            return (bool)this.GetValue(EnableCursorPositionProperty)!;
        }

        set
        {
            this.SetValue(EnableCursorPositionProperty, value);
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
        get
        {
            return (bool)GetValue(EnableD2DRenderingProperty)!;
        }
        set
        {
            SetValue(EnableD2DRenderingProperty, value);
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
            return (bool)GetValue(EnableDeferredRenderingProperty)!;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable design mode rendering].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable design mode rendering]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableDesignModeRendering
    {
        get
        {
            return (bool)GetValue(EnableDesignModeRenderingProperty)!;
        }
        set
        {
            SetValue(EnableDesignModeRenderingProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable dpi scale].
    /// Enable this option if you want to render high definition image with using high definition monitor and using dpi scaling in windows.
    /// This option may impact rendering performance due to higher resolution.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable dpi scale]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableDpiScale
    {
        get
        {
            return (bool)GetValue(EnableDpiScaleProperty)!;
        }
        set
        {
            SetValue(EnableDpiScaleProperty, value);
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
            return (bool)GetValue(EnableMouseButtonHitTestProperty)!;
        }
    }

    public bool EnableOITDepthPeelingDynamicIteration
    {
        get
        {
            return (bool)GetValue(EnableOITDepthPeelingDynamicIterationProperty)!;
        }
        set
        {
            SetValue(EnableOITDepthPeelingDynamicIterationProperty, value);
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
            return (bool)GetValue(EnableRenderFrustumProperty)!;
        }
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
        get
        {
            return (bool)GetValue(EnableRenderOrderProperty)!;
        }
        set
        {
            SetValue(EnableRenderOrderProperty, value);
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
            return (bool)GetValue(EnableSharedModelModeProperty)!;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable ScreenSpaced Ambient Occlusion].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable ssao]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableSSAO
    {
        get
        {
            return (bool)GetValue(EnableSSAOProperty)!;
        }
        set
        {
            SetValue(EnableSSAOProperty, value);
        }
    }

#if false
#elif WINUI
#elif WPF
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
#elif AVALONIA
#else
#error Unknown framework
#endif

    /// <summary>
    /// Gets or sets the field of view text.
    /// </summary>
    /// <value>
    /// The field of view text.
    /// </value>
    public string? FieldOfViewText
    {
        get
        {
            return (string?)this.GetValue(FieldOfViewTextProperty);
        }

        set
        {
            this.SetValue(FieldOfViewTextProperty, value);
        }
    }

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
            return (Point3D)GetValue(FixedRotationPointProperty)!;
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
            return (bool)GetValue(FixedRotationPointEnabledProperty)!;
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
            return (double)this.GetValue(FrameRateProperty)!;
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
    public string? FrameRateText
    {
        get
        {
            return (string?)this.GetValue(FrameRateTextProperty);
        }

        set
        {
            this.SetValue(FrameRateTextProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the fxaa. If MSAA is set, FXAA will be disabled automatically
    /// </summary>
    /// <value>
    /// The enable fxaa.
    /// </value>
    public FXAALevel FXAALevel
    {
        get
        {
            return (FXAALevel)GetValue(FXAALevelProperty)!;
        }
        set
        {
            SetValue(FXAALevelProperty, value);
        }
    }

    /// <summary>
    /// Increase swapchain fps by speed up the wpf composition target frame rate.
    /// This may negatively impact the performance on low end graphics card.
    /// Default is enabled.
    /// </summary>
    public bool IncreaseSwapchainFPS
    {
        get
        {
            return (bool)GetValue(IncreaseSwapchainFPSProperty)!;
        }
        set
        {
            SetValue(IncreaseSwapchainFPSProperty, value);
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
            return (bool)this.GetValue(InfiniteSpinProperty)!;
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
            return (Brush)this.GetValue(InfoBackgroundProperty)!;
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
            return (Brush)this.GetValue(InfoForegroundProperty)!;
        }

        set
        {
            this.SetValue(InfoForegroundProperty, value);
        }
    }

#if false
#elif WINUI
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
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif

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
            return (bool)this.GetValue(IsChangeFieldOfViewEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsChangeFieldOfViewEnabledProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether coordinate system mover enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if coordinate system mover enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsCoordinateSystemMoverEnabled
    {
        get
        {
            return (bool)GetValue(IsCoordinateSystemMoverEnabledProperty)!;
        }
        set
        {
            SetValue(IsCoordinateSystemMoverEnabledProperty, value);
        }
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
    /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
    /// </summary>
    /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
    public bool IsInertiaEnabled
    {
        get
        {
            return (bool)this.GetValue(IsInertiaEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsInertiaEnabledProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether move is enabled.
    /// </summary>
    /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
    public bool IsMoveEnabled
    {
        get
        {
            return (bool)this.GetValue(IsMoveEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsMoveEnabledProperty, value);
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
            return (bool)this.GetValue(IsPanEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsPanEnabledProperty, value);
        }
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
            return (bool)this.GetValue(IsPinchZoomEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsPinchZoomEnabledProperty, value);
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
            return (bool)this.GetValue(IsRotationEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsRotationEnabledProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether shadow mapping is enabled
    /// </summary>
    /// <value>
    /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsShadowMappingEnabled
    {
        get
        {
            return (bool)this.GetValue(IsShadowMappingEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsShadowMappingEnabledProperty, value);
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
        get
        {
            return (bool)GetValue(IsThreeFingerPanningEnabledProperty)!;
        }
        set
        {
            SetValue(IsThreeFingerPanningEnabledProperty, value);
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
        get
        {
            return (bool)GetValue(IsTouchRotateEnabledProperty)!;
        }
        set
        {
            SetValue(IsTouchRotateEnabledProperty, value);
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
        get
        {
            return (bool)GetValue(IsViewCubeEdgeClicksEnabledProperty)!;
        }
        set
        {
            SetValue(IsViewCubeEdgeClicksEnabledProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is view cube mover enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is view cube mover enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsViewCubeMoverEnabled
    {
        get
        {
            return (bool)this.GetValue(IsViewCubeMoverEnabledProperty)!;
        }

        set
        {
            this.SetValue(IsViewCubeMoverEnabledProperty, value);
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
            return (bool)this.GetValue(IsZoomEnabledProperty)!;
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
            return (double)this.GetValue(LeftRightPanSensitivityProperty)!;
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
            return (double)this.GetValue(LeftRightRotationSensitivityProperty)!;
        }

        set
        {
            this.SetValue(LeftRightRotationSensitivityProperty, value);
        }
    }

    /// <summary>
    /// Manually move camera to look at a point in 3D space. (Same as calling Viewport3DX.LookAt() function)
    /// Since camera may have been moved by mouse, the value gets does not reflect the actual point camera currently looking at.
    /// </summary>
    public Point3D ManualLookAtPoint
    {
        get
        {
            return (Point3D)this.GetValue(ManualLookAtPointProperty)!;
        }

        set
        {
            this.SetValue(ManualLookAtPointProperty, value);
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
            return (double)this.GetValue(MaximumFieldOfViewProperty)!;
        }

        set
        {
            this.SetValue(MaximumFieldOfViewProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    /// <value>
    /// The message text.
    /// </value>
    public string? MessageText
    {
        get
        {
            return (string?)this.GetValue(MessageTextProperty);
        }

        set
        {
            this.SetValue(MessageTextProperty, value);
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
            return (double)this.GetValue(MinimumFieldOfViewProperty)!;
        }

        set
        {
            this.SetValue(MinimumFieldOfViewProperty, value);
        }
    }

    /// <summary>
    /// The update count. Used to render at least N frames for each InvalidateRenderer. 
    /// D3DImage sometimes not getting refresh if only render once.
    /// Default = 6.
    /// </summary>
    /// <value>
    /// The minimum update count.
    /// </value>
    public int MinimumUpdateCount
    {
        get
        {
            return (int)GetValue(MinimumUpdateCountProperty)!;
        }
        set
        {
            SetValue(MinimumUpdateCountProperty, value);
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
            return (Vector3D)this.GetValue(ModelUpDirectionProperty)!;
        }

        set
        {
            this.SetValue(ModelUpDirectionProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the move sensitivity.
    /// </summary>
    /// <value> The move sensitivity. </value>
    public double MoveSensitivity
    {
        get
        {
            return (double)this.GetValue(MoveSensitivityProperty)!;
        }

        set
        {
            this.SetValue(MoveSensitivityProperty, value);
        }
    }

    /// <summary>
    /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
    /// </summary>
    public MSAALevel MSAA
    {
        get
        {
            return (MSAALevel)this.GetValue(MSAAProperty)!;
        }
        set
        {
            this.SetValue(MSAAProperty, value);
        }
    }

    /// <summary>
    /// Sets or gets Order independent transparency depth peeling mode iteration.
    /// </summary>
    public int OITDepthPeelingIteration
    {
        get
        {
            return (int)GetValue(OITDepthPeelingIterationProperty)!;
        }
        set
        {
            SetValue(OITDepthPeelingIterationProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets order independent transparency render mode
    /// <see cref="MaterialGeometryModel3D.IsTransparent"/>, <see cref="BillboardTextModel3D.IsTransparent"/>
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
    /// </value>
    public OITRenderType OITRenderMode
    {
        get
        {
            return (OITRenderType)GetValue(OITRenderModeProperty)!;
        }
        set
        {
            SetValue(OITRenderModeProperty, value);
        }
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
        get
        {
            return (double)GetValue(OITWeightDepthSlopeProperty)!;
        }
        set
        {
            SetValue(OITWeightDepthSlopeProperty, value);
        }
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
        get
        {
            return (OITWeightMode)GetValue(OITWeightModeProperty)!;
        }
        set
        {
            SetValue(OITWeightModeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the Order independent transparency rendering color weight power. 
    /// Used for color weight calculation. 
    /// <para>Different near field/far field settings may need different power value for z value based weight calculation.</para>
    /// </summary>
    /// <value>
    /// The oit weight power.
    /// </value>
    public double OITWeightPower
    {
        get
        {
            return (double)GetValue(OITWeightPowerProperty)!;
        }
        set
        {
            SetValue(OITWeightPowerProperty, value);
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
            return (bool)this.GetValue(OrthographicProperty)!;
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
            return (double)this.GetValue(PageUpDownZoomSensitivityProperty)!;
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
    public UICursor PanCursor
    {
        get
        {
            return (UICursor)this.GetValue(PanCursorProperty)!;
        }

        set
        {
            this.SetValue(PanCursorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [pinch zoom at center] instead of at finger down point.
    /// Default is false.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [pinch zoom at center]; otherwise, <c>false</c>.
    /// </value>
    public bool PinchZoomAtCenter
    {
        get
        {
            return (bool)GetValue(PinchZoomAtCenterProperty)!;
        }
        set
        {
            SetValue(PinchZoomAtCenterProperty, value);
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
        get
        {
            return (string)GetValue(RenderDetailOutputProperty)!;
        }
        set
        {
            SetValue(RenderDetailOutputProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Exception"/> that occured at rendering subsystem.
    /// </summary>
    public Exception? RenderException
    {
        get
        {
            return (Exception?)this.GetValue(RenderExceptionProperty);
        }
        set
        {
            this.SetValue(RenderExceptionProperty, value);
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
            return (bool)this.GetValue(RotateAroundMouseDownPointProperty)!;
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
    public UICursor RotateCursor
    {
        get
        {
            return (UICursor)this.GetValue(RotateCursorProperty)!;
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
            return (double)this.GetValue(RotationSensitivityProperty)!;
        }

        set
        {
            this.SetValue(RotationSensitivityProperty, value);
        }
    }

    /// <summary>
    /// Binding to the element inherit with <see cref="IModelContainer"/> 
    /// </summary>
    public IModelContainer? SharedModelContainer
    {
        set
        {
            SetValue(SharedModelContainerProperty, value);
        }
        get
        {
            return (IModelContainer?)GetValue(SharedModelContainerProperty);
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
            return (bool)this.GetValue(ShowCameraInfoProperty)!;
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
            return (bool)this.GetValue(ShowCameraTargetProperty)!;
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
            return (bool)this.GetValue(ShowCoordinateSystemProperty)!;
        }

        set
        {
            this.SetValue(ShowCoordinateSystemProperty, value);
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
            return (bool)GetValue(ShowFrameDetailsProperty)!;
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
            return (bool)this.GetValue(ShowFrameRateProperty)!;
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
            return (bool)this.GetValue(ShowTriangleCountInfoProperty)!;
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
            return (bool)this.GetValue(ShowViewCubeProperty)!;
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
            return (int)this.GetValue(SpinReleaseTimeProperty)!;
        }

        set
        {
            this.SetValue(SpinReleaseTimeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the ssao intensity.
    /// </summary>
    /// <value>
    /// The ssao intensity.
    /// </value>
    public double SSAOIntensity
    {
        get
        {
            return (double)GetValue(SSAOIntensityProperty)!;
        }
        set
        {
            SetValue(SSAOIntensityProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the ssao quality.
    /// </summary>
    /// <value>
    /// The ssao quality.
    /// </value>
    public SSAOQuality SSAOQuality
    {
        get
        {
            return (SSAOQuality)GetValue(SSAOQualityProperty)!;
        }
        set
        {
            SetValue(SSAOQualityProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the ssao sampling radius.
    /// </summary>
    /// <value>
    /// The ssao sampling radius.
    /// </value>
    public double SSAOSamplingRadius
    {
        get
        {
            return (double)GetValue(SSAOSamplingRadiusProperty)!;
        }
        set
        {
            SetValue(SSAOSamplingRadiusProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the sub title.
    /// </summary>
    /// <value>
    /// The sub title.
    /// </value>
    public string? SubTitle
    {
        get
        {
            return (string?)this.GetValue(SubTitleProperty);
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
            return (double)this.GetValue(SubTitleSizeProperty)!;
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
            return (Brush)this.GetValue(TextBrushProperty)!;
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
    public string? Title
    {
        get
        {
            return (string?)this.GetValue(TitleProperty);
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
            return (Brush)this.GetValue(TitleBackgroundProperty)!;
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
    public string? TitleFontFamily
    {
        get
        {
            return (string?)this.GetValue(TitleFontFamilyProperty);
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
            return (double)this.GetValue(TitleSizeProperty)!;
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
            return (double)this.GetValue(UpDownPanSensitivityProperty)!;
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
            return (double)this.GetValue(UpDownRotationSensitivityProperty)!;
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
            return (bool)this.GetValue(UseDefaultGesturesProperty)!;
        }

        set
        {
            this.SetValue(UseDefaultGesturesProperty, value);
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
            return (double)this.GetValue(ViewCubeHorizontalPositionProperty)!;
        }

        set
        {
            this.SetValue(ViewCubeHorizontalPositionProperty, value);
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
            return (double)this.GetValue(ViewCubeSizeProperty)!;
        }

        set
        {
            this.SetValue(ViewCubeSizeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the view cube texture;
    /// The view cube texture. It must be a 6x1 (ex: 600x100) ratio image. You can also use BitmapExtension.CreateViewBoxBitmapSource to create
    /// </summary>
    /// <value>
    /// The view cube texture.
    /// </value>
    public TextureModel? ViewCubeTexture
    {
        get
        {
            return (TextureModel?)this.GetValue(ViewCubeTextureProperty);
        }

        set
        {
            this.SetValue(ViewCubeTextureProperty, value);
        }
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
            return (double)this.GetValue(ViewCubeVerticalPositionProperty)!;
        }

        set
        {
            this.SetValue(ViewCubeVerticalPositionProperty, value);
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
            return (bool)this.GetValue(ZoomAroundMouseDownPointProperty)!;
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
    public UICursor ZoomCursor
    {
        get
        {
            return (UICursor)this.GetValue(ZoomCursorProperty)!;
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
            return (double)this.GetValue(ZoomDistanceLimitFarProperty)!;
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
            return (double)this.GetValue(ZoomDistanceLimitNearProperty)!;
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
            return (bool)this.GetValue(ZoomExtentsWhenLoadedProperty)!;
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
    public UICursor ZoomRectangleCursor
    {
        get
        {
            return (UICursor)this.GetValue(ZoomRectangleCursorProperty)!;
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
            return (double)this.GetValue(ZoomSensitivityProperty)!;
        }

        set
        {
            this.SetValue(ZoomSensitivityProperty, value);
        }
    }
}

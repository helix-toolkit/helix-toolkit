using System;
using System.Collections.Generic;
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Model.Scene;
    using Model.Scene2D;
    using Cameras;

    public partial class ViewportCore
    {
        #region Properties
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is shadow mapping enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is shadow mapping enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowMappingEnabled { set; get; }

        private IEffectsManager effectsManager;
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            set
            {
                if (effectsManager != value)
                {
                    effectsManager = value;
                    if (RenderHost != null)
                    {
                        RenderHost.EffectsManager = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the camera core.
        /// </summary>
        /// <value>
        /// The camera core.
        /// </value>
        public CameraCore CameraCore
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the renderables.
        /// </summary>
        /// <value>
        /// The renderables.
        /// </value>
        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                foreach (var node in Items.ItemsInternal)
                {
                    yield return node;
                }
                yield return ViewCube;
                yield return CoordinateSystem;
            }
        }
        /// <summary>
        /// Gets the d2 d renderables.
        /// </summary>
        /// <value>
        /// The d2 d renderables.
        /// </value>
        public IEnumerable<SceneNode2D> D2DRenderables { get { yield return Items2D; } }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public GroupNode Items { get; } = new GroupNode();
        /// <summary>
        /// Gets the items2 d.
        /// </summary>
        /// <value>
        /// The items2 d.
        /// </value>
        public SceneNode2D Items2D { get; } = new OverlayNode2D() { EnableBitmapCache = false };
        /// <summary>
        /// Gets or sets a value indicating whether [show FPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show FPS]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFPS
        {
            set
            {
                if (value)
                {
                    RenderHost.ShowRenderDetail |= RenderDetail.FPS;
                }
                else
                {
                    RenderHost.ShowRenderDetail &= ~RenderDetail.FPS;
                }
            }
            get
            {
                return (RenderHost.ShowRenderDetail & ~RenderDetail.FPS) != 0;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show render detail].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show render detail]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRenderDetail
        {
            set
            {
                if (value)
                {
                    RenderHost.ShowRenderDetail |= RenderDetail.Statistics;
                }
                else
                {
                    RenderHost.ShowRenderDetail &= ~RenderDetail.Statistics;
                }
            }
            get
            {
                return (RenderHost.ShowRenderDetail & ~RenderDetail.Statistics) != 0;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [render d2d].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render d2d]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderD2D
        {
            set => RenderHost.RenderConfiguration.RenderD2D = value;
            get => RenderHost.RenderConfiguration.RenderD2D;
        }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public Color4 BackgroundColor
        {
            set => RenderHost.ClearColor = value;
            get => RenderHost.ClearColor;
        }
        /// <summary>
        /// Gets or sets the FXAA level.
        /// </summary>
        /// <value>
        /// The FXAA level.
        /// </value>
        public FXAALevel FXAALevel
        {
            set => RenderHost.RenderConfiguration.FXAALevel = value;
            get => RenderHost.RenderConfiguration.FXAALevel;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable render frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderFrustum
        {
            set => RenderHost.EnableRenderFrustum = value;
            get => RenderHost.EnableRenderFrustum;
        }
        /// <summary>
        /// Gets the viewport rectangle.
        /// </summary>
        /// <value>
        /// The viewport rectangle.
        /// </value>
        public Rectangle ViewportRectangle { get { return new Rectangle(0, 0, (int)(RenderHost.ActualWidth / DpiScale), (int)(RenderHost.ActualHeight / DpiScale)); } }
        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <value>
        /// The render context.
        /// </value>
        public RenderContext RenderContext { get => RenderHost.RenderContext; }
        /// <summary>
        /// Gets the render stat.
        /// </summary>
        /// <value>
        /// The render stat.
        /// </value>
        public Utilities.IRenderStatistics RenderStat { get => RenderHost.RenderStatistics; }
        /// <summary>
        /// Gets or sets a value indicating whether [enable vertical synchronize].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable vertical synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableVSync
        {
            set => RenderHost.RenderConfiguration.EnableVSync = value;
            get => RenderHost.RenderConfiguration.EnableVSync;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable ssao].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable ssao]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSSAO
        {
            set => RenderHost.RenderConfiguration.EnableSSAO = value;
            get => RenderHost.RenderConfiguration.EnableSSAO;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable render order].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render order]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderOrder
        {
            set => RenderHost.RenderConfiguration.EnableRenderOrder = value;
            get => RenderHost.RenderConfiguration.EnableRenderOrder;
        }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth { private set; get; }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight { private set; get; }

        private double dpiScale = 1;
        public double DpiScale
        {
            set
            {
                dpiScale = value;
                if (RenderHost != null)
                {
                    RenderHost.DpiScale = (float)value;
                }
            }
            get
            {
                return dpiScale;
            }
        } 

        private Vector3 modelUpDirection = Vector3.UnitY;
        /// <summary>
        /// Gets or sets the model up direction.
        /// </summary>
        /// <value>
        /// The model up direction.
        /// </value>
        internal Vector3 ModelUpDirection
        {
            set
            {
                if(Set(ref modelUpDirection, value))
                {
                    ViewCube.UpDirection = value;
                }
            }
            get => modelUpDirection;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show coordinate system].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show coordinate system]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCoordinateSystem
        {
            set => CoordinateSystem.Visible = value;
            get => CoordinateSystem.Visible;
        }
        /// <summary>
        /// Gets or sets the color of the coordinate system label.
        /// </summary>
        /// <value>
        /// The color of the coordinate system label.
        /// </value>
        public Color4 CoordinateSystemLabelColor
        {
            set => CoordinateSystem.LabelColor = value;
            get => CoordinateSystem.LabelColor;
        }
        /// <summary>
        /// Gets or sets the coordinate system axis x label.
        /// </summary>
        /// <value>
        /// The coordinate system axis x label.
        /// </value>
        public string CoordinateSystemAxisXLabel
        {
            set => CoordinateSystem.LabelX = value;
            get => CoordinateSystem.LabelX;
        }
        /// <summary>
        /// Gets or sets the coordinate system axis y label.
        /// </summary>
        /// <value>
        /// The coordinate system axis y label.
        /// </value>
        public string CoordinateSystemAxisYLabel
        {
            set => CoordinateSystem.LabelY = value;
            get => CoordinateSystem.LabelY;
        }
        /// <summary>
        /// Gets or sets the coordinate system axis z label.
        /// </summary>
        /// <value>
        /// The coordinate system axis z label.
        /// </value>
        public string CoordinateSystemAxisZLabel
        {
            set => CoordinateSystem.LabelZ = value;
            get => CoordinateSystem.LabelZ;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show view cube].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show view cube]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowViewCube
        {
            set => ViewCube.Visible = value;
            get => ViewCube.Visible;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when [on start rendering].
        /// </summary>
        public event EventHandler StartRendering;
        /// <summary>
        /// Occurs when [on stop rendering].
        /// </summary>
        public event EventHandler StopRendering;
        /// <summary>
        /// Occurs when [on error occurred].
        /// </summary>
        public event EventHandler<Exception> ErrorOccurred;

        public event EventHandler<SceneNodeMouseDownArgs> NodeHitOnMouseDown;
        public event EventHandler<SceneNodeMouseUpArgs> NodeHitOnMouseUp;
        public event EventHandler<SceneNodeMouseMoveArgs> NodeHitOnMouseMove;
        #endregion

        internal ViewBoxNode ViewCube { get; } = new ViewBoxNode();

        internal CoordinateSystemNode CoordinateSystem { get; } = new CoordinateSystemNode();

        private List<HitTestResult> hits = new List<HitTestResult>();

        private SceneNode currentNode;

        private FrameStatisticsNode2D frameStatisticsNode = new FrameStatisticsNode2D();
    }
}

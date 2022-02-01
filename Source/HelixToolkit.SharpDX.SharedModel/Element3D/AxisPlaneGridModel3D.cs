using SharpDX;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;

namespace HelixToolkit.WinUI
#else
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// 
    /// </summary>
    public class AxisPlaneGridModel3D : Element3D
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic spacing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic spacing]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSpacing
        {
            get
            {
                return (bool)GetValue(AutoSpacingProperty);
            }
            set
            {
                SetValue(AutoSpacingProperty, value);
            }
        }

        /// <summary>
        /// The automatic spacing property
        /// </summary>
        public static readonly DependencyProperty AutoSpacingProperty =
            DependencyProperty.Register("AutoSpacing", typeof(bool), typeof(AxisPlaneGridModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).AutoSpacing = (bool)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the automatic spacing rate. Default perspective camera =5. If using Orthographic camera, increase the rate to > 15
        /// </summary>
        /// <value>
        /// The automatic spacing rate.
        /// </value>
        public double AutoSpacingRate
        {
            get
            {
                return (double)GetValue(AutoSpacingRateProperty);
            }
            set
            {
                SetValue(AutoSpacingRateProperty, value);
            }
        }

        /// <summary>
        /// The automatic spacing rate property
        /// </summary>
        public static readonly DependencyProperty AutoSpacingRateProperty =
            DependencyProperty.Register("AutoSpacingRate", typeof(double), typeof(AxisPlaneGridModel3D), new PropertyMetadata(5.0,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).AutoSpacingRate = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the grid spacing.
        /// </summary>
        /// <value>
        /// The grid spacing.
        /// </value>
        public double GridSpacing
        {
            get
            {
                return (double)GetValue(GridSpacingProperty);
            }
            set
            {
                SetValue(GridSpacingProperty, value);
            }
        }

        /// <summary>
        /// The grid spacing property
        /// </summary>
        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register("GridSpacing", typeof(double), typeof(AxisPlaneGridModel3D), new PropertyMetadata(10.0,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).GridSpacing = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the grid thickness.
        /// </summary>
        /// <value>
        /// The grid thickness.
        /// </value>
        public double GridThickness
        {
            get
            {
                return (double)GetValue(GridThicknessProperty);
            }
            set
            {
                SetValue(GridThicknessProperty, value);
            }
        }

        /// <summary>
        /// The grid thickness property
        /// </summary>
        public static readonly DependencyProperty GridThicknessProperty =
            DependencyProperty.Register("GridThickness", typeof(double), typeof(AxisPlaneGridModel3D), new PropertyMetadata(0.05,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).GridThickness = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the fading factor.
        /// </summary>
        /// <value>
        /// The fading factor.
        /// </value>
        public double FadingFactor
        {
            get
            {
                return (double)GetValue(FadingFactorProperty);
            }
            set
            {
                SetValue(FadingFactorProperty, value);
            }
        }

        /// <summary>
        /// The fading factor property
        /// </summary>
        public static readonly DependencyProperty FadingFactorProperty =
            DependencyProperty.Register("FadingFactor", typeof(double), typeof(AxisPlaneGridModel3D), new PropertyMetadata(0.2, (d, e) =>
            {
                ((d as Element3D).SceneNode as AxisPlaneGridNode).FadingFactor = (float)(double)e.NewValue;
            }));


        /// <summary>
        /// Gets or sets the color of the plane.
        /// </summary>
        /// <value>
        /// The color of the plane.
        /// </value>
        public Media.Color PlaneColor
        {
            get
            {
                return (Media.Color)GetValue(PlaneColorProperty);
            }
            set
            {
                SetValue(PlaneColorProperty, value);
            }
        }
        /// <summary>
        /// The plane color property
        /// </summary>
        public static readonly DependencyProperty PlaneColorProperty =
            DependencyProperty.Register("PlaneColor", typeof(Media.Color), typeof(AxisPlaneGridModel3D),
#if WINUI
                new PropertyMetadata(Microsoft.UI.Colors.Gray,
#else
                new PropertyMetadata(Media.Colors.Gray,
#endif     
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).PlaneColor = ((Media.Color)e.NewValue).ToColor4();
                }));


        /// <summary>
        /// Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        /// The color of the grid.
        /// </value>
        public Media.Color GridColor
        {
            get
            {
                return (Media.Color)GetValue(GridColorProperty);
            }
            set
            {
                SetValue(GridColorProperty, value);
            }
        }
        /// <summary>
        /// The grid color property
        /// </summary>
        public static readonly DependencyProperty GridColorProperty =
            DependencyProperty.Register("GridColor", typeof(Media.Color), typeof(AxisPlaneGridModel3D),
#if WINUI
                new PropertyMetadata(Microsoft.UI.Colors.DarkGray,
#else
                new PropertyMetadata(Media.Colors.DarkGray,
#endif       
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).GridColor = ((Media.Color)e.NewValue).ToColor4();
                }));


        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            get
            {
                return (bool)GetValue(RenderShadowMapProperty);
            }
            set
            {
                SetValue(RenderShadowMapProperty, value);
            }
        }

        /// <summary>
        /// The render shadow map property
        /// </summary>
        public static readonly DependencyProperty RenderShadowMapProperty =
            DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(AxisPlaneGridModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).RenderShadowMap = (bool)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets up axis.
        /// </summary>
        /// <value>
        /// Up axis.
        /// </value>
        public Axis UpAxis
        {
            get
            {
                return (Axis)GetValue(UpAxisProperty);
            }
            set
            {
                SetValue(UpAxisProperty, value);
            }
        }

        /// <summary>
        /// Up axis property
        /// </summary>
        public static readonly DependencyProperty UpAxisProperty =
            DependencyProperty.Register("UpAxis", typeof(Axis), typeof(AxisPlaneGridModel3D), new PropertyMetadata(Axis.Y,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).UpAxis = (Axis)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }
            set
            {
                SetValue(OffsetProperty, value);
            }
        }

        /// <summary>
        /// The offset property
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(AxisPlaneGridModel3D), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).Offset = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// Gets or sets the grid pattern.
        /// </summary>
        /// <value>
        /// The grid pattern.
        /// </value>
        public GridPattern GridPattern
        {
            get
            {
                return (GridPattern)GetValue(GridPatternProperty);
            }
            set
            {
                SetValue(GridPatternProperty, value);
            }
        }

        /// <summary>
        /// The grid pattern property
        /// </summary>
        public static readonly DependencyProperty GridPatternProperty =
            DependencyProperty.Register("GridPattern", typeof(GridPattern), typeof(AxisPlaneGridModel3D), new PropertyMetadata(GridPattern.Tile,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as AxisPlaneGridNode).GridPattern = (GridPattern)e.NewValue;
                }));


        protected override SceneNode OnCreateSceneNode()
        {
            return new AxisPlaneGridNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            var n = node as AxisPlaneGridNode;
            n.AutoSpacing = AutoSpacing;
            n.GridSpacing = (float)GridSpacing;
            n.GridThickness = (float)GridThickness;
            n.FadingFactor = (float)FadingFactor;
            n.PlaneColor = PlaneColor.ToColor4();
            n.GridColor = GridColor.ToColor4();
            n.RenderShadowMap = RenderShadowMap;
            n.UpAxis = UpAxis;
            n.Offset = (float)Offset;
            n.AutoSpacingRate = (float)AutoSpacingRate;
            n.GridPattern = GridPattern;
        }
    }
}

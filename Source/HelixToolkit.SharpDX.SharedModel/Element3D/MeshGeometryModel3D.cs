/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using global::SharpDX.Direct3D11;
#if NETFX_CORE
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public class MeshGeometryModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties        
        /// <summary>
        /// The front counter clockwise property
        /// </summary>
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).FrontCCW = (bool)e.NewValue; }));
        /// <summary>
        /// The cull mode property
        /// </summary>
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D),
            new PropertyMetadata(CullMode.None, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).CullMode = (CullMode)e.NewValue; }));
        /// <summary>
        /// The invert normal property
        /// </summary>
        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).InvertNormal = (bool)e.NewValue; }));
        /// <summary>
        /// The enable tessellation property
        /// </summary>
        public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).EnableTessellation = (bool)e.NewValue; }));
        /// <summary>
        /// The maximum tessellation factor property
        /// </summary>
        public static readonly DependencyProperty MaxTessellationFactorProperty =
            DependencyProperty.Register("MaxTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).MaxTessellationFactor = (float)(double)e.NewValue; }));
        /// <summary>
        /// The minimum tessellation factor property
        /// </summary>
        public static readonly DependencyProperty MinTessellationFactorProperty =
            DependencyProperty.Register("MinTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(2.0, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).MinTessellationFactor = (float)(double)e.NewValue; }));
        /// <summary>
        /// The maximum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MaxTessellationDistanceProperty =
            DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(50.0, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).MaxTessellationDistance = (float)(double)e.NewValue; }));
        /// <summary>
        /// The minimum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MinTessellationDistanceProperty =
            DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).MinTessellationDistance = (float)(double)e.NewValue; }));

        /// <summary>
        /// The mesh topology property
        /// </summary>
        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(MeshTopologyEnum), typeof(MeshGeometryModel3D), new PropertyMetadata(
                MeshTopologyEnum.PNTriangles, (d, e) =>
                { ((d as Element3DCore).SceneNode as MeshNode).MeshType = (MeshTopologyEnum)e.NewValue; }));

        /// <summary>
        /// The render wireframe property
        /// </summary>
        public static readonly DependencyProperty RenderWireframeProperty =
            DependencyProperty.Register("RenderWireframe", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).RenderWireframe = (bool)e.NewValue; }));

        /// <summary>
        /// The wireframe color property
        /// </summary>
        public static readonly DependencyProperty WireframeColorProperty =
            DependencyProperty.Register("WireframeColor", typeof(Color), typeof(MeshGeometryModel3D), new PropertyMetadata(Colors.SkyBlue, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).WireframeColor = ((Color)e.NewValue).ToColor4(); }));

        /// <summary>
        /// Gets or sets a value indicating whether [render overlapping wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get { return (bool)GetValue(RenderWireframeProperty); }
            set { SetValue(RenderWireframeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color WireframeColor
        {
            get { return (Color)GetValue(WireframeColorProperty); }
            set { SetValue(WireframeColorProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [front counter clockwise].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front counter clockwise]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontCounterClockwise
        {
            set
            {
                SetValue(FrontCounterClockwiseProperty, value);
            }
            get
            {
                return (bool)GetValue(FrontCounterClockwiseProperty);
            }
        }

        /// <summary>
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
        public CullMode CullMode
        {
            set
            {
                SetValue(CullModeProperty, value);
            }
            get
            {
                return (CullMode)GetValue(CullModeProperty);
            }
        }

        /// <summary>
        /// Invert the surface normal during rendering
        /// </summary>
        public bool InvertNormal
        {
            set
            {
                SetValue(InvertNormalProperty, value);
            }
            get
            {
                return (bool)GetValue(InvertNormalProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable tessellation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable tessellation]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTessellation
        {
            set
            {
                SetValue(EnableTessellationProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableTessellationProperty);
            }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation factor.
        /// </summary>
        /// <value>
        /// The maximum tessellation factor.
        /// </value>
        public double MaxTessellationFactor
        {
            get { return (double)GetValue(MaxTessellationFactorProperty); }
            set { SetValue(MaxTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation factor.
        /// </summary>
        /// <value>
        /// The minimum tessellation factor.
        /// </value>
        public double MinTessellationFactor
        {
            get { return (double)GetValue(MinTessellationFactorProperty); }
            set { SetValue(MinTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation distance.
        /// </summary>
        /// <value>
        /// The maximum tessellation distance.
        /// </value>
        public double MaxTessellationDistance
        {
            get { return (double)GetValue(MaxTessellationDistanceProperty); }
            set { SetValue(MaxTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation distance.
        /// </summary>
        /// <value>
        /// The minimum tessellation distance.
        /// </value>
        public double MinTessellationDistance
        {
            get { return (double)GetValue(MinTessellationDistanceProperty); }
            set { SetValue(MinTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the mesh topology.
        /// </summary>
        /// <value>
        /// The mesh topology.
        /// </value>
        public MeshTopologyEnum MeshTopology
        {
            set { SetValue(MeshTopologyProperty, value); }
            get { return (MeshTopologyEnum)GetValue(MeshTopologyProperty); }
        }
        #endregion        
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new MeshNode();
        }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            var c = node as MeshNode;
            c.InvertNormal = this.InvertNormal;
            c.WireframeColor = this.WireframeColor.ToColor4();
            c.RenderWireframe = this.RenderWireframe;
            c.MaxTessellationFactor = (float)this.MaxTessellationFactor;
            c.MinTessellationFactor = (float)this.MinTessellationFactor;
            c.MaxTessellationDistance = (float)this.MaxTessellationDistance;
            c.MinTessellationDistance = (float)this.MinTessellationDistance;
            c.MeshType = this.MeshTopology;
            c.EnableTessellation = this.EnableTessellation;
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}

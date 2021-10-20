/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using global::SharpDX.Direct3D11;
#if NETFX_CORE
using  Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
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
        /// The render wireframe property
        /// </summary>
        public static readonly DependencyProperty RenderWireframeProperty =
            DependencyProperty.Register("RenderWireframe", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RenderWireframe = (bool)e.NewValue;
            }));

        /// <summary>
        /// The wireframe color property
        /// </summary>
        public static readonly DependencyProperty WireframeColorProperty =
            DependencyProperty.Register("WireframeColor", typeof(Color), typeof(MeshGeometryModel3D), new PropertyMetadata(Colors.SkyBlue, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).WireframeColor = ((Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [render overlapping wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get
            {
                return (bool)GetValue(RenderWireframeProperty);
            }
            set
            {
                SetValue(RenderWireframeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color WireframeColor
        {
            get
            {
                return (Color)GetValue(WireframeColorProperty);
            }
            set
            {
                SetValue(WireframeColorProperty, value);
            }
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
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Model;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Colors = System.Windows.Media.Colors;
using Color = System.Windows.Media.Color;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Core;
    using Model.Scene;
#endif
    /// <summary>
    /// Static mesh batching. Supports multiple <see cref="BatchedMaterials"/>. All geometries are merged into single buffer for rendering. Indivisual material color infomations are encoded into vertex buffer.
    /// <para>
    /// <see cref="Material"/> is used if <see cref="BatchedMaterials"/> = null. And also used for shared material texture binding.
    /// </para>
    /// </summary>
    public class BatchedMeshGeometryModel3D : Element3D, IHitable, IThrowingShadow, IApplyPostEffect
    {
        #region Dependency Properties
        public IList<BatchedMeshGeometryConfig> BatchedGeometries
        {
            get
            {
                return (IList<BatchedMeshGeometryConfig>)GetValue(BatchedGeometriesProperty);
            }
            set
            {
                SetValue(BatchedGeometriesProperty, value);
            }
        }

        public static readonly DependencyProperty BatchedGeometriesProperty =
            DependencyProperty.Register("BatchedGeometries", typeof(IList<BatchedMeshGeometryConfig>), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as BatchedMeshGeometryModel3D).SceneNode as BatchedMeshNode).Geometries = e.NewValue == null ? null : ((IList<BatchedMeshGeometryConfig>)e.NewValue).ToArray();
                }));

        public IList<Material> BatchedMaterials
        {
            get
            {
                return (IList<Material>)GetValue(BatchedMaterialsProperty);
            }
            set
            {
                SetValue(BatchedMaterialsProperty, value);
            }
        }

        public static readonly DependencyProperty BatchedMaterialsProperty =
            DependencyProperty.Register("BatchedMaterials", typeof(IList<Material>), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as BatchedMeshGeometryModel3D).SceneNode as BatchedMeshNode).Materials = e.NewValue == null ?
                    null : ((IList<Material>)e.NewValue).Where(x => x.Core is PhongMaterialCore).Select(x => x.Core as PhongMaterialCore).ToArray();
                }));

        public static readonly DependencyProperty IsThrowingShadowProperty =
        DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
        {
            if ((d as Element3D).SceneNode is IThrowingShadow t)
            {
                t.IsThrowingShadow = (bool)e.NewValue;
            }
        }));

        /// <summary>
        /// The depth bias property
        /// </summary>
        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).DepthBias = (int)e.NewValue;
            }));
        /// <summary>
        /// The slope scaled depth bias property
        /// </summary>
        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(0.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).SlopeScaledDepthBias = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// The is selected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(false));
        /// <summary>
        /// The is multisample enabled property
        /// </summary>
        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).IsMSAAEnabled = (bool)e.NewValue;
            }));
        /// <summary>
        /// The fill mode property
        /// </summary>
        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(BatchedMeshGeometryModel3D),
            new PropertyMetadata(FillMode.Solid, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).FillMode = (FillMode)e.NewValue;
            }));
        /// <summary>
        /// The is scissor enabled property
        /// </summary>
        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).IsScissorEnabled = (bool)e.NewValue;
            }));
        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as BatchedMeshNode).EnableViewFrustumCheck = (bool)e.NewValue;
                }));
        /// <summary>
        /// The is depth clip enabled property
        /// </summary>
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(BatchedMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).IsDepthClipEnabled = (bool)e.NewValue;
            }));


        // Using a DependencyProperty as the backing store for PostEffects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostEffectsProperty =
            DependencyProperty.Register("PostEffects", typeof(string), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata("", (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).PostEffects = e.NewValue as string;
            }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).Material = e.NewValue as Material;
            }));

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public static readonly DependencyProperty IsTransparentProperty =
            DependencyProperty.Register("IsTransparent", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).IsTransparent = (bool)e.NewValue;
            }));

        /// <summary>
        /// The front counter clockwise property
        /// </summary>
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(BatchedMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) => { ((d as Element3DCore).SceneNode as BatchedMeshNode).FrontCCW = (bool)e.NewValue; }));
        /// <summary>
        /// The cull mode property
        /// </summary>
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(BatchedMeshGeometryModel3D),
            new PropertyMetadata(CullMode.None, (d, e) => { ((d as Element3DCore).SceneNode as BatchedMeshNode).CullMode = (CullMode)e.NewValue; }));
        /// <summary>
        /// The invert normal property
        /// </summary>
        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(BatchedMeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as Element3DCore).SceneNode as BatchedMeshNode).InvertNormal = (bool)e.NewValue; }));

        /// <summary>
        /// The render wireframe property
        /// </summary>
        public static readonly DependencyProperty RenderWireframeProperty =
            DependencyProperty.Register("RenderWireframe", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).RenderWireframe = (bool)e.NewValue;
            }));

        /// <summary>
        /// The wireframe color property
        /// </summary>
        public static readonly DependencyProperty WireframeColorProperty =
            DependencyProperty.Register("WireframeColor", typeof(Color), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(Colors.SkyBlue, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).WireframeColor = ((Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// The always hittable property
        /// </summary>
        public static readonly DependencyProperty AlwaysHittableProperty =
            DependencyProperty.Register("AlwaysHittable", typeof(bool), typeof(BatchedMeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BatchedMeshNode).AlwaysHittable = (bool)e.NewValue;
            }));


        public string PostEffects
        {
            get
            {
                return (string)GetValue(PostEffectsProperty);
            }
            set
            {
                SetValue(PostEffectsProperty, value);
            }
        }


        /// <summary>
        /// <see cref="IThrowingShadow.IsThrowingShadow"/>
        /// </summary>
        public bool IsThrowingShadow
        {
            set
            {
                SetValue(IsThrowingShadowProperty, value);
            }
            get
            {
                return (bool)GetValue(IsThrowingShadowProperty);
            }
        }

        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            get
            {
                return (int)this.GetValue(DepthBiasProperty);
            }
            set
            {
                this.SetValue(DepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public double SlopeScaledDepthBias
        {
            get
            {
                return (double)this.GetValue(SlopeScaledDepthBiasProperty);
            }
            set
            {
                this.SetValue(SlopeScaledDepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Only works under FillMode = Wireframe. MSAA is determined by viewport MSAA settings for FillMode = Solid
        /// </summary>
        public bool IsMultisampleEnabled
        {
            set
            {
                SetValue(IsMultisampleEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsMultisampleEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            set
            {
                SetValue(FillModeProperty, value);
            }
            get
            {
                return (FillMode)GetValue(FillModeProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            set
            {
                SetValue(IsScissorEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsScissorEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableViewFrustumCheck
        {
            set
            {
                SetValue(EnableViewFrustumCheckProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableViewFrustumCheckProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get
            {
                return (Material)this.GetValue(MaterialProperty);
            }
            set
            {
                this.SetValue(MaterialProperty, value);
            }
        }

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get
            {
                return (bool)GetValue(IsTransparentProperty);
            }
            set
            {
                SetValue(IsTransparentProperty, value);
            }
        }



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
        /// <summary>
        /// Gets or sets a value indicating whether [always hittable] even it is not rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [always hittable]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysHittable
        {
            get
            {
                return (bool)GetValue(AlwaysHittableProperty);
            }
            set
            {
                SetValue(AlwaysHittableProperty, value);
            }
        }
        #endregion

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            if (node is BatchedMeshNode n)
            {
                n.DepthBias = this.DepthBias;
                n.IsDepthClipEnabled = this.IsDepthClipEnabled;
                n.SlopeScaledDepthBias = (float)this.SlopeScaledDepthBias;
                n.IsMSAAEnabled = this.IsMultisampleEnabled;
                n.FillMode = this.FillMode;
                n.IsScissorEnabled = this.IsScissorEnabled;
                n.EnableViewFrustumCheck = this.EnableViewFrustumCheck;
                n.PostEffects = this.PostEffects;
                n.Material = this.Material;
                n.InvertNormal = this.InvertNormal;
                n.WireframeColor = this.WireframeColor.ToColor4();
                n.RenderWireframe = this.RenderWireframe;
                n.CullMode = this.CullMode;
                n.AlwaysHittable = this.AlwaysHittable;
            }
            base.AssignDefaultValuesToSceneNode(node);
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new BatchedMeshNode();
        }
    }
}

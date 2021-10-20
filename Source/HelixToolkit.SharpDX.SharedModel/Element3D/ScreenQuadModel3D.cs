/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D11;
using System;
#if NETFX_CORE
using  Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using System.Runtime.Versioning;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Shaders;
using HelixToolkit.SharpDX.Core;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Shaders;
using HelixToolkit.SharpDX.Core;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
    using Shaders;
#endif

    /// <summary>
    /// 
    /// </summary>
#if WINUI
    [SupportedOSPlatform("windows")]
#endif
    public class ScreenQuadModel3D : Element3D
    {
        public TextureModel Texture
        {
            get
            {
                return (TextureModel)GetValue(TextureProperty);
            }
            set
            {
                SetValue(TextureProperty, value);
            }
        }

        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register("Texture", typeof(TextureModel), typeof(ScreenQuadModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as ScreenQuadModel3D).SceneNode as ScreenQuadNode).Texture = (TextureModel)e.NewValue;
            }));



        public SamplerStateDescription SamplerDescription
        {
            get
            {
                return (SamplerStateDescription)GetValue(SamplerDescriptionProperty);
            }
            set
            {
                SetValue(SamplerDescriptionProperty, value);
            }
        }


        public static readonly DependencyProperty SamplerDescriptionProperty =
            DependencyProperty.Register("SamplerDescription", typeof(SamplerStateDescription), typeof(ScreenQuadModel3D),
                new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1, (d, e) =>
                {
                    ((d as ScreenQuadModel3D).SceneNode as ScreenQuadNode).Sampler = (SamplerStateDescription)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the depth of the quad, range from 0 ~ 1.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public double Depth
        {
            get
            {
                return (double)GetValue(DepthProperty);
            }
            set
            {
                SetValue(DepthProperty, value);
            }
        }

        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(double), typeof(ScreenQuadModel3D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as ScreenQuadModel3D).SceneNode as ScreenQuadNode).Depth = (float)Math.Max(0, Math.Min(1, (double)e.NewValue));
                }));



        protected override SceneNode OnCreateSceneNode()
        {
            return new ScreenQuadNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            if (node is ScreenQuadNode n)
            {
                n.Texture = Texture;
                n.Sampler = SamplerDescription;
                n.Depth = (float)Math.Max(0, Math.Min(1, Depth));
            }
        }
    }
}

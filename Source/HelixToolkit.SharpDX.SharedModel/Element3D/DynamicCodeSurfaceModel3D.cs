using System.Collections.Generic;

#if !NETFX_CORE && !WINUI
using System.CodeDom.Compiler;
#if NETFX_CORE
using  Windows.UI.Xaml;
using Media = Windows.UI;

namespace HelixToolkit.UWP
#elif WINUI
using Microsoft.UI.Xaml;
using Media = Windows.UI;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !WINDOWS_UWP && !COREWPF && !WINUI
    using Model.Scene;

    public class DynamicCodeSurfaceModel3D : MeshGeometryModel3D
    {
        public string SourceCode
        {
            get
            {
                return (string)GetValue(SourceCodeProperty);
            }
            set
            {
                SetValue(SourceCodeProperty, value);
            }
        }


        public static readonly DependencyProperty SourceCodeProperty =
            DependencyProperty.Register("SourceCode", typeof(string), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as DynamicCodeSurfaceModel3D).SceneNode as DynamicCodeSurface3DNode).Source = e.NewValue as string;
            }));



        public double ParameterW
        {
            get
            {
                return (double)GetValue(ParameterWProperty);
            }
            set
            {
                SetValue(ParameterWProperty, value);
            }
        }


        public static readonly DependencyProperty ParameterWProperty =
            DependencyProperty.Register("ParameterW", typeof(double), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as DynamicCodeSurfaceModel3D).SceneNode as DynamicCodeSurface3DNode).ParameterW = (float)(double)e.NewValue;
            }));



        public int MeshSizeU
        {
            get
            {
                return (int)GetValue(MeshSizeUProperty);
            }
            set
            {
                SetValue(MeshSizeUProperty, value);
            }
        }


        public static readonly DependencyProperty MeshSizeUProperty =
            DependencyProperty.Register("MeshSizeU", typeof(int), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(120, (d, e) =>
            {
                ((d as DynamicCodeSurfaceModel3D).SceneNode as DynamicCodeSurface3DNode).MeshSizeU = (int)e.NewValue;
            }));

        public int MeshSizeV
        {
            get
            {
                return (int)GetValue(MeshSizeVProperty);
            }
            set
            {
                SetValue(MeshSizeVProperty, value);
            }
        }


        public static readonly DependencyProperty MeshSizeVProperty =
            DependencyProperty.Register("MeshSizeV", typeof(int), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(120, (d, e) =>
            {
                ((d as DynamicCodeSurfaceModel3D).SceneNode as DynamicCodeSurface3DNode).MeshSizeV = (int)e.NewValue;
            }));



        public CompilerErrorCollection ErrorList
        {
            get
            {
                return (CompilerErrorCollection)GetValue(ErrorListProperty);
            }
            set
            {
                SetValue(ErrorListProperty, value);
            }
        }

        public static readonly DependencyProperty ErrorListProperty =
            DependencyProperty.Register("ErrorList", typeof(CompilerErrorCollection), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(null));


        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            var n = SceneNode as DynamicCodeSurface3DNode;
            n.Source = SourceCode;
            n.ParameterW = (float)ParameterW;
            n.OnCompileError += N_OnCompileError;
            base.AssignDefaultValuesToSceneNode(node);
        }

        private void N_OnCompileError(object sender, System.EventArgs e)
        {
            if (sender is DynamicCodeSurface3DNode n)
                ErrorList = n.Errors;
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new DynamicCodeSurface3DNode();
        }
    }
#endif
}
#endif
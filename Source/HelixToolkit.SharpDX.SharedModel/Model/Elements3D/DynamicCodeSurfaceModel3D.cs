using HelixToolkit.SharpDX.Model.Scene;
using System.CodeDom.Compiler;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

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
            if (d is DynamicCodeSurfaceModel3D { SceneNode: DynamicCodeSurface3DNode node })
            {
                node.Source = (string)e.NewValue;
            }
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
            if (d is DynamicCodeSurfaceModel3D { SceneNode: DynamicCodeSurface3DNode node })
            {
                node.ParameterW = (float)(double)e.NewValue;
            }
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
            if (d is DynamicCodeSurfaceModel3D { SceneNode: DynamicCodeSurface3DNode node })
            {
                node.MeshSizeU = (int)e.NewValue;
            }
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
            if (d is DynamicCodeSurfaceModel3D { SceneNode: DynamicCodeSurface3DNode node })
            {
                node.MeshSizeV = (int)e.NewValue;
            }
        }));

#if NET48
    public CompilerErrorCollection? ErrorList
    {
        get
        {
            return (CompilerErrorCollection?)GetValue(ErrorListProperty);
        }
        set
        {
            SetValue(ErrorListProperty, value);
        }
    }

    public static readonly DependencyProperty ErrorListProperty =
        DependencyProperty.Register("ErrorList", typeof(CompilerErrorCollection), typeof(DynamicCodeSurfaceModel3D), new PropertyMetadata(null));
#endif

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (SceneNode is DynamicCodeSurface3DNode n)
        {
            n.Source = SourceCode;
            n.ParameterW = (float)ParameterW;
#if NET48
            n.OnCompileError += N_OnCompileError;
#endif
        }

        base.AssignDefaultValuesToSceneNode(node);
    }

#if NET48
    private void N_OnCompileError(object? sender, System.EventArgs e)
    {
        if (sender is DynamicCodeSurface3DNode n)
            ErrorList = n.Errors;
    }
#endif

    protected override SceneNode OnCreateSceneNode()
    {
        return new DynamicCodeSurface3DNode();
    }
}

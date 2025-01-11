﻿using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class DynamicReflectionMap3D : GroupModel3D
{
    /// <summary>
    /// Gets or sets a value indicating whether [enable reflector].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable reflector]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableReflector
    {
        get
        {
            return (bool)GetValue(EnableReflectorProperty);
        }
        set
        {
            SetValue(EnableReflectorProperty, value);
        }
    }

    /// <summary>
    /// The enable reflector property
    /// </summary>
    public static readonly DependencyProperty EnableReflectorProperty =
        DependencyProperty.Register("EnableReflector", typeof(bool), typeof(DynamicReflectionMap3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.EnableReflector = (bool)e.NewValue;
            }
        }));


    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public int Size
    {
        get
        {
            return (int)GetValue(SizeProperty);
        }
        set
        {
            SetValue(SizeProperty, value);
        }
    }

    /// <summary>
    /// The size property
    /// </summary>
    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register("Size", typeof(int), typeof(DynamicReflectionMap3D), new PropertyMetadata(256, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.FaceSize = (int)e.NewValue;
            }
        }));


    /// <summary>
    /// Gets or sets the far field.
    /// </summary>
    /// <value>
    /// The far field.
    /// </value>
    public double FarField
    {
        get
        {
            return (double)GetValue(FarFieldProperty);
        }
        set
        {
            SetValue(FarFieldProperty, value);
        }
    }

    /// <summary>
    /// The far field property
    /// </summary>
    public static readonly DependencyProperty FarFieldProperty =
        DependencyProperty.Register("FarField", typeof(double), typeof(DynamicReflectionMap3D), new PropertyMetadata(100.0, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.FarField = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the near field.
    /// </summary>
    /// <value>
    /// The near field.
    /// </value>
    public double NearField
    {
        get
        {
            return (double)GetValue(NearFieldProperty);
        }
        set
        {
            SetValue(NearFieldProperty, value);
        }
    }

    /// <summary>
    /// The near field property
    /// </summary>
    public static readonly DependencyProperty NearFieldProperty =
        DependencyProperty.Register("NearField", typeof(double), typeof(DynamicReflectionMap3D), new PropertyMetadata(0.1, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.NearField = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets a value indicating whether this instance is left handed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is left handed; otherwise, <c>false</c>.
    /// </value>
    public bool IsLeftHanded
    {
        get
        {
            return (bool)GetValue(IsLeftHandedProperty);
        }
        set
        {
            SetValue(IsLeftHandedProperty, value);
        }
    }

    /// <summary>
    /// The is left handed property
    /// </summary>
    public static readonly DependencyProperty IsLeftHandedProperty =
        DependencyProperty.Register("IsLeftHanded", typeof(bool), typeof(DynamicReflectionMap3D), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.IsLeftHanded = (bool)e.NewValue;
            }
        }));


    /// <summary>
    /// Gets or sets a value indicating whether this scene is dynamic scene.
    /// If true, reflection map will be updated in each frame. Otherwise it will only be updated if scene graph or visibility changed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dynamic scene; otherwise, <c>false</c>.
    /// </value>
    public bool IsDynamicScene
    {
        get
        {
            return (bool)GetValue(IsDynamicSceneProperty);
        }
        set
        {
            SetValue(IsDynamicSceneProperty, value);
        }
    }

    public static readonly DependencyProperty IsDynamicSceneProperty =
        DependencyProperty.Register("IsDynamicScene", typeof(bool), typeof(DynamicReflectionMap3D), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3D { SceneNode: IDynamicReflector node })
            {
                node.IsDynamicScene = (bool)e.NewValue;
            }
        }));

    protected override SceneNode OnCreateSceneNode()
    {
        return new DynamicReflectionNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        base.AssignDefaultValuesToSceneNode(node);

        if (node is DynamicReflectionNode n)
        {
            n.IsDynamicScene = IsDynamicScene;
            n.IsLeftHanded = IsLeftHanded;
            n.NearField = (float)NearField;
            n.FarField = (float)FarField;
            n.EnableReflector = EnableReflector;
            n.FaceSize = Size;
        }
    }
}

using System.Runtime.Serialization;
using HelixToolkit.SharpDX.Model;

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

[DataContract]
public abstract class Material : Freezable
{
    private MaterialCore? core;

    public MaterialCore? Core
    {
        get
        {
            core ??= OnCreateCore();
            return core;
        }
    }

    public static readonly
#if AVALONIA
        new
#endif
        DependencyProperty NameProperty =
        HelixProperty.Register<Material, string>("Name", string.Empty,
            (d, e) =>
            {
                if (d is Material material && material.Core is not null)
                {
                    material.Core.Name = (string)e.NewValue!;
                }
            });

    public
#if AVALONIA
        new
#endif
        string Name
    {
        get
        {
            return (string)this.GetValue(NameProperty)!;
        }
        set
        {
            this.SetValue(NameProperty, value);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Material"/> class.
    /// </summary>
    public Material()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Material"/> class.
    /// </summary>
    /// <param name="core">The core.</param>
    public Material(MaterialCore? core)
    {
        this.core = core;
        Name = core?.Name ?? string.Empty;
    }

    public override string ToString()
    {
        return Name;
    }

    protected abstract MaterialCore OnCreateCore();

    public static implicit operator MaterialCore?(Material? m)
    {
        return m?.Core;
    }
}

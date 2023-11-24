using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class VolumeTextureModel3D : Element3D
{
    /// <summary>
    /// Gets or sets the volume material.
    /// </summary>
    /// <value>
    /// The volume material.
    /// </value>
    public Material VolumeMaterial
    {
        get
        {
            return (Material)GetValue(VolumeMaterialProperty);
        }
        set
        {
            SetValue(VolumeMaterialProperty, value);
        }
    }

    public static readonly DependencyProperty VolumeMaterialProperty =
        DependencyProperty.Register("VolumeMaterial", typeof(Material), typeof(VolumeTextureModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is VolumeTextureModel3D { SceneNode: VolumeTextureNode node })
                {
                    node.Material = (Material)e.NewValue;
                }
            }));

    protected override SceneNode OnCreateSceneNode()
    {
        return new VolumeTextureNode()
        {
            Material = VolumeMaterial
        };
    }
}

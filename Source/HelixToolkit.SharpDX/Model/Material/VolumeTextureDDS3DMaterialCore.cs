using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Default Volume Texture Material. Supports 3D DDS memory stream as <see cref="VolumeTextureMaterialCoreBase{T}.VolumeTexture"/>
/// </summary>
public sealed class VolumeTextureDDS3DMaterialCore : VolumeTextureMaterialCoreBase<TextureModel>
{
    protected override ShaderResourceViewProxy? OnCreateTexture(IEffectsManager? manager)
    {
        return manager?.MaterialTextureManager?.Register(VolumeTexture, true);
    }
}

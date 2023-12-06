using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

public class Sprite2DNode : SceneNode
{
    private TextureModel? texture;
    public TextureModel? Texture
    {
        set
        {
            if (SetAffectsRender(ref texture, value) && IsAttached)
            {
                if (EffectsManager?.MaterialTextureManager is not null && value is not null)
                {
                    (RenderCore as Sprite2DRenderCore)?.UpdateTexture(value, EffectsManager.MaterialTextureManager);
                }
            }
        }
        get
        {
            return texture;
        }
    }

    public Matrix ProjectionMatrix
    {
        set
        {
            if (RenderCore is Sprite2DRenderCore core)
            {
                core.ProjectionMatrix = value;
            }
        }
        get
        {
            if (RenderCore is Sprite2DRenderCore core)
            {
                return core.ProjectionMatrix;
            }

            return Matrix.Identity;
        }
    }

    private SpriteStruct[]? sprites;
    public SpriteStruct[]? Sprites
    {
        set
        {
            if (Set(ref sprites, value) && IsAttached)
            {
                if (bufferModel is not null)
                {
                    bufferModel.Sprites = value;
                }
            }
        }
        get
        {
            return sprites;
        }
    }

    private int spriteCount;
    public int SpriteCount
    {
        set
        {
            if (SetAffectsRender(ref spriteCount, value) && IsAttached)
            {
                if (bufferModel is not null)
                {
                    bufferModel.SpriteCount = value;
                }
            }
        }
        get
        {
            return spriteCount;
        }
    }

    private int[]? indices;
    public int[]? Indices
    {
        set
        {
            if (SetAffectsRender(ref indices, value) && IsAttached)
            {
                if (bufferModel is not null)
                {
                    bufferModel.Indices = value;
                }
            }
        }
        get
        {
            return indices;
        }
    }

    private int indexCount;
    public int IndexCount
    {
        set
        {
            if (SetAffectsRender(ref indexCount, value) && IsAttached)
            {
                if (bufferModel is not null)
                {
                    bufferModel.IndexCount = value;
                }
            }
        }
        get
        {
            return indexCount;
        }
    }

    private Sprite2DBufferModel? bufferModel;

    protected override RenderCore OnCreateRenderCore()
    {
        return new Sprite2DRenderCore();
    }

    protected override void OnAttached()
    {
        bufferModel = new Sprite2DBufferModel
        {
            Sprites = Sprites,
            SpriteCount = SpriteCount
        };
        if (texture != null && EffectTechnique?.EffectsManager?.MaterialTextureManager is not null)
        {
            (RenderCore as Sprite2DRenderCore)?.UpdateTexture(texture, EffectTechnique.EffectsManager.MaterialTextureManager);
        }
        base.OnAttached();
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref bufferModel);
        base.OnDetach();
    }

    protected override bool CanRender(RenderContext context)
    {
        return base.CanRender(context) && sprites != null && indices != null
            && spriteCount != 0 && indexCount != 0;
    }

    protected override bool CanHitTest(HitTestContext? context)
    {
        return false;
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }
}

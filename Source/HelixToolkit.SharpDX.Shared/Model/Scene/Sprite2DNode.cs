/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;
        using Utilities;

        public class Sprite2DNode : SceneNode
        {
            private TextureModel texture;
            public TextureModel Texture
            {
                set
                {
                    if (SetAffectsRender(ref texture, value) && IsAttached)
                    {
                        (RenderCore as Sprite2DRenderCore).UpdateTexture(value, EffectsManager.MaterialTextureManager);
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
                    (RenderCore as Sprite2DRenderCore).ProjectionMatrix = value;
                }
                get
                {
                    return (RenderCore as Sprite2DRenderCore).ProjectionMatrix;
                }
            }

            private SpriteStruct[] sprites;
            public SpriteStruct[] Sprites
            {
                set
                {
                    if (Set(ref sprites, value) && IsAttached)
                    {
                        bufferModel.Sprites = value;
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
                        bufferModel.SpriteCount = value;
                    }
                }
                get
                {
                    return spriteCount;
                }
            }

            private int[] indices;
            public int[] Indices
            {
                set
                {
                    if (SetAffectsRender(ref indices, value) && IsAttached)
                    {
                        bufferModel.Indices = value;
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
                        bufferModel.IndexCount = value;
                    }
                }
                get
                {
                    return indexCount;
                }
            }

            private Sprite2DBufferModel bufferModel;

            protected override RenderCore OnCreateRenderCore()
            {
                return new Sprite2DRenderCore();
            }

            protected override void OnAttached()
            {
                bufferModel = new Sprite2DBufferModel();
                bufferModel.Sprites = Sprites;
                bufferModel.SpriteCount = SpriteCount;
                if (texture != null)
                {
                    (RenderCore as Sprite2DRenderCore).UpdateTexture(texture, EffectTechnique.EffectsManager.MaterialTextureManager);
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

            protected override bool CanHitTest(HitTestContext context)
            {
                return false;
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }
}

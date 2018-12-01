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
            private Stream texture;
            public Stream Texture
            {
                set
                {
                    if (SetAffectsRender(ref texture, value) && IsAttached)
                    {
                        RemoveAndDispose(ref textureView);
                        if (value != null)
                        {
                            TextureView = EffectTechnique.EffectsManager.MaterialTextureManager.Register(value, true);
                        }
                    }
                }
                get { return texture; }
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
                    if(Set(ref sprites, value) && IsAttached)
                    {
                        bufferModel.Sprites = value;
                    }
                }
                get { return sprites; }
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
                get { return spriteCount; }
            }

            private int[] indices;
            public int[] Indices
            {
                set
                {
                    if(SetAffectsRender(ref indices, value) && IsAttached)
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
                    if(SetAffectsRender(ref indexCount, value) && IsAttached)
                    {
                        bufferModel.IndexCount = value;
                    }
                }
                get { return indexCount; }
            }

            private Sprite2DBufferModel bufferModel;

            private ShaderResourceViewProxy textureView;
            protected ShaderResourceViewProxy TextureView
            {
                set
                {
                    if(SetAffectsRender(ref textureView, value))
                    {
                        (RenderCore as Sprite2DRenderCore).TextureView = value;
                    }
                }
                get { return textureView; }
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return new Sprite2DRenderCore();
            }

            protected override void OnAttached()
            {
                bufferModel = Collect(new Sprite2DBufferModel());
                bufferModel.Sprites = Sprites;
                bufferModel.SpriteCount = SpriteCount;
                if (texture != null)
                {
                    TextureView = Collect(EffectTechnique.EffectsManager.MaterialTextureManager.Register(texture, true));
                }
                base.OnAttached();
            }

            protected override void OnDetach()
            {
                bufferModel = null;
                TextureView = null;
                base.OnDetach();
            }

            protected override bool CanRender(RenderContext context)
            {
                return base.CanRender(context) && sprites != null && indices != null 
                    && spriteCount != 0 && indexCount != 0 && textureView != null;
            }

            protected override bool CanHitTest(RenderContext context)
            {
                return false;
            }

            protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}

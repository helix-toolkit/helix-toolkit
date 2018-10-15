/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class Sprite2DNode : SceneNode
    {
        public Stream Texture
        {
            set
            {
                (RenderCore as Sprite2DRenderCore).Texture = value;
            }
            get
            {
                return (RenderCore as Sprite2DRenderCore).Texture;
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

        protected override RenderCore OnCreateRenderCore()
        {
            return new Sprite2DRenderCore();
        }

        protected override void OnAttached()
        {
            bufferModel = Collect(new Sprite2DBufferModel());
            bufferModel.Sprites = Sprites;
            bufferModel.SpriteCount = SpriteCount;
            base.OnAttached();
        }

        protected override void OnDetach()
        {
            bufferModel = null;
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && sprites != null && indices != null && spriteCount != 0 && indexCount != 0;
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

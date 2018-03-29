/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    public class PostEffectXRayNode : SceneNode
    {

        public string EffectName
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).EffectName = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).EffectName;
            }
        }

        public Color4 Color
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).Color = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).Color;
            }
        }

        public float OutlineFadingFactor
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).OutlineFadingFactor = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).OutlineFadingFactor;
            }
        }

        public bool EnableDoublePass
        {
            set
            {
                (RenderCore as IPostEffectMeshXRay).DoublePass = value;
            }
            get
            {
                return (RenderCore as IPostEffectMeshXRay).DoublePass;
            }
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PostEffectMeshXRayCore();
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}

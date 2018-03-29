/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /*
    public class ParticleStormNode : SceneNode
    {
        public IList<Matrix> Instances
        {
            set
            {
                InstanceBuffer.Elements = value;
            }
            get
            {
                return InstanceBuffer.Elements;
            }
        }

        public bool HasInstances { get { return InstanceBuffer.HasElements; } }
        public IElementsBufferModel<Matrix> InstanceBuffer { get; } = new MatrixInstanceBufferModel();

        private ParticleRenderCore particleCore
        {
            get
            {
                return (ParticleRenderCore)RenderCore;
            }
        }
        private bool blendChanged = true;

        private void OnBlendStateChanged()
        {
            blendChanged = true;
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new ParticleRenderCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ParticleStorm];
        }

        protected override bool OnAttach(IRenderHost host)
        {
            base.OnAttach(host);
            InstanceBuffer.Initialize();
            InstanceBuffer.Elements = Instances;
            particleCore.InstanceBuffer = InstanceBuffer;
            return true;
        }

        public override void Update(IRenderContext context)
        {
            base.Update(context);
            if (blendChanged)
            {
                var desc = new BlendStateDescription();
                desc.RenderTarget[0] = new RenderTargetBlendDescription
                {
                    IsBlendEnabled = true,
                    BlendOperation = this.Blend,
                    AlphaBlendOperation = this.AlphaBlend,
                    SourceBlend = this.SourceBlend,
                    DestinationBlend = this.DestBlend,
                    SourceAlphaBlend = this.SourceAlphaBlend,
                    DestinationAlphaBlend = this.DestAlphaBlend,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };
                particleCore.BlendDescription = desc;
                blendChanged = false;
            }
        }

        protected override void OnDetach()
        {
            InstanceBuffer.Dispose();
            base.OnDetach();
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
    */
}

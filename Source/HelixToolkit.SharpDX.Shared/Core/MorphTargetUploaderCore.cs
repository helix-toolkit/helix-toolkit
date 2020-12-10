/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;

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
    namespace Core
    {
        using Render;
        using Utilities;
        using Shaders;

        class MorphTargetUploaderCore : RenderCore
        {
            //TODO: add morph target offset buffer support as well

            public event EventHandler WeightsChanged;
            private bool weightUpdated;
            private float[] morphTargetWeights = new float[0];
            public float[] MorphTargetWeights
            {
                get { return morphTargetWeights; }
                set
                {
                    if (SetAffectsRender(ref morphTargetWeights, value))
                    {
                        weightUpdated = true;
                        WeightsChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            public StructuredBufferProxy MTWeightsB { get; private set; }

            public MorphTargetUploaderCore()
                : base(RenderType.None)
            {
                NeedUpdate = false;
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                
            }

            protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (weightUpdated)
                {
                    MTWeightsB.UploadDataToBuffer(deviceContext, morphTargetWeights, morphTargetWeights.Length, 0);
                    weightUpdated = false;
                }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                MTWeightsB = Collect(new StructuredBufferProxy(sizeof(float), false));
                return true;
            }

            protected override void OnDetach()
            {
                MTWeightsB = null;
                base.OnDetach();
            }

            public void BindBuffers(DeviceContextProxy devCtx, int slot)
            {
                devCtx.SetShaderResource(VertexShader.Type, slot, MTWeightsB);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                    WeightsChanged = null;
                base.OnDispose(disposeManagedResources);
            }
        }
	}
}

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
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
        using Components;

        class MorphTargetUploaderCore : RenderCore
        {
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

            private bool setDeltas = false;
            private Vector4[] morphTargetsDeltas = null;

            private bool setCBuffer = false;
            private int mtCount;
            private int mtPitch;

            public StructuredBufferProxy MTWeightsB { get; private set; }
            public ImmutableBufferProxy MTDeltasB { get; private set; }

            private ShaderResourceViewProxy mtDeltasSRV;

            private ConstantBufferComponent cbMorphTarget;

            public MorphTargetUploaderCore()
                : base(RenderType.None)
            {
                NeedUpdate = false;

                //Setup cbuffer
                var cbd = new ConstantBufferDescription(DefaultBufferNames.MorphTargetCB, 16); //maybe no slot issue
                cbMorphTarget = Collect(new ConstantBufferComponent(cbd));
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

                if (setDeltas)
                {
                    int c = morphTargetsDeltas.Length;
                    MTDeltasB.UploadDataToBuffer(deviceContext, morphTargetsDeltas, c);

                    //Handle srv
                    mtDeltasSRV = Collect(new ShaderResourceViewProxy(MTDeltasB.Buffer.Device, MTDeltasB.Buffer));
                    mtDeltasSRV.CreateTextureView();

                    setDeltas = false;
                }

                if (setCBuffer)
                {
                    //Set Values
                    cbMorphTarget.WriteValue<int>(mtCount, 0);
                    cbMorphTarget.WriteValue<int>(mtPitch, sizeof(int));

                    //Update/upload or whatever
                    cbMorphTarget.Upload(deviceContext);

                    setCBuffer = false;
                }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                MTWeightsB = Collect(new StructuredBufferProxy(sizeof(float), false));
                cbMorphTarget.Attach(technique);
                return true;
            }

            protected override void OnDetach()
            {
                MTWeightsB = null;
                MTDeltasB = null;
                cbMorphTarget.Detach();
                base.OnDetach();
            }

            public void BindBuffers(DeviceContextProxy devCtx, int weightsSlot, int deltasSlot)
            {
                devCtx.SetShaderResource(VertexShader.Type, weightsSlot, MTWeightsB);
                devCtx.SetShaderResource(VertexShader.Type, deltasSlot, mtDeltasSRV);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                    WeightsChanged = null;
                base.OnDispose(disposeManagedResources);
            }

            public bool InitializeMorphTargets(MorphTargetVertex[] targets, int pitch)
            {
                //The buffer is immutable, if it was already created, dont allow for recreation
                if (MTDeltasB != null)
                    return false;

                //Setup buffer and keep track of data to update
                MTDeltasB = new ImmutableBufferProxy(sizeof(float) * 4, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured);
                setDeltas = true;

                morphTargetsDeltas = new Vector4[targets.Length * 3];
                for (int i = 0; i < targets.Length; i++)
                {
                    morphTargetsDeltas[i * 3] = targets[i].deltaPosition.ToVector4();
                    morphTargetsDeltas[i * 3 + 1] = targets[i].deltaNormal.ToVector4();
                    morphTargetsDeltas[i * 3 + 2] = targets[i].deltaTangent.ToVector4();
                }

                //Set cbuffer data {int count, int pitch}
                setCBuffer = true;
                mtCount = targets.Length / pitch;
                mtPitch = pitch;

                return true;
            }
        }
	}
}

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
            private Vector3[] morphTargetsDeltas = null;

            private int[] morphTargetOffsets = null;

            private bool setCBuffer = false;
            private int mtCount;
            private int mtPitch;

            public StructuredBufferProxy MTWeightsB { get; private set; }
            public ImmutableBufferProxy MTDeltasB { get; private set; }
            public ImmutableBufferProxy MTOffsetsB { get; private set; }

            private ShaderResourceViewProxy mtDeltasSRV;
            private ShaderResourceViewProxy mtOffsetsSRV;

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
                    //Setup deltas buffer
                    int c = morphTargetsDeltas.Length;
                    MTDeltasB.UploadDataToBuffer(deviceContext, morphTargetsDeltas, c);

                    //Handle deltas srv
                    mtDeltasSRV = Collect(new ShaderResourceViewProxy(MTDeltasB.Buffer.Device, MTDeltasB.Buffer));
                    mtDeltasSRV.CreateTextureView();

                    //Setup offsets buffer
                    c = morphTargetOffsets.Length;
                    MTOffsetsB.UploadDataToBuffer(deviceContext, morphTargetOffsets, c);

                    //Handle offsets srv
                    mtOffsetsSRV = Collect(new ShaderResourceViewProxy(MTOffsetsB.Buffer.Device, MTOffsetsB.Buffer));
                    mtOffsetsSRV.CreateTextureView();


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
                MTOffsetsB = null;
                cbMorphTarget.Detach();
                base.OnDetach();
            }

            public void BindBuffers(DeviceContextProxy devCtx, int weightsSlot, int deltasSlot, int offsetsSlot)
            {
                devCtx.SetShaderResource(VertexShader.Type, weightsSlot, MTWeightsB);
                devCtx.SetShaderResource(VertexShader.Type, deltasSlot, mtDeltasSRV);
                devCtx.SetShaderResource(VertexShader.Type, offsetsSlot, mtOffsetsSRV);
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
                MTDeltasB = new ImmutableBufferProxy(sizeof(float) * 3, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured);
                MTOffsetsB = new ImmutableBufferProxy(sizeof(int), BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured);
                setDeltas = true;

                //Setup arrays for morph target data
                FastList<Vector3> mtdList = new FastList<Vector3>(targets.Length * 3);
                morphTargetOffsets = new int[targets.Length];

                //First element is always 0 delta
                mtdList.Add(Vector3.Zero);
                mtdList.Add(Vector3.Zero);
                mtdList.Add(Vector3.Zero);

                //Subsequent elements should never need 0 delta vertex
                Vector3 zv = Vector3.Zero;

                int current = 1;
                for (int i = 0; i < targets.Length; i++)
                {
                    //Skip if 0 delta
                    if (targets[i].deltaNormal == zv && targets[i].deltaPosition == zv && targets[i].deltaTangent == zv)
                    {
                        morphTargetOffsets[i] = 0;
                    }
                    else
                    {
                        morphTargetOffsets[i] = current * 3;

                        mtdList.Add(targets[i].deltaPosition);
                        mtdList.Add(targets[i].deltaNormal);
                        mtdList.Add(targets[i].deltaTangent);

                        current++;
                    }
                }
                morphTargetsDeltas = mtdList.ToArray();

                //Set cbuffer data {int count, int pitch}
                setCBuffer = true;
                mtCount = targets.Length / pitch;
                mtPitch = pitch;

                return true;
            }

            public void SetWeight(int i, float w)
            {
                weightUpdated = true;
                WeightsChanged?.Invoke(this, EventArgs.Empty);
                MorphTargetWeights[i] = w;
            }
        }
	}
}

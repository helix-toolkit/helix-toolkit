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
            private float[] morphTargetWeights = Array.Empty<float>();
            public float[] MorphTargetWeights
            {
                get
                {
                    return morphTargetWeights;
                }
                set
                {
                    if (SetAffectsRender(ref morphTargetWeights, value ?? Array.Empty<float>()))
                    {
                        weightUpdated = true;
                        WeightsChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            private bool setDeltas = false;
            private Vector3[] morphTargetsDeltas = Array.Empty<Vector3>();

            private int[] morphTargetOffsets = Array.Empty<int>();

            private bool hasMorphTarget => mtCount > 0 && mtPitch > 0;
            private bool setCBuffer = true;
            private int mtCount;
            private int mtPitch;
            private StructuredBufferProxy mtWeightsB;
            private ImmutableBufferProxy mtDeltasB;
            private ImmutableBufferProxy mtOffsetsB;
            public StructuredBufferProxy MTWeightsB => mtWeightsB;
            public ImmutableBufferProxy MTDeltasB => mtDeltasB;
            public ImmutableBufferProxy MTOffsetsB => mtOffsetsB;

            private ShaderResourceViewProxy mtDeltasSRV;
            private ShaderResourceViewProxy mtOffsetsSRV;

            private ConstantBufferComponent cbMorphTarget;

            public MorphTargetUploaderCore()
                : base(RenderType.None)
            {
                NeedUpdate = false;

                //Setup cbuffer
                var cbd = new ConstantBufferDescription(DefaultBufferNames.MorphTargetCB, 16); //maybe no slot issue
                cbMorphTarget = AddComponent(new ConstantBufferComponent(cbd));
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
                    var c = morphTargetsDeltas.Length;
                    MTDeltasB.UploadDataToBuffer(deviceContext, morphTargetsDeltas, c);
                    RemoveAndDispose(ref mtDeltasSRV);
                    //Handle deltas srv
                    mtDeltasSRV = new ShaderResourceViewProxy(MTDeltasB.Buffer.Device, MTDeltasB.Buffer);
                    mtDeltasSRV.CreateTextureView();

                    //Setup offsets buffer
                    c = morphTargetOffsets.Length;
                    MTOffsetsB.UploadDataToBuffer(deviceContext, morphTargetOffsets, c);
                    RemoveAndDispose(ref mtOffsetsSRV);
                    //Handle offsets srv
                    mtOffsetsSRV = new ShaderResourceViewProxy(MTOffsetsB.Buffer.Device, MTOffsetsB.Buffer);
                    mtOffsetsSRV.CreateTextureView();


                    setDeltas = false;
                }

                if (setCBuffer)
                {
                    //Set Values
                    cbMorphTarget.WriteValue<int>(mtCount, 0);
                    cbMorphTarget.WriteValue<int>(mtPitch, sizeof(int));

                    setCBuffer = false;
                }
                //Update/upload or whatever
                cbMorphTarget.Upload(deviceContext);
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                mtWeightsB = new StructuredBufferProxy(sizeof(float), false);
                mtDeltasB = new ImmutableBufferProxy(sizeof(float) * 3, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured);
                mtOffsetsB = new ImmutableBufferProxy(sizeof(int), BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured);
                return true;
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref mtWeightsB);
                RemoveAndDispose(ref mtDeltasB);
                RemoveAndDispose(ref mtOffsetsB);
            }

            public void BindBuffers(DeviceContextProxy devCtx, int weightsSlot, int deltasSlot, int offsetsSlot)
            {
                if (hasMorphTarget)
                {
                    devCtx.SetShaderResource(VertexShader.Type, weightsSlot, MTWeightsB);
                    devCtx.SetShaderResource(VertexShader.Type, deltasSlot, mtDeltasSRV);
                    devCtx.SetShaderResource(VertexShader.Type, offsetsSlot, mtOffsetsSRV);
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                    WeightsChanged = null;
                base.OnDispose(disposeManagedResources);
            }

            public bool InitializeMorphTargets(MorphTargetVertex[] targets, int pitch)
            {
                if (targets == null || targets.Length == 0)
                {
                    mtCount = 0;
                    mtPitch = 0;
                    return true;
                }
                //Setup buffer and keep track of data to update
                setDeltas = true;

                //Setup arrays for morph target data
                var mtdList = new FastList<Vector3>(targets.Length * 3);
                morphTargetOffsets = new int[targets.Length];

                //First element is always 0 delta
                mtdList.Add(Vector3.Zero);
                mtdList.Add(Vector3.Zero);
                mtdList.Add(Vector3.Zero);

                //Subsequent elements should never need 0 delta vertex
                var zv = Vector3.Zero;

                var current = 1;
                for (var i = 0; i < targets.Length; i++)
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
                MorphTargetWeights[i] = w;
                InvalidateWeight();
            }

            public void InvalidateWeight()
            {
                weightUpdated = true;
                WeightsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

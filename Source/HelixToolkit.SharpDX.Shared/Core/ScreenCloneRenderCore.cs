/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.InteropServices;
using Texture2D = SharpDX.Direct3D11.Texture2D;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using HelixToolkit.Wpf.SharpDX.Utilities;
    using Render;
    using Shaders;

    public class ScreenCloneRenderCore : RenderCoreBase<ModelStruct>
    {
        private int output = 0;
        public int Output
        {
            set
            {
                if(Set(ref output, value))
                {
                    Detach();
                    if (IsAttached)
                    {
                        Attach(this.EffectTechnique);
                    }
                }
            }
            get
            {
                return output;
            }
        }

        private DuplicationResource duplicationResource;
        private IShaderPass DefaultShaderPass;
        private int textureBindSlot = 0;
        private int samplerBindSlot = 0;
        private SamplerProxy textureSampler;

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                DefaultShaderPass = technique.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication][DefaultPassNames.Default];// technique[DefaultPassNames.Default];
                textureBindSlot = DefaultShaderPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerBindSlot = DefaultShaderPass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                textureSampler = Collect(new SamplerProxy(technique.EffectsManager.StateManager, DefaultSamplers.LinearSamplerWrapAni2));
                return Initialize(technique.EffectsManager);
            }
            else
            {
                return false;
            }
        }

        private bool Initialize(IEffectsManager manager)
        {
            RemoveAndDispose(ref duplicationResource);
            duplicationResource = Collect(new DuplicationResource(manager.Device, Output));
            bool succ = duplicationResource.Initialize();
            if (!succ)
            {
                RemoveAndDispose(ref duplicationResource);
            }
            return succ;
        }

        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && duplicationResource != null;
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            FrameData data;
            bool isTimeOut;
            if (duplicationResource.GetFrame(out data, out isTimeOut))
            {
                if (data.FrameInfo.TotalMetadataBufferSize > 0)
                {
                    DefaultShaderPass.BindShader(deviceContext);
                    DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
                    deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = global::SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;
                    DefaultShaderPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerBindSlot, textureSampler);
                    using (var textureView = new global::SharpDX.Direct3D11.ShaderResourceView(deviceContext.DeviceContext.Device, data.Frame))
                    {
                        DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureBindSlot, textureView);                                      
                        deviceContext.DeviceContext.Draw(4, 0);
                    }
                }
            }
            if (isTimeOut)
            {
                InvalidateRenderer();
            }
            else
            {
                duplicationResource.ReleaseFrame();
                InvalidateRenderer();
            }
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {

        }

        private class DuplicationResource : DisposeObject
        {
            public OutputDescription OutputDescription { get { return outputDesc; } }
            private OutputDescription outputDesc;
            private OutputDuplication duplication;
            private bool IsInitialized = false;

            private Texture2D texture2D;

            private readonly global::SharpDX.Direct3D11.Device device;
            private readonly int outputIndex;

            private int metaDataSize = 0;
            private OutputDuplicateMoveRectangle[] moveBuffer = null;
            private RawRectangle[] dirtyBuffer = null;

            public DuplicationResource(global::SharpDX.Direct3D11.Device device, int outputIdx)
            {
                this.device = device;
                this.outputIndex = outputIdx;
            }

            public bool Initialize()
            {
                using (var dxgDevice = device.QueryInterface<Device>())
                {
                    using (var adapter = dxgDevice.GetParent<Adapter>())
                    {
                        if(adapter.GetOutputCount() <= outputIndex)
                        {
                            return false;
                        }
                        using (var output = adapter.GetOutput(outputIndex))
                        {
                            outputDesc = output.Description;
                            using (var output1 = output.QueryInterface<Output1>())
                            {
                                duplication = Collect(output1.DuplicateOutput(device));
                                IsInitialized = true;
                                return true;
                            }
                        }                     
                    }
                }
            }
            

            public bool GetFrame(out FrameData data, out bool timeOut)
            {
                data = new FrameData();
                if (!IsInitialized)
                {
                    timeOut = false;
                    return false;
                }
                OutputDuplicateFrameInformation frameInfo;
                Resource desktopResource;
                try
                {
                    duplication.AcquireNextFrame(500, out frameInfo, out desktopResource);
                }
                catch(SharpDXException ex)
                {
                    if(ex.ResultCode.Code == ResultCode.WaitTimeout.Result.Code)
                    {
                        timeOut = true;
                        return false;
                    }
                    else
                    {
                        throw new HelixToolkitException("Failed to acquire next frame.");
                    }
                }
                texture2D = Collect(desktopResource.QueryInterface<global::SharpDX.Direct3D11.Texture2D>());
                desktopResource.Dispose();
                timeOut = false;
                if (frameInfo.TotalMetadataBufferSize > 0)
                {
                    if (frameInfo.TotalMetadataBufferSize > metaDataSize)
                    {
                        metaDataSize = frameInfo.TotalMetadataBufferSize;
                    }
                    int moveRectSize;
                    if (moveBuffer == null || moveBuffer.Length < metaDataSize)
                    {
                        moveBuffer = new OutputDuplicateMoveRectangle[metaDataSize];
                    }
                    duplication.GetFrameMoveRects(metaDataSize, moveBuffer, out moveRectSize);
                    data.MoveRectangles = moveBuffer;
                    data.MoveCount = moveRectSize / Marshal.SizeOf(typeof(OutputDuplicateMoveRectangle));

                    if (dirtyBuffer == null || dirtyBuffer.Length < metaDataSize)
                    {
                        dirtyBuffer = new RawRectangle[metaDataSize];
                    }
                    int dirtyRectSize;
                    duplication.GetFrameDirtyRects(metaDataSize, dirtyBuffer, out dirtyRectSize);
                    data.DirtyRectangles = dirtyBuffer;
                    data.DirtyCount = dirtyRectSize / Marshal.SizeOf(typeof(RawRectangle));
                    data.FrameInfo = frameInfo;
                    data.Frame = texture2D;
                    return true;
                }
                else
                {
                    data.DirtyCount = 0;
                    data.MoveCount = 0;
                    return false;
                }
            }

            public void ReleaseFrame()
            {                                
                RemoveAndDispose(ref texture2D);
                duplication.ReleaseFrame();
            }
        }

        private struct FrameData
        {
            public global::SharpDX.Direct3D11.Texture2D Frame;
            public OutputDuplicateFrameInformation FrameInfo;
            public RawRectangle[] DirtyRectangles;
            public OutputDuplicateMoveRectangle[] MoveRectangles;
            public int DirtyCount;
            public int MoveCount;
        }
    }
}

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
using ResourceRegion = SharpDX.Direct3D11.ResourceRegion;

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
        private FrameProcessResource frameResource;
        private IShaderPass DefaultShaderPass;

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                DefaultShaderPass = technique.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication][DefaultPassNames.Default];// technique[DefaultPassNames.Default];
               
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
            RemoveAndDispose(ref frameResource);
            frameResource = Collect(new FrameProcessResource(manager, DefaultShaderPass));
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
            if(duplicationResource.GetFrame(out data, out isTimeOut))
            {
                if (isTimeOut)
                {
                    InvalidateRenderer();
                    return;
                }
                DefaultShaderPass.BindShader(deviceContext);
                DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
                var targets = deviceContext.DeviceContext.OutputMerger.GetRenderTargets(1);
                using (var sharedTexture = targets[0].QueryInterface<Texture2D>())
                {
                    frameResource.ProcessFrame(deviceContext, ref data, sharedTexture, 0, 0, duplicationResource.OutputDescription);
                }
                foreach(var target in targets)
                {
                    target.Dispose();
                }
            }
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {

        }



        private class FrameProcessResource : DisposeObject
        {
            private Texture2D moveSurf;
            private readonly global::SharpDX.Direct3D11.Device device;

            private readonly DynamicBufferProxy vertexBuffer;
            private readonly IShaderPass shaderPass;
            private readonly int textureBindSlot = 0;
            private readonly int samplerBindSlot = 0;
            private SamplerProxy textureSampler;
            public FrameProcessResource(IEffectsManager manager, IShaderPass shaderPass)
            {
                this.device = manager.Device;
                this.shaderPass = shaderPass;
                textureBindSlot = shaderPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerBindSlot = shaderPass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                textureSampler = Collect(new SamplerProxy(manager.StateManager, DefaultSamplers.LinearSamplerWrapAni2));
                vertexBuffer = Collect(new DynamicBufferProxy(ScreenDuplicationVertexStruct.SizeInBytes, global::SharpDX.Direct3D11.BindFlags.VertexBuffer));
            }

            public bool ProcessFrame(DeviceContextProxy deviceContext,
                ref FrameData data, global::SharpDX.Direct3D11.Texture2D sharedSurface, int offsetX, int offsetY, OutputDescription deskDesc)
            {
                if(data.FrameInfo.TotalMetadataBufferSize > 0)
                {
                    var desc = data.Frame.Description;
                    if (data.MoveCount > 0)
                    {
                        CopyMove(deviceContext, sharedSurface, ref data, offsetX, offsetY, ref deskDesc, desc.Width, desc.Height);
                    }
                    if (data.DirtyCount > 0)
                    {
                        CopyDirty(deviceContext, sharedSurface, data.Frame, ref data.DirtyRectangles, data.DirtyCount, offsetX, offsetY, ref deskDesc);
                    }
                    return true;
                }
                return false;
            }

            private void CopyMove(DeviceContextProxy deviceContext, Texture2D sharedSurface, ref FrameData data, int offsetX, int offsetY, ref OutputDescription deskDesc, int texWidth, int texHeight)
            {
                var desc = sharedSurface.Description;
                if (moveSurf == null)
                {
                    var moveSurfDesc = desc;
                    moveSurfDesc.Width = deskDesc.DesktopBounds.Right - deskDesc.DesktopBounds.Left;
                    moveSurfDesc.Height = deskDesc.DesktopBounds.Bottom - deskDesc.DesktopBounds.Top;
                    moveSurfDesc.BindFlags = global::SharpDX.Direct3D11.BindFlags.RenderTarget;
                    moveSurfDesc.OptionFlags = global::SharpDX.Direct3D11.ResourceOptionFlags.None;
                    moveSurf = Collect(new Texture2D(device, moveSurfDesc));
                }
                for(int i=0; i<data.MoveCount; ++i)
                {
                    RawRectangle sourceRect, destRect;
                    SetMoveRect(out sourceRect, out destRect, ref deskDesc, ref data.MoveRectangles[i], texWidth, texHeight);
                    ResourceRegion box = new ResourceRegion()
                    {
                        Left = sourceRect.Left + deskDesc.DesktopBounds.Left - offsetX,
                        Top = sourceRect.Top + deskDesc.DesktopBounds.Top - offsetY,
                        Right = sourceRect.Right + deskDesc.DesktopBounds.Left - offsetX,
                        Bottom = sourceRect.Bottom + deskDesc.DesktopBounds.Top - offsetY,
                        Front=0,
                        Back =1
                    };
                    deviceContext.DeviceContext.CopySubresourceRegion(sharedSurface, 0, box, moveSurf, 0, sourceRect.Left, sourceRect.Top, 0);

                    box.Left = sourceRect.Left;
                    box.Top = sourceRect.Top;
                    box.Right = sourceRect.Right;
                    box.Bottom = sourceRect.Bottom;
                    deviceContext.DeviceContext.CopySubresourceRegion(moveSurf, 0, box, sharedSurface, 0, destRect.Left + deskDesc.DesktopBounds.Left - offsetX, destRect.Top + deskDesc.DesktopBounds.Top - offsetY, 0);
                }
            }

            private void SetMoveRect(out RawRectangle sourceRect, out RawRectangle destRect, ref OutputDescription deskDesc, ref OutputDuplicateMoveRectangle moveRect, int width, int height)
            {
                sourceRect = new RawRectangle();
                destRect = new RawRectangle();
                switch (deskDesc.Rotation)
                {
                    case DisplayModeRotation.Unspecified:
                    case DisplayModeRotation.Identity:
                        sourceRect.Left = moveRect.SourcePoint.X;
                        sourceRect.Top = moveRect.SourcePoint.Y;
                        sourceRect.Right = moveRect.SourcePoint.X + moveRect.DestinationRect.Right - moveRect.DestinationRect.Left;
                        sourceRect.Bottom = moveRect.SourcePoint.Y + moveRect.DestinationRect.Bottom - moveRect.DestinationRect.Top;
                        break;
                    case DisplayModeRotation.Rotate90:
                        sourceRect.Left = height - (moveRect.SourcePoint.Y + moveRect.DestinationRect.Bottom - moveRect.DestinationRect.Top);
                        sourceRect.Top = moveRect.SourcePoint.X;
                        sourceRect.Right = height - moveRect.SourcePoint.Y;
                        sourceRect.Bottom = moveRect.SourcePoint.X + moveRect.DestinationRect.Right - moveRect.DestinationRect.Left;

                        destRect.Left = height - moveRect.DestinationRect.Bottom;
                        destRect.Top = moveRect.DestinationRect.Left;
                        destRect.Right = height - moveRect.DestinationRect.Top;
                        destRect.Bottom = moveRect.DestinationRect.Right;
                        break;
                    case DisplayModeRotation.Rotate180:
                        sourceRect.Left = width - (moveRect.SourcePoint.X + moveRect.DestinationRect.Right - moveRect.DestinationRect.Left);
                        sourceRect.Top = height - (moveRect.SourcePoint.Y + moveRect.DestinationRect.Bottom - moveRect.DestinationRect.Top);
                        sourceRect.Right = width - moveRect.SourcePoint.X;
                        sourceRect.Bottom = height - moveRect.SourcePoint.Y;

                        destRect.Left = width - moveRect.DestinationRect.Right;
                        destRect.Top = height - moveRect.DestinationRect.Bottom;
                        destRect.Right = width - moveRect.DestinationRect.Left;
                        destRect.Bottom = height - moveRect.DestinationRect.Top;
                        break;
                    case DisplayModeRotation.Rotate270:
                        sourceRect.Left = moveRect.SourcePoint.X;
                        sourceRect.Top = width - (moveRect.SourcePoint.X + moveRect.DestinationRect.Right - moveRect.DestinationRect.Left);
                        sourceRect.Right = moveRect.SourcePoint.Y + moveRect.DestinationRect.Bottom - moveRect.DestinationRect.Top;
                        sourceRect.Bottom = width - moveRect.SourcePoint.X;

                        destRect.Left = moveRect.DestinationRect.Top;
                        destRect.Top = width - moveRect.DestinationRect.Right;
                        destRect.Right = moveRect.DestinationRect.Bottom;
                        destRect.Bottom = width - moveRect.DestinationRect.Left;
                        break;
                    default:
                        break;
                }
            }

            private void CopyDirty(DeviceContextProxy deviceContext,
                Texture2D sharedSurface, Texture2D sourceSurface, ref RawRectangle[] dirtyBuffer, int dirtyCount, 
                int offsetX, int offsetY, ref OutputDescription deskDesc)
            {
                using (var sourceView = new global::SharpDX.Direct3D11.ShaderResourceView(device, sourceSurface, new global::SharpDX.Direct3D11.ShaderResourceViewDescription()
                {
                    Format = sourceSurface.Description.Format, Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                    Texture2D = new global::SharpDX.Direct3D11.ShaderResourceViewDescription.Texture2DResource()
                    {
                        MostDetailedMip = sourceSurface.Description.MipLevels - 1,
                        MipLevels = sourceSurface.Description.MipLevels
                    }
                }))
                {
                    shaderPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureBindSlot, sourceView);
                    shaderPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerBindSlot, textureSampler);
                    var vertices = new ScreenDuplicationVertexStruct[6 * dirtyCount];
                    int startIdx = 0;
                    for (int i = 0; i < dirtyCount; ++i, startIdx += 6)
                    {
                        SetDirtyVert(vertices, startIdx, ref dirtyBuffer[i], offsetX, offsetY, ref deskDesc, 
                            sharedSurface.Description.Width, sharedSurface.Description.Height, sourceSurface.Description.Width, sourceSurface.Description.Height);
                    }

                    vertexBuffer.UploadDataToBuffer(deviceContext, vertices, vertices.Length);
                    deviceContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new global::SharpDX.Direct3D11.VertexBufferBinding(vertexBuffer.Buffer, 
                        ScreenDuplicationVertexStruct.SizeInBytes, 0));
                    deviceContext.DeviceContext.Draw(vertices.Length, 0);
                }
            }

            private void SetDirtyVert(ScreenDuplicationVertexStruct[] verts, int startIdx, ref RawRectangle dirtyRect, int offsetX, int offsetY, ref OutputDescription deskDesc, 
                int targetWidth, int targetHeight, int sourceWidth, int sourceHeight)
            {
                int centerX = targetWidth / 2;
                int centerY = targetHeight / 2;
                int width = deskDesc.DesktopBounds.Right - deskDesc.DesktopBounds.Left;
                int height = deskDesc.DesktopBounds.Bottom - deskDesc.DesktopBounds.Top;
                switch (deskDesc.Rotation)
                {
                    case DisplayModeRotation.Unspecified:
                    case DisplayModeRotation.Identity:
                        verts[0 + startIdx].TexCoord = new Vector2((float)dirtyRect.Left / sourceWidth, (float)dirtyRect.Bottom / sourceHeight);
                        verts[1 + startIdx].TexCoord = new Vector2((float)dirtyRect.Left / sourceWidth, (float)dirtyRect.Top / sourceHeight);
                        verts[2 + startIdx].TexCoord = new Vector2((float)dirtyRect.Right / sourceWidth, (float)dirtyRect.Bottom / sourceHeight);
                        verts[5 + startIdx].TexCoord = new Vector2((float)dirtyRect.Right / sourceWidth, (float)dirtyRect.Top / sourceHeight);
                        break;
                }

                verts[0 + startIdx].Position = new Vector4((dirtyRect.Left + deskDesc.DesktopBounds.Left - offsetX - centerX) / centerX,
                         -1 * (dirtyRect.Bottom + deskDesc.DesktopBounds.Top - offsetY - centerY) / (float)centerY,
                         0.0f, 0.0f);
                verts[1 + startIdx].Position = new Vector4((dirtyRect.Left + deskDesc.DesktopBounds.Left - offsetX - centerX) / centerX,
                                         -1 * (dirtyRect.Top + deskDesc.DesktopBounds.Top - offsetY - centerY) / (float)centerY,
                                         0.0f, 0.0f);
                verts[2 + startIdx].Position = new Vector4((dirtyRect.Right + deskDesc.DesktopBounds.Left - offsetX - centerX) / centerX,
                                         -1 * (dirtyRect.Bottom + deskDesc.DesktopBounds.Top - offsetY - centerY) / (float)centerY,
                                         0.0f, 0.0f);
                verts[3 + startIdx].Position = verts[2].Position;
                verts[4 + startIdx].Position = verts[1].Position;
                verts[5 + startIdx].Position = new Vector4((dirtyRect.Right + deskDesc.DesktopBounds.Left - offsetX - centerX) / centerX,
                                         -1 * (dirtyRect.Top + deskDesc.DesktopBounds.Top - offsetY - centerY) / (float)centerY,
                                         0.0f, 0.0f);

                verts[3 + startIdx].TexCoord = verts[2].TexCoord;
                verts[4 + startIdx].TexCoord = verts[1].TexCoord;
            }
        }


        private class DuplicationResource : DisposeObject
        {
            public OutputDescription OutputDescription { get { return outputDesc; } }
            private OutputDescription outputDesc;
            private OutputDuplication duplication;
            private bool IsInitialized = false;

            private global::SharpDX.Direct3D11.Texture2D texture2D;

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
                duplication.AcquireNextFrame(500, out frameInfo, out desktopResource);
                if(desktopResource == null)
                {
                    timeOut = true;
                    return false;
                }
                RemoveAndDispose(ref texture2D);
                texture2D = Collect(desktopResource.QueryInterface<global::SharpDX.Direct3D11.Texture2D>());
                RemoveAndDispose(ref desktopResource);
                timeOut = false;
                if (frameInfo.TotalMetadataBufferSize > 0)
                {
                    if (frameInfo.TotalMetadataBufferSize > metaDataSize)
                    {
                        metaDataSize = frameInfo.TotalMetadataBufferSize;
                    }
                    int moveRectSize;
                    if(moveBuffer == null || moveBuffer.Length < metaDataSize)
                    {
                        moveBuffer = new OutputDuplicateMoveRectangle[metaDataSize];
                    }
                    duplication.GetFrameMoveRects(metaDataSize, moveBuffer, out moveRectSize);
                    data.MoveRectangles = moveBuffer;
                    data.MoveCount = moveRectSize / Marshal.SizeOf(typeof(OutputDuplicateMoveRectangle));

                    if(dirtyBuffer == null || dirtyBuffer.Length < metaDataSize)
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

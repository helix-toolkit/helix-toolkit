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
{
    using Utilities;
    using Render;
    using Shaders;
    using System.Collections.Generic;
    using System.Linq;
    /// <summary>
    /// 
    /// </summary>
    public interface IScreenClone
    {
        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
        /// </value>
        int Output { set; get; }

        /// <summary>
        /// Gets or sets the clone rectangle.
        /// </summary>
        /// <value>
        /// The clone rectangle.
        /// </value>
        Rectangle CloneRectangle { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether cloned rectangle is stretched during rendering, default is false;
        /// </summary>
        /// <value>
        ///   <c>true</c> if stretch; otherwise, <c>false</c>.
        /// </value>
        bool StretchToFill { set; get; }
    }
    /// <summary>
    /// Limitation: Under switchable graphics card setup(Laptop with integrated graphics card and external graphics card), 
    /// only monitor outputs using integrated graphics card is fully supported.
    /// Trying to clone monitor outputs by external graphics card, 
    /// the clone window must reside in those monitors which is rendered by external graphics card, or error will be occurred.
    /// Ref: https://support.microsoft.com/en-us/help/3019314/error-generated-when-desktop-duplication-api-capable-application-is-ru
    /// </summary>
    public class ScreenCloneRenderCore : RenderCoreBase<ScreenDuplicationModelStruct>, IScreenClone
    {
        private int output = 0;
        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
        /// </value>
        public int Output
        {
            set
            {
                Set(ref output, value);
            }
            get
            {
                return output;
            }
        }

        private Rectangle cloneRectangle = new Rectangle();
        /// <summary>
        /// Gets or sets the clone rectangle.
        /// </summary>
        /// <value>
        /// The clone rectangle.
        /// </value>
        public Rectangle CloneRectangle
        {
            set
            {
                if(Set(ref cloneRectangle, value))
                {
                    clearTarget = true;
                }
            }
            get { return cloneRectangle; }
        }
        /// <summary>
        /// Gets or sets a value indicating cloned rectangle is stretched during rendering, default is false;
        /// </summary>
        /// <value>
        ///   <c>true</c> if stretch; otherwise, <c>false</c>.
        /// </value>
        public bool StretchToFill
        {
            set;
            get;
        } = false;

        private DuplicationResource duplicationResource;
        private IShaderPass DefaultShaderPass;
        private int textureBindSlot = 0;
        private int samplerBindSlot = 0;
        private SamplerProxy textureSampler;
        private bool clearTarget = true;

        private DX11RenderHostConfiguration config = new DX11RenderHostConfiguration() { ClearEachFrame = false, RenderD2D = false, RenderLights = false, UpdatePerFrameData = false };
        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ScreenDuplicationCB, ScreenDuplicationModelStruct.SizeInBytes);
        }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="technique">The technique.</param>
        /// <returns></returns>
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
            duplicationResource = Collect(new DuplicationResource(manager.Device));
            bool succ = duplicationResource.Initialize();
            if (!succ)
            {
                RemoveAndDispose(ref duplicationResource);
            }
            return succ;
        }
        /// <summary>
        /// Determines whether this instance can render the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && duplicationResource != null;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            context.RenderHost.RenderConfiguration = config;
            FrameData data;
            bool isTimeOut;
            if (clearTarget)
            {
                clearTarget = false;
                context.RenderHost.ClearRenderTarget(deviceContext, true, false);
            }

            if (duplicationResource.GetFrame(Output, out data, out isTimeOut))
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
        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref ScreenDuplicationModelStruct model, IRenderContext context)
        {
            DuplicationInfo info;
            if (!duplicationResource.TryGetInfo(Output, out info))
            {
                return;
            }
            int width = Math.Abs(info.OutputDesc.DesktopBounds.Right - info.OutputDesc.DesktopBounds.Left);
            int height = Math.Abs(info.OutputDesc.DesktopBounds.Bottom - info.OutputDesc.DesktopBounds.Top);
            var texBound = GetTextureBound(width, height);
            var verBound = GetVertexBound((int)context.ActualWidth, (int)context.ActualHeight, width, height);
            model.TopLeft = new Vector4(verBound.X, verBound.Z, 0, 1);
            model.TopRight = new Vector4(verBound.Y, verBound.Z, 0, 1);
            model.BottomLeft = new Vector4(verBound.X, verBound.W, 0, 1);
            model.BottomRight = new Vector4(verBound.Y, verBound.W, 0, 1);
            model.TexTopLeft = new Vector2(texBound.X, texBound.Z);
            model.TexTopRight = new Vector2(texBound.Y, texBound.Z);
            model.TexBottomLeft = new Vector2(texBound.X, texBound.W);
            model.TexBottomRight = new Vector2(texBound.Y, texBound.W);
        }
        /// <summary>
        /// Gets the texture bound. Ouput: x(left), y(right), z(top), w(bottom)
        /// </summary>
        /// <param name="screenWidth">Width of the screen.</param>
        /// <param name="screenHeight">Height of the screen.</param>
        /// <returns></returns>
        protected virtual Vector4 GetTextureBound(int screenWidth, int screenHeight)
        {
            if(cloneRectangle.Width == 0 || cloneRectangle.Height == 0)
            {
                return new Vector4(0, 1, 0, 1);
            }
            var bound = new Vector4();
            bound.X = (float)cloneRectangle.Left / screenWidth;
            bound.Y = (float)cloneRectangle.Right / screenWidth;
            bound.Z = (float)cloneRectangle.Top / screenHeight;
            bound.W = (float)cloneRectangle.Bottom / screenHeight;
            return bound;
        }
        /// <summary>
        /// Return Quad NDC coordinate for vertex. Ouput: x(left), y(right), z(top), w(bottom)
        /// </summary>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        /// <param name="deskWidth"></param>
        /// <param name="deskHeight"></param>
        /// <returns></returns>
        protected virtual Vector4 GetVertexBound(int viewportWidth, int viewportHeight, int deskWidth, int deskHeight)
        {
            int cloneWidth = cloneRectangle.Width;
            int cloneHeight = cloneRectangle.Height;
            if ( cloneWidth == 0 || cloneHeight == 0)
            {
                cloneWidth = deskWidth;
                cloneHeight = deskHeight;
            }
            if (StretchToFill)
            {
                cloneWidth = viewportWidth;
                cloneHeight = viewportHeight;
            }
            var bound = new Vector4();
            var viewportRatio = (float)viewportWidth / viewportHeight;
            var cloneRatio = (float)cloneWidth / cloneHeight;
            if (viewportRatio >= cloneRatio)
            {
                var ndcCloneW = (2.0f * cloneRatio) / viewportRatio;
                bound.X = -1 + (2 - ndcCloneW)/2.0f;
                bound.Y = bound.X + ndcCloneW;
                bound.Z = 1;
                bound.W = -1;
            }
            else
            {
                var ndcCloneH = viewportRatio * 2.0f / cloneRatio;
                bound.X = -1;
                bound.Y = 1;
                bound.Z = 1 - (2 - ndcCloneH) / 2.0f;
                bound.W = bound.Z - ndcCloneH;
            }
            return bound;
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            clearTarget = true;
            base.OnDetach();
        }

        private sealed class DuplicationInfo
        {
            public readonly OutputDescription OutputDesc;
            public readonly OutputDuplication Duplication;
            public DuplicationInfo(OutputDescription desc, OutputDuplication duplication)
            {
                OutputDesc = desc;
                Duplication = duplication;
            }
        }

        private class DuplicationResource : DisposeObject
        {
            private readonly Dictionary<int, DuplicationInfo> duplicationDict = new Dictionary<int, DuplicationInfo>();
            private bool IsInitialized = false;

            private readonly global::SharpDX.Direct3D11.Device device;

            private int metaDataSize = 0;
            private OutputDuplicateMoveRectangle[] moveBuffer = null;
            private RawRectangle[] dirtyBuffer = null;

            private readonly Stack<OutputDuplication> currentDuplicationStack = new Stack<OutputDuplication>();
            private readonly Stack<Texture2D> currentDuplicationTexture = new Stack<Texture2D>();

            public DuplicationResource(global::SharpDX.Direct3D11.Device device)
            {
                this.device = device;
            }

            public bool Initialize()
            {
                var list = new List<DuplicationInfo>();
                using (var dxgDevice = device.QueryInterface<Device>())
                {
                    using (var adapter = dxgDevice.GetParent<Adapter>())
                    {
                        int outputCount = adapter.GetOutputCount();
                        for(int i=0; i<outputCount; ++i)
                        {
                            using (var output = adapter.GetOutput(i))
                            {
                                using (var output1 = output.QueryInterface<Output1>())
                                {
                                    var duplication = Collect(output1.DuplicateOutput(device));
                                    list.Add(new DuplicationInfo(output.Description, duplication));
                                }
                            }
                        }
                    }
                }
                int index = 0;
                foreach(var dup in list.OrderBy(x=>x.OutputDesc.DesktopBounds.Left))
                {
                    duplicationDict.Add(index++, dup);
                }
                IsInitialized = true;
                return true;
            }
            

            public bool GetFrame(int outputIndex, out FrameData data, out bool timeOut)
            {
                data = new FrameData();
                if (!IsInitialized)
                {
                    timeOut = false;
                    return false;
                }
                DuplicationInfo info;
                if(!duplicationDict.TryGetValue(outputIndex, out info))
                {
                    timeOut = false;
                    return false;
                }
                OutputDuplicateFrameInformation frameInfo;
                Resource desktopResource;
                try
                {
                    info.Duplication.AcquireNextFrame(500, out frameInfo, out desktopResource);
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
                currentDuplicationStack.Push(info.Duplication);
                var texture2D = desktopResource.QueryInterface<Texture2D>();
                currentDuplicationTexture.Push(texture2D);
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
                    info.Duplication.GetFrameMoveRects(metaDataSize, moveBuffer, out moveRectSize);
                    data.MoveRectangles = moveBuffer;
                    data.MoveCount = moveRectSize / Marshal.SizeOf(typeof(OutputDuplicateMoveRectangle));

                    if (dirtyBuffer == null || dirtyBuffer.Length < metaDataSize)
                    {
                        dirtyBuffer = new RawRectangle[metaDataSize];
                    }
                    int dirtyRectSize;
                    info.Duplication.GetFrameDirtyRects(metaDataSize, dirtyBuffer, out dirtyRectSize);
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
                while (currentDuplicationTexture.Count > 0)
                {
                    currentDuplicationTexture.Pop().Dispose();
                }
                while (currentDuplicationStack.Count > 0)
                {
                    currentDuplicationStack.Pop().ReleaseFrame();
                }
            }

            protected override void Dispose(bool disposeManagedResources)
            {
                ReleaseFrame();
                duplicationDict.Clear();
                base.Dispose(disposeManagedResources);
            }

            public bool TryGetInfo(int output, out DuplicationInfo info)
            {
                return duplicationDict.TryGetValue(output, out info);
            }
        }

        private struct FrameData
        {
            public Texture2D Frame;
            public OutputDuplicateFrameInformation FrameInfo;
            public RawRectangle[] DirtyRectangles;
            public OutputDuplicateMoveRectangle[] MoveRectangles;
            public int DirtyCount;
            public int MoveCount;
        }
    }
}
#endif
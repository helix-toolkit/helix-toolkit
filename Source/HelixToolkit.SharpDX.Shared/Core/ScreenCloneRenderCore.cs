/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define OUTPUTDETAIL
using HelixToolkit.Mathematics;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using BindFlags = SharpDX.Direct3D11.BindFlags;
using CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using ShaderResourceViewDescription = SharpDX.Direct3D11.ShaderResourceViewDescription;
using Texture2D = SharpDX.Direct3D11.Texture2D;
using Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
{
    using Render;
    using Shaders;
    using Utilities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using Components;

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
        /// <summary>
        /// Gets or sets a value indicating whether [show mouse cursor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
        /// </value>
        bool ShowMouseCursor { set; get; }
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
                if(Set(ref output, value))
                {
                    invalidRender = true;
                }
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
                    invalidRender = true;
                    clearTarget = true;
                }
            }
            get { return cloneRectangle; }
        }

        private bool stretchToFill = false;
        /// <summary>
        /// Gets or sets a value indicating cloned rectangle is stretched during rendering, default is false;
        /// </summary>
        /// <value>
        ///   <c>true</c> if stretch; otherwise, <c>false</c>.
        /// </value>
        public bool StretchToFill
        {
            set
            {
                if(Set(ref stretchToFill, value))
                {
                    invalidRender = true;
                    if (!value)
                    {
                        clearTarget = true;
                    }
                }
            }
            get { return stretchToFill; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show mouse cursor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMouseCursor
        {
            set; get;
        } = true;
        private DuplicationResource duplicationResource;
        private FrameProcessing frameProcessor;
        private ShaderPass DefaultShaderPass;
        private ShaderPass CursorShaderPass;
        private int textureBindSlot = 0;
        private int samplerBindSlot = 0;
        private SamplerStateProxy textureSampler;
        private bool clearTarget = true;
        private bool invalidRender = true;
        private PointerInfo pointer = new PointerInfo();
        private readonly ConstantBufferComponent modelCB;

        private DX11RenderHostConfiguration config = new DX11RenderHostConfiguration() { ClearEachFrame = false, RenderD2D = false, RenderLights = false, UpdatePerFrameData = false };
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenCloneRenderCore"/> class.
        /// </summary>
        public ScreenCloneRenderCore() : base(RenderType.Opaque)
        { 
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.ScreenDuplicationCB, ScreenDuplicationModelStruct.SizeInBytes)));
        }

        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="technique">The technique.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            DefaultShaderPass = technique.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication][DefaultPassNames.Default];
            CursorShaderPass = technique.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication][DefaultPassNames.ScreenQuad];
            textureBindSlot = DefaultShaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
            samplerBindSlot = DefaultShaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
            textureSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.ScreenDupSampler));
            return Initialize(technique.EffectsManager);
        }

        private bool Initialize(IEffectsManager manager)
        {
            RemoveAndDispose(ref duplicationResource);
            duplicationResource = Collect(new DuplicationResource(manager.Device));
            frameProcessor = Collect(new FrameProcessing());
            return true;
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && duplicationResource != null;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            bool succ = duplicationResource.Initialize();
            if (!succ)
            {
                InvalidateRenderer();
                return;
            }
            context.RenderHost.RenderConfiguration = config;
            FrameData data;
            
            bool isTimeOut;
            bool accessLost;


            if (duplicationResource.GetFrame(Output, out data, ref pointer, out isTimeOut, out accessLost))
            {              
                if (data.FrameInfo.TotalMetadataBufferSize > 0)
                {
                    frameProcessor.ProcessFrame(ref data, deviceContext);
                }
                invalidRender = true;
            }

            if (frameProcessor.SharedTexture != null && !accessLost)
            {
                if (clearTarget)
                {
                    clearTarget = false;
                    context.RenderHost.ClearRenderTarget(deviceContext, true, false);
                }
                bool cursorValid = false;
                if (pointer.Visible)
                {
                    Vector4 rect;
                    if(frameProcessor.ProcessCursor(ref pointer, deviceContext, out rect))
                    {
                        GetCursorVertexBound((int)context.ActualWidth, (int)context.ActualHeight, frameProcessor.TextureWidth, frameProcessor.TextureHeight, ref rect);
                        invalidRender = true;
                        cursorValid = true;
                    }
                }
                if (invalidRender)
                {
                    modelCB.Upload(deviceContext, ref modelStruct);
                    DefaultShaderPass.BindShader(deviceContext);
                    DefaultShaderPass.BindStates(deviceContext,StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
                    DefaultShaderPass.PixelShader.BindSampler(deviceContext, samplerBindSlot, textureSampler);
                    int left = (int)(context.ActualWidth * Math.Abs(modelStruct.TopLeft.X + 1) / 2);
                    int top = (int)(context.ActualHeight * Math.Abs(modelStruct.TopLeft.Y - 1) / 2);
                    deviceContext.SetScissorRectangle(left, top, (int)context.ActualWidth - left, (int)context.ActualHeight - top);
                    using (var textureView = new global::SharpDX.Direct3D11.ShaderResourceView(deviceContext, frameProcessor.SharedTexture))
                    {
                        deviceContext.SetShaderResource(PixelShader.Type, textureBindSlot, textureView);                       
                        deviceContext.Draw(4, 0);
                    }
                    if (ShowMouseCursor && cursorValid)
                    {                    
                        DrawCursor(ref pointer, deviceContext);
                    }
                    invalidRender = false;
                }
            }
            if (isTimeOut)
            {

            }
            else if (accessLost)
            {
                throw new SharpDXException(ResultCode.AccessLost);
            }
            else
            {
                duplicationResource.ReleaseFrame();
            }
            InvalidateRenderer();
        }

        #region Draw Cursor



        private void DrawCursor(ref PointerInfo pointer, DeviceContextProxy deviceContext)
        {
            if (!pointer.Visible || frameProcessor.PointerResource == null)
            {
                return;
            }
            CursorShaderPass.PixelShader.BindTexture(deviceContext, textureBindSlot, frameProcessor.PointerResource);
            CursorShaderPass.BindShader(deviceContext);
            CursorShaderPass.BindStates(deviceContext, StateType.DepthStencilState | StateType.RasterState | StateType.BlendState);
            //deviceContext.DeviceContext.OutputMerger.SetBlendState(CursorShaderPass.BlendState, new RawColor4(0, 0, 0, 0)); //Set special blend factor
            deviceContext.Draw(4, 0);
        }

        #endregion
        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref ScreenDuplicationModelStruct model, RenderContext context)
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
            if (cloneWidth == 0 || cloneHeight == 0)
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

        protected virtual void GetCursorVertexBound(int viewportWidth, int viewportHeight, int deskWidth, int deskHeight, ref Vector4 rect)
        {
            int cloneWidth = cloneRectangle.Width;
            int cloneHeight = cloneRectangle.Height;
            if (cloneWidth == 0 || cloneHeight == 0)
            {
                cloneWidth = deskWidth;
                cloneHeight = deskHeight;
            }

            var centerX = cloneRectangle.Width != 0 ? (cloneRectangle.Left + cloneWidth / 2) : cloneWidth / 2;
            var centerY = cloneRectangle.Height != 0 ? (cloneRectangle.Top + cloneHeight / 2) : cloneHeight / 2;
            var viewportCenterX = viewportWidth / 2;
            var viewportCenterY = viewportHeight / 2;

            var offX = viewportCenterX - centerX;
            var offY = viewportCenterY - centerY;

            var viewportRatio = (float)viewportWidth / viewportHeight;
            var cloneRatio = (float)cloneWidth / cloneHeight;
            var scaleX = 1f;
            var scaleY = 1f;

            if (StretchToFill)
            {
                scaleX = (float)viewportWidth / cloneWidth;
                scaleY = (float)viewportHeight / cloneHeight;
            }
            else
            {
                if (viewportRatio >= cloneRatio)
                {
                    scaleX = scaleY = (float)viewportHeight / cloneHeight;
                }
                else
                {
                    scaleX = scaleY = (float)viewportWidth / cloneWidth;
                }
            }


            modelStruct.CursorTopRight.X = ((rect.X + rect.Z) - viewportCenterX + offX) / viewportCenterX * scaleX;
            modelStruct.CursorTopRight.Y = -1 * (rect.Y - viewportCenterY + offY) / viewportCenterY * scaleY;
            modelStruct.CursorTopRight.W = 1;

            modelStruct.CursorTopLeft.X = (rect.X - viewportCenterX + offX) / viewportCenterX * scaleX;
            modelStruct.CursorTopLeft.Y = -1 * (rect.Y - viewportCenterY + offY) / viewportCenterY * scaleY;
            modelStruct.CursorTopLeft.W = 1;

            modelStruct.CursorBottomRight.X = ((rect.X + rect.Z) - viewportCenterX + offX) / viewportCenterX * scaleX;
            modelStruct.CursorBottomRight.Y = -1 * ((rect.Y + rect.W) - viewportCenterY + offY) / viewportCenterY * scaleY;
            modelStruct.CursorBottomRight.W = 1;

            modelStruct.CursorBottomLeft.X = (rect.X - viewportCenterX + offX) / viewportCenterX * scaleX;
            modelStruct.CursorBottomLeft.Y = -1 * ((rect.Y + rect.W) - viewportCenterY + offY) / viewportCenterY * scaleY;
            modelStruct.CursorBottomLeft.W = 1;
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            clearTarget = true;
            invalidRender = true;
            pointer = new PointerInfo();
            textureSampler = null;
            base.OnDetach();
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
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

        private sealed class FrameProcessing : DisposeObject
        {
            private Texture2D sharedTexture;
            private Texture2DDescription sharedDescription;
            public Texture2D SharedTexture { get { return sharedTexture; } }

            public int TextureWidth { get { return sharedDescription.Width; } }
            public int TextureHeight { get { return sharedDescription.Height; } }

            public void ProcessFrame(ref FrameData data, DeviceContextProxy context)
            {
                if (sharedTexture == null || sharedDescription.Width != data.Frame.Description.Width || sharedDescription.Height != data.Frame.Description.Height)
                {
                    RemoveAndDispose(ref sharedTexture);
                    sharedDescription = new Texture2DDescription()
                    {
                        BindFlags = BindFlags.ShaderResource,
                        CpuAccessFlags = CpuAccessFlags.None, Format = data.Frame.Description.Format, Width = data.Frame.Description.Width, Height = data.Frame.Description.Height,
                        MipLevels = 1, Usage = ResourceUsage.Default, SampleDescription = new SampleDescription(1, 0), OptionFlags = ResourceOptionFlags.None, ArraySize=1
                    };
                    sharedTexture = Collect(new Texture2D(context, sharedDescription));
                }
                context.CopyResource(data.Frame, sharedTexture);
            }

            private const int BPP = 4;

            public ShaderResourceViewProxy PointerResource { get { return pointerResource; } }
            private ShaderResourceViewProxy pointerResource;

            private Texture2DDescription pointerTexDesc = new Texture2DDescription()
            {
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write
            };

            private Texture2D copyBuffer;
            private Texture2DDescription stageTextureDesc = new Texture2DDescription()
            {
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None,
                BindFlags = BindFlags.None
            };

            private ShaderResourceViewDescription pointerSRVDesc = new ShaderResourceViewDescription()
            {
                Format = Format.B8G8R8A8_UNorm,
                Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = 1, MostDetailedMip = 0 }
            };

            byte[] initBuffer = new byte[0];
            private int currentType = 0;

            public bool ProcessCursor(ref PointerInfo pointer, DeviceContextProxy context, out Vector4 rect)
            {               
                
                int width = 0;
                int height = 0;
                int left = pointer.Position.X;
                int top = pointer.Position.Y;
                
                switch (pointer.ShapeInfo.Type)
                {
                    case (int)OutputDuplicatePointerShapeType.Color:
                        width = pointer.ShapeInfo.Width;
                        height = pointer.ShapeInfo.Height;
                        break;
                    case (int)OutputDuplicatePointerShapeType.Monochrome:
                        ProcessMonoMask(context, true, ref pointer, out width, out height, out left, out top);
                        break;
                    case (int)OutputDuplicatePointerShapeType.MaskedColor:
                        ProcessMonoMask(context, false, ref pointer, out width, out height, out left, out top);
                        break;
                    default:
                        rect = Vector4.Zero; //Invalid cursor type
                        return false;
                }

                rect = new Vector4(pointer.Position.X, pointer.Position.Y, width, height);
                int rowPitch = pointer.ShapeInfo.Type == (int)OutputDuplicatePointerShapeType.Color ? pointer.ShapeInfo.Pitch : width * BPP;
                int slicePitch = 0;

                if (pointerResource == null || currentType != pointer.ShapeInfo.Type
                    || pointerTexDesc.Width != width || pointerTexDesc.Height != height)
                {
                    RemoveAndDispose(ref pointerResource);
                    pointerTexDesc.Width = width;
                    pointerTexDesc.Height = height;
                    currentType = pointer.ShapeInfo.Type;
                    
                    global::SharpDX.Utilities.Pin(pointer.ShapeInfo.Type == (int)OutputDuplicatePointerShapeType.Color ? pointer.PtrShapeBuffer : initBuffer, ptr =>
                    {
                        pointerResource = Collect(new ShaderResourceViewProxy(context,
                            new Texture2D(context, pointerTexDesc, new[] { new DataBox(ptr, rowPitch, slicePitch) })));
                    });
                    pointerResource.CreateView(pointerSRVDesc);
#if OUTPUTDETAIL
                    Console.WriteLine("Create new cursor texture. Type = " + pointer.ShapeInfo.Type);
#endif
                }
                else
                {
                    var dataBox = context.MapSubresource(pointerResource.Resource, 0, global::SharpDX.Direct3D11.MapMode.WriteDiscard, 
                        global::SharpDX.Direct3D11.MapFlags.None);
                    if (pointer.ShapeInfo.Type == (int)OutputDuplicatePointerShapeType.Color)
                    {
#if OUTPUTDETAIL
                        Console.WriteLine("Reuse existing cursor texture for Color.");
#endif
                        unsafe
                        {
                            int row = pointer.ShapeInfo.Height;
                            int sourceCounter = 0;
                            byte* target32 = (byte*)dataBox.DataPointer;
                            for (int i = 0; i < row; ++i)
                            {
                                int targetCounter = i * dataBox.RowPitch;
                                for (int j = 0; j < pointer.ShapeInfo.Pitch; ++j)
                                {
                                    target32[targetCounter++] = pointer.PtrShapeBuffer[sourceCounter++];
                                }
                            }
                        }
                    }
                    else
                    {
#if OUTPUTDETAIL
                        Console.WriteLine("Reuse existing cursor texture for Mono and Mask.");
#endif
                        unsafe // Call unmanaged code
                        {
                            byte* target32 = (byte*)dataBox.DataPointer;
                            for(int i = 0; i < initBuffer.Length; ++i)
                            {
                                target32[i] = initBuffer[i];
                            }
                        }
                    }
                    context.UnmapSubresource(pointerResource.Resource, 0);
                }
                return true;
            }           

            private void ProcessMonoMask(DeviceContextProxy context,
                bool isMono, ref PointerInfo info, out int width, out int height, out int left, out int top)
            {
                int deskWidth = sharedDescription.Width;
                int deskHeight = sharedDescription.Height;
                int givenLeft = info.Position.X;
                int givenTop = info.Position.Y;
                if (givenLeft < 0)
                {
                    width = givenLeft + info.ShapeInfo.Width;
                }
                else if (givenLeft + info.ShapeInfo.Width > deskWidth)
                {
                    width = deskWidth - givenLeft;
                }
                else
                {
                    width = info.ShapeInfo.Width;
                }

                if (isMono)
                {
                    info.ShapeInfo.Height /= 2;
                }

                if (givenTop < 0)
                {
                    height = givenTop + info.ShapeInfo.Height;
                }
                else if(givenTop + info.ShapeInfo.Height > deskHeight)
                {
                    height = deskHeight - givenTop;
                }
                else
                {
                    height = info.ShapeInfo.Height;
                }

                if (isMono)
                {
                    info.ShapeInfo.Height *= 2;
                }

                left = givenLeft < 0 ? 0 : givenLeft;
                top = givenTop < 0 ? 0 : givenTop;
                stageTextureDesc.Width = width;
                stageTextureDesc.Height = height;
                if (initBuffer.Length != width * height * BPP)
                {
                    initBuffer = new byte[width * height * BPP];
                }

                if(copyBuffer == null || stageTextureDesc.Width != width || stageTextureDesc.Height != height)
                {
                    RemoveAndDispose(ref copyBuffer);
                    copyBuffer = Collect(new Texture2D(context, stageTextureDesc));
                }

                context.CopySubresourceRegion(SharedTexture, 0,
                    new global::SharpDX.Direct3D11.ResourceRegion(left, top, 0, left + width, top + height, 1), copyBuffer, 0);

                var dataBox = context.MapSubresource(copyBuffer, 0, global::SharpDX.Direct3D11.MapMode.Read, global::SharpDX.Direct3D11.MapFlags.None);
#region process
                unsafe // Call unmanaged code
                {
                    fixed(byte* initBufferPtr = initBuffer)
                    {                                                         
                        uint* initBuffer32 = (uint*)initBufferPtr;
                        uint* desktop32 = (uint*)dataBox.DataPointer;
                        int desktopPitchInPixels = dataBox.RowPitch / sizeof(int);
                        uint skipX = (givenLeft < 0) ? (uint)(-1 * givenLeft) : 0;
                        uint skipY = (givenTop < 0) ? (uint)(-1 * givenTop) : 0;
                        if (isMono)
                        {
                            for (int row = 0; row < stageTextureDesc.Height; ++row)
                            {
                                // Set mask
                                byte Mask = 0x80;
                                Mask = (byte)(Mask >> (byte)(skipX % 8));
                                for (int col = 0; col < stageTextureDesc.Width; ++col)
                                {
                                    // Get masks using appropriate offsets
                                    byte AndMask = (byte)(info.PtrShapeBuffer[((col + skipX) / 8) + ((row + skipY) * (info.ShapeInfo.Pitch))] & Mask);
                                    byte XorMask = (byte)(info.PtrShapeBuffer[((col + skipX) / 8) + ((row + skipY + (info.ShapeInfo.Height / 2)) 
                                        * (info.ShapeInfo.Pitch))] & Mask);
                                    uint AndMask32 = (AndMask > 0) ? 0xFFFFFFFF : 0xFF000000;
                                    uint XorMask32 = (XorMask > 0) ? (uint)0x00FFFFFF : 0x00000000;

                                    // Set new pixel
                                    initBuffer32[(row * stageTextureDesc.Width) + col] = (desktop32[(row * desktopPitchInPixels) + col] & AndMask32) ^ XorMask32;

                                    // Adjust mask
                                    if (Mask == 0x01)
                                    {
                                        Mask = 0x80;
                                    }
                                    else
                                    {
                                        Mask = (byte)(Mask >> 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            fixed(byte* shapeBufferPtr = info.PtrShapeBuffer)
                            {
                                uint* Buffer32 = (uint*)shapeBufferPtr;

                                // Iterate through pixels
                                for (int row = 0; row < stageTextureDesc.Height; ++row)
                                {
                                    for (int col = 0; col < stageTextureDesc.Width; ++col)
                                    {
                                        // Set up mask
                                        uint MaskVal = 0xFF000000 & Buffer32[(col + skipX) + ((row + skipY) * (info.ShapeInfo.Pitch / sizeof(uint)))];
                                        if (MaskVal > 0)
                                        {
                                            // Mask was 0xFF
                                            initBuffer32[(row * stageTextureDesc.Width) + col] = (desktop32[(row * desktopPitchInPixels) + col] 
                                                ^ Buffer32[(col + skipX) + ((row + skipY) * (info.ShapeInfo.Pitch / sizeof(uint)))]) | 0xFF000000;
                                        }
                                        else
                                        {
                                            // Mask was 0x00
                                            initBuffer32[(row * stageTextureDesc.Width) + col] 
                                                = Buffer32[(col + skipX) + ((row + skipY) * (info.ShapeInfo.Pitch / sizeof(uint)))] | 0xFF000000;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
#endregion
                context.UnmapSubresource(copyBuffer, 0);
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

            private int getFrameTimeOut = 5;

            public DuplicationResource(global::SharpDX.Direct3D11.Device device)
            {
                this.device = device;
            }

            public bool Initialize()
            {
                if (IsInitialized)
                { return true; }
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
                                    try
                                    {
                                        var duplication = Collect(output1.DuplicateOutput(device));
                                        list.Add(new DuplicationInfo(output.Description, duplication));
                                    }
                                    catch(SharpDXException ex)
                                    {
                                        if (ex.ResultCode.Code == global::SharpDX.Result.AccessDenied.Code)
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            throw new SharpDXException(ex.ResultCode);
                                        }
                                    }
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
            

            public bool GetFrame(int outputIndex, out FrameData data, ref PointerInfo pointer, out bool timeOut, out bool accessLost)
            {
                accessLost = false;
                timeOut = false;
                data = new FrameData();
                if (!IsInitialized)
                {
                    return false;
                }
                DuplicationInfo info;
                if(!duplicationDict.TryGetValue(outputIndex, out info))
                {
                    return false;
                }
                OutputDuplicateFrameInformation frameInfo;
                Resource desktopResource;
                var code = info.Duplication.TryAcquireNextFrame(getFrameTimeOut, out frameInfo, out desktopResource);
                if (!code.Success)
                {
                    if (code.Code == ResultCode.WaitTimeout.Result.Code)
                    {
                        timeOut = true;
                        return false;
                    }
                    else if (code.Code == ResultCode.AccessLost.Code)
                    {
                        accessLost = true;
                        return false;
                    }
                    else
                    {
#if DEBUG
                        throw new HelixToolkitException("Failed to acquire next frame.");
#endif
                    }
                }
                else
                {
                    RetrieveCursorMetadata(ref frameInfo, info.Duplication, outputIndex, ref pointer);
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

            private void RetrieveCursorMetadata(ref OutputDuplicateFrameInformation frameInfo, OutputDuplication duplication, int outputDevice, ref PointerInfo pointerInfo)
            {
                // A non-zero mouse update timestamp indicates that there is a mouse position update and optionally a shape change
                if (frameInfo.LastMouseUpdateTime == 0)
                    return;

                bool updatePosition = true;

                // Make sure we don't update pointer position wrongly
                // If pointer is invisible, make sure we did not get an update from another output that the last time that said pointer
                // was visible, if so, don't set it to invisible or update.

                if (!frameInfo.PointerPosition.Visible && (pointerInfo.WhoUpdatedPositionLast != outputDevice))
                    updatePosition = false;

                // If two outputs both say they have a visible, only update if new update has newer timestamp
                if (frameInfo.PointerPosition.Visible && pointerInfo.Visible && (pointerInfo.WhoUpdatedPositionLast != outputDevice) && (pointerInfo.LastTimeStamp > frameInfo.LastMouseUpdateTime))
                    updatePosition = false;

                // Update position
                if (updatePosition)
                {
                    pointerInfo.Position = new Point(frameInfo.PointerPosition.Position.X, frameInfo.PointerPosition.Position.Y);
                    pointerInfo.WhoUpdatedPositionLast = outputDevice;
                    pointerInfo.LastTimeStamp = frameInfo.LastMouseUpdateTime;
                    pointerInfo.Visible = frameInfo.PointerPosition.Visible;
                }

                // No new shape
                if (frameInfo.PointerShapeBufferSize == 0)
                    return;

                if (frameInfo.PointerShapeBufferSize > pointerInfo.BufferSize)
                {
                    pointerInfo.PtrShapeBuffer = new byte[frameInfo.PointerShapeBufferSize];
                    pointerInfo.BufferSize = frameInfo.PointerShapeBufferSize;
                }

                try
                {
                    unsafe
                    {
                        fixed (byte* ptrShapeBufferPtr = pointerInfo.PtrShapeBuffer)
                        {
                            OutputDuplicatePointerShapeInformation info;                           
                            duplication.GetFramePointerShape(frameInfo.PointerShapeBufferSize, (IntPtr)ptrShapeBufferPtr, out pointerInfo.BufferSize, out info);
                            if(info.Type != 0)
                            {
                                pointerInfo.ShapeInfo = info;
                            }
                        }
                    }
                }
                catch (SharpDXException ex)
                {
                    if (ex.ResultCode.Failure)
                    {
                        throw new HelixToolkitException("Failed to get frame pointer shape.");
                    }
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

            protected override void OnDispose(bool disposeManagedResources)
            {
                while (currentDuplicationTexture.Count > 0)
                {
                    currentDuplicationTexture.Pop().Dispose();
                }
                duplicationDict.Clear();
                base.OnDispose(disposeManagedResources);
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

        private struct PointerInfo
        {
            public byte[] PtrShapeBuffer;
            public OutputDuplicatePointerShapeInformation ShapeInfo;
            public Point Position;
            public bool Visible;
            public int BufferSize;
            public int WhoUpdatedPositionLast;
            public long LastTimeStamp;
        }
    }
}
#endif
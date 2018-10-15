using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using System.Linq;
using System.IO;
using SharpDX;

namespace HelixToolkit.SharpDX.Core.Model
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using UWP.Core;   
    using UWP;
    using UWP.Model.Scene;
    using UWP.Utilities;
    using UWP.Render;
    using System.Runtime.InteropServices;
    using HelixToolkit.UWP.Shaders;
    using global::SharpDX.DXGI;

    public class ImGuiNode : SceneNode
    {
        #region Custom Render Technique
        public const string ImGuiRenderTechnique = "ImGuiRender";
        public static InputElement[] VSInputImGui2D { get; } = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
            new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm,  InputElement.AppendAligned, 0),
        };

        public static readonly TechniqueDescription RenderTechnique = new TechniqueDescription(ImGuiRenderTechnique)
        {
            InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSSprite2D,
                VSInputImGui2D),
            PassDescriptions = new[]
            {
                new ShaderPassDescription(DefaultPassNames.Default)
                {
                    ShaderList = new[]
                    {
                        DefaultVSShaderDescriptions.VSSprite2D,
                        DefaultPSShaderDescriptions.PSSprite2D,
                    },
                    Topology = PrimitiveTopology.TriangleStrip,
                    BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                    DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                    RasterStateDescription = DefaultRasterDescriptions.RSSpriteCW,
                }
            }
        };
        #endregion
        private ImGui2DBufferModel bufferModel;

        protected override RenderCore OnCreateRenderCore()
        {
            return new Sprite2DRenderCore();
        }

        protected override void OnDetach()
        {
            bufferModel = null;
            (RenderCore as Sprite2DRenderCore).TextureView = null;
            base.OnDetach();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[ImGuiRenderTechnique];
        }

        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && bufferModel.SpriteCount != 0 && bufferModel.IndexCount != 0;
        }

        protected override bool CanHitTest(RenderContext context)
        {
            return false;
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }

        protected override void OnAttached()
        {
            bufferModel = Collect(new ImGui2DBufferModel());
            bufferModel.Sprites = new DrawVert[1024];
            bufferModel.Indices = new ushort[2048];
            (RenderCore as Sprite2DRenderCore).Buffer = bufferModel;
            var io = ImGui.GetIO();
            
            var textureData = io.FontAtlas.GetTexDataAsRGBA32();
            var stream = new byte[textureData.BytesPerPixel * textureData.Width * textureData.Height];

            
            unsafe
            {
                var ptr = textureData.Pixels;
                for(int i=0; i < stream.Length; ++i)
                {
                    stream[i] = *ptr++;
                }
       
            }
            var textureView = Collect(new ShaderResourceViewProxy(EffectsManager.Device));
            textureView.CreateView(stream, textureData.Width, textureData.Height,
                global::SharpDX.DXGI.Format.R8G8B8A8_UNorm);    
            io.FontAtlas.ClearTexData();
            (RenderCore as Sprite2DRenderCore).TextureView = textureView;
            base.OnAttached();
        }

        public override void Update(RenderContext context)
        {
            base.Update(context);
            ImGui.GetIO().DisplaySize = new System.Numerics.Vector2((float)context.ActualWidth, (float)context.ActualHeight);
            ImGui.NewFrame();
            ImGui.Text("Test ImGui");
            ImGui.Render();
            unsafe
            {
                var data = ImGui.GetDrawData();
                if (data->CmdListsCount == 0)
                {
                    return;
                }
                bufferModel.SpriteCount = data->TotalVtxCount;
                if(bufferModel.Sprites.Length < bufferModel.SpriteCount)
                {
                    bufferModel.Sprites = new DrawVert[bufferModel.SpriteCount];
                }


                bufferModel.IndexCount = data->TotalIdxCount;
                if (bufferModel.Indices.Length < bufferModel.IndexCount)
                {
                    bufferModel.Indices = new ushort[bufferModel.IndexCount];
                }
                
                var vertArray = bufferModel.Sprites;
                var idxArray = bufferModel.Indices;
                int vertSize = bufferModel.SpriteCount * sizeof(DrawVert);
                int idxSize = bufferModel.IndexCount * sizeof(ushort);
                fixed(void* vertPtr = vertArray)
                {
                    byte* vPtr = (byte*)vertPtr;
                    fixed(void* idxPtr = idxArray)
                    {
                        byte* iPtr = (byte*)idxPtr;
                        for (int i = 0; i < data->CmdListsCount; i++)
                        {
                            NativeDrawList* cmd_list = data->CmdLists[i];
                            System.Buffer.MemoryCopy(cmd_list->VtxBuffer.Data, vPtr, 
                                vertSize, cmd_list->VtxBuffer.Size * sizeof(DrawVert));
                            vPtr += cmd_list->VtxBuffer.Size * sizeof(DrawVert);

                            System.Buffer.MemoryCopy(cmd_list->IdxBuffer.Data, iPtr,
                                idxSize, cmd_list->IdxBuffer.Size * sizeof(ushort));
                            vertSize -= cmd_list->VtxBuffer.Size * sizeof(DrawVert);
                            idxSize -= cmd_list->IdxBuffer.Size * sizeof(ushort);
                        }
                    }
                }

                IO io = ImGui.GetIO();

                (RenderCore as Sprite2DRenderCore).ProjectionMatrix = Matrix.OrthoOffCenterLH(
                    0f,
                    io.DisplaySize.X,
                    io.DisplaySize.Y,
                    0.0f,
                    -1.0f,
                    1.0f);
            }

        }
    }

    
    public sealed class ImGui2DBufferModel : ReferenceCountDisposeObject, IGUID, IAttachableBufferModel
    {
        public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.TriangleList;

        public IElementsBufferProxy[] VertexBuffer { get; } = new DynamicBufferProxy[1];

        public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }

        public IElementsBufferProxy IndexBuffer { get; }

        public Guid GUID { get; } = Guid.NewGuid();

        public DrawVert[] Sprites;
        public int SpriteCount;

        public ushort[] Indices;
        public int IndexCount;

        private readonly DynamicBufferProxy vertextBuffer;

        public ImGui2DBufferModel()
        {
            vertextBuffer = Collect(new DynamicBufferProxy(global::SharpDX.Utilities.SizeOf<DrawVert>(), BindFlags.VertexBuffer));
            VertexBuffer[0] = vertextBuffer;
            IndexBuffer = Collect(new DynamicBufferProxy(sizeof(ushort), BindFlags.IndexBuffer));
        }

        public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
        {
            if (UpdateBuffers(context, deviceResources))
            {
                context.SetVertexBuffers(0, new VertexBufferBinding(vertextBuffer.Buffer, vertextBuffer.StructureSize, vertextBuffer.Offset));
                context.SetIndexBuffer(IndexBuffer.Buffer, global::SharpDX.DXGI.Format.R16_UInt, IndexBuffer.Offset);
                return true;
            }
            return false;
        }

        public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
        {
            if (SpriteCount == 0 || IndexCount == 0 || Sprites == null || Indices == null || Sprites.Length < SpriteCount || Indices.Length < IndexCount)
            {
                return false;
            }
            vertextBuffer.UploadDataToBuffer(context, Sprites, SpriteCount);
            IndexBuffer.UploadDataToBuffer(context, Indices, IndexCount);
            return true;
        }
    }

}

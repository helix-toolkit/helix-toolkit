using ImGuiNET;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelixToolkit.SharpDX.Core.Model
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;
    using Core.Components;
    using Shaders;
    using Core;
    using Model.Scene;
    using Render;
    using Utilities;

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

        public static readonly TechniqueDescription RenderTechnique;

        static ImGuiNode()
        {
            RenderTechnique = new TechniqueDescription(ImGuiRenderTechnique)
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
                    Topology = PrimitiveTopology.TriangleList,
                    BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                    DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                    RasterStateDescription = DefaultRasterDescriptions.RSSpriteCW,
                }
            }
            };
        }
        #endregion
        private ImGui2DBufferModel bufferModel;

        private IntPtr fontAtlasID = (IntPtr)1;

        private bool newFrame = false;

        private TimeSpan previousTime = TimeSpan.Zero;

        public event EventHandler UpdatingImGuiUI;

        protected override RenderCore OnCreateRenderCore()
        {
            return new ImGuiRenderCore();
        }

        protected override void OnDetach()
        {
            bufferModel = null;
            (RenderCore as ImGuiRenderCore).TextureView = null;
            base.OnDetach();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[ImGuiRenderTechnique];
        }

        public override void Update(RenderContext context)
        {
            base.Update(context);
            if (newFrame)
            {
                newFrame = false;
                ImGui.Render();
            }
            var io = ImGui.GetIO();      
            io.DisplaySize = new System.Numerics.Vector2((int)context.ActualWidth, (int)context.ActualHeight);
            if(previousTime == TimeSpan.Zero)
            {
                previousTime = context.TimeStamp;
            }
            io.Framerate = (float)(context.TimeStamp - previousTime).TotalSeconds;
            previousTime = context.TimeStamp;
            ImGui.NewFrame();
            UpdatingImGuiUI?.Invoke(this, EventArgs.Empty);
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
            previousTime = TimeSpan.Zero;
            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            ImGui.GetIO().Fonts.AddFontDefault();
            bufferModel = Collect(new ImGui2DBufferModel());
            (RenderCore as ImGuiRenderCore).Buffer = bufferModel;
            var io = ImGui.GetIO();
            unsafe
            {
                io.Fonts.GetTexDataAsRGBA32(out IntPtr textureData, out var width, out var height);
                var textureView = Collect(new ShaderResourceViewProxy(EffectsManager.Device));
                textureView.CreateView(textureData, width, height, 
                    Format.R8G8B8A8_UNorm);
                io.Fonts.SetTexID(fontAtlasID);
                io.Fonts.ClearTexData();
                (RenderCore as ImGuiRenderCore).TextureView = textureView;
            }
            ImGui.NewFrame();
            newFrame = true;
            base.OnAttached();
        }
    }

    public sealed class ImGuiRenderCore : RenderCore
    {
        public ImGui2DBufferModel Buffer { set; get; }

        public Matrix ProjectionMatrix
        {
            set; get;
        } = Matrix.Identity;

        public ShaderResourceViewProxy TextureView;

        private int texSlot;

        private int samplerSlot;

        private ShaderPass spritePass;

        private SamplerStateProxy sampler;

        private readonly ConstantBufferComponent globalTransformCB;
        public ImGuiRenderCore()
            : base(RenderType.ScreenSpaced)
        {
            globalTransformCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
        }

        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (Buffer == null || TextureView == null || spritePass.IsNULL)
            {
                return;
            }        
            
            var io = ImGui.GetIO();
            ImGui.Render();
            if (!UpdateBuffer(deviceContext))
            {
                return;
            }
          
            ProjectionMatrix = Matrix.OrthoOffCenterRH(
                0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            int slot = 0;
            if (!Buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager))
            {
                return;
            }
            var globalTrans = context.GlobalTransform;
            globalTrans.Projection = ProjectionMatrix;
            globalTransformCB.Upload(deviceContext, ref globalTrans);
            spritePass.BindShader(deviceContext);
            spritePass.BindStates(deviceContext, StateType.All);
            spritePass.PixelShader.BindTexture(deviceContext, texSlot, TextureView);
            spritePass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            deviceContext.SetViewport(0, 0, io.DisplaySize.X, io.DisplaySize.Y);
            #region Render
            unsafe
            {
                var draw_data = ImGui.GetDrawData();
                int idx_offset = 0;
                int vtx_offset = 0;
                for (int n = 0; n < draw_data.CmdListsCount; n++)
                {
                    var cmd_list = draw_data.CmdListsRange[n];
                    for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                    {
                        var pcmd = &(((ImDrawCmd*)cmd_list.CmdBuffer.Data)[cmd_i]);
                        if (pcmd->UserCallback != IntPtr.Zero)
                        {

                        }
                        else
                        {
                            deviceContext.SetScissorRectangle(
                                (int)pcmd->ClipRect.X,
                                (int)pcmd->ClipRect.Y,
                                (int)(pcmd->ClipRect.Z),
                                (int)(pcmd->ClipRect.W));

                            deviceContext.DrawIndexed((int)pcmd->ElemCount,
                                idx_offset, vtx_offset);
                        }

                        idx_offset += (int)pcmd->ElemCount;
                    }
                    vtx_offset += cmd_list.VtxBuffer.Size;
                }
                #endregion
            }
            RaiseInvalidateRender();
        }

        private bool UpdateBuffer(DeviceContextProxy deviceContext)
        {
            unsafe
            {
                var data = ImGui.GetDrawData();
                if (data.CmdListsCount == 0)
                {
                    return false;
                }
                Buffer.SpriteCount = data.TotalVtxCount;
                Buffer.IndexCount = data.TotalIdxCount;
                
                Buffer.VertexBufferInternal.EnsureBufferCapacity(deviceContext, data.TotalVtxCount, data.TotalVtxCount * 2);
                Buffer.IndexBufferInternal.EnsureBufferCapacity(deviceContext, data.TotalIdxCount, data.TotalIdxCount * 2);
                Buffer.VertexBufferInternal.MapBuffer(deviceContext, (stream) => 
                {
                    for (int i = 0; i < data.CmdListsCount; i++)
                    {
                        var cmd_list = data.CmdListsRange[i];
                        int vCount = cmd_list.VtxBuffer.Size * sizeof(ImDrawVert);
                        stream.WriteRange((IntPtr)cmd_list.VtxBuffer.Data, vCount);
                    }
                });
                Buffer.IndexBufferInternal.MapBuffer(deviceContext, (stream) => 
                {
                    for (int i = 0; i < data.CmdListsCount; i++)
                    {
                        var cmd_list = data.CmdListsRange[i];
                        int iCount = cmd_list.IdxBuffer.Size * sizeof(ushort);
                        stream.WriteRange((IntPtr)cmd_list.IdxBuffer.Data, iCount);
                    }
                });
            }
            return true;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            spritePass = technique[DefaultPassNames.Default];
            texSlot = spritePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SpriteTB);
            samplerSlot = spritePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SpriteSampler);
            sampler = Collect(EffectTechnique.EffectsManager.StateManager.Register(DefaultSamplers.PointSamplerWrap));
            return true;
        }

        protected override void OnDetach()
        {
            TextureView = null;
            sampler = null;
            base.OnDetach();
        }
    }


    public sealed class ImGui2DBufferModel : ReferenceCountDisposeObject, IGUID, IAttachableBufferModel
    {
        public PrimitiveTopology Topology { get; set; } = PrimitiveTopology.TriangleList;

        public IElementsBufferProxy[] VertexBuffer { get; } = new DynamicBufferProxy[1];

        public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }

        public IElementsBufferProxy IndexBuffer { get; }

        public Guid GUID { get; } = Guid.NewGuid();

        public int SpriteCount;
        public int IndexCount;

        internal readonly DynamicBufferProxy VertexBufferInternal;

        internal readonly DynamicBufferProxy IndexBufferInternal;

        public ImGui2DBufferModel()
        {
            VertexBufferInternal = Collect(new DynamicBufferProxy(global::SharpDX.Utilities.SizeOf<ImDrawVert>(), BindFlags.VertexBuffer));
            VertexBuffer[0] = VertexBufferInternal;
            IndexBuffer = IndexBufferInternal = Collect(new DynamicBufferProxy(sizeof(ushort), BindFlags.IndexBuffer));
        }

        public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
        {
            if (SpriteCount == 0 || IndexCount == 0)
            {
                return false;
            }
            context.SetVertexBuffers(0, new VertexBufferBinding(VertexBufferInternal.Buffer, VertexBufferInternal.StructureSize, VertexBufferInternal.Offset));
            context.SetIndexBuffer(IndexBufferInternal.Buffer, Format.R16_UInt, IndexBufferInternal.Offset);
            return true;
        }

        public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
        {
            return true;
        }
    }
}

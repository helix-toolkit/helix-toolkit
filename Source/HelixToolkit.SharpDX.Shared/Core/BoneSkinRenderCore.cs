/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;

    public class BoneSkinRenderCore : MeshRenderCore
    {
        private bool matricsChanged = true;
        private Matrix[] boneMatrices;
        public Matrix[] BoneMatrices
        {
            set
            {
                if(SetAffectsRender(ref boneMatrices, value))
                {
                    matricsChanged = true;
                    if(value == null)
                    {
                        boneMatrices = new Matrix[0];
                    }
                }
            }
            get { return boneMatrices; }
        }

        private StructuredBufferProxy boneSkinSB;
        private int boneSkinSBSlot;
        private ShaderPass preComputeBoneSkinPass;
        private IBoneSkinPreComputehBufferModel preComputeBoneBuffer;
        

        public BoneSkinRenderCore()
        {
            NeedUpdate = true;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                matricsChanged = true;
                preComputeBoneSkinPass = technique[DefaultPassNames.PreComputeMeshBoneSkinned];
                boneSkinSBSlot = preComputeBoneSkinPass.VertexShader.ShaderResourceViewMapping.GetMapping(DefaultBufferNames.BoneSkinSB).Slot;
                boneSkinSB = Collect(new StructuredBufferProxy(MatrixHelper.SizeInBytes, false));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnGeometryBufferChanged(IAttachableBufferModel buffer)
        {
            base.OnGeometryBufferChanged(buffer);
            preComputeBoneBuffer = buffer as IBoneSkinPreComputehBufferModel;
        }

        protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (preComputeBoneSkinPass.IsNULL || preComputeBoneBuffer == null || !preComputeBoneBuffer.CanPreCompute || !matricsChanged)
            {
                return;
            }
            if(boneMatrices.Length == 0)
            {
                preComputeBoneBuffer.ResetSkinnedVertexBuffer(deviceContext);
            }
            else
            {
                GeometryBuffer.UpdateBuffers(deviceContext, EffectTechnique.EffectsManager);
                preComputeBoneBuffer.BindSkinnedVertexBufferToOutput(deviceContext);
                boneSkinSB.UploadDataToBuffer(deviceContext, BoneMatrices, BoneMatrices.Length);
                preComputeBoneSkinPass.BindShader(deviceContext);
                deviceContext.SetShaderResource(VertexShader.Type, boneSkinSBSlot, boneSkinSB);
                deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
                preComputeBoneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            }
            matricsChanged = false;         
        }

        protected override void OnDetach()
        {
            preComputeBoneBuffer = null;
            boneSkinSB = null;
            base.OnDetach();
        }
    }
}

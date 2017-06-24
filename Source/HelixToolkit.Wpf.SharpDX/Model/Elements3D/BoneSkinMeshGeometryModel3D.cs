using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BoneSkinMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static DependencyProperty VertexBoneIdsProperty = DependencyProperty.Register("VertexBoneIds", typeof(IList<BoneIds>), typeof(BoneSkinMeshGeometryModel3D), 
            new AffectsRenderPropertyMetadata(null, (d,e)=>
            {
                (d as BoneSkinMeshGeometryModel3D).OnBoneParameterChanged();
            }));

        public IList<BoneIds> VertexBoneIds
        {
            set
            {
                SetValue(VertexBoneIdsProperty, value);
            }
            get
            {
                return (IList<BoneIds>)GetValue(VertexBoneIdsProperty);
            }
        }

        public static DependencyProperty BoneMatricesProperty = DependencyProperty.Register("BoneMatrices", typeof(BoneMatricesStruct), typeof(BoneSkinMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(new BoneMatricesStruct() { Bones = new Matrix[BoneMatricesStruct.NumberOfBones] }, 
                (d, e) =>
                {
                    (d as BoneSkinMeshGeometryModel3D).OnBoneMatricesChanged();
                }));

        public BoneMatricesStruct BoneMatrices
        {
            set
            {
                SetValue(BoneMatricesProperty, value);
            }
            get
            {
                return (BoneMatricesStruct)GetValue(BoneMatricesProperty);
            }
        }

        private readonly DynamicBufferProxy<BoneIds> vertexBoneParamsBuffer = new DynamicBufferProxy<BoneIds>(BoneIds.SizeInBytes, BindFlags.VertexBuffer);
        private EffectScalarVariable hasBonesVar;
        private EffectMatrixVariable boneMatricesVar;
        private bool hasBoneParameter = false;
        private bool isBoneParamChanged = false;
        private bool hasBoneMatrices = false;
        private BoneMatricesStruct mBones;

        private static readonly BufferDescription cBufferDesc = new BufferDescription()
        {
            BindFlags = BindFlags.ConstantBuffer,
            CpuAccessFlags = CpuAccessFlags.Write,
            Usage = ResourceUsage.Dynamic,
            OptionFlags = ResourceOptionFlags.None,
            SizeInBytes = BoneMatricesStruct.SizeInBytes,
            StructureByteStride = 0
        };

        private void OnBoneParameterChanged()
        {
            isBoneParamChanged = true;
            hasBoneParameter = VertexBoneIds != null;
        }

        private void OnBoneMatricesChanged()
        {
            hasBoneMatrices = BoneMatrices.Bones != null;
            mBones = BoneMatrices;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BoneSkinBlinn];
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            OnBoneParameterChanged();
            OnBoneMatricesChanged();
            hasBonesVar = effect.GetVariableByName("bHasBones").AsScalar();
            boneMatricesVar = effect.GetVariableByName("SkinMatrices").AsMatrix();
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            vertexBoneParamsBuffer.Dispose();
            Disposer.RemoveAndDispose(ref boneMatricesVar);
            Disposer.RemoveAndDispose(ref hasBonesVar);
        }

        protected override void OnRender(RenderContext renderContext)
        {
            this.bHasInstances.Set(this.hasInstances);
            // --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));
            hasBonesVar.Set(hasBoneParameter && hasBoneMatrices);
            
            if (hasBoneMatrices)
            {
                boneMatricesVar.SetMatrix(mBones.Bones);
            }
            if (this.hasBoneParameter)
            {
                if (isBoneParamChanged && this.VertexBoneIds.Count >= geometryInternal.Positions.Count)
                {
                    vertexBoneParamsBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.VertexBoneIds);
                    this.isBoneParamChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.vertexBoneParamsBuffer.Buffer, this.vertexBoneParamsBuffer.StructureSize, 0));
            }
            if (this.hasInstances)
            {
                // --- update instance buffer
                if (this.isInstanceChanged)
                {
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.instanceInternal);
                    this.isInstanceChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(2, new VertexBufferBinding(this.InstanceBuffer.Buffer, this.InstanceBuffer.StructureSize, 0));
                OnInstancedDrawCall(renderContext);
            }
            else
            {
                // --- bind buffer                
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));
                OnDrawCall(renderContext);
            }
        }

        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return true;
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && !hasBoneParameter;
        }
    }
}

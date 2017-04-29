using HelixToolkit.Wpf.SharpDX.Core;
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
            new PropertyMetadata(null, (d,e)=>
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
            new PropertyMetadata(new BoneMatricesStruct() { Bones = new Matrix[BoneMatricesStruct.NumberOfBones] }, (d, e) =>
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

        private Buffer vertexBoneParamsBuffer = null;
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
            Disposer.RemoveAndDispose(ref vertexBoneParamsBuffer);
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
            this.effectMaterial.AttachMaterial();
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
            hasBonesVar.Set(hasBoneParameter && hasBoneMatrices);
            
            if (hasBoneMatrices)
            {
                boneMatricesVar.SetMatrix(mBones.Bones);
            }
            if (this.hasBoneParameter)
            {
                if (isBoneParamChanged && this.VertexBoneIds.Count >= Geometry.Positions.Count)
                {
                    if (vertexBoneParamsBuffer == null || this.vertexBoneParamsBuffer.Description.SizeInBytes < BoneIds.SizeInBytes * this.VertexBoneIds.Count)
                    {
                        Disposer.RemoveAndDispose(ref vertexBoneParamsBuffer);
                        this.vertexBoneParamsBuffer = Buffer.Create(this.Device, this.VertexBoneIds.ToArray(),
                            new BufferDescription(BoneIds.SizeInBytes * this.VertexBoneIds.Count, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                            CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    }
                    else
                    {
                        DataStream stream;
                        renderContext.DeviceContext.MapSubresource(this.vertexBoneParamsBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.VertexBoneIds.ToArray(), 0, this.VertexBoneIds.Count);
                        renderContext.DeviceContext.UnmapSubresource(this.vertexBoneParamsBuffer, 0);
                        stream.Dispose();
                    }
                    this.isBoneParamChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.vertexBoneParamsBuffer, BoneIds.SizeInBytes, 0));
            }
            if (this.hasInstances)
            {
                // --- update instance buffer
                if (this.isInstanceChanged)
                {
                    if (instanceBuffer == null || instanceBuffer.Description.SizeInBytes < Matrix.SizeInBytes * this.Instances.Count)
                    {
                        Disposer.RemoveAndDispose(ref instanceBuffer);
                        this.instanceBuffer = Buffer.Create(this.Device, this.Instances.ToArray(), new BufferDescription(Matrix.SizeInBytes * this.Instances.Count, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    }
                    else
                    {
                        DataStream stream;
                        renderContext.DeviceContext.MapSubresource(this.instanceBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.Instances.ToArray(), 0, this.Instances.Count);
                        renderContext.DeviceContext.UnmapSubresource(this.instanceBuffer, 0);
                        stream.Dispose();
                    }
                    this.isInstanceChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(2, new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0));
                // --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);
                // --- draw              
                renderContext.DeviceContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.Instances.Count, 0, 0, 0);
                this.bHasInstances.Set(false);
            }
            else
            {
                // --- bind buffer                
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
                // --- render the geometry
                // 
                var pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
            }
        }

        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return true;
        }

        public override bool HitTest(RenderContext context, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (hasBoneParameter)
            {
                //Disable for now. Pending implementation.
                return false;
            }
            return base.HitTest(context, rayWS, ref hits);
        }
    }
}

using System.Linq;
using global::SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
    {
        protected Buffer instanceAdvBuffer = null;
        protected bool instanceAdvArrayChanged = true;
        protected bool hasAdvInstancing = false;
        protected InstanceParameter[] instanceAdvArray;
        private EffectScalarVariable hasAdvInstancingVar;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<InstanceParameter> InstanceAdvArray
        {
            get { return (IEnumerable<InstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceAdvArray", typeof(IEnumerable<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), new UIPropertyMetadata(null, InstancesChanged));


        protected static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingMeshGeometryModel3D)d;
            if (e.NewValue != null)
            {
                model.instanceAdvArray = ((IEnumerable<InstanceParameter>)e.NewValue).ToArray();
            }
            else
            {
                model.instanceAdvArray = null;
            }
            model.instanceAdvArrayChanged = true;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.InstancingBlinn];
        }

        protected override void OnRender(RenderContext renderContext)
        {
            /// --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            /// --- set material params      
            if (phongMaterial != null)
            {
                this.effectMaterial.vMaterialDiffuseVariable.Set(phongMaterial.DiffuseColor);
                this.effectMaterial.vMaterialAmbientVariable.Set(phongMaterial.AmbientColor);
                this.effectMaterial.vMaterialEmissiveVariable.Set(phongMaterial.EmissiveColor);
                this.effectMaterial.vMaterialSpecularVariable.Set(phongMaterial.SpecularColor);
                this.effectMaterial.vMaterialReflectVariable.Set(phongMaterial.ReflectiveColor);
                this.effectMaterial.sMaterialShininessVariable.Set(phongMaterial.SpecularShininess);

                /// --- has samples              
                this.effectMaterial.bHasDiffuseMapVariable.Set(phongMaterial.DiffuseMap != null);
                this.effectMaterial.bHasDiffuseAlphaMapVariable.Set(phongMaterial.DiffuseAlphaMap != null);
                this.effectMaterial.bHasNormalMapVariable.Set(phongMaterial.NormalMap != null);

                /// --- set samplers
                if (phongMaterial.DiffuseMap != null)
                {
                    this.effectMaterial.texDiffuseMapVariable.SetResource(this.texDiffuseMapView);
                }

                if (phongMaterial.NormalMap != null)
                {
                    this.effectMaterial.texNormalMapVariable.SetResource(this.texNormalMapView);
                }

                if (phongMaterial.DiffuseAlphaMap != null)
                {
                    this.effectMaterial.texDiffuseAlphaMapVariable.SetResource(this.texDiffuseAlphaMapView);
                }
            }

            /// --- check instancing
            this.hasInstances = (this.Instances != null) && (this.Instances.Any());
            this.hasAdvInstancing = (this.InstanceAdvArray != null && this.instanceAdvArray.Any());
            this.bHasInstances?.Set(this.hasInstances);
            this.hasAdvInstancingVar?.Set(this.hasAdvInstancing);
            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
            if (this.hasAdvInstancing)
            {
                if (instanceAdvArrayChanged)
                {
                    Disposer.RemoveAndDispose(ref instanceAdvBuffer);
                    if (instanceAdvArray != null)
                    {
                        this.instanceAdvBuffer = Buffer.Create(this.Device, this.instanceAdvArray, new BufferDescription(InstanceParameter.SizeInBytes * this.instanceAdvArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                        DataStream stream;
                        Device.ImmediateContext.MapSubresource(this.instanceAdvBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.instanceAdvArray, 0, this.instanceAdvArray.Length);
                        Device.ImmediateContext.UnmapSubresource(this.instanceAdvBuffer, 0);
                        stream.Dispose();
                    }
                    this.instanceAdvArrayChanged = false;
                }
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.instanceAdvBuffer, InstanceParameter.SizeInBytes, 0));
                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.instanceAdvArray.Length, 0, 0, 0);
            }
            else if (this.hasInstances)
            {
                /// --- update instance buffer
                if (this.isChanged)
                {
                    Disposer.RemoveAndDispose(ref instanceBuffer);
                    this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    DataStream stream;
                    Device.ImmediateContext.MapSubresource(this.instanceBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                    stream.Position = 0;
                    stream.WriteRange(this.instanceArray, 0, this.instanceArray.Length);
                    Device.ImmediateContext.UnmapSubresource(this.instanceBuffer, 0);
                    stream.Dispose();
                    this.isChanged = false;
                }
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0));
                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.instanceArray.Length, 0, 0, 0);
            }
            else
            {
                /// --- render the geometry
                /// 
                var pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            instanceAdvArrayChanged = true;
            hasAdvInstancingVar = effect.GetVariableByName("bHasAdvInstancing").AsScalar();
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            Disposer.RemoveAndDispose(ref instanceAdvBuffer);
            Disposer.RemoveAndDispose(ref hasAdvInstancingVar);
        }
    }
}

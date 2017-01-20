using System.Linq;
using global::SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Model.Elements3D
{
    public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
    {
        protected Buffer instanceColorBuffer = null;
        protected Buffer instanceTextureOffsetBuffer = null;
        protected bool instanceColorArrayChanged = true;
        protected bool instanceTextureOffsetBufferChanged = true;

        protected Color4[] instanceColorArray;
        protected Vector2[] instanceTextureOffsetArray;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Color4> InstanceColorArray
        {
            get { return (IEnumerable<Color4>)this.GetValue(InstanceColorArrayProperty); }
            set { this.SetValue(InstanceColorArrayProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InstanceColorArrayProperty =
            DependencyProperty.Register("InstanceColorArray", typeof(IEnumerable<Color4>), typeof(InstancingMeshGeometryModel3D), new UIPropertyMetadata(null, InstancesColorChanged));


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Vector2> InstanceTextureOffsetArray
        {
            get { return (IEnumerable<Vector2>)this.GetValue(InstanceTextureOffsetArrayProperty); }
            set { this.SetValue(InstanceTextureOffsetArrayProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InstanceTextureOffsetArrayProperty =
            DependencyProperty.Register("InstanceTextureOffsetArray", typeof(IEnumerable<Vector2>), typeof(InstancingMeshGeometryModel3D), new UIPropertyMetadata(null, InstancesTexOffsetChanged));

        protected static void InstancesColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingMeshGeometryModel3D)d;
            if (e.NewValue != null)
            {
                model.instanceColorArray = ((IEnumerable<Color4>)e.NewValue).ToArray();
            }
            else
            {
                model.instanceColorArray = null;
            }
            model.instanceColorArrayChanged = true;
        }

        protected static void InstancesTexOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingMeshGeometryModel3D)d;
            if (e.NewValue != null)
            {
                model.instanceTextureOffsetArray = ((IEnumerable<Vector2>)e.NewValue).ToArray();
            }
            else
            {
                model.instanceTextureOffsetArray = null;
            }
            model.instanceTextureOffsetBufferChanged = true;
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
            if (this.bHasInstances != null)
            {
                this.bHasInstances.Set(this.hasInstances);
            }

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;
            if (this.hasInstances)
            {
                var bufferList = new List<VertexBufferBinding>(4);
                bufferList.Add(new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
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
                    bufferList.Add(new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0));
                }

                if(instanceColorArrayChanged)
                {
                    Disposer.RemoveAndDispose(ref instanceColorBuffer);
                    if(instanceColorArray !=null && instanceColorArray.Length == instanceArray.Length)
                    {
                        this.instanceColorBuffer = Buffer.Create(this.Device, this.instanceColorArray, new BufferDescription(Vector4.SizeInBytes * this.instanceColorArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                        DataStream stream;
                        Device.ImmediateContext.MapSubresource(this.instanceColorBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.instanceColorArray, 0, this.instanceColorArray.Length);
                        Device.ImmediateContext.UnmapSubresource(this.instanceColorBuffer, 0);
                        stream.Dispose();
                        bufferList.Add(new VertexBufferBinding(this.instanceColorBuffer, Vector4.SizeInBytes, 0));
                    }
                    this.instanceColorArrayChanged = false;
                }

                if (instanceTextureOffsetBufferChanged)
                {
                    Disposer.RemoveAndDispose(ref instanceTextureOffsetBuffer);
                    if(instanceTextureOffsetArray != null && instanceTextureOffsetArray.Length == instanceArray.Length)
                    {
                        this.instanceTextureOffsetBuffer = Buffer.Create(this.Device, this.instanceTextureOffsetArray, new BufferDescription(Vector2.SizeInBytes * this.instanceTextureOffsetArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                        DataStream stream;
                        Device.ImmediateContext.MapSubresource(this.instanceTextureOffsetBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.instanceTextureOffsetArray, 0, this.instanceTextureOffsetArray.Length);
                        Device.ImmediateContext.UnmapSubresource(this.instanceTextureOffsetBuffer, 0);
                        stream.Dispose();
                        bufferList.Add(new VertexBufferBinding(this.instanceTextureOffsetBuffer, Vector2.SizeInBytes, 0));
                    }
                    this.instanceTextureOffsetBufferChanged = false;
                }
                
                /// --- INSTANCING: need to set 2 buffers            
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, bufferList.ToArray());

                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.instanceArray.Length, 0, 0, 0);
            }
            else
            {
                /// --- bind buffer                
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
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
            instanceColorArrayChanged = true;
            instanceTextureOffsetBufferChanged = true;
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            Disposer.RemoveAndDispose(ref instanceColorBuffer);
            Disposer.RemoveAndDispose(ref instanceTextureOffsetBuffer);
        }
    }
}

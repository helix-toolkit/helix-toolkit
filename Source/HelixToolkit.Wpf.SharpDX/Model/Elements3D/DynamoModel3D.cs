using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace HelixToolkit.Wpf.SharpDX
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DynamoVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;

        /// <summary>
        /// Store a number of parameters for Dynamo in a Vector4.
        /// 0 - IsSelected
        /// 1 - RequiresPerVertexColoration
        /// </summary>
        public Vector4 DynamoParams;

        public const int SizeInBytes = 4 * (4 + 4 + 2 + 3 + 3 + 3 + 4);
    }

    public class DynamoGeometryModel3D : MaterialGeometryModel3D
    {
        public bool RequiresPerVertexColoration
        {
            get
            {
                return (bool)this.GetValue(RequiresPerVertexColorationProperty);
            }
            set { this.SetValue(RequiresPerVertexColorationProperty, value); }
        }
        public static readonly DependencyProperty RequiresPerVertexColorationProperty =
            DependencyProperty.Register("RequiresPerVertexColoration", typeof(bool), typeof(GeometryModel3D), new UIPropertyMetadata(false));

        protected override void OnRasterStateChanged(int depthBias)
        {
            if (this.IsAttached)
            {
                Disposer.RemoveAndDispose(ref this.rasterState);
                /// --- set up rasterizer states
                var rasterStateDesc = new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    DepthBias = depthBias,
                    DepthBiasClamp = -1000,
                    SlopeScaledDepthBias = +0,
                    IsDepthClipEnabled = true,
                    IsFrontCounterClockwise = true,

                    //IsMultisampleEnabled = true,
                    //IsAntialiasedLineEnabled = true,                    
                    //IsScissorEnabled = true,
                };
                try
                {
                    this.rasterState = new RasterizerState(this.Device, rasterStateDesc);
                }
                catch (System.Exception)
                {
                }
            }
        }

        public override void Attach(IRenderHost host)
        {
            base.Attach(host);

            this.renderTechnique = Techniques.RenderDynamo;

            if (this.Geometry == null)
                return;

            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            this.effectTransforms = new EffectTransformVariables(this.effect);

            this.AttachMaterial();

            var geometry = this.Geometry as MeshGeometry3D;

            if (geometry == null)
            {
                throw new Exception("Geometry must not be null");
            }

            this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, DynamoVertex.SizeInBytes,
                this.CreateDynamoVertexArray());
            this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int),
                this.Geometry.Indices.ToArray());

            this.hasInstances = (this.Instances != null) && (this.Instances.Any());
            this.bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            if (this.hasInstances)
            {
                this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
            }

            this.OnRasterStateChanged(this.DepthBias);

            this.Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vertexBuffer);
            Disposer.RemoveAndDispose(ref this.indexBuffer);
            Disposer.RemoveAndDispose(ref this.instanceBuffer);
            Disposer.RemoveAndDispose(ref this.effectMaterial);
            Disposer.RemoveAndDispose(ref this.effectTransforms);
            Disposer.RemoveAndDispose(ref this.texDiffuseMapView);
            Disposer.RemoveAndDispose(ref this.texNormalMapView);
            Disposer.RemoveAndDispose(ref this.bHasInstances);

            this.renderTechnique = null;
            this.phongMaterial = null;
            this.effectTechnique = null;
            this.vertexLayout = null;

            base.Detach();
        }

        public override void Render(RenderContext renderContext)
        {
            /// --- check to render the model
            {
                if (!this.IsRendering)
                    return;

                if (this.Geometry == null)
                    return;

                if (this.Visibility != System.Windows.Visibility.Visible)
                    return;

                if (renderContext.IsShadowPass)
                    if (!this.IsThrowingShadow)
                        return;
            }

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
                /// --- update instance buffer
                if (this.isChanged)
                {
                    this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    DataStream stream;
                    Device.ImmediateContext.MapSubresource(this.instanceBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                    stream.Position = 0;
                    stream.WriteRange(this.instanceArray, 0, this.instanceArray.Length);
                    Device.ImmediateContext.UnmapSubresource(this.instanceBuffer, 0);
                    stream.Dispose();
                    this.isChanged = false;
                }

                /// --- INSTANCING: need to set 2 buffers            
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new[] 
                {
                    new VertexBufferBinding(this.vertexBuffer, DynamoVertex.SizeInBytes, 0),
                    new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0),
                });

                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.instanceArray.Length, 0, 0, 0);
            }
            else
            {
                /// --- bind buffer                
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, DynamoVertex.SizeInBytes, 0));
                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
            }
        }

        public override void Dispose()
        {
            this.Detach();
        }

        private DynamoVertex[] CreateDynamoVertexArray()
        {
            var geometry = (MeshGeometry3D)this.Geometry;
            var colors = geometry.Colors != null ? geometry.Colors.ToArray() : null;
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.ToArray() : null;
            var texScale = this.TextureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.ToArray() : null;
            var tangents = geometry.Tangents != null ? geometry.Tangents.ToArray() : null;
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.ToArray() : null;
            var positions = geometry.Positions.ToArray();
            var vertexCount = geometry.Positions.Count;
            var result = new DynamoVertex[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                result[i] = new DynamoVertex
                {
                    Position = new Vector4(positions[i], 1f),
                    Color = colors != null ? colors[i] : Color4.White,
                    TexCoord = textureCoordinates != null ? texScale * textureCoordinates[i] : Vector2.Zero,
                    Normal = normals != null ? normals[i] : Vector3.Zero,
                    Tangent = tangents != null ? tangents[i] : Vector3.Zero,
                    BiTangent = bitangents != null ? bitangents[i] : Vector3.Zero,
                    DynamoParams = new Vector4(this.IsSelected?1:0, this.RequiresPerVertexColoration?1:0,0,0)
                };
            }

            return result;
        }
    }
}

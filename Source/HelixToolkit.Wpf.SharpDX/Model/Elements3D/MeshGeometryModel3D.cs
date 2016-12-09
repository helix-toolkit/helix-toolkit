// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Linq;

    using global::SharpDX;

    using global::SharpDX.Direct3D;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using Buffer = global::SharpDX.Direct3D11.Buffer;

    public class MeshGeometryModel3D : MaterialGeometryModel3D
    {
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(true, RasterStateChanged));

        public bool FrontCounterClockwise
        {
            set
            {
                SetValue(FrontCounterClockwiseProperty, value);
            }
            get
            {
                return (bool)GetValue(FrontCounterClockwiseProperty);
            }
        }

        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D), new PropertyMetadata(CullMode.None, RasterStateChanged));

        public CullMode CullMode
        {
            set
            {
                SetValue(CullModeProperty, value);
            }
            get
            {
                return (CullMode)GetValue(CullModeProperty);
            }
        }

        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(MeshGeometryModel3D), new PropertyMetadata(FillMode.Solid, RasterStateChanged));

        public FillMode FillMode
        {
            set
            {
                SetValue(FillModeProperty, value);
            }
            get
            {
                return (FillMode)GetValue(FillModeProperty);
            }
        }

        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(true, RasterStateChanged));

        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }

        public static readonly DependencyProperty ReuseVertexArrayBufferProperty = DependencyProperty.Register("ReuseVertexArrayBuffer", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (s, e) =>
            {
                if (!(bool)e.NewValue)
                {
                    (s as MeshGeometryModel3D).vertexArrayBuffer = null;
                }
            }));

        /// <summary>
        /// Reuse previous vertext array buffer during CreateBuffer. Reduce excessive memory allocation during rapid geometry model changes. 
        /// Example: Repeatly updates textures, or geometries with close number of vertices.
        /// </summary>
        public bool ReuseVertexArrayBuffer
        {
            set
            {
                SetValue(ReuseVertexArrayBufferProperty, value);
            }
            get
            {
                return (bool)GetValue(ReuseVertexArrayBufferProperty);
            }
        }

        public override int VertexSizeInBytes
        {
            get
            {
                return DefaultVertex.SizeInBytes;
            }
        }

        private DefaultVertex[] vertexArrayBuffer = null;

        protected override void OnRasterStateChanged()
        {
            if (this.IsAttached)
            {
                Disposer.RemoveAndDispose(ref this.rasterState);
                /// --- set up rasterizer states
                var rasterStateDesc = new RasterizerStateDescription()
                {
                    FillMode = FillMode,
                    CullMode = CullMode,
                    DepthBias = DepthBias,
                    DepthBiasClamp = -1000,
                    SlopeScaledDepthBias = +0,
                    IsDepthClipEnabled = IsDepthClipEnabled,
                    IsFrontCounterClockwise = FrontCounterClockwise,

                    IsMultisampleEnabled = IsMultisampleEnabled,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        public override void Attach(IRenderHost host)
        {
            // --- attach
            this.renderTechnique = host.RenderTechnique;
            base.Attach(host);

            if (this.Geometry == null
                || this.Geometry.Positions == null || this.Geometry.Positions.Count == 0
                || this.Geometry.Indices == null || this.Geometry.Indices.Count == 0)
            { return; }

            // --- get variables
            this.vertexLayout = renderHost.EffectsManager.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            // --- transformations
            this.effectTransforms = new EffectTransformVariables(this.effect);

            // --- material 
            this.AttachMaterial();

            // --- scale texcoords
            var texScale = TextureCoodScale;

            // --- get geometry
            var geometry = this.Geometry as MeshGeometry3D;

            // -- set geometry if given
            if (geometry != null)
            {
                //throw new HelixToolkitException("Geometry not found!");                

                /// --- init vertex buffer
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, VertexSizeInBytes, this.CreateDefaultVertexArray(), geometry.Positions.Count);

                /// --- init index buffer
                this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), this.Geometry.Indices.Array);
            }
            else
            {
                throw new System.Exception("Geometry must not be null");
            }

            /// --- init instances buffer            
            this.hasInstances = (this.Instances != null) && (this.Instances.Any());
            this.bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            if (this.hasInstances)
            {
                this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
            }

            /// --- set rasterstate
            this.OnRasterStateChanged();

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public override void Render(RenderContext renderContext)
        {
            /// --- check to render the model
            {
                if (!this.IsRendering)
                    return;

                if (this.Geometry == null
                    || this.Geometry.Positions == null || this.Geometry.Positions.Count == 0
                    || this.Geometry.Indices == null || this.Geometry.Indices.Count == 0)
                { return; }

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
                    new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0),
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
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
                /// --- render the geometry
                /// 
                var pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Detach();
        }

        /// <summary>
        /// Creates a <see cref="T:DefaultVertex[]"/>.
        /// </summary>
        private DefaultVertex[] CreateDefaultVertexArray()
        {
            var geometry = (MeshGeometry3D)this.Geometry;
            var colors = geometry.Colors != null ? geometry.Colors.Array : null;
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.Array : null;
            var texScale = this.TextureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.Array : null;
            var tangents = geometry.Tangents != null ? geometry.Tangents.Array : null;
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.Array : null;
            var positions = geometry.Positions.Array;
            var vertexCount = geometry.Positions.Count;
            if (!ReuseVertexArrayBuffer || vertexArrayBuffer == null || vertexArrayBuffer.Length < vertexCount)
                vertexArrayBuffer = new DefaultVertex[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                vertexArrayBuffer[i].Position = new Vector4(positions[i], 1f);
                vertexArrayBuffer[i].Color = colors != null ? colors[i] : Color4.White;
                vertexArrayBuffer[i].TexCoord = textureCoordinates != null ? texScale * textureCoordinates[i] : Vector2.Zero;
                vertexArrayBuffer[i].Normal = normals != null ? normals[i] : Vector3.Zero;
                vertexArrayBuffer[i].Tangent = tangents != null ? tangents[i] : Vector3.Zero;
                vertexArrayBuffer[i].BiTangent = bitangents != null ? bitangents[i] : Vector3.Zero;
            }

            return vertexArrayBuffer;
        }

    }
}
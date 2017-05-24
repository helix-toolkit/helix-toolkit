using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using HelixToolkit.Wpf.SharpDX.Utilities;

namespace CustomShaderDemo
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CustomVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;

        /// <summary>
        /// Store a number of custom parameters in a Vector4.
        /// 0 - IsSelected
        /// 1 - RequiresPerVertexColoration
        /// </summary>
        public Vector4 CustomParams;

        public const int SizeInBytes = 4 * (4 + 4 + 2 + 3 + 3 + 3 + 4);
    }

    public class CustomGeometryModel3D : MaterialGeometryModel3D
    {
        private readonly ImmutableBufferProxy<CustomVertex> vertexBuffer = new ImmutableBufferProxy<CustomVertex>(CustomVertex.SizeInBytes, BindFlags.VertexBuffer);
        private readonly ImmutableBufferProxy<int> indexBuffer = new ImmutableBufferProxy<int>(sizeof(int), BindFlags.IndexBuffer);
        protected Color4 selectionColor = new Color4(1.0f, 0.0f, 1.0f, 1.0f);

        public override IBufferProxy VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }

        public override IBufferProxy IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
        }

        public static readonly DependencyProperty RequiresPerVertexColorationProperty =
            DependencyProperty.Register("RequiresPerVertexColoration", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(false));

        public bool RequiresPerVertexColoration
        {
            get
            {
                return (bool)GetValue(RequiresPerVertexColorationProperty);
            }
            set { SetValue(RequiresPerVertexColorationProperty, value); }
        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref rasterState);
            if (!IsAttached) { return; }
            /// --- set up rasterizer states
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                DepthBias = DepthBias,
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
                rasterState = new RasterizerState(Device, rasterStateDesc);
            }
            catch (Exception)
            {
            }
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques["RenderCustom"];
        }
        protected override bool OnAttach(IRenderHost host)
        {
            if (!base.OnAttach(host))
            {
                return false;
            }

            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            effectTransforms = new EffectTransformVariables(effect);

            AttachMaterial();

            var geometry = Geometry as MeshGeometry3D;

            if (geometry == null)
            {
                throw new Exception("Geometry must not be null");
            }
            vertexBuffer.CreateBufferFromDataArray(Device, CreateCustomVertexArray());
            indexBuffer.CreateBufferFromDataArray(Device, Geometry.Indices.ToArray());
            hasInstances = (Instances != null) && (Instances.Any());
            bHasInstances = effect.GetVariableByName("bHasInstances").AsScalar();
            OnRasterStateChanged();
            if (hasInstances)
            {
                isInstanceChanged = true;
            }
            // Device.ImmediateContext.Flush();
            return true;
        }

        protected override void OnDetach()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            Disposer.RemoveAndDispose(ref effectMaterial);
            Disposer.RemoveAndDispose(ref effectTransforms);
            Disposer.RemoveAndDispose(ref bHasInstances);

            renderTechnique = null;
            effectTechnique = null;
            vertexLayout = null;

            base.OnDetach();
        }

        protected override void OnRender(RenderContext renderContext)
        {
            /// --- set constant paramerers             
            var worldMatrix = modelMatrix * renderContext.WorldMatrix;
            effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            hasShadowMap = renderHost.IsShadowMapEnabled;
            effectMaterial.bHasShadowMapVariable.Set(hasShadowMap);
            effectMaterial.AttachMaterial();

            /// --- check instancing
            hasInstances = (Instances != null) && (Instances.Any());
            if (bHasInstances != null)
            {
                bHasInstances.Set(hasInstances);
            }

            /// --- set context
            Device.ImmediateContext.InputAssembler.InputLayout = vertexLayout;
            Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Device.ImmediateContext.InputAssembler.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, 0);

            /// --- set rasterstate            
            Device.ImmediateContext.Rasterizer.State = rasterState;

            if (hasInstances)
            {
                /// --- update instance buffer
                if (isInstanceChanged)
                {
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, Instances.ToArray());
                    isInstanceChanged = false;
                }

                /// --- INSTANCING: need to set 2 buffers            
                Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new[]
                {
                    new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, 0),
                    new VertexBufferBinding(InstanceBuffer.Buffer, InstanceBuffer.StructureSize, 0),
                });

                /// --- render the geometry
                effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                Device.ImmediateContext.DrawIndexedInstanced(Geometry.Indices.Count, Instances.Count, 0, 0, 0);
            }
            else
            {
                /// --- bind buffer                
                Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, 0));
                /// --- render the geometry
                effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                Device.ImmediateContext.DrawIndexed(Geometry.Indices.Count, 0, 0);
            }
        }

        public override void Dispose()
        {
            Detach();
        }

        private CustomVertex[] CreateCustomVertexArray()
        {
            var geometry = (MeshGeometry3D)Geometry;
            var colors = geometry.Colors != null ? geometry.Colors.ToArray() : null;
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.ToArray() : null;
            var texScale = TextureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.ToArray() : null;
            var tangents = geometry.Tangents != null ? geometry.Tangents.ToArray() : null;
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.ToArray() : null;
            var positions = geometry.Positions.ToArray();
            var vertexCount = geometry.Positions.Count;
            var result = new CustomVertex[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                result[i] = new CustomVertex
                {
                    Position = new Vector4(positions[i], 1f),
                    Color = colors != null ? colors[i] : Color4.White,
                    TexCoord = textureCoordinates != null ? texScale * textureCoordinates[i] : Vector2.Zero,
                    Normal = normals != null ? normals[i] : Vector3.Zero,
                    Tangent = tangents != null ? tangents[i] : Vector3.Zero,
                    BiTangent = bitangents != null ? bitangents[i] : Vector3.Zero,
                    CustomParams = new Vector4((bool)GetValue(AttachedProperties.ShowSelectedProperty) ? 1 : 0, RequiresPerVertexColoration ? 1 : 0, 0, 0)
                };
            }

            return result;
        }

        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            return false;
        }
    }

}

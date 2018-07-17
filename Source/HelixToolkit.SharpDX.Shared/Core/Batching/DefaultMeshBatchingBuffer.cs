using SharpDX.Direct3D;
using SharpDX;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Model;
    using HelixToolkit.Wpf.SharpDX.Render;

    public struct BatchedMeshGeometryConfig : IBatchedGeometry
    {
        public Geometry3D Geometry { private set; get; }
        public Matrix ModelTransform { private set; get; }
        public int MaterialIndex { private set; get; }
        public BatchedMeshGeometryConfig(Geometry3D geometry, Matrix modelTransform, int materialIndex)
        {
            Geometry = geometry;
            ModelTransform = modelTransform;
            MaterialIndex = materialIndex;
        }
    }

    public abstract class DefaultStaticMeshBatchingBufferBase<MaterialType> 
        : StaticGeometryBatchingBufferBase<BatchedMeshGeometryConfig, BatchedMeshVertex> where MaterialType : MaterialCore
    {
        private bool materialChanged = true;
        private MaterialType[] materials;
        public MaterialType[] Materials
        {
            set
            {
                if(Set(ref materials, value))
                {
                    materialChanged = true;
                }
            }
            get
            {
                return materials;
            }
        }

        public DefaultStaticMeshBatchingBufferBase(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
            : base(topology, vertexBuffer, indexBuffer)
        {

        }

        protected override void OnSubmitGeometries(RenderContext context, DeviceContextProxy deviceContext, ref Matrix parentTransform)
        {
            base.OnSubmitGeometries(context, deviceContext, ref parentTransform);
            if (materialChanged)
            {
                OnSubmitMaterials(deviceContext);
                materialChanged = false;
            }
        }

        protected abstract void OnSubmitMaterials(DeviceContextProxy deviceContext);

        protected override void OnFillVertArray(BatchedMeshVertex[] array, int offset, ref BatchedMeshGeometryConfig geometry, ref Matrix transform)
        {
            if(geometry.Geometry is MeshGeometry3D mesh)
            {
                var vertexCount = mesh.Positions.Count;
                var positions = mesh.Positions.GetEnumerator();
                var normals = mesh.Normals != null ? mesh.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
                var tangents = mesh.Tangents != null ? mesh.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
                var bitangents = mesh.BiTangents != null ? mesh.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
                var textures = mesh.TextureCoordinates != null ? mesh.TextureCoordinates.GetEnumerator() : Enumerable.Repeat(Vector2.Zero, vertexCount).GetEnumerator();
                var colors = mesh.Colors != null ? mesh.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();

                if(transform == Matrix.Identity)
                {
                    for (int i = offset; i < offset + vertexCount; ++i)
                    {
                        positions.MoveNext();
                        normals.MoveNext();
                        tangents.MoveNext();
                        bitangents.MoveNext();
                        textures.MoveNext();
                        colors.MoveNext();
                        array[i] = new BatchedMeshVertex()
                        {
                            Position = positions.Current.ToVector4(),
                            Normal = normals.Current,
                            Tangent =tangents.Current,
                            BiTangent = bitangents.Current,
                            TexCoord = textures.Current,
                            Color = colors.Current
                        };
                    }
                }
                else
                {
                    for (int i = offset; i < offset + vertexCount; ++i)
                    {
                        positions.MoveNext();
                        normals.MoveNext();
                        tangents.MoveNext();
                        bitangents.MoveNext();
                        textures.MoveNext();
                        colors.MoveNext();
                        array[i] = new BatchedMeshVertex()
                        {
                            Position = Vector3.Transform(positions.Current, transform),
                            Normal = Vector3.TransformNormal(normals.Current, transform),
                            Tangent = Vector3.TransformNormal(tangents.Current, transform),
                            BiTangent = Vector3.TransformNormal(bitangents.Current, transform),
                            TexCoord = textures.Current,
                            Color = colors.Current
                        };
                    }
                }

                positions.Dispose();
                normals.Dispose();
                tangents.Dispose();
                bitangents.Dispose();
                textures.Dispose();
                colors.Dispose();
            }
        }
    }


}

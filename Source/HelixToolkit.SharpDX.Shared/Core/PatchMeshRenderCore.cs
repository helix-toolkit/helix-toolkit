using System;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public enum MeshTopologyEnum
    {
        None,
        PNTriangles,
        PNQuads        
    }
    public static class MeshTopologies
    {
        public static IEnumerable<MeshTopologyEnum> Topologies
        {
            get
            {
                yield return MeshTopologyEnum.None;
                yield return MeshTopologyEnum.PNTriangles;
                yield return MeshTopologyEnum.PNQuads;
            }
        }
    }
}

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class PatchMeshRenderCore : MeshRenderCore
    {
        public float TessellationFactor = 1.0f;

        private MeshTopologyEnum meshType = MeshTopologyEnum.None;
        public MeshTopologyEnum MeshType
        {
            set
            {
                meshType = value;
                switch (meshType)
                {
                    case MeshTopologyEnum.PNTriangles:
                        DefaultShaderPassName = DefaultPassNames.MeshTriTessellation;
                        break;
                    case MeshTopologyEnum.PNQuads:
                        DefaultShaderPassName = DefaultPassNames.MeshQuadTessellation;
                        break;
                    default:
                        DefaultShaderPassName = DefaultPassNames.Default;
                        break;
                }
            }
            get
            {
                return meshType;
            }
        }

        public PatchMeshRenderCore()
        {
            DefaultShaderPassName = DefaultPassNames.Default;
        }

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdateModelStruct(ref model, context);
            model.Params.X = TessellationFactor;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            switch (meshType)
            {
                case MeshTopologyEnum.PNTriangles:
                    context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith3ControlPoints;
                    break;
                case MeshTopologyEnum.PNQuads:
                    context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith4ControlPoints;
                    break;
            }
            base.OnRender(context);
        }
    }
}

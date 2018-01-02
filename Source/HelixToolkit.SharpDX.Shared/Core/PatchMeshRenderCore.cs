/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
        PNTriangles,
        PNQuads        
    }
    public static class MeshTopologies
    {
        public static IEnumerable<MeshTopologyEnum> Topologies
        {
            get
            {
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
    using Utilities;
    using Shaders;
    public interface IPatchRenderParams
    {
        float MinTessellationDistance { set; get; }
        float MaxTessellationDistance { set; get; }
        float MinTessellationFactor { set; get; }
        float MaxTessellationFactor { set; get; }
        MeshTopologyEnum MeshType { set; get; }
        bool EnableTessellation { set; get; }
    }

    public class PatchMeshRenderCore : MeshRenderCore, IPatchRenderParams
    {
        public float MinTessellationDistance
        {
            set;get;
        } = 10;

        public float MaxTessellationDistance
        {
            set;get;
        } = 100;

        public float MinTessellationFactor
        {
            set; get;
        } = 2;

        public float MaxTessellationFactor
        {
            set; get;
        } = 1;

        private MeshTopologyEnum meshType = MeshTopologyEnum.PNTriangles;
        public MeshTopologyEnum MeshType
        {
            set
            {
                meshType = value;
            }
            get
            {
                return meshType;
            }
        }

        private bool enableTessellation = false;
        public bool EnableTessellation
        {
            set
            {
                if(enableTessellation == value)
                {
                    return;
                }
                enableTessellation = value;
                if (enableTessellation)
                {
                    switch (meshType)
                    {
                        case MeshTopologyEnum.PNTriangles:
                            DefaultShaderPassName = DefaultPassNames.MeshTriTessellation;
                            break;
                        case MeshTopologyEnum.PNQuads:
                            DefaultShaderPassName = DefaultPassNames.MeshQuadTessellation;
                            break;
                    }
                }
                else
                {
                    DefaultShaderPassName = DefaultPassNames.Default;
                }
            }
            get
            {
                return enableTessellation;
            }
        }

        public PatchMeshRenderCore()
        {
            DefaultShaderPassName = DefaultPassNames.Default;
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.MaxTessDistance = this.MaxTessellationDistance;
            model.MinTessDistance = this.MinTessellationDistance;
            model.MaxTessFactor = this.MaxTessellationFactor;
            model.MinTessFactor = this.MinTessellationFactor;
        }

        protected override void OnRender(IRenderContext context)
        {
            if (EnableTessellation)
            {
                OnRenderTessellation(context);
            }
            else
            {
                base.OnRender(context);
            }
        }

        protected virtual void OnRenderTessellation(IRenderContext context)
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
            context.DeviceContext.InputAssembler.PrimitiveTopology = GeometryBuffer.Topology;
        }
    }
}

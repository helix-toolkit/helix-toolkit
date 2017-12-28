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
    public interface IPatchRenderCore
    {
        float MinTessellationDistance { set; get; }
        float MaxTessellationDistance { set; get; }
        float MinTessellationFactor { set; get; }
        float MaxTessellationFactor { set; get; }
        MeshTopologyEnum MeshType { set; get; }
        bool EnableTessellation { set; get; }
    }

    public class PatchMeshRenderCore : MeshRenderCore, IPatchRenderCore
    {
        public float MinTessellationDistance
        {
            set
            {
                TessellationParameters.MinTessDistance = value;
                tessParamUpdated = true;
            }
            get
            {
                return TessellationParameters.MinTessDistance;
            }
        }

        public float MaxTessellationDistance
        {
            set
            {
                TessellationParameters.MaxTessDistance = value;
                tessParamUpdated = true;
            }
            get
            {
                return TessellationParameters.MaxTessDistance;
            }
        }

        public float MinTessellationFactor
        {
            set
            {
                TessellationParameters.MinTessFactor = value;
                tessParamUpdated = true;
            }
            get
            {
                return TessellationParameters.MinTessFactor;
            }
        }

        public float MaxTessellationFactor
        {
            set
            {
                TessellationParameters.MaxTessFactor = value;
                tessParamUpdated = true;
            }
            get
            {
                return TessellationParameters.MaxTessFactor;
            }
        }

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


        private bool tessParamUpdated = true;
        private TessellationStruct TessellationParameters = new TessellationStruct()
        { MaxTessDistance = 50, MinTessDistance = 1, MaxTessFactor = 1, MinTessFactor = 4 };

        private IBufferProxy tessParamBuffer;

        public PatchMeshRenderCore()
        {
            DefaultShaderPassName = DefaultPassNames.Default;
        }


        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                tessParamBuffer = technique.ConstantBufferPool.Register(DefaultBufferNames.TessellationParamsCB, TessellationStruct.SizeInBytes);
                tessParamUpdated = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            if (tessParamUpdated)
            {
                tessParamBuffer.UploadDataToBuffer(context, ref TessellationParameters);
                tessParamUpdated = false;
            }
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

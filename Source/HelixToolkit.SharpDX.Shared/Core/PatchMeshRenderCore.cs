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
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Shaders;
    using Render;
    public class PatchMeshRenderCore : MeshRenderCore, IPatchRenderParams
    {
        public float MinTessellationDistance
        {
            set
            {
                SetAffectsRender(ref modelStruct.MinTessDistance, value);
            }
            get { return modelStruct.MinTessDistance; }
        }

        public float MaxTessellationDistance
        {
            set
            {
                SetAffectsRender(ref modelStruct.MaxTessDistance, value);
            }
            get { return modelStruct.MaxTessDistance; }
        }

        public float MinTessellationFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.MinTessFactor, value);
            }
            get
            {
                return modelStruct.MinTessFactor;
            }
        }


        public float MaxTessellationFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.MaxTessFactor, value);
            }
            get
            {
                return modelStruct.MaxTessFactor;
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
                if(SetAffectsRender(ref enableTessellation, value))
                {
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
            }
            get
            {
                return enableTessellation;
            }
        }

        public PatchMeshRenderCore()
        {
            DefaultShaderPassName = DefaultPassNames.Default;
            MinTessellationDistance = 10;
            MaxTessellationDistance = 100;
            MinTessellationFactor = 2;
            MaxTessellationFactor = 1;
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (EnableTessellation)
            {
                OnRenderTessellation(context, deviceContext);
            }
            else
            {
                base.OnRender(context, deviceContext);
            }
        }

        protected virtual void OnRenderTessellation(IRenderContext context, DeviceContextProxy deviceContext)
        {
            switch (meshType)
            {
                case MeshTopologyEnum.PNTriangles:
                    deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith3ControlPoints;
                    break;
                case MeshTopologyEnum.PNQuads:
                    deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith4ControlPoints;
                    break;
            }
            base.OnRender(context, deviceContext);
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = GeometryBuffer.Topology;
        }
    }
}

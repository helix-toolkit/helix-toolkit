/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
Reference: https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Model;
        using Render;
        using Shaders;
        using System.Runtime.CompilerServices;
        using Utilities;

        public sealed class VolumeRenderCore : RenderCore
        {
            private static readonly MeshGeometry3D BoxMesh;

            static VolumeRenderCore()
            {
                BoxMesh = new MeshGeometry3D()
                {
                    Positions = new Vector3Collection()
                    {
                         new Vector3(-0.5f, -0.5f, -0.5f),
                         new Vector3(0.5f, -0.5f, -0.5f),
                         new Vector3(-0.5f, 0.5f, -0.5f),
                         new Vector3(0.5f, 0.5f, -0.5f),
                         new Vector3(-0.5f, -0.5f, 0.5f),
                         new Vector3(0.5f, -0.5f, 0.5f),
                         new Vector3(-0.5f, 0.5f, 0.5f),
                         new Vector3(0.5f, 0.5f, 0.5f),
                    },
                    Indices = new IntCollection()
                    {
                        0,2,3,
                        3,1,0,
                        4,5,7,
                        7,6,4,
                        0,1,5,
                        5,4,0,
                        1,3,7,
                        7,5,1,
                        3,2,6,
                        6,7,3,
                        2,0,4,
                        4,6,2
                    }
                };
            }

            private VolumeCubeBufferModel buffer;
        
            private ShaderPass cubeBackPass;
            private ShaderPass volumePass;
            private int backTexSlot;

            private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
            /// <summary>
            /// Used to wrap all material resources
            /// </summary>
            public MaterialVariable MaterialVariables
            {
                set
                {
                    var old = materialVariables;
                    if (SetAffectsCanRenderFlag(ref materialVariables, value))
                    {
                        if (value == null)
                        {
                            materialVariables = EmptyMaterialVariable.EmptyVariable;
                        }
                    }
                }
                get
                {
                    return materialVariables;
                }
            }

            public VolumeRenderCore() 
                : base(RenderType.Opaque)
            {
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                buffer = Collect(new VolumeCubeBufferModel());
                buffer.Geometry = BoxMesh;
                buffer.Topology = PrimitiveTopology.TriangleList;
                cubeBackPass = technique[DefaultPassNames.Backface];
                return true;
            }

            protected override void OnDetach()
            {
                buffer = null;
                base.OnDetach();
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (!materialVariables.UpdateMaterialStruct(deviceContext, ref ModelMatrix, Matrix.SizeInBytes))
                {
                    return;
                }
                int slot = 0;
                buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager);
                cubeBackPass.BindShader(deviceContext);
                cubeBackPass.BindStates(deviceContext, StateType.All);
                using (var back = context.GetOffScreenRT(OffScreenTextureSize.Full, global::SharpDX.DXGI.Format.R16G16B16A16_Float))
                {
                    BindTarget(null, back, deviceContext, (int)context.ActualWidth, (int)context.ActualHeight);
                    deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
                    context.RenderHost.SetDefaultRenderTargets(false);
                    var pass = materialVariables.GetPass(RenderType.Opaque, context);
                    if (pass != volumePass)
                    {
                        volumePass = pass;
                        backTexSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeBack);
                    }
                    materialVariables.BindMaterialResources(context, deviceContext, pass);
                    volumePass.PixelShader.BindTexture(deviceContext, backTexSlot, back);
                    volumePass.BindShader(deviceContext);
                    volumePass.BindStates(deviceContext, StateType.All);
                    deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
                    volumePass.PixelShader.BindTexture(deviceContext, backTexSlot, null);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContextProxy context, int width, int height)
            {
                context.SetRenderTargets(dsv, targetView == null ? null : new RenderTargetView[] { targetView });
            }

            /// <summary>
            /// 
            /// </summary>
            private sealed class VolumeCubeBufferModel : MeshGeometryBufferModel<Vector3>
            {
                public VolumeCubeBufferModel() : base(Vector3.SizeInBytes)
                {
                    Topology = PrimitiveTopology.TriangleList;
                }

                protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
                {
                    // -- set geometry if given
                    if (geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
                    {

                        buffer.UploadDataToBuffer(context, geometry.Positions, geometry.Positions.Count);
                    }
                    else
                    {
                        buffer.UploadDataToBuffer(context, emptyVerts, 0);
                    }
                }
            }
        }
    }

}

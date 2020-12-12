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
        using Components;
        using Model;
        using Render;
        using Shaders;
        using System.Runtime.CompilerServices;
        using System.Runtime.InteropServices;
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

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            struct ModelMatrices{
                public Matrix ModelMatrix;
                public Matrix ModelMatrixInv;
                public void Update(ref Matrix modelMatrix)
                {
                    if(ModelMatrix != modelMatrix)
                    {
                        ModelMatrix = modelMatrix;
                        ModelMatrixInv = modelMatrix.Inverted();
                    }
                }
            }

            private VolumeCubeBufferModel buffer;

            private ShaderPass cubeBackPass;
            private ShaderPass volumePass;
            private ShaderPass meshFrontPass;
            private int backTexSlot;
            private readonly ConstantBufferComponent modelCB;
            private ModelMatrices modelMatrices;
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
                modelCB = Collect(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.VolumeModelCB, 
                    VolumeParamsStruct.SizeInBytes)));
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                buffer = Collect(new VolumeCubeBufferModel());
                buffer.Geometry = BoxMesh;
                buffer.Topology = PrimitiveTopology.TriangleList;
                cubeBackPass = technique[DefaultPassNames.Backface];
                meshFrontPass = technique[DefaultPassNames.Positions];
                modelCB.Attach(technique);
                return true;
            }

            protected override void OnDetach()
            {
                buffer = null;
                modelCB.Detach();
                base.OnDetach();
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                using (var back = context.GetOffScreenRT(OffScreenTextureSize.Full, global::SharpDX.DXGI.Format.R16G16B16A16_Float))
                {
                    int slot = 0;
                    using (var depth = context.GetOffScreenDS(OffScreenTextureSize.Full, global::SharpDX.DXGI.Format.D32_Float_S8X24_UInt))
                    {
                        deviceContext.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 1);
                        deviceContext.ClearRenderTargetView(back, new Color4(0, 0, 0, 0));
                        BindTarget(depth, back, deviceContext, (int)context.ActualWidth, (int)context.ActualHeight);
                        #region Render box back face and set stencil buffer to 0
                        modelMatrices.Update(ref ModelMatrix);
                        if (!materialVariables.UpdateMaterialStruct(deviceContext, ref modelMatrices, Matrix.SizeInBytes * 2))
                        {
                            return;
                        }
                        buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager);
                        cubeBackPass.BindShader(deviceContext);
                        cubeBackPass.BindStates(deviceContext, StateType.All);
                        deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
                        #endregion

                        #region Render all mesh Positions onto off-screen texture region with stencil = 0 only
                        if (context.RenderHost.PerFrameOpaqueNodesInFrustum.Count > 0)
                        {
                            for (int i = 0; i < context.RenderHost.PerFrameOpaqueNodesInFrustum.Count; ++i)
                            {
                                var mesh = context.RenderHost.PerFrameOpaqueNodesInFrustum[i];
                                var meshPass = mesh.EffectTechnique[DefaultPassNames.Positions];
                                if (meshPass.IsNULL) { continue; }
                                meshPass.BindShader(deviceContext);
                                meshPass.BindStates(deviceContext, StateType.BlendState);
                                // Set special depth stencil state to only render into region with stencil region is 0
                                meshFrontPass.BindStates(deviceContext, StateType.DepthStencilState);
                                mesh.RenderCustom(context, deviceContext);
                            }
                        }
                        #endregion
                    }

                    #region Render box back face again and do actual volume sampling
                    context.RenderHost.SetDefaultRenderTargets(false);
                    var pass = materialVariables.GetPass(RenderType.Opaque, context);
                    if (pass != volumePass)
                    {
                        volumePass = pass;
                        backTexSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeBack);
                    }
                    slot = 0;
                    buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager);
                    materialVariables.BindMaterialResources(context, deviceContext, pass);
                    volumePass.PixelShader.BindTexture(deviceContext, backTexSlot, back);
                    volumePass.BindShader(deviceContext);
                    volumePass.BindStates(deviceContext, StateType.All);
                    deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
                    #endregion
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

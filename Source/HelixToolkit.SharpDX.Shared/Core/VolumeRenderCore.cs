/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
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
            var builder = new MeshBuilder(false);
            builder.AddBox(Vector3.Zero, 1, 1, 1);
            BoxMesh = builder.ToMesh();
        }

        private VolumeCubeBufferModel buffer;

        private ShaderPass cubeFrontPass;
        private ShaderPass cubeBackPass;
        private ShaderPass volumePass;
        private VolumeParamsStruct modelStruct = new VolumeParamsStruct() { StepSize = 0.1f, Iterations = 10 };
        private int frontTexSlot, backTexSlot;

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
            cubeFrontPass = technique[DefaultPassNames.Positions];
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
            modelStruct.World = ModelMatrix;
            
            if (!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, VolumeParamsStruct.SizeInBytes))
            {
                return;
            }
            int slot = 0;
            buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager);
            
            var front = context.RenderHost.RenderBuffer.FullResRenderTargetPool.Get(global::SharpDX.DXGI.Format.R16G16B16A16_Float);
            BindTarget(null, front, deviceContext, (int)context.ActualWidth, (int)context.ActualHeight);
            cubeFrontPass.BindShader(deviceContext);
            cubeFrontPass.BindStates(deviceContext, StateType.All);
            deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
            cubeBackPass.BindStates(deviceContext, StateType.All);
            var back = context.RenderHost.RenderBuffer.FullResRenderTargetPool.Get(global::SharpDX.DXGI.Format.R16G16B16A16_Float);
            BindTarget(null, back, deviceContext, (int)context.ActualWidth, (int)context.ActualHeight);
            deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
            context.RenderHost.SetDefaultRenderTargets(false);
            var pass = materialVariables.GetPass(RenderType.Opaque, context);
            if (pass != volumePass)
            {
                volumePass = pass;
                frontTexSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeFront);
                backTexSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeBack);
            }
            materialVariables.BindMaterialResources(context, deviceContext, pass);
            volumePass.PixelShader.BindTexture(deviceContext, frontTexSlot, front);
            volumePass.PixelShader.BindTexture(deviceContext, backTexSlot, back);
            volumePass.BindShader(deviceContext);
            volumePass.BindStates(deviceContext, StateType.All);
            deviceContext.DrawIndexed(buffer.IndexBuffer.ElementCount, 0, 0);
            context.RenderHost.RenderBuffer.FullResRenderTargetPool.Put(global::SharpDX.DXGI.Format.R16G16B16A16_Float, front);
            context.RenderHost.RenderBuffer.FullResRenderTargetPool.Put(global::SharpDX.DXGI.Format.R16G16B16A16_Float, back);
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContextProxy context, int width, int height, bool clear = true)
        {
            if (clear)
            {
                context.ClearRenderTargetView(targetView, global::SharpDX.Color.Transparent);
            }
            context.SetRenderTargets(dsv, targetView == null ? null : new RenderTargetView[] { targetView });
            context.SetViewport(0, 0, width, height);
            context.SetScissorRectangle(0, 0, width, height);
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

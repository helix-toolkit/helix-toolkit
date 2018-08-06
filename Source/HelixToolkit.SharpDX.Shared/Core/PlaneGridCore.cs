/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;

    public class PlaneGridCore : RenderCoreBase<PointLineModelStruct>
    {
        private ShaderPass DefaultShaderPass;

        public float GridSpacing
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.X, value);
            }
            get
            {
                return modelStruct.Params.X;
            }
        }

        public float GridThickness
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get
            {
                return modelStruct.Params.Y;
            }
        }

        public float FadingFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Z, value);
            }
            get { return modelStruct.Params.Z; }
        }

        public PlaneGridCore() : base(RenderType.Opaque)
        {
            modelStruct = new PointLineModelStruct()
            {
                World = Matrix.Identity,                
            };
            GridSpacing = 10;
            GridThickness = 0.1f;
            FadingFactor = 0.1f;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            DefaultShaderPass = technique[DefaultPassNames.Default];
            return base.OnAttach(technique);
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
            deviceContext.Draw(4, 0);
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            model.Params.X = GridSpacing;
            model.Params.Y = GridThickness;
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}

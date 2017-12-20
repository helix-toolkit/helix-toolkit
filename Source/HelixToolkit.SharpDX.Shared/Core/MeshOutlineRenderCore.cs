using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshOutlineRenderCore : MeshRenderCore
    {
        /// <summary>
        /// Outline color
        /// </summary>
        public Color4 Color { set; get; } = new Color4(1, 1, 1, 1);
        /// <summary>
        /// Enable outline
        /// </summary>
        public bool OutlineEnabled { set; get; } = false;

        /// <summary>
        /// Draw original mesh
        /// </summary>
        public bool DrawMesh { set; get; } = true;

        /// <summary>
        /// Draw outline order
        /// </summary>
        public bool DrawOutlineBeforeMesh { set; get; } = false;

        /// <summary>
        /// Outline fading
        /// </summary>
        public float OutlineFadingFactor { set; get; } = 1.5f;

        private string outlinePassName = DefaultPassNames.MeshOutline;
        public string OutlinePassName
        {
            set
            {
                if (outlinePassName == value)
                { return; }
                outlinePassName = value;
                if (IsAttached)
                {
                    outlineShaderPass = EffectTechnique[value];
                }
            }
            get
            {
                return outlinePassName;
            }
        }

        protected IShaderPass outlineShaderPass { private set; get; }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            outlineShaderPass = technique[OutlinePassName];
            return base.OnAttach(technique);
        }

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {            
            base.OnUpdateModelStruct(ref model, context);
            model.Color = Color;
            model.Params.X = OutlineFadingFactor;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            UpdateModelConstantBuffer(context.DeviceContext);
            context.DeviceContext.Rasterizer.State = RasterState;
            if (DrawOutlineBeforeMesh)
            {
                outlineShaderPass.BindShader(context.DeviceContext);
                outlineShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (DrawMesh)
            {
                if (!UpdateMaterialConstantBuffer(context.DeviceContext))
                {
                    return;
                }
                DefaultShaderPass.BindShader(context.DeviceContext);
                DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                if (!BindMaterialTextures(context.DeviceContext, DefaultShaderPass.GetShader(ShaderStage.Pixel)))
                {
                    return;
                }             
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (!DrawOutlineBeforeMesh)
            {
                outlineShaderPass.BindShader(context.DeviceContext);
                outlineShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
        }
    }

    public class MeshXRayRenderCore : MeshOutlineRenderCore
    {
        public MeshXRayRenderCore()
        {
            DrawOutlineBeforeMesh = true;
            OutlinePassName = DefaultPassNames.MeshXRay;
        }
    }
}

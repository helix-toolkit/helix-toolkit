#if !NETFX_CORE
using SharpDX;

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

        private IShaderPass xRayTechnique;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            xRayTechnique = technique.GetPass(GetPassName());
            return base.OnAttach(technique);
        }

        protected virtual string GetPassName()
        {
            return DefaultRenderTechniqueNames.MeshOutline;
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
                xRayTechnique.BindShader(context.DeviceContext);
                xRayTechnique.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (DrawMesh)
            {
                if (!UpdateMaterialConstantBuffer(context.DeviceContext))
                {
                    return;
                }
                EffectTechnique[0].BindShader(context.DeviceContext);
                EffectTechnique[0].BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                if (!BindMaterialTextures(context.DeviceContext, EffectTechnique[0].GetShader(ShaderStage.Pixel)))
                {
                    return;
                }             
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (!DrawOutlineBeforeMesh)
            {
                xRayTechnique.BindShader(context.DeviceContext);
                xRayTechnique.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
        }
    }

    public class MeshXRayRenderCore : MeshOutlineRenderCore
    {
        public MeshXRayRenderCore()
        {
            DrawOutlineBeforeMesh = true;
        }

        protected override string GetPassName()
        {
            return DefaultRenderTechniqueNames.MeshXRay;
        }
    }
}

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshOutlineRenderCore : PatchMeshRenderCore
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

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {            
            base.OnUpdatePerModelStruct(ref model, context);
            model.Color = Color;
            model.Params.Y = OutlineFadingFactor;
        }

        protected override void OnRender(IRenderContext context)
        {
            if (DrawOutlineBeforeMesh)
            {
                outlineShaderPass.BindShader(context.DeviceContext);
                outlineShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
                OnDraw(context.DeviceContext, InstanceBuffer);
            }
            if (DrawMesh)
            {
                base.OnRender(context);
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

using SharpDX.Direct3D11;


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    /// <summary>
    /// 
    /// </summary>
    public abstract class MaterialGeometryRenderCore : GeometryRenderCore, IMaterialRenderCore
    {
        private IEffectMaterialVariables materialVariables;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public IEffectMaterialVariables MaterialVariables { get { return materialVariables; } }
        private Material material = PhongMaterials.Black;
        public Material Material
        {
            set
            {
                if (material != value)
                {
                    material = value;
                    if (materialVariables != null)
                    {
                        materialVariables.Material = value;
                    }
                }
            }
            get
            {
                return material;
            }
        }
        public bool RenderDiffuseMap { set; get; } = true;
        public bool RenderDiffuseAlphaMap { set; get; } = true;
        public bool RenderNormalMap { set; get; } = true;
        public bool RenderDisplacementMap { set; get; } = true;
        public bool HasShadowMap { set; get; } = false;

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                if (materialVariables != null)
                {
                    materialVariables.OnInvalidateRenderer -= InvalidateRenderer;
                    RemoveAndDispose(ref materialVariables);
                }
                materialVariables = Collect(CreateEffectMaterialVariables(Effect));
                materialVariables.Material = Material;
                materialVariables.OnInvalidateRenderer += InvalidateRenderer;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Create effect material varaible model
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        protected virtual IEffectMaterialVariables CreateEffectMaterialVariables(Effect effect)
        {
            return new EffectMaterialVariables(Effect);
        }
        /// <summary>
        /// Upload material into shader variables
        /// </summary>
        /// <param name="model"></param>
        protected void SetMaterialVariables(MeshGeometry3D model)
        {
            if (!IsAttached || model == null) { return; }
            materialVariables.HasShadowMap = this.HasShadowMap;
            materialVariables.RenderDiffuseMap = this.RenderDiffuseMap;
            materialVariables.RenderNormalMap = this.RenderNormalMap;
            materialVariables.RenderDisplacementMap = this.RenderDisplacementMap;
            materialVariables.RenderDiffuseAlphaMap = this.RenderDiffuseAlphaMap;
            materialVariables.AttachMaterial(model);
        }        
    }
}

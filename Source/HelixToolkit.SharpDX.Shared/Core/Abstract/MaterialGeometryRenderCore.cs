/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using ShaderManager;
    using Shaders;
    using System.Collections.Generic;

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
        private IMaterial material = null;
        public IMaterial Material
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

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                if (materialVariables != null)
                {
                    materialVariables.OnInvalidateRenderer -= InvalidateRenderer;
                    RemoveAndDispose(ref materialVariables);
                }
                materialVariables = Collect(CreateEffectMaterialVariables(technique.EffectsManager));
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
        protected virtual IEffectMaterialVariables CreateEffectMaterialVariables(IEffectsManager manager)
        {
            return new PhongMaterialVariables(manager);
        }
        /// <summary>
        /// Set control variables into material variables object
        /// </summary>
        /// <param name="model"></param>
        protected virtual void SetMaterialVariables()
        {
            if (!IsAttached)
            { return; }
            materialVariables.HasShadowMap = this.HasShadowMap;
            materialVariables.RenderDiffuseMap = this.RenderDiffuseMap;
            materialVariables.RenderNormalMap = this.RenderNormalMap;
            materialVariables.RenderDisplacementMap = this.RenderDisplacementMap;
            materialVariables.RenderDiffuseAlphaMap = this.RenderDiffuseAlphaMap;
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            SetMaterialVariables();
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            MaterialVariables.UpdateMaterialConstantBuffer(context);
        }
        
        protected bool BindMaterialTextures(DeviceContext context, IShader shader)
        {
            return MaterialVariables.BindMaterialTextures(context, shader);
        }

        protected bool BindMaterialTextures(DeviceContext context, IEnumerable<IShader> shader)
        {
            return MaterialVariables.BindMaterialTextures(context, shader);
        }
    }
}

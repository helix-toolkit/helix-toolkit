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
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public abstract class MaterialGeometryRenderCore : GeometryRenderCore<ModelStruct>, IMaterialRenderParams
    {
        private IEffectMaterialVariables materialVariables;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public IEffectMaterialVariables MaterialVariables { get { return materialVariables; } }
        private MaterialCore material = null;
        /// <summary>
        /// 
        /// </summary>
        public MaterialCore Material
        {
            set
            {
                if(Set(ref material, value))
                {
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
        private bool renderDiffuseMap = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderDiffuseMap
        {
            set
            {
                if(Set(ref renderDiffuseMap, value) && materialVariables != null)
                {
                    materialVariables.RenderDiffuseMap = value;
                }               
            }
            get { return renderDiffuseMap; }
        }

        private bool renderDiffuseAlphaMap = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderDiffuseAlphaMap
        {
            set
            {
                if(Set(ref renderDiffuseAlphaMap, value) && materialVariables != null)
                {
                    materialVariables.RenderDiffuseAlphaMap = value;
                }
            }
            get
            {
                return renderDiffuseAlphaMap;
            }
        }
        private bool renderNormalMap = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderNormalMap
        {
            set
            {
                if(Set(ref renderNormalMap, value) && materialVariables != null)
                {
                    materialVariables.RenderNormalMap = value;
                }
            }
            get
            {
                return renderNormalMap;
            }
        }
        private bool renderDisplacementMap = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderDisplacementMap
        {
            set
            {
                if(Set(ref renderDisplacementMap, value) && materialVariables != null)
                {
                    materialVariables.RenderDisplacementMap = value;
                }
            }
            get { return renderDisplacementMap; }
        }

        private bool renderShadowMap = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderShadowMap
        {
            set
            {
                if(Set(ref renderShadowMap, value) && materialVariables != null)
                {
                    materialVariables.RenderShadowMap = value;
                }
            }
            get { return renderShadowMap; }
        }
        private bool renderEnvironmentMap = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderEnvironmentMap
        {
            set
            {
                if(Set(ref renderEnvironmentMap, value) && materialVariables != null)
                {
                    materialVariables.RenderEnvironmentMap = value;
                }
            }
            get { return renderEnvironmentMap; }
        }
        /// <summary>
        /// <see cref="RenderCoreBase{TModelStruct}.OnAttach(IRenderTechnique)"/>
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                if (materialVariables != null)
                {
                    RemoveAndDispose(ref materialVariables);
                }
                materialVariables = Collect(CreateEffectMaterialVariables(technique.EffectsManager));
                materialVariables.Material = Material;
                MaterialVariables.RenderShadowMap = this.RenderShadowMap;
                MaterialVariables.RenderDiffuseMap = this.RenderDiffuseMap;
                MaterialVariables.RenderNormalMap = this.RenderNormalMap;
                MaterialVariables.RenderDisplacementMap = this.RenderDisplacementMap;
                MaterialVariables.RenderDiffuseAlphaMap = this.RenderDiffuseAlphaMap;
                MaterialVariables.RenderEnvironmentMap = this.RenderEnvironmentMap;
                MaterialVariables.OnInvalidateRenderer += (s,e)=> { InvalidateRenderer(); };                
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            materialVariables = null;
            base.OnDetach();
        }
        /// <summary>
        /// <see cref="RenderCoreBase{TModelStruct}.GetModelConstantBufferDescription"/>
        /// </summary>
        /// <returns></returns>
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        /// <summary>
        /// Create effect material varaible model
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        protected virtual IEffectMaterialVariables CreateEffectMaterialVariables(IEffectsManager manager)
        {
            return new TextureSharedPhongMaterialVariables(manager, this.GUID);//new PhongMaterialVariables(manager);
        }
        /// <summary>
        /// <see cref="RenderCoreBase{TModelStruct}.OnUpdatePerModelStruct(ref TModelStruct, IRenderContext)"/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            MaterialVariables.UpdateMaterialVariables(ref model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContext context, IShaderPass shader)
        {
            return MaterialVariables.BindMaterialTextures(context, shader);
        }
    }
}

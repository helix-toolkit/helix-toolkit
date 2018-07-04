/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public abstract class MaterialGeometryRenderCore : GeometryRenderCore<ModelStruct>, IMaterialRenderParams
    {
        private IEffectMaterialVariables materialVariables = EmptyMaterialVariable.EmptyVariable;
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
                if(Set(ref material, value) && IsAttached)
                {
                    RemoveAndDispose(ref materialVariables);
                    if (value != null)
                    {
                        materialVariables = Collect(value.CreateMaterialVariables(EffectTechnique.EffectsManager));
                        AssignMaterialVariableProperties(technique);
                    }
                    else
                    {
                        materialVariables = EmptyMaterialVariable.EmptyVariable;
                    }
                }
            }
            get
            {
                return material;
            }
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

        private IRenderTechnique technique;
        /// <summary>
        /// <see cref="RenderCoreBase{TModelStruct}.OnAttach(IRenderTechnique)"/>
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                this.technique = technique;
                if (material != null)
                {
                    materialVariables = Collect(material.CreateMaterialVariables(technique.EffectsManager));
                    AssignMaterialVariableProperties(technique);
                }
                else
                {
                    materialVariables = EmptyMaterialVariable.EmptyVariable;
                }  
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AssignMaterialVariableProperties(IRenderTechnique technique)
        {
            materialVariables.OnInvalidateRenderer -= MaterialVariables_OnInvalidateRenderer;
            materialVariables.RenderShadowMap = this.RenderShadowMap;
            materialVariables.RenderEnvironmentMap = this.RenderEnvironmentMap;
            materialVariables.OnInvalidateRenderer += MaterialVariables_OnInvalidateRenderer;
            materialVariables.Attach(technique);
        }

        private void MaterialVariables_OnInvalidateRenderer(object sender, System.EventArgs e)
        {
            InvalidateRenderer();
        }

        protected override void OnDetach()
        {
            materialVariables = EmptyMaterialVariable.EmptyVariable;
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
        /// <see cref="RenderCoreBase{TModelStruct}.OnUpdatePerModelStruct(ref TModelStruct, RenderContext)"/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
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
        public bool BindMaterialTextures(DeviceContextProxy context, ShaderPass shader)
        {
            return MaterialVariables.BindMaterialTextures(context, shader);
        }
    }
}

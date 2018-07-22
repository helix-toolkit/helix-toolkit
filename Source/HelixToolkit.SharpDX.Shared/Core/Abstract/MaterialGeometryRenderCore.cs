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
        private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
        private bool needMaterialUpdate = false;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public MaterialVariable MaterialVariables { get { return materialVariables; } }
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
                    if(materialVariables != null)
                    {
                        materialVariables.OnUpdateNeeded -= MaterialVariables_OnUpdateNeeded;
                    }
                    RemoveAndDispose(ref materialVariables);
                    if (value != null)
                    {
                        materialVariables = Collect(EffectTechnique.EffectsManager.MaterialVariableManager.Register(value));
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
                    materialVariables = Collect(technique.EffectsManager.MaterialVariableManager.Register(material));
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
            materialVariables.Attach(technique);
            materialVariables.OnUpdateNeeded += MaterialVariables_OnUpdateNeeded;
            needMaterialUpdate = true;
        }

        private void MaterialVariables_OnUpdateNeeded(object sender, System.EventArgs e)
        {
            needMaterialUpdate = true;
        }

        protected override void OnDetach()
        {
            if (materialVariables != null)
            {
                materialVariables.OnUpdateNeeded -= MaterialVariables_OnUpdateNeeded;
            }
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
            model.World = ModelMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            if (needMaterialUpdate)
            {
                MaterialVariables.UpdateMaterialStruct(ref model);
                needMaterialUpdate = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContextProxy context, ShaderPass shader)
        {
            return MaterialVariables.BindMaterial(context, shader);
        }
    }
}

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
        private bool needMaterialUpdate = false;
        private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public MaterialVariable MaterialVariables
        {
            set
            {
                var old = materialVariables;
                if(Set(ref materialVariables, value))
                {
                    if (old != null)
                    {
                        old.OnUpdateNeeded -= MaterialVariables_OnUpdateNeeded;
                    }
                    if (value != null)
                    {
                        value.OnUpdateNeeded += MaterialVariables_OnUpdateNeeded;
                        needMaterialUpdate = true;
                    }
                    else
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
                needMaterialUpdate = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MaterialVariables_OnUpdateNeeded(object sender, System.EventArgs e)
        {
            needMaterialUpdate = true;
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

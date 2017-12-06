using System;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class PatchMeshRenderCore : MeshRenderCore
    {
        public float TessellationFactor = 1.0f;
       // private EffectVectorVariable vTessellationVariables;
        public string TessellationTechniqueName;
        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {            // --- init tessellation vars
             //   vTessellationVariables = Collect(Effect.GetVariableByName(ShaderVariableNames.TessellationFactorVariable).AsVector());
                if (technique.Name.Equals(TessellationRenderTechniqueNames.PNTriangles))
                {
                    this.GeometryBuffer.Topology = PrimitiveTopology.PatchListWith3ControlPoints;
                }
                else if(technique.Name.Equals(TessellationRenderTechniqueNames.PNQuads))
                {
                    this.GeometryBuffer.Topology = PrimitiveTopology.PatchListWith4ControlPoints;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //protected override void SetShaderVariables(IRenderMatrices context)
        //{
        //    base.SetShaderVariables(context);
        //    vTessellationVariables.Set(new Vector4(TessellationFactor, 0, 0, 0));
        //}

        protected override bool CanRender()
        {
            return base.CanRender() && !string.IsNullOrEmpty(TessellationTechniqueName);
        }

        protected override void OnRender(IRenderMatrices context)
        {
            //EffectTechnique.GetPassByName(TessellationTechniqueName).Apply(context.DeviceContext);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}

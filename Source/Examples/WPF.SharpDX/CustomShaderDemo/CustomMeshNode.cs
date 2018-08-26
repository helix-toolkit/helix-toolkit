using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShaderDemo
{
    public class CustomMeshNode : MeshNode
    {
        public float HeightScale
        {
            set
            {
                (RenderCore as CustomMeshCore).DataHeightScale = value;
            }
            get
            {
                return (RenderCore as CustomMeshCore).DataHeightScale;
            }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new CustomMeshCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[CustomShaderNames.DataSampling];
        }
    }
}

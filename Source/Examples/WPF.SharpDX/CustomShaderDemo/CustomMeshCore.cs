using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Render;
using HelixToolkit.Wpf.SharpDX.Shaders;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShaderDemo
{
    public class CustomMeshCore : MeshRenderCore
    {
        private int ColorTextureSlot;
        private int ColorTextureSamplerSlot;
        private SamplerStateProxy colorTextureSampler;

        private bool colorChanged = true;

        private ShaderResourceViewProxy colorGradientResource;

        private Color4Collection colorGradients;
        /// <summary>
        /// Gets or sets the colorGradients.
        /// </summary>
        /// <value>
        /// ColorGradients
        /// </value>
        public Color4Collection ColorGradients
        {
            set
            {
                if (SetAffectsCanRenderFlag(ref colorGradients, value))
                {
                    colorChanged = true;
                }
            }
            get { return colorGradients; }
        }

        private float dataHeightScale = 5;
        public float DataHeightScale
        {
            set
            {
                SetAffectsRender(ref dataHeightScale, value);
            }
            get { return dataHeightScale; }
        }

        protected override void OnDefaultPassChanged(ShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            ColorTextureSlot = pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(CustomShaderNames.TexData);
            ColorTextureSamplerSlot = pass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(CustomShaderNames.TexDataSampler);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                colorTextureSampler = Collect(technique.EffectsManager.StateManager.Register(new SamplerStateDescription()
                {
                     AddressU = TextureAddressMode.Clamp,
                     AddressV = TextureAddressMode.Clamp,
                     AddressW = TextureAddressMode.Clamp,
                     Filter = Filter.MinMagMipLinear
                }));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.Params.Y = dataHeightScale;
        }

        protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            if(base.OnAttachBuffers(context, ref vertStartSlot))
            {
                if (colorChanged)
                {
                    RemoveAndDispose(ref colorGradientResource);
                    if(ColorGradients != null)
                    {
                        colorGradientResource = new ShaderResourceViewProxy(Device);
                        colorGradientResource.CreateViewFromColorArray(ColorGradients.ToArray());
                    }
                    colorChanged = false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && ColorGradients != null;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.PixelShader.BindSampler(deviceContext, ColorTextureSamplerSlot, colorTextureSampler);
            DefaultShaderPass.PixelShader.BindTexture(deviceContext, ColorTextureSlot, colorGradientResource);
            base.OnRender(context, deviceContext);
        }
    }
}

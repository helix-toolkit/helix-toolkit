using System;
using System.Collections.Generic;
using System.IO;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Shaders;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace CustomShaderDemo
{
    public static class CustomShaderNames
    {
        public static readonly string DataSampling = "DataSampling";

        public static readonly string TexData = "texData";
        public static readonly string TexDataSampler = "texDataSampler";
    }
    public static class ShaderHelper
    {
        public static byte[] LoadShaderCode(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new ArgumentException($"Shader File not found: {path}");
            }
        }
    }


    public static class CustomVSShaderDescription
    {
        public static byte[] VSMeshDataSamplerByteCode
        {
            get
            {
                return ShaderHelper.LoadShaderCode(@"Shaders\vsMeshDataSampling.cso");
            }
        }
        public static ShaderDescription VSDataSampling = new ShaderDescription(nameof(VSDataSampling), ShaderStage.Vertex,
            new ShaderReflector(), VSMeshDataSamplerByteCode);
    }

    public static class CustomPSShaderDescription
    {
        public static ShaderDescription PSDataSampling = new ShaderDescription(nameof(PSDataSampling), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psMeshDataSampling.cso"));
    }

    public class CustomEffectsManager : DefaultEffectsManager
    {
        protected override IList<TechniqueDescription> LoadTechniqueDescriptions()
        {
            var techniqueList = base.LoadTechniqueDescriptions();
            var dataSampling = new TechniqueDescription(CustomShaderNames.DataSampling)
            {
                InputLayoutDescription = new InputLayoutDescription(CustomVSShaderDescription.VSMeshDataSamplerByteCode, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSDataSampling,
                            CustomPSShaderDescription.PSDataSampling
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            techniqueList.Add(dataSampling);
            return techniqueList;
        }
    }
}

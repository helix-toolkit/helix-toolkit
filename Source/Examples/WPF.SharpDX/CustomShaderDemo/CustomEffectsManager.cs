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
        public static readonly string NoiseMesh = "NoiseMesh";
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

    /// <summary>
    /// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
    /// </summary>
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
    /// <summary>
    /// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
    /// </summary>
    public static class CustomPSShaderDescription
    {
        public static ShaderDescription PSDataSampling = new ShaderDescription(nameof(PSDataSampling), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psMeshDataSampling.cso"));

        public static ShaderDescription PSNoiseMesh = new ShaderDescription(nameof(PSNoiseMesh), ShaderStage.Pixel,
            new ShaderReflector(), ShaderHelper.LoadShaderCode(@"Shaders\psMeshNoiseBlinnPhong.cso"));
    }

    public class CustomEffectsManager : DefaultEffectsManager
    {
        public CustomEffectsManager()
        {
            LoadCustomTechniqueDescriptions();
        }


        private void LoadCustomTechniqueDescriptions()
        {
            var dataSampling = new TechniqueDescription(CustomShaderNames.DataSampling)
            {
                InputLayoutDescription = new InputLayoutDescription(CustomVSShaderDescription.VSMeshDataSamplerByteCode, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSDataSampling,
                            //DefaultVSShaderDescriptions.VSMeshDefault,
                            CustomPSShaderDescription.PSDataSampling
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSDataSampling,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };
            var noiseMesh = new TechniqueDescription(CustomShaderNames.NoiseMesh)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            CustomPSShaderDescription.PSNoiseMesh
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };
            AddTechnique(dataSampling);
            AddTechnique(noiseMesh);
        }
    }
}

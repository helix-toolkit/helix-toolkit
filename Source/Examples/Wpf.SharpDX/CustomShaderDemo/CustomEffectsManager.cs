using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Shaders;

namespace CustomShaderDemo;

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

        var points = GetTechnique(DefaultRenderTechniqueNames.Points);
        points?.AddPass(new ShaderPassDescription("CustomPointPass")
        {
            ShaderList = new[]
            {
                    DefaultVSShaderDescriptions.VSPoint,
                    DefaultGSShaderDescriptions.GSPoint,
                    CustomPSShaderDescription.PSCustomPoint
                },
            BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
            DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
        });
    }
}

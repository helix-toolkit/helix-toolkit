using HelixToolkit.SharpDX.Shaders;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Model;

[DataContract]
public sealed class GenericLineMaterialCore : GenericMaterialCore
{
    public GenericLineMaterialCore()
        : base(MaterialVariable.DefaultPointLineConstantBufferDesc)
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericLineMaterialCore"/> class.
    /// </summary>
    /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
    /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
    public GenericLineMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
        : base(shaderPass, modelMaterialConstantBufferName)
    {

    }
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new GenericMeshMaterialVariable(manager, technique, this, cbDescription,
            MaterialPassName, ShadowPassName, string.Empty);
    }
}

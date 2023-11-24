using HelixToolkit.SharpDX.Shaders;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Model;

[DataContract]
public sealed class GenericMeshMaterialCore : GenericMaterialCore
{
    public GenericMeshMaterialCore()
        : base(MaterialVariable.DefaultMeshConstantBufferDesc)
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericMeshMaterialCore"/> class.
    /// </summary>
    /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
    /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
    public GenericMeshMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
        : base(shaderPass, modelMaterialConstantBufferName)
    {

    }
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new GenericMeshMaterialVariable(manager, technique, this, cbDescription,
            MaterialPassName, ShadowPassName, WireframePassName);
    }
}

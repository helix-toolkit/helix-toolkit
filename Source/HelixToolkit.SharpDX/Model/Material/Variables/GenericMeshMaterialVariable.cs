using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;

namespace HelixToolkit.SharpDX.Model;

public sealed class GenericMeshMaterialVariable : GenericMaterialVariable
{
    public GenericMeshMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
        GenericMaterialCore materialCore, ConstantBufferDescription? constantBufferDescription,
        string materialShaderPassName = DefaultPassNames.Default,
        string shadowShaderPassName = DefaultPassNames.ShadowPass,
        string wireframePassName = DefaultPassNames.Wireframe)
        : base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, wireframePassName)
    {

    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.IndexBuffer is null)
        {
            return;
        }

        DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
    }
}

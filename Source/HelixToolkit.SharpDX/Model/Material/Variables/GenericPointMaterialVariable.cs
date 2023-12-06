using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;

namespace HelixToolkit.SharpDX.Model;

public sealed class GenericPointMaterialVariable : GenericMaterialVariable
{
    public GenericPointMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
        GenericMaterialCore materialCore, ConstantBufferDescription? constantBufferDescription,
        string materialShaderPassName = DefaultPassNames.Default,
        string shadowShaderPassName = DefaultPassNames.ShadowPass)
        : base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, string.Empty)
    {

    }

    public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
    {
        if (bufferModel.VertexBuffer[0] is null)
        {
            return;
        }

        DrawPoints(deviceContext, bufferModel.VertexBuffer[0]!.ElementCount, instanceCount);
    }
}

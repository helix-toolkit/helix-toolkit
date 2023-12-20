using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CustomShaderDemo.Materials;

public class CustomPointMaterialVariable : PointMaterialVariable
{
    private readonly ConstantBufferComponent customConstantBuffer;
    private Vector3 colorChanges = new(1, 1, 1);

    public CustomPointMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PointMaterialCore materialCore,
        string pointPassName = "CustomPointPass")
        : base(manager, technique, materialCore, pointPassName)
    {
        customConstantBuffer = new ConstantBufferComponent(new ConstantBufferDescription("CustomBuffer", Marshal.SizeOf<Vector4>()));
        customConstantBuffer.Attach(technique);
    }

    protected override void OnInitialPropertyBindings()
    {
        base.OnInitialPropertyBindings();
    }

    public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
    {
        colorChanges += new Vector3(0.1f, 0.3f, 0.7f);
        colorChanges.X %= 100;
        colorChanges.Y %= 100;
        colorChanges.Z %= 100;
        customConstantBuffer.WriteValueByName("random_color", new Vector3(colorChanges.X / 100, colorChanges.Y / 100, colorChanges.Z / 100));
        customConstantBuffer.Upload(deviceContext);
        return base.BindMaterialResources(context, deviceContext, shaderPass);
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        customConstantBuffer.Detach();
        customConstantBuffer.Dispose();
        base.OnDispose(disposeManagedResources);
    }
}

using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public sealed class BoneUploaderCore : RenderCore
{
    public event EventHandler? BoneChanged;
    private static readonly Matrix[] empty = Array.Empty<Matrix>();
    private bool matricesChanged = true;
    private Matrix[]? boneMatrices = empty;
    public Matrix[]? BoneMatrices
    {
        set
        {
            if (SetAffectsRender(ref boneMatrices, value))
            {
                matricesChanged = true;
                if (value == null)
                {
                    boneMatrices = empty;
                }
                BoneChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        get
        {
            return boneMatrices;
        }
    }
    public StructuredBufferProxy? boneSkinSB = null;
    public StructuredBufferProxy? BoneSkinSB => boneSkinSB;

    public BoneUploaderCore() : base(RenderType.None)
    {
        NeedUpdate = false;
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {

    }

    protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (matricesChanged && BoneSkinSB != null && boneMatrices is not null)
        {
            BoneSkinSB.UploadDataToBuffer(deviceContext, boneMatrices, boneMatrices.Length);
            matricesChanged = false;
        }
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        boneSkinSB = new StructuredBufferProxy(NativeHelper.SizeOf<Matrix>(), false);
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref boneSkinSB);
    }

    public void BindBuffer(DeviceContextProxy deviceContext, int slot)
    {
        if (BoneSkinSB != null)
        {
            deviceContext.SetShaderResource(VertexShader.Type, slot, BoneSkinSB);
        }
    }

    public void InvalidateBoneMatrices()
    {
        matricesChanged = true;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        if (disposeManagedResources)
        {
            BoneChanged = null;
        }
        base.OnDispose(disposeManagedResources);
    }
}

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class DynamicBillboardBufferModel : BillboardBufferModel<BillboardVertex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicBillboardBufferModel"/> class.
    /// </summary>
    public DynamicBillboardBufferModel() : base(BillboardVertex.SizeInBytes, true) { }
}

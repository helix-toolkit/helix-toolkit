namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class DefaultBillboardBufferModel : BillboardBufferModel<BillboardVertex>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBillboardBufferModel"/> class.
    /// </summary>
    public DefaultBillboardBufferModel() : base(BillboardVertex.SizeInBytes) { }
}

using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public class MatrixInstanceBufferModel : ElementsBufferModel<Matrix>
{
    public MatrixInstanceBufferModel()
        : base(NativeHelper.SizeOf<Matrix>())
    {
    }
}

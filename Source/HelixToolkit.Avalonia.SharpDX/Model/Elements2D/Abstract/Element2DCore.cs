using Avalonia.LogicalTree;

namespace HelixToolkit.Avalonia.SharpDX.Core2D;

public partial class Element2DCore
{
    protected void AddLogicalChild(ILogical? child)
    {
        if (child is null)
        {
            return;
        }

        this.LogicalChildren.Add(child);
    }

    protected void RemoveLogicalChild(ILogical? child)
    {
        if (child is null)
        {
            return;
        }

        this.LogicalChildren.Remove(child);
    }
}

using Avalonia.LogicalTree;

namespace HelixToolkit.Avalonia.SharpDX.Model;

public partial class Element3DCore
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

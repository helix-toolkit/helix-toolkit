using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public class PanelNode2D : SceneNode2D
{
    protected readonly Dictionary<Guid, SceneNode2D> itemHashSet = new();

    public PanelNode2D()
    {
        ItemsInternal = new ObservableFastList<SceneNode2D>();
        Items = new ReadOnlyObservableFastList<SceneNode2D>(ItemsInternal);
    }

    public virtual bool AddChildNode(SceneNode2D node)
    {
        if (!itemHashSet.ContainsKey(node.GUID))
        {
            itemHashSet.Add(node.GUID, node);
            ItemsInternal.Add(node);
            node.Parent = this;
            if (IsAttached)
            {
                node.Attach(RenderHost);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public virtual void Clear()
    {
        for (var i = 0; i < ItemsInternal.Count; ++i)
        {
            ItemsInternal[i].Detach();
            ItemsInternal[i].Parent = null;
        }
        itemHashSet.Clear();
        ItemsInternal.Clear();
    }

    /// <summary>
    /// Removes the child node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns></returns>
    public virtual bool RemoveChildNode(SceneNode2D node)
    {
        if (itemHashSet.Remove(node.GUID))
        {
            node.Detach();
            ItemsInternal.Remove(node);
            node.Parent = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Tries the get node.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="node">The node.</param>
    /// <returns></returns>
    public bool TryGetNode(Guid guid, [NotNullWhen(true)] out SceneNode2D? node)
    {
        return itemHashSet.TryGetValue(guid, out node);
    }

    protected override bool OnAttach(IRenderHost host)
    {
        for (var i = 0; i < ItemsInternal.Count; ++i)
        {
            ItemsInternal[i].Attach(host);
        }
        return true;
    }

    protected override void OnDetach()
    {
        for (var i = 0; i < ItemsInternal.Count; ++i)
        {
            ItemsInternal[i].Detach();
        }
        base.OnDetach();
    }

    protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
    {
        hitResult = null;
        if (!LayoutBoundWithTransform.Contains(mousePoint))
        {
            return false;
        }
        foreach (var item in ItemsInternal.Reverse())
        {
            if (item.HitTest(mousePoint, out hitResult))
            {
                return true;
            }
        }
        return false;
    }
}

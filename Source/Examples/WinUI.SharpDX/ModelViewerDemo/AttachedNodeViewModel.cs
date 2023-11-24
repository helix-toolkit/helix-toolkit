using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

namespace ModelViewerDemo;

/// <summary>
/// Provide your own view model to manipulate the scene nodes
/// </summary>
public partial class AttachedNodeViewModel : ObservableObject
{
    [ObservableProperty]
    private bool selected = false;

    [ObservableProperty]
    private bool expanded = false;

    public bool IsAnimationNode => node.IsAnimationNode;

    public string Name => node.Name;

    private readonly SceneNode node;

    public AttachedNodeViewModel(SceneNode node)
    {
        this.node = node;
        node.Tag = this;
    }

    partial void OnSelectedChanged(bool value)
    {
        if (node is GeometryNode m)
        {
            m.PostEffects = value ? $"highlight[color:#FFFF00]" : "";

            foreach (var n in node.TraverseUp())
            {
                if (n.Tag is AttachedNodeViewModel vm)
                {
                    vm.Expanded = true;
                }
            }
        }
    }
}

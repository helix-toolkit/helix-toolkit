using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViewer.ViewModels;

/// <summary>
/// Provide your own view model to manipulate the scene nodes
/// </summary>
/// <seealso cref="DemoCore.ObservableObject" />
[ObservableObject]
partial class AttachedNodeViewModel
{
    [ObservableProperty]
    private bool selected = false;
    [ObservableProperty]
    private bool expanded = false;

    public bool IsAnimationNode  => node.IsAnimationNode;

    public string Name => node.Name;

    private SceneNode node;

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


using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoadDemo
{
    /// <summary>
    /// Provide your own view model to manipulate the scene nodes
    /// </summary>
    /// <seealso cref="DemoCore.ObservableObject" />
    public class AttachedNodeViewModel : ObservableObject
    {
        private bool selected = false;
        public bool Selected
        {
            set
            {
                if (SetValue(ref selected, value))
                {
                    if(node is MeshNode m)
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
            get => selected;
        }

        private bool expanded = false;
        public bool Expanded
        {
            set => SetValue(ref expanded, value);
            get => expanded;
        }

        public bool IsAnimationNode { get => node.IsAnimationNode; }

        public string Name { get => node.Name; }

        private SceneNode node;

        public AttachedNodeViewModel(SceneNode node)
        {
            this.node = node;
            node.Tag = this;
        }
    }
}

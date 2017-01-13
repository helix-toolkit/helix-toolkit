using System.Collections.Generic;
using HelixToolkit.SharpDX.Shared.Utilities;

namespace HelixToolkit.Wpf.SharpDX
{
    public interface IOctreeManager
    {
        bool Enabled { get; set; }
        IOctree Octree { get; set; }
        OctreeBuildParameter Parameter { get; set; }
        bool RequestUpdateOctree { get; }

        bool AddPendingItem(Element3D item);
        void Clear();
        void RebuildTree(IList<Element3D> items);
        void RemoveItem(Element3D item);
        void RequestRebuild();
    }
}
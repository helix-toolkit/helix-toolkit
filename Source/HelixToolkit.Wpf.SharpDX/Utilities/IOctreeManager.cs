// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOctreeManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace HelixToolkit.Wpf.SharpDX
{
    public interface IOctreeManager
    {
        bool Enabled { get; set; }
        IOctree Octree { get; set; }
        bool RequestUpdateOctree { get; }

        bool AddPendingItem(Element3D item);
        void Clear();
        void RebuildTree(IList<Element3D> items);
        void RemoveItem(Element3D item);
        void RequestRebuild();
    }
}
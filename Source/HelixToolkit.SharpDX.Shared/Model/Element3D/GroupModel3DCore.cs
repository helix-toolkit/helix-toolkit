/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    public class GroupModel3DCore : Element3DCore
    {
        public readonly IList<Element3DCore> Children = new List<Element3DCore>();

        public void Add(Element3DCore child)
        {
            Children.Add(child);
        }

        public void Remove(Element3DCore child)
        {
            Children.Remove(child);
        }
    }
}

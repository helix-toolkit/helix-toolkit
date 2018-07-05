/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelixToolkit.UWP
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.UWP.GroupElement3D" />
    /// <seealso cref="HelixToolkit.UWP.IHitable" />
    public class GroupModel3D : GroupElement3D, IHitable
    {
        public GroupModel3D()
        {
            this.DefaultStyleKey = typeof(GroupModel3D);
        }
    }
}

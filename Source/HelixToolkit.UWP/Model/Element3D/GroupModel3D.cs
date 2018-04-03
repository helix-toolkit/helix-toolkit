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
    [TemplatePart(Name = "PART_ItemsContainer", Type = typeof(ItemsControl))]
    public class GroupModel3D : GroupElement3D, IHitable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupModel3D"/> class.
        /// </summary>
        public GroupModel3D()
        {
            this.DefaultStyleKey = typeof(GroupModel3D);
        }
    }
}

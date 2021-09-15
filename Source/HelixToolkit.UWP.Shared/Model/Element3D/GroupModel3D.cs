/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if WINUI
using HelixToolkit.SharpDX.Core;
namespace HelixToolkit.WinUI
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelixToolkit.UWP
#endif
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
        }
    }
}

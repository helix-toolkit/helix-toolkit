// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Indicates, if this element should be hit-tested.        
//   default is true
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
#if COREWPF
using HelixToolkit.SharpDX.Core;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
    public interface ITraversable
    {
        IList<ITraversable> Items
        {
            get;
        }
    }

    public interface IVisible
    {
        Visibility Visibility
        {
            get; set;
        }
    }

    public interface ITransformable : ITransform
    {
        Transform3D Transform
        {
            get; set;
        }
    }

    public interface ISelectable
    {

        bool IsSelected
        {
            get; set;
        }
    }

    public interface IMouse3D
    {
        event RoutedEventHandler MouseDown3D;
        event RoutedEventHandler MouseUp3D;
        event RoutedEventHandler MouseMove3D;
    }
}
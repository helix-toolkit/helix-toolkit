// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Indicates, if this element should be hit-tested.        
//   default is true
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

    using Matrix = global::SharpDX.Matrix;
    using System.Windows.Media.Imaging;
    using HelixToolkit.Wpf.SharpDX.Core;
    using System.IO;
    using System;

    public interface ITraversable
    {
        IList<ITraversable> Items { get; }
    }
    
    public interface IVisible
    {
        Visibility Visibility { get; set; }
    }

    public interface IThrowingShadow
    {
        bool IsThrowingShadow { get; set; }
    }

    public interface IHitable : IVisible
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context">Used to get view/projection matrices during hit test. <para>Only needs for screen space model hit test(line/point/billboard). Can be set to null for mesh geometry hit test.</para></param>
        /// <param name="ray"></param>
        /// <param name="hits"></param>
        /// <returns>Return all hitted details with distance from nearest to farest.</returns>
        bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits);
        
        //void OnMouse3DDown(object sender, RoutedEventArgs e);
        //void OnMouse3DUp(object sender, RoutedEventArgs e);
        //void OnMouse3DMove(object sender, RoutedEventArgs e);

        //event RoutedEventHandler MouseDown3D;
        //event RoutedEventHandler MouseUp3D;
        //event RoutedEventHandler MouseMove3D;

        /// <summary>
        /// Indicates, if this element should be hit-tested.        
        /// default is true
        /// </summary>
        bool IsHitTestVisible { get; set; }
    }

    public interface IBoundable : IVisible
    {
        BoundingBox Bounds { get; }
        BoundingBox BoundsWithTransform { get; }
        BoundingSphere BoundsSphere { get; }
        BoundingSphere BoundsSphereWithTransform { get; }
    }

    public interface ITransformable
    {
        void PushMatrix(Matrix matrix);
        void PopMatrix();
        Matrix ModelMatrix { get; }
        Transform3D Transform { get; set; }               
    }

    public interface ISelectable
    {

        bool IsSelected { get; set; }
    }

    public interface IMouse3D
    {
        event RoutedEventHandler MouseDown3D;
        event RoutedEventHandler MouseUp3D;
        event RoutedEventHandler MouseMove3D;
    }
}
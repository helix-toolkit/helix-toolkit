// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;
    using System.Linq;
    using System.Collections;
    using Core2D;
    using System;
    using SharpDX;
    using global::SharpDX;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement2DCollection 
    /// </summary>
    public class Canvas2D : Panel2D
    {
        #region Attached Properties
        //public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas2D),
        //    new PropertyMetadata(0.0, (d,e)=> { (d as Element2D).Position = new System.Windows.Point((double)e.NewValue, (d as Element2D).Position.Y); }));

        //public static void SetLeft(Element2D element, double value)
        //{
        //    element.SetValue(LeftProperty, value);
        //}

        //public static double GetLeft(Element2D element)
        //{
        //    return (double)element.GetValue(LeftProperty);
        //}

        //public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas2D),
        //    new PropertyMetadata(0.0, (d, e) => { (d as Element2D).Position = new System.Windows.Point((d as Element2D).Position.X, (double)e.NewValue); }));
        //public static void SetTop(Element2D element, double value)
        //{
        //    element.SetValue(TopProperty, value);
        //}

        //public static double GetTop(Element2D element)
        //{
        //    return (double)element.GetValue(TopProperty);
        //}
        #endregion
    }
}
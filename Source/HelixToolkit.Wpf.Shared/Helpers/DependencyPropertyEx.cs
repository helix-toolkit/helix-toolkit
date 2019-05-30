// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyEx.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides generic dependency property register methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;

    /// <summary>
    /// Provides generic dependency property register methods.
    /// </summary>
    public static class DependencyPropertyEx
    {
        /// <summary>
        /// Registers a dependency property with the specified name.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TOwner">The type of the owner class.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// A DependencyProperty.
        /// </returns>
        public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue) where TOwner : DependencyObject
        {
            return DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner), new FrameworkPropertyMetadata(defaultValue));
        }

        /// <summary>
        /// Registers a dependency property with the specified name.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TOwner">The type of the owner class.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>A DependencyProperty.</returns>
        public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> callback) where TOwner : DependencyObject
        {
            return DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner), new FrameworkPropertyMetadata(defaultValue, (s, e) => callback((TOwner)s, e)));
        }
    }
}
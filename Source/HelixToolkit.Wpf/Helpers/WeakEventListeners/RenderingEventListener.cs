// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderingEventListener.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Media;

    /// <summary>
    /// Provides a weak event listener that pass the CompositionTarget.Rendering event to the specified handler.
    /// </summary>
    public class RenderingEventListener : WeakEventListener<RenderingEventManager, RenderingEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingEventListener"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public RenderingEventListener(EventHandler<RenderingEventArgs> handler)
            : base(handler)
        {
        }
    }
}
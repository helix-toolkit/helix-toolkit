// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for 3D elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.WinUI
{
    using HelixToolkit.WinUI.CommonDX;

    using Microsoft.UI.Xaml;

    /// <summary>
    /// Base class for 3D elements.
    /// </summary>
    public abstract class Element3D : FrameworkElement
    {
        /// <summary>
        /// Initializes the element.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        public virtual void Initialize(DeviceManager deviceManager)
        {
        }

        /// <summary>
        /// Renders the element.
        /// </summary>
        /// <param name="render">The render.</param>
        public virtual void Render(TargetBase render)
        {
        }
    }
}
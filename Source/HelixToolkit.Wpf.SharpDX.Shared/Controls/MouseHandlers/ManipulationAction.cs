// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationAction.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Specifies constants that define actions performed by manipulation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.ComponentModel;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Specifies constants that define actions performed by manipulation.
    /// </summary>
    [TypeConverter(typeof(ManipulationActionConverter))]
    public enum ManipulationAction
    {
        None,

        Pan,

        Pinch,

        TwoFingerPan,

        ThreeFingerPan,
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationAction.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Specifies constants that define actions performed by manipulation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    /// <summary>
    /// Specifies constants that define actions performed by manipulation.
    /// </summary>
    public enum ManipulationAction
    {
        None,

        Pan,

        Pinch,

        TwoFingerPan,

        ThreeFingerPan,
    }

    public static class ManipulationActionExtensions
    {
        public static int FingerCount(this ManipulationAction manipulationAction)
        {
            switch (manipulationAction)
            {
                case ManipulationAction.Pan:
                    return 1;
                case ManipulationAction.Pinch:
                case ManipulationAction.TwoFingerPan:
                    return 2;
                case ManipulationAction.ThreeFingerPan:
                    return 3;
            }

            return 0;
        }
    }
}
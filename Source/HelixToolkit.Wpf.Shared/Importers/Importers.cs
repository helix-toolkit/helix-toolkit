// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importers.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Contains a list of all supported importers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// Contains a list of all supported importers.
    /// </summary>
    public static class Importers
    {
        /// <summary>
        /// Default file import extension.
        /// </summary>
        public static readonly string DefaultExtension = ".obj";

        /// <summary>
        /// File filter for all the supported importers.
        /// </summary>
        public static readonly string Filter =
            "All supported files|*.3ds;*.lwo;*.obj;*.objx;*.stl;*.off;*.ply|3D Studio (*.3ds)|*.3ds|Lightwave (*.lwo)|*.lwo|Wavefront (*.obj)|*.obj;*.objx|StereoLithography (*.stl)|*.stl|OFF (*.off)|*.off|PolygonFile (*.ply)|*.ply";
    }
}

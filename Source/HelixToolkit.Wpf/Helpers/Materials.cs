// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Materials.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Contains a set of predefined materials.
    /// </summary>
    public static class Materials
    {
        /// <summary>
        /// The black.
        /// </summary>
        public static Material Black = MaterialHelper.CreateMaterial(Brushes.Black);

        /// <summary>
        /// The blue.
        /// </summary>
        public static Material Blue = MaterialHelper.CreateMaterial(Brushes.Blue);

        /// <summary>
        /// The dark gray.
        /// </summary>
        public static Material DarkGray = MaterialHelper.CreateMaterial(Brushes.DarkGray);

        /// <summary>
        /// The gold.
        /// </summary>
        public static Material Gold = MaterialHelper.CreateMaterial(Brushes.Gold);

        /// <summary>
        /// The gray.
        /// </summary>
        public static Material Gray = MaterialHelper.CreateMaterial(Brushes.Gray);

        /// <summary>
        /// The green.
        /// </summary>
        public static Material Green = MaterialHelper.CreateMaterial(Brushes.Green);

        /// <summary>
        /// The hue.
        /// </summary>
        public static Material Hue = MaterialHelper.CreateMaterial(BrushHelper.CreateHsvBrush(1.0));

        /// <summary>
        /// The light gray.
        /// </summary>
        public static Material LightGray = MaterialHelper.CreateMaterial(Brushes.LightGray);

        /// <summary>
        /// The rainbow.
        /// </summary>
        public static Material Rainbow = MaterialHelper.CreateMaterial(BrushHelper.CreateRainbowBrush());

        /// <summary>
        /// The red.
        /// </summary>
        public static Material Red = MaterialHelper.CreateMaterial(Brushes.Red);

        /// <summary>
        /// The white.
        /// </summary>
        public static Material White = MaterialHelper.CreateMaterial(Brushes.White);

        /// <summary>
        /// The yellow.
        /// </summary>
        public static Material Yellow = MaterialHelper.CreateMaterial(Brushes.Yellow);

        /// <summary>
        /// The brown.
        /// </summary>
        public static Material Brown = MaterialHelper.CreateMaterial(Brushes.Brown);

    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultLights.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that contains a light model containing 4 directional lights.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a light model containing 4 directional lights.
    /// </summary>
    public class DefaultLights : LightSetup
    {
        /// <summary>
        /// Adds the lights to the element.
        /// </summary>
        /// <param name="lightGroup">
        /// The light group.
        /// </param>
        protected override void AddLights(Model3DGroup lightGroup)
        {
            // key light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(180, 180, 180), new Vector3D(-1, -1, -1)));

            // fill light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(120, 120, 120), new Vector3D(1, -1, -0.1)));

            // rim/back light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(60, 60, 60), new Vector3D(0.1, 1, -1)));

            // and a little bit from below
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(50, 50, 50), new Vector3D(0.1, 0.1, 1)));

            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(30, 30, 30)));
        }
    }
}
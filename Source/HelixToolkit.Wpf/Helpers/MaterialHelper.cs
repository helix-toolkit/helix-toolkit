// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Creates diffuse/specular materials.
    /// </summary>
    public static class MaterialHelper
    {
        /// <summary>
        /// Changes the opacity of a material.
        /// </summary>
        /// <param name="material">
        /// The material.
        /// </param>
        /// <param name="d">
        /// The d.
        /// </param>
        public static void ChangeOpacity(Material material, double d)
        {
            var mg = material as MaterialGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    ChangeOpacity(m, d);
                }
            }

            var dm = material as DiffuseMaterial;
            if (dm != null && dm.Brush != null)
            {
                dm.Brush.Opacity = d;
            }
        }

        /// <summary>
        /// Creates a material from the specified bitmap file.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="uriKind">Kind of the URI.</param>
        /// <returns>The image material (texture).</returns>
        public static Material CreateImageMaterial(string uri, double opacity = 1.0, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            var image = GetImage(uri, uriKind);
            if (image == null)
            {
                return null;
            }

            return CreateImageMaterial(image, opacity);
        }

        /// <summary>
        /// Creates an emissive image material.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="diffuseBrush">The diffuse brush.</param>
        /// <param name="uriKind">Kind of the URI.</param>
        /// <returns>The image material.</returns>
        public static Material CreateEmissiveImageMaterial(string uri, Brush diffuseBrush, UriKind uriKind)
        {
            var image = GetImage(uri, uriKind);
            if (image == null)
            {
                return null;
            }

            return CreateEmissiveImageMaterial(image, diffuseBrush);
        }

        /// <summary>
        /// Creates a material from the specified image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="opacity">
        /// The opacity value.
        /// </param>
        /// <returns>
        /// The image material.
        /// </returns>
        public static Material CreateImageMaterial(BitmapImage image, double opacity)
        {
            var brush = new ImageBrush(image) { Opacity = opacity };
            return new DiffuseMaterial(brush);
        }

        /// <summary>
        /// Creates an emissive material from the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="diffuseBrush">The diffuse brush.</param>
        /// <returns>The image material</returns>
        public static Material CreateEmissiveImageMaterial(BitmapImage image, Brush diffuseBrush)
        {
            var brush = new ImageBrush(image);
            var em = new EmissiveMaterial(brush);
            var dm = new DiffuseMaterial(diffuseBrush);
            var mg = new MaterialGroup();
            mg.Children.Add(dm);
            mg.Children.Add(em);
            return mg; // new DiffuseMaterial(brush);
        }

        /// <summary>
        /// Creates a material for the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(Color color)
        {
            return CreateMaterial(new SolidColorBrush(color));
        }

        /// <summary>
        /// Creates a material for the specified color and opacity.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(Color color, double opacity)
        {
            return CreateMaterial(Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B));
        }

        /// <summary>
        /// Creates a material with the specifed brush as diffuse material.
        /// This method will also add a white specular material.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="specularPower">
        /// The specular power.
        /// </param>
        /// <returns>
        /// The material.
        /// </returns>
        public static Material CreateMaterial(Brush brush, double specularPower = 100)
        {
            var mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(brush));
            if (specularPower > 0)
            {
                mg.Children.Add(new SpecularMaterial(Brushes.White, specularPower));
            }

            return mg;
        }

        /// <summary>
        /// Creates a material with the specified diffuse, emissive and specular brushes.
        /// </summary>
        /// <param name="diffuse">The diffuse color.</param>
        /// <param name="emissive">The emissive color.</param>
        /// <param name="specular">The specular color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(
            Brush diffuse, Brush emissive, Brush specular = null, double opacity = 1, double specularPower = 85)
        {
            var mg = new MaterialGroup();
            if (diffuse != null)
            {
                diffuse = diffuse.Clone();
                diffuse.Opacity = opacity;
                mg.Children.Add(new DiffuseMaterial(diffuse));
            }

            if (emissive != null)
            {
                emissive = emissive.Clone();
                emissive.Opacity = opacity;
                mg.Children.Add(new EmissiveMaterial(emissive));
            }

            if (specular != null)
            {
                specular = specular.Clone();
                specular.Opacity = opacity;
                mg.Children.Add(new SpecularMaterial(specular, specularPower));
            }

            return mg;
        }

        /// <summary>
        /// Gets the first material of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of material</typeparam>
        /// <param name="material">The source material.</param>
        /// <returns>The first material.</returns>
        public static T GetFirst<T>(Material material) where T : Material
        {
            if (material.GetType() == typeof(T))
            {
                return (T)material;
            }

            var mg = material as MaterialGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    var result = GetFirst<T>(m);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the image from the specified uri.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="uriKind">Specifies whether the uri string is relative or absolute.</param>
        /// <returns>
        /// The image.
        /// </returns>
        private static BitmapImage GetImage(string uri, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(uri, uriKind);
            image.EndInit();
            return image;
        }
    }
}
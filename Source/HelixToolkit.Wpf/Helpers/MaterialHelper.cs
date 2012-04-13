// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Creates diffuse/specular materials.
    /// </summary>
    public static class MaterialHelper
    {
        #region Constants and Fields

        /// <summary>
        ///   The default specular power.
        /// </summary>
        public static double DefaultSpecularPower = 100;

        #endregion

        #region Public Methods

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
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateImageMaterial(string path)
        {
            return CreateImageMaterial(path, 1);
        }

        /// <summary>
        /// Creates a material from the specified bitmap file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="opacity">
        /// The opacity.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateImageMaterial(string path, double opacity)
        {
            var image = GetImage(path);
            if (image == null) return null;
            return CreateImageMaterial(image, opacity);
        }

        /// <summary>
        /// Creates an emissive image material.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="diffuseBrush">The diffuse brush.</param>
        /// <returns>The image material.</returns>
        public static Material CreateEmissiveImageMaterial(string path, Brush diffuseBrush)
        {
            var image = GetImage(path);
            if (image == null) return null;
            return CreateEmissiveImageMaterial(image, diffuseBrush);
        }

        /// <summary>
        /// Gets the image from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The image.</returns>
        private static BitmapImage GetImage(string path)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fullPath);
            image.EndInit();
            return image;
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
        /// <param name="color">
        /// The color.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateMaterial(Color color)
        {
            return CreateMaterial(new SolidColorBrush(color));
        }

        /// <summary>
        /// Creates a material for the specified color and opacity.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="opacity">
        /// The opacity.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateMaterial(Color color, double opacity)
        {
            return CreateMaterial(Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B));
        }

        /// <summary>
        /// Creates a material for the specified brush.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateMaterial(Brush brush)
        {
            return CreateMaterial(brush, DefaultSpecularPower);
        }

        /// <summary>
        /// Creates a material with the specifed brush as diffuse material. 
        ///   This method will also add a white specular material.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="specularPower">
        /// The specular power.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateMaterial(Brush brush, double specularPower)
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
        /// <param name="diffuse">
        /// The diffuse.
        /// </param>
        /// <param name="emissive">
        /// The emissive.
        /// </param>
        /// <param name="specular">
        /// The specular.
        /// </param>
        /// <param name="opacity">
        /// The opacity.
        /// </param>
        /// <param name="specularPower">
        /// The specular power.
        /// </param>
        /// <returns>
        /// </returns>
        public static Material CreateMaterial(
            Brush diffuse, Brush emissive = null, Brush specular = null, double opacity = 1, double specularPower = 85)
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

        #endregion

        /// <summary>
        /// Gets the first material of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of material</typeparam>
        /// <param name="material">The source material.</param>
        /// <returns></returns>
        public static T GetFirst<T>(Material material) where T : Material
        {
            if (material.GetType() == typeof(T))
                return (T)material;

            var mg = material as MaterialGroup;
            if (mg != null)
                foreach (var m in mg.Children)
                {
                    var result = GetFirst<T>(m);
                    if (result != null) return result;
                }
            return null;
        }


    }
}
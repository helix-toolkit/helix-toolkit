using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides methods that creates materials.
/// </summary>
public static class MaterialHelper
{
    /// <summary>
    /// Changes the opacity of a material.
    /// </summary>
    /// <param name="material">The material.</param>
    /// <param name="opacity">The new opacity.</param>
    /// <remarks>The method will traverse children of <see cref="MaterialGroup" /> and change the opacity of all <see cref="DiffuseMaterial" /> objects.
    /// Remember that the material must not be frozen.</remarks>
    public static void ChangeOpacity(Material material, double opacity)
    {
        if (material is MaterialGroup mg)
        {
            foreach (var m in mg.Children)
            {
                ChangeOpacity(m, opacity);
            }
        }

        if (material is DiffuseMaterial dm && dm.Brush != null)
        {
            dm.Brush.Opacity = opacity;
        }
    }

    /// <summary>
    /// Creates a material from the specified bitmap file.
    /// </summary>
    /// <param name="uri">The uri.</param>
    /// <param name="opacity">The opacity.</param>
    /// <param name="uriKind">Kind of the URI.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material (texture).</returns>
    public static Material CreateImageMaterial(string uri, double opacity = 1.0, UriKind uriKind = UriKind.RelativeOrAbsolute, bool freeze = true)
    {
        var image = GetImage(uri, uriKind);
        return CreateImageMaterial(image, opacity, freeze: freeze);
    }

    /// <summary>
    /// Creates a scaled material version from the specified bitmap file.
    /// </summary>
    /// <param name="uri">The uri.</param>
    /// <param name="scaleW">Horizontal image scale.</param>
    /// <param name="scaleH">Vertical image scale.</param>
    /// <param name="opacity">The opacity.</param>
    /// <param name="uriKind">Kind of the URI.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material (texture).</returns>
    public static Material CreateImageMaterial(string uri, double scaleW, double scaleH, double opacity = 1.0, UriKind uriKind = UriKind.RelativeOrAbsolute, bool freeze = true)
    {
        var image = GetImage(uri, uriKind);
        return CreateTiledImageMaterial(image, opacity, scaleW, scaleH, freeze: freeze);
    }

    /// <summary>
    /// Creates a material from the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="opacity">The opacity value.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material.</returns>
    public static Material CreateImageMaterial(BitmapImage image, double opacity, bool freeze = true)
    {
        var brush = new ImageBrush(image) { Opacity = opacity };
        var material = new DiffuseMaterial(brush);
        if (freeze)
        {
            material.Freeze();
        }

        return material;
    }

    /// <summary>
    /// Creates a tiled material from the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="scaleW">Horizontal image scale.</param>
    /// <param name="scaleH">Vertical image scale.</param>
    /// <param name="opacity">The opacity value.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material.</returns>
    public static Material CreateTiledImageMaterial(BitmapImage image, double opacity, double scaleW, double scaleH, bool freeze = true)
    {
        var brush = new ImageBrush(image)
        {
            Opacity = opacity,
            TileMode = TileMode.Tile,
            Stretch = Stretch.Uniform,
            Transform = new ScaleTransform(scaleW, scaleH)
        };

        var material = new DiffuseMaterial(brush);

        if (freeze)
        {
            material.Freeze();
        }

        return material;
    }

    /// <summary>
    /// Creates an emissive image material.
    /// </summary>
    /// <param name="uri">The uri of the image.</param>
    /// <param name="diffuseBrush">The diffuse brush.</param>
    /// <param name="uriKind">Kind of the <paramref name="uri" />.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material.</returns>
    public static Material? CreateEmissiveImageMaterial(string uri, Brush diffuseBrush, UriKind uriKind, bool freeze = true)
    {
        var image = GetImage(uri, uriKind);
        if (image == null)
        {
            return null;
        }

        return CreateEmissiveImageMaterial(image, diffuseBrush, freeze: freeze);
    }

    /// <summary>
    /// Creates an emissive material from the specified image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="diffuseBrush">The diffuse brush.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The image material</returns>
    public static Material CreateEmissiveImageMaterial(BitmapImage image, Brush diffuseBrush, bool freeze = true)
    {
        var brush = new ImageBrush(image);
        var em = new EmissiveMaterial(brush);
        var dm = new DiffuseMaterial(diffuseBrush);
        var mg = new MaterialGroup();
        mg.Children.Add(dm);
        mg.Children.Add(em);
        if (freeze)
        {
            mg.Freeze();
        }

        return mg;
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
    /// Creates a material with the specified brush as diffuse material.
    /// This method will also add a white specular material.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="specularPower">The specular power.</param>
    /// <param name="ambient">The ambient component.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>
    /// The material.
    /// </returns>
    public static Material CreateMaterial(
        Brush brush,
        double specularPower = 100,
        byte ambient = 255,
        bool freeze = true)
    {
        return CreateMaterial(brush, 1d, specularPower, ambient, freeze);
    }

    /// <summary>
    /// Creates a material with the specified brush as diffuse material.
    /// This method will also add a white specular material.
    /// </summary>
    /// <param name="brush">The brush of the diffuse material.</param>
    /// <param name="specularBrightness">The brightness of the specular material.</param>
    /// <param name="specularPower">The specular power.</param>
    /// <param name="ambient">The ambient component.</param>
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>
    /// The material.
    /// </returns>
    public static Material CreateMaterial(Brush brush, double specularBrightness, double specularPower = 100, byte ambient = 255, bool freeze = true)
    {
        var mg = new MaterialGroup();
        mg.Children.Add(new DiffuseMaterial(brush) { AmbientColor = Color.FromRgb(ambient, ambient, ambient) });
        if (specularPower > 0)
        {
            var b = (byte)(255 * specularBrightness);
            mg.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromRgb(b, b, b)), specularPower));
        }

        if (freeze && mg.CanFreeze)
        {
            mg.Freeze();
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
    /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
    /// <returns>The material.</returns>
    public static Material CreateMaterial(Brush? diffuse, Brush? emissive, Brush? specular = null, double opacity = 1, double specularPower = 85, bool freeze = true)
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

        if (freeze)
        {
            mg.Freeze();
        }

        return mg;
    }

    /// <summary>
    /// Gets the first material of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of material</typeparam>
    /// <param name="material">The source material.</param>
    /// <returns>The first material of the specified type.</returns>
    public static T? GetFirst<T>(Material material) where T : Material
    {
        if (material.GetType() == typeof(T))
        {
            return (T)material;
        }

        if (material is MaterialGroup mg)
        {
            return mg.Children.Select(GetFirst<T>).FirstOrDefault(m => m != null);
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

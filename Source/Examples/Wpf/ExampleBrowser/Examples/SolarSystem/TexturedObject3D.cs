using DependencyPropertyGenerator;
using HelixToolkit.Wpf;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SolarSystem;

[DependencyProperty<string>("ObjectName", OnChanged = nameof(UpdateTexture))]
public partial class TexturedObject3D : ModelVisual3D
{
    public ImageBrush? Texture { get; private set; }

    public SphereVisual3D Sphere { get; private set; }

    public double MeanRadius { get; set; }

    public TexturedObject3D()
    {
        Sphere = new SphereVisual3D() { ThetaDiv = 60, PhiDiv = 30 };
        Children.Add(Sphere);
    }

    private static readonly Assembly asm = Assembly.GetExecutingAssembly();

    private BitmapImage GetTextureFromResource(string name)
    {
        var img = new BitmapImage();
        img.BeginInit();
        img.UriSource = new Uri("pack://application:,,/Examples/SolarSystem/Textures/" + name + ".jpg");
        img.EndInit();
        return img;
    }

    private static BitmapImage GetTextureFromFile(string name)
    {
        var texture = "textures/" + name + ".jpg";
        return new BitmapImage(new Uri(texture, UriKind.Relative));
    }
    private void UpdateTexture()
    {
        if (string.IsNullOrEmpty(ObjectName))
        {
            return;
        }

        var img = GetTextureFromResource(ObjectName!);
        Texture = new ImageBrush(img);
        if (ObjectName == "Sun")
            Sphere.Material = MaterialHelper.CreateMaterial(Brushes.Black, Texture, Brushes.Gray, 1.0, 20);
        else
            Sphere.Material = MaterialHelper.CreateMaterial(Texture);
    }
}

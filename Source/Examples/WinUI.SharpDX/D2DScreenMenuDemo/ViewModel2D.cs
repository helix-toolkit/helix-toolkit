using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;
using Media = Microsoft.UI.Xaml.Media;

namespace D2DScreenMenuDemo;

public partial class ViewModel2D : ObservableObject
{
    [ObservableProperty]
    private Media.Transform textTransform = new Media.MatrixTransform { Matrix = Media.Matrix.Identity };

    [ObservableProperty]
    private string text = "Text Model 2D";

    public Stream ImageStream
    {
        private set; get;
    }

    private const string Texture = @"TextureCheckerboard2.jpg";

    public ViewModel2D()
    {
        //TextTransform = new Media.RotateTransform { Angle = 45, CenterX = 100, CenterY = 0 };
        TextTransform = CreateAnimatedTransform2(8);
        ImageStream = LoadFileToMemory(Texture);
    }

    public static MemoryStream LoadFileToMemory(string filePath)
    {
        filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, filePath);

        using var file = new FileStream(filePath, FileMode.Open);
        var memory = new MemoryStream();
        file.CopyTo(memory);
        return memory;
    }

    private Media.Transform CreateAnimatedTransform2(double speed = 4)
    {
        var lightTrafo = new Media.TransformGroup();
        var rotateAnimation = new Media.Animation.DoubleAnimation
        {
            RepeatBehavior = Media.Animation.RepeatBehavior.Forever,
            By = 360,
            AutoReverse = false,
            Duration = TimeSpan.FromSeconds(speed / 4),
        };

        var rotateTransform = new Media.RotateTransform { Angle = 0, CenterX = 0, CenterY = 0 };
        Media.Animation.Storyboard.SetTarget(rotateAnimation, rotateTransform);
        Media.Animation.Storyboard.SetTargetProperty(rotateAnimation, "Angle");
        lightTrafo.Children.Add(rotateTransform);

        var scaleAnimation = new Media.Animation.DoubleAnimation
        {
            RepeatBehavior = Media.Animation.RepeatBehavior.Forever,
            From = 1,
            To = 5,
            AutoReverse = true,
            Duration = TimeSpan.FromSeconds(speed / 4),
        };
        var scaleTransform = new Media.ScaleTransform { ScaleX = 1, ScaleY = 1, CenterX = 0, CenterY = 0 };
        Media.Animation.Storyboard.SetTarget(scaleAnimation, scaleTransform);
        Media.Animation.Storyboard.SetTargetProperty(scaleAnimation, "ScaleX");
        Media.Animation.Storyboard.SetTarget(scaleAnimation, scaleTransform);
        Media.Animation.Storyboard.SetTargetProperty(scaleAnimation, "ScaleY");
        lightTrafo.Children.Add(scaleTransform);
        return lightTrafo;
    }
}

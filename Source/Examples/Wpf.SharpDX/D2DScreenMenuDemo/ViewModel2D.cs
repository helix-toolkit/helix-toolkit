using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using Media = System.Windows.Media;

namespace D2DScreenMenuDemo;

public partial class ViewModel2D : ObservableObject
{
    [ObservableProperty]
    private Media.Transform textTransform = Media.Transform.Identity;

    [ObservableProperty]
    private string text = "Text Model 2D";

    public Stream ImageStream
    {
        private set; get;
    }

    private const string Texture = @"TextureCheckerboard2.jpg";

    public ViewModel2D()
    {
        //TextTransform = new Media.RotateTransform(45, 100, 0);
        TextTransform = CreateAnimatedTransform2(8);
        ImageStream = LoadFileToMemory(Texture);
    }

    public static MemoryStream LoadFileToMemory(string filePath)
    {
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

        var rotateTransform = new Media.RotateTransform(0, 0, 0);
        rotateTransform.BeginAnimation(Media.RotateTransform.AngleProperty, rotateAnimation);
        lightTrafo.Children.Add(rotateTransform);

        var scaleAnimation = new Media.Animation.DoubleAnimation
        {
            RepeatBehavior = Media.Animation.RepeatBehavior.Forever,
            From = 1,
            To = 5,
            AutoReverse = true,
            Duration = TimeSpan.FromSeconds(speed / 4),
        };
        var scaleTransform = new Media.ScaleTransform(1, 1, 0, 0);
        scaleTransform.BeginAnimation(Media.ScaleTransform.ScaleXProperty, scaleAnimation);
        scaleTransform.BeginAnimation(Media.ScaleTransform.ScaleYProperty, scaleAnimation);
        lightTrafo.Children.Add(scaleTransform);
        return lightTrafo;
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TexturedObject3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using HelixToolkit.Wpf;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;

namespace SolarsystemDemo
{
    public class TexturedObject3D : ModelVisual3D
    {
        public ImageBrush Texture { get; private set; }
        public SphereVisual3D Sphere { get; private set; }

        public string ObjectName
        {
            get { return (string)GetValue(ObjectNameProperty); }
            set { SetValue(ObjectNameProperty, value); UpdateTexture(); }
        }

        public static readonly DependencyProperty ObjectNameProperty =
            DependencyProperty.Register("ObjectName", typeof(string), typeof(Satellite3D), new UIPropertyMetadata(null, NameChanged));

        protected static void NameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((TexturedObject3D)obj).UpdateTexture();
        }
        public double MeanRadius { get; set; }

        public TexturedObject3D()
        {
            Sphere = new SphereVisual3D() { ThetaDiv = 60, PhiDiv = 30 };
            Children.Add(Sphere);
        }

        private static Assembly asm = Assembly.GetExecutingAssembly();

        BitmapImage GetTextureFromResource(string name)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,/Examples/SolarSystemDemo/Textures/"+name+".jpg");
            img.EndInit();
            return img;
        }
        private static BitmapImage GetTextureFromFile(string name)
        {
            var texture = "textures/" + name + ".jpg";
            return new BitmapImage(new Uri(texture, UriKind.Relative));
        }
        void UpdateTexture()
        {
            var img = GetTextureFromResource(ObjectName );
            Texture = new ImageBrush(img);
            if (ObjectName == "Sun")
                Sphere.Material = MaterialHelper.CreateMaterial(Brushes.Black, Texture, Brushes.Gray, 1.0, 20);
            else
                Sphere.Material = MaterialHelper.CreateMaterial(Texture);
        }

    }
}
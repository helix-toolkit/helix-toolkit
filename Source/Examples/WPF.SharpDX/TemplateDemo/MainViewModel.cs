namespace TemplateDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel
    {
        public IList<Shape> Items { get; set; }         
        public MainViewModel()
        {
            this.Items = new ObservableCollection<Shape>
                             {
                                 new Sphere
                                     {
                                         Transform = new TranslateTransform3D(0, 0, 0),
                                         Material = PhongMaterials.Red
                                     },
                                 new Cube
                                     {
                                         Transform = new TranslateTransform3D(1, 0, 0),
                                         Material = PhongMaterials.Green
                                     },
                                 new Cube
                                     {
                                         Transform = new TranslateTransform3D(-1, 0, 0),
                                         Material = PhongMaterials.Blue
                                     }
                             };
        }
    }
}
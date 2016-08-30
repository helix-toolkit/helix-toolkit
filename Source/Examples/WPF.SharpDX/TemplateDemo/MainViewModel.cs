// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TemplateDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel
    {
        public IList<Shape> Items { get; set; }   
        
        public DefaultEffectsManager EffectsManager { get; private set; }      

        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }
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

            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
        }
    }
}
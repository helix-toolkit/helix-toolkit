// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MorphTargetAnimationDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Media3D;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;

    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<SelectionViewModel> ViewModels { get; } = new ObservableCollection<SelectionViewModel>();
        private SelectionViewModel selectedViewModel = null;
        public SelectionViewModel SelectedViewModel
        {
            set
            {
                SetValue(ref selectedViewModel, value);
            }
            get { return selectedViewModel; }
        }

        private PhongMaterialCollection materials = new PhongMaterialCollection();

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            CreateViewModels();
        }

        private void CreateViewModels()
        {
            var vm = new SelectionViewModel(nameof(Sphere));
            for(int i=0; i<10; ++i)
            {
                vm.Items.Add(new Sphere() { Transform = new TranslateTransform3D(0, i, 0), Material = materials[i] });
            }
            ViewModels.Add(vm);

            vm = new SelectionViewModel(nameof(Cube));
            for (int i = 0; i < 10; ++i)
            {
                vm.Items.Add(new Cube() { Transform = new TranslateTransform3D(i, i, 0), Material = materials[i] });
            }
            ViewModels.Add(vm);
        }
    }

    public class SelectionViewModel
    {
        public string Name { private set; get; }
        public ObservableCollection<Shape> Items { get; } = new ObservableCollection<Shape>();

        public SelectionViewModel(string name)
        {
            Name = name;
        }
    }
}
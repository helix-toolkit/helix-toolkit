using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OffScreenRendering
{
    internal class MainWindowViewModel : ObservableObject
    {
        private ImageSource image;
        public ImageSource Image { get => image; set => SetProperty(ref image, value); }

        public ICommand RenderCommand { get; }

        private readonly Renderer renderer = new Renderer();

        public MainWindowViewModel()
        {
            RenderCommand = new RelayCommand(() =>
            {
                renderer.Resize(1024, 768);
                Task.Run(() =>
                {
                    return renderer.Render();
                }).ContinueWith((result) => { Image = result.Result; },
                TaskScheduler.FromCurrentSynchronizationContext());
            });
        }
    }
}

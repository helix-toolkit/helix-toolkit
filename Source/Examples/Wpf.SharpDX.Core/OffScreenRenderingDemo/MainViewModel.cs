using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OffScreenRenderingDemo;

public partial class MainViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private ImageSource? image;

    private readonly Renderer? renderer = new();

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void Render()
    {
        if (renderer is null)
        {
            return;
        }

        renderer.Resize(1024, 768);

        Task.Run(() =>
        {
            return renderer.Render();
        }).ContinueWith((result) =>
        {
            Image = result.Result;
        },
        TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void Dispose()
    {
        renderer?.Dispose();
    }
}

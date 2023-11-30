using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;

namespace ModelViewer.Services;

public sealed class FilePickerService
{
    public async Task<string> StartFilePicker(string filters)
    {
        var tokens = filters.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return await StartFilePicker(tokens);
    }

    public async Task<string> StartFilePicker(string[] filters)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        // Pass in the current WinUI window and get its handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.GetService<Window>());
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        foreach (var filter in filters)
        {
            if (filter == ".mesh.xml") { continue; }
            picker.FileTypeFilter.Add(filter);
        }
        return await picker.PickSingleFileAsync().AsTask().ContinueWith((result) => {
            return result is null || result.Result is null ? string.Empty : result.Result.Path;
        });
    }
}

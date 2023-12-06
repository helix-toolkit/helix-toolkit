using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;
using Windows.Storage.Pickers;

namespace ModelViewerDemo;

public sealed class FilePickerService
{
    public static async Task<string> StartFilePicker(object? target, string filters)
    {
        var tokens = filters.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return await StartFilePicker(target, tokens);
    }

    public static async Task<string> StartFilePicker(object? target, string[] filters)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        // Pass in the current WinUI window and get its handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(target);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        foreach (var filter in filters)
        {
            if (filter == ".mesh.xml") { continue; }
            picker.FileTypeFilter.Add(filter);
        }

        return await picker.PickSingleFileAsync().AsTask().ContinueWith((result) =>
        {
            return result?.Result is null ? string.Empty : result.Result.Path;
        });
    }
}

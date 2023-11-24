using Microsoft.UI.Xaml.Markup;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// 
/// </summary>
public class ViewportCommandExtension : MarkupExtension
{
    private readonly ViewportCommand? value;

    public ViewportCommandExtension(ViewportCommand? value)
    {
        this.value = value;
    }

    protected override object? ProvideValue()
    {
        return this.value;
    }
}

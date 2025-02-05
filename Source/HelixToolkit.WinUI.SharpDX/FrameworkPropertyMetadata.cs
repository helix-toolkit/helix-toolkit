namespace HelixToolkit.WinUI.SharpDX;

public class FrameworkPropertyMetadata : PropertyMetadata
{
    public FrameworkPropertyMetadata(object? defaultValue)
        : base(defaultValue)
    {
    }

    public FrameworkPropertyMetadata(object? defaultValue, PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
    }

    public FrameworkPropertyMetadata(object? defaultValue, FrameworkPropertyMetadataOptions flags)
        : base(defaultValue)
    {
    }

    public FrameworkPropertyMetadata(object? defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback)
        : base(defaultValue, propertyChangedCallback)
    {
    }
}

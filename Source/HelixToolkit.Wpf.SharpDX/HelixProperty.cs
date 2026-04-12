using System.Windows;

namespace HelixToolkit.Wpf.SharpDX;

public static class HelixProperty
{
    public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue, bool isTwoWayBinding)
        where TOwner : DependencyObject
    {
        PropertyMetadata metadata;

        if (isTwoWayBinding)
        {
            metadata = new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
        }
        else
        {
            metadata = new FrameworkPropertyMetadata(defaultValue);
        }

        DependencyProperty property = DependencyProperty.Register(
                name,
                typeof(TValue),
                typeof(TOwner),
                metadata);

        return property;
    }

    public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue = default!, PropertyChangedCallback? changeCallback = null)
        where TOwner : DependencyObject
    {
        PropertyMetadata metadata;

        if (changeCallback is null)
        {
            metadata = new PropertyMetadata(defaultValue);
        }
        else
        {
            metadata = new PropertyMetadata(defaultValue, changeCallback);
        }

        DependencyProperty property = DependencyProperty.Register(
                name,
                typeof(TValue),
                typeof(TOwner),
                metadata);

        return property;
    }

    public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue = default!, PropertyChangedCallback? changeCallback = null)
        where TOwner : DependencyObject
    {
        PropertyMetadata metadata;

        if (changeCallback is null)
        {
            metadata = new PropertyMetadata(defaultValue);
        }
        else
        {
            metadata = new PropertyMetadata(defaultValue, changeCallback);
        }

        DependencyProperty property = DependencyProperty.RegisterAttached(
                name,
                typeof(TValue),
                typeof(TOwner),
                metadata);

        return property;
    }
}

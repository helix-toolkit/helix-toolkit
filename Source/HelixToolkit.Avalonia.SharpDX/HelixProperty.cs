using Avalonia;
using Avalonia.Data;

namespace HelixToolkit.Avalonia.SharpDX;

#pragma warning disable AVP1001

public static class HelixProperty
{
    public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue, bool isTwoWayBinding)
        where TOwner : AvaloniaObject
    {
        DependencyProperty property;

        if (isTwoWayBinding)
        {
            property = AvaloniaProperty.Register<TOwner, TValue>(name, defaultValue: defaultValue, defaultBindingMode: BindingMode.TwoWay);
        }
        else
        {
            property = AvaloniaProperty.Register<TOwner, TValue>(name, defaultValue: defaultValue);
        }

        return property;
    }

    public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue = default!, Action<TOwner, AvaloniaPropertyChangedEventArgs>? changeCallback = null)
        where TOwner : AvaloniaObject
    {
        DependencyProperty property = AvaloniaProperty.Register<TOwner, TValue>(name, defaultValue: defaultValue, defaultBindingMode: BindingMode.Default);

        if (changeCallback is not null)
        {
            property.Changed.AddClassHandler<TOwner>(changeCallback);
        }

        return property;
    }

    public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue = default!, Action<TOwner, AvaloniaPropertyChangedEventArgs>? changeCallback = null)
        where TOwner : AvaloniaObject
    {
        DependencyProperty property = AvaloniaProperty.RegisterAttached<TOwner, AvaloniaObject, TValue>(name, defaultValue: defaultValue);

        if (changeCallback is not null)
        {
            property.Changed.AddClassHandler<TOwner>(changeCallback);
        }

        return property;
    }
}

#pragma warning restore AVP1001

using Avalonia.Labs.Input;

namespace HelixToolkit.Avalonia.SharpDX;

public static class ViewportCommands
{
    public static RoutedCommand Zoom { get; } = new(nameof(Zoom));

    public static RoutedCommand ZoomExtents { get; } = new(nameof(ZoomExtents));

    public static RoutedCommand ZoomRectangle { get; } = new(nameof(ZoomRectangle));

    public static RoutedCommand Pan { get; } = new(nameof(Pan));

    public static RoutedCommand Rotate { get; } = new(nameof(Rotate));

    public static RoutedCommand SetTarget { get; } = new(nameof(SetTarget));

    public static RoutedCommand Reset { get; } = new(nameof(Reset));

    public static RoutedCommand ChangeFieldOfView { get; } = new(nameof(ChangeFieldOfView));

    public static RoutedCommand BackView { get; } = new(nameof(BackView));

    public static RoutedCommand FrontView { get; } = new(nameof(FrontView));

    public static RoutedCommand TopView { get; } = new(nameof(TopView));

    public static RoutedCommand BottomView { get; } = new(nameof(BottomView));

    public static RoutedCommand LeftView { get; } = new(nameof(LeftView));

    public static RoutedCommand RightView { get; } = new(nameof(RightView));
}

namespace HelixToolkit.Wpf.SharpDX.Controls;

public sealed class DX11ImageSourceArgs : EventArgs
{
    public readonly DX11ImageSource Source;
    public DX11ImageSourceArgs(DX11ImageSource source)
    {
        Source = source;
    }
}

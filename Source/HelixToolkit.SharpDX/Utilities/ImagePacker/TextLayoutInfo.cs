using SharpDX.DirectWrite;
using SharpDX;

namespace HelixToolkit.SharpDX.Utilities.ImagePacker;

public sealed class TextLayoutInfo : IDisposable
{
    public readonly TextLayout TextLayout;
    public readonly Color4 Foreground;
    public readonly Color4 Background;
    public readonly Vector4 Padding;
    public TextLayoutInfo(TextLayout layout, Color4 foreground, Color4 background, Vector4 padding)
    {
        TextLayout = layout;
        Foreground = foreground;
        Background = background;
        Padding = padding;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            TextLayout.Dispose();
            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ImagePacker() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}

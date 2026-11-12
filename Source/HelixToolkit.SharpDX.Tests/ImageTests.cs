using NUnit.Framework;
using SharpDX.Toolkit.Graphics;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class ImageTests
{
    [TestCase(2, 3, 4)]
    [TestCase(4, 2, 3)]
    public void New3DPreservesEachDimension(int width, int height, int depth)
    {
        using var image = Image.New3D(width, height, depth, 1, (PixelFormat)global::SharpDX.DXGI.Format.R8G8B8A8_UNorm);

        Assert.That(image.Description.Width, Is.EqualTo(width));
        Assert.That(image.Description.Height, Is.EqualTo(height));
        Assert.That(image.Description.Depth, Is.EqualTo(depth));
        var buffers = image.PixelBuffer!;
        Assert.That(buffers.Count, Is.EqualTo(depth));
        Assert.That(buffers[0].Width, Is.EqualTo(width));
        Assert.That(buffers[0].Height, Is.EqualTo(height));
        Assert.That(buffers[0].RowStride, Is.EqualTo(width * 4));
        Assert.That(buffers[0].BufferStride, Is.EqualTo(width * height * 4));
    }
}

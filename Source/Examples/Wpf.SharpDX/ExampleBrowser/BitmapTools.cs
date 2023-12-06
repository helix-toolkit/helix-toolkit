using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExampleBrowser;

public static class BitmapTools
{
    public static Bitmap Resize(Bitmap bitmap, int newWidth, int newHeight)
    {
        var resizedBitmap = new Bitmap(newWidth, newHeight);
        using var g = Graphics.FromImage(resizedBitmap);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
        return resizedBitmap;
    }
}

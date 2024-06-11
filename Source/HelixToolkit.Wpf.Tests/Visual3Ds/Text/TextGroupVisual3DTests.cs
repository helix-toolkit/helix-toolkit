using NUnit.Framework;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.Tests.Visual3Ds.Text;

[TestFixture]
public class TextGroupVisual3DTests
{
    [Test, Apartment(ApartmentState.STA)]
    public void CreateTextMaterial_Correct_ElementPositions()
    {
        List<TextItem> items = new() { new BillboardTextItem { Text = "Test" } };

        var material = TextGroupVisual3D.CreateTextMaterial(
            items, CreateElement,
            Brushes.White, false,
            out Dictionary<string, FrameworkElement> elementMap,
            out Dictionary<FrameworkElement, Rect> elementPositions);

        Assert.Multiple(() =>
        {
            Assert.That(material, Is.Not.Null);

            // The element positions are rounded and the values should be between 0 and 1 (inclusive)
            Assert.That(elementPositions.All(kv => kv.Value.Top >= 0 && kv.Value.Top <= 1));
            Assert.That(elementPositions.All(kv => kv.Value.Bottom >= 0 && kv.Value.Bottom <= 1));
            Assert.That(elementPositions.All(kv => kv.Value.Left >= 0 && kv.Value.Left <= 1));
            Assert.That(elementPositions.All(kv => kv.Value.Right >= 0 && kv.Value.Right <= 1));
        });
    }

    private static FrameworkElement CreateElement(string text)
    {
        return new TextBlock { Width = 100.1, Height = 100.1 };
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReaderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests.Visual3Ds.Text
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using NUnit.Framework;

    [TestFixture]
    public class TextGroupVisual3DTests
    {
        [Test, Apartment(ApartmentState.STA)]
        public void CreateTextMaterial_Correct_ElementPositions()
        {
            List<TextItem> items = new List<TextItem> {new BillboardTextItem {Text = "Test"}};
            Dictionary<string, FrameworkElement> elementMap;
            Dictionary<FrameworkElement, Rect> elementPositions;

            var material = TextGroupVisual3D.CreateTextMaterial(
                items, CreateElement, Brushes.White, out elementMap, out elementPositions);

            Assert.NotNull(material);

            // The element positions are rounded and the values should be between 0 and 1 (inclusive)
            Assert.True(elementPositions.All(kv => kv.Value.Top >= 0 && kv.Value.Top <= 1));
            Assert.True(elementPositions.All(kv => kv.Value.Bottom >= 0 && kv.Value.Bottom<= 1));
            Assert.True(elementPositions.All(kv => kv.Value.Left >= 0 && kv.Value.Left <= 1));
            Assert.True(elementPositions.All(kv => kv.Value.Right >= 0 && kv.Value.Right <= 1));
        }

        private static FrameworkElement CreateElement(string text)
        {
            return new TextBlock {Width = 100.1, Height = 100.1};
        }
    }
}

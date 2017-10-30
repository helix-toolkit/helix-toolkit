// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillboardTextVisual3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests.Visual3Ds.Text
{
    using System.Windows;

    using NUnit.Framework;

    [TestFixture]
    public class BillboardTextVisual3DTests
    {
        [Test]
        public void MaterialTypeProperty_Metadata_DefaultValues()
        {
            PropertyMetadata metadata = BillboardTextVisual3D.MaterialTypeProperty.GetMetadata(typeof(BillboardTextVisual3D));

            Assert.AreEqual((MaterialType)metadata.DefaultValue, MaterialType.Diffuse);
            Assert.NotNull(metadata.PropertyChangedCallback);
        }
    }
}

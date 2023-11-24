using NUnit.Framework;
using System.Windows;

namespace HelixToolkit.Wpf.Tests.Visual3Ds.Text;

[TestFixture]
public class BillboardTextVisual3DTests
{
    [Test]
    public void MaterialTypeProperty_Metadata_DefaultValues()
    {
        PropertyMetadata metadata = BillboardTextVisual3D.MaterialTypeProperty.GetMetadata(typeof(BillboardTextVisual3D));

        Assert.That((MaterialType)metadata.DefaultValue, Is.EqualTo(MaterialType.Diffuse));
        Assert.That(metadata.PropertyChangedCallback, Is.Not.Null);
    }
}

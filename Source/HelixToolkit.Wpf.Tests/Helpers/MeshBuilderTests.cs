// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilderTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class MeshBuilderTests
    {
        [Test]
        public void A_B_C()
        {
            var mb = new MeshBuilder();
            Assert.NotNull(mb);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
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
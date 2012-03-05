// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StereoHelperTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HelixToolkit.Wpf;

namespace HelixToolkitTests
{
    [TestFixture]
    public class StereoHelperTests
    {
        [Test]
        public void CalculateStereoBase_40mm_ExpectedBehavior()
        {
            // fov=48.45deg, 36mm film => 40mm focal length
            Assert.AreEqual(40, StereoHelper.FindFocalLength(48.4554, 36), 1e-3);

            // F=40mm, P=1.2mm, 2m-10m, 36mm film
            Assert.AreEqual(74.09, StereoHelper.CalculateStereoBase(1.2, 10e3, 2e3, 40), 1e-2);

            // fov=48.45deg, 1/30, 2m-10m, 36mm film
            Assert.AreEqual(74.09, StereoHelper.CalculateStereoBase(10e3, 2e3, 36, 1.0 / 30, 48.4554), 1e-2);
        }
    }
}
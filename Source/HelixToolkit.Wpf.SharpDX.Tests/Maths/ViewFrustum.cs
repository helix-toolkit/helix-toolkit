using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Mathematics;
using FrustumH = HelixToolkit.Mathematics.BoundingFrustum;
using FrustumS = SharpDX.BoundingFrustum;
using Vector3H = System.Numerics.Vector3;
using Vector3S = SharpDX.Vector3;

namespace HelixToolkit.Maths
{
    [TestFixture]
    class ViewFrustum
    {
        [Test]
        public void TestConstruction()
        {
            Random rnd = new Random((int)Stopwatch.GetTimestamp());
            for(int i=0; i<100; ++i)
            {
                var cPosh = new Vector3H(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100));
                var cLookH = new Vector3H(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100));
                float fov = rnd.Next(10, 90);
                float znear = rnd.Next(1, 1000);
                float zfar = rnd.Next(1, 1000);
                float aspect = (float)rnd.Next(100, 1000) / rnd.Next(100, 1000);
                var fh = FrustumH.FromCamera(cPosh, cLookH, new Vector3H(0, 1, 0), fov, znear, zfar, aspect);

                var cPosS = new Vector3S(cPosh.X, cPosh.Y, cPosh.Z);
                var clookS = new Vector3S(cLookH.X, cLookH.Y, cLookH.Z);
                var fs = FrustumS.FromCamera(cPosS, clookS, new Vector3S(0, 1, 0), fov, znear, zfar, aspect);

                for(int j=0; j < 6; ++j)
                {
                    var ph = fh.GetPlane(j);
                    var ps = fs.GetPlane(j);
                    Assert.IsTrue(Common.Equal(ref ph.Normal, ref ps.Normal));
                    Assert.IsTrue(MathUtil.NearEqual(ph.D, ps.D));                   
                }

                var ch = fh.GetCorners();
                var cs = fs.GetCorners();
                Assert.IsTrue(ch.Length == cs.Length);
                for(int j = 0; j < ch.Length; ++j)
                {
                    Assert.IsTrue(Common.Equal(ref ch[j], ref cs[j]));
                }
            }
        }
    }
}

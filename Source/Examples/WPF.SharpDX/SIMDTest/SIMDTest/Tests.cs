using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumM = System.Numerics.Matrix4x4;
using SharpM = SharpDX.Matrix;
using NumV3 = System.Numerics.Vector3;
using SharpV3 = SharpDX.Vector3;
using NumV4 = System.Numerics.Vector4;
using SharpV4 = SharpDX.Vector4;
using System.Diagnostics;

namespace SIMDTest
{
    public static class Tests
    {
        private static readonly Random rnd = new Random((int)Stopwatch.GetTimestamp());
        private const int Iteration = 1000000;
        private const int Size = 1000000;
        private static NumV4[] NumV4s = new NumV4[Size];
        public static SharpV4[] SharpV4s = new SharpV4[Size];

        static Tests()
        {
            for (int i = 0; i < Size; ++i)
            {
                NumV4s[i] = new NumV4(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100), 1);
                SharpV4s[i] = new SharpV4(NumV4s[i].X, NumV4s[i].Y, NumV4s[i].Z, NumV4s[i].W);
            }
        }

        public static void Init() { }

        public static bool TestNumMatrixMultiplication()
        {
            var m = NumM.Identity;
            var scale = NumM.CreateScale(rnd.Next(-100, 100));
            var rotate = NumM.CreateFromAxisAngle(new NumV3(1, 0, 0), 30f / 180 * (float)Math.PI);
            var translate = NumM.CreateTranslation(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
            var view = NumM.CreateLookAt(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(0, 1, 0));
            var pers = NumM.CreatePerspective(800, 600, 1, 1000);
            for(int i=0; i < Iteration; ++i)
            {
                m = scale * rotate * translate * view * pers;
            }
            return true;
        }
        public static bool TestSharpMatrixMultiplication()
        {
            var m = SharpM.Identity;
            var scale = SharpM.Scaling(rnd.Next(-100, 100));
            var rotate = SharpM.RotationAxis(new SharpV3(1, 0, 0), 30f / 180 * (float)Math.PI);
            var translate = SharpM.Translation(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
            var view = SharpM.LookAtRH(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(0, 1, 0));
            var pers = SharpM.PerspectiveRH(800, 600, 1, 1000);
            for (int i = 0; i < Iteration; ++i)
            {
                m = scale * rotate * translate * view * pers;
            }
            return true;
        }

        public static bool TestNumVector4Normalization()
        {
            for(int i=0; i < Size; ++i)
            {
                NumV4s[i] = NumV4.Normalize(NumV4s[i]);
            }
            return true;
        }

        public static bool TestSharpVector4Normalization()
        {
            for (int i = 0; i < Size; ++i)
            {
                SharpV4s[i] = SharpV4.Normalize(SharpV4s[i]);
            }
            return true;
        }

        public static bool TestNumVector4MulMatrix()
        {
            var scale = NumM.CreateScale(rnd.Next(-100, 100));
            var rotate = NumM.CreateFromAxisAngle(new NumV3(1, 0, 0), 30f / 180 * (float)Math.PI);
            var translate = NumM.CreateTranslation(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
            var view = NumM.CreateLookAt(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(0, 1, 0));
            var pers = NumM.CreatePerspective(800, 600, 1, 1000);
            var m = scale * rotate * translate * view * pers;
            for (int i = 0; i < Size; ++i)
            {
                NumV4s[i] = NumV4.Transform(NumV4s[i], m);
            }
            return true;
        }

        public static bool TestSharpVector4MulMatrix()
        {
            var scale = SharpM.Scaling(rnd.Next(-100, 100));
            var rotate = SharpM.RotationAxis(new SharpV3(1, 0, 0), 30f / 180 * (float)Math.PI);
            var translate = SharpM.Translation(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
            var view = SharpM.LookAtRH(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(0, 1, 0));
            var pers = SharpM.PerspectiveRH(800, 600, 1, 1000);
            var m = scale * rotate * translate * view * pers;
            for (int i = 0; i < Size; ++i)
            {
                SharpV4s[i] = SharpV4.Transform(SharpV4s[i], m);
            }
            return true;
        }
    }
}

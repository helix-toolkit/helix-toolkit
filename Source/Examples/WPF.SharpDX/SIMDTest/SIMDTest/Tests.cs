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
using HelixToolkit.Mathematics;

namespace SIMDTest
{
    public static class Tests
    {
        private static readonly Random rnd = new Random((int)Stopwatch.GetTimestamp());
        private const int Iteration = 1000000;
        private const int Size = 1000000;
        private static NumV4[] NumV4s = new NumV4[Size];
        public static SharpV4[] SharpV4s = new SharpV4[Size];
        private static NumV3[] NumV3s = new NumV3[Size];
        public static SharpV3[] SharpV3s = new SharpV3[Size];

        static Tests()
        {
            for (int i = 0; i < Size; ++i)
            {
                NumV4s[i] = new NumV4(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100), 1);
                SharpV4s[i] = new SharpV4(NumV4s[i].X, NumV4s[i].Y, NumV4s[i].Z, NumV4s[i].W);
                NumV3s[i] = new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100));
                SharpV3s[i] = new SharpV3(NumV3s[i].X, NumV3s[i].Y, NumV3s[i].Z);
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

        public static bool TestNumVector4Dot()
        {
            NumV4 v1 = new NumV4(1, 2, 3, 5);
            float sum = 0;
            for (int i = 0; i < Size; ++i)
            {
                sum += NumV4.Dot(NumV4s[i], v1);
            }
            return true;
        }

        public static bool TestSharpVector4Dot()
        {
            SharpV4 v1 = new SharpV4(1, 2, 3, 5);
            float sum = 0;
            for (int i = 0; i < Size; ++i)
            {
                sum += SharpV4.Dot(SharpV4s[i], v1);
            }
            return true;
        }
        public static bool TestNumVector3Cross()
        {
            NumV3 v1 = new NumV3(1, 2, 3);
            for (int i = 0; i < Size; ++i)
            {
                NumV3s[i] += NumV3.Cross(NumV3s[i], v1);
            }
            return true;
        }

        public static bool TestSharpVector3Cross()
        {
            SharpV3 v1 = new SharpV3(1, 2, 3);
            for (int i = 0; i < Size; ++i)
            {
                SharpV3s[i] += SharpV3.Cross(SharpV3s[i], v1);
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
                m = scale * rotate * translate * view;
                MatrixHelper.Orthogonalize(ref m);
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
                m = scale * rotate * translate * view;
                m.Orthogonalize();
            }
            return true;
        }

        public static bool TestNumMatrixOrthogonalize()
        {
            NumM m;
            for (int i = 0; i < Iteration; ++i)
            {
                var scale = NumM.CreateScale(rnd.Next(-100, 100));
                var rotate = NumM.CreateFromAxisAngle(new NumV3(1, 0, 0), 30f / 180 * (float)Math.PI);
                var translate = NumM.CreateTranslation(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
                var view = NumM.CreateLookAt(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(0, 1, 0));
                m = scale * rotate * translate * view;
                MatrixHelper.Orthogonalize(ref m);
            }
            return true;
        }

        public static bool TestSharpMatrixOrthogonalize()
        {
            SharpM m;
            for (int i = 0; i < Iteration; ++i)
            {
                var scale = SharpM.Scaling(rnd.Next(-100, 100));
                var rotate = SharpM.RotationAxis(new SharpV3(1, 0, 0), 30f / 180 * (float)Math.PI);
                var translate = SharpM.Translation(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
                var view = SharpM.LookAtRH(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(0, 1, 0));
                m = scale * rotate * translate * view;
                m.Orthogonalize();
            }
            return true;
        }

        public static bool TestNumMatrixInvert()
        {
            NumM m;
            for (int i = 0; i < Iteration; ++i)
            {
                var scale = NumM.CreateScale(rnd.Next(-100, 100));
                var rotate = NumM.CreateFromAxisAngle(new NumV3(1, 0, 0), 30f / 180 * (float)Math.PI);
                var translate = NumM.CreateTranslation(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
                var view = NumM.CreateLookAt(new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new NumV3(0, 1, 0));
                m = scale * rotate * translate * view;
                NumM.Invert(m, out m);
            }
            return true;
        }

        public static bool TestSharpMatrixInvert()
        {
            SharpM m;
            for (int i = 0; i < Iteration; ++i)
            {
                var scale = SharpM.Scaling(rnd.Next(-100, 100));
                var rotate = SharpM.RotationAxis(new SharpV3(1, 0, 0), 30f / 180 * (float)Math.PI);
                var translate = SharpM.Translation(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)));
                var view = SharpM.LookAtRH(new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(rnd.Next(-100, 100), rnd.Next(-100, 100), rnd.Next(-100, 100)), new SharpV3(0, 1, 0));
                m = scale * rotate * translate * view;
                m = SharpM.Invert(m);
            }
            return true;
        }

        public static bool TestNumVector4IsZero()
        {

            bool isZero = false;
            for(int i=0; i < NumV4s.Length; ++i)
            {
                isZero &= NumV4s[i].IsZero();
            }
            return true;
        }

        public static bool TestSharpVector4IsZero()
        {

            bool isZero = false;
            for (int i = 0; i < NumV4s.Length; ++i)
            {
                isZero &= SharpV4s[i].IsZero;
            }
            return true;
        }
    }
}

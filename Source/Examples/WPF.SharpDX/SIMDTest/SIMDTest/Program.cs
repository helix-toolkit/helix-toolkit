using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMDTest
{
    class Program
    {
        private static List<TestWrapper> tests = new List<TestWrapper>()
        {
            new TestWrapper("Vector3 Dot SIMD", Tests.TestNumVector3Cross),
            new TestWrapper("Vector3 Dot Sharp", Tests.TestSharpVector3Cross),
            new TestWrapper("Vector4 Dot SIMD", Tests.TestNumVector4Dot),
            new TestWrapper("Vector4 Dot Sharp", Tests.TestSharpVector4Dot),
            new TestWrapper("Vector4 Norm SIMD", Tests.TestNumVector4Normalization),
            new TestWrapper("Vector4 Norm Sharp", Tests.TestSharpVector4Normalization),
            new TestWrapper("Vector4 Mul Matrix SIMD", Tests.TestNumVector4MulMatrix),
            new TestWrapper("Vector4 Mul Matrix Sharp", Tests.TestSharpVector4MulMatrix),
            new TestWrapper("Matrix Mult SIMD", Tests.TestNumMatrixMultiplication),
            new TestWrapper("Matrix Mult Sharp", Tests.TestSharpMatrixMultiplication),
            new TestWrapper("Matrix Ortho SIMD", Tests.TestNumMatrixOrthogonalize),
            new TestWrapper("Matrix Ortho Sharp", Tests.TestSharpMatrixOrthogonalize),
            new TestWrapper("Matrix Invert SIMD", Tests.TestNumMatrixInvert),
            new TestWrapper("Matrix Invert Sharp", Tests.TestSharpMatrixInvert),
            new TestWrapper("Vector4 is zero SIMD", Tests.TestNumVector4IsZero),
            new TestWrapper("Vector4 is zero Sharp", Tests.TestSharpVector4IsZero),
        };
        static void Main(string[] args)
        {
            Tests.Init();
            for(int i=0; i < tests.Count; ++i)
            {
                tests[i].Run();
            }

            Console.Read();
        }
    }
}

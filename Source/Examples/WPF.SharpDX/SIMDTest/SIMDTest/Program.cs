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
            new TestWrapper("Vector4 Norm SIMD", Tests.TestNumVector4Normalization),
            new TestWrapper("Vector4 Norm Sharp", Tests.TestSharpVector4Normalization),
            new TestWrapper("Vector4 Mul Matrix SIMD", Tests.TestNumVector4MulMatrix),
            new TestWrapper("Vector4 Mul Matrix Sharp", Tests.TestSharpVector4MulMatrix),
            new TestWrapper("Matrix Mult SIMD", Tests.TestNumMatrixMultiplication),
            new TestWrapper("Matrix Mult Sharp", Tests.TestSharpMatrixMultiplication),
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

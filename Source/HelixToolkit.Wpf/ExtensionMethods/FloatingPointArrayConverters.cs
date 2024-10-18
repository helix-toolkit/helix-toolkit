#if NET6_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
#endif

namespace HelixToolkit.Wpf;

internal static class FloatingPointArrayConverters
{
    public unsafe static void ConvertFloatToDouble<T1, T2>(int arrayCount, T1[] floatArray, T2[] doubleArray)
    {
#if NET8_0_OR_GREATER
        if (Avx512F.IsSupported)
        {
            int floatCount = Vector256<float>.Count;
            int length = arrayCount / floatCount;

#pragma warning disable CS8500
            fixed (void* floatDataPtr = floatArray)
            fixed (void* doubleDataPtr = doubleArray)
#pragma warning restore CS8500
            {
                float* floatData = (float*)floatDataPtr;
                double* doubleData = (double*)doubleDataPtr;

                for (nuint i = 0; i < (uint)length; i++)
                {
                    Vector256<float> d = Avx.LoadVector256(floatData);
                    Vector512<double> r = Avx512F.ConvertToVector512Double(d);
                    Avx512F.Store(doubleData, r);

                    floatData += floatCount;
                    doubleData += floatCount;
                }

                for (int i = length * floatCount; i < arrayCount; i++)
                {
                    *doubleData = *floatData;
                    floatData++;
                    doubleData++;
                }
            }

            return;
        }
#endif

#if NET6_0_OR_GREATER
        if (Avx.IsSupported)
        {
            int floatCount = Vector128<float>.Count;
            int length = arrayCount / floatCount;

#pragma warning disable CS8500
            fixed (void* floatDataPtr = floatArray)
            fixed (void* doubleDataPtr = doubleArray)
#pragma warning restore CS8500
            {
                float* floatData = (float*)floatDataPtr;
                double* doubleData = (double*)doubleDataPtr;

                for (nuint i = 0; i < (uint)length; i++)
                {
                    Vector128<float> d = Sse.LoadVector128(floatData);
                    Vector256<double> r = Avx.ConvertToVector256Double(d);
                    Avx.Store(doubleData, r);

                    floatData += floatCount;
                    doubleData += floatCount;
                }

                for (int i = length * floatCount; i < arrayCount; i++)
                {
                    *doubleData = *floatData;
                    floatData++;
                    doubleData++;
                }
            }

            return;
        }
#endif

#pragma warning disable CS8500
        fixed (void* floatDataPtr = floatArray)
        fixed (void* doubleDataPtr = doubleArray)
#pragma warning restore CS8500
        {
            float* floatData = (float*)floatDataPtr;
            double* doubleData = (double*)doubleDataPtr;

            for (int i = 0; i < arrayCount; i++)
            {
                *doubleData = *floatData;
                floatData++;
                doubleData++;
            }
        }
    }

    public unsafe static void ConvertDoubleToFloat<T1, T2>(int arrayCount, T1[] doubleArray, T2[] floatArray)
    {
#if NET8_0_OR_GREATER
        if (Avx512F.IsSupported)
        {
            int doubleCount = Vector512<double>.Count;
            int length = arrayCount / doubleCount;

#pragma warning disable CS8500
            fixed (void* doubleDataPtr = doubleArray)
            fixed (void* floatDataPtr = floatArray)
#pragma warning restore CS8500
            {
                double* doubleData = (double*)doubleDataPtr;
                float* floatData = (float*)floatDataPtr;

                for (nuint i = 0; i < (uint)length; i++)
                {
                    Vector512<double> d = Avx512F.LoadVector512(doubleData);
                    Vector256<float> r = Avx512F.ConvertToVector256Single(d);
                    Avx.Store(floatData, r);

                    doubleData += doubleCount;
                    floatData += doubleCount;
                }

                for (int i = length * doubleCount; i < arrayCount; i++)
                {
                    *floatData = (float)*doubleData;
                    doubleData++;
                    floatData++;
                }
            }

            return;
        }
#endif

#if NET6_0_OR_GREATER
        if (Avx.IsSupported)
        {
            int doubleCount = Vector256<double>.Count;
            int length = arrayCount / doubleCount;

#pragma warning disable CS8500
            fixed (void* doubleDataPtr = doubleArray)
            fixed (void* floatDataPtr = floatArray)
#pragma warning restore CS8500
            {
                double* doubleData = (double*)doubleDataPtr;
                float* floatData = (float*)floatDataPtr;

                for (nuint i = 0; i < (uint)length; i++)
                {
                    Vector256<double> d = Avx.LoadVector256(doubleData);
                    Vector128<float> r = Avx.ConvertToVector128Single(d);
                    Sse.Store(floatData, r);

                    doubleData += doubleCount;
                    floatData += doubleCount;
                }

                for (int i = length * doubleCount; i < arrayCount; i++)
                {
                    *floatData = (float)*doubleData;
                    doubleData++;
                    floatData++;
                }
            }

            return;
        }
#endif

#pragma warning disable CS8500
        fixed (void* doubleDataPtr = doubleArray)
        fixed (void* floatDataPtr = floatArray)
#pragma warning restore CS8500
        {
            double* doubleData = (double*)doubleDataPtr;
            float* floatData = (float*)floatDataPtr;

            for (int i = 0; i < arrayCount; i++)
            {
                *floatData = (float)*doubleData;
                doubleData++;
                floatData++;
            }
        }
    }
}

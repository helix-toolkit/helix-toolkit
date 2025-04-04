﻿// See https://aka.ms/new-console-template for more information
using HelixToolkit.Perf;
using HelixToolkit.Perf.Maths;
using System.Runtime.Intrinsics.X86;


Console.WriteLine("Start running CPU performance tests.");
Console.WriteLine($"SIMD support: {Vector.IsHardwareAccelerated}");
Console.WriteLine($"SSE2: {Sse2.IsSupported}");

Console.WriteLine($"SSE3: {Sse3.IsSupported}");
Console.WriteLine($"SSE41: {Sse41.IsSupported}");
Console.WriteLine($"SSE42: {Sse42.IsSupported}");

Console.WriteLine($"AVX: {Avx.IsSupported}");
Console.WriteLine($"AVX2: {Avx2.IsSupported}");
Console.WriteLine($"AVX512: {Avx512F.IsSupported}");

AddTestCases();

TestCase.RunAll();


static void AddTestCases()
{
    TestCase.Add(nameof(Normalization.NormalizeInPlaceTest), () =>
    {
        Normalization.NormalizeInPlaceTest(Settings.DataSetSize, Settings.Iteration);
    });
    TestCase.Add(nameof(Vector3Tester.MinMax), () =>
    {
        Vector3Tester.MinMax(Settings.DataSetSize, Settings.Iteration);
    });
    TestCase.Add(nameof(Vector3Tester.TransformCoordinate), () =>
    {
        Vector3Tester.TransformCoordinate(Settings.DataSetSize, Settings.Iteration);
    });
    TestCase.Add(nameof(Vector3Tester.TransformNormal), () =>
    {
        Vector3Tester.TransformNormal(Settings.DataSetSize, Settings.Iteration);
    }); 
    TestCase.Add(nameof(Vector3Tester.GetCentroidTest), () =>
    {
        Vector3Tester.GetCentroidTest(Settings.DataSetSize, Settings.Iteration);
    });
    TestCase.Add(nameof(Vector4Tester.Transform), () =>
    {
        Vector4Tester.Transform(Settings.DataSetSize, Settings.Iteration);
    });
}

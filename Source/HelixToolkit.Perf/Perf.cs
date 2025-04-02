using System.Diagnostics;

namespace HelixToolkit.Perf;

internal static class Perf
{
    public static void Profile(string description, Action codeBlock)
    {
        var start = Stopwatch.GetTimestamp();
        codeBlock();
        var end = Stopwatch.GetTimestamp();
        var elapsed = end - start;
        var time = (double)elapsed / Stopwatch.Frequency;
        Console.WriteLine($"SIMD: {MathSettings.EnableSIMD}; Description: {description}; Took: {time} sec.");
    }
}

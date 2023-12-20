using System.Numerics;

namespace HelixToolkit;

/// <summary>
/// Implements improved Perlin noise in 2D. 
/// Transcribed from http://www.siafoo.net/snippet/144?nolinenos#perlin2003
/// From StackOverflow: https://stackoverflow.com/questions/8659351/2d-perlin-noise
/// </summary>
public static class Noise2d
{
    private static readonly Random _random = new();
    private static int[] _permutation;
    private static Vector2[] _gradients;

    static Noise2d()
    {
        CalculatePermutation(out _permutation);
        CalculateGradients(out _gradients);
    }

    private static void CalculatePermutation(out int[] p)
    {
        p = Enumerable.Range(0, 256).ToArray();

        // shuffle the array
        for (var i = 0; i < p.Length; i++)
        {
            var source = _random.Next(p.Length);

            (p[source], p[i]) = (p[i], p[source]);
        }
    }

    /// <summary>
    /// generate a new permutation.
    /// </summary>
    public static void Reseed()
    {
        CalculatePermutation(out _permutation);
    }

    private static void CalculateGradients(out Vector2[] grad)
    {
        grad = new Vector2[256];

        for (var i = 0; i < grad.Length; i++)
        {
            Vector2 gradient;

            do
            {
#if NET6_0_OR_GREATER
                gradient = new Vector2(_random.NextSingle() * 2 - 1, _random.NextSingle() * 2 - 1);
#else
                gradient = new Vector2((float)(_random.NextDouble() * 2 - 1), (float)(_random.NextDouble() * 2 - 1));
#endif
            }
            while (gradient.LengthSquared() >= 1);

            gradient = Vector2.Normalize(gradient);

            grad[i] = gradient;
        }
    }

    private static float Drop(float t)
    {
        t = Math.Abs(t);
        return 1f - t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Q(float u, float v)
    {
        return Drop(u) * Drop(v);
    }

    /// <summary>
    /// Noises the specified x.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public static float Noise(float x, float y)
    {
#if NET6_0_OR_GREATER
        var cell = new Vector2(MathF.Floor(x), MathF.Floor(y));
#else
        var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));
#endif

        float total = 0;

        var corners = new[] { Vector2.Zero, Vector2.UnitY, Vector2.UnitX, Vector2.One };

        foreach (var n in corners)
        {
            var ij = cell + n;
            var uv = new Vector2(x - ij.X, y - ij.Y);

            var index = _permutation[(int)ij.X % _permutation.Length];
            index = _permutation[(index + (int)ij.Y) % _permutation.Length];

            var grad = _gradients[index % _gradients.Length];

            total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
        }

        return Math.Max(Math.Min(total, 1f), -1f);
    }

    /// <summary>
    /// Generates the noise map.
    /// From https://stackoverflow.com/questions/8659351/2d-perlin-noise
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="result">The result.</param>
    /// <param name="octaves">The octaves.</param>
    public static void GenerateNoiseMap(int width, int height, int octaves, out float[] result)
    {
        var data = new float[width * height];

        // track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
        var min = float.MaxValue;
        var max = float.MinValue;

        // rebuild the permutation table to get a different noise pattern. 
        // Leave this out if you want to play with changing the number of octaves while 
        // maintaining the same overall pattern.
        Noise2d.Reseed();

        var frequency = 0.5f;
        var amplitude = 1f;
        //var persistence = 0.25f;

        for (var octave = 0; octave < octaves; octave++)
        {
            // parallel loop - easy and fast.
            Parallel.For(0
                , width * height
                , (offset) =>
                {
                    var i = offset % width;
                    var j = offset / width;
                    var noise = Noise2d.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
                    noise = data[j * width + i] += noise * amplitude;

                    min = Math.Min(min, noise);
                    max = Math.Max(max, noise);

                }
            );

            frequency *= 2;
            amplitude /= 2;
        }

        //Normalize
        for (var i = 0; i < data.Length; ++i)
        {
            data[i] = (data[i] - min) / (max - min);
        }

        result = data;
    }
}

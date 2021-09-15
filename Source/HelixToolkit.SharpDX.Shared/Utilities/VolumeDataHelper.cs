/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
Reference: https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
*/
using SharpDX;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        public static class VolumeDataHelper
        {
            /// <summary>
            /// Generates gradients using a central differences scheme.
            /// </summary>
            /// <param name="data">Normalized voxel data</param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="depth"></param>
            /// <param name="sampleSize">The size/radius of the sample to take.</param>
            public static Half4[] GenerateGradients(float[] data, int width, int height, int depth, int sampleSize)
            {
                int n = sampleSize;

                var gradients = new Half4[width * height * depth];
                Parallel.For(0, depth, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (z) =>
                {
                    int index = z * width * height;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, ++index)
                        {
                            Vector3 s1, s2;
                            s1.X = SampleVolume(data, width, height, depth, x - n, y, z);
                            s2.X = SampleVolume(data, width, height, depth, x + n, y, z);
                            s1.Y = SampleVolume(data, width, height, depth, x, y - n, z);
                            s2.Y = SampleVolume(data, width, height, depth, x, y + n, z);
                            s1.Z = SampleVolume(data, width, height, depth, x, y, z - n);
                            s2.Z = SampleVolume(data, width, height, depth, x, y, z + n);
                            var v = Vector3.Normalize(s2 - s1);
                            var sample = SampleVolume(data, width, height, depth, x, y, z);
                            gradients[index] = new Half4(v.X, v.Y, v.Z, sample);
                            if (float.IsNaN(gradients[index].X))
                            {
                                gradients[index] = new Half4(0, 0, 0, sample);
                            }
                        }
                    }
                });
                return gradients;
            }

            /// <summary>
            /// Applies an NxNxN filter to the gradients. 
            /// Should be an odd number of samples. 3 used by default.
            /// </summary>
            /// <param name="data">Gradient data from <see cref="GenerateGradients(float[], int, int, int, int)"/></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="depth"></param>
            /// <param name="n"></param>
            public static void FilterNxNxN(Half4[] data, int width, int height, int depth, int n)
            {
                int index = 0;
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            float w = data[index].W;
                            data[index++] = SampleNxNxN(data, width, height, depth, x, y, z, n).ToVector4(w);
                        }
                    }
                }
            }

            /// <summary>
            /// Samples the sub-volume graident volume and returns the average.
            /// Should be an odd number of samples.
            /// </summary>
            /// <param name="data"></param>
            /// <param name="depth"></param>
            /// <param name="height"></param>
            /// <param name="width"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Vector3 SampleNxNxN(Half4[] data, int width, int height, int depth, int x, int y, int z, int n)
            {
                n = (n - 1) / 2;

                Vector3 average = Vector3.Zero;
                int num = 0;

                for (int k = z - n; k <= z + n; k++)
                {
                    for (int j = y - n; j <= y + n; j++)
                    {
                        for (int i = x - n; i <= x + n; i++)
                        {
                            if (IsInBounds(width, height, depth, i, j, k))
                            {
                                average += SampleGradients(data, width, height, depth, i, j, k);
                                num++;
                            }
                        }
                    }
                }

                average /= (float)num;
                if (average.X != 0.0f && average.Y != 0.0f && average.Z != 0.0f)
                    average.Normalize();

                return average;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsInBounds(int width, int height, int depth, int x, int y, int z)
            {
                return ((x >= 0 && x < width) &&
                        (y >= 0 && y < height) &&
                        (z >= 0 && z < depth));
            }
            /// <summary>
            /// Samples the gradient volume
            /// </summary>
            /// <param name="data"></param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Vector3 SampleGradients(Half4[] data, int width, int height, int depth, int x, int y, int z)
            {
                var half = data[x + (y * width) + (z * width * height)];
                return new Vector3(half.X, half.Y, half.Z);
            }
            /// <summary>
            /// Samples the volume.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="z">The z.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static float SampleVolume(float[] data, int width, int height, int depth, int x, int y, int z)
            {
                x = (int)Math.Min(Math.Max(x, 0), width - 1);
                y = (int)Math.Min(Math.Max(y, 0), height - 1);
                z = (int)Math.Min(Math.Max(z, 0), depth - 1);

                return data[x + (y * width) + (z * width * height)];
            }
        }
    }

}

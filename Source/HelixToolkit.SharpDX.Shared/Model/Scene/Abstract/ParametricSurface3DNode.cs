/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Runtime.CompilerServices;
using System.Threading;
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
    namespace Model.Scene
    {
        public abstract class ParametricSurface3DNode : MeshNode
        {
            private Task tesselationTask;
            private CancellationTokenSource cancelToken = new CancellationTokenSource();

            private int meshSizeU = 120;
            public int MeshSizeU
            {
                set
                {
                    if (Set(ref meshSizeU, value))
                    {
                        TessellateAsync();
                    }
                }
                get => meshSizeU;
            }

            private int meshSizeV = 120;
            public int MeshSizeV
            {
                set
                {
                    if(Set(ref meshSizeV, value))
                    {
                        TessellateAsync();
                    }
                }
                get => meshSizeV;
            }

            private bool isTessellating = false;
            public bool IsTessellating
            {
                private set => Set(ref isTessellating, value);
                get => isTessellating;
            }

            protected override bool OnAttach(IRenderHost host)
            {
                cancelToken = Collect(new CancellationTokenSource());
                return base.OnAttach(host);
            }

            protected override void OnDetach()
            {
                cancelToken.Cancel(true);
                RemoveAndDispose(ref cancelToken);
                base.OnDetach();
            }

            protected void TessellateAsync()
            {
                cancelToken.Cancel(true);
                RemoveAndDispose(ref cancelToken);
                cancelToken = Collect(new CancellationTokenSource());
                IsTessellating = true;
                var token = cancelToken.Token;
                tesselationTask = Task.Run(() => 
                {
                    var mesh = OnTesselatingAsync(token);
                    mesh.Normals = mesh.CalculateNormals();
                    mesh?.UpdateOctree();
                    mesh?.UpdateBounds();
                    return mesh;
                }, 
                token).ContinueWith(result => 
                {
                    IsTessellating = false;
                    if (result.IsCompleted)
                    {
                        this.Geometry = result.Result;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            protected virtual MeshGeometry3D OnTesselatingAsync(CancellationToken token)
            {
                var mesh = new MeshGeometry3D()
                {
                    Positions = new Vector3Collection(),
                    TextureCoordinates = new Vector2Collection(),
                    Indices = new IntCollection()
                };

                int n = this.MeshSizeU;
                int m = this.MeshSizeV;
                var p = new Vector3[m * n];
                var tc = new Vector2[m * n];

                // todo: use MeshBuilder

                // todo: parallel execution...
                // Parallel.For(0, n, (i) =>
                for (int i = 0; i < n && !token.IsCancellationRequested; i++)
                {
                    double u = 1.0 * i / (n - 1);

                    for (int j = 0; j < m; j++)
                    {
                        double v = 1.0 * j / (m - 1);
                        int ij = (i * m) + j;
                        p[ij] = this.Evaluate(u, v, out tc[ij]);
                    }
                }

                // );
                int idx = 0;
                for (int i = 0; i < n && !token.IsCancellationRequested; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        mesh.Positions.Add(p[idx]);
                        mesh.TextureCoordinates.Add(tc[idx]);
                        idx++;
                    }
                }

                for (int i = 0; i + 1 < n && !token.IsCancellationRequested; i++)
                {
                    for (int j = 0; j + 1 < m; j++)
                    {
                        int x0 = i * m;
                        int x1 = (i + 1) * m;
                        int y0 = j;
                        int y1 = j + 1;
                        AddTriangle(mesh, x0 + y0, x1 + y0, x0 + y1);
                        AddTriangle(mesh, x1 + y0, x1 + y1, x0 + y1);
                    }
                }

                return mesh;
            }
            /// <summary>
            /// The add triangle.
            /// </summary>
            /// <param name="mesh">
            /// The mesh.
            /// </param>
            /// <param name="i1">
            /// The i 1.
            /// </param>
            /// <param name="i2">
            /// The i 2.
            /// </param>
            /// <param name="i3">
            /// The i 3.
            /// </param>
            private static void AddTriangle(MeshGeometry3D mesh, int i1, int i2, int i3)
            {
                var p1 = mesh.Positions[i1];
                if (!IsDefined(p1))
                {
                    return;
                }

                var p2 = mesh.Positions[i2];
                if (!IsDefined(p2))
                {
                    return;
                }

                var p3 = mesh.Positions[i3];
                if (!IsDefined(p3))
                {
                    return;
                }

                mesh.TriangleIndices.Add(i1);
                mesh.TriangleIndices.Add(i2);
                mesh.TriangleIndices.Add(i3);
            }
            /// <summary>
            /// Evaluates the surface at the specified u,v parameters.
            /// </summary>
            /// <param name="u">
            /// The u parameter.
            /// </param>
            /// <param name="v">
            /// The v parameter.
            /// </param>
            /// <param name="textureCoord">
            /// The texture coordinates.
            /// </param>
            /// <returns>
            /// The evaluated <see cref="Vector3"/>.
            /// </returns>
            protected abstract Vector3 Evaluate(double u, double v, out Vector2 textureCoord);

            /// <summary>
            /// Determines whether the specified point is defined.
            /// </summary>
            /// <param name="point">
            /// The point.
            /// </param>
            /// <returns>
            /// The is defined.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsDefined(Vector3 point)
            {
                return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z);
            }
        }
    }

}

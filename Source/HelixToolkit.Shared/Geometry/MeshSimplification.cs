/////////////////////////////////////////////
//
// Mesh Simplification
// Port from https://github.com/sp4cerat/Fast-Quadric-Mesh-Simplification
// Original Author: Sven Forstmann in 2014
// Port and Modified by Lunci 2017
// License : MIT
// http://opensource.org/licenses/MIT
/////////////////////////////////////////////

#if SHARPDX
#if NETFX_CORE
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
#else
namespace HelixToolkit.Wpf
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
#if SHARPDX
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    using Point = global::SharpDX.Vector2;
    using Int32Collection = IntCollection;
    using Vector3DCollection = Vector3Collection;
    using Point3DCollection = Vector3Collection;
    using PointCollection = Vector2Collection;
    using DoubleOrSingle = System.Single;
    using Matrix3D = global::SharpDX.Matrix;
    using HelixToolkit.Wpf;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using DoubleOrSingle = System.Double;
#endif
    /// <summary>
    /// Fast-Quadric-Mesh-Simplification, port from https://github.com/sp4cerat/Fast-Quadric-Mesh-Simplification
    /// </summary>
    public class MeshSimplification
    {
        private struct SymmetricMatrix
        {
            const int Size = 10;
            public double M11, M12, M13, M14, M22, M23, M24, M33, M34, M44;
            public SymmetricMatrix(double c = 0)
            {
                M11 = M12 = M13 = M14 = M22 = M23 = M24 = M33 = M34 = M44 = c;
            }

            public SymmetricMatrix(double a, double b, double c, double d)
            {
                M11 = a * a; M12 = a * b; M13 = a * c; M14 = a * d;
                M22 = b * b; M23 = b * c; M24 = b * d;
                M33 = c * c; M34 = c * d;
                M44 = d * d;
            }

            public SymmetricMatrix(double m11, double m12, double m13, double m14, double m22, double m23, double m24, double m33, double m34, double m44)
            {
                M11 = m11;
                M12 = m12;
                M13 = m13;
                M14 = m14;
                M22 = m22;
                M23 = m23;
                M24 = m24;
                M33 = m33;
                M34 = m34;
                M44 = m44;
            }

            public double this[int c]
            {
                get
                {
                    switch (c)
                    {
                        case 0: return M11;
                        case 1: return M12;
                        case 2: return M13;
                        case 3: return M14;
                        case 4: return M22;
                        case 5: return M23;
                        case 6: return M24;
                        case 7: return M33;
                        case 8: return M34;
                        case 9: return M44;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public double det(int a11, int a12, int a13, int a21, int a22, int a23, int a31, int a32, int a33)
            {
                double det = this[a11] * this[a22] * this[a33] + this[a13] * this[a21] * this[a32] + this[a12] * this[a23] * this[a31]
                            - this[a13] * this[a22] * this[a31] - this[a11] * this[a23] * this[a32] - this[a12] * this[a21] * this[a33];
                return det;
            }

            public static SymmetricMatrix operator +(SymmetricMatrix n1, SymmetricMatrix n2)
            {
                return new SymmetricMatrix(n1[0] + n2[0], n1[1] + n2[1], n1[2] + n2[2], n1[3] + n2[3],
                                                        n1[4] + n2[4], n1[5] + n2[5], n1[6] + n2[6],
                                                                     n1[7] + n2[7], n1[8] + n2[8],
                                                                                  n1[9] + n2[9]);
            }

            public void SetAll(double c)
            {
                M11 = M12 = M13 = M14 = M22 = M23 = M24 = M33 = M34 = M44 = c;
            }
        }

        private sealed class Triangle
        {
            public readonly int[] v = new int[3];
            public readonly double[] err = new double[4];
            public bool deleted = false;
            public bool dirty = false;
            public Vector3D normal = new Vector3D();

            public Triangle Clone()
            {
                var t = new Triangle() { deleted = this.deleted, dirty = this.dirty, normal = this.normal };
                t.v[0] = this.v[0];
                t.v[1] = this.v[1];
                t.v[2] = this.v[2];
                t.err[0] = this.err[0];
                t.err[1] = this.err[1];
                t.err[2] = this.err[2];
                t.err[3] = this.err[3];
                return t;
            }
        }

        private sealed class Vertex
        {
            public Vector3D p;
            public int tStart = 0;
            public int tCount = 0;
            public SymmetricMatrix q = new SymmetricMatrix();
            public bool border = false;
            public Vertex()
            {
                p = new Vector3D();
            }
            public Vertex(Point3D v)
            {
                p = new Vector3D(v.X, v.Y, v.Z);
            }
            public Vertex(ref Vector3D v)
            {
                p = v;
            }

            public Vertex Clone()
            {
                return new Vertex() { p = this.p, border = this.border, q = this.q, tCount = this.tCount, tStart = this.tStart };
            }
        }

        private struct Ref
        {
            public int tid;
            public int tvertex;
            public Ref(int id = 0, int tvert = 0)
            { tid = id; tvertex = tvert; }

            //public Ref Clone()
            //{
            //    return new Ref() { tid = this.tid, tvertex = this.tvertex };
            //}

            public void Reset()
            {
                tid = 0;
                tvertex = 0;
            }
        }

        private readonly List<Triangle> triangles;
        private readonly List<Vertex> vertices;
        private readonly List<Ref> refs;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public MeshSimplification(MeshGeometry3D model)
        {
            triangles = new List<Triangle>(Enumerable.Range(0, model.TriangleIndices.Count / 3).Select(x=>new Triangle()));
            int i = 0;
            foreach(var tri in triangles)
            {
                tri.v[0] = model.TriangleIndices[i++];
                tri.v[1] = model.TriangleIndices[i++];
                tri.v[2] = model.TriangleIndices[i++];
            }
            vertices = model.Positions.Select(x => new Vertex(x)).ToList();
            refs = new List<Ref>(Enumerable.Range(0, model.TriangleIndices.Count).Select(x=>new Ref()));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public MeshGeometry3D Simplify(bool verbose = false)
        {
            return Simplify(int.MaxValue, 7, verbose, true);
        }
        /// <summary>
        /// Mesh Simplification using Fast-Quadric-Mesh-Simplification
        /// </summary>
        /// <param name="targetCount">Target Number of Triangles</param>
        /// <param name="aggressive">sharpness to increase the threshold, 5->8 are usually good, more iteration yields higher quality</param>
        /// <param name="verbose"></param>
        /// <param name="lossless"></param>
        /// <returns></returns>
        public MeshGeometry3D Simplify(int targetCount, double aggressive = 7, bool verbose = false, bool lossless = false)
        {
            foreach (var tri in triangles)
            {
                tri.deleted = false;
            }
            int deletedTris = 0;
            var deleted0 = new List<bool>();
            var deleted1 = new List<bool>();
            int triCount = triangles.Count;
            int maxIteration = 9999;
            if (!lossless)
            {
                maxIteration = 100;
            }
            for (int iteration = 0; iteration < maxIteration; ++iteration)
            {
                if (!lossless && triCount - deletedTris <= targetCount)
                {
                    break;
                }
                if (lossless || iteration % 5 == 0)
                {
                    UpdateMesh(iteration);
                }

                foreach (var tri in triangles)
                {
                    tri.dirty = false;
                }
                //
                // All triangles with edges below the threshold will be removed
                //
                // The following numbers works well for most models.
                // If it does not, try to adjust the 3 parameters
                //
                double threshold = 0.001;
                if(!lossless)
                    threshold = 0.000000001 * Math.Pow(iteration + 3.0, aggressive);
                if (verbose)
                {
                    Debug.WriteLine($"Iteration: {iteration}; Triangles: {triCount - deletedTris}; Threshold: {threshold};");
                }

                foreach (var tri in triangles)
                {
                    if (tri.err[3] > threshold || tri.deleted || tri.dirty) { continue; }

                    for (int j = 0; j < 3; ++j)
                    {
                        if (tri.err[j] < threshold)
                        {
                            int i0 = tri.v[j];
                            var v0 = vertices[i0];
                            int i1 = tri.v[(j + 1) % 3];
                            var v1 = vertices[i1];
                            //border check
                            if (v0.border != v1.border)
                            {
                                continue;
                            }
                            //Compute vertex to collapse to
                            Vector3D p;
                            CalculateError(i0, i1, out p);
                            deleted0.Clear();
                            deleted1.Clear();
                            deleted0.AddRange(Enumerable.Repeat(false, v0.tCount));
                            deleted1.AddRange(Enumerable.Repeat(false, v1.tCount));

                            if (Flipped(ref p, i0, i1, ref v0, ref v1, deleted0)
                                || Flipped(ref p, i1, i0, ref v1, ref v0, deleted1))
                            { continue; }
                            v0.p = p;
                            v0.q = v1.q + v0.q;

                            int tStart = refs.Count;
                            UpdateTriangles(i0, ref v0, deleted0, ref deletedTris);
                            UpdateTriangles(i0, ref v1, deleted1, ref deletedTris);

                            int tcount = refs.Count - tStart;
                            if (tcount <= v0.tCount)
                            {
                                if (tcount > 0)
                                {
                                    for (int k = 0; k < tcount; ++k)
                                    {
                                        refs[v0.tStart + k] = refs[tStart + k];
                                    }
                                }
                            }
                            else
                            {
                                v0.tStart = tStart;
                            }

                            v0.tCount = tcount;
                            break;
                        }

                    }
                    if (!lossless && triCount - deletedTris <= targetCount)
                    {
                        break;
                    }
                }
                if (lossless)
                {
                    if (deletedTris <= 0)
                    {
                        break;
                    }
                    deletedTris = 0;
                }
            }
            CompactMesh();
            return GetMesh();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MeshGeometry3D GetMesh()
        {
            var pos = new Point3DCollection(vertices.Select(x=>new Point3D(x.p.X, x.p.Y, x.p.Z)));
            var tris = new Int32Collection(triangles.Count*3);
            foreach (var tri in triangles)
            {
                tris.Add(tri.v[0]);
                tris.Add(tri.v[1]);
                tris.Add(tri.v[2]);
            }
            return new MeshGeometry3D() { Positions = pos, TriangleIndices = tris };
        }

        private bool Flipped(ref Vector3D p, int i0, int i1, ref Vertex v0, ref Vertex v1, IList<bool> deleted)
        {
            for (int i = 0; i < v0.tCount; ++i)
            {
                var t = triangles[refs[v0.tStart + i].tid];
                if (t.deleted) { continue; }
                int s = refs[v0.tStart + i].tvertex;
                int id1 = t.v[(s + 1) % 3];
                int id2 = t.v[(s + 2) % 3];
                if (id1 == i1 || id2 == i1)
                {
                    deleted[i] = true;
                    continue;
                }

                Vector3D d1 = vertices[id1].p - p;
                d1.Normalize();
                Vector3D d2 = vertices[id2].p - p;
                d2.Normalize();
                if (SharedFunctions.DotProduct(ref d1, ref d2) > 0.999)
                {
                    return true;
                }
                var n = SharedFunctions.CrossProduct(ref d1, ref d2);
                n.Normalize();
                deleted[i] = false;
                if (SharedFunctions.DotProduct(ref n, ref t.normal) < 0.2)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateTriangles(int i0, ref Vertex v, IList<bool> deleted, ref int deletedTriangles)
        {
            Vector3D p;
            for (int i = 0; i < v.tCount; ++i)
            {
                var r = refs[v.tStart + i];
                var t = triangles[r.tid];
                if (t.deleted) { continue; }
                if (deleted[i])
                {
                    t.deleted = true;
                    deletedTriangles++;
                    continue;
                }

                t.v[r.tvertex] = i0;
                t.dirty = true;
                t.err[0] = CalculateError(t.v[0], t.v[1], out p);
                t.err[1] = CalculateError(t.v[1], t.v[2], out p);
                t.err[2] = CalculateError(t.v[2], t.v[0], out p);
                t.err[3] = Math.Min(t.err[0], Math.Min(t.err[1], t.err[2]));
                refs.Add(r);
            }
        }

        private double CalculateError(int id_v1, int id_v2, out Vector3D p_result)
        {
            p_result = new Vector3D();
            // compute interpolated vertex
            var q = vertices[id_v1].q + vertices[id_v2].q;
            bool border = vertices[id_v1].border & vertices[id_v2].border;
            double error = 0;

            double det = q.det(0, 1, 2, 1, 4, 5, 2, 5, 7);
            if (det != 0 && !border)
            {
                // q_delta is invertible
                p_result.X = (float)(-1 / det * (q.det(1, 2, 3, 4, 5, 6, 5, 7, 8))); // vx = A41/det(q_delta)
                p_result.Y = (float)(1 / det * (q.det(0, 2, 3, 1, 5, 6, 2, 7, 8)));  // vy = A42/det(q_delta)
                p_result.Z = (float)(-1 / det * (q.det(0, 1, 3, 1, 4, 6, 2, 5, 8))); // vz = A43/det(q_delta)

                error = VertexError(ref q, p_result.X, p_result.Y, p_result.Z);
            }
            else
            {
                // det = 0 -> try to find best result
                var p1 = vertices[id_v1].p;
                var p2 = vertices[id_v2].p;
                var p3 = (p1 + p2) / 2;
                double error1 = VertexError(ref q, p1.X, p1.Y, p1.Z);
                double error2 = VertexError(ref q, p2.X, p2.Y, p2.Z);
                double error3 = VertexError(ref q, p3.X, p3.Y, p3.Z);
                error = Math.Min(error1, Math.Min(error2, error3));
                if (error1 == error) p_result = p1;
                if (error2 == error) p_result = p2;
                if (error3 == error) p_result = p3;
            }
            return error;
        }

        private double VertexError(ref SymmetricMatrix q, double x, double y, double z)
        {
            return q.M11 * x * x + 2 * q.M12 * x * y + 2 * q.M13 * x * z + 2 * q.M14 * x + q.M22 * y * y
                 + 2 * q.M23 * y * z + 2 * q.M24 * y + q.M33 * z * z + 2 * q.M34 * z + q.M44;
        }

        private void UpdateMesh(int iteration)
        {
            if (iteration > 0) // compact triangles
            {
                int dst = 0;
                for (int i = 0; i < triangles.Count; ++i)
                {
                    if (!triangles[i].deleted)
                    {
                        triangles[dst++] = triangles[i];
                    }
                }
                triangles.RemoveRange(dst, triangles.Count - dst);
            }

            if (iteration == 0)
            {
                foreach (var vert in vertices)
                {
                    vert.q.SetAll(0);
                }

                foreach (var tri in triangles)
                {
                    var p0 = vertices[tri.v[0]].p;
                    var p1 = vertices[tri.v[1]].p;
                    var p2 = vertices[tri.v[2]].p;
                    var n = SharedFunctions.CrossProduct(p1 - p0, p2 - p0);
                    n.Normalize();
                    tri.normal = n;
                    for (int j = 0; j < 3; ++j)
                    {
                        vertices[tri.v[j]].q += new SymmetricMatrix(n.X, n.Y, n.Z, -SharedFunctions.DotProduct(ref n, ref p0));
                    }
                }
                Vector3D p;
                foreach (var tri in triangles)
                {
                    for (int i = 0; i < 3; ++i)
                    {                       
                        tri.err[i] = CalculateError(tri.v[i], tri.v[(i + 1) % 3], out p);
                    }
                    tri.err[3] = Math.Min(tri.err[0], Math.Min(tri.err[1], tri.err[2]));
                }
            }

            foreach (var vert in vertices)
            {
                vert.tStart = 0;
                vert.tCount = 0;
            }

            foreach (var tri in triangles)
            {
                vertices[tri.v[0]].tCount++;
                vertices[tri.v[1]].tCount++;
                vertices[tri.v[2]].tCount++;
            }

            int tstart = 0;
            foreach (var vert in vertices)
            {
                vert.tStart = tstart;
                tstart += vert.tCount;
                vert.tCount = 0;
            }
            int totalTris = triangles.Count * 3;
            if(refs.Count < totalTris)
            {
                refs.Clear();
                refs.AddRange(Enumerable.Range(0, totalTris).Select(x=>new Ref()));
            }
            else
            {
                refs.RemoveRange(totalTris, refs.Count - totalTris);
                refs.ForEach(x => x.Reset());
            }
            int count = 0;
            foreach (var tri in triangles)
            {
                for (int j = 0; j < 3; ++j)
                {
                    var v = vertices[tri.v[j]];
                    var r = refs[v.tStart + v.tCount];
                    r.tid = count;
                    r.tvertex = j;
                    refs[v.tStart + v.tCount] = r;
                    v.tCount++;
                    
                }
                ++count;
            }

            if (iteration == 0)
            {
                var vCount = new List<int>();
                var vids = new List<int>();
                foreach (var vert in vertices)
                {
                    vert.border = false;
                }

                foreach (var vert in vertices)
                {
                    vCount.Clear();
                    vids.Clear();
                    for (int j = 0; j < vert.tCount; ++j)
                    {
                        var t = triangles[refs[vert.tStart + j].tid];
                        for (int k = 0; k < 3; ++k)
                        {
                            int ofs = 0;
                            int id = t.v[k];
                            while (ofs < vCount.Count)
                            {
                                if (vids[ofs] == id) { break; }
                                ++ofs;
                            }
                            if (ofs == vCount.Count)
                            {
                                vCount.Add(1);
                                vids.Add(id);
                            }
                            else
                            {
                                vCount[ofs]++;
                            }
                        }
                    }

                    for (int j = 0; j < vCount.Count; ++j)
                    {
                        if (vCount[j] == 1)
                        {
                            vertices[vids[j]].border = true;
                        }
                    }
                }
            }
        }

        private void CompactMesh()
        {
            int dst = 0;
            foreach (var vert in vertices)
            {
                vert.tCount = 0;
            }

            for (int i = 0; i < triangles.Count; ++i)
            {
                if (!triangles[i].deleted)
                {
                    triangles[dst++] = triangles[i];
                    vertices[triangles[i].v[0]].tCount = 1;
                    vertices[triangles[i].v[1]].tCount = 1;
                    vertices[triangles[i].v[2]].tCount = 1;
                }
            }

            triangles.RemoveRange(dst, triangles.Count - dst);
            dst = 0;
            foreach (var vert in vertices)
            {
                if (vert.tCount > 0)
                {
                    vert.tStart = dst;
                    vertices[dst++].p = vert.p;
                }
            }

            foreach (var tri in triangles)
            {
                tri.v[0] = vertices[tri.v[0]].tStart;
                tri.v[1] = vertices[tri.v[1]].tStart;
                tri.v[2] = vertices[tri.v[2]].tStart;
            }

            vertices.RemoveRange(dst, vertices.Count - dst);
        }
    }
}

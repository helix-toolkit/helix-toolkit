// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OffReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A Geomview Object File Format (OFF) reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    using Object3DGroup = System.Collections.Generic.List<Object3D>;
    using Point = global::SharpDX.Vector2;
    using Point3D = global::SharpDX.Vector3;
    using global::SharpDX;
    using Model;

    /// <summary>
    /// A Geomview Object File Format (OFF) reader.
    /// </summary>
    /// <remarks>
    /// The reader does not parse colors, normals and texture coordinates.
    /// Only 3 dimensional vertices are supported.
    /// Homogeneous coordinates are not supported.
    /// See the following links for information about the file format:
    /// http://www.geomview.org/
    /// http://people.sc.fsu.edu/~jburkardt/data/off/off.html
    /// http://people.sc.fsu.edu/~jburkardt/html/off_format.html
    /// http://segeval.cs.princeton.edu/public/off_format.html
    /// http://paulbourke.net/dataformats/off/
    /// </remarks>
    [Obsolete("Suggest to use HelixToolkit.SharpDX.Assimp")]
    public class OffReader : IModelReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffReader" /> class.
        /// </summary>
        public OffReader()
        {
            this.Vertices = new List<Point3D>();

            // this.VertexColors = new List<Color>();
            // this.TexCoords = new PointCollection();
            // this.Normals = new Vector3DCollection();
            this.Faces = new List<int[]>();
        }

        /// <summary>
        /// Gets the faces.
        /// </summary>
        public IList<int[]> Faces { get; private set; }

        // public IList<Color> FaceColors { get; set; }
        // public IList<Color> VertexColors { get; set; }
        // public IList<Vector3D> Normals { get; set; }
        // public IList<Point> TexCoords { get; set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public IList<Point3D> Vertices { get; private set; }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="info">The model info.</param>
        /// <returns>The model.</returns>
        public Object3DGroup Read(string path, ModelInfo info = default(ModelInfo))
        {
            using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return this.Read(s, info);
            }            
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="info">The model info.</param>
        /// <returns>The model.</returns>
        public Object3DGroup Read(Stream s, ModelInfo info = default(ModelInfo))
        {
            this.Load(s);
            return this.BuildModel(info);
        }

        /// <summary>
        /// Creates a MeshGeometry3D object from the loaded file. Polygons are triangulated using triangle fans.
        /// </summary>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>
        /// A MeshGeometry3D.
        /// </returns>
        public MeshGeometry3D CreateMeshGeometry3D(ModelInfo info = default(ModelInfo))
        {
            var mb = new MeshBuilder(info.Normals, info.Tangents);
            foreach (var p in this.Vertices)
            {
                mb.Positions.Add(p);
            }

            foreach (var face in this.Faces)
            {
                mb.AddTriangleFan(face);
            }
            mb.ComputeNormalsAndTangents(info.Faces);
            return mb.ToMeshGeometry3D();
        }

        /// <summary>
        /// Creates a Model3D object from the loaded file.
        /// </summary>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>A Model3D group.</returns>
        public Object3DGroup BuildModel(ModelInfo info = default(ModelInfo))
        {
            Object3DGroup modelGroup = null;

            modelGroup = new Object3DGroup();
            var g = this.CreateMeshGeometry3D(info);
            var gm = new Object3D() { Geometry = g, Transform =  new List<Matrix>() };

            gm.Material = new PhongMaterialCore()
            {
                Name = "DefaultVRML",
                AmbientColor = new Color(0.2f, 0.2f, 0.2f, 1.0f),
                DiffuseColor = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                SpecularColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                EmissiveColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                SpecularShininess = 25.6f,
            };
            modelGroup.Add(gm);
            return modelGroup;
        }

        



        /// <summary>
        /// Loads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        private void Load(Stream s)
        {
            using (var reader = new StreamReader(s))
            {
                bool containsNormals = false;
                bool containsTextureCoordinates = false;
                bool containsColors = false;
                bool containsHomogeneousCoordinates = false;
                int vertexDimension = 3;
                bool nextLineContainsVertexDimension = false;
                bool nextLineContainsNumberOfVertices = false;
                int numberOfVertices = 0;
                int numberOfFaces = 0;
                // int numberOfEdges = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    if (line.StartsWith("#") || line.Length == 0)
                    {
                        continue;
                    }

                    if (nextLineContainsVertexDimension)
                    {
                        var values = GetIntValues(line);
                        vertexDimension = values[0];
                        nextLineContainsVertexDimension = false;
                        continue;
                    }

                    if (line.Contains("OFF"))
                    {
                        containsNormals = line.Contains("N");
                        containsColors = line.Contains("C");
                        containsTextureCoordinates = line.Contains("ST");
                        if (line.Contains("4"))
                        {
                            containsHomogeneousCoordinates = true;
                        }

                        if (line.Contains("n"))
                        {
                            nextLineContainsVertexDimension = true;
                        }

                        nextLineContainsNumberOfVertices = true;
                        continue;
                    }

                    if (nextLineContainsNumberOfVertices)
                    {
                        var values = GetIntValues(line);
                        numberOfVertices = values[0];
                        numberOfFaces = values[1];

                        /* numberOfEdges = values[2]; */
                        nextLineContainsNumberOfVertices = false;
                        continue;
                    }

                    if (this.Vertices.Count < numberOfVertices)
                    {
                        var x = new double[vertexDimension];
                        var values = GetValues(line);
                        int i = 0;
                        for (int j = 0; j < vertexDimension; j++)
                        {
                            x[j] = values[i++];
                        }

                        var n = new double[vertexDimension];
                        var uv = new double[2];
                        double w = 0;
                        if (containsHomogeneousCoordinates)
                        {
                            w = values[i++];
                        }

                        if (containsNormals)
                        {
                            for (int j = 0; j < vertexDimension; j++)
                            {
                                n[j] = values[i++];
                            }
                        }

                        if (containsColors)
                        {
                            // read color
                        }

                        if (containsTextureCoordinates)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                uv[j] = values[i++];
                            }
                        }

                        this.Vertices.Add(new Point3D((float)x[0], (float)x[1], (float)x[2]));

                        continue;
                    }

                    if (this.Faces.Count < numberOfFaces)
                    {
                        var values = GetIntValues(line);
                        int nv = values[0];
                        var vertices = new int[nv];
                        for (int i = 0; i < nv; i++)
                        {
                            vertices[i] = values[i + 1];
                        }

                        if (containsColors)
                        {
                            // read colorspec
                        }

                        this.Faces.Add(vertices);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Parses integer values from a string.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// Array of integer values.
        /// </returns>
        private static int[] GetIntValues(string input)
        {
            var fields = RemoveComments(input).SplitOnWhitespace();
            var result = new int[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                result[i] = int.Parse(fields[i]);
            }

            return result;
        }

        /// <summary>
        /// Parses double values from a string.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// Array of double values.
        /// </returns>
        private static double[] GetValues(string input)
        {
            var fields = RemoveComments(input).SplitOnWhitespace();
            var result = new double[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                result[i] = double.Parse(fields[i], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Removes comments from the line.
        /// </summary>
        /// <param name="input">
        /// The line.
        /// </param>
        /// <returns>
        /// A line without comments.
        /// </returns>
        private static string RemoveComments(string input)
        {
            int commentIndex = input.IndexOf('#');
            if (commentIndex >= 0)
            {
                return input.Substring(0, commentIndex);
            }

            return input;
        }
    }
}
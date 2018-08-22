/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Mathematics;
    using Model;
    using Model.Scene;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Export the 3D visual tree to a Wavefront OBJ file
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Obj
    /// http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// http://www.eg-models.de/formats/Format_Obj.html
    /// </remarks>
    public class ObjExporter : Exporter
    {
        /// <summary>
        /// Gets or sets a value indicating whether to export normals.
        /// </summary>
        public bool ExportNormals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use "d" for transparency (default is "Tr").
        /// </summary>
        public bool UseDissolveForTransparency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to switch Y and Z coordinates.
        /// </summary>
        public bool SwitchYZ { get; set; }

        /// <summary>
        /// The directory.
        /// </summary>
        private readonly string directory;

        /// <summary>
        /// The exported materials.
        /// </summary>
        private readonly Dictionary<MaterialCore, string> exportedMaterials = new Dictionary<MaterialCore, string>();

        /// <summary>
        /// The mwriter.
        /// </summary>
        private readonly StreamWriter mwriter;

        /// <summary>
        /// The writer.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// Normal index counter.
        /// </summary>
        private int normalIndex = 1;

        /// <summary>
        /// Texture index counter.
        /// </summary>
        private int textureIndex = 1;

        /// <summary>
        /// Vertex index counter.
        /// </summary>
        private int vertexIndex = 1;

        /// <summary>
        /// Object index counter.
        /// </summary>
        private int objectNo = 1;

        /// <summary>
        /// Group index counter.
        /// </summary>
        private int groupNo = 1;

        /// <summary>
        /// Material index counter.
        /// </summary>
        private int matNo = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjExporter"/> class.
        /// </summary>
        /// <param name="outputFileName">
        /// Name of the output file.
        /// </param>
        public ObjExporter(string outputFileName)
            : this(outputFileName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjExporter"/> class.
        /// </summary>
        /// <param name="outputFileName">
        /// Name of the output file.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        public ObjExporter(string outputFileName, string comment)
        {
            this.SwitchYZ = true;
            this.ExportNormals = false;

            var fullPath = Path.GetFullPath(outputFileName);
            var mtlPath = Path.ChangeExtension(outputFileName, ".mtl");
            string mtlFilename = Path.GetFileName(mtlPath);
            this.directory = Path.GetDirectoryName(fullPath);

            this.writer = new StreamWriter(outputFileName);
            this.mwriter = new StreamWriter(mtlPath);

            if (!string.IsNullOrEmpty(comment))
            {
                this.writer.WriteLine(string.Format("# {0}", comment));
            }

            this.writer.WriteLine("mtllib ./" + mtlFilename);
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public override void Close()
        {
            this.writer.Close();
            this.mwriter.Close();
            base.Close();
        }

        /// <summary>
        /// The export model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        protected override void ExportModel(MeshNode model, Transform3D transform)
        {
            if(model.GeometryValid && model.Material != null)
            {
                if(transform == null)
                {
                    transform = Transform3D.Identity;
                }
                this.writer.WriteLine(string.Format("o object{0}", this.objectNo++));
                this.writer.WriteLine(string.Format("g group{0}", this.groupNo++));

                if (this.exportedMaterials.ContainsKey(model.Material))
                {
                    string matName = this.exportedMaterials[model.Material];
                    this.writer.WriteLine(string.Format("usemtl {0}", matName));
                }
                else
                {
                    string matName = string.Format("mat{0}", this.matNo++);
                    this.writer.WriteLine(string.Format("usemtl {0}", matName));
                    this.ExportMaterial(matName, model.Material);
                    this.exportedMaterials.Add(model.Material, matName);
                }

                var mesh = model.Geometry as MeshGeometry3D;
                if (model.HasInstances)
                {
                    var m = transform.ToMatrix();
                    for(int i=0; i<model.Instances.Count; ++i)
                    {
                        this.ExportMesh(mesh, model.Instances[i] * m);
                    }
                }
                else
                {
                    this.ExportMesh(mesh, transform.ToMatrix());
                }
            }
        }

        /// <summary>
        /// The export mesh.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        public void ExportMesh(MeshGeometry3D m, Matrix t)
        {
            if (m == null)
            {
                throw new ArgumentNullException("m");
            }

            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            // mapping from local indices (0-based) to the obj file indices (1-based)
            var vertexIndexMap = new Dictionary<int, int>();
            var textureIndexMap = new Dictionary<int, int>();
            var normalIndexMap = new Dictionary<int, int>();

            int index = 0;
            if (m.Positions != null)
            {
                foreach (var v in m.Positions)
                {
                    vertexIndexMap.Add(index++, this.vertexIndex++);
                    var p = Vector3Helper.TransformCoordinate(v, t);
                    this.writer.WriteLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "v {0} {1} {2}",
                            p.X,
                            this.SwitchYZ ? p.Z : p.Y,
                            this.SwitchYZ ? -p.Y : p.Z));
                }

                this.writer.WriteLine(string.Format("# {0} vertices", index));
            }

            if (m.TextureCoordinates != null)
            {
                index = 0;
                foreach (var vt in m.TextureCoordinates)
                {
                    textureIndexMap.Add(index++, this.textureIndex++);
                    this.writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "vt {0} {1}", vt.X, 1 - vt.Y));
                }

                this.writer.WriteLine(string.Format("# {0} texture coordinates", index));
            }

            if (m.Normals != null && ExportNormals)
            {
                index = 0;
                foreach (var vn in m.Normals)
                {
                    normalIndexMap.Add(index++, this.normalIndex++);
                    this.writer.WriteLine(
                        string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", vn.X, vn.Y, vn.Z));
                }

                this.writer.WriteLine(string.Format("# {0} normals", index));
            }

            Func<int, string> formatIndices = i0 =>
            {
                bool hasTextureIndex = textureIndexMap.ContainsKey(i0);
                bool hasNormalIndex = normalIndexMap.ContainsKey(i0);
                if (hasTextureIndex && hasNormalIndex)
                {
                    return string.Format("{0}/{1}/{2}", vertexIndexMap[i0], textureIndexMap[i0], normalIndexMap[i0]);
                }

                if (hasTextureIndex)
                {
                    return string.Format("{0}/{1}", vertexIndexMap[i0], textureIndexMap[i0]);
                }

                if (hasNormalIndex)
                {
                    return string.Format("{0}//{1}", vertexIndexMap[i0], normalIndexMap[i0]);
                }

                return vertexIndexMap[i0].ToString();
            };

            if (m.Indices != null)
            {
                for (int i = 0; i < m.Indices.Count; i += 3)
                {
                    int i0 = m.Indices[i];
                    int i1 = m.Indices[i + 1];
                    int i2 = m.Indices[i + 2];

                    this.writer.WriteLine("f {0} {1} {2}", formatIndices(i0), formatIndices(i1), formatIndices(i2));
                }

                this.writer.WriteLine(string.Format("# {0} faces", m.Indices.Count / 3));
            }

            this.writer.WriteLine();
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="matName">
        /// The mat name.
        /// </param>
        /// <param name="material">
        /// The material.
        /// </param>
        private void ExportMaterial(string matName, MaterialCore material)
        {
            this.mwriter.WriteLine(string.Format("newmtl {0}", matName));
            var pm = material as PhongMaterialCore;

            if (pm != null)
            {
                if (pm.DiffuseMap == null)
                {
                    this.mwriter.WriteLine(string.Format("Kd {0}", this.ToColorString(pm.DiffuseColor)));

                    if (this.UseDissolveForTransparency)
                    {
                        // Dissolve factor
                        this.mwriter.WriteLine(
                            string.Format(CultureInfo.InvariantCulture, "d {0:F4}", pm.DiffuseColor.Alpha));
                    }
                    else
                    {
                        // Transparency
                        this.mwriter.WriteLine(
                            string.Format(CultureInfo.InvariantCulture, "Tr {0:F4}", pm.DiffuseColor.Alpha));
                    }
                }
                else
                {
                    var textureFilename = matName + ".png";
                    var texturePath = Path.Combine(this.directory, textureFilename);

                    // create .png bitmap file for the brush
                    RenderBrush(texturePath, pm.DiffuseMap);
                    this.mwriter.WriteLine(string.Format("map_Ka {0}", textureFilename));
                }
            }

            // Illumination model 1
            // This is a diffuse illumination model using Lambertian shading. The
            // color includes an ambient constant term and a diffuse shading term for
            // each light source.  The formula is
            // color = KaIa + Kd { SUM j=1..ls, (N * Lj)Ij }
            int illum = 1; // Lambertian

            if (pm != null)
            {
                this.mwriter.WriteLine(
                    string.Format(
                        "Ks {0}", this.ToColorString(pm.DiffuseMap == null ? pm.SpecularColor : new Color4(0.2f, 0.2f, 0.2f, 1.0f))));

                // Illumination model 2
                // This is a diffuse and specular illumination model using Lambertian
                // shading and Blinn's interpretation of Phong's specular illumination
                // model (BLIN77).  The color includes an ambient constant term, and a
                // diffuse and specular shading term for each light source.  The formula
                // is: color = KaIa + Kd { SUM j=1..ls, (N*Lj)Ij } + Ks { SUM j=1..ls, ((H*Hj)^Ns)Ij }
                illum = 2;

                // Specifies the specular exponent for the current material.  This defines the focus of the specular highlight.
                // "exponent" is the value for the specular exponent.  A high exponent results in a tight, concentrated highlight.  Ns values normally range from 0 to 1000.
                this.mwriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "Ns {0:F4}", pm.SpecularShininess));
            }

            // roughness
            this.mwriter.WriteLine(string.Format("Ns {0}", 2));

            // Optical density (index of refraction)
            this.mwriter.WriteLine(string.Format("Ni {0}", 1));

            // Transmission filter
            this.mwriter.WriteLine(string.Format("Tf {0} {1} {2}", 1, 1, 1));

            // Illumination model
            // Illumination    Properties that are turned on in the
            // model           Property Editor
            // 0		Color on and Ambient off
            // 1		Color on and Ambient on
            // 2		Highlight on
            // 3		Reflection on and Ray trace on
            // 4		Transparency: Glass on
            // Reflection: Ray trace on
            // 5		Reflection: Fresnel on and Ray trace on
            // 6		Transparency: Refraction on
            // Reflection: Fresnel off and Ray trace on
            // 7		Transparency: Refraction on
            // Reflection: Fresnel on and Ray trace on
            // 8		Reflection on and Ray trace off
            // 9		Transparency: Glass on
            // Reflection: Ray trace off
            // 10		Casts shadows onto invisible surfaces
            this.mwriter.WriteLine(string.Format("illum {0}", illum));
        }

        /// <summary>
        /// Converts a color to a string.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <returns>
        /// The string.
        /// </returns>
        private string ToColorString(Color4 color)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:F4} {1:F4} {2:F4}",
                color.Red,
                color.Green,
                color.Blue);
        }
    }
}

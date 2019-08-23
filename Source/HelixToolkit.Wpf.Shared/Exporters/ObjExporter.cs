// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Export the 3D visual tree to a Wavefront OBJ file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Export the 3D visual tree to a Wavefront OBJ file
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Obj
    /// http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// http://www.eg-models.de/formats/Format_Obj.html
    /// </remarks>
    public class ObjExporter : Exporter<ObjExporter.ObjWriters>
    {
        /// <summary>
        /// The exported materials.
        /// </summary>
        private readonly Dictionary<Material, string> exportedMaterials = new Dictionary<Material, string>();

        /// <summary>
        /// The group no.
        /// </summary>
        private int groupNo = 1;

        /// <summary>
        /// The mat no.
        /// </summary>
        private int matNo = 1;

        /// <summary>
        /// Normal index counter.
        /// </summary>
        private int normalIndex = 1;

        /// <summary>
        /// The object no.
        /// </summary>
        private int objectNo = 1;

        /// <summary>
        /// Texture index counter.
        /// </summary>
        private int textureIndex = 1;

        /// <summary>
        /// Vertex index counter.
        /// </summary>
        private int vertexIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjExporter" /> class.
        /// </summary>
        public ObjExporter()
        {
            this.TextureExtension = ".png";
            this.TextureSize = 1024;
            this.TextureQualityLevel = 90;
            this.TextureFolder = ".";

            this.SwitchYZ = false;
            this.ExportNormals = false;
            this.FileCreator = File.Create;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to export normals.
        /// </summary>
        public bool ExportNormals { get; set; }

        /// <summary>
        /// Gets or sets the texture image and materials file creator.
        /// </summary>
        /// <value>A function used to create streams for texture images and material files.</value>
        public Func<string, Stream> FileCreator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use "d" for transparency (default is "Tr").
        /// </summary>
        public bool UseDissolveForTransparency { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the materials file.
        /// </summary>
        /// <value>
        /// The materials file.
        /// </value>
        public string MaterialsFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to switch Y and Z coordinates.
        /// </summary>
        public bool SwitchYZ { get; set; }

        /// <summary>
        /// Gets or sets the texture folder.
        /// </summary>
        public string TextureFolder { get; set; }

        /// <summary>
        /// Gets or sets the texture extension (.png or .jpg).
        /// </summary>
        /// <value>
        /// The default value is ".png".
        /// </value>
        public string TextureExtension { get; set; }

        /// <summary>
        /// Gets or sets the texture size.
        /// </summary>
        /// <value>
        /// The default value is 1024.
        /// </value>
        public int TextureSize { get; set; }

        /// <summary>
        /// Gets or sets the texture quality level (for JPEG encoding).
        /// </summary>
        /// <value>
        /// The quality level of the JPEG image. The value range is 1 (lowest quality) to 100 (highest quality) inclusive. 
        /// The default value is 90.
        /// </value>
        public int TextureQualityLevel { get; set; }

        /// <summary>
        /// Exports the mesh.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="m">The mesh geometry.</param>
        /// <param name="t">The transform.</param>
        public void ExportMesh(StreamWriter writer, MeshGeometry3D m, Transform3D t)
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
                    var p = t.Transform(v);
                    writer.WriteLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "v {0} {1} {2}",
                            p.X,
                            this.SwitchYZ ? p.Z : p.Y,
                            this.SwitchYZ ? -p.Y : p.Z));
                }

                writer.WriteLine(string.Format("# {0} vertices", index));
            }

            if (m.TextureCoordinates != null)
            {
                index = 0;
                foreach (var vt in m.TextureCoordinates)
                {
                    textureIndexMap.Add(index++, this.textureIndex++);
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "vt {0} {1}", vt.X, 1 - vt.Y));
                }

                writer.WriteLine(string.Format("# {0} texture coordinates", index));
            }

            if (m.Normals != null && ExportNormals)
            {
                index = 0;
                foreach (var vn in m.Normals)
                {
                    normalIndexMap.Add(index++, this.normalIndex++);
                    writer.WriteLine(
                        string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", vn.X, this.SwitchYZ ? vn.Z : vn.Y, this.SwitchYZ ? -vn.Y : vn.Z));
                }

                writer.WriteLine(string.Format("# {0} normals", index));
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

            if (m.TriangleIndices != null)
            {
                for (int i = 0; i < m.TriangleIndices.Count; i += 3)
                {
                    int i0 = m.TriangleIndices[i];
                    int i1 = m.TriangleIndices[i + 1];
                    int i2 = m.TriangleIndices[i + 2];

                    writer.WriteLine("f {0} {1} {2}", formatIndices(i0), formatIndices(i1), formatIndices(i2));
                }

                writer.WriteLine(string.Format("# {0} faces", m.TriangleIndices.Count / 3));
            }

            writer.WriteLine();
        }

        /// <summary>
        /// Creates the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>StreamWriter.</returns>
        protected override ObjWriters Create(Stream stream)
        {
            if (this.MaterialsFile == null)
            {
                throw new InvalidOperationException("The `MaterialsFile` property must be set.");
            }

            var writer = new StreamWriter(stream);

            if (!string.IsNullOrEmpty(this.Comment))
            {
                writer.WriteLine("# {0}", this.Comment);
            }

            writer.WriteLine("mtllib ./" + this.MaterialsFile);

            var materialStream = this.FileCreator(this.MaterialsFile);
            var materialWriter = new StreamWriter(materialStream);

            return new ObjWriters { ObjWriter = writer, MaterialsWriter = materialWriter };
        }

        /// <summary>
        /// Closes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Close(ObjWriters writer)
        {
            writer.ObjWriter.Close();
            writer.MaterialsWriter.Close();
        }

        /// <summary>
        /// The export model.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="model">The model.</param>
        /// <param name="transform">The transform.</param>
        protected override void ExportModel(ObjWriters writer, GeometryModel3D model, Transform3D transform)
        {
            writer.ObjWriter.WriteLine("o object{0}", this.objectNo++);
            writer.ObjWriter.WriteLine("g group{0}", this.groupNo++);

            if (this.exportedMaterials.ContainsKey(model.Material))
            {
                string matName = this.exportedMaterials[model.Material];
                writer.ObjWriter.WriteLine("usemtl {0}", matName);
            }
            else
            {
                string matName = string.Format("mat{0}", this.matNo++);
                writer.ObjWriter.WriteLine("usemtl {0}", matName);
                this.ExportMaterial(writer.MaterialsWriter, matName, model.Material, model.BackMaterial);
                this.exportedMaterials.Add(model.Material, matName);
            }

            var mesh = model.Geometry as MeshGeometry3D;
            this.ExportMesh(writer.ObjWriter, mesh, Transform3DHelper.CombineTransform(transform, model.Transform));
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="materialWriter">The material writer.</param>
        /// <param name="matName">The mat name.</param>
        /// <param name="material">The material.</param>
        /// <param name="backMaterial">The back material.</param>
        private void ExportMaterial(StreamWriter materialWriter, string matName, Material material, Material backMaterial)
        {
            materialWriter.WriteLine("newmtl {0}", matName);
            var dm = material as DiffuseMaterial;
            var sm = material as SpecularMaterial;
            var mg = material as MaterialGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    if (m is DiffuseMaterial)
                    {
                        dm = m as DiffuseMaterial;
                    }

                    if (m is SpecularMaterial)
                    {
                        sm = m as SpecularMaterial;
                    }
                }
            }

            if (dm != null)
            {
                var adjustedAmbientColor = dm.AmbientColor.ChangeIntensity(0.2);

                // materialWriter.WriteLine(string.Format("Ka {0}", this.ToColorString(adjustedAmbientColor)));
                var scb = dm.Brush as SolidColorBrush;
                if (scb != null)
                {
                    materialWriter.WriteLine(string.Format("Kd {0}", this.ToColorString(scb.Color)));

                    if (this.UseDissolveForTransparency)
                    {
                        // Dissolve factor
                        materialWriter.WriteLine(
                            string.Format(CultureInfo.InvariantCulture, "d {0:F4}", scb.Color.A / 255.0));
                    }
                    else
                    {
                        // Transparency
                        materialWriter.WriteLine(
                            string.Format(CultureInfo.InvariantCulture, "Tr {0:F4}", scb.Color.A / 255.0));
                    }
                }
                else
                {
                    var textureFilename = matName + this.TextureExtension;
                    var texturePath = Path.Combine(this.TextureFolder, textureFilename);
                    using (var s = this.FileCreator(texturePath))
                    {
                        // create bitmap file for the brush
                        if (this.TextureExtension == ".jpg")
                        {
                            this.RenderBrush(s, dm.Brush, this.TextureSize, this.TextureSize, this.TextureQualityLevel);
                        }
                        else
                        {
                            this.RenderBrush(s, dm.Brush, this.TextureSize, this.TextureSize);
                        }
                    }

                    materialWriter.WriteLine(string.Format("map_Kd {0}", textureFilename));
                }
            }

            // Illumination model 1
            // This is a diffuse illumination model using Lambertian shading. The
            // color includes an ambient constant term and a diffuse shading term for
            // each light source.  The formula is
            // color = KaIa + Kd { SUM j=1..ls, (N * Lj)Ij }
            int illum = 1; // Lambertian

            if (sm != null)
            {
                var scb = sm.Brush as SolidColorBrush;
                materialWriter.WriteLine(
                    string.Format(
                        "Ks {0}", this.ToColorString(scb != null ? scb.Color : Color.FromScRgb(1.0f, 0.2f, 0.2f, 0.2f))));

                // Illumination model 2
                // This is a diffuse and specular illumination model using Lambertian
                // shading and Blinn's interpretation of Phong's specular illumination
                // model (BLIN77).  The color includes an ambient constant term, and a
                // diffuse and specular shading term for each light source.  The formula
                // is: color = KaIa + Kd { SUM j=1..ls, (N*Lj)Ij } + Ks { SUM j=1..ls, ((H*Hj)^Ns)Ij }
                illum = 2;

                // Specifies the specular exponent for the current material.  This defines the focus of the specular highlight.
                // "exponent" is the value for the specular exponent.  A high exponent results in a tight, concentrated highlight.  Ns values normally range from 0 to 1000.
                materialWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "Ns {0:F4}", sm.SpecularPower));
            }

            // roughness
            materialWriter.WriteLine(string.Format("Ns {0}", 2));

            // Optical density (index of refraction)
            materialWriter.WriteLine(string.Format("Ni {0}", 1));

            // Transmission filter
            materialWriter.WriteLine(string.Format("Tf {0} {1} {2}", 1, 1, 1));

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
            materialWriter.WriteLine("illum {0}", illum);
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
        private string ToColorString(Color color)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:F4} {1:F4} {2:F4}", color.R / 255.0, color.G / 255.0, color.B / 255.0);
        }

        /// <summary>
        /// Represents the stream writers for the <see cref="ObjExporter"/>.
        /// </summary>
        public class ObjWriters
        {
            /// <summary>
            /// Gets or sets the object file writer.
            /// </summary>
            public StreamWriter ObjWriter { get; set; }

            /// <summary>
            /// Gets or sets the material file writer.
            /// </summary>
            public StreamWriter MaterialsWriter { get; set; }
        }
    }
}
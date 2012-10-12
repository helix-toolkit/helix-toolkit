// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColladaExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml;

    /// <summary>
    /// Exports the 3D visual tree to a Collada 1.5.0 file.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/COLLADA
    /// http://www.khronos.org/collada/
    /// https://collada.org/mediawiki/index.php/COLLADA_-_Digital_Asset_and_FX_Exchange_Schema
    /// https://collada.org/mediawiki/index.php/COLLADA.net
    /// http://www.mogware.com/index.php?page=collada.NET
    /// http://www.okino.com/conv/exp_collada.htm
    /// </remarks>
    public class ColladaExporter : Exporter
    {
        /// <summary>
        /// The effect dictionary.
        /// </summary>
        private readonly Dictionary<Material, string> effects = new Dictionary<Material, string>();

        /// <summary>
        /// The geometry dictionary.
        /// </summary>
        private readonly Dictionary<MeshGeometry3D, string> geometries = new Dictionary<MeshGeometry3D, string>();

        /// <summary>
        /// The light dictionary.
        /// </summary>
        private readonly Dictionary<Light, string> lights = new Dictionary<Light, string>();

        /// <summary>
        /// The material dictionary.
        /// </summary>
        private readonly Dictionary<Material, string> materials = new Dictionary<Material, string>();

        /// <summary>
        /// The node dictionary.
        /// </summary>
        private readonly Dictionary<Model3D, string> nodes = new Dictionary<Model3D, string>();

        /// <summary>
        /// The writer.
        /// </summary>
        private readonly XmlTextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColladaExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public ColladaExporter(string path)
        {
            this.writer = new XmlTextWriter(path, Encoding.UTF8) { Formatting = Formatting.Indented };
            this.writer.WriteStartDocument(false);
            this.writer.WriteStartElement("COLLADA");
            this.writer.WriteAttributeString("xmlns", "http://www.collada.org/2008/03/COLLADASchema");
            this.writer.WriteAttributeString("version", "1.5.0");
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        public string Copyright { get; set; }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public override void Close()
        {
            this.writer.WriteEndElement(); // COLLADA
            this.writer.WriteEndDocument();
            this.writer.Close();
            base.Close();
        }

        /// <summary>
        /// Exports the specified viewport.
        /// Exports model, camera and lights.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public override void Export(Viewport3D viewport)
        {
            this.ExportViewport(viewport);

            // Export camera

            // Export lights
            Visual3DHelper.Traverse<Light>(viewport.Children, this.ExportLight);

            this.writer.WriteStartElement("library_materials");
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, this.ExportMaterial);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("library_effects");
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, this.ExportEffect);
            this.writer.WriteEndElement();

            // writer.WriteStartElement("library_cameras");
            // this.ExportCamera(viewport.Camera);
            // writer.WriteEndElement();

            // writer.WriteStartElement("library_lights");
            // Visual3DHelper.Traverse<Light>(viewport.Children, this.ExportLight);
            // writer.WriteEndElement();
            this.writer.WriteStartElement("library_geometries");
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, this.ExportGeometry);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("library_nodes");
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, this.ExportNode);
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("library_visual_scenes");
            this.writer.WriteStartElement("visual_scene");
            this.writer.WriteAttributeString("id", "RootNode");
            this.writer.WriteAttributeString("name", "RootNode");

            // this.ExportCameraNode(viewport.Camera);
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, this.ExportSceneNode);
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("scene");
            this.writer.WriteStartElement("instance_visual_scene");
            this.writer.WriteAttributeString("url", "#RootNode");
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Exports the model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="inheritedTransform">
        /// The inherited transform.
        /// </param>
        protected override void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
            // var mesh = model.Geometry as MeshGeometry3D;
            // var indices = new StringBuilder();
            // foreach (int i in mesh.TriangleIndices)
            // {
            // indices.Append(i + " ");
            // }

            // var points = new StringBuilder();
            // foreach (var pt in mesh.Positions)
            // {
            // points.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", pt.X, pt.Y, pt.Z);
            // }

            // this.writer.WriteStartElement("Transform");
            // this.writer.WriteStartElement("Shape");
            // this.writer.WriteStartElement("IndexedFaceSet");
            // this.writer.WriteAttributeString("coordIndex", indices.ToString());
            // this.writer.WriteStartElement("Coordinate");
            // this.writer.WriteAttributeString("point", points.ToString());
            // this.writer.WriteEndElement();
            // this.writer.WriteEndElement(); // IndexedFaceSet
            // this.writer.WriteStartElement("Appearance");

            // this.writer.WriteStartElement("Material");
            // this.writer.WriteAttributeString("diffuseColor", "0.8 0.8 0.2");
            // this.writer.WriteAttributeString("specularColor", "0.5 0.5 0.5");
            // this.writer.WriteEndElement();
            // this.writer.WriteEndElement(); // Appearance

            // this.writer.WriteEndElement(); // Shape
            // this.writer.WriteEndElement(); // Transform
        }

        /// <summary>
        /// Exports the viewport.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        protected override void ExportViewport(Viewport3D viewport)
        {
            base.ExportViewport(viewport);
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            var assemblyName = assembly.GetName();
            var authoringTool = string.Format("{0} {1}", assemblyName.Name, assemblyName.Version.ToString(3));

            var created = DateTime.Now;
            var createdString = created.ToString("u").Replace(' ', 'T');
            var upaxis = "Z_UP";
            var pcamera = viewport.Camera as ProjectionCamera;
            if (pcamera != null && pcamera.UpDirection.Y > pcamera.UpDirection.Z)
            {
                upaxis = "Y_UP";
            }

            this.writer.WriteStartElement("asset");

            this.writer.WriteStartElement("contributor");
            if (this.Author != null)
            {
                this.writer.WriteElementString("author", this.Author);
            }

            if (this.Copyright != null)
            {
                this.writer.WriteElementString("copyright", this.Copyright);
            }

            if (this.Comments != null)
            {
                this.writer.WriteElementString("comments", this.Comments);
            }

            this.writer.WriteElementString("authoring_tool", authoringTool);
            this.writer.WriteEndElement(); // contributor

            this.writer.WriteElementString("created", createdString);
            this.writer.WriteElementString("modified", createdString);

            // this.writer.WriteStartElement("unit");
            // writer.WriteAttributeString("meter", "1.0");
            // writer.WriteAttributeString("name", "meter");
            // this.writer.WriteEndElement(); // unit
            this.writer.WriteElementString("up_axis", upaxis);
            this.writer.WriteEndElement(); // asset
        }

        /// <summary>
        /// The bind material.
        /// </summary>
        /// <param name="geometryId">
        /// The geometry id.
        /// </param>
        /// <param name="materialId">
        /// The material id.
        /// </param>
        private void BindMaterial(string geometryId, string materialId)
        {
            this.writer.WriteStartElement("instance_geometry");
            this.writer.WriteAttributeString("url", "#" + geometryId);
            this.writer.WriteStartElement("bind_material");
            this.writer.WriteStartElement("technique_common");
            this.writer.WriteStartElement("instance_material");
            this.writer.WriteAttributeString("symbol", "Material2");
            this.writer.WriteAttributeString("target", "#" + materialId);
            this.writer.WriteStartElement("bind_vertex_input");
            this.writer.WriteAttributeString("semantic", "UVSET0");
            this.writer.WriteAttributeString("input_semantic", "TEXCOORD");
            this.writer.WriteAttributeString("input_set", "0");
            this.writer.WriteEndElement(); // bind_vertex_input
            this.writer.WriteEndElement(); // instance_material
            this.writer.WriteEndElement(); // technique_common
            this.writer.WriteEndElement(); // bind_material
            this.writer.WriteEndElement(); // instance_geometry
        }

        /// <summary>
        /// The export effect.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ExportEffect(GeometryModel3D model, Transform3D transform)
        {
            this.ExportEffect(model.Material);
            this.ExportEffect(model.BackMaterial);
        }

        /// <summary>
        /// The export effect.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        private void ExportEffect(Material m)
        {
            if (m == null)
            {
                return;
            }

            string id = this.effects[m];
            this.writer.WriteStartElement("effect");
            this.writer.WriteAttributeString("id", id);
            this.writer.WriteAttributeString("name", id);
            this.writer.WriteStartElement("profile_COMMON");
            this.writer.WriteStartElement("technique");
            this.writer.WriteAttributeString("sid", "common");
            this.writer.WriteStartElement("phong");

            var diffuse = MaterialHelper.GetFirst<DiffuseMaterial>(m);
            var specular = MaterialHelper.GetFirst<SpecularMaterial>(m);
            var emissive = MaterialHelper.GetFirst<EmissiveMaterial>(m);
            if (emissive != null)
            {
                this.WritePhongMaterial("emission", emissive.Color);
            }

            if (diffuse != null)
            {
                this.WritePhongMaterial("diffuse", diffuse.Color);
            }

            if (specular != null)
            {
                this.WritePhongMaterial("specular", specular.Color);
            }

            this.writer.WriteEndElement(); // phong
            this.writer.WriteEndElement(); // technique
            this.writer.WriteEndElement(); // profile_COMMON
            this.writer.WriteEndElement(); // effect
        }

        /// <summary>
        /// The export geometry.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ExportGeometry(GeometryModel3D model, Transform3D transform)
        {
            var mg = model.Geometry as MeshGeometry3D;
            if (mg == null)
            {
                throw new InvalidOperationException("Model is not a MeshGeometry3D.");
            }

            this.writer.WriteStartElement("geometry");
            this.writer.WriteStartElement("mesh");

            // write positions
            int id = this.geometries.Count;
            string meshId = "mesh" + id;
            this.geometries.Add(mg, meshId);

            this.writer.WriteStartElement("source");
            string positionsId = "p" + id;
            this.writer.WriteAttributeString("id", positionsId);
            this.writer.WriteStartElement("float_array");
            string positionsArrayId = positionsId + "-array";
            this.writer.WriteAttributeString("id", positionsArrayId);
            this.writer.WriteAttributeString("count", (mg.Positions.Count * 3).ToString(CultureInfo.InvariantCulture));
            var psb = new StringBuilder();
            foreach (var p in mg.Positions)
            {
                psb.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", p.X, p.Y, p.Z);
            }

            this.writer.WriteRaw(psb.ToString());
            this.writer.WriteEndElement(); // float array

            this.writer.WriteStartElement("technique_common");
            this.writer.WriteStartElement("accessor");
            this.writer.WriteAttributeString("source", "#" + positionsArrayId);
            this.writer.WriteAttributeString("count", mg.Positions.Count.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteAttributeString("stride", "3");
            this.writer.WriteStartElement("param");
            this.writer.WriteAttributeString("name", "X");
            this.writer.WriteAttributeString("type", "float");
            this.writer.WriteEndElement(); // param
            this.writer.WriteStartElement("param");
            this.writer.WriteAttributeString("name", "Y");
            this.writer.WriteAttributeString("type", "float");
            this.writer.WriteEndElement(); // param
            this.writer.WriteStartElement("param");
            this.writer.WriteAttributeString("name", "Z");
            this.writer.WriteAttributeString("type", "float");
            this.writer.WriteEndElement(); // param
            this.writer.WriteEndElement(); // accessor
            this.writer.WriteEndElement(); // technique_common

            this.writer.WriteEndElement(); // source

            this.writer.WriteStartElement("vertices");
            string verticesId = "v" + id;
            this.writer.WriteAttributeString("id", verticesId);

            this.writer.WriteStartElement("input");
            this.writer.WriteAttributeString("semantic", "POSITION");
            this.writer.WriteAttributeString("source", "#" + positionsId);
            this.writer.WriteEndElement(); // input

            // writer.WriteStartElement("input");
            // writer.WriteAttributeString("semantic", "NORMAL");
            // writer.WriteAttributeString("source", normalsId);
            // writer.WriteEndElement(); // input
            this.writer.WriteEndElement(); // vertices

            this.writer.WriteStartElement("triangles");
            this.writer.WriteAttributeString("count", mg.TriangleIndices.Count.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteAttributeString("material", "xx");
            this.writer.WriteStartElement("input");
            this.writer.WriteAttributeString("offset", "0");
            this.writer.WriteAttributeString("semantic", "VERTEX");
            this.writer.WriteAttributeString("source", "#" + verticesId);
            this.writer.WriteEndElement(); // input
            var sb = new StringBuilder();
            foreach (var i in mg.TriangleIndices)
            {
                sb.Append(i + " ");
            }

            this.writer.WriteElementString("p", sb.ToString());
            this.writer.WriteEndElement(); // triangles

            this.writer.WriteEndElement(); // mesh
            this.writer.WriteEndElement(); // geometry
        }

        /// <summary>
        /// The export light.
        /// </summary>
        /// <param name="light">
        /// The light.
        /// </param>
        private void ExportLight(Light light)
        {
            if (light == null || this.lights.ContainsKey(light))
            {
                return;
            }

            string id = "light_" + this.lights.Count;
            this.lights.Add(light, id);
            this.writer.WriteStartElement("light");
            this.writer.WriteAttributeString("id", id);
            this.writer.WriteAttributeString("name", id);
            this.writer.WriteStartElement("technique_common");

            var al = light as AmbientLight;
            if (al != null)
            {
                this.writer.WriteStartElement("ambient");
                this.WriteColor(al.Color);
                this.writer.WriteEndElement();
            }

            var dl = light as DirectionalLight;
            if (dl != null)
            {
                this.writer.WriteStartElement("directional");
                this.WriteColor(dl.Color);
                this.writer.WriteEndElement();
            }

            var pl = light as PointLight;
            if (pl != null)
            {
                this.writer.WriteStartElement("point");
                this.WriteColor(pl.Color);
                this.WriteDouble("constant_attenuation", pl.ConstantAttenuation);
                this.WriteDouble("linear_attenuation", pl.LinearAttenuation);
                this.WriteDouble("quadratic_attenuation", pl.QuadraticAttenuation);
                this.writer.WriteEndElement();
            }

            var sl = light as SpotLight;
            if (sl != null)
            {
                this.writer.WriteStartElement("spot");
                this.WriteColor(sl.Color);
                this.WriteDouble("constant_attenuation", sl.ConstantAttenuation);
                this.WriteDouble("linear_attenuation", sl.LinearAttenuation);
                this.WriteDouble("quadratic_attenuation", sl.QuadraticAttenuation);
                this.WriteDouble("falloff_angle", sl.InnerConeAngle);

                // writer.WriteElementString("falloff_exponent",sl.xxx.ToString(CultureInfo.InvariantCulture));
                this.writer.WriteEndElement();
            }

            this.writer.WriteEndElement(); // technique_common
            this.writer.WriteEndElement(); // light
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ExportMaterial(GeometryModel3D model, Transform3D transform)
        {
            this.ExportMaterial(model.Material);
            this.ExportMaterial(model.BackMaterial);
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        private void ExportMaterial(Material m)
        {
            if (m == null || this.materials.ContainsKey(m))
            {
                return;
            }

            string id = "material_" + this.materials.Count;
            string effectid = "effect_" + this.materials.Count;
            this.materials.Add(m, id);
            this.effects.Add(m, effectid);
            this.writer.WriteStartElement("material");
            this.writer.WriteAttributeString("id", id);
            this.writer.WriteAttributeString("name", id);
            this.writer.WriteStartElement("instance_effect");
            this.writer.WriteAttributeString("url", "#" + effectid);
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// The export node.
        /// </summary>
        /// <param name="gm">
        /// The gm.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ExportNode(GeometryModel3D gm, Transform3D transform)
        {
            var mg = gm.Geometry as MeshGeometry3D;
            if (mg == null)
            {
                throw new InvalidOperationException("Model is not a MeshGeometry3D.");
            }

            string geometryId = this.geometries[mg];
            string nodeId = geometryId + "-node";
            this.nodes.Add(gm, nodeId);
            this.writer.WriteStartElement("node");
            this.writer.WriteAttributeString("id", nodeId);
            this.writer.WriteAttributeString("name", nodeId);
            string frontMaterialId;
            string backMaterialId;
            if (gm.Material != null && this.materials.TryGetValue(gm.Material, out frontMaterialId))
            {
                this.BindMaterial(geometryId, frontMaterialId);
            }

            if (gm.BackMaterial != null && this.materials.TryGetValue(gm.BackMaterial, out backMaterialId))
            {
                this.BindMaterial(geometryId, backMaterialId);
            }

            this.writer.WriteEndElement(); // node
        }

        /// <summary>
        /// The export scene node.
        /// </summary>
        /// <param name="gm">
        /// The gm.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ExportSceneNode(Model3D gm, Transform3D transform)
        {
            string nodeId = this.nodes[gm];
            string instanceId = nodeId + "-instance";
            this.writer.WriteStartElement("node");
            this.writer.WriteAttributeString("id", instanceId);
            this.writer.WriteAttributeString("name", instanceId);
            var totalTransform = Transform3DHelper.CombineTransform(transform, gm.Transform);
            this.WriteMatrix("matrix", totalTransform.Value);
            this.writer.WriteStartElement("instance_node");
            this.writer.WriteAttributeString("url", "#" + nodeId);
            this.writer.WriteEndElement(); // instance node
            this.writer.WriteEndElement(); // node
        }

        /// <summary>
        /// The write color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        private void WriteColor(Color color)
        {
            // this.writer.WriteElementString("color", string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", color.R / 255.0, color.G / 255.0, color.B / 255.0));
            this.writer.WriteElementString(
                "color",
                string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0));
        }

        /// <summary>
        /// The write double.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteDouble(string name, double value)
        {
            this.writer.WriteElementString(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The write matrix.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="m">
        /// The m.
        /// </param>
        private void WriteMatrix(string name, Matrix3D m)
        {
            string value = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
                m.M11,
                m.M12,
                m.M13,
                m.OffsetX,
                m.M21,
                m.M22,
                m.M23,
                m.OffsetY,
                m.M31,
                m.M32,
                m.M33,
                m.OffsetZ,
                0,
                0,
                0,
                1);

            this.writer.WriteElementString(name, value);
        }

        /// <summary>
        /// The write phong material.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void WritePhongMaterial(string name, Color color)
        {
            this.writer.WriteStartElement(name);
            this.WriteColor(color);
            this.writer.WriteEndElement();
        }

    }
}
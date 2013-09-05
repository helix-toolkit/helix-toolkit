// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KerkytheaExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Xml;

    /// <summary>
    /// Exports the 3D visual tree to a Kerkythea xml file.
    /// </summary>
    /// <remarks>
    /// Kerkythea: http://www.kerkythea.net/joomla
    /// </remarks>
    public class KerkytheaExporter : Exporter
    {
        /// <summary>
        /// Dictionary of registered materials.
        /// </summary>
        public Dictionary<Material, XmlDocument> RegisteredMaterials = new Dictionary<Material, XmlDocument>();

        /// <summary>
        /// The names.
        /// </summary>
        private readonly HashSet<string> names = new HashSet<string>();

        /// <summary>
        /// Texture bitmaps are reused. This dictionary contains a map from brush to filename
        /// </summary>
        private readonly Dictionary<Brush, string> textureFiles = new Dictionary<Brush, string>();

        /// <summary>
        /// The writer.
        /// </summary>
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KerkytheaExporter"/> class.
        /// </summary>
        /// <param name="outputFileName">
        /// Name of the output file.
        /// </param>
        public KerkytheaExporter(string outputFileName)
        {
            this.Name = "My Scene";
            this.BackgroundColor = Colors.Black;
            this.ReflectionColor = Colors.Gray;
            this.Reflections = true;
            this.Shadows = true;
            this.SoftShadows = true;
            this.LightMultiplier = 3.0;
            this.Threads = 2;

            this.ShadowColor = Color.FromArgb(255, 100, 100, 100);
            this.RenderSetting = RenderSettings.RayTracer;
            this.Aperture = "Pinhole";
            this.FocusDistance = 1.0;
            this.LensSamples = 3;

            this.Width = 500;
            this.Height = 500;

            this.TexturePath = Path.GetDirectoryName(outputFileName);
            this.TextureWidth = 1024;
            this.TextureHeight = 1024;

            var settings = new XmlWriterSettings { Indent = true, };

            this.writer = XmlWriter.Create(outputFileName, settings);
        }

        /// <summary>
        /// Render settings.
        /// </summary>
        public enum RenderSettings
        {
            /// <summary>
            /// Use RayTracer.
            /// </summary>
            RayTracer,

            /// <summary>
            /// Use PhotonMap.
            /// </summary>
            PhotonMap,

            /// <summary>
            /// Use MetropolisLightTransport.
            /// </summary>
            MetropolisLightTransport
        }

        /// <summary>
        /// Gets or sets the aperture.
        /// </summary>
        /// <value>The aperture.</value>
        public string Aperture { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the length of the focal.
        /// </summary>
        /// <value>The length of the focal.</value>
        public double FocalLength { get; set; }

        /// <summary>
        /// Gets or sets the focus distance.
        /// </summary>
        /// <value>The focus distance.</value>
        public double FocusDistance { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the lens samples.
        /// </summary>
        /// <value>The lens samples.</value>
        public int LensSamples { get; set; }

        /// <summary>
        /// Gets or sets the light multiplier.
        /// </summary>
        /// <value>The light multiplier.</value>
        public double LightMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color of the reflection.
        /// </summary>
        /// <value>The color of the reflection.</value>
        public Color ReflectionColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref = "KerkytheaExporter" /> is reflections.
        /// </summary>
        /// <value><c>true</c> if reflections; otherwise, <c>false</c>.</value>
        public bool Reflections { get; set; }

        /// <summary>
        /// Gets or sets the render setting.
        /// </summary>
        /// <value>The render setting.</value>
        public RenderSettings RenderSetting { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>The color of the shadow.</value>
        public Color ShadowColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref = "KerkytheaExporter" /> is shadows.
        /// </summary>
        /// <value><c>true</c> if shadows; otherwise, <c>false</c>.</value>
        public bool Shadows { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [soft shadows].
        /// </summary>
        /// <value><c>true</c> if [soft shadows]; otherwise, <c>false</c>.</value>
        public bool SoftShadows { get; set; }

        /// <summary>
        /// Gets or sets the height of the texture.
        /// </summary>
        /// <value>The height of the texture.</value>
        public int TextureHeight { get; set; }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath { get; set; }

        /// <summary>
        /// Gets or sets the width of the texture.
        /// </summary>
        /// <value>The width of the texture.</value>
        public int TextureWidth { get; set; }

        /// <summary>
        /// Gets or sets the threads.
        /// </summary>
        /// <value>The threads.</value>
        public int Threads { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public override void Close()
        {
            // end of scene description.
            this.writer.WriteFullEndElement();

            // it is necessary to describe the primary/active modules as there might exist more than one!
            this.WriteParameter("Mip Mapping", true);
            this.WriteParameter("./Interfaces/Active", "Null Interface");
            this.WriteParameter("./Modellers/Active", "XML Modeller");
            this.WriteParameter("./Image Handlers/Active", "Free Image Support");

            this.WriteParameter("./Ray Tracers/Active", "Threaded Ray Tracer");
            this.WriteParameter("./Irradiance Estimators/Active", "Null Irradiance Estimator");
            this.WriteParameter("./Direct Light Estimators/Active", "Refraction Enhanced");
            this.WriteParameter("./Environments/Active", "Octree Environment");
            this.WriteParameter("./Filters/Active", "Simple Tone Mapping");
            this.WriteParameter("./Scenes/Active", this.Name);
            this.WriteParameter("./Libraries/Active", "Material Librarian");

            // end of root element
            this.writer.WriteFullEndElement();

            this.writer.WriteEndDocument();
            this.writer.Close();
            base.Close();
        }

        /// <summary>
        /// Exports the mesh.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        public void ExportMesh(MeshGeometry3D m)
        {
            this.WriteStartObject("Triangular Mesh", "Triangular Mesh", string.Empty, "Surface");

            this.writer.WriteStartElement("Parameter");
            {
                this.writer.WriteAttributeString("Name", "Vertex List");
                this.writer.WriteAttributeString("Type", "Point3D List");
                this.writer.WriteAttributeString("Value", m.Positions.Count.ToString());
                foreach (var p in m.Positions)
                {
                    this.writer.WriteStartElement("P");
                    this.writer.WriteAttributeString("xyz", ToKerkytheaString(p));
                    this.writer.WriteEndElement();
                }
            }

            this.writer.WriteFullEndElement();

            int triangles = m.TriangleIndices.Count / 3;

            // NORMALS
            // todo: write normal list per vertex instead of per triangle index
            if (m.Normals != null && m.Normals.Count > 0)
            {
                this.writer.WriteStartElement("Parameter");
                {
                    this.writer.WriteAttributeString("Name", "Normal List");
                    this.writer.WriteAttributeString("Type", "Point3D List");
                    this.writer.WriteAttributeString("Value", m.TriangleIndices.Count.ToString());
                    foreach (int index in m.TriangleIndices)
                    {
                        if (index >= m.Normals.Count)
                        {
                            continue;
                        }

                        var n = m.Normals[index];
                        this.writer.WriteStartElement("P");
                        this.writer.WriteAttributeString("xyz", ToKerkytheaString(n));
                        this.writer.WriteEndElement();
                    }
                }

                this.writer.WriteFullEndElement();
            }

            // TRIANGLE INDICES
            this.writer.WriteStartElement("Parameter");
            {
                this.writer.WriteAttributeString("Name", "Index List");
                this.writer.WriteAttributeString("Type", "Triangle Index List");
                this.writer.WriteAttributeString("Value", triangles.ToString());
                for (int a = 0; a < triangles; a++)
                {
                    int i = m.TriangleIndices[a * 3];
                    int j = m.TriangleIndices[a * 3 + 1];
                    int k = m.TriangleIndices[a * 3 + 2];
                    this.writer.WriteStartElement("F");
                    this.writer.WriteAttributeString("ijk", string.Format("{0} {1} {2}", i, j, k));
                    this.writer.WriteEndElement();
                }
            }

            this.writer.WriteFullEndElement();

            this.WriteParameter("Smooth", true);
            this.WriteParameter("AA Tolerance", 15.0);

            this.WriteEndObject();
        }

        /// <summary>
        /// Registers the material.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public void RegisterMaterial(Material m, string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);
            this.RegisteredMaterials.Add(m, doc);
        }

        /// <summary>
        /// Writes the metropolis light transport.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void WriteMetropolisLightTransport(string name)
        {
            this.WriteStartObject("./Ray Tracers/" + name, "Metropolis Light Transport", name, "Ray Tracer");
            this.WriteParameter("Max Ray Tracing Depth", 100);
            this.WriteParameter("Max Iterations", 10000);
            this.WriteParameter("Linear Lightflow", true);
            this.WriteParameter("Seed Paths", 50000);
            this.WriteParameter("Large Step Probability", 0.2);
            this.WriteParameter("Max Mutation Distance", 0.02);
            this.WriteParameter("Live Probability", 0.7);
            this.WriteParameter("Max Consecutive Rejections", 200);
            this.WriteParameter("Bidirectional", true);
            this.WriteParameter("Super Sampling", "3x3");
            this.WriteParameter("Image Filename", "temp.jpg");
            this.WriteParameter("Random Seed", "Automatic");
            this.WriteEndObject();
        }

        /// <summary>
        /// Writes the standard ray tracer.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void WriteStandardRayTracer(string name)
        {
            this.WriteStartObject("./Ray Tracers/" + name, "Standard Ray Tracer", name, "Ray Tracer");
            this.WriteParameter("Rasterization", "Auto");

            // WriteParameter("Antialiasing", "Extra Pass 3x3");
            this.WriteParameter("Antialiasing", "Production AA");
            this.WriteParameter("Antialiasing Filter", "Mitchell-Netravali 0.5 0.8");
            this.WriteParameter("Antialiasing Threshold", 0.3);
            this.WriteParameter("Texture Filtering", true);
            this.WriteParameter("Ambient Lighting", true);
            this.WriteParameter("Direct Lighting", true);
            this.WriteParameter("Sky Lighting", true);
            this.WriteParameter("Brightness Threshold", 0.002);
            this.WriteParameter("Max Ray Tracing Depth", 5);
            this.WriteParameter("Max Scatter Bounces", 5);
            this.WriteParameter("Max Dirac Bounces", 5);
            this.WriteParameter("Irradiance Precomputation", 4);
            this.WriteParameter("Irradiance Scale", Colors.White);
            this.WriteParameter("Linear Lightflow", true);
            this.WriteParameter("Max Iterations", 5);
            this.WriteParameter("Super Sampling", "None");
            this.WriteParameter("Image Filename", "temp.jpg");
            this.WriteParameter("./Sampling Criteria/Diffuse Samples", 1024);
            this.WriteParameter("./Sampling Criteria/Specular Samples", 32);
            this.WriteParameter("./Sampling Criteria/Dispersion Samples", true);
            this.WriteParameter("./Sampling Criteria/Trace Diffusers", false);
            this.WriteParameter("./Sampling Criteria/Trace Translucencies", false);
            this.WriteParameter("./Sampling Criteria/Trace Fuzzy Reflections", true);
            this.WriteParameter("./Sampling Criteria/Trace Fuzzy Refractions", true);
            this.WriteParameter("./Sampling Criteria/Trace Reflections", true);
            this.WriteParameter("./Sampling Criteria/Trace Refractions", true);
            this.WriteParameter("./Sampling Criteria/Random Generator", "Pure");
            this.WriteEndObject();
        }

        /// <summary>
        /// Writes the threaded raytracer.
        /// </summary>
        /// <param name="threads">
        /// The threads.
        /// </param>
        public void WriteThreadedRaytracer(int threads)
        {
            this.WriteStartObject(
                "./Ray Tracers/Threaded Ray Tracer", "Threaded Ray Tracer", "Threaded Ray Tracer", "Ray Tracer");
            for (int i = 0; i < threads; i++)
            {
                this.WriteParameter("Thread #" + i, "#" + i);
            }

            this.WriteParameter("Network Mode", "None");
            this.WriteParameter("Listening Port", 6200);
            this.WriteParameter("Host", "127.0.0.1");
            this.WriteEndObject();
        }

        /// <summary>
        /// Writes the transform.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="m">
        /// The m.
        /// </param>
        public void WriteTransform(string name, Matrix3D m)
        {
            string value = string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.######} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######} {9:0.######} {10:0.######} {11:0.######}",
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
                m.OffsetZ);

            this.WriteParameter(name, "Transform", value);
        }

        /// <summary>
        /// Exports the camera.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        protected override void ExportCamera(Camera c)
        {
            var pc = c as PerspectiveCamera;
            if (pc == null)
            {
                throw new InvalidOperationException("Only perspective cameras are supported.");
            }

            const string name = "Camera #1";
            this.WriteStartObject("./Cameras/" + name, "Pinhole Camera", name, "Camera");

            // FOV = 2 arctan (x / (2 f)), x is diagonal, f is focal length
            // f = x / 2 / Tan(FOV/2)
            // http://en.wikipedia.org/wiki/Angle_of_view
            // http://kmp.bdimitrov.de/technology/fov.html

            // PerspectiveCamera.FieldOfView: Horizontal field of view
            // Must multiply by ratio of Viewport Width/Height
            double ratio = this.Width / (double)this.Height;
            const double x = 40;
            double f = 0.5 * ratio * x / Math.Tan(0.5 * pc.FieldOfView / 180.0 * Math.PI);

            WriteParameter("Focal Length (mm)", f);
            WriteParameter("Film Height (mm)", x);
            this.WriteParameter(
                "Resolution", string.Format(CultureInfo.InvariantCulture, "{0}x{1}", this.Width, this.Height));

            var t = CreateTransform(pc.Position, pc.LookDirection, pc.UpDirection);
            this.WriteTransform("Frame", t);

            this.WriteParameter("Focus Distance", this.FocusDistance);
            this.WriteParameter("f-number", this.Aperture);
            this.WriteParameter("Lens Samples", this.LensSamples);
            this.WriteParameter("Blades", 6);
            this.WriteParameter("Diaphragm", "Circular");
            this.WriteParameter("Projection", "Planar");

            this.WriteEndObject();
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        protected override void ExportHeader()
        {
            this.writer.WriteStartDocument();

            this.writer.WriteStartElement("Root");
            this.writer.WriteAttributeString("Label", "Default Kernel");
            this.writer.WriteAttributeString("Name", string.Empty);
            this.writer.WriteAttributeString("Type", "Kernel");

            this.WriteStartObject("./Modellers/XML Modeller", "XML Modeller", "XML Modeller", "Modeller");
            this.WriteEndObject();

            this.WriteStartObject(
                "./Image Handlers/Free Image Support", "Free Image Support", "Free Image Support", "Image Handler");
            this.WriteParameter("Tone Mapping", "External");
            this.WriteParameter("Jpeg Quality", "Higher");
            this.WriteEndObject();

            this.WriteStartObject(
                "./Direct Light Estimators/Refraction Enhanced",
                "Refraction Enhanced",
                "Refraction Enhanced",
                "Direct Light Estimator");
            this.WriteParameter("Enabled", "Boolean", "1");
            this.WriteParameter("PseudoCaustics", "Boolean", "0");
            this.WriteParameter("PseudoTranslucencies", "Boolean", "0");
            this.WriteParameter("Area Light Evaluation", "Boolean", "1");
            this.WriteParameter("Optimized Area Lights", "Boolean", "1");
            this.WriteParameter("Accurate Soft Shadows", "Boolean", "0");
            this.WriteParameter("Antialiasing", "String", "High");
            this.WriteParameter("./Evaluation/Diffuse", "Boolean", "1");
            this.WriteParameter("./Evaluation/Specular", "Boolean", "1");
            this.WriteParameter("./Evaluation/Translucent", "Boolean", "1");
            this.WriteParameter("./Evaluation/Transmitted", "Boolean", "1");
            this.WriteEndObject();

            // add ray tracer module.
            for (int i = 0; i < this.Threads; i++)
            {
                this.WriteStandardRayTracer("#" + i);
            }

            this.WriteThreadedRaytracer(this.Threads);

            // add spatial subdivision module.
            this.WriteStartObject(
                "./Environments/Octree Environment", "Octree Environment", "Octree Environment", "Environment");
            this.WriteParameter("Max Objects per Cell", 20);
            this.WriteParameter("Instancing Switch", 1000000);
            this.WriteParameter("Caching Switch", 6000000);
            this.WriteEndObject();

            // add basic post filtering / tone mapping.
            this.WriteStartObject("./Filters/Simple Tone Mapping", "Simple Tone Mapping", string.Empty, "Filter");
            this.WriteParameter("Enabled", true);
            this.WriteParameter("Method", "Simple");
            this.WriteParameter("Exposure", 1.0);
            this.WriteParameter("Gamma", 1.0);
            this.WriteParameter("Dark Multiplier", 1.0);
            this.WriteParameter("Bright Multiplier", 1.0);
            this.WriteParameter("Reverse Correction", true);
            this.WriteParameter("Reverse Gamma", 2.2);
            this.WriteEndObject();

            // start of scene description.
            this.WriteStartObject("./Scenes/" + this.Name, "Default Scene", this.Name, "Scene");
        }

        /// <summary>
        /// Exports the light.
        /// </summary>
        /// <param name="l">
        /// The l.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        protected override void ExportLight(Light l, Transform3D t)
        {
            if (l is AmbientLight)
            {
                return;
            }

            string name = this.GetUniqueName(l, l.GetType().Name);

            var d = l as DirectionalLight;
            var s = l as SpotLight;
            var p = l as PointLight;

            this.WriteStartObject("./Lights/" + name, "Default Light", name, "Light");
            {
                string stype = "Projector Light";
                if (s != null)
                {
                    stype = "Spot Light";
                }

                if (p != null)
                {
                    stype = "Omni Light";
                }

                this.WriteStartObject(stype, stype, string.Empty, "Emittance");

                // emitter Radiance
                this.WriteStartObject("./Radiance/Constant Texture", "Constant Texture", string.Empty, "Texture");
                var c = Colors.White;
                WriteParameter("Color", c);
                this.WriteEndObject();

                // var v = new Vector3D(l.Color.R, l.Color.G, l.Color.B);
                // double lum = v.Length;
                this.WriteParameter("Attenuation", "None");

                // SpotLight (Spot Light)
                if (s != null)
                {
                    // todo : export the specular parameters
                    // s.ConstantAttenuation
                    // s.LinearAttenuation
                    // s.QuadraticAttenuation
                    WriteParameter("Fall Off", s.OuterConeAngle);
                    WriteParameter("Hot Spot", s.InnerConeAngle);
                }

                // DirectionalLight (Projector Light)
                if (d != null)
                {
                    this.WriteParameter("Width", 2.0);
                    this.WriteParameter("Height", 2.0);
                }

                // PointLight (Omni light)
                if (p != null)
                {
                    // todo: export pointlight parameters
                    // p.ConstantAttenuation
                    // p.LinearAttenuation
                    // p.QuadraticAttenuation
                    // p.Range // distance beyond which the light has no effect
                }

                this.WriteParameter("Focal Length", 1.0);

                this.WriteEndObject(); // stype

                this.WriteParameter("Enabled", true);
                this.WriteParameter("Shadow", this.Shadows);
                this.WriteParameter("Soft Shadow", this.SoftShadows);

                this.WriteParameter("Negative Light", false);
                this.WriteParameter("Global Photons", true);
                this.WriteParameter("Caustic Photons", true);
                this.WriteParameter("Multiplier", this.LightMultiplier);

                Matrix3D transform;
                var upVector = new Vector3D(0, 0, 1);
                if (s != null)
                {
                    transform = CreateTransform(s.Position, s.Direction, upVector);
                    this.WriteTransform("Frame", transform);
                }

                if (d != null)
                {
                    var origin = new Point3D(-1000 * d.Direction.X, -1000 * d.Direction.Y, -1000 * d.Direction.Z);
                    transform = CreateTransform(origin, d.Direction, upVector);
                    this.WriteTransform("Frame", transform);
                }

                if (p != null)
                {
                    var direction = new Vector3D(-p.Position.X, -p.Position.Y, -p.Position.Z);
                    transform = CreateTransform(p.Position, direction, upVector);
                    this.WriteTransform("Frame", transform);
                }

                this.WriteParameter("Focus Distance", 4.0);
                this.WriteParameter("Radius", 0.2);
                this.WriteParameter("Shadow Color", this.ShadowColor);
            }

            this.WriteEndObject();
        }

        /// <summary>
        /// Exports the model.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        protected override void ExportModel(GeometryModel3D g, Transform3D transform)
        {
            var mesh = g.Geometry as MeshGeometry3D;
            if (mesh == null)
            {
                return;
            }

            string name = this.GetUniqueName(g, g.GetType().Name);
            this.WriteStartObject("./Models/" + name, "Default Model", name, "Model");

            this.ExportMesh(mesh);

            if (g.Material != null)
            {
                this.ExportMaterial(g.Material);
            }

            var tg = new Transform3DGroup();
            tg.Children.Add(g.Transform);
            tg.Children.Add(transform);

            if (mesh.TextureCoordinates != null)
            {
                this.ExportMapChannel(mesh);
            }

            this.WriteTransform("Frame", tg.Value);

            this.WriteParameter("Enabled", true);
            this.WriteParameter("Visible", true);
            this.WriteParameter("Shadow Caster", true);
            this.WriteParameter("Shadow Receiver", true);
            this.WriteParameter("Caustics Transmitter", true);
            this.WriteParameter("Caustics Receiver", true);
            this.WriteParameter("Exit Blocker", false);

            this.WriteEndObject();
        }

        // Viewport3D
        // ModelVisual3D : Visual3D
        // GeometryModel3D
        // DirectionalLight
        // AmbientLight
        // PointLight
        // SpotLight
        // Model3DGroup
        // Model3DCollection
        // GeometryModel3D
        // Model3DGroup
        // ModelUIElement3D : UIElement3D : Visual3D

        /// <summary>
        /// Exports the viewport.
        /// </summary>
        /// <param name="v">
        /// The v.
        /// </param>
        protected override void ExportViewport(Viewport3D v)
        {
            var ambient = Visual3DHelper.Find<AmbientLight>(v);

            // default global settings
            this.WriteStartObject("Default Global Settings", "Default Global Settings", string.Empty, "Global Settings");
            if (ambient != null)
            {
                WriteParameter("Ambient Light", ambient.Color);
            }

            this.WriteParameter("Background Color", this.BackgroundColor);
            this.WriteParameter("Compute Volume Transfer", false);
            this.WriteParameter("Transfer Recursion Depth", 1);
            this.WriteParameter("Background Type", "Sky Color");
            this.WriteParameter("Sky Intensity", 1.0);
            this.WriteParameter("Sky Frame", "Transform", "1 0 0 0 0 1 0 0 0 0 1 0 ");
            this.WriteParameter("Sun Direction", "0 0 1");
            this.WriteParameter("Sky Turbidity", 2.0);
            this.WriteParameter("Sky Luminance Gamma", 1.2);
            this.WriteParameter("Sky Chromaticity Gamma", 1.8);
            this.WriteParameter("Linear Lightflow", true);
            this.WriteParameter("Index of Refraction", 1.0);
            this.WriteParameter("Scatter Density", 0.1);
            this.WriteParameter("./Location/Latitude", 0.0);
            this.WriteParameter("./Location/Longitude", 0.0);
            this.WriteParameter("./Location/Timezone", 0);
            this.WriteParameter("./Location/Date", "0/0/2007");
            this.WriteParameter("./Location/Time", "12:0:0");
            this.WriteParameter("./Background Image/Filename", "[No Bitmap]");
            this.WriteParameter("./Background Image/Projection", "UV");
            this.WriteParameter("./Background Image/Offset X", 0.0);
            this.WriteParameter("./Background Image/Offset Y", 0.0);
            this.WriteParameter("./Background Image/Scale X", 1.0);
            this.WriteParameter("./Background Image/Scale Y", 1.0);
            this.WriteParameter("./Background Image/Rotation", 0.0);
            this.WriteParameter("./Background Image/Smooth", true);
            this.WriteParameter("./Background Image/Inverted", false);
            this.WriteParameter("./Background Image/Alpha Channel", false);
            this.WriteEndObject();

            // Visual3DHelper.Traverse<Light>(v.Children, ExportLight);
            // Visual3DHelper.Traverse<GeometryModel3D>(v.Children, ExportGeometryModel3D);
        }

        /// <summary>
        /// Writes the end object.
        /// </summary>
        protected void WriteEndObject()
        {
            this.writer.WriteFullEndElement();
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        protected void WriteObject(string identifier, string label, string name, string type)
        {
            this.WriteStartObject(identifier, label, name, type);
            this.WriteEndObject();
        }

        /// <summary>
        /// Writes the start object.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        protected void WriteStartObject(string identifier, string label, string name, string type)
        {
            this.writer.WriteStartElement("Object");
            this.writer.WriteAttributeString("Identifier", identifier);
            this.writer.WriteAttributeString("Label", label);
            this.writer.WriteAttributeString("Name", name);
            this.writer.WriteAttributeString("Type", type);
        }

        /// <summary>
        /// Create transform from the original coordinate system to the system defined by translation origin
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="direction">
        /// The direction.
        /// </param>
        /// <param name="up">
        /// The up.
        /// </param>
        private static Matrix3D CreateTransform(Point3D origin, Vector3D direction, Vector3D up)
        {
            var z = direction;
            var x = Vector3D.CrossProduct(direction, up);
            var y = up;

            x.Normalize();
            y.Normalize();
            z.Normalize();

            var m = new Matrix3D(x.X, y.X, z.X, 0, x.Y, y.Y, z.Y, 0, x.Z, y.Z, z.Z, 0, origin.X, origin.Y, origin.Z, 1);

            return m;
        }

        /// <summary>
        /// The not na n.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The not na n.
        /// </returns>
        private static double NotNaN(double value, double defaultValue)
        {
            if (double.IsNaN(value))
            {
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// The to kerkythea string.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The to kerkythea string.
        /// </returns>
        private static string ToKerkytheaString(Point p)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", p.X, p.Y);
        }

        /// <summary>
        /// Converts a point to a kerkythea string.
        /// </summary>
        /// <param name="point">
        /// The vector.
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        private static string ToKerkytheaString(Point3D point)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.######} {1:0.######} {2:0.######}",
                NotNaN(point.X, 1),
                NotNaN(point.Y, 0),
                NotNaN(point.Z, 0));
        }

        /// <summary>
        /// Converts a vector to a kerkythea string.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        private static string ToKerkytheaString(Vector3D vector)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.######} {1:0.######} {2:0.######}",
                NotNaN(vector.X, 1),
                NotNaN(vector.Y, 0),
                NotNaN(vector.Z, 0));
        }

        /// <summary>
        /// The to kerkythea string.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The to kerkythea string.
        /// </returns>
        private static string ToKerkytheaString(Color c)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.######} {1:0.######} {2:0.######}",
                c.R / 255.0,
                c.G / 255.0,
                c.B / 255.0);
        }

        /// <summary>
        /// The export map channel.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        private void ExportMapChannel(MeshGeometry3D m)
        {
            this.writer.WriteStartElement("Parameter");
            {
                this.writer.WriteAttributeString("Name", "Map Channel");
                this.writer.WriteAttributeString("Type", "Point2D List");
                int n = m.TriangleIndices.Count;
                this.writer.WriteAttributeString("Value", n.ToString());
                foreach (int index in m.TriangleIndices)
                {
                    if (index >= m.TextureCoordinates.Count)
                    {
                        continue;
                    }

                    var uv = m.TextureCoordinates[index];
                    this.writer.WriteStartElement("P");
                    this.writer.WriteAttributeString("xy", ToKerkytheaString(uv));
                    this.writer.WriteEndElement();
                }
            }

            this.writer.WriteFullEndElement();
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="material">
        /// The material.
        /// </param>
        /// <param name="weights">
        /// The weights.
        /// </param>
        private void ExportMaterial(string name, Material material, IList<double> weights)
        {
            var g = material as MaterialGroup;
            if (g != null)
            {
                foreach (var m in g.Children)
                {
                    this.ExportMaterial(name, m, weights);
                }
            }

            var d = material as DiffuseMaterial;
            if (d != null)
            {
                string texture = null;
                Color? color = null;
                double alpha = 1.0;
                if (d.Brush is SolidColorBrush)
                {
                    color = this.GetSolidColor(d.Brush, d.Color);
                    alpha = color.Value.A / 255.0;
                }
                else
                {
                    texture = this.GetTexture(d.Brush, name);
                }

                if (alpha > 0)
                {
                    this.WriteWhittedMaterial(string.Format("#{0}", weights.Count), texture, color, null, null);
                    weights.Add(alpha);
                }

                // The refractive part
                if (alpha < 1)
                {
                    this.WriteWhittedMaterial(string.Format("#{0}", weights.Count), null, null, null, Colors.White);
                    weights.Add(1 - alpha);
                }
            }

            var s = material as SpecularMaterial;
            if (s != null)
            {
                var color = this.GetSolidColor(s.Brush, s.Color);

                // color = Color.FromArgb((byte)(color.A * factor), (byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
                this.WriteWhittedMaterial(
                    string.Format("#{0}", weights.Count), null, null, color, null, s.SpecularPower * 0.5);
                double weight = color.A / 255.0;
                weight *= 0.01;
                weights.Add(weight);
            }

            var e = material as EmissiveMaterial;
            if (e != null)
            {
                Trace.WriteLine("KerkytheaExporter: Emissive materials are not yet supported.");

                // Color color = GetSolidColor(e.Brush, d.Color);
                // WriteWhittedMaterial(string.Format("#{0}", weights.Count + 1), color, null, null);
                // WriteStartObject("./Translucent/Constant Texture", "Constant Texture", "", "Texture");
                // WriteParameter("Color", e.Color);
                // WriteEndObject();
            }
        }

        /// <summary>
        /// The export material.
        /// </summary>
        /// <param name="material">
        /// The material.
        /// </param>
        private void ExportMaterial(Material material)
        {
            // If the material is registered, simply output the xml
            if (this.RegisteredMaterials.ContainsKey(material))
            {
                var doc = this.RegisteredMaterials[material];
                if (doc != null && doc.DocumentElement != null)
                {
                    foreach (XmlNode e in doc.DocumentElement.ChildNodes)
                    {
                        e.WriteTo(this.writer);
                    }
                }

                return;
            }

            string name = this.GetUniqueName(material, "Material");
            this.WriteStartObject(name, "Layered Material", name, "Material");

            var weights = new List<double>();

            this.ExportMaterial(name, material, weights);

            // if (Reflections)
            // {
            // WriteConstantTexture("Reflection", ReflectionColor);
            // }
            for (int i = 0; i < weights.Count; i++)
            {
                this.WriteWeight("Weight #" + i, weights[i]);
            }

            /*
             switch (MaterialType)
             {
                 case MaterialTypes.Ashikhmin:
                     WriteParameter("Rotation", 0.0);
                     WriteParameter("Attenuation", "Schlick");
                     WriteParameter("Index of Refraction", 1.0);
                     WriteParameter("N-K File", "");
                     break;
                 case MaterialTypes.Diffusive: // Whitted material
                     WriteParameter("Shininess", 60.0);
                     WriteParameter("Transmitted Shininess", 128.0);
                     WriteParameter("Index of Refraction", 1.0);
                     WriteParameter("Specular Sampling", true);
                     WriteParameter("Transmitted Sampling", false);
                     WriteParameter("Specular Attenuation", "Cosine");
                     WriteParameter("Transmitted Attenuation", "Cosine");
                     break;
             }
             */
            this.WriteEndObject();
        }

        /// <summary>
        /// The get solid color.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="fallbackColor">
        /// The fallback color.
        /// </param>
        /// <returns>
        /// </returns>
        private Color GetSolidColor(Brush brush, Color fallbackColor)
        {
            var scb = brush as SolidColorBrush;
            if (scb != null)
            {
                return scb.Color;
            }

            return fallbackColor;
        }

        /// <summary>
        /// The get texture.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The get texture.
        /// </returns>
        private string GetTexture(Brush brush, string name)
        {
            // reuse textures
            if (this.textureFiles.ContainsKey(brush))
            {
                return this.textureFiles[brush];
            }

            string filename = name + ".png";
            string path = Path.Combine(this.TexturePath, filename);
            RenderBrush(path, brush, this.TextureWidth, this.TextureHeight);

            this.textureFiles.Add(brush, filename);
            return filename;
        }

        /// <summary>
        /// The get unique name.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="defaultName">
        /// The default name.
        /// </param>
        /// <returns>
        /// The get unique name.
        /// </returns>
        private string GetUniqueName(DependencyObject o, string defaultName)
        {
            var name = o.GetValue(FrameworkElement.NameProperty) as string;
            if (string.IsNullOrEmpty(name))
            {
                int n = 1;
                while (true)
                {
                    // name = defaultName + " #" + n;
                    name = defaultName + n;
                    if (!this.names.Contains(name))
                    {
                        break;
                    }

                    n++;
                }
            }

            this.names.Add(name);
            return name;
        }

        /// <summary>
        /// The write ashikhmin material.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="diffuse">
        /// The diffuse.
        /// </param>
        /// <param name="specular">
        /// The specular.
        /// </param>
        /// <param name="shininessXMap">
        /// The shininess x map.
        /// </param>
        /// <param name="shininessYMap">
        /// The shininess y map.
        /// </param>
        /// <param name="rotationMap">
        /// The rotation map.
        /// </param>
        /// <param name="shininessX">
        /// The shininess x.
        /// </param>
        /// <param name="shininessY">
        /// The shininess y.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="indexOfRefraction">
        /// The index of refraction.
        /// </param>
        /// <param name="nkfile">
        /// The nkfile.
        /// </param>
        private void WriteAshikhminMaterial(
            string identifier,
            Color? diffuse,
            Color? specular,
            Color? shininessXMap,
            Color? shininessYMap,
            Color? rotationMap,
            double shininessX = 100,
            double shininessY = 100,
            double rotation = 0,
            double indexOfRefraction = 1.0,
            string nkfile = null)
        {
            this.WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

            if (diffuse.HasValue)
            {
                this.WriteConstantTexture("Diffuse", diffuse.Value);
            }

            if (specular.HasValue)
            {
                this.WriteConstantTexture("Specular", specular.Value);
            }

            if (shininessXMap.HasValue)
            {
                this.WriteConstantTexture("Shininess X Map", shininessXMap.Value);
            }

            if (shininessYMap.HasValue)
            {
                this.WriteConstantTexture("Shininess Y Map", shininessYMap.Value);
            }

            if (rotationMap.HasValue)
            {
                this.WriteConstantTexture("RotationMap", rotationMap.Value);
            }

            WriteParameter("Shininess X", shininessX);
            WriteParameter("Shininess Y", shininessY);
            WriteParameter("Rotation", rotation);
            this.WriteParameter("Attenuation", "Schlick");
            WriteParameter("Index of Refraction", indexOfRefraction);
            WriteParameter("N-K File", nkfile);
            this.WriteEndObject();
        }

        /// <summary>
        /// The write bitmap texture.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private void WriteBitmapTexture(string name, string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                this.WriteStartObject("./" + name + "/Bitmap Texture", "Bitmap Texture", string.Empty, "Texture");
                WriteParameter("Filename", filename);
                this.WriteParameter("Projection", "UV");
                this.WriteParameter("Offset X", 0.0);
                this.WriteParameter("Offset Y", 0.0);
                this.WriteParameter("Scale X", 1.0);
                this.WriteParameter("Scale Y", 1.0);
                this.WriteParameter("Rotation", 0.0);
                this.WriteParameter("Smooth", true);
                this.WriteParameter("Inverted", false);
                this.WriteParameter("Alpha Channel", false);
                this.WriteEndObject();
            }
        }

        /// <summary>
        /// The write constant texture.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void WriteConstantTexture(string name, Color color)
        {
            this.WriteStartObject("./" + name + "/Constant Texture", "Constant Texture", string.Empty, "Texture");
            WriteParameter("Color", color);
            this.WriteEndObject();
        }

        /// <summary>
        /// The write dielectric material.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="reflection">
        /// The reflection.
        /// </param>
        /// <param name="refraction">
        /// The refraction.
        /// </param>
        /// <param name="indexOfRefraction">
        /// The index of refraction.
        /// </param>
        /// <param name="dispersion">
        /// The dispersion.
        /// </param>
        /// <param name="nkfile">
        /// The nkfile.
        /// </param>
        private void WriteDielectricMaterial(
            string identifier,
            Color? reflection,
            Color? refraction,
            double indexOfRefraction = 1.0,
            double dispersion = 0.0,
            string nkfile = null)
        {
            this.WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

            if (reflection.HasValue)
            {
                this.WriteConstantTexture("Reflection", reflection.Value);
            }

            if (refraction.HasValue)
            {
                this.WriteConstantTexture("Refraction", refraction.Value);
            }

            WriteParameter("Index of Refraction", indexOfRefraction);
            WriteParameter("Dispersion", dispersion);
            this.WriteParameter("N-K File", string.Empty);
            this.WriteEndObject();
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteParameter(string name, string type, string value)
        {
            this.writer.WriteStartElement("Parameter");
            this.writer.WriteAttributeString("Name", name);
            this.writer.WriteAttributeString("Type", type);
            this.writer.WriteAttributeString("Value", value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteParameter(string p, string value)
        {
            this.WriteParameter(p, "String", value);
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void WriteParameter(string p, Color color)
        {
            this.WriteParameter(p, "RGB", ToKerkytheaString(color));
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        private void WriteParameter(string p, bool flag)
        {
            this.WriteParameter(p, "Boolean", flag ? "1" : "0");
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteParameter(string p, double value)
        {
            this.WriteParameter(p, "Real", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The write parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void WriteParameter(string p, int value)
        {
            this.WriteParameter(p, "Integer", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The write weight.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="weight">
        /// The weight.
        /// </param>
        private void WriteWeight(string identifier, double weight)
        {
            this.WriteStartObject(identifier, "Weighted Texture", identifier, "Texture");
            this.WriteStartObject("Constant Texture", "Constant Texture", string.Empty, "Texture");
            this.WriteParameter("Color", Colors.White);
            this.WriteEndObject();
            WriteParameter("Weight #0", weight);
            this.WriteEndObject();
        }

        /// <summary>
        /// The write whitted material.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="diffuse">
        /// The diffuse.
        /// </param>
        /// <param name="specular">
        /// The specular.
        /// </param>
        /// <param name="refraction">
        /// The refraction.
        /// </param>
        /// <param name="shininess">
        /// The shininess.
        /// </param>
        /// <param name="indexOfRefraction">
        /// The index of refraction.
        /// </param>
        private void WriteWhittedMaterial(
            string identifier,
            string texture,
            Color? diffuse,
            Color? specular,
            Color? refraction,
            double shininess = 128.0,
            double indexOfRefraction = 1.0)
        {
            this.WriteStartObject(identifier, "Whitted Material", identifier, "Material");

            if (texture != null)
            {
                this.WriteBitmapTexture("Diffuse", texture);
            }

            if (diffuse.HasValue)
            {
                this.WriteConstantTexture("Diffuse", diffuse.Value);
            }

            if (specular.HasValue)
            {
                this.WriteConstantTexture("Specular", specular.Value);
            }

            if (refraction.HasValue)
            {
                this.WriteConstantTexture("Refraction", refraction.Value);
            }

            WriteParameter("Shininess", shininess);
            this.WriteParameter("Transmitted Shininess", 128.0);
            WriteParameter("Index of Refraction", indexOfRefraction);
            this.WriteParameter("Specular Sampling", false);
            this.WriteParameter("Transmitted Sampling", false);
            this.WriteParameter("Specular Attenuation", "Cosine");
            this.WriteParameter("Transmitted Attenuation", "Cosine");

            this.WriteEndObject();
        }

    }
}
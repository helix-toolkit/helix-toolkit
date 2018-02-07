// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using static HelixToolkit.Wpf.ViewCubeVisual3D;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// <para>Viewbox replacement for Viewport using swapchain rendering.</para>
    /// <para>To replace box texture (such as text, colors), bind to custom material with different diffuseMap. </para>
    /// <para>Create a image with 1 row and 6 evenly distributed columns. Each column occupies one box face. The face order is Front, Back, Down, Up, Left, Right</para>
    /// </summary>
    public class ViewBoxModel3D : ScreenSpacedElement3D
    {
        public static readonly DependencyProperty ViewBoxTextureProperty = DependencyProperty.Register("ViewBoxTexture", typeof(Stream), typeof(ViewBoxModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                (d as ViewBoxModel3D).UpdateTexture((Stream)e.NewValue);
            }));

        public Stream ViewBoxTexture
        {
            set
            {
                SetValue(ViewBoxTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ViewBoxTextureProperty);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [enable edge click].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable edge click]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableEdgeClick
        {
            get { return (bool)GetValue(EnableEdgeClickProperty); }
            set { SetValue(EnableEdgeClickProperty, value); }
        }

        /// <summary>
        /// The enable edge click property
        /// </summary>
        public static readonly DependencyProperty EnableEdgeClickProperty =
            DependencyProperty.Register("EnableEdgeClick", typeof(bool), typeof(ViewBoxModel3D), new PropertyMetadata(false, (d,e)=> 
            {
                (d as ViewBoxModel3D).CornerModel.IsRendering = (d as ViewBoxModel3D).EdgeModel.IsRendering = (bool)e.NewValue;
            }));

        private const float size = 5;

        private static readonly Vector3[] xAligned = { new Vector3(0, -1, -1), new Vector3(0, 1, -1), new Vector3(0, -1, 1), new Vector3(0, 1, 1) }; //x
        private static readonly Vector3[] yAligned = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };//y
        private static readonly Vector3[] zAligned = { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0) };//z

        private static readonly Vector3[] cornerPoints =   {
                new Vector3(-1,-1,-1 ), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1),
                new Vector3(-1,-1,1 ),new Vector3(1,-1,1 ),new Vector3(1,1,1 ),new Vector3(-1,1,1 )};

        private static readonly Matrix[] cornerInstances;
        private static readonly Matrix[] edgeInstances;
        private static readonly Geometry3D cornerGeometry;
        private static readonly Geometry3D edgeGeometry;

        private readonly MeshGeometryModel3D ViewBoxMeshModel;
        private readonly InstancingMeshGeometryModel3D EdgeModel;
        private readonly InstancingMeshGeometryModel3D CornerModel;
        

        public static readonly RoutedEvent ViewBoxClickedEvent =
            EventManager.RegisterRoutedEvent("ViewBoxClicked", RoutingStrategy.Bubble, typeof(EventHandler<ViewBoxClickedEventArgs>), typeof(ViewBoxModel3D));

        public class ViewBoxClickedEventArgs : RoutedEventArgs
        {
            /// <summary>
            /// Gets or sets the look direction.
            /// </summary>
            /// <value>The look direction.</value>
            public Media3D.Vector3D LookDirection { get; set; }

            /// <summary>
            /// Gets or sets up direction.
            /// </summary>
            /// <value>Up direction.</value>
            public Media3D.Vector3D UpDirection { get; set; }
            public ViewBoxClickedEventArgs(object source, Media3D.Vector3D lookDir, Media3D.Vector3D upDir)
                : base(ViewBoxClickedEvent, source)
            {
                LookDirection = lookDir;
                UpDirection = upDir;
            }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event EventHandler<ViewBoxClickedEventArgs> ViewBoxClicked
        {
            add { AddHandler(ViewBoxClickedEvent, value); }
            remove { RemoveHandler(ViewBoxClickedEvent, value); }
        }

        static ViewBoxModel3D()
        {
            var builder = new MeshBuilder(true, false);
            float cornerSize = size / 5;
            builder.AddBox(Vector3.Zero, cornerSize, cornerSize, cornerSize);
            cornerGeometry = builder.ToMesh();

            builder = new MeshBuilder(true, false);
            float halfSize = size / 2;
            float edgeSize = halfSize * 1.5f;
            builder.AddBox(Vector3.Zero, cornerSize, edgeSize, cornerSize);
            edgeGeometry = builder.ToMesh();

            cornerInstances = new Matrix[cornerPoints.Length];
            for (int i = 0; i < cornerPoints.Length; ++i)
            {
                cornerInstances[i] = Matrix.Translation(cornerPoints[i] * size / 2 * 0.95f);
            }
            int count = xAligned.Length;
            edgeInstances = new Matrix[count * 3];

            for (int i = 0; i < count; ++i)
            {
                edgeInstances[i] = Matrix.RotationZ((float)Math.PI / 2) * Matrix.Translation(xAligned[i] * halfSize * 0.95f);
            }
            for (int i = count; i < count * 2; ++i)
            {
                edgeInstances[i] = Matrix.Translation(yAligned[i % count] * halfSize * 0.95f);
            }
            for (int i = count * 2; i < count * 3; ++i)
            {
                edgeInstances[i] = Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(zAligned[i % count] * halfSize * 0.95f);
            }
        }

        public ViewBoxModel3D()
        {
            RelativeScreenLocationX = 0.8;
            ViewBoxMeshModel = new MeshGeometryModel3D() { EnableViewFrustumCheck = false };
            var sampler = (SamplerStateDescription)PhongMaterial.DiffuseAlphaMapSamplerProperty.DefaultMetadata.DefaultValue;
            sampler.BorderColor = Color.Gray;
            sampler.AddressU = sampler.AddressV = sampler.AddressW = TextureAddressMode.Border;

            ViewBoxMeshModel.CullMode = CullMode.Back;
            ViewBoxMeshModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.ViewCube]; };
            this.Children.Add(ViewBoxMeshModel);
            UpdateModel(UpDirection.ToVector3());
            ViewBoxMeshModel.Material = new PhongMaterial()
            {
                DiffuseColor = Color.White,
                DiffuseMapSampler = sampler
            };

            CornerModel = new InstancingMeshGeometryModel3D() { EnableViewFrustumCheck = false, Material = PhongMaterials.Yellow,
                Geometry = cornerGeometry, Instances = cornerInstances, IsRendering = false };
            CornerModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Diffuse]; };
            Children.Add(CornerModel);

            EdgeModel = new InstancingMeshGeometryModel3D() { EnableViewFrustumCheck = false, Material = PhongMaterials.Silver,
                Geometry = edgeGeometry, Instances = edgeInstances, IsRendering = false };
            EdgeModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Diffuse]; };
            Children.Add(EdgeModel);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                var material = (ViewBoxMeshModel.Material as PhongMaterial);
                if (material.DiffuseMap == null)
                {
                    material.DiffuseMap = ViewBoxTexture == null ? BitmapExtensions.CreateViewBoxTexture(host.EffectsManager,
                        "F", "B", "L", "R", "U", "D", Color.Red, Color.Red, Color.Blue, Color.Blue, Color.Green, Color.Green,
                        Color.White, Color.White, Color.White, Color.White, Color.White, Color.White) : ViewBoxTexture;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateTexture(Stream texture)
        {
            (ViewBoxMeshModel.Material as PhongMaterial).DiffuseMap = texture;
        }

        protected override void UpdateModel(Vector3 up)
        {
            var left = new Vector3(up.Y, up.Z, up.X);
            var front = Vector3.Cross(left, up);
            var builder = new MeshBuilder(true, true, false);
            builder.AddCubeFace(new Vector3(0, 0, 0), front, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -front, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), up, left, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -up, -left, size, size, size);

            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);


            var pts = new List<Vector3>();

            var center = up * -size / 2 * 1.3f;
            for (int i = 0; i < 20; i++)
            {
                double angle = 0 + (360 * i / (20 - 1));
                double angleRad = angle / 180 * Math.PI;
                var dir = (left * (float)Math.Cos(angleRad)) + (front * (float)Math.Sin(angleRad));
                pts.Add(center + (dir * (size - 1.0f)));
                pts.Add(center + (dir * (size + 0.9f)));
            }
            builder = new MeshBuilder(false, false, false);
            builder.AddTriangleStrip(pts);
            var pie = builder.ToMesh();
            int count = pie.Indices.Count;
            for (int i = 0; i < count; i += 3)
            {
                pie.Indices.Add(pie.Indices[i + 2]);
                pie.Indices.Add(pie.Indices[i + 1]);
                pie.Indices.Add(pie.Indices[i]);
            }

            var newMesh = MeshGeometry3D.Merge(new MeshGeometry3D[] { pie, mesh });

            newMesh.TextureCoordinates = new Core.Vector2Collection(Enumerable.Repeat(new Vector2(-1, -1), pie.Positions.Count));
            newMesh.Colors = new Core.Color4Collection(Enumerable.Repeat(new Color4(1f, 1f, 1f, 1f), pie.Positions.Count));
            newMesh.TextureCoordinates.AddRange(mesh.TextureCoordinates);
            newMesh.Colors.AddRange(Enumerable.Repeat(new Color4(1, 1, 1, 1), mesh.Positions.Count));
            newMesh.Normals = newMesh.CalculateNormals();

            ViewBoxMeshModel.Geometry = newMesh;
        }

        private static void CreateTextureCoordinates(MeshGeometry3D mesh)
        {
            int faces = 6;
            int segment = 4;
            float inc = 1f / faces;
            for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
            {
                mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i / segment), mesh.TextureCoordinates[i].Y);
            }
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            var p = Vector4.Transform(new Vector4(ray.Position, 1), context.ScreenViewProjectionMatrix);
            if (Math.Abs(p.W) > 1e-7)
            {
                p /= p.W;
            }
            else
            {
                return false;
            }

            float viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale;
            var px = p.X - (float)(context.ActualWidth / 2 * (1 + screenSpaceCore.RelativeScreenLocationX) - viewportSize / 2);
            var py = p.Y - (float)(context.ActualHeight / 2 * (1 - screenSpaceCore.RelativeScreenLocationY) - viewportSize / 2);
            if (px < 0 || py < 0 || px > viewportSize || py > viewportSize)
            {
                return false;
            }
            var viewMatrix = screenSpaceCore.GlobalTransform.View;
            Vector3 v = new Vector3();

            var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
            var aspectRatio = screenSpaceCore.ScreenRatio;
            var projMatrix = screenSpaceCore.GlobalTransform.Projection;
            Vector3 zn, zf;
            v.X = (2 * px / viewportSize - 1) / projMatrix.M11;
            v.Y = -(2 * py / viewportSize - 1) / projMatrix.M22;
            v.Z = 1 / projMatrix.M33;
            Vector3.TransformCoordinate(ref v, ref matrix, out zf);
            if (screenSpaceCore.IsPerspective)
            {
                zn = screenSpaceCore.GlobalTransform.EyePos;
            }
            else
            {
                v.Z = 0;
                Vector3.TransformCoordinate(ref v, ref matrix, out zn);
            }

            Vector3 r = zf - zn;
            r.Normalize();

            ray = new Ray(zn, r);
            List<HitTestResult> viewBoxHit = new List<HitTestResult>();

            if (base.OnHitTest(context, totalModelMatrix, ref ray, ref viewBoxHit))
            {
                hits?.Clear();
                hits = viewBoxHit;
                Debug.WriteLine("View box hit.");
                var hit = viewBoxHit[0];
                Vector3 normal = Vector3.Zero;
                if(hit.ModelHit == ViewBoxMeshModel)
                {
                    normal = -hit.NormalAtHit;
                }
                else if(hit.Tag is int)
                {
                    int index = (int)hit.Tag;
                    if(hit.ModelHit == EdgeModel && index < edgeInstances.Length)
                    {
                        Matrix transform = edgeInstances[index];
                        normal = -transform.TranslationVector;
                    }
                    else if(hit.ModelHit == CornerModel && index < cornerInstances.Length)
                    {
                        Matrix transform = cornerInstances[index];
                        normal = -transform.TranslationVector;
                    }
                }
                else
                {
                    return false;
                }
                normal.Normalize();
                if (Vector3.Cross(normal, UpDirection.ToVector3()).LengthSquared() < 1e-5)
                {
                    var vecLeft = new Media3D.Vector3D(-normal.Y, -normal.Z, -normal.X);
                    RaiseEvent(new ViewBoxClickedEventArgs(this, normal.ToVector3D(), vecLeft));
                }
                else
                {
                    RaiseEvent(new ViewBoxClickedEventArgs(this, normal.ToVector3D(), UpDirection));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

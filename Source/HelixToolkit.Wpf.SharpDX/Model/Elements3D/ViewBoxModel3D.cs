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

        private MeshGeometryModel3D ViewBoxMeshModel;

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

        public ViewBoxModel3D()
        {
            RelativeScreenLocationX = 0.8;
            ViewBoxMeshModel = new MeshGeometryModel3D();
            var map = new MemoryStream();
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.DefaultViewboxTexture.jpg");
            stream.CopyTo(map);
            stream.Dispose();
            var sampler = (SamplerStateDescription)PhongMaterial.DiffuseAlphaMapSamplerProperty.DefaultMetadata.DefaultValue;
            sampler.BorderColor = Color.Gray;
            sampler.AddressU = sampler.AddressV = sampler.AddressW = TextureAddressMode.Border;
            ViewBoxMeshModel.Material = new PhongMaterial()
            {
                DiffuseColor = Color.White,
                DiffuseMap = BitmapExtension.CreateViewBoxBitmapSource("F", "B", "L", "R", "U", "D", Media.Colors.Red, Media.Colors.Red,
                    Media.Colors.Blue, Media.Colors.Blue, Media.Colors.Green, Media.Colors.Green).ToMemoryStream(),
                DiffuseMapSampler = sampler
            };
            ViewBoxMeshModel.CullMode = CullMode.Back;
            ViewBoxMeshModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.ViewCube]; };
            this.Children.Add(ViewBoxMeshModel);
            UpdateModel(UpDirection.ToVector3());
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
            float size = 5;
            builder.AddCubeFace(new Vector3(0, 0, 0), left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), front, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -front, up, size, size, size);

            builder.AddCubeFace(new Vector3(0, 0, 0), up, left, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -up, -left, size, size, size);

            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);


            var pts = new List<Vector3>();

            var center = up * -size / 2;
            for (int i = 0; i < 20; i++)
            {
                double angle = 0 + (360 * i / (20 - 1));
                double angleRad = angle / 180 * Math.PI;
                var dir = (left * (float)Math.Cos(angleRad)) + (front * (float)Math.Sin(angleRad));
                pts.Add(center + (dir * (size - 1.5f)));
                pts.Add(center + (dir * (size + 0.5f)));
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
                var normal = -hit.NormalAtHit;
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

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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// <para>Viewbox replacement for Viewport using swapchain rendering.</para>
    /// <para>To replace box texture (such as text, colors), bind to custom material with different diffuseMap. </para>
    /// <para>Create a image with 1 row and 6 evenly distributed columns. Each column occupies one box face. The face order is Front, Back, Down, Up, Left, Right</para>
    /// </summary>
    public class ViewBoxModel3D : ScreenSpacedElement3D
    {
        private static readonly MeshGeometry3D defaultBoxModel;
        static ViewBoxModel3D()
        {
            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(Vector3.Zero, 10, 10, 10);
            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);


            var pts = new List<Vector3>();
            var up = new Vector3(1, 0, 0);
            var normal = new Vector3(0, 1, 0);
            var right = Vector3.Cross(up, normal);
            var center = new Vector3(0, -7, 0);
            for (int i = 0; i < 20; i++)
            {
                double angle = 0 + (360 * i / (20 - 1));
                double angleRad = angle / 180 * Math.PI;
                var dir = (right * (float)Math.Cos(angleRad)) + (up * (float)Math.Sin(angleRad));
                pts.Add(center + (dir * 8));
                pts.Add(center + (dir * 12));
            }           
            builder = new MeshBuilder(false, false, false);
            builder.AddTriangleStrip(pts);
            var pie = builder.ToMesh();
            int count = pie.Indices.Count;
            var newMesh = MeshGeometry3D.Merge(new MeshGeometry3D[] { pie, mesh});

            for (int i = 0; i < count; i += 3)
            {
                newMesh.Indices.Add(pie.Indices[i + 2]);
                newMesh.Indices.Add(pie.Indices[i + 1]);
                newMesh.Indices.Add(pie.Indices[i]);
            }

            newMesh.TextureCoordinates = new Core.Vector2Collection(Enumerable.Repeat(new Vector2(0, 0), pie.Positions.Count));
            newMesh.TextureCoordinates.AddRange(mesh.TextureCoordinates);
            newMesh.TextureCoordinates.AddRange(Enumerable.Repeat(new Vector2(0, 0), pie.Positions.Count));

            newMesh.Normals = newMesh.CalculateNormals();

            defaultBoxModel = newMesh;
        }

        private MeshGeometryModel3D ViewBoxMeshModel;

        public ViewBoxModel3D()
        {
            ViewBoxMeshModel = new MeshGeometryModel3D();
            ViewBoxMeshModel.Geometry = defaultBoxModel;
            var map = new MemoryStream();
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.DefaultViewboxTexture.jpg");
            stream.CopyTo(map);
            stream.Dispose();
            ViewBoxMeshModel.Material = new PhongMaterial()
            {
                DiffuseColor = Color.White,
                DiffuseMap = map
            };
            ViewBoxMeshModel.CullMode = CullMode.Back;
            ViewBoxMeshModel.OnSetRenderTechnique += (host) => { return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Diffuse]; };
            this.Children.Add(ViewBoxMeshModel);
        }

        private static void CreateTextureCoordinates(MeshGeometry3D mesh)
        {
            int faces = 6;
            int segment = 4;
            float inc = 1f / faces;
            for(int i=0; i<mesh.TextureCoordinates.Count; ++i)
            {
                mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i/segment), mesh.TextureCoordinates[i].Y);
            }
            ///Correct texture orientation
            var t = mesh.TextureCoordinates[3];
            for(int i = 2; i >=0; --i)
            {
                mesh.TextureCoordinates[i+1] = mesh.TextureCoordinates[i];
            }
            mesh.TextureCoordinates[0] = t;

            t = mesh.TextureCoordinates[4];
            for(int i=4; i<7; ++i)
            {
                mesh.TextureCoordinates[i] = mesh.TextureCoordinates[i + 1];
            }
            mesh.TextureCoordinates[7] = t;
        }
    }
}

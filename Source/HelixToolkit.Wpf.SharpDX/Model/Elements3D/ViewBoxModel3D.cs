// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ViewBoxModel3D : ScreenSpaceMeshGeometry3D
    {
        public ViewBoxModel3D()
        {
            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(Vector3.Zero, 10, 10, 10);
            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);
            UpdateAxisColor(mesh, 0, Color.Red);
            UpdateAxisColor(mesh, 1, Color.Blue);
            UpdateAxisColor(mesh, 2, Color.Green);
            UpdateAxisColor(mesh, 3, Color.Purple);
            UpdateAxisColor(mesh, 4, Color.Yellow);
            UpdateAxisColor(mesh, 5, Color.Gray);
            Geometry = mesh;
            var map = new MemoryStream();
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.DefaultViewboxTexture.jpg");
            stream.CopyTo(map);
            stream.Dispose();
            Material = new PhongMaterial()
            {
                DiffuseColor = Color.White,
                DiffuseMap = map
            };
            CullMode = CullMode.Back;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Diffuse];
        }

        private void UpdateAxisColor(Geometry3D mesh, int which, Color4 color)
        {
            int segment = mesh.Positions.Count / 6;
            var colors = new Core.Color4Collection(mesh.Colors == null ? Enumerable.Repeat<Color4>(Color.Black, mesh.Positions.Count) : mesh.Colors);
            for (int i = segment * which; i < segment * (which + 1); ++i)
            {
                colors[i] = color;
            }
            mesh.Colors = colors;
        }

        protected virtual void CreateTextureCoordinates(MeshGeometry3D mesh)
        {
            int faces = 6;
            int segment = 4;
            float inc = 1f / faces;
            for(int i=0; i<mesh.TextureCoordinates.Count; ++i)
            {
                mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i/segment), mesh.TextureCoordinates[i].Y);
            }
        }
    }
}

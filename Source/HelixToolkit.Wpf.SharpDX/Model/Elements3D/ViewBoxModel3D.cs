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
        private static readonly MeshGeometry3D defaultBoxModel;
        static ViewBoxModel3D()
        {
            var builder = new MeshBuilder(true, true, false);
            builder.AddBox(Vector3.Zero, 10, 10, 10);
            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);
            defaultBoxModel = mesh;
        }
        public ViewBoxModel3D()
        {
            Geometry = defaultBoxModel;
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

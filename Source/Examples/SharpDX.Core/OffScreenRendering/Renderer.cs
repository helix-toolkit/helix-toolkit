using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using SharpDX;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace OffScreenRendering
{
    internal class Renderer
    {
        private readonly ViewportCore viewport = new ViewportCore() { EffectsManager = new DefaultEffectsManager() };
        private readonly Random random = new Random((int)Stopwatch.GetTimestamp());
        private readonly DirectionalLightNode lightNode = new DirectionalLightNode()
        { Direction = new Vector3(-1, -1, 0), Color = Color.White };
        private GroupNode currentScene;

        public Renderer()
        {
            viewport.CameraCore = new OrthographicCameraCore()
            {
                LookDirection = new Vector3(-100, 0, 0),
                UpDirection = Vector3.UnitY,
                Position = new Vector3(100, 0, 0),
                Width = 100,
                NearPlaneDistance = 0.1f,
                FarPlaneDistance = 500
            };
            viewport.FXAALevel = FXAALevel.Medium;
            viewport.Items.AddChildNode(lightNode);
            viewport.StartD3D(100, 100);
        }
        /// <summary>
        /// Must be called from same thread created the viewport object.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            viewport.Resize(width, height);
        }
        /// <summary>
        /// Can be called by different thread.
        /// </summary>
        /// <returns></returns>
        public BitmapSource Render()
        {
            lock (viewport)
            {
                // Remove existing scene and create an new scene
                currentScene?.RemoveSelf();
                currentScene = new GroupNode();
                GenerateSomeMesh(currentScene);
                viewport.Items.AddChildNode(currentScene);
                viewport.Render();
                using var bitmapStream = viewport.RenderToBitmapStream();
                bitmapStream.Position = 0;
                var frame = BitmapFrame.Create(bitmapStream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
                return frame;
            }
        }

        private void GenerateSomeMesh(GroupNode root)
        {
            var builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 5);
            var mesh = builder.ToMesh();
            var numSphere = random.Next(50, 100);
            for (int i = 0; i < numSphere; ++i)
            {
                var meshNode = new MeshNode()
                {
                    Geometry = mesh,
                    Material = new PhongMaterialCore() { DiffuseColor = random.NextColor() },
                    ModelMatrix = Matrix.Translation(random.NextVector3(new Vector3(-50, -50, -50), new Vector3(50, 50, 50)))
                };
                root.AddChildNode(meshNode);
            }
        }
    }
}

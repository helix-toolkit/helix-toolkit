using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Cameras;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace OffScreenRenderingDemo;

internal class Renderer : IDisposable
{
    private readonly ViewportCore viewport = new()
    {
        EffectsManager = new DefaultEffectsManager()
    };

    private readonly Random random = new((int)Stopwatch.GetTimestamp());

    private readonly DirectionalLightNode lightNode = new()
    {
        Direction = new Vector3(-1, -1, 0),
        Color = Color.White
    };

    private GroupNode? currentScene;

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
            if (bitmapStream is not null)
            {
                bitmapStream.Position = 0;
            }
            var frame = BitmapFrame.Create(bitmapStream, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            return frame;
        }
    }

    private void GenerateSomeMesh(GroupNode root)
    {
        var builder = new MeshBuilder();
        builder.AddSphere(Vector3.Zero.ToVector(), 5);
        var mesh = builder.ToMesh().ToMeshGeometry3D();
        var numSphere = random.Next(50, 100);

        for (int i = 0; i < numSphere; ++i)
        {
            var meshNode = new MeshNode()
            {
                Geometry = mesh,
                Material = new PhongMaterialCore()
                {
                    DiffuseColor = random.NextColor()
                },
                ModelMatrix = Matrix.Translation(random.NextVector3(new Vector3(-50, -50, -50), new Vector3(50, 50, 50)))
            };

            root.AddChildNode(meshNode);
        }
    }

    public void Dispose()
    {
        viewport.EffectsManager?.Dispose();
    }
}

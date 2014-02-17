namespace HelixToolkit.Win8
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using HelixToolkit.Win8.CommonDX;

    using SharpDX;
    using SharpDX.DXGI;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;
    using SharpDX.IO;

    public class ExampleCube3D : Element3D
    {
        private SharpDX.Direct3D11.Buffer constantBuffer;
        private InputLayout layout;
        private VertexBufferBinding vertexBufferBinding;
        private Stopwatch clock;
        private VertexShader vertexShader;
        private PixelShader pixelShader;

        public ExampleCube3D()
        {
            this.Scale = 1.0f;
            this.ShowCube = true;
            this.EnableClear = true;
            this.RotationSpeed = 1;
        }

        public bool EnableClear { get; set; }

        public bool ShowCube { get; set; }

        public double Scale { get; set; }

        public double RotationSpeed { get; set; }

        public override void Render(TargetBase render)
        {
            base.Render(render);
            var context = render.DeviceManager.ContextDirect3D;

            var width = render.RenderTargetSize.Width;
            var height = render.RenderTargetSize.Height;

            // Prepare matrices
            var view = SharpDX.Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = SharpDX.Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)(width / height), 0.1f, 100.0f);
            var viewProj = SharpDX.Matrix.Multiply(view, proj);

            var time = (float)(this.clock.ElapsedMilliseconds / 1000.0);

            // Set targets (This is mandatory in the loop)
            context.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);

            // Clear the views
            context.ClearDepthStencilView(render.DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            if (this.EnableClear)
            {
                context.ClearRenderTargetView(render.RenderTargetView, Color.Black);
            }

            if (this.ShowCube)
            {
                // Calculate WorldViewProj
                var worldViewProj = SharpDX.Matrix.Scaling((float)this.Scale) * SharpDX.Matrix.RotationX((float)this.RotationSpeed * time)
                                    * SharpDX.Matrix.RotationY((float)this.RotationSpeed * time * 2.0f) * SharpDX.Matrix.RotationZ((float)this.RotationSpeed * time * .7f)
                                    * viewProj;
                worldViewProj.Transpose();

                // Setup the pipeline
                context.InputAssembler.SetVertexBuffers(0, this.vertexBufferBinding);
                context.InputAssembler.InputLayout = this.layout;
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                context.VertexShader.SetConstantBuffer(0, this.constantBuffer);
                context.VertexShader.Set(this.vertexShader);
                context.PixelShader.Set(this.pixelShader);

                // Update Constant Buffer
                context.UpdateSubresource(ref worldViewProj, this.constantBuffer, 0);

                // Draw the cube
                context.Draw(36, 0);
            }
        }

        public override void Initialize(DeviceManager deviceManager)
        {
            base.Initialize(deviceManager);

            // Remove previous buffer
            if (this.constantBuffer != null)
            {
                this.constantBuffer.Dispose();
            }
            // RemoveAndDispose(ref constantBuffer);

            // Setup local variables
            var d3dDevice = deviceManager.DeviceDirect3D;
            var d3dContext = deviceManager.ContextDirect3D;

            var path = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.Win8");

            // Loads vertex shader bytecode
            var vertexShaderByteCode = NativeFile.ReadAllBytes(path + "\\MiniCube_VS.fxo");
            this.vertexShader = new VertexShader(d3dDevice, vertexShaderByteCode);

            // Loads pixel shader bytecode
            this.pixelShader = new PixelShader(d3dDevice, NativeFile.ReadAllBytes(path + "\\MiniCube_PS.fxo"));

            // Layout from VertexShader input signature
            this.layout = new InputLayout(
                d3dDevice,
                vertexShaderByteCode,
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            // Instantiate Vertex buffer from vertex data
            var vertices = SharpDX.Direct3D11.Buffer.Create(
                d3dDevice,
                BindFlags.VertexBuffer,
                new[]
                    {
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                        new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                        new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                        new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                        new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    });

            this.vertexBufferBinding = new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0);

            // Create Constant Buffer
            this.constantBuffer = new SharpDX.Direct3D11.Buffer(
                d3dDevice,
                Utilities.SizeOf<SharpDX.Matrix>(),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);

            this.clock = new Stopwatch();
            this.clock.Start();
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleCube3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.WinUI
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using HelixToolkit.WinUI.CommonDX;

    public class ExampleCube3D : Element3D
    {
        private global::SharpDX.Direct3D11.Buffer constantBuffer;
        private global::SharpDX.Direct3D11.InputLayout layout;
        private global::SharpDX.Direct3D11.VertexBufferBinding vertexBufferBinding;
        private Stopwatch clock;
        private global::SharpDX.Direct3D11.VertexShader vertexShader;
        private global::SharpDX.Direct3D11.PixelShader pixelShader;

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
            var view = global::SharpDX.Matrix.LookAtLH(new global::SharpDX.Vector3(0, 0, -5), new global::SharpDX.Vector3(0, 0, 0), global::SharpDX.Vector3.UnitY);
            var proj = global::SharpDX.Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)(width / height), 0.1f, 100.0f);
            var viewProj = global::SharpDX.Matrix.Multiply(view, proj);

            var time = (float)(this.clock.ElapsedMilliseconds / 1000.0);

            // Set targets (This is mandatory in the loop)
            context.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);

            // Clear the views
            context.ClearDepthStencilView(render.DepthStencilView, global::SharpDX.Direct3D11.DepthStencilClearFlags.Depth, 1.0f, 0);
            if (this.EnableClear)
            {
                context.ClearRenderTargetView(render.RenderTargetView, global::SharpDX.Color.LightGray);
            }

            if (this.ShowCube)
            {
                // Calculate WorldViewProj
                var worldViewProj = global::SharpDX.Matrix.Scaling((float)this.Scale) * global::SharpDX.Matrix.RotationX((float)this.RotationSpeed * time)
                                    * global::SharpDX.Matrix.RotationY((float)this.RotationSpeed * time * 2.0f) * global::SharpDX.Matrix.RotationZ((float)this.RotationSpeed * time * .7f)
                                    * viewProj;
                worldViewProj.Transpose();
                context.UpdateSubresource(ref worldViewProj, this.constantBuffer, 0);
                // Setup the pipeline
                context.InputAssembler.SetVertexBuffers(0, this.vertexBufferBinding);
                context.InputAssembler.InputLayout = this.layout;
                context.InputAssembler.PrimitiveTopology = global::SharpDX.Direct3D.PrimitiveTopology.TriangleList;

                context.VertexShader.Set(this.vertexShader);
                context.PixelShader.Set(this.pixelShader);
                context.VertexShader.SetConstantBuffer(0, this.constantBuffer);
                // Update Constant Buffer


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

            var path = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.WinUI");

            // Loads vertex shader bytecode
            var vertexShaderByteCode = global::SharpDX.IO.NativeFile.ReadAllBytes(path + "\\MiniCube_VS.fxo");
            this.vertexShader = new global::SharpDX.Direct3D11.VertexShader(d3dDevice, vertexShaderByteCode);

            // Loads pixel shader bytecode
            this.pixelShader = new global::SharpDX.Direct3D11.PixelShader(d3dDevice, global::SharpDX.IO.NativeFile.ReadAllBytes(path + "\\MiniCube_PS.fxo"));

            // Layout from VertexShader input signature
            this.layout = new global::SharpDX.Direct3D11.InputLayout(
                d3dDevice,
                vertexShaderByteCode,
                new[]
                    {
                        new global::SharpDX.Direct3D11.InputElement("POSITION", 0, global::SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new global::SharpDX.Direct3D11.InputElement("COLOR", 0, global::SharpDX.DXGI.Format.R32G32B32A32_Float, 16, 0)
                    });

            // Instantiate Vertex buffer from vertex data
            var vertices = global::SharpDX.Direct3D11.Buffer.Create(
                d3dDevice,
                global::SharpDX.Direct3D11.BindFlags.VertexBuffer,
                new[]
                    {
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                        new global::SharpDX.Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                        new global::SharpDX.Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                        new global::SharpDX.Vector4(1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                        new global::SharpDX.Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(-1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, -1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, -1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                        new global::SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f), new global::SharpDX.Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                    });

            this.vertexBufferBinding = new global::SharpDX.Direct3D11.VertexBufferBinding(vertices, global::SharpDX.Utilities.SizeOf<global::SharpDX.Vector4>() * 2, 0);

            // Create Constant Buffer
            this.constantBuffer = new global::SharpDX.Direct3D11.Buffer(
                d3dDevice,
                global::SharpDX.Utilities.SizeOf< global::SharpDX.Matrix>(),
                global::SharpDX.Direct3D11.ResourceUsage.Default,
                global::SharpDX.Direct3D11.BindFlags.ConstantBuffer,
                global::SharpDX.Direct3D11.CpuAccessFlags.None,
                global::SharpDX.Direct3D11.ResourceOptionFlags.None,
                0);

            this.clock = new Stopwatch();
            this.clock.Start();
        }
    }
}
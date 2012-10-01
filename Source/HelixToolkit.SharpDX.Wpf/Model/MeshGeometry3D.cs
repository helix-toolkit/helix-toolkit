namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;
    using global::SharpDX.DXGI;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D10;

    using Point3D = global::SharpDX.Vector3;
    using Point = global::SharpDX.Vector2;

    public class MeshGeometry3D : Geometry3D
    {
        public Point3D[] Positions { get; set; }
        public Point[] TextureCoordinates { get; set; }
        public int[] TriangleIndices { get; set; }

        private Buffer Vertices;
        private InputLayout VertexLayout;
        private DataStream VertexStream;

        private DataStream IndexStream;
        private Buffer IndexBuffer;

        private global::SharpDX.Direct3D10.Device device;

        internal void Attach(IRenderHost host)
        {
            this.device = host.Device;
            //this.VertexLayout = new InputLayout(device, pass.Description.Signature, new[] {
            //    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
            //    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0) 
            //});

            this.VertexStream = new DataStream(4 * this.Positions.Length, true, true);
            this.VertexStream.WriteRange(this.Positions);
            this.VertexStream.Position = 0;

            var bufferDescription = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = 4 * Positions.Length,
                    Usage = ResourceUsage.Default
                };

            this.Vertices = new Buffer(device, this.VertexStream, bufferDescription);

            this.IndexStream = new DataStream(this.TriangleIndices.Length, true, true);
            this.IndexStream.WriteRange(this.TriangleIndices);
            this.IndexStream.Position = 0;

            var indexBufferDescription = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * TriangleIndices.Length,
                Usage = ResourceUsage.Default
            };
            this.IndexBuffer = new Buffer(device, this.IndexStream, indexBufferDescription);
        }

        internal void Detach()
        {
            if (Vertices != null) Vertices.Dispose();
            Vertices = null;
            if (VertexLayout != null) VertexLayout.Dispose();
            VertexLayout = null;
            if (VertexStream != null) VertexStream.Dispose();
            VertexStream = null;

            if (IndexBuffer != null) IndexBuffer.Dispose();
            IndexBuffer = null;
            if (IndexStream != null) IndexStream.Dispose();
            IndexStream = null;

            this.device = null;
        }

        internal void Render()
        {
            if (device == null)
                return;

            //device.InputAssembler.InputLayout = this.VertexLayout;
            device.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.Vertices, 24, 0));

            //EffectTechnique technique = this.SimpleEffect.GetTechniqueByIndex(0);
            //EffectPass pass = technique.GetPassByIndex(0);

            //EffectVectorVariable overlayColor = this.SimpleEffect.GetVariableBySemantic("OverlayColor").AsVector();

            //overlayColor.Set(this.OverlayColor);

            //for (int i = 0; i < technique.Description.PassCount; ++i)
            //{
            //    pass.Apply();
            //    device.Draw(3, 0);
            //}

            device.InputAssembler.SetIndexBuffer(this.IndexBuffer, Format.R32_SInt, 0);
            device.DrawIndexed(this.TriangleIndices.Length, 0, 0);
        }
    }
}
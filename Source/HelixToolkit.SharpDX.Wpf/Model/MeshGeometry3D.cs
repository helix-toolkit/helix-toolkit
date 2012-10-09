namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;
    using global::SharpDX.DXGI;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D10;
    using global::SharpDX.D3DCompiler;

    using Direct3D = global::SharpDX.Direct3D;
    using Direct3D10 = global::SharpDX.Direct3D10;
    using Device = global::SharpDX.Direct3D10.Device;
    using Point3D = global::SharpDX.Vector3;
    using Point2D = global::SharpDX.Vector2;

    using System.Runtime.InteropServices;
    using System.Resources;
    using System.Linq;
    using System.IO;
   
    

    public class MeshGeometry3D : Geometry3D
    {
        public Point3D[] Positions { get; set; }
        public Point3D[] Normals { get; set; }
        public Point2D[] TextureCoordinates { get; set; }
        public int[] TriangleIndices { get; set; }

        private Color4 m_overlayColor = new Color4(0.99f);
        private Device m_device;
        private Effect m_effect;        
        private InputLayout m_vertexLayout;        
        private Buffer m_vertexBuffer;
        private Buffer m_indexBuffer;         

        private EffectMatrixVariable m_world;
        private EffectMatrixVariable m_view;
        private EffectMatrixVariable m_projection;
        private EffectVectorVariable m_vLightDir;
        private EffectVectorVariable m_vLightColor;
        private EffectVectorVariable m_vOutputColor;
        private EffectVectorVariable m_vOverlayColor;
        private EffectShaderResourceVariable m_textureDiffuse;

        private EffectTechnique m_techniqueRender;
        private EffectTechnique m_techniqueRenderLight;
        
        private Camera m_camera;
        private FpsCounter m_counter;
        private Viewport3DX m_viewport;

        private static System.Diagnostics.Stopwatch s_stopWatch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct Vertex
        {          
            public Vector4 Position;
            public Color4 Color;
            public Vector2 TexCoord;
            public Vector3 Normal;
            public const int SizeInBytes = 4 * (4 + 4 + 2 + 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        internal void Attach(IRenderHost host)
        {
            m_device = host.Device;

            var sFlags = ShaderFlags.None;
            var eFlags = EffectFlags.None;
#if DEBUG
            sFlags |= ShaderFlags.Debug;
            eFlags |= EffectFlags.None;
#endif
            /// --- load effect
            var shaderBytes = ShaderBytecode.Compile(Properties.Resources.Default, "fx_4_0", sFlags, eFlags, null, null);
            if (shaderBytes.HasErrors)            
                System.Windows.MessageBox.Show(string.Format("Error compiling effect: {0}", shaderBytes.Message), "Error");            

            /// --- effect and techiques
            m_effect = new Effect(m_device, shaderBytes);
            m_techniqueRender = m_effect.GetTechniqueByName("Render");
            m_techniqueRenderLight = m_effect.GetTechniqueByName("RenderLight");                       
            var pass = m_techniqueRender.GetPassByIndex(0);
            
            /// --- create a vertex layout for current shader
            m_vertexLayout = new InputLayout(m_device, pass.Description.Signature, new[] 
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 40, 0),                 
            });

            
            /// --- set up buffers
            var rnd = new System.Random(951357);
            m_vertexBuffer = CreateBuffer(m_device, Vertex.SizeInBytes, Positions.Select((x, ii) => new Vertex()
            {
                Position = new Vector4(x, 1f),
                Color = new Color4((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), 1f),
                TexCoord = TextureCoordinates[ii],
                Normal = Normals[ii],
            }).ToArray());
            m_indexBuffer = CreateBuffer(m_device, sizeof(int), TriangleIndices.Select(x => (ushort)x).ToArray());

            /// --- provisionally, add the camera parameters here.... 
            var canvas = host as DPFCanvas;
            m_viewport = canvas.Renderable as Viewport3DX;
            m_camera = m_viewport.Camera;
            m_counter = m_viewport.FpsCounter;

            /// --- set world matrix
            var world = Matrix.Identity;
            /// --- compute view matrix
            var view = m_camera.CreateViewMatrix();
            /// --- compute projection            
            var projection = m_camera.CreateProjectionMatrix(m_viewport.RenderSize.Width / m_viewport.RenderSize.Height);

            /// --- constant paramerers
            m_world = m_effect.GetVariableByName("World").AsMatrix();
            m_world.SetMatrix(ref world);
            m_view = m_effect.GetVariableByName("View").AsMatrix();
            m_view.SetMatrix(ref view);
            m_projection = m_effect.GetVariableByName("Projection").AsMatrix();
            m_projection.SetMatrix(projection);

            /// --- light contant params
            m_vLightDir = m_effect.GetVariableByName("vLightDir").AsVector();
            m_vLightColor = m_effect.GetVariableByName("vLightColor").AsVector();
            m_vOutputColor = m_effect.GetVariableByName("vOutputColor").AsVector();

            /// --- overlay color
            m_vOverlayColor = m_effect.GetVariableBySemantic("OverlayColor").AsVector();

            /// --- textures
            m_textureDiffuse = m_effect.GetVariableByName("texDiffuse").AsShaderResource();
            var bitmap = Properties.Resources.SeamlessWallTexture06;
            var data = ToByteArray(Properties.Resources.SeamlessWallTexture06, System.Drawing.Imaging.ImageFormat.Bmp);

            //var diffuseTex = Texture2D.FromMemory<Texture2D>(m_device, data);
            var diffuseTexView = ShaderResourceView.FromMemory(m_device, data);            
            m_textureDiffuse.SetResource(diffuseTexView);

            /// --- start stop watch
            s_stopWatch.Start();

            /// --- flush
            m_device.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Detach()
        {
            Disposer.RemoveAndDispose(ref m_vertexBuffer);           
            Disposer.RemoveAndDispose(ref m_vertexLayout);
            Disposer.RemoveAndDispose(ref m_indexBuffer);
            Disposer.RemoveAndDispose(ref m_effect);
            Disposer.RemoveAndDispose(ref m_techniqueRender);
            Disposer.RemoveAndDispose(ref m_techniqueRenderLight);
            Disposer.RemoveAndDispose(ref m_textureDiffuse);            
        }        
        
        /// <summary>
        /// 
        /// </summary>
        internal void Render()
        {
            if (m_device == null) return;
            
            m_counter.AddFrame(s_stopWatch.Elapsed);

            /// --- set buffers
            m_device.InputAssembler.InputLayout = m_vertexLayout;
            m_device.InputAssembler.PrimitiveTopology = Direct3D.PrimitiveTopology.TriangleList;
            m_device.InputAssembler.SetIndexBuffer(m_indexBuffer, Format.R16_UInt, 0);
            m_device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(m_vertexBuffer, Vertex.SizeInBytes, 0));

            /// --- compute world matrix
            var t = s_stopWatch.ElapsedMilliseconds;
            var world = Matrix.RotationY(t / 1000f);
            /// --- compute view matrix
            var view = m_camera.CreateViewMatrix();
            /// --- compute projection matrix
            var projection = m_camera.CreateProjectionMatrix(m_viewport.RenderSize.Width / m_viewport.RenderSize.Height);
            /// --- set constant paramerers            
            m_world.SetMatrix(ref world);
            m_view.SetMatrix(ref view);
            m_projection.SetMatrix(ref projection);

            /// --- set overlay                                    
            m_vOverlayColor.Set(ref m_overlayColor);

            /// --- render the cube
            for (int i = 0; i < m_techniqueRender.Description.PassCount; i++)
            {
                m_techniqueRender.GetPassByIndex(i).Apply();
                m_device.DrawIndexed(TriangleIndices.Length, 0, 0);
            }

            /// --- Setup our lighting parameters
            var vLightDirs = new[]
            {
                new Vector4( -0.577f, 0.577f, -0.577f, 1.0f ),
                new Vector4( 0.0f, 0.0f, -1.0f, 1.0f ),
            };
            var vLightColors = new[]
            {
                new Color4( 0.5f, 0.5f, 0.5f, 1.0f ),
                new Color4( 0.5f, 0.0f, 0.0f, 1.0f ),
            };

            /// --- Rotate the second light around the origin
            Matrix mRotate = Matrix.RotationY(-2.0f * t/1000);
            var dir1 = vLightDirs[1];
            vLightDirs[1] = Vector3.Transform(new Vector3(dir1.X, dir1.Y, dir1.Z), mRotate);            

            /// --- Update lighting variables                     
            m_vLightDir.Set(vLightDirs);            
            m_vLightColor.Set(vLightColors);

            /// --- Render each light            
            for (int m = 0; m < 2; m++)
            {                                
                Vector4 vLightPos = vLightDirs[m] * 5.0f;
                Matrix mLight = Matrix.Translation(vLightPos.X, vLightPos.Y, vLightPos.Z);
                Matrix mLightScale = Matrix.Scaling(0.2f);                
                mLight = mLightScale * mLight;

                /// --- Update the world variable to reflect the current light                
                m_world.SetMatrix(ref mLight);                
                m_vOutputColor.Set(vLightColors[m]);

                for (int p = 0; p < m_techniqueRenderLight.Description.PassCount; p++)
                {
                    m_techniqueRenderLight.GetPassByIndex(p).Apply();                    
                    m_device.DrawIndexed(TriangleIndices.Length, 0, 0);
                }
            } 
        }
    }
}
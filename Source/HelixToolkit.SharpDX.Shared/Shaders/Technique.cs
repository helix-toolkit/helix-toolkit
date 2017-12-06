using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using ShaderManager;
    public class Technique : DisposeObject
    {
        public InputLayout Layout { private set; get; }
        public Device Device { private set; get; }
        public string Name { private set; get; }
        private readonly Dictionary<ShaderStage, ShaderBase> shaders = new Dictionary<ShaderStage, ShaderBase>();
        public IEnumerable<ShaderBase> Shaders { get { return shaders.Values; } }

        public BlendState BlendState { private set; get; }

        public Technique(string name, Device device, byte[] byteCode, InputElement[] inputElements, IList<ShaderDescription> shaderList, IConstantBufferPool cbPool)
        {
            Name = name;
            Layout = Collect(new InputLayout(device, byteCode, inputElements));
            if (shaderList != null)
            {
                foreach(var shader in shaderList)
                {
                    shaders.Add(shader.ShaderType, Collect(shader.CreateShader(device, cbPool)));
                }
            }
        }
        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        public void BindShader(DeviceContext context)
        {
            foreach(var shader in Shaders)
            {
                shader.Bind(context);
                shader.BindConstantBuffers(context);
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            shaders.Clear();
            Device = null;
            Layout = null;
            base.Dispose(disposeManagedResources);
        }
    }
}

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
    public class Technique :  DisposeObject, IRenderTechnique
    {
        public InputLayout Layout { private set; get; }
        public Device Device { private set; get; }
        public string Name { private set; get; }
        private readonly Dictionary<ShaderStage, ShaderBase> shaders = new Dictionary<ShaderStage, ShaderBase>();
        public IEnumerable<ShaderBase> Shaders { get { return shaders.Values; } }

        public BlendState BlendState { private set; get; } = null;

        public DepthStencilState DepthStencilState { private set; get; } = null;

        public RasterizerState RasterState { private set; get; } = null;

        public IConstantBufferPool ConstantBufferPool { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Technique Name</param>
        /// <param name="device"></param>
        /// <param name="byteCode">Vertex shader byte code</param>
        /// <param name="inputElements">Vertex shader input layout elements</param>
        /// <param name="shaderList"></param>
        /// <param name="cbPool">Constant Buffer Pool</param>
        public Technique(TechniqueDescription description, Device device, IEffectsManager manager)
        {
            Name = description.Name;
            Device = device;
            Layout = manager.ShaderManager.RegisterInputLayout(description.InputLayoutDescription);
            if (description.ShaderList != null)
            {
                foreach(var shader in description.ShaderList)
                {
                    shaders.Add(shader.ShaderType, manager.ShaderManager.RegisterShader(shader));
                }
            }

            if (!shaders.ContainsKey(ShaderStage.Domain))
            {
                shaders.Add(ShaderStage.Domain, new NullShader(ShaderStage.Domain));
            }
            if (!shaders.ContainsKey(ShaderStage.Hull))
            {
                shaders.Add(ShaderStage.Hull, new NullShader(ShaderStage.Hull));
            }
            if (!shaders.ContainsKey(ShaderStage.Geometry))
            {
                shaders.Add(ShaderStage.Geometry, new NullShader(ShaderStage.Geometry));
            }
            if (!shaders.ContainsKey(ShaderStage.Compute))
            {
                shaders.Add(ShaderStage.Compute, new NullShader(ShaderStage.Compute));
            }
            ConstantBufferPool = manager.ConstantBufferPool;

            BlendState = description.BlendStateDescription != null ? manager.StateManager.Register((BlendStateDescription)description.BlendStateDescription) : null;

            DepthStencilState = description.DepthStencilStateDescription != null ? manager.StateManager.Register((DepthStencilStateDescription)description.DepthStencilStateDescription) : null;

            RasterState = description.RasterStateDescription != null ? manager.StateManager.Register((RasterizerStateDescription)description.RasterStateDescription) : null;
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

        public ShaderBase GetShader(ShaderStage type)
        {
            if (shaders.ContainsKey(type))
            {
                return shaders[type];
            }
            else
            {
                return new NullShader(type);
            }
        }

        public void BindStates(DeviceContext context, StateType type)
        {
            if(type == StateType.None)
            {
                return;
            }
            if(type.HasFlag(StateType.BlendState))
            {
                context.OutputMerger.BlendState = BlendState;             
            }
            if(type.HasFlag(StateType.DepthStencilState))
            {
                context.OutputMerger.DepthStencilState = DepthStencilState;
            }
            if(type.HasFlag(StateType.RasterState))
            {
                context.Rasterizer.State = RasterState;
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

using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using global::SharpDX.Direct3D11;
    using Shaders;

    public class ShaderPool : ResourcePoolBase<byte[], IShader, ShaderDescription>
    {
        public IConstantBufferPool ConstantBufferPool { private set; get; }
        public ShaderPool(Device device, IConstantBufferPool cbPool)
            :base(device)
        {
            ConstantBufferPool = cbPool;
        }

        public override IShader Register(ShaderDescription description)
        {
            if (description.ByteCode == null)
            {
                return new NullShader(description.ShaderType);
            }
            else if (pool.ContainsKey(description.ByteCode))
            {
                return pool[description.ByteCode];
            }
            else
            {
                var shader = Collect(description.CreateShader(Device, ConstantBufferPool));
                pool.Add(description.ByteCode, shader);
                return shader;
            }
        }
    }

    public class LayoutPool : ResourcePoolBase<byte[], InputLayout, Tuple<byte[], InputElement[]>>
    {
        public LayoutPool(Device device)
            :base(device)
        { }

        public override InputLayout Register(Tuple<byte[], InputElement[]> description)
        {
            if (description.Item1 == null || description.Item2 == null)
            {
                return null;
            }
            else if (pool.ContainsKey(description.Item1))
            {
                return pool[description.Item1];
            }
            else
            {
                var layout = Collect(new InputLayout(Device, description.Item1, description.Item2));
                pool.Add(description.Item1, layout);
                return layout;
            }
        }
    }

    public class ShaderPoolManager : DisposeObject, IShaderPoolManager
    {
        private readonly Dictionary<ShaderStage, ShaderPool> shaderPools = new Dictionary<ShaderStage, ShaderPool>();
        private readonly LayoutPool layoutPool;

        public ShaderPoolManager(Device device, IConstantBufferPool cbPool)
        {
            shaderPools.Add(ShaderStage.Vertex, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Domain, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Hull, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Geometry, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Pixel, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Compute, Collect(new ShaderPool(device, cbPool)));
            layoutPool = Collect(new LayoutPool(device));
        }

        public IShader RegisterShader(ShaderDescription description)
        {
            return shaderPools[description.ShaderType].Register(description);
        }

        public InputLayout RegisterInputLayout(InputLayoutDescription description)
        {
            return layoutPool.Register(description.Description);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            shaderPools.Clear();
            base.Dispose(disposeManagedResources);
        }       
    }
}

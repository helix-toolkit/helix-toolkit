/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using global::SharpDX;
using SharpDX.Direct3D11;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;
    

    [DataContract]
    public abstract class GenericMaterialCore : MaterialCore
    {
        internal sealed class UpdateEvent
        {
            public readonly GenericMaterialVariable.ResourceType Type;
            public readonly string Name;
            public UpdateEvent(GenericMaterialVariable.ResourceType type, string name)
            {
                Type = type;
                Name = name;
            }
        }

        internal event EventHandler<UpdateEvent> UpdatingResource;
        [DataMember]
        public Dictionary<string, Stream> TextureDict { get; } = new Dictionary<string, Stream>();
        [DataMember]
        public Dictionary<string, SamplerStateDescription> SamplerDict { get; } = new Dictionary<string, SamplerStateDescription>();
        [DataMember]
        public Dictionary<string, float> FloatDict { get; } = new Dictionary<string, float>();
        [DataMember]
        public Dictionary<string, bool> BoolDict { get; } = new Dictionary<string, bool>();
        [DataMember]
        public Dictionary<string, Vector2> Vector2Dict { get; } = new Dictionary<string, Vector2>();
        [DataMember]
        public Dictionary<string, Vector3> Vector3Dict { get; } = new Dictionary<string, Vector3>();
        [DataMember]
        public Dictionary<string, Vector4> Vector4Dict { get; } = new Dictionary<string, Vector4>();
        [DataMember]
        public Dictionary<string, Matrix> MatrixDict { get; } = new Dictionary<string, Matrix>();
        [DataMember]
        public string MaterialPassName { set; get; } = DefaultPassNames.Default;
        [DataMember]
        public string ShadowPassName { set; get; } = DefaultPassNames.ShadowPass;
        [DataMember]
        public string WireframePassName { set; get; } = DefaultPassNames.Wireframe;

        public string[] PropertieNames { get; }
        public string[] TextureNames { get; }
        public string[] SamplerNames { get; }

        protected readonly ConstantBufferDescription cbDescription;

        public GenericMaterialCore(
            string materialShaderPassName,
            string shadowShaderPassName,
            string wireframePassName, 
            ConstantBufferDescription constantBufferDesc)
        {
            this.MaterialPassName = materialShaderPassName;
            this.ShadowPassName = shadowShaderPassName;
            this.WireframePassName = wireframePassName;
            cbDescription = constantBufferDesc;
        }

        public GenericMaterialCore(ConstantBufferDescription constantBufferDesc)
        {
            cbDescription = constantBufferDesc;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMaterialCore"/> class.
        /// </summary>
        /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
        /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
        public GenericMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
        {
            if (shaderPass.IsNULL || shaderPass.PixelShader.IsNULL)
            {
                return;
            }
            List<string> properties = new List<string>();
            var cb = shaderPass.PixelShader.ConstantBufferMapping.Mappings
                .Where(x => x.Value.Name == modelMaterialConstantBufferName).FirstOrDefault();

            if (cb.Value != null)
            {
                cbDescription = new ConstantBufferDescription(cb.Value.Name, cb.Value.bufferDesc.SizeInBytes);
                properties.AddRange(cb.Value.VariableDictionary.Keys);
            }                       
            
            PropertieNames = properties.ToArray();
            TextureNames = shaderPass.PixelShader.ShaderResourceViewMapping.Mappings.Select(x => x.Value.Description.Name).ToArray();
            SamplerNames = shaderPass.PixelShader.SamplerMapping.Mappings.Select(x => x.Value.Name).ToArray();
        }

        public void SetTexture(string name, Stream texture)
        {
            if (TextureDict.ContainsKey(name))
            {
                TextureDict[name] = texture;
            }
            else
            {
                TextureDict.Add(name, texture);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Texture, name));
        }


        public void SetSampler(string name, SamplerStateDescription samplerDesc)
        {
            if (SamplerDict.ContainsKey(name))
            {
                SamplerDict[name] = samplerDesc;
            }
            else
            {
                SamplerDict.Add(name, samplerDesc);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Sampler, name));
        }

        public Stream GetTexture(string name)
        {
            if(TextureDict.TryGetValue(name, out var texture))
            {
                return texture;
            }
            else
            {
                return null;
            }
            
        }

        public SamplerStateDescription GetSampler(string name)
        {
            if(SamplerDict.TryGetValue(name, out var samplerDesc))
            {
                return samplerDesc;
            }
            else
            {
                return new SamplerStateDescription();
            }         
        }


        public void SetProperty(string name, int value)
        {
            if (FloatDict.ContainsKey(name))
            {
                FloatDict[name] = value;
            }
            else
            {
                FloatDict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Float, name));
        }

        public void SetProperty(string name, float value)
        {
            if (FloatDict.ContainsKey(name))
            {
                FloatDict[name] = value;
            }
            else
            {
                FloatDict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Float, name));
        }

        public void SetProperty(string name, bool value)
        {
            if (FloatDict.ContainsKey(name))
            {
                FloatDict[name] = value ? 1 : 0;
            }
            else
            {
                FloatDict.Add(name, value ? 1 : 0);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Float, name));
        }

        public void SetProperty(string name, Vector2 value)
        {
            if (Vector2Dict.ContainsKey(name))
            {
                Vector2Dict[name] = value;
            }
            else
            {
                Vector2Dict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Vector2, name));
        }

        public void SetProperty(string name, Vector3 value)
        {
            if (Vector3Dict.ContainsKey(name))
            {
                Vector3Dict[name] = value;
            }
            else
            {
                Vector3Dict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Vector3, name));
        }

        public void SetProperty(string name, Vector4 value)
        {
            if (Vector4Dict.ContainsKey(name))
            {
                Vector4Dict[name] = value;
            }
            else
            {
                Vector4Dict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Vector4, name));
        }

        public void SetProperty(string name, Matrix value)
        {
            if (MatrixDict.ContainsKey(name))
            {
                MatrixDict[name] = value;
            }
            else
            {
                MatrixDict.Add(name, value);
            }
            UpdatingResource?.Invoke(this, new UpdateEvent(GenericMaterialVariable.ResourceType.Matrix, name));
        }
    }

    public sealed class GenericMeshMaterialCore : GenericMaterialCore
    {
        public GenericMeshMaterialCore()
            :base(MaterialVariable.DefaultMeshConstantBufferDesc)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMeshMaterialCore"/> class.
        /// </summary>
        /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
        /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
        public GenericMeshMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
            :base(shaderPass, modelMaterialConstantBufferName)
        {

        }
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericMeshMaterialVariable(manager, technique, this, cbDescription,
                MaterialPassName, ShadowPassName, WireframePassName);
        }
    }

    public sealed class GenericLineMaterialCore : GenericMaterialCore
    {
        public GenericLineMaterialCore()
            :base(MaterialVariable.DefaultPointLineConstantBufferDesc)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericLineMaterialCore"/> class.
        /// </summary>
        /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
        /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
        public GenericLineMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
            : base(shaderPass, modelMaterialConstantBufferName)
        {

        }
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericMeshMaterialVariable(manager, technique, this, cbDescription,
                MaterialPassName, ShadowPassName, "");
        }
    }

    public sealed class GenericPointMaterialCore : GenericMaterialCore
    {
        public GenericPointMaterialCore()
            :base(MaterialVariable.DefaultPointLineConstantBufferDesc)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPointMaterialCore"/> class.
        /// </summary>
        /// <param name="shaderPass">The shader pass. Currently only supports pixel shader parameter properties</param>
        /// <param name="modelMaterialConstantBufferName">Name of the model material constant buffer in pixel shader.</param>
        public GenericPointMaterialCore(ShaderPass shaderPass, string modelMaterialConstantBufferName)
            : base(shaderPass, modelMaterialConstantBufferName)
        {

        }
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericPointMaterialVariable(manager, technique, this, cbDescription,
                MaterialPassName, ShadowPassName);
        }
    }
}

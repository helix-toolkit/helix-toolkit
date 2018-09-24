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

        public ConstantBufferDescription ConstantBufferDesc { set; get; } = MaterialVariable.DefaultMeshConstantBufferDesc;

        public GenericMaterialCore(
            string materialShaderPassName,
            string shadowShaderPassName,
            string wireframePassName)
        {
            this.MaterialPassName = materialShaderPassName;
            this.ShadowPassName = shadowShaderPassName;
            this.WireframePassName = wireframePassName;
        }

        public GenericMaterialCore()
        {

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
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericMeshMaterialVariable(manager, technique, this, MaterialVariable.DefaultMeshConstantBufferDesc,
                MaterialPassName, ShadowPassName, WireframePassName);
        }
    }

    public sealed class GenericLineMaterialCore : GenericMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericMeshMaterialVariable(manager, technique, this, MaterialVariable.DefaultPointLineConstantBufferDesc,
                MaterialPassName, ShadowPassName, "");
        }
    }

    public sealed class GenericPointMaterialCore : GenericMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new GenericPointMaterialVariable(manager, technique, this, MaterialVariable.DefaultPointLineConstantBufferDesc,
                MaterialPassName, ShadowPassName);
        }
    }
}

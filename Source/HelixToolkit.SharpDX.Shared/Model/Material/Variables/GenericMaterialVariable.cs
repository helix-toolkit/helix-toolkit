/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using Shaders;
    using System.Collections.Generic;
    using System.IO;
    using Utilities;

    public abstract class GenericMaterialVariable : MaterialVariable
    {
        public enum ResourceType
        {
            Texture, Sampler, Float, Vector2, Vector3, Vector4, Matrix
        }
        private readonly ShaderPass materialPass, shadowPass, wireframePass;


        private readonly KeyValuePair<int, ShaderResourceViewProxy>[] shaderResources;
        private readonly KeyValuePair<int, SamplerStateProxy>[] samplerResources;

        private readonly Dictionary<string, int> resourceIdxDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> samplerIdxDict = new Dictionary<string, int>();
        private readonly GenericMaterialCore core;

        public GenericMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
            GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
            string materialShaderPassName = DefaultPassNames.Default,
            string shadowShaderPassName = DefaultPassNames.ShadowPass,
            string wireframePassName = DefaultPassNames.Wireframe)
            : base(manager, technique, constantBufferDescription, materialCore)
        {
            core = materialCore;
            materialPass = technique[materialShaderPassName];
            shadowPass = technique[shadowShaderPassName];
            wireframePass = technique[wireframePassName];
            shaderResources = new KeyValuePair<int, ShaderResourceViewProxy>[materialPass.PixelShader.ShaderResourceViewMapping.Count];

            for(int i=0; i < materialPass.PixelShader.ShaderResourceViewMapping.Count; ++i)
            {
                var mapping = materialPass.PixelShader.ShaderResourceViewMapping.Mappings[i];
                resourceIdxDict.Add(mapping.Value.Description.Name, i);
                shaderResources[i] = new KeyValuePair<int, ShaderResourceViewProxy>(mapping.Key, null);
            }

            samplerResources = new KeyValuePair<int, SamplerStateProxy>[materialPass.PixelShader.SamplerMapping.Count];

            for(int i=0; i < materialPass.PixelShader.SamplerMapping.Count; ++i)
            {
                var mapping = materialPass.PixelShader.SamplerMapping.Mappings[i];
                samplerIdxDict.Add(mapping.Value.Name, i);
                samplerResources[i] = new KeyValuePair<int, SamplerStateProxy>(mapping.Key, null);
            }

            foreach(var texture in materialCore.TextureDict)
            {
                SetTexture(texture.Key, texture.Value);
            }

            foreach(var sampler in materialCore.SamplerDict)
            {
                SetSampler(sampler.Key, sampler.Value);
            }

            foreach(var prop in materialCore.FloatDict)
            {
                WriteValue(prop.Key, prop.Value);
            }
            foreach (var prop in materialCore.Vector2Dict)
            {
                WriteValue(prop.Key, prop.Value);
            }
            foreach (var prop in materialCore.Vector3Dict)
            {
                WriteValue(prop.Key, prop.Value);
            }
            foreach (var prop in materialCore.Vector4Dict)
            {
                WriteValue(prop.Key, prop.Value);
            }
            foreach (var prop in materialCore.MatrixDict)
            {
                WriteValue(prop.Key, prop.Value);
            }
            materialCore.UpdatingResource += MaterialCore_UpdatingResource;
        }

        private void MaterialCore_UpdatingResource(object sender, GenericMaterialCore.UpdateEvent e)
        {
            switch (e.Type)
            {
                case ResourceType.Sampler:
                    SetSampler(e.Name, core.GetSampler(e.Name));
                    break;
                case ResourceType.Texture:
                    SetTexture(e.Name, core.GetTexture(e.Name));
                    break;
                case ResourceType.Float:
                    WriteValue(e.Name, core.FloatDict[e.Name]);
                    break;
                case ResourceType.Vector2:
                    WriteValue(e.Name, core.Vector2Dict[e.Name]);
                    break;
                case ResourceType.Vector3:
                    WriteValue(e.Name, core.Vector3Dict[e.Name]);
                    break;
                case ResourceType.Vector4:
                    WriteValue(e.Name, core.Vector4Dict[e.Name]);
                    break;
                case ResourceType.Matrix:
                    WriteValue(e.Name, core.MatrixDict[e.Name]);
                    break;
            }
        }

        public bool SetTexture(string name, Stream texture)
        {
            if(resourceIdxDict.TryGetValue(name, out var idx))
            {
                var exist = shaderResources[idx].Value;
                RemoveAndDispose(ref exist);
                if (texture == null)
                {
                    shaderResources[idx] = new KeyValuePair<int, ShaderResourceViewProxy>(shaderResources[idx].Key, null);
                }
                else
                {
                    var res = Collect(EffectsManager.MaterialTextureManager.Register(texture));
                    shaderResources[idx] = new KeyValuePair<int, ShaderResourceViewProxy>(shaderResources[idx].Key, res);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetSampler(string name, global::SharpDX.Direct3D11.SamplerStateDescription sampler)
        {
            if (resourceIdxDict.TryGetValue(name, out var idx))
            {
                var exist = samplerResources[idx].Value;
                RemoveAndDispose(ref exist);
                var res = Collect(EffectsManager.StateManager.Register(sampler));
                samplerResources[idx] = new KeyValuePair<int, SamplerStateProxy>(samplerResources[idx].Key, res);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            foreach(var res in shaderResources)
            {
                deviceContext.SetShaderResource(PixelShader.Type, res.Key, res.Value);
            }
            return true;
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return materialPass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return shadowPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return wireframePass;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                core.UpdatingResource -= MaterialCore_UpdatingResource;
            }
            base.OnDispose(disposeManagedResources);
        }
    }

    public sealed class GenericMeshMaterialVariable : GenericMaterialVariable
    {
        public GenericMeshMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
            GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
            string materialShaderPassName = DefaultPassNames.Default,
            string shadowShaderPassName = DefaultPassNames.ShadowPass,
            string wireframePassName = DefaultPassNames.Wireframe)
            :base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, wireframePassName)
        {

        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }
    }

    public sealed class GenericPointMaterialVariable : GenericMaterialVariable
    {
        public GenericPointMaterialVariable(IEffectsManager manager, IRenderTechnique technique,
            GenericMaterialCore materialCore, ConstantBufferDescription constantBufferDescription,
            string materialShaderPassName = DefaultPassNames.Default,
            string shadowShaderPassName = DefaultPassNames.ShadowPass)
            : base(manager, technique, materialCore, constantBufferDescription, materialShaderPassName, shadowShaderPassName, "")
        {

        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawPoints(deviceContext, bufferModel.VertexBuffer[0].ElementCount, instanceCount);
        }
    }
}

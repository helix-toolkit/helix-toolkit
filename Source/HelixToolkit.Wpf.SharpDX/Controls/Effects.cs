// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Effects.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Names of techniques which are implemented by default
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using global::SharpDX;
    using global::SharpDX.DXGI;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.D3DCompiler;
    using global::SharpDX.Direct3D;
    using System.IO;

    public interface IEffectsManager
    {
        InputLayout GetLayout(RenderTechnique technique);
        Effect GetEffect(RenderTechnique technique);
        global::SharpDX.Direct3D11.Device Device { get; }
        int AdapterIndex { get; }
    }

    /// <summary>
    /// An Effects manager which includes all standard effects, 
    /// tessellation, and deferred effects.
    /// </summary>
    public class DefaultEffectsManager : IEffectsManager, IDisposable
    {
        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        private const FeatureLevel MinimumFeatureLevel = FeatureLevel.Level_10_0;

        protected IRenderTechniquesManager renderTechniquesManager;

        /// <summary>
        /// Construct an EffectsManager
        /// </summary>
        public DefaultEffectsManager(IRenderTechniquesManager renderTechniquesManager)
        {
            this.renderTechniquesManager = renderTechniquesManager;
#if DX11
            var adapter = GetBestAdapter(out adapterIndex);

            if (adapter != null)
            {
                if (adapter.Description.VendorId == 0x1414 && adapter.Description.DeviceId == 0x8c)
                {
                    driverType = DriverType.Warp;
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
                }
                else
                {
                    driverType = DriverType.Hardware;
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
                    // DeviceCreationFlags.Debug should not be used in productive mode!
                    // See: http://sharpdx.org/forum/4-general/1774-how-to-debug-a-sharpdxexception
                    // See: http://stackoverflow.com/questions/19810462/launching-sharpdx-directx-app-with-devicecreationflags-debug
                }
            }
#else
            this.device = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1);
#endif
            InitEffects();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Adapter GetBestAdapter(out int bestAdapterIndex)
        {
            using (var f = new Factory1())
            {
                Adapter bestAdapter = null;
                bestAdapterIndex = -1;
                int adapterIndex = -1;
                long bestVideoMemory = 0;
                long bestSystemMemory = 0;

                foreach (var item in f.Adapters)
                {
                    adapterIndex++;

                    // not skip the render only WARP device
                    if (item.Description.VendorId != 0x1414 || item.Description.DeviceId != 0x8c)
                    {
                        // Windows 10 fix
                        if (item.Outputs == null || item.Outputs.Length == 0)
                        {
                            continue;
                        }
                    }

                    var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(item);

                    if (level < DefaultEffectsManager.MinimumFeatureLevel)
                    {
                        continue;
                    }

                    long videoMemory = item.Description.DedicatedVideoMemory;
                    long systemMemory = item.Description.DedicatedSystemMemory;

                    if ((bestAdapter == null) || (videoMemory > bestVideoMemory) || ((videoMemory == bestVideoMemory) && (systemMemory > bestSystemMemory)))
                    {
                        bestAdapter = item;
                        bestAdapterIndex = adapterIndex;
                        bestVideoMemory = videoMemory;
                        bestSystemMemory = systemMemory;
                    }
                }

                return bestAdapter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public global::SharpDX.Direct3D11.Device Device { get { return device; } }

        /// <summary>
        /// Gets the index of the adapter in use.
        /// </summary>
        public int AdapterIndex { get { return adapterIndex; } }

        /// <summary>
        /// Gets the device's driver type.
        /// </summary>
        public DriverType DriverType { get { return driverType; } }

        /// <summary>
        /// 
        /// </summary>
        protected global::SharpDX.Direct3D11.Device device;

        /// <summary>
        /// The index of the adapter in use.
        /// </summary>
        private int adapterIndex;

        /// <summary>
        /// The driver type.
        /// </summary>
        private DriverType driverType;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        ~DefaultEffectsManager()
        {
            Dispose();
        }

        #region protected methods

        /// <summary>
        /// Initialize all effects.
        /// 
        /// Override in a derived class to control how effects are initialized.
        /// </summary>
        protected virtual void InitEffects()
        {
            InputLayout defaultInputLayout;
            InputLayout cubeMapInputLayout;
            RegisterDefaultLayoutsAndEffects(Properties.Resources.Tessellation, out defaultInputLayout, out cubeMapInputLayout);
        }

        /// <summary>
        /// Register an effect for a set of RenderTechniques.
        /// </summary>
        /// <param name="shaderEffectBytecode">A byte array representing the compiled shader.</param>
        /// <param name="techniques">A set of RenderTechnique objects for which to associate the Effect.</param>
        /// <param name="eFlags"></param>
        protected void RegisterEffect(byte[] shaderEffectBytecode, RenderTechnique[] techniques, EffectFlags eFlags = EffectFlags.None)
        {
            var effect = new Effect(device, shaderEffectBytecode, eFlags);
            foreach (var tech in techniques)
                data[tech.Name] = effect;
        }

        #endregion

        #region private methods

        protected void RegisterDefaultLayoutsAndEffects(string shaderEffectString,
            out InputLayout defaultInputLayout,
            out InputLayout cubeMapInputLayout)
        {
            try
            {
                RegisterEffect(shaderEffectString,
                new[]
                { 
                    // put here the techniques which you want to use with this effect
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Phong],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.CubeMap],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Diffuse],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Positions],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Normals],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.PerturbedNormals],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Tangents],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.TexCoords],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Wires],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Lines],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.InstancingBlinn],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BoneSkinBlinn],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardInstancing],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.ParticleStorm]
                });

                var phong = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Phong];
                defaultInputLayout = new InputLayout(device, GetEffect(phong).GetTechniqueByName(DefaultRenderTechniqueNames.Phong).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
           
                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                });

                var instancingblinn = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.InstancingBlinn];
                var instancingInputLayout = new InputLayout(device, GetEffect(instancingblinn).GetTechniqueByName(DefaultRenderTechniqueNames.InstancingBlinn).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),  
           
                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("COLOR", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("COLOR", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 5, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                });

                var boneSkinBlinn = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BoneSkinBlinn];
                var boneSkinInputLayout = new InputLayout(device, GetEffect(instancingblinn).GetTechniqueByName(DefaultRenderTechniqueNames.BoneSkinBlinn).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("BONEIDS", 0, Format.R32G32B32A32_SInt, InputElement.AppendAligned, 1),
                    new InputElement("BONEWEIGHTS", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1),
                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                });

                var lines = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Lines];
                var linesInputLayout = new InputLayout(device, GetEffect(lines).GetTechniqueByName(DefaultRenderTechniqueNames.Lines).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),

                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                });

                var cubeMap = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.CubeMap];
                cubeMapInputLayout = new InputLayout(device, GetEffect(cubeMap).GetTechniqueByName(DefaultRenderTechniqueNames.CubeMap).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                });

                var points = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
                var pointsInputLayout = new InputLayout(device, GetEffect(points).GetTechniqueByName(DefaultRenderTechniqueNames.Points).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0)
                });

                var text = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText];
                var billboardInputLayout = new InputLayout(device, GetEffect(text).GetTechniqueByName(DefaultRenderTechniqueNames.BillboardText).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                });

                var billboardinstancing = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardInstancing];
                var billboardInstancingInputLayout = new InputLayout(device, GetEffect(billboardinstancing)
                    .GetTechniqueByName(DefaultRenderTechniqueNames.BillboardInstancing).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
           
                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("COLOR", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 5, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 6, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                });

                //var particle = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.ParticleStorm];
                //var particleLayout = new InputLayout(device, GetEffect(particle).GetTechniqueByName(DefaultRenderTechniqueNames.ParticleStorm)
                //    .GetPassByIndex(2).Description.Signature,
                //    null);

                RegisterLayout(new[] { cubeMap }, cubeMapInputLayout);

                RegisterLayout(new[]
                {
                    lines
                },
                linesInputLayout);

                RegisterLayout(new[]
                {
                    points
                },
                pointsInputLayout);

                RegisterLayout(new[]
                {
                    text
                },
                billboardInputLayout);

                RegisterLayout(new[]
                { 
                    // put here techniques which use the vertex layout below
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Phong],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Diffuse],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Positions],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Normals],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.PerturbedNormals],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Tangents],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.TexCoords],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors],
                    renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Wires],
                }, defaultInputLayout);

                RegisterLayout(new[] { instancingblinn }, instancingInputLayout);

                RegisterLayout(new[] { boneSkinBlinn }, boneSkinInputLayout);

                RegisterLayout(new[] { billboardinstancing }, billboardInstancingInputLayout);

             //   RegisterLayout(new[] { particle }, particleLayout);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error registering effect: {0}", ex.Message), "Error");
                throw;
            }
        }

        /// <summary>
        /// Register an effect for a set of RenderTechniques.
        /// </summary>
        /// <param name="shaderEffectString">A string representing the shader code.</param>
        /// <param name="techniques">A set of RenderTechnique objects for which to associate the Effect.</param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        protected void RegisterEffect(string shaderEffectString, RenderTechnique[] techniques, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
#if PRECOMPILED_SHADERS

            try
            {
                var shaderBytes = ((DefaultRenderTechniquesManager)renderTechniquesManager).TechniquesSourceDict[techniques[0]];
                RegisterEffect(shaderBytes, techniques);
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(string.Format("Error registering effect: {0}", ex.Message), "Error");
                Debug.WriteLine(string.Format("Error registering effect: {0}", ex.Message), "Error");
                throw;
            }
#else

#if DEBUG
            sFlags |= ShaderFlags.Debug;
            eFlags |= EffectFlags.None;
#else
            sFlags |= ShaderFlags.OptimizationLevel3;
            eFlags |= EffectFlags.None;       
#endif

            var preposessMacros = new List<ShaderMacro>();

#if DEFERRED
#if DEFERRED_MSAA
            preposessMacros.Add(new ShaderMacro("DEFERRED_MSAA", true));
#endif

#if SSAO
            preposessMacros.Add(new ShaderMacro("SSAO", true));
#endif
#endif
            var preprocess = ShaderBytecode.Preprocess(shaderEffectString, preposessMacros.ToArray(), new IncludeHandler());
            var hashCode = preprocess.GetHashCode();
            if (!File.Exists(hashCode.ToString()))
            {
                try
                {
                    var shaderBytes = ShaderBytecode.Compile(preprocess, "fx_5_0", sFlags, eFlags);
                    shaderBytes.Bytecode.Save(hashCode.ToString());
                    this.RegisterEffect(shaderBytes.Bytecode, techniques);
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show(string.Format("Error compiling effect: {0}", ex.Message), "Error");
                    Debug.WriteLine(string.Format("Error compiling effect: {0}", ex.Message), "Error");
                    throw;
                }
            }
            else
            {
                var shaderBytes = ShaderBytecode.FromFile(hashCode.ToString());
                this.RegisterEffect(shaderBytes, techniques);
            }
#endif
        }

        /// <summary>
        /// Register an effect for a RenderTechnique.
        /// 
        /// Override in a derived class to control how effects are registered.
        /// </summary>
        /// <param name="shaderEffectString">A string representing the shader code.</param>
        /// <param name="technique"></param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        protected void RegisterEffect(string shaderEffectString, RenderTechnique technique, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
            RegisterEffect(shaderEffectString, new[] { technique }, sFlags, eFlags);
        }
        
        /// <summary>
        /// Register an InputLayout for a RenderTechnique.
        /// </summary>
        /// <param name="technique">A RenderTechnique object.</param>
        /// <param name="layout">An InputLayout object.</param>
        private void RegisterLayout(RenderTechnique technique, InputLayout layout)
        {
            data[technique.Name + "Layout"] = layout;
        }

        /// <summary>
        /// Register an InputLayout for a set of RenderTechniques
        /// </summary>
        /// <param name="techniques">An array of RenderTechnique objects.</param>
        /// <param name="layout">An InputLayout object.</param>
        protected void RegisterLayout(RenderTechnique[] techniques, InputLayout layout)
        {
            foreach (var tech in techniques)
                data[tech.Name + "Layout"] = layout;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Get the Effect associated with a RenderTechnique.
        /// </summary>
        /// <param name="technique">A RenderTechnique object.</param>
        /// <returns></returns>
        public Effect GetEffect(RenderTechnique technique)
        {
            return (Effect)data[technique.Name];
        }

        /// <summary>
        /// Get the InputLayout associated with a RenderTechnique
        /// </summary>
        /// <param name="technique">A RenderTechnique object.</param>
        /// <returns></returns>
        public InputLayout GetLayout(RenderTechnique technique)
        {
            return (InputLayout)data[technique.Name + "Layout"];
        }

        public void Dispose()
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    var o = item.Value as IDisposable;
                    Disposer.RemoveAndDispose(ref o);
                }
            }
            data = null;

            Disposer.RemoveAndDispose(ref device);
            device = null;
        }

        #endregion

        private class IncludeHandler : Include, ICallbackable, IDisposable
        {
            public void Close(Stream stream)
            {
                stream.Close();
            }

            public Stream Open(IncludeType type, string fileName, Stream parentStream)
            {
                var codeString = Properties.Resources.ResourceManager.GetString(Path.GetFileNameWithoutExtension(fileName), System.Globalization.CultureInfo.InvariantCulture);

                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(codeString);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }

            public IDisposable Shadow
            {
                get
                {
                    return stream;
                }
                set
                {
                    if (stream != null)
                        stream.Dispose();
                    stream = value as Stream;
                }
            }

            public void Dispose()
            {
                stream.Dispose();
            }

            private Stream stream;
        }
    }

    public class DeferredEffectsManager : DefaultEffectsManager
    {
        public DeferredEffectsManager(IRenderTechniquesManager renderTechniquesManager) : base(renderTechniquesManager) { }

        protected override void InitEffects()
        {
            InputLayout defaultInputLayout;
            InputLayout cubeMapInputLayout;
            RegisterDefaultLayoutsAndEffects(Properties.Resources.Tessellation, out defaultInputLayout, out cubeMapInputLayout);
            RegisterDeferredLayoutsAndEffects(Properties.Resources.Tessellation, defaultInputLayout, cubeMapInputLayout);
        }

        private void RegisterDeferredLayoutsAndEffects(string shaderEffectString, InputLayout defaultInputLayout, InputLayout cubeMapInputLayout)
        {
            RegisterEffect(shaderEffectString,
            new[]
            {
                renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.Deferred],
                renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.GBuffer],
                renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.DeferredLighting],
            });

            RegisterLayout(new[]
            {
                renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.Deferred],
                renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.GBuffer],
            }, defaultInputLayout);

            var deferredLighting = renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.DeferredLighting];
            RegisterLayout(new[] { deferredLighting }, cubeMapInputLayout);
        }
    }

    public class TessellationEffectsManager : DefaultEffectsManager
    {
        public TessellationEffectsManager(IRenderTechniquesManager renderTechniquesManager) : base(renderTechniquesManager) { }

        protected override void InitEffects()
        {
            InputLayout defaultInputLayout;
            InputLayout cubeMapInputLayout;
            RegisterDefaultLayoutsAndEffects(Properties.Resources.Tessellation, out defaultInputLayout, out cubeMapInputLayout);
            RegisterTessellationLayoutsAndEffects(Properties.Resources.Tessellation);
        }

        private void RegisterTessellationLayoutsAndEffects(string shaderEffectString)
        {
            RegisterEffect(shaderEffectString,
            new[]
            {
                renderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles],
                renderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads],
            });

            var tesselation = renderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles];
            var tessellationInputLayout = new InputLayout(device, GetEffect(tesselation).GetTechniqueByName(TessellationRenderTechniqueNames.PNTriangles).GetPassByIndex(0).Description.Signature, new[]
            {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                    new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                    new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                });

            RegisterLayout(new[]
            {
                renderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles],
                renderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads]
            }, tessellationInputLayout);
        }
    }
}

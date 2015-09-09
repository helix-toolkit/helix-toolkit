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

    public sealed class EffectsManager : IDisposable
    {
        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        private const FeatureLevel MinimumFeatureLevel = FeatureLevel.Level_10_0;

        /// <summary>
        /// Stores the singleton instance.
        /// </summary>
        private static EffectsManager instance;

        /// <summary>
        /// 
        /// </summary>
        private EffectsManager()
        {
#if DX11
            var adapter = GetBestAdapter();

            if (adapter != null)
            {
                if (adapter.Description.VendorId == 0x1414 && adapter.Description.DeviceId == 0x8c)
                {
                    this.driverType = DriverType.Warp;
                    this.device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
                }
                else
                {
                    this.driverType = DriverType.Hardware;
                    this.device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
                }
            }
#else
            this.device = new Direct3D11.Device(Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, Direct3D.FeatureLevel.Level_10_1);                        
#endif
            this.InitEffects();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Adapter GetBestAdapter()
        {
            using (var f = new Factory())
            {
                Adapter bestAdapter = null;
                long bestVideoMemory = 0;
                long bestSystemMemory = 0;

                foreach (var item in f.Adapters)
                {
                    var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(item);

                    if (level < EffectsManager.MinimumFeatureLevel)
                    {
                        continue;
                    }

                    long videoMemory = item.Description.DedicatedVideoMemory;
                    long systemMemory = item.Description.DedicatedSystemMemory;

                    if ((bestAdapter == null) || (videoMemory > bestVideoMemory) || ((videoMemory == bestVideoMemory) && (systemMemory > bestSystemMemory)))
                    {
                        bestAdapter = item;
                        bestVideoMemory = videoMemory;
                        bestSystemMemory = systemMemory;
                    }
                }

                return bestAdapter;
            }
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static EffectsManager Instance { get { return instance ?? (instance = new EffectsManager()); } }

        /// <summary>
        /// 
        /// </summary>
        public static global::SharpDX.Direct3D11.Device Device { get { return Instance.device; } }

        /// <summary>
        /// Gets the device's driver type.
        /// </summary>
        public static DriverType DriverType { get { return Instance.driverType; } }

        /// <summary>
        /// 
        /// </summary>
        private global::SharpDX.Direct3D11.Device device;

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
        ~EffectsManager()
        {
            this.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        private void InitEffects()
        {
            try
            {

                // ------------------------------------------------------------------------------------
#if TESSELLATION
                RegisterEffect(Properties.Resources.Tessellation,
#else                
                RegisterEffect(Properties.Resources.Default,                                    
#endif
 new[] 
                { 
                    // put here the techniques which you want to use with this effect
                    Techniques.RenderPhong, 
                    Techniques.RenderBlinn, 
                    Techniques.RenderCubeMap, 
                    Techniques.RenderColors, 
                    Techniques.RenderDiffuse,
                    Techniques.RenderPositions,
                    Techniques.RenderNormals,
                    Techniques.RenderPerturbedNormals,
                    Techniques.RenderTangents, 
                    Techniques.RenderTexCoords, 
                    Techniques.RenderWires, 
                    Techniques.RenderLines,
                    Techniques.RenderPoints,
                    Techniques.RenderBillboard,
#if TESSELLATION
                    Techniques.RenderPNTriangs,
                    Techniques.RenderPNQuads,
#endif
                    
                });

#if DEFERRED  
                // ------------------------------------------------------------------------------------                
                RegisterEffect(Properties.Resources.Deferred, //Properties.Resources.Deferred,
                new[] 
                { 
                    Techniques.RenderDeferred, 
                    Techniques.RenderGBuffer,
                    Techniques.RenderDeferredLighting 
                });
#endif

                // ##############################################################################################

                // ------------------------------------------------------------------------------------
                var defaultInputLayout = new InputLayout(device, GetEffect(Techniques.RenderPhong).GetTechniqueByName(Techniques.RenderPhong.Name).GetPassByIndex(0).Description.Signature, new[]
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

                // ------------------------------------------------------------------------------------
                var linesInputLayout = new InputLayout(device, GetEffect(Techniques.RenderLines).GetTechniqueByName(Techniques.RenderLines.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),

                    //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                    new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),                 
                    new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                });

                // ------------------------------------------------------------------------------------
                var cubeMapInputLayout = new InputLayout(device, GetEffect(Techniques.RenderCubeMap).GetTechniqueByName(Techniques.RenderCubeMap.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),                                                             
                });

                var pointsInputLayout = new InputLayout(device, GetEffect(Techniques.RenderPoints).GetTechniqueByName(Techniques.RenderPoints.Name).GetPassByIndex(0).Description.Signature, new[] 
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0)
                });

                var billboardInputLayout = new InputLayout(device, GetEffect(Techniques.RenderBillboard).GetTechniqueByName(Techniques.RenderBillboard.Name).GetPassByIndex(0).Description.Signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                    new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                });

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    Techniques.RenderCubeMap, 
#if DEFERRED 
                    Techniques.RenderDeferredLighting
#endif
                }, cubeMapInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    Techniques.RenderLines 
                },
                linesInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    Techniques.RenderPoints 
                },
                pointsInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new []
                {
                    Techniques.RenderBillboard
                },
                billboardInputLayout);

                // ------------------------------------------------------------------------------------
                RegisterLayout(new[] 
                { 
                    // put here techniques which use the vertex layout below
                    Techniques.RenderPhong, 
                    Techniques.RenderBlinn,
                                        
                    Techniques.RenderDiffuse,
                    Techniques.RenderPositions,
                    Techniques.RenderNormals,
                    Techniques.RenderPerturbedNormals,
                    Techniques.RenderTangents, 
                    Techniques.RenderTexCoords, 
                    Techniques.RenderColors, 
                    Techniques.RenderWires, 
#if DEFERRED 
                    Techniques.RenderDeferred,
                    Techniques.RenderGBuffer,
#endif
                }, defaultInputLayout);

#if TESSELLATION
                // ------------------------------------------------------------------------------------
                var tessellationInputLayout = new InputLayout(device, GetEffect(Techniques.RenderPNTriangs).GetTechniqueByName(Techniques.RenderPNTriangs.Name).GetPassByIndex(0).Description.Signature, new[] 
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
                    // put here techniques which use the vertex layout below
                    Techniques.RenderPNTriangs,
                    Techniques.RenderPNQuads, 
                }, tessellationInputLayout);
#endif
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(string.Format("Error registering effect: {0}", ex.Message), "Error");
                Debug.WriteLine(string.Format("Error registering effect: {0}", ex.Message), "Error");
                throw;
            }

        }

        /// <summary>
        /// Register an effect for a RenderTechnique.
        /// </summary>
        /// <param name="shaderEffectString">A string representing the shader code.</param>
        /// <param name="techniqueName"></param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        public void RegisterEffect(string shaderEffectString, RenderTechnique technique, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
            RegisterEffect(shaderEffectString, new[] { technique }, sFlags, eFlags);
        }

        /// <summary>
        /// Register an effect for a set of RenderTechniques.
        /// </summary>
        /// <param name="shaderEffectString">A string representing the shader code.</param>
        /// <param name="techniques">A set of RenderTechnique objects for which to associate the Effect.</param>
        /// <param name="sFlags"></param>
        /// <param name="eFlags"></param>
        public void RegisterEffect(string shaderEffectString, RenderTechnique[] techniques, ShaderFlags sFlags = ShaderFlags.None, EffectFlags eFlags = EffectFlags.None)
        {
#if PRECOMPILED_SHADERS

            try
            {
                var shaderBytes = Techniques.TechniquesSourceDict[techniques[0]];
                this.RegisterEffect(shaderBytes, techniques);
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
        /// Register an effect for a set of RenderTechniques.
        /// </summary>
        /// <param name="shaderEffectBytecode">A byte array representing the compiled shader.</param>
        /// <param name="techniques">A set of RenderTechnique objects for which to associate the Effect.</param>
        /// <param name="eFlags"></param>
        public void RegisterEffect(byte[] shaderEffectBytecode, RenderTechnique[] techniques, EffectFlags eFlags = EffectFlags.None)
        {
            var effect = new Effect(device, shaderEffectBytecode, eFlags);
            foreach (var tech in techniques)
                data[tech.Name] = effect;
        }

        /// <summary>
        /// Register an InputLayout for a RenderTechnique.
        /// </summary>
        /// <param name="technique">A RenderTechnique object.</param>
        /// <param name="layout">An InputLayout object.</param>
        public void RegisterLayout(RenderTechnique technique, InputLayout layout)
        {
            data[technique.Name + "Layout"] = layout;
        }

        /// <summary>
        /// Register an InputLayout for a set of RenderTechniques
        /// </summary>
        /// <param name="techniques">An array of RenderTechnique objects.</param>
        /// <param name="layout">An InputLayout object.</param>
        public void RegisterLayout(RenderTechnique[] techniques, InputLayout layout)
        {
            foreach (var tech in techniques)
                data[tech.Name + "Layout"] = layout;
        }

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

        /// <summary>
        /// 
        /// </summary>
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
            this.data = null;

            Disposer.RemoveAndDispose(ref this.device);
            this.device = null;
        }

        /// <summary>
        /// 
        /// </summary>
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
                    return this.stream;
                }
                set
                {
                    if (this.stream != null)
                        this.stream.Dispose();
                    this.stream = value as Stream;
                }
            }

            public void Dispose()
            {
                stream.Dispose();
            }

            private Stream stream;
        }
    }
}

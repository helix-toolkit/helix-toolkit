/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
#endif

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using ShaderManager;
    public class Technique :  DisposeObject, IRenderTechnique
    {
        private readonly Dictionary<string, Lazy<ShaderPass>> passDict = new Dictionary<string, Lazy<ShaderPass>>();
        private readonly List<Lazy<ShaderPass>> passList = new List<Lazy<ShaderPass>>();

        /// <summary>
        /// <see cref="IRenderTechnique.Layout"/>
        /// </summary>
        public InputLayout Layout { private set; get; }
        /// <summary>
        /// <see cref="IRenderTechnique.Device"/>
        /// </summary>
        public Device Device { get { return EffectsManager.Device; } }
        /// <summary>
        /// <see cref="IRenderTechnique.Name"/>
        /// </summary>
        public string Name { private set; get; }

        /// <summary>
        /// <see cref="IRenderTechnique.ShaderPassNames"/>
        /// </summary>
        public IEnumerable<string> ShaderPassNames { get { return passDict.Keys; } }
        /// <summary>
        /// <see cref="IRenderTechnique.ConstantBufferPool"/>
        /// </summary>
        public IConstantBufferPool ConstantBufferPool { get { return EffectsManager.ConstantBufferPool; } }
        /// <summary>
        /// <see cref="IRenderTechnique.EffectsManager"/>
        /// </summary>
        public IEffectsManager EffectsManager { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="device"></param>
        /// <param name="manager"></param>
        public Technique(TechniqueDescription description, Device device, IEffectsManager manager)
        {
            Name = description.Name;
            EffectsManager = manager;
            Layout = manager.ShaderManager.RegisterInputLayout(description.InputLayoutDescription);
            if (description.PassDescriptions != null)
            {
                foreach(var desc in description.PassDescriptions)
                {
                    var pass = new Lazy<ShaderPass>(()=> { return Collect(new ShaderPass(desc, manager)); }, true);
                    passDict.Add(desc.Name, pass);
                    passList.Add(pass);
                }
            }
        }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShaderPass GetPass(string name)
        {
            return passDict.ContainsKey(name) ? passDict[name].Value : ShaderPass.NullPass;
        }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ShaderPass GetPass(int index)
        {
            return passList.Count > index ? passList[index].Value : ShaderPass.NullPass;
        }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ShaderPass this[int index] { get { return GetPass(index); } }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShaderPass this[string name] { get { return GetPass(name); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            passDict.Clear();
            passList.Clear();
            EffectsManager = null;
            Layout = null;
            base.OnDispose(disposeManagedResources);
        }
    }
}

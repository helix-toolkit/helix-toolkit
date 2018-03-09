/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
        private readonly Dictionary<string, Lazy<IShaderPass>> passDict = new Dictionary<string, Lazy<IShaderPass>>();
        private readonly List<Lazy<IShaderPass>> passList = new List<Lazy<IShaderPass>>();

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
                    var pass = new Lazy<IShaderPass>(()=> { return Collect(new ShaderPass(desc, manager)); }, true);
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
        public IShaderPass GetPass(string name)
        {
            return passDict.ContainsKey(name) ? passDict[name].Value : NullShaderPass.NullPass;
        }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IShaderPass GetPass(int index)
        {
            return passList.Count > index ? passList[index].Value : NullShaderPass.NullPass;
        }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IShaderPass this[int index] { get { return GetPass(index); } }

        /// <summary>
        /// <see cref="IRenderTechnique.GetPass(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IShaderPass this[string name] { get { return GetPass(name); } }

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

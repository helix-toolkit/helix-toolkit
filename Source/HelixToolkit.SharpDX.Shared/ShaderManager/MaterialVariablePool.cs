using System;
using System.Collections.Generic;
using System.Linq;
using HelixToolkit.Wpf.SharpDX.Model;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public class MaterialVariablePool : DisposeObject, IMaterialVariablePool
    {
        private readonly Dictionary<Guid, MaterialVariable> dictionary = new Dictionary<Guid, MaterialVariable>();
        private readonly IEffectsManager effectsManager;

        public MaterialVariablePool(IEffectsManager manager)
        {
            effectsManager = manager;
        }

        public MaterialVariable Register(IMaterial material)
        {
            if (material == null)
            {
                return EmptyMaterialVariable.EmptyVariable;
            }
            var guid = material.Guid;
            lock (dictionary)
            {
                if(dictionary.TryGetValue(guid, out MaterialVariable value))
                {
                    value.IncRef();
                    return value;
                }
                else
                {
                    var v = material.CreateMaterialVariables(effectsManager);                  
                    v.Disposed += (s, e) => 
                    {
                        lock (dictionary)
                        {
                            dictionary.Remove(guid);
                        }
                    };
                    dictionary.Add(guid, v);
                    return v;
                }
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            base.OnDispose(disposeManagedResources);
            if (disposeManagedResources)
            {
                lock (dictionary)
                {
                    foreach (var v in dictionary.Values.ToArray())
                    {
                        v.ForceDispose();
                    }
                    dictionary.Clear();
                }
            }
        }
    }
}

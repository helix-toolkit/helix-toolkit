using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Model;
    public class MaterialVariablePool : DisposeObject, IMaterialVariablePool
    {
        private readonly DoubleKeyDictionary<Guid, Guid, MaterialVariable> dictionary = new DoubleKeyDictionary<Guid, Guid, MaterialVariable>();
        private readonly IEffectsManager effectsManager;

        public MaterialVariablePool(IEffectsManager manager)
        {
            effectsManager = manager;
        }

        public MaterialVariable Register(IMaterial material, IRenderTechnique technique)
        {
            if (material == null || technique.IsNull)
            {
                return EmptyMaterialVariable.EmptyVariable;
            }
            var guid = material.Guid;
            var techGuid = technique.GUID;
            lock (dictionary)
            {
                if(dictionary.TryGetValue(guid, techGuid, out MaterialVariable value))
                {
                    value.IncRef();
                    return value;
                }
                else
                {
                    var v = material.CreateMaterialVariables(effectsManager, technique);                  
                    v.Disposed += (s, e) => 
                    {
                        lock (dictionary)
                        {
                            dictionary.Remove(guid, techGuid);
                        }
                    };
                    dictionary.Add(guid, techGuid, v);
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

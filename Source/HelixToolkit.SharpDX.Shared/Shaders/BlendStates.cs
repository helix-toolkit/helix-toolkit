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
    public class BlendStates : DisposeObject
    {
        private readonly Dictionary<string, BlendState> dictionary = new Dictionary<string, BlendState>();
        public readonly Device Device;
        public BlendStates(Device device)
        {
            Device = device;
        }

        public void Add(string name, BlendStateDescription desc)
        {
            if (dictionary.ContainsKey(name))
            {
                throw new ArgumentException("Same blendstate name already exists.");
            }
            dictionary.Add(name, Collect(new BlendState(Device, desc)));
        }

        public void Remove(string name)
        {
            if (dictionary.ContainsKey(name))
            {
                var state = dictionary[name];
                RemoveAndDispose(ref state);
                dictionary.Remove(name);
            }
        }       
    }
}

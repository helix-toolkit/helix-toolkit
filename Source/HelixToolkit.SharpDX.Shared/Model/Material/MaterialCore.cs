/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Utilities;
namespace HelixToolkit.UWP.Model
#endif
{
    public abstract class MaterialCore : ObservableObject, IMaterial
    {
        private string name;
        public string Name
        {
            set
            {
                Set(ref name, value);
            }
            get
            {
                return name;
            }
        }

        public Guid Guid { get; } = Guid.NewGuid();

        public abstract MaterialVariable CreateMaterialVariables(IEffectsManager manager);
    }
}

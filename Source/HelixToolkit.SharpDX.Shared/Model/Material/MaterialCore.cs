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

        public abstract IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager);
    }
}

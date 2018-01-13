/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Utilities;
namespace HelixToolkit.UWP.Model
#endif
{
    public class MaterialCore : ObservableObject, IMaterial
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
    }
}

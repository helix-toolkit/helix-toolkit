/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if NETFX_CORE
namespace HelixToolkit.UWP
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Elements2D;
#if !COREWPF
    namespace Model.Scene2D
    {
        public partial class SceneNode2D
        {
            public static implicit operator Element2D(SceneNode2D s)
            {
                return s.WrapperSource as Element2D;
            }
        }
    }
#endif
}

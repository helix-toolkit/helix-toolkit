/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    using Elements2D;
    public partial class SceneNode2D
    {
        public static implicit operator Element2D(SceneNode2D s)
        {
            return s.WrapperSource as Element2D;
        }
    }
}

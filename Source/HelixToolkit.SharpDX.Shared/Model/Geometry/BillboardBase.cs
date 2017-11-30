using System.Collections.Generic;
using SharpDX;
using System.IO;
#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif


#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public abstract class BillboardBase : MeshGeometry3D, IBillboardText
    {
        public float Height
        {
            protected set;
            get;
        }

        public abstract BillboardType Type
        {
            get;
        }

        public abstract IList<Vector2> TextureOffsets
        {
            get;
        }

        public virtual BitmapSource Texture
        {
            protected set;
            get;
        }

        public virtual Stream AlphaTexture
        {
            protected set;
            get;
        }

        public float Width
        {
            protected set;
            get;
        }

        public BillboardBase()
        {
        }

        public virtual void DrawTexture()
        {
            UpdateBounds();
        }
    }
}

using System.Collections.Generic;
using SharpDX;
using System.IO;
#if NETFX_CORE

#else
using System.Windows.Media.Imaging;
#endif


#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
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

        public virtual Stream Texture
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

        public IList<BillboardVertex> BillboardVertices { set; get; } = new List<BillboardVertex>();

        public BillboardBase()
        {
        }

        public virtual void DrawTexture()
        {
            UpdateBounds();
        }
    }
}

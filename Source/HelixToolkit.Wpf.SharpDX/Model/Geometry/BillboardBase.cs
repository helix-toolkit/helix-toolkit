using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System.IO;

namespace HelixToolkit.Wpf.SharpDX
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Utilities;
    public class SceneGraphManager
    {
        private IList<IRenderable> RootRenderables = new List<IRenderable>();

        public IEnumerable<IRenderable> FlattenedRenderables
        {
            get
            {
                return RootRenderables.PreorderDFT();
            }
        }

        public bool IsAttached { get { return renderHost != null; } }

        private IRenderHost renderHost;


        public void Attach(IRenderHost host)
        {
            renderHost = host;
            foreach(var item in RootRenderables)
            {
                item.Attach(host);
            }
        }

        public void Detach()
        {
            renderHost = null;
            foreach(var item in RootRenderables)
            {
                item.Detach();
            }
        }
    }

}

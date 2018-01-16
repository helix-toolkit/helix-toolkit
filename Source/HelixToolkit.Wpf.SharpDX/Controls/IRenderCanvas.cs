using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderCanvas
    {
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        IRenderHost RenderHost { get; }
    }
}

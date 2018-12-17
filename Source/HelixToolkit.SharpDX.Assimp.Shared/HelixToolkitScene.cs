using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Model.Scene;
    using Animations;

    namespace Assimp
    {
        /// <summary>
        /// Scene for importer output
        /// </summary>
        public class HelixToolkitScene
        {
            public SceneNode Root { set; get; }
            public IList<Animation> Animations { set; get; }
            public bool HasAnimation { get => Animations != null && Animations.Count > 0; }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixScene"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public HelixToolkitScene(SceneNode root)
            {
                Root = root;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixScene"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="animations">The animations.</param>
            public HelixToolkitScene(SceneNode root, IList<Animation> animations = null)
            {
                Root = root;
                Animations = animations.ToArray();
            }
        }
    }

}

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
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
            /// <summary>
            /// Gets or sets the root.
            /// </summary>
            /// <value>
            /// The root.
            /// </value>
            public SceneNode Root { set; get; }
            /// <summary>
            /// Gets or sets the animations.
            /// </summary>
            /// <value>
            /// The animations.
            /// </value>
            public IList<Animation> Animations { set; get; }
            /// <summary>
            /// Gets a value indicating whether this instance has animation.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance has animation; otherwise, <c>false</c>.
            /// </value>
            public bool HasAnimation { get => Animations != null && Animations.Count > 0; }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixToolkitScene"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public HelixToolkitScene(SceneNode root)
            {
                Root = root;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixToolkitScene"/> class.
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

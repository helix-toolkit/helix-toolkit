/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;

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
    namespace Model.Scene
    {
        using Core;

        public class MeshOutlineNode : MeshNode
        {
            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether [enable outline].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable outline]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableOutline
            {
                set
                {
                    (RenderCore as IMeshOutlineParams).OutlineEnabled = value;
                }
                get
                {
                    return (RenderCore as IMeshOutlineParams).OutlineEnabled;
                }
            }
            /// <summary>
            /// Gets or sets the color of the outline.
            /// </summary>
            /// <value>
            /// The color of the outline.
            /// </value>
            public Color4 OutlineColor
            {
                set
                {
                    (RenderCore as IMeshOutlineParams).Color = value;
                }
                get
                {
                    return (RenderCore as IMeshOutlineParams).Color;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is draw geometry.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is draw geometry; otherwise, <c>false</c>.
            /// </value>
            public bool IsDrawGeometry
            {
                set
                {
                    (RenderCore as IMeshOutlineParams).DrawMesh = value;
                }
                get
                {
                    return (RenderCore as IMeshOutlineParams).DrawMesh;
                }
            }
            /// <summary>
            /// Gets or sets the outline fading factor.
            /// </summary>
            /// <value>
            /// The outline fading factor.
            /// </value>
            public float OutlineFadingFactor
            {
                set
                {
                    (RenderCore as IMeshOutlineParams).OutlineFadingFactor = value;
                }
                get
                {
                    return (RenderCore as IMeshOutlineParams).OutlineFadingFactor;
                }
            } 
            #endregion
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new MeshOutlineRenderCore();
            }
        }
    }

}
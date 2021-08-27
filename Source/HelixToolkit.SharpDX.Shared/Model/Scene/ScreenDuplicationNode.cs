/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;
#if !WINDOWS_UWP
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;
        /// <summary>
        /// 
        /// </summary>
        public class ScreenDuplicationNode : SceneNode
        {
        #region Properties
            /// <summary>
            /// Gets or sets the capture rectangle.
            /// </summary>
            /// <value>
            /// The capture rectangle.
            /// </value>
            public Rectangle CaptureRectangle
            {
                set
                {
                    (RenderCore as IScreenClone).CloneRectangle = value;
                }
                get
                {
                    return (RenderCore as IScreenClone).CloneRectangle;
                }
            }
            /// <summary>
            /// Gets or sets the display index.
            /// </summary>
            /// <value>
            /// The display index.
            /// </value>
            public int DisplayIndex
            {
                set
                {
                    (RenderCore as IScreenClone).Output = value;
                }
                get
                {
                    return (RenderCore as IScreenClone).Output;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [stretch to fill].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [stretch to fill]; otherwise, <c>false</c>.
            /// </value>
            public bool StretchToFill
            {
                set
                {
                    (RenderCore as IScreenClone).StretchToFill = value;
                }
                get
                {
                    return (RenderCore as IScreenClone).StretchToFill;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether [show mouse cursor].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
            /// </value>
            public bool ShowMouseCursor
            {
                set
                {
                    (RenderCore as IScreenClone).ShowMouseCursor = value;
                }
                get
                {
                    return (RenderCore as IScreenClone).ShowMouseCursor;
                }
            }
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="ScreenDuplicationNode"/> class.
            /// </summary>
            public ScreenDuplicationNode()
            {
                IsHitTestVisible = false;
            }

            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new ScreenCloneRenderCore();
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}
#endif
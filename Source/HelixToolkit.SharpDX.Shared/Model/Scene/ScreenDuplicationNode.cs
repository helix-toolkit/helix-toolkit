/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

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
#if !NETFX_CORE

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
        /// Initializes a new instance of the <see cref="ScreenDuplicationModel"/> class.
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

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }

#endif
    }

}
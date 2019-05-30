/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class ScreenSpacedNode : GroupNode
    {
        #region Properties
        /// <summary>
        /// Gets or sets the relative screen location x.
        /// </summary>
        /// <value>
        /// The relative screen location x.
        /// </value>
        public float RelativeScreenLocationX
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX;
            }
        }
        /// <summary>
        /// Gets or sets the relative screen location y.
        /// </summary>
        /// <value>
        /// The relative screen location y.
        /// </value>
        public float RelativeScreenLocationY
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY;
            }
        }
        /// <summary>
        /// Gets or sets the size scale.
        /// </summary>
        /// <value>
        /// The size scale.
        /// </value>
        public float SizeScale
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).SizeScale = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).SizeScale;
            }
        }
        #endregion
        /// <summary>
        /// Gets or sets a value indicating whether [need clear depth buffer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need clear depth buffer]; otherwise, <c>false</c>.
        /// </value>
        protected bool NeedClearDepthBuffer { set; get; } = true;

        public ScreenSpacedNode()
        {
            this.OnAddChildNode += ScreenSpacedNode_OnAddChildNode;
        }

        private void ScreenSpacedNode_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
        {
            e.Node.RenderType = RenderType.ScreenSpaced;
        }

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            var core = new ScreenSpacedMeshRenderCore();
            core.OnCoordinateSystemChanged += Core_OnCoordinateSystemChanged;
            return core;
        }

        private void Core_OnCoordinateSystemChanged(object sender, BoolArgs e)
        {
            OnCoordinateSystemChanged(e.Value);
        }

        protected virtual void OnCoordinateSystemChanged(bool e) { }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderHost host)
        {
            RenderCore.Attach(EffectTechnique);
            var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
            screenSpaceCore.RelativeScreenLocationX = RelativeScreenLocationX;
            screenSpaceCore.RelativeScreenLocationY = RelativeScreenLocationY;
            screenSpaceCore.SizeScale = SizeScale;
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].RenderType = RenderType.ScreenSpaced;
            }
            return base.OnAttach(host);
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            RenderCore.Detach();
            base.OnDetach();
        }
    }
}
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;

    /// <summary>
    /// 
    /// </summary>
    public abstract class LightCoreBase : RenderCore, ILight3D
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty { get; } = false;
        /// <summary>
        /// Gets or sets the type of the light.
        /// </summary>
        /// <value>
        /// The type of the light.
        /// </value>
        public LightType LightType { protected set; get; }

        private Color4 color = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            set { SetAffectsRender(ref color, value); }
            get { return color; }
        }

        protected LightCoreBase() : base(RenderType.Light) { }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            return true;
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public sealed override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRender(context.LightScene))
            {
                OnRender(context.LightScene, context.LightScene.LightModels.LightCount);
                switch (LightType)
                {
                    case LightType.Ambient:
                        break;
                    default:
                        context.LightScene.LightModels.IncrementLightCount();
                        break;
                }
            }
        }
        /// <summary>
        /// Determines whether this instance can render the specified light scene.
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified light scene; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender(Light3DSceneShared lightScene)
        {
            return IsAttached && lightScene.LightModels.LightCount < Constants.MaxLights;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <param name="idx">The index.</param>
        protected virtual void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.Lights[idx].LightColor = Color;
            lightScene.LightModels.Lights[idx].LightType = (int)LightType;
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AmbientLightCore : LightCoreBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientLightCore"/> class.
        /// </summary>
        public AmbientLightCore()
        {
            LightType = LightType.Ambient;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="lightScene">The light scene.</param>
        /// <param name="idx">The index.</param>
        protected override void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.AmbientLight = Color;
        }
    }
}

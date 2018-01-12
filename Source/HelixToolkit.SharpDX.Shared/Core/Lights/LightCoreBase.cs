/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    using Render;

    public abstract class LightCoreBase : DisposeObject, ILight3D, IRenderCore
    {
        public Guid GUID { get; } = Guid.NewGuid();

        public LightType LightType { protected set; get; }

        private Color4 color = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
        public Color4 Color
        {
            set { SetAffectsRender(ref color, value); }
            get { return color; }
        }

        public Matrix ModelMatrix { set; get; } = Matrix.Identity;

        public IRenderTechnique EffectTechnique => throw new NotImplementedException();

        public Device Device => throw new NotImplementedException();

        public bool IsAttached { private set; get; } = false;

        public bool IsThrowingShadow { get; set; } = false;

        public event EventHandler<bool> OnInvalidateRenderer;

        public void Render(IRenderContext context, DeviceContextProxy deviceContext)
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

        protected virtual bool CanRender(Light3DSceneShared lightScene)
        {
            return IsAttached && lightScene.LightModels.LightCount < Constants.MaxLights;
        }

        protected virtual void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.Lights[idx].LightColor = Color;
            lightScene.LightModels.Lights[idx].LightType = (int)LightType;
        }

        protected void InvalidateRenderer()
        {
            OnInvalidateRenderer?.Invoke(this, true);
        }

        public void Attach(IRenderTechnique technique)
        {
            IsAttached = true;
        }

        public void Detach()
        {
            IsAttached = false;
        }

        public void ResetInvalidateHandler()
        {
            OnInvalidateRenderer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="affectsRender"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            InvalidateRenderer();
            return true;
        }
    }

    public class AmbientLightCore : LightCoreBase
    {
        public AmbientLightCore()
        {
            LightType = LightType.Ambient;
        }

        protected override void OnRender(Light3DSceneShared lightScene, int idx)
        {
            lightScene.LightModels.AmbientLight = Color;
        }
    }
}

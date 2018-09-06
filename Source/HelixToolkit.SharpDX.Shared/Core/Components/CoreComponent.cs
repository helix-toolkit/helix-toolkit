using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core.Components
#else
namespace HelixToolkit.UWP.Core.Components
#endif
{
    public abstract class CoreComponent : DisposeObject
    {
        public event EventHandler InvalidateRender;
        public bool IsAttached { private set; get; } = false;
        public IRenderTechnique Technique { private set; get; }

        public void Attach(IRenderTechnique technique)
        {
            if (!IsAttached)
            {
                IsAttached = true;
                Technique = technique;
                OnAttach(technique);
            }
        }

        protected abstract void OnAttach(IRenderTechnique technique);

        public void Detach()
        {
            IsAttached = false;
            OnDetach();
            DisposeAndClear();
        }

        protected abstract void OnDetach();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            RaiseInvalidateRender();
            return true;
        }

        public void RaiseInvalidateRender()
        {
            InvalidateRender?.Invoke(this, EventArgs.Empty);
        }
    }
}

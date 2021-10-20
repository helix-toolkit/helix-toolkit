/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    namespace Core.Components
    {
        public abstract class CoreComponent : DisposeObject
        {
            public event EventHandler InvalidateRender;
            public bool IsAttached { private set; get; } = false;
            public IRenderTechnique Technique
            {
                private set; get;
            }

            public void Attach(IRenderTechnique technique)
            {
                if (IsAttached)
                {
                    return;
                }
                IsAttached = true;
                Technique = technique;
                OnAttach(technique);
            }

            protected abstract void OnAttach(IRenderTechnique technique);

            public void Detach()
            {
                if (!IsAttached)
                {
                    return;
                }
                OnDetach();
                IsAttached = false;
            }

            protected abstract void OnDetach();

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="backingField"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected bool SetAffectsRender<T>(ref T backingField, T value)
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                {
                    return false;
                }

                backingField = value;
                RaiseInvalidateRender();
                return true;
            }

            public void RaiseInvalidateRender()
            {
                InvalidateRender?.Invoke(this, EventArgs.Empty);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                Detach();
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}

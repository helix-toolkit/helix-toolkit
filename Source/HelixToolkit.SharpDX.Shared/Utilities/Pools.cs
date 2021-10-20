/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGRESOURCE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
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
    namespace Utilities
    {
        /// <summary>
        /// Base implementation for reference counted dictionary pool.
        /// Object created by the pool will be added back to the pool once being disposed externally.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TArgument"></typeparam>
        public abstract class ReferenceCountedDictionaryPool<TKey, TValue, TArgument> : DisposeObject where TValue : DisposeObject
        {
            private readonly ConcurrentDictionary<TKey, TValue> pool_ = new ConcurrentDictionary<TKey, TValue>();
            private readonly bool autoDispose_ = false;

            public int DictionaryCount => pool_.Count;

            public int Count => pool_.Count;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="autoDispose">Dispose object if no more exteranl references.</param>
            protected ReferenceCountedDictionaryPool(bool autoDispose)
            {
                autoDispose_ = autoDispose;
            }

            /// <summary>
            /// Try to create or get object from the pool. Reference is incremented before returning.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="argument"></param>
            /// <param name="objOut"></param>
            /// <returns>success or failed</returns>
            public bool TryCreateOrGet(TKey key, TArgument argument, out TValue objOut)
            {
                if (IsDisposed)
                {
                    objOut = default;
                    return false;
                }
                if (!CanCreate(ref key, ref argument))
                {
                    objOut = default;
                    return false;
                }
                do
                {
                    if (!pool_.TryGetValue(key, out objOut))
                    {
                        objOut = pool_.GetOrAdd(key, (k) => OnCreate(ref k, ref argument));
                        if (objOut == null)
                        {
                            pool_.TryRemove(key, out objOut);
                            return false;
                        }
                        objOut.AddBackToPool = Item_AddBackToPool;
                        objOut.Disposing += (s, e) =>
                        {
                            pool_.TryRemove(key, out var val);
                        };
                    }
                    if (objOut == null)
                    {
                        return false;
                    }
                } while (objOut.IncRef() <= 1 || objOut.IsDisposed);
                return true;
            }

            /// <summary>
            /// Try to get object by key. Reference will be incremented before returning.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="objOut"></param>
            /// <returns></returns>
            public bool TryGet(TKey key, out TValue objOut)
            {
                objOut = default;
                if (IsDisposed)
                {
#if DEBUG
                    throw new InvalidOperationException("Pool has been disposed.");
#else
                return false;
#endif
                }
                if (!pool_.TryGetValue(key, out objOut))
                {
                    return false;
                }
                return objOut.IncRef() > 1 && !objOut.IsDisposed;
            }
            /// <summary>
            /// Try detach from the pool. The object will be removed from the pool and reference is not incremented before returning.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="objOut"></param>
            /// <returns></returns>
            public bool TryDetach(TKey key, out TValue objOut)
            {
                objOut = default;
                if (IsDisposed)
                {
#if DEBUG
                    throw new InvalidOperationException("Pool has been disposed.");
#else
                return false;
#endif
                }
                if (!pool_.TryRemove(key, out objOut))
                {
                    return false;
                }
                objOut.AddBackToPool = null;
                return !objOut.IsDisposed;
            }

            private void Item_AddBackToPool(DisposeObject e)
            {
                if (autoDispose_)
                {
                    e.AddBackToPool = null;
                    e.Dispose();
                }
            }

            protected IEnumerable<TValue> Items => pool_.Values;

            protected abstract bool CanCreate(ref TKey key, ref TArgument argument);

            protected abstract TValue OnCreate(ref TKey key, ref TArgument argument);

            protected void Clear()
            {
                if (IsDisposed)
                {
                    throw new InvalidOperationException("Pool has been disposed.");
                }
                var items = pool_.Values.ToArray();
                pool_.Clear();
                foreach (var item in items)
                {
                    item.Dispose();
                    if (!item.IsDisposed)
                    {
                        Debug.Assert(false);                    
                    }
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                Clear();
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}

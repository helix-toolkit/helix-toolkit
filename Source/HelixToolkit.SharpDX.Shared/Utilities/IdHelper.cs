/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Concurrent;
using System.Threading;
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
        public sealed class IdHelper
        {
            private int maxId_ = 0;
            private readonly ConcurrentStack<int> freedIds_ = new ConcurrentStack<int>();
            public int GetNextId()
            {
                return freedIds_.TryPop(out var id) ? id : Interlocked.Increment(ref maxId_);
            }

            public int MaxId => Interlocked.CompareExchange(ref maxId_, 0, 0);

            public int Count => MaxId - freedIds_.Count;

            public void ReleaseId(int id)
            {
                freedIds_.Push(id);
            }
        }
    }
}

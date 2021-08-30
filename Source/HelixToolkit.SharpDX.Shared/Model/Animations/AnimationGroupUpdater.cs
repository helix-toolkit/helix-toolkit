/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE && !WINUI_NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Animations
    {
        public class AnimationGroupUpdater : IAnimationUpdater
        {
            public string Name
            {
                set; get;
            } = "";

            private List<IAnimationUpdater> children = new List<IAnimationUpdater>();
            public IList<IAnimationUpdater> Children => children;

            private float speed = 1.0f;
            public float Speed
            {
                get => speed;
                set
                {
                    speed = value;
                    foreach (var updater in Children)
                    {
                        updater.Speed = value;
                    }
                }
            }

            private AnimationRepeatMode repeatMode = AnimationRepeatMode.PlayOnce;
            public AnimationRepeatMode RepeatMode
            {
                get => repeatMode;
                set
                {
                    repeatMode = value;
                    foreach (var updater in Children)
                    {
                        updater.RepeatMode = value;
                    }
                }
            }

            public AnimationGroupUpdater(string name = "")
            {
                Name = name;
            }

            public AnimationGroupUpdater(IEnumerable<IAnimationUpdater> updaters, string name = "") 
                : this(name)
            {
                children.AddRange(updaters);
            }

            public void Reset()
            {
                foreach (var updater in Children)
                {
                    updater.Reset();
                }
            }

            public void Update(long timeStamp, long frequency)
            {
                foreach (var updater in Children)
                {
                    updater.Update(timeStamp, frequency);
                }
            }
        }
    }
}
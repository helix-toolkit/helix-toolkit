/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;

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
    namespace Core2D
    {
        /// <summary>
        /// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
        /// </summary>
        public interface ISegment
        {
            bool IsDirty { get; }
            void Create(D2D.GeometrySink sink);
        }

        /// <summary>
        /// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
        /// </summary>
        public abstract class Segment : ISegment
        {
            public bool IsDirty { private set; get; } = false;

            protected void Invalidate()
            {
                IsDirty = true;
            }

            public abstract void Create(D2D.GeometrySink sink);
        }
    }

}

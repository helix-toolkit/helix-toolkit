/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    using System.Collections.Generic;
    using Extensions;

    public class ExposedArrayList<T> : List<T>
    {
        public ExposedArrayList()
        {
        }

        public ExposedArrayList(int capacity)
            : base(capacity)
        {

        }

        public ExposedArrayList(IEnumerable<T> collection)
            : base(collection)
        {
        }
        /// <summary>
        /// Using with caustious(Array Length >= List.Count).
        /// </summary>
        internal T[] Array
        {
            get
            {
                return this.GetInternalArray();
            }
        }
    }
}

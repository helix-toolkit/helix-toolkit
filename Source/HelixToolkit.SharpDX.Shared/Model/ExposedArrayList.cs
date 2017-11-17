// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposedArrayList.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

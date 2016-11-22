// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposedArrayList.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Core
{
    using System.Collections.Generic;

    using HelixToolkit.Wpf.SharpDX.Extensions;

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

        internal T[] Array
        {
            get
            {
                return this.GetInternalArray();
            }
        }
    }
}

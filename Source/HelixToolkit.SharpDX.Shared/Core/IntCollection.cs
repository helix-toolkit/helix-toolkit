// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;

    using HelixToolkit.Wpf.SharpDX.Utilities;

#if !NETFX_CORE
    [Serializable]
#endif
    [TypeConverter(typeof(IntCollectionConverter))]
    public sealed class IntCollection : ExposedArrayList<int>
    {
        public IntCollection()
        {
        }

        public IntCollection(int capacity)
            : base(capacity)
        {
        }

        public IntCollection(IEnumerable<int> items)
            : base(items)
        {
        }

        public static IntCollection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            var th = new TokenizerHelper(source, formatProvider);
            var resource = new IntCollection();
            while (th.NextToken())
            {
                var value = Convert.ToInt32(th.GetCurrentToken(), formatProvider);
                resource.Add(value);
            }

            return resource;
        }

        public string ConvertToString(string format, IFormatProvider provider)
        {
            if (this.Count == 0)
            {
                return String.Empty;
            }

            var str = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                str.AppendFormat(provider, "{0:" + format + "}", this[i]);
                if (i != this.Count - 1)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }
    }
}

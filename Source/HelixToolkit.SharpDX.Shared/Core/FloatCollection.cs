// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FloatCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
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
    [TypeConverter(typeof(FloatCollectionConverter))]
    public sealed class FloatCollection : ExposedArrayList<float>
    {
        public FloatCollection()
        {
        }

        public FloatCollection(int capacity)
            : base(capacity)
        {
        }

        public FloatCollection(IEnumerable<float> items)
            : base(items)
        {
        }

        public static FloatCollection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;
            var th = new TokenizerHelper(source, formatProvider);
            var resource = new FloatCollection();
            while (th.NextToken())
            {
                var value = Convert.ToSingle(th.GetCurrentToken(), formatProvider);
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

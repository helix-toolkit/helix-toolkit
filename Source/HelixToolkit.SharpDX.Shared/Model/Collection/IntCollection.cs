/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;

    using Utilities;

#if !NETFX_CORE
    [Serializable]
    [TypeConverter(typeof(IntCollectionConverter))]
#endif
    public sealed class IntCollection : FastList<int>
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

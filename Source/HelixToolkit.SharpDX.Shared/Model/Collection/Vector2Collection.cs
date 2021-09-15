/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using global::SharpDX;
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
    using Utilities;

#if !NETFX_CORE
    [Serializable]
    [TypeConverter(typeof(Vector2CollectionConverter))]
#endif
    public sealed class Vector2Collection : FastList<Vector2>
    {
        public Vector2Collection()
        {
        }

        public Vector2Collection(int capacity)
            : base(capacity)
        {
        }

        public Vector2Collection(IEnumerable<Vector2> items)
            : base(items)
        {
        }

        public static Vector2Collection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            var th = new TokenizerHelper(source, formatProvider);
            var resource = new Vector2Collection();

            Vector2 value;

            while (th.NextToken())
            {
                value = new Vector2(
                    Convert.ToSingle(th.GetCurrentToken(), formatProvider),
                    Convert.ToSingle(th.NextTokenRequired(), formatProvider));

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
                //str.AppendFormat(provider, "{0:" + format + "}", this[i]);
                str.AppendFormat(provider, "{0},{1}", this[i].X, this[i].Y);
                if (i != this.Count - 1)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }
    }
}

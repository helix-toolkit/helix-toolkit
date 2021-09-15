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
    [TypeConverter(typeof(Vector3CollectionConverter))]
#endif
    public sealed class Vector3Collection : FastList<Vector3>
    {
        public Vector3Collection()
        {
        }

        public Vector3Collection(int capacity)
            : base(capacity)
        {
        }

        public Vector3Collection(IEnumerable<Vector3> items)
            : base(items)
        {
        }

        public static Vector3Collection Parse(string source)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture;

            var th = new TokenizerHelper(source, formatProvider);
            var resource = new Vector3Collection();

            Vector3 value;

            while (th.NextToken())
            {
                value = new Vector3(
                    Convert.ToSingle(th.GetCurrentToken(), formatProvider),
                    Convert.ToSingle(th.NextTokenRequired(), formatProvider),
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
                str.AppendFormat(provider, "{0},{1},{2}", this[i].X, this[i].Y, this[i].Z);
                if (i != this.Count - 1)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }
    }
}

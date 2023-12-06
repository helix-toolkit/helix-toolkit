
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace HelixToolkit.SharpDX;

[Serializable]
[TypeConverter(typeof(Color4CollectionConverter))]
public sealed class Color4Collection : FastList<Color4>
{
    public Color4Collection()
    {
    }

    public Color4Collection(int capacity)
        : base(capacity)
    {
    }

    public Color4Collection(IEnumerable<Color4> items)
        : base(items)
    {
    }

    public static Color4Collection Parse(string source)
    {
        IFormatProvider formatProvider = CultureInfo.InvariantCulture;

        var th = new TokenizerHelper(source, formatProvider);
        var resource = new Color4Collection();

        Color4 value;

        while (th.NextToken())
        {
            value = new Color4(
                Convert.ToSingle(th.GetCurrentToken(), formatProvider),
                Convert.ToSingle(th.NextTokenRequired(), formatProvider),
                Convert.ToSingle(th.NextTokenRequired(), formatProvider),
                Convert.ToSingle(th.NextTokenRequired(), formatProvider));

            resource.Add(value);
        }

        return resource;
    }

    public string ConvertToString(string? format, IFormatProvider? provider)
    {
        if (this.Count == 0)
        {
            return string.Empty;
        }

        var str = new StringBuilder();
        for (var i = 0; i < this.Count; i++)
        {
            //str.AppendFormat(provider, "{0:" + format + "}", this[i]);
            str.AppendFormat(provider, "{0},{1},{2},{3}", this[i].Red, this[i].Green, this[i].Blue, this[i].Alpha);
            if (i != this.Count - 1)
            {
                str.Append(' ');
            }
        }

        return str.ToString();
    }
}

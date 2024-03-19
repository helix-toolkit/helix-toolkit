using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace HelixToolkit;

[Serializable]
[TypeConverter(typeof(Vector2CollectionConverter))]
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
            str.AppendFormat(provider, "{0},{1}", this[i].X, this[i].Y);
            if (i != this.Count - 1)
            {
                str.Append(' ');
            }
        }

        return str.ToString();
    }
}

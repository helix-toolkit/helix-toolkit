using System.ComponentModel;

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
                NumericHelpers.ParseSingle(th.GetCurrentToken(), formatProvider),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), formatProvider));

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

        var builder = new DefaultInterpolatedStringHandler(this.Count * 2 - 1, this.Count * 2, provider);
        Vector2 value;

        for (var i = 0; i < this.Count; i++)
        {
            value = this[i];

            builder.AppendFormatted(value.X);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Y);

            if (i != this.Count - 1)
            {
                builder.AppendLiteral(" ");
            }
        }

        return builder.ToStringAndClear();
    }
}

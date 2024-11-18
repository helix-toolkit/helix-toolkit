using System.ComponentModel;

namespace HelixToolkit;

[Serializable]
[TypeConverter(typeof(Vector3CollectionConverter))]
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
                NumericHelpers.ParseSingle(th.GetCurrentToken(), formatProvider),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), formatProvider),
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

        var builder = new DefaultInterpolatedStringHandler(this.Count * 3 - 1, this.Count * 3, provider);
        Vector3 value;

        for (var i = 0; i < this.Count; i++)
        {
            value = this[i];

            builder.AppendFormatted(value.X);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Y);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Z);

            if (i != this.Count - 1)
            {
                builder.AppendLiteral(" ");
            }
        }

        return builder.ToStringAndClear();
    }
}

using HelixToolkit.Maths;
using System.ComponentModel;

namespace HelixToolkit;

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
                NumericHelpers.ParseSingle(th.GetCurrentToken(), formatProvider),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), formatProvider),
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

        var builder = new DefaultInterpolatedStringHandler(this.Count * 4 - 1, this.Count * 4, provider);
        Color4 value;

        for (var i = 0; i < this.Count; i++)
        {
            value = this[i];

            builder.AppendFormatted(value.Red);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Green);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Blue);
            builder.AppendLiteral(",");
            builder.AppendFormatted(value.Alpha);

            if (i != this.Count - 1)
            {
                builder.AppendLiteral(" ");
            }
        }

        return builder.ToStringAndClear();
    }
}

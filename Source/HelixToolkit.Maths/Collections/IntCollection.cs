using System.ComponentModel;

namespace HelixToolkit;

[Serializable]
[TypeConverter(typeof(IntCollectionConverter))]
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
            int value = NumericHelpers.ParseInt32(th.GetCurrentToken(), formatProvider);
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

        var builder = new DefaultInterpolatedStringHandler(this.Count - 1, this.Count, provider);

        for (var i = 0; i < this.Count; i++)
        {
            int value = this[i];

            builder.AppendFormatted(value);

            if (i != this.Count - 1)
            {
                builder.AppendLiteral(" ");
            }
        }

        return builder.ToStringAndClear();
    }
}

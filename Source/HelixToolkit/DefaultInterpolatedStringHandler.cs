#if NET6_0_OR_GREATER
#else
using System.Text;
#endif

namespace System.Runtime.CompilerServices;

#if NET6_0_OR_GREATER
#else
public ref struct DefaultInterpolatedStringHandler
{
    private readonly IFormatProvider? _provider;

    private readonly StringBuilder _builder;

    public DefaultInterpolatedStringHandler(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, null)
    {
    }

    public DefaultInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider)
    {
        _provider = provider;
        _builder = new();
    }

    public void AppendLiteral(string value)
    {
        _builder.Append(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _builder.AppendFormat(_provider, "{0}", value);
    }

    public string ToStringAndClear()
    {
        return _builder.ToString();
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
#endif

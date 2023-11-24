namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public sealed class DefaultEffectAttributeParser : IEffectAttributeParser
{
    public readonly static char[] EffectSeparator = new char[] { ';', ' ' };
    public readonly static char[] AttributeSeparator = new char[] { ',', ' ' };
    public readonly static char[] AttributeNameValueSeparator = new char[] { ':', ' ' };
    public readonly static char[] NameAttributeSeparator = new char[] { '[', ']', ' ' };

    /// <summary>
    /// Parses the specified att string.
    /// </summary>
    /// <param name="attString">The att string.</param>
    /// <returns></returns>
    public EffectAttributes[] Parse(string attString)
    {
        var effects = attString.Split(EffectSeparator, StringSplitOptions.RemoveEmptyEntries);
        IList<EffectAttributes> attributes = new List<EffectAttributes>();
        foreach (var effect in effects)
        {
            var nameAttTokens = effect.Split(NameAttributeSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (nameAttTokens.Length > 0)
            {
                var att = new EffectAttributes(nameAttTokens[0]);
                for (var i = 1; i < nameAttTokens.Length; ++i)
                {
                    var attTokens = nameAttTokens[i].Split(AttributeSeparator, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var attToken in attTokens)
                    {
                        var token = attToken.Split(AttributeNameValueSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (token.Length == 2)
                        {
                            att.AddAttribute(token[0].ToLower(), token[1]);
                        }
                    }
                }
                attributes.Add(att);
            }
        }
        return attributes.ToArray();
    }
}

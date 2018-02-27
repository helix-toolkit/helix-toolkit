/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    public interface IEffectAttributes
    {
        string EffectName { get; }
        void AddAttribute(string attName, object parameter);
        void RemoveAttribute(string attName);
        object GetAttribute(string attName);
        bool TryGetAttribute(string attName, out object attribute);
    }

    public sealed class EffectAttributes : IEffectAttributes
    {
        public string EffectName { private set; get; }
        private readonly Dictionary<string, object> attributes = new Dictionary<string, object>();
        public EffectAttributes(string name)
        {
            EffectName = name;
        }

        public void AddAttribute(string attName, object parameter)
        {
            if (attributes.ContainsKey(attName))
            { return; }
            attributes.Add(attName, parameter);
        }

        public void RemoveAttribute(string attName)
        {
            attributes.Remove(attName);
        }

        public object GetAttribute(string attName)
        {
            object obj;
            if(attributes.TryGetValue(attName, out obj))
            {
                return obj;
            }
            else { return null; }
        }

        public bool TryGetAttribute(string attName, out object attribute)
        {
            return attributes.TryGetValue(attName, out attribute);
        }

        public static EffectAttributes[] Parse(string attString)
        {
            return Parse(attString, EffectParserConfiguration.Parser);
        }

        public static EffectAttributes[] Parse(string attString, IEffectAttributeParser parser)
        {
            return parser.Parse(attString);
        }
    }

    public interface IEffectAttributeParser
    {
        EffectAttributes[] Parse(string attString);
    }

    public static class EffectParserConfiguration
    {
        public static IEffectAttributeParser Parser = new DefaultEffectAttributeParser();
    }

    public sealed class DefaultEffectAttributeParser : IEffectAttributeParser
    {
        public readonly static char[] EffectSeparator = new char[] { ';', ' ' };
        public readonly static char[] AttributeSeparator = new char[] { ',', ' ' };
        public readonly static char[] AttributeNameValueSeparator = new char[] { ':', ' ' };
        public readonly static char[] NameAttributeSeparator = new char[] { '[', ']', ' ' };
        public EffectAttributes[] Parse(string attString)
        {
            var effects = attString.Split(EffectSeparator, StringSplitOptions.RemoveEmptyEntries);
            IList<EffectAttributes> attributes = new List<EffectAttributes>();
            foreach(var effect in effects)
            {
                var nameAttTokens = effect.Split(NameAttributeSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (nameAttTokens.Length > 0)
                {
                    var att = new EffectAttributes(nameAttTokens[0]);
                    for(int i = 1; i < nameAttTokens.Length; ++i)
                    {
                        var attTokens = nameAttTokens[i].Split(AttributeSeparator, StringSplitOptions.RemoveEmptyEntries);
                        foreach(var attToken in attTokens)
                        {
                            var token = attToken.Split(AttributeNameValueSeparator, StringSplitOptions.RemoveEmptyEntries);
                            if(token.Length == 2)
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
}

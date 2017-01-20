namespace DataTemplateDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;

    public static class Extensions
    {
        public static IEnumerable<Type> GetTypeAndBaseTypes(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return GetTypeAndBaseTypesImpl(type);
        }
        private static IEnumerable<Type> GetTypeAndBaseTypesImpl(Type type)
        {
            yield return type;

            var current = type;
            while (current.BaseType != null)
            {
                yield return current.BaseType;
                current = current.BaseType;
            }
        }

        public static IEnumerable<FieldInfo> GetPublicStaticFields(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return GetPublicStaticFieldsImpl(type);
        }
        private static IEnumerable<FieldInfo> GetPublicStaticFieldsImpl(Type type)
        {
            foreach (var t in GetTypeAndBaseTypes(type))
            {
                foreach (var fi in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                    yield return fi;
            }
        }

        public static IEnumerable<XmlNode> TraverseAllNodes(this XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return TraverseNodesImpl(doc).Skip(1);
        }
        private static IEnumerable<XmlNode> TraverseNodesImpl(XmlNode node)
        {
            yield return node;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                foreach (var node1 in TraverseNodesImpl(childNode))
                    yield return node1;
            }
        }
    }
}

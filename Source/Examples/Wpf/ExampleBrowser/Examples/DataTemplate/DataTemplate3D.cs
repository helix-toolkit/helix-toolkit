using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System;
using System.Collections;

namespace DataTemplate;

[ContentProperty(nameof(Content))]
public class DataTemplate3D : DispatcherObject
{
    static DataTemplate3D()
    {
        // add converters for XamlWriter.Save
        TypeDescriptor.AddAttributes(typeof(BindingExpression), new TypeConverterAttribute(typeof(BindingConverter)));
        TypeDescriptor.AddAttributes(typeof(MultiBindingExpression), new TypeConverterAttribute(typeof(MultiBindingConverter)));
    }

    /// <summary>
    /// Regex to match binding expressions.
    /// </summary>
    private static readonly Regex bindingRegex = new("{(.+:)?Binding.*}");

    private XmlDocument? contentXamlSerialized;
    private List<PathInfo>[]? listOfBindingPaths;

    /// <summary>
    /// Gets or sets the template object.
    /// </summary>
    public Visual3D? Content { get; set; }

    /// <summary>
    /// Gets or sets whether the model has a generated content or not.
    /// Defaults to true.
    /// </summary>
    public bool HasGeneratedContent { get; set; }

    public DataTemplate3D()
    {
        HasGeneratedContent = true;
    }

    /// <summary>
    /// Creates a copy of the template with <paramref name="dataContext"/> as source on all bindings.
    /// </summary>
    /// <param name="dataContext">The source object for all data bindings on the new object.</param>
    /// <returns>A clone of the template object.</returns>
    public Visual3D CreateItem(object dataContext)
    {
        if (contentXamlSerialized == null)
        {
            // cache serialized xml document
            var xmlDoc = ObjectToXaml(Content);
            contentXamlSerialized = xmlDoc;
        }

        var clonedObj = (Visual3D)XamlReader.Parse(contentXamlSerialized.InnerXml);
        // find and cache all bindings paths
        listOfBindingPaths ??= FindInlineBindingsInTree(contentXamlSerialized)
                                .Concat(FindElementBindingsInTree(contentXamlSerialized))
                                .ToArray();

        // clear references from previous runs because they belong to another object (reflection results do not need to be cleared)
        foreach (var path in listOfBindingPaths.SelectMany(x => x))
            path.Reference = null;

        var listOfBindingPathsClone = new LinkedList<List<PathInfo>>(listOfBindingPaths);

        while (listOfBindingPathsClone.Count > 0)
        {
            var curPath = listOfBindingPathsClone.First!.Value;
            listOfBindingPathsClone.RemoveFirst();

            if (UpdateBindingSource(clonedObj, dataContext, curPath))
            {
                // update newly discovered references on the yet to proccess paths
                UpdateKnownReferencesInTree(curPath, listOfBindingPathsClone);
            }
        }

        return clonedObj;
    }

    /// <summary>
    /// Creates a <see cref="PathInfo"/> from an <see cref="XmlNode"/>.
    /// </summary>
    /// <param name="cur">The xml node.</param>
    private PathInfo CreatePathSegment(XmlNode cur)
    {
        var pi = new PathInfo();

        string name = cur.Name;
        var collonIndex = name.IndexOf(':');
        if (collonIndex >= 0)
            name = name.Substring(collonIndex + 1);

        var pointIndex = name.LastIndexOf('.');
        if (pointIndex >= 0)
        {
            name = name.Substring(pointIndex + 1);
            pi.IsProperty = true;
        }
        else
        {
            pi.IsProperty = false;
        }

        pi.Name = name;

        var parent = cur.ParentNode;
        if (parent != null && parent.ChildNodes.Count > 1)
        {
            int index = 0;
            foreach (var node1 in parent.ChildNodes)
            {
                if (node1 == cur)
                {
                    pi.Position = index;
                    break;
                }

                index++;
            }
        }
        else
        {
            pi.Position = 0;
        }

        return pi;
    }
    /// <summary>
    /// Performs the action described by <paramref name="path"/>.
    /// </summary>
    /// <param name="obj">The root object.</param>
    /// <param name="path">A <see cref="PathInfo"/> describing how to retrieve the next object.</param>
    /// <returns>The next object as described by <paramref name="path"/>.</returns>
    private object? GetValueOf(object obj, PathInfo path)
    {
        if (path.Reference != null)
            return path.Reference;

        if (obj.GetType().Name == path.Name)
            return obj;

        if (path.IsProperty)
        {
            var pi = obj.GetType().GetProperty(path.Name);
            return pi?.GetValue(obj);
        }

        if (obj is IEnumerable collection)
        {
            var value = collection.Cast<object>()
                                  .Skip(path.Position)
                                  .First();

            // check that the object type is the correct one
            if (value.GetType().Name != path.Name)
                throw new Exception(String.Format("Class name mismatch: {0} vs {1}", value.GetType().Name, path.Name));

            return value;
        };

        throw new Exception("Unkown retrieval method.");
    }
    /// <summary>
    /// Finds bound attributes in the xml document representing the object.
    /// The bindings must be inlined as an attribute expression.
    /// </summary>
    /// <param name="xmlDoc">An <see cref="XmlDocument"/>.</param>
    /// <returns>A list of paths that have a data binding expression.</returns>
    private IEnumerable<List<PathInfo>> FindInlineBindingsInTree(XmlDocument xmlDoc)
    {
        foreach (var attr in xmlDoc.TraverseAllNodes().SelectMany(n => n.Attributes!.Cast<XmlAttribute>()))
        {
            if (bindingRegex.IsMatch(attr.InnerText))
            {
                var nodes = new List<PathInfo>
                {
                    new PathInfo { Name = attr.Name }
                };

                for (XmlNode cur = attr.OwnerElement!; cur != null; cur = cur.ParentNode!)
                {
                    var segment = CreatePathSegment(cur);
                    nodes.Add(segment);
                }

                nodes.Reverse();
                nodes.RemoveAt(0); // remove #document

                yield return nodes;
            }
        }
    }
    /// <summary>
    /// Finds bound attributes and elements (nodes) in the xml document representing the object.
    /// The bindings must be normal xml elements and not inlined as an attribute expression.
    /// </summary>
    /// <param name="xmlDoc">An <see cref="XmlDocument"/>.</param>
    /// <returns>A list of paths that have a data multi binding expression.</returns>
    private IEnumerable<List<PathInfo>> FindElementBindingsInTree(XmlDocument xmlDoc)
    {
        foreach (var node in xmlDoc.TraverseAllNodes())
        {
            if (node.Name.EndsWith("Binding"))
            {
                var nodes = new List<PathInfo>();

                for (XmlNode cur = node.ParentNode!; cur != null; cur = cur.ParentNode!)
                {
                    var segment = CreatePathSegment(cur);
                    nodes.Add(segment);
                }

                nodes.Reverse();
                nodes.RemoveAt(0); // remove #document

                yield return nodes;
            }
        }
    }
    /// <summary>
    /// Converts the <paramref name="obj"/> into an xaml representation wrapped by an <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>An <see cref="XmlDocument"/> holding the xaml representation of <paramref name="obj"/>.</returns>
    private XmlDocument ObjectToXaml(object? obj)
    {
        var outstr = new StringBuilder();

        //this code is needed for right XAML fomating
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        //http://www.codeproject.com/Articles/27158/XamlWriter-and-Bindings-Serialization
        var dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings))
        {
            XamlWriterMode = XamlWriterMode.Expression
        };

        XamlWriter.Save(Content, dsm);
        var xaml = outstr.ToString();

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xaml);

        if (HasGeneratedContent)
        {
            var contentNode = xmlDoc.DocumentElement?.ChildNodes.Cast<XmlNode>().Where(n => n.LocalName.EndsWith(".Content")).FirstOrDefault();

            // reomve generated content except when it is a Binding/MultiBinding sub node
            if (contentNode != null && (contentNode.ChildNodes.Count != 1 || !new[] { "Binding", "MultiBinding" }.Contains(contentNode?.ChildNodes[0]?.LocalName)))
                contentNode?.ParentNode?.RemoveChild(contentNode);
        }

        return xmlDoc;
    }
    /// <summary>
    /// Updates the binding on the property described by <paramref name="path"/> to use <paramref name="dataContext"/> as source.
    /// </summary>
    /// <param name="obj">The root object.</param>
    /// <param name="dataContext">The object to use as source.</param>
    /// <param name="path">A list of objects describing the path through the object tree to the property.</param>
    private bool UpdateBindingSource(object obj, object dataContext, ICollection<PathInfo> path)
    {
        object nestedObj = obj;
        bool hasNewlyDiscoveredObjects = false;
        var segmentsToTraverse = path.Take(path.Count - 1)
                                     .SkipWhile(p => p.Reference != null)
                                     .ToArray();

        if (segmentsToTraverse.Length > 0)
        {
            // update object with last known reference
            var index = path.Count - segmentsToTraverse.Length - 1 - 1;
            if (index >= 0)
            {
                nestedObj = path.ElementAt(index)
                                .Reference!;
            }

            foreach (var pathSegment in segmentsToTraverse)
            {
                nestedObj = GetValueOf(nestedObj, pathSegment)!;
                pathSegment.Reference = nestedObj;

                hasNewlyDiscoveredObjects = true;
            }
        }
        else
        {
            // update object with last known reference
            nestedObj = path.ElementAt(path.Count - 2)
                            .Reference!;
        }

        var type = nestedObj!.GetType();

        var visual = nestedObj as DependencyObject;
        if (visual != null)
        {
            var secondLastSegment = path.ElementAt(path.Count - 2);
            if (secondLastSegment.PublicDependencyProperties == null)
                secondLastSegment.PublicDependencyProperties = type.GetPublicStaticFields().ToArray();

            var lastSegmentName = path.Last().Name;

            foreach (var fi in secondLastSegment.PublicDependencyProperties)
            {
                var dp = fi.GetValue(null) as DependencyProperty;
                if (dp != null && dp.Name == lastSegmentName)
                {
                    var binding = BindingOperations.GetBinding(visual, dp);
                    if (binding != null && binding.Source == null)
                    {
                        BindingOperations.ClearBinding(visual, dp);

                        var b = binding.Clone();
                        b.Source = dataContext;

                        BindingOperations.SetBinding(visual, dp, b);

                        break;
                    }

                    var multiBinding = BindingOperations.GetMultiBinding(visual, dp);
                    if (multiBinding != null)
                    {
                        var newMultiBinding = multiBinding.Clone();

                        for (int a = 0; a < multiBinding.Bindings.Count; a++)
                        {
                            var innerBinding = multiBinding.Bindings[a] as Binding;
                            if (innerBinding != null && innerBinding.Source == null)
                            {
                                var newInnerBinding = innerBinding.Clone();
                                newInnerBinding.Source = dataContext;

                                newMultiBinding.Bindings.Add(newInnerBinding);
                            }
                            else
                            {
                                newMultiBinding.Bindings.Add(multiBinding.Bindings[a]);
                            }
                        }

                        BindingOperations.ClearBinding(visual, dp);
                        BindingOperations.SetBinding(visual, dp, newMultiBinding);
                    }
                }
            }
        }

        return hasNewlyDiscoveredObjects;
    }
    /// <summary>
    /// Shares the references of the <paramref name="currentlyUpdatedPath"/> with all <paramref name="otherPaths"/>.
    /// </summary>
    /// <param name="currentlyUpdatedPath">The source tree to take the reference from.</param>
    /// <param name="otherPaths">The destination trees to update.</param>
    private void UpdateKnownReferencesInTree(IList<PathInfo> currentlyUpdatedPath, LinkedList<List<PathInfo>> otherPaths)
    {
        foreach (var otherPath in otherPaths)
        {
            for (int a = 0; a < Math.Min(otherPath.Count, currentlyUpdatedPath.Count); a++)
            {
                var curPathSeg = currentlyUpdatedPath[a];
                var othPathSeg = otherPath[a];

                if (curPathSeg.Name == othPathSeg.Name && curPathSeg.IsProperty == othPathSeg.IsProperty && curPathSeg.Position == othPathSeg.Position)
                {
                    if (othPathSeg.Reference == null)
                        othPathSeg.Reference = curPathSeg.Reference;

                    if (othPathSeg.PublicDependencyProperties == null)
                        othPathSeg.PublicDependencyProperties = curPathSeg.PublicDependencyProperties;
                }
                else
                {
                    // other tree path -> no further checks needed
                    break;
                }
            }
        }
    }

    private sealed class PathInfo
    {
        public bool IsProperty { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;

        public object? Reference { get; set; }
        public FieldInfo[]? PublicDependencyProperties { get; set; }
    }
}

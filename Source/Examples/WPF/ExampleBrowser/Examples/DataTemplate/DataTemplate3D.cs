namespace DataTemplateDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    [ContentProperty("Content")]
    public class DataTemplate3D : DispatcherObject
    {
        public Visual3D Content { get; set; }

        public Visual3D CreateItem(object source)
        {
            var type = this.Content.GetType();
            var types = new List<Type>();
            types.Add(type);
            var current = type;
            while (current.BaseType != null)
            {
                types.Add(current.BaseType);
                current = current.BaseType;
            }

            var visual = (Visual3D)Activator.CreateInstance(type);
            var boundProperties = new HashSet<string>();
            foreach (var t in types)
            {
                foreach (var fi in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var dp = fi.GetValue(null) as DependencyProperty;
                    if (dp != null)
                    {
                        var binding = BindingOperations.GetBinding(this.Content, dp);
                        if (binding != null)
                        {
                            boundProperties.Add(dp.Name);
                            BindingOperations.SetBinding(
                                visual, dp, new Binding { Path = binding.Path, Source = source });
                        }
                    }
                }
            }

            //foreach (var p in type.GetProperties())
            //{
            //    if (!p.CanWrite) continue;
            //    if (boundProperties.Contains(p.Name)) 
            //        continue;
            //    var value = p.GetValue(this.Content);
            //    p.SetValue(visual, value);
            //}

            return visual;
        }
    }
}
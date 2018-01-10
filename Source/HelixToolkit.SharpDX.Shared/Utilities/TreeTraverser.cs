using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Model.Utilities
#endif
{
    public static class TreeTraverser
    {
        /// <summary>
        /// Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderable> PreorderDFT(this IEnumerable<IRenderable> nodes, Func<IRenderable, bool> condition)
        {
            var stack = new Stack<IEnumerator<IRenderable>>(20);
            var e = nodes.GetEnumerator();
            try
            {
                while (true)
                {
                    while (e.MoveNext())
                    {
                        var item = e.Current;
                        if (!condition(item)) { continue; }
                        yield return item;
                        var elements = item.Items;
                        if (elements == null) continue;
                        stack.Push(e);
                        e = elements.GetEnumerator();
                    }
                    if (stack.Count == 0) break;
                    e.Dispose();
                    e = stack.Pop();
                }
            }
            finally
            {
                e.Dispose();
                while (stack.Count != 0) stack.Pop().Dispose();
            }
        }
    }
}

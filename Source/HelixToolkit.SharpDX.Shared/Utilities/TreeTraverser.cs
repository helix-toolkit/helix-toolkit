using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Core;
    public static class TreeTraverser
    {
        /// <summary>
        /// Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderable> PreorderDFT(this IEnumerable<IRenderable> nodes, Func<IRenderable, bool> condition, 
            Stack<IEnumerator<IRenderable>> stackCache = null)
        {
            var stack = stackCache == null ? new Stack<IEnumerator<IRenderable>>(20) : stackCache;
            var e = nodes.GetEnumerator();

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

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }
        /// <summary>
        /// Preorders the DFT without using Linq.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="context">The context.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="results">The results.</param>
        /// <param name="stackCache">The stack cache.</param>
        public static void PreorderDFT(this IList<IRenderable> nodes, IRenderContext context,
            Func<IRenderable, IRenderContext, bool> condition, IList<IRenderable> results,
            Stack<KeyValuePair<int, IList<IRenderable>>> stackCache = null)
        {
            var stack = stackCache == null ? new Stack<KeyValuePair<int, IList<IRenderable>>>(20) : stackCache;
            int i = -1;
            while (true)
            {
                while(++i < nodes.Count)
                {
                    var item = nodes[i];              
                    if (!condition(item, context))
                    { continue; }
                    results.Add(item);
                    var elements = item.Items;
                    if(elements == null || elements.Count == 0)
                    { continue; }
                    stack.Push(new KeyValuePair<int, IList<IRenderable>>(i, nodes));
                    i = -1;
                    nodes = elements;
                }
                if (stack.Count == 0)
                { break; }
                var prev = stack.Pop();
                i = prev.Key;
                nodes = prev.Value;
            }
        }

        /// <summary>
        /// Get render cores by Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderCore> PreorderDFTGetCores(this IEnumerable<IRenderable> nodes, Func<IRenderable, bool> condition,
            Stack<IEnumerator<IRenderable>> stackCache = null)
        {
            var stack = stackCache == null ? new Stack<IEnumerator<IRenderable>>(20) : stackCache;
            var e = nodes.GetEnumerator();

            while (true)
            {
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!condition(item)) { continue; }
                    yield return item.RenderCore;
                    var elements = item.Items;
                    if (elements == null) continue;
                    stack.Push(e);
                    e = elements.GetEnumerator();
                }
                if (stack.Count == 0) break;
                e.Dispose();
                e = stack.Pop();
            }

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }

        /// <summary>
        /// Preorders the DFT without Linq;
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="stackCache">The stack cache.</param>
        public static void PreorderDFTRun(this IList<IRenderable2D> nodes, Func<IRenderable2D, bool> condition, 
            Stack<KeyValuePair<int, IList<IRenderable2D>>> stackCache = null)
        {
            var stack = stackCache == null ? new Stack<KeyValuePair<int, IList<IRenderable2D>>>(20) : stackCache;
            int i = -1;
            while (true)
            {
                while (++i < nodes.Count)
                {
                    var item = nodes[i];
                    if (!condition(item))
                    { continue; }
                    var elements = item.Items;
                    if (elements == null || elements.Count == 0)
                    { continue; }
                    stack.Push(new KeyValuePair<int, IList<IRenderable2D>>(i, nodes));
                    i = -1;
                    nodes = elements;
                }
                if (stack.Count == 0)
                { break; }
                var prev = stack.Pop();
                i = prev.Key;
                nodes = prev.Value;
            }
        }

        /// <summary>
        /// Get render cores by Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderCore2D> PreorderDFTGetCores(this IEnumerable<IRenderable2D> nodes, Func<IRenderable2D, bool> condition,
            Stack<IEnumerator<IRenderable2D>> stackCache = null)
        {
            var stack = stackCache == null ? new Stack<IEnumerator<IRenderable2D>>(20) : stackCache;
            var e = nodes.GetEnumerator();

            while (true)
            {
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!condition(item)) { continue; }
                    yield return item.RenderCore;
                    var elements = item.Items;
                    if (elements == null) continue;
                    stack.Push(e);
                    e = elements.GetEnumerator();
                }
                if (stack.Count == 0) break;
                e.Dispose();
                e = stack.Pop();
            }

            e.Dispose();
            while (stack.Count != 0)
            { stack.Pop().Dispose(); }
        }
    }
}

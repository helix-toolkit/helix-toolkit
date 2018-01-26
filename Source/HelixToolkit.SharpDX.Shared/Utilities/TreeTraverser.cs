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
        /// Pre-ordered depth first traverse
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="condition"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        public static IEnumerable<IRenderable2D> PreorderDFT(this IEnumerable<IRenderable2D> nodes, Func<IRenderable2D, bool> condition,
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

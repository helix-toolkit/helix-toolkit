/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
#if NETFX_CORE

#else
    using System;
    using System.Reflection.Emit;
#endif

    public static class CollectionExtensions
    {
#if NETFX_CORE
        /// <summary>
        /// Gets the internal array of a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="list">The respective list.</param>
        /// <returns>The internal array of the list.</returns>
        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return list.ToArray();
        }

        public static T[] GetArrayByType<T>(this IList<T> list)
        {
            T[] array;
            if (list is T[] t)
            {
                array = t;
            }
            else if (list is FastList<T> f)
            {
                array = f.Items;
            }
            else
            {
                array = list.ToArray();
            }
            return array;
        }
#else
        static class ArrayAccessor<T>
        {
            public static readonly Func<List<T>, T[]> Getter;

            static ArrayAccessor()
            {
                var dm = new DynamicMethod(
                    "get",
                    MethodAttributes.Static | MethodAttributes.Public,
                    CallingConventions.Standard,
                    typeof(T[]),
                    new Type[] { typeof(List<T>) },
                    typeof(ArrayAccessor<T>),
                    true);
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // Load List<T> argument
                il.Emit(OpCodes.Ldfld,
                    typeof(List<T>).GetField("_items",
                    BindingFlags.NonPublic | BindingFlags.Instance)); // Replace argument by field
                il.Emit(OpCodes.Ret); // Return field
                Getter = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
            }
        }

        /// <summary>
        /// Gets the internal array of a <see cref="List{T}"/>.
        /// <para>Warning: Internal array length >= List.Count. Use with cautious.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="list">The respective list.</param>
        /// <returns>The internal array of the list.</returns>
        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return ArrayAccessor<T>.Getter(list);
        }

        public static T[] GetArrayByType<T>(this IList<T> list)
        {
            T[] array;
            if (list is T[] t)
            {
                array = t;
            }
            else if(list is FastList<T> f)
            {
                array = f.Items;
            }
            else if (list is List<T> l)
            {
                array = l.GetInternalArray();
            }
            else
            {
                array = list.ToArray();
            }
            return array;
        }
#endif

        /// <summary>
        /// Tries to get a value from a <see cref="IDictionary{K,V}"/>.
        /// </summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dict">The respective dictionary.</param>
        /// <param name="key">The respective key.</param>
        /// <returns>The value if exists, else <c>null</c>.</returns>
        public static V Get<K, V>(this IDictionary<K, V> dict, K key)
        {
            V val;
            if (dict.TryGetValue(key, out val))
            {
                return val;
            }

            return default(V);
        }
    }
}

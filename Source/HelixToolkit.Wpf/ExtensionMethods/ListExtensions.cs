using System.Reflection.Emit;
using System.Reflection;

namespace HelixToolkit.Wpf;

internal static class ListExtensions
{
    private static class ArrayAccessor<T>
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
                BindingFlags.NonPublic | BindingFlags.Instance)!); // Replace argument by field
            il.Emit(OpCodes.Ret); // Return field

            Getter = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
        }
    }

    public static T[] GetInternalArray<T>(this List<T> list)
    {
        return ArrayAccessor<T>.Getter(list);
    }

    public static void SetInternalSize<T>(this List<T> list, int size)
    {
        list.GetType().GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance)
            !.SetValue(list, size);
    }
}

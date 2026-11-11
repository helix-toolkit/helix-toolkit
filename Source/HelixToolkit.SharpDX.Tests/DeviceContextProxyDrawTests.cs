using System.Runtime.InteropServices;
using HelixToolkit.SharpDX.Render;
using NUnit.Framework;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class DeviceContextProxyDrawTests
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DrawIndexedInstancedCallback(IntPtr context, int indexCount, int instanceCount, int startIndex, int baseVertex, int startInstance);

    [Test]
    [Platform("Win")]
    public void DrawIndexedInstancedForwardsEachArgument()
    {
        int[]? observed = null;
        DrawIndexedInstancedCallback draw = (_, indexCount, instanceCount, startIndex, baseVertex, startInstance) =>
            observed = [indexCount, instanceCount, startIndex, baseVertex, startInstance];

        // ID3D11DeviceContext places DrawIndexedInstanced at vtable slot 20, including inherited methods.
        const int drawSlot = 20;
        var table = Marshal.AllocHGlobal((drawSlot + 1) * IntPtr.Size);
        var instance = Marshal.AllocHGlobal(IntPtr.Size);
        for (var slot = 0; slot <= drawSlot; slot++)
        {
            Marshal.WriteIntPtr(table, slot * IntPtr.Size, IntPtr.Zero);
        }

        Marshal.WriteIntPtr(table, drawSlot * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(draw));
        Marshal.WriteIntPtr(instance, table);
        var context = new global::SharpDX.Direct3D11.DeviceContext1(instance);
        var proxy = new DeviceContextProxy(context, null!);
        try
        {
            proxy.DrawIndexedInstanced(3, 5, 7, 11, 13);

            Assert.That(observed, Is.EqualTo(new[] { 3, 5, 7, 11, 13 }));
            Assert.That(proxy.NumberOfDrawCalls, Is.EqualTo(1));
        }
        finally
        {
            context.NativePointer = IntPtr.Zero;
            context.Dispose();
            proxy.Dispose();
            GC.KeepAlive(draw);
            Marshal.FreeHGlobal(instance);
            Marshal.FreeHGlobal(table);
        }
    }
}

using HelixToolkit.SharpDX.Core;
using NUnit.Framework;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class ThreadBufferManagerTests
{
    [Test]
    public void GetBufferReturnsAtLeastRequestedSizeAboveTwoPow24()
    {
        // Above 2^24 the int -> float conversion inside the size computation rounds to
        // the nearest representable float; when it rounded down, GetBuffer allocated
        // fewer elements than requested and BuildVertexArray indexed past the end of
        // the returned buffer (IndexOutOfRangeException during OnAttachBuffers for
        // meshes with more than 16,777,216 vertices).
        // 67,108,867 and 100,000,003 sit in float ranges with spacing 8 and round down
        // on conversion; both exceed MaximumElementCount for byte (64 MB), taking the
        // unscaled allocation path where no growth factor masks the loss.
        int[] requests = { 16_777_217, 67_108_867, 67_108_869, 100_000_003 };
        foreach (var request in requests)
        {
            var buffer = ThreadBufferManager<byte>.GetBuffer(request);
            Assert.That(buffer.Length, Is.GreaterThanOrEqualTo(request),
                $"GetBuffer({request}) returned a buffer {request - buffer.Length} elements short.");
        }
    }
}

using HelixToolkit.SharpDX.Animations;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SharpAssimp;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HelixToolkit.SharpDX.Assimp.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ImporterAnimationTests
    {
        private sealed class TestImporter : Importer
        {
            public FastList<Keyframe> ProcessChannel(NodeAnimationChannel channel, double ticksPerSecond)
            {
                var code = ProcessNodeAnimation(channel, ticksPerSecond, out var list);
                ClassicAssert.AreEqual(ErrorCode.Succeed, code);
                return list;
            }
        }

        [Test]
        public void ProcessNodeAnimation_MissingScalingKeys_CreatesKeyframes()
        {
            var channel = new NodeAnimationChannel { NodeName = "node" };
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.5f);
            for (var i = 0; i < 3; ++i)
            {
                channel.PositionKeys.Add(new VectorKey(i, new Vector3(i, 0, 0)));
                channel.RotationKeys.Add(new QuaternionKey(i, rotation));
            }

            using var importer = new TestImporter();
            var keyframes = importer.ProcessChannel(channel, 1);

            ClassicAssert.AreEqual(3, keyframes.Count);
            for (var i = 0; i < keyframes.Count; ++i)
            {
                ClassicAssert.AreEqual((float)i, keyframes[i].Time, 1e-6f);
                ClassicAssert.AreEqual(new Vector3(i, 0, 0), keyframes[i].Translation);
                ClassicAssert.AreEqual(rotation, keyframes[i].Rotation);
                ClassicAssert.AreEqual(Vector3.One, keyframes[i].Scale);
            }
        }

        [Test]
        public void ProcessNodeAnimation_MissingRotationKeys_UsesIdentityRotation()
        {
            var channel = new NodeAnimationChannel { NodeName = "node" };
            for (var i = 0; i < 2; ++i)
            {
                channel.PositionKeys.Add(new VectorKey(i, new Vector3(i, 0, 0)));
                channel.ScalingKeys.Add(new VectorKey(i, new Vector3(2, 2, 2)));
            }

            using var importer = new TestImporter();
            var keyframes = importer.ProcessChannel(channel, 1);

            ClassicAssert.AreEqual(2, keyframes.Count);
            foreach (var keyframe in keyframes)
            {
                ClassicAssert.AreEqual(Quaternion.Identity, keyframe.Rotation);
            }
        }
    }
}

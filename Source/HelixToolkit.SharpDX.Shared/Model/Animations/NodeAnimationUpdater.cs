/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    namespace Animations
    {
        public class NodeAnimationUpdater : IAnimationUpdater
        {
            private struct IndexTime
            {
                public int Index;
            }

            public string Name
            {
                set; get;
            } = string.Empty;

            public Animation Animation
            {
                get;
            }

            public float Speed { set; get; } = 1.0f;

            private long currentTime;
            private IndexTime[] keyframeIndices;
            private bool isStartFrame = false;
            private bool changed = false;
            private float previousTimeElapsed = float.MinValue;

            public IList<NodeAnimation> NodeCollection
            {
                get => Animation.NodeAnimationCollection;
            }
            public NodeAnimation[] OrderedNodeCollection;

            public AnimationRepeatMode RepeatMode
            {
                set; get;
            } = AnimationRepeatMode.Loop;

            public NodeAnimationUpdater(Animation animation)
            {
                Animation = animation;
                Name = animation.Name;
                keyframeIndices = new IndexTime[NodeCollection.Count];
                OrderedNodeCollection = new NodeAnimation[NodeCollection.Count];
                NodeCollection.CopyTo(OrderedNodeCollection, 0);
                int index = 0;
                while (index < NodeCollection.Count)
                {
                    NodeAnimation animationNode = OrderedNodeCollection[index];
                    if (animationNode.Node.Parent != null)
                    {
                        int index2 = index;
                        for (; index2 < NodeCollection.Count; index2++)
                        {
                            if (OrderedNodeCollection[index2].Node == animationNode.Node.Parent)
                            {
                                (OrderedNodeCollection[index2], OrderedNodeCollection[index]) =
                                    (OrderedNodeCollection[index], OrderedNodeCollection[index2]);
                                break;
                            }
                        }
                        if (index2 == NodeCollection.Count) index++;
                    }
                };
            }

            public void Update(long timeStamp, long frequency)
            {
                if (currentTime == 0)
                {
                    currentTime = timeStamp;
                    SetToStart();
                    UpdateBoneSkinMesh();
                    return;
                }

                var timeElapsed = (float)Math.Max(0, timeStamp - currentTime) / frequency * Speed;

                if (timeElapsed >= Animation.EndTime)
                {
                    switch (RepeatMode)
                    {
                        case AnimationRepeatMode.PlayOnce:
                            isStartFrame = false;
                            SetToStart();
                            UpdateBoneSkinMesh();
                            return;
                        case AnimationRepeatMode.PlayOnceHold:
                            timeElapsed = Animation.EndTime;
                            break;
                        case AnimationRepeatMode.Loop:
                            timeElapsed = timeElapsed % Animation.EndTime;
                            break;
                    }
                }
                if (timeElapsed < previousTimeElapsed)
                    Array.Clear(keyframeIndices, 0, keyframeIndices.Length);
                previousTimeElapsed = timeElapsed;
                UpdateNodes(timeElapsed);
                UpdateBoneSkinMesh();
            }

            private void UpdateBoneSkinMesh()
            {
                foreach (var node in OrderedNodeCollection)
                    node.Node.ComputeTransformMatrix();
                if (Animation.HasBoneSkinMeshes && changed)
                {
                    foreach (var m in Animation.BoneSkinMeshes)
                    {
                        if (m.IsRenderable && !m.HasBoneGroup)// Do not update if has a bone group. Update the group only
                        {
                            var inv = m.TotalModelMatrix.Inverted();
                            var matrices = OnGetNewBoneMatrices(m.Bones.Length);
                            BoneSkinnedMeshGeometry3D.CreateNodeBasedBoneMatrices(m.Bones, ref inv, ref matrices);
                            var old = m.BoneMatrices;
                            m.BoneMatrices = matrices;
                            OnReturnOldBoneMatrices(old);
                        }
                    }
                    changed = false;
                }
            }

            /// <summary>
            /// Called when [get new bone matrices]. Override this to provide your own matices pool to avoid small object creation
            /// </summary>
            /// <param name="count">The count.</param>
            /// <returns></returns>
            protected virtual Matrix[] OnGetNewBoneMatrices(int count)
            {
                return new Matrix[count];
            }
            /// <summary>
            /// Called when [return old bone matrices]. Override this to return the old matrix array back to your own matices pool. <see cref="OnGetNewBoneMatrices(int)"/>
            /// </summary>
            /// <param name="m">The m.</param>
            protected virtual void OnReturnOldBoneMatrices(Matrix[] m)
            {

            }

            private void UpdateNodes(float timeElapsed)
            {
                for (var i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    var count = n.KeyFrames.Count; // Make sure to use this count
                    var frames = n.KeyFrames.Items;
                    ref var idxTime = ref keyframeIndices[i];
                    while (idxTime.Index < count - 1 && timeElapsed > frames[idxTime.Index + 1].Time)//check if should move to next time frame
                    {
                        ++idxTime.Index;
                    }
                    ref var currFrame = ref frames[idxTime.Index];
                    if (count == 1)
                    {
                        n.Node.ModelMatrix = Matrix.Scaling(currFrame.Scale) *
                                Matrix.RotationQuaternion(currFrame.Rotation) *
                                Matrix.Translation(currFrame.Translation);
                    }
                    else
                    {
                        ref var nextFrame = ref frames[idxTime.Index + 1];
                        var diff = timeElapsed - currFrame.Time;
                        var length = nextFrame.Time - currFrame.Time;
                        var amount = diff / length;
                        var transform = Matrix.Scaling(Vector3.Lerp(currFrame.Scale, nextFrame.Scale, amount)) *
                                    Matrix.RotationQuaternion(Quaternion.Slerp(currFrame.Rotation, nextFrame.Rotation, amount)) *
                                    Matrix.Translation(Vector3.Lerp(currFrame.Translation, nextFrame.Translation, amount));
                        n.Node.ModelMatrix = transform;
                    }
                }
                changed = true;
            }

            public void Reset()
            {
                Array.Clear(keyframeIndices, 0, keyframeIndices.Length);
                currentTime = 0;
                changed = false;
                isStartFrame = false;
                previousTimeElapsed = float.MinValue;
            }

            private void SetToStart()
            {
                if (isStartFrame)
                {
                    return;
                }
                for (var i = 0; i < NodeCollection.Count; ++i)
                {
                    var n = NodeCollection[i];
                    var count = n.KeyFrames.Count; // Make sure to use this count
                    if (count == 0)
                    {
                        //n.Node.ModelMatrix = Matrix.Identity;
                        continue;
                    }
                    var frames = n.KeyFrames.Items;
                    ref var currFrame = ref frames[0];
                    n.Node.ModelMatrix = Matrix.Scaling(currFrame.Scale) *
                            Matrix.RotationQuaternion(currFrame.Rotation) *
                            Matrix.Translation(currFrame.Translation);
                }
                isStartFrame = true;
                changed = true;
            }
        }
    }
}

using SharpDX;
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
        /// <summary>
        /// 
        /// </summary>
        public enum AnimationRepeatMode
        {
            PlayOnce,
            Loop,
            PlayOnceHold,
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IAnimationUpdater
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            string Name
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets the repeat mode.
            /// </summary>
            /// <value>
            /// The repeat mode.
            /// </value>
            AnimationRepeatMode RepeatMode
            {
                set; get;
            }
            /// <summary>
            /// Start time for the animation, usually it is 0.
            /// </summary>
            float StartTime { get; }
            /// <summary>
            /// End time for the animation.
            /// </summary>
            float EndTime { get; }
            /// <summary>
            /// Updates the animation with a time stamp.
            /// </summary>
            /// <param name="timeStamp">The time stamp.</param>
            /// <param name="frequency">The frequency. If time stamp is second based, frequency is 1.</param>
            void Update(float timeStamp, long frequency);
            /// <summary>
            /// Resets this animation.
            /// </summary>
            void Reset();
        }
        /// <summary>
        ///
        /// </summary>
        public interface IAnimationNode
        {
            /// <summary>
            /// Gets or sets a value indicating whether this scene node is animation node.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this scene node is animation node; otherwise, <c>false</c>.
            /// </value>
            bool IsAnimationNode
            {
                set; get;
            }
            /// <summary>
            /// Gets a value indicating whether this scene node is animation node root.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this scene node is animation node root; otherwise, <c>false</c>.
            /// </value>
            bool IsAnimationNodeRoot
            {
                get;
            }
        }

        public interface IBoneMatricesNode
        {
            Matrix[] BoneMatrices
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets the bones.
            /// </summary>
            /// <value>
            /// The bones.
            /// </value>
            Bone[] Bones
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets the morph target weights.
            /// </summary>
            /// <value>
            /// The morph target weights.
            /// </value>
            float[] MorphTargetWeights
            {
                set; get;
            }
            /// <summary>
            /// Gets a value indicating whether this instance is renderable.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
            /// </value>
            bool IsRenderable
            {
                get;
            }
            /// <summary>
            /// Gets the total model matrix.
            /// </summary>
            /// <value>
            /// The total model matrix.
            /// </value>
            Matrix TotalModelMatrix
            {
                get;
            }
            /// <summary>
            /// Gets a value indicating whether this instance has bone group.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
            /// </value>
            bool HasBoneGroup
            {
                get;
            }
        }
    }
}

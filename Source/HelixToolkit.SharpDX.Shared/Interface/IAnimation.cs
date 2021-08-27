using SharpDX;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
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
            string Name { set; get; }
            /// <summary>
            /// Playback speed. Default is 1x.
            /// </summary>
            float Speed { set; get; }
            /// <summary>
            /// Gets or sets the repeat mode.
            /// </summary>
            /// <value>
            /// The repeat mode.
            /// </value>
            AnimationRepeatMode RepeatMode { set; get; }
            /// <summary>
            /// Updates the animation with current time stamp.
            /// </summary>
            /// <param name="timeStamp">The time stamp.</param>
            /// <param name="frequency">The frequency.</param>
            void Update(long timeStamp, long frequency);
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
            bool IsAnimationNode { set; get; }
            /// <summary>
            /// Gets a value indicating whether this scene node is animation node root.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this scene node is animation node root; otherwise, <c>false</c>.
            /// </value>
            bool IsAnimationNodeRoot { get; }
        }

        public interface IBoneMatricesNode
        {
            Matrix[] BoneMatrices { set; get; }
            /// <summary>
            /// Gets or sets the bones.
            /// </summary>
            /// <value>
            /// The bones.
            /// </value>
            Bone[] Bones { set; get; }
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
            bool IsRenderable { get; }
            /// <summary>
            /// Gets the total model matrix.
            /// </summary>
            /// <value>
            /// The total model matrix.
            /// </value>
            Matrix TotalModelMatrix { get; }
            /// <summary>
            /// Gets a value indicating whether this instance has bone group.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
            /// </value>
            bool HasBoneGroup { get; }
        }
    }
}

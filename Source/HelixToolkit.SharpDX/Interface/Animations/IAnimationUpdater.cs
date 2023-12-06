namespace HelixToolkit.SharpDX.Animations;

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

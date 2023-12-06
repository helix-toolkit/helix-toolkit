using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
/// <typeparam name="StateType">The type of the tate type.</typeparam>
public abstract class StateProxy<StateType> : DisposeObject where StateType : ComObject
{
    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public StateType? State
    {
        get
        {
            return state;
        }
    }
    private StateType? state;

    public StateProxy(StateType? state)
    {
        this.state = state;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref state);
        base.OnDispose(disposeManagedResources);
    }
    /// <summary>
    /// Performs an implicit conversion
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator StateType?(StateProxy<StateType>? proxy)
    {
        return proxy?.State;
    }
}

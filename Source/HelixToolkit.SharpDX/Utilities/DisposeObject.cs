using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Base class to handle disposable.
/// </summary>
public abstract class DisposeObject : IDisposable
{
    /// <summary>
    /// Occurs when this instance is starting to be disposed.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public event EventHandler<BoolEventArgs>? Disposing;

    /// <summary>
    /// Occurs when this instance is fully disposed.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public event EventHandler<BoolEventArgs>? Disposed;
    internal Action<DisposeObject>? AddBackToPool;
    /// <summary>
    /// Disposes of object resources.
    /// </summary>
    /// <param name="disposeManagedResources">If true, managed resources should be
    /// disposed of in addition to unmanaged resources.</param>
    protected virtual void OnDispose(bool disposeManagedResources)
    {
    }

    /// <summary>
    /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
    /// </summary>
    /// <param name="objectToDispose">Object to dispose.</param>
    public static void RemoveAndDispose<T>(ref T? objectToDispose) where T : class, IDisposable
    {
        if (objectToDispose is IDisposable disposible)
        {
            // Dispose the component
            disposible.Dispose();
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
#pragma warning disable CS8601 // Possible null reference assignment.
            objectToDispose = null;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
        }
    }
    /// <summary>
    /// Dispose a disposable object. Removes this object from this instance..
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectToDispose">The object to dispose.</param>
    public static void RemoveAndDispose<T>(T objectToDispose) where T : class, IDisposable
    {
        if (objectToDispose is IDisposable disposible)
        {
            // Dispose the component
            disposible.Dispose();
        }
    }

    #region IDisposible
    public int RefCount => AtomicHelper.Read(ref refCounter_);

    private int refCounter_ = 1;
    /// <summary>
    /// Increase reference counter
    /// </summary>
    /// <returns></returns>
    public int IncRef()
    {
        // Increment only greater than 1
        AtomicHelper.IncrementIfGreaterThan(ref refCounter_, 0);
        return AtomicHelper.Read(ref refCounter_);
    }
    /// <summary>
    /// Forces the dispose.
    /// </summary>
    public void ForceDispose()
    {
        // Set ref counter to 1 if greater than 1
        AtomicHelper.ExchangeIfGreaterThan(ref refCounter_, 1, 1);
        Dispose();
    }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public bool IsDisposed
    {
        get; private set;
    }

    private int disposeCount_ = 0;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        Dispose(true);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
#pragma warning disable CA1063 // Implement IDisposable Correctly
    private void Dispose(bool disposing)
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        // If already 0, return.
        if (!AtomicHelper.DecrementIfGreaterThan(ref refCounter_, 0))
        {
            Debug.Assert(RefCount == 0);
            return;
        }
        var currRef = RefCount;
        if (currRef == 0 && !IsDisposed)
        {
            if (Interlocked.Increment(ref disposeCount_) == 1)
            {
                AddBackToPool = null;
                Disposing?.Invoke(this, disposing ? BoolEventArgs.TrueArgs : BoolEventArgs.FalseArgs);
                Disposing = null;
                OnDispose(disposing);
                //GC.SuppressFinalize(this);

                IsDisposed = true;

                Disposed?.Invoke(this, disposing ? BoolEventArgs.TrueArgs : BoolEventArgs.FalseArgs);
                Disposed = null;
            }
        }
        else if (currRef == 1)
        {
            AddBackToPool?.Invoke(this);
        }
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static bool Set<T>(ref T backingField, T value)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }
        backingField = value;
        return true;
    }
}

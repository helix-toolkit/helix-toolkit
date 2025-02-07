﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IVertexExtraBufferModel : IGUID, IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this <see cref="IVertexExtraBufferModel"/> is initialized.
    /// </summary>
    /// <value>
    ///   <c>true</c> if initialized; otherwise, <c>false</c>.
    /// </value>
    bool Initialized
    {
        get;
    }
    /// <summary>
    /// Gets a value indicating whether this <see cref="IVertexExtraBufferModel"/> is changed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if changed; otherwise, <c>false</c>.
    /// </value>
    bool Changed
    {
        get;
    }
    /// <summary>
    /// Gets the buffer.
    /// </summary>
    /// <value>
    /// The buffer.
    /// </value>
    IElementsBufferProxy? Buffer
    {
        get;
    }
    /// <summary>
    /// Attaches the buffer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="vertexBufferStartSlot">The vertex buffer slot. Returns the next available slot after binding</param>
    void AttachBuffer(DeviceContextProxy context, ref int vertexBufferStartSlot);
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    void Initialize();
}

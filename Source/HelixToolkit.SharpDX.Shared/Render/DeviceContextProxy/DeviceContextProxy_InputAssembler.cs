using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    public partial class DeviceContextProxy
    {
        private PrimitiveTopology currPrimitiveTopology = PrimitiveTopology.Undefined;
        /// <summary>
        /// Gets or sets the primitive topology.
        /// </summary>
        /// <value>
        /// The primitive topology.
        /// </value>
        public PrimitiveTopology PrimitiveTopology
        {
            set
            {
                if(currPrimitiveTopology == value)
                {
                    return;
                }
                currPrimitiveTopology = value;
                deviceContext.InputAssembler.PrimitiveTopology = value;
            }
            get
            {
                return currPrimitiveTopology;
            }
        }

        private InputLayout currInputLayout;
        /// <summary>
        /// Gets or sets the input layout.
        /// </summary>
        /// <value>
        /// The input layout.
        /// </value>
        public InputLayout InputLayout
        {
            set
            {
                if(currInputLayout == value) { return; }
                currInputLayout = value;
                deviceContext.InputAssembler.InputLayout = value;
            }
            get
            {
                return currInputLayout;
            }
        }

        #region set buffers 
        /// <summary>
        /// Bind an index buffer to the input-assembler stage.
        /// </summary>
        /// <param name="indexBufferRef">
        ///     A reference to an SharpDX.Direct3D11.Buffer object, that contains indices. The
        ///     index buffer must have been created with the SharpDX.Direct3D11.BindFlags.IndexBuffer
        ///     flag. 
        /// </param>
        /// <param name="format">
        ///     A SharpDX.DXGI.Format that specifies the format of the data in the index buffer.
        ///     The only formats allowed for index buffer data are 16-bit (SharpDX.DXGI.Format.R16_UInt)
        ///     and 32-bit (SharpDX.DXGI.Format.R32_UInt) integers.
        /// </param>
        /// <param name="offset">Offset (in bytes) from the start of the index buffer to the first index to use.</param>
        /// <remarks>
        ///     For information about creating index buffers, see How to: Create an Index Buffer.
        ///     Calling this method using a buffer that is currently bound for writing (i.e.
        ///     bound to the stream output pipeline stage) will effectively bind null instead
        ///     because a buffer cannot be bound as both an input and an output at the same time.
        ///     The debug layer will generate a warning whenever a resource is prevented from
        ///     being bound simultaneously as an input and an output, but this will not prevent
        ///     invalid data from being used by the runtime. The method will hold a reference
        ///     to the interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10. Windows?Phone?8: This API is supported.  
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(Buffer indexBufferRef, global::SharpDX.DXGI.Format format, int offset)
        {
            deviceContext.InputAssembler.SetIndexBuffer(indexBufferRef, format, offset);
        }

        /// <summary>
        /// Bind a single vertex buffer to the input-assembler stage.
        /// </summary>
        /// <param name="slot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level. 
        /// </param>
        /// <param name="vertexBufferBinding">
        ///     A SharpDX.Direct3D11.VertexBufferBinding. The vertex buffer must have been created
        ///     with the SharpDX.Direct3D11.BindFlags.VertexBuffer flag. 
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int slot, ref VertexBufferBinding vertexBufferBinding)
        {
            deviceContext.InputAssembler.SetVertexBuffers(slot, vertexBufferBinding);
        }

        /// <summary>
        /// Bind an array of vertex buffers to the input-assembler stage.
        /// </summary>
        /// <param name="firstSlot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level. 
        /// </param>
        /// <param name="vertexBufferBindings">
        ///     A reference to an array of SharpDX.Direct3D11.VertexBufferBinding. The vertex
        ///     buffers must have been created with the SharpDX.Direct3D11.BindFlags.VertexBuffer
        ///     flag. 
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int firstSlot, params VertexBufferBinding[] vertexBufferBindings)
        {
            deviceContext.InputAssembler.SetVertexBuffers(firstSlot, vertexBufferBindings);
        }

        /// <summary>
        /// Sets the vertex buffers.
        /// </summary>
        /// <param name="slot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level.
        /// </param>
        /// <param name="vertexBuffers">
        ///     A reference to an array of vertex buffers (see SharpDX.Direct3D11.Buffer). The
        ///     vertex buffers must have been created with the SharpDX.Direct3D11.BindFlags.VertexBuffer
        ///     flag.
        /// </param>
        /// <param name="stridesRef">
        ///     Pointer to an array of stride values; one stride value for each buffer in the
        ///     vertex-buffer array. Each stride is the size (in bytes) of the elements that
        ///     are to be used from that vertex buffer.
        /// </param>
        /// <param name="offsetsRef">
        ///     Pointer to an array of offset values; one offset value for each buffer in the
        ///     vertex-buffer array. Each offset is the number of bytes between the first element
        ///     of a vertex buffer and the first element that will be used.
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10.     
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int slot, Buffer[] vertexBuffers, int[] stridesRef, int[] offsetsRef)
        {
            deviceContext.InputAssembler.SetVertexBuffers(slot, vertexBuffers, stridesRef, offsetsRef);
        }

        #endregion
    }
}

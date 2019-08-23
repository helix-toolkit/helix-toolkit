using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

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
    namespace Render
    {
        public partial class DeviceContextProxy
        {
            #region DrawCall

            /// <summary>
            /// Draw non-indexed, non-instanced primitives.
            /// </summary>
            /// <param name="vertexCount">Number of vertices to draw.</param>
            /// <param name="startVertexLocation">Index of the first vertex, which is usually an offset in a vertex buffer.</param>
            /// <remarks>
            ///     Draw submits work to the rendering pipeline.The vertex data for a draw call normally
            ///    comes from a vertex buffer that is bound to the pipeline.Even without any vertex
            ///     buffer bound to the pipeline, you can generate your own vertex data in your vertex
            ///     shader by using the SV_VertexID system-value semantic to determine the current
            ///     vertex that the runtime is processing.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Draw(int vertexCount, int startVertexLocation)
            {
                ++NumberOfDrawCalls;
                deviceContext.Draw(vertexCount, startVertexLocation);
            }

            /// <summary>
            /// Draw geometry of an unknown size.
            /// </summary>
            /// <remarks>
            ///     A draw API submits work to the rendering pipeline. This API submits work of an
            ///     unknown size that was processed by the input assembler, vertex shader, and stream-output
            ///     stages; the work may or may not have gone through the geometry-shader stage.After
            ///     data has been streamed out to stream-output stage buffers, those buffers can
            ///     be again bound to the Input Assembler stage at input slot 0 and DrawAuto will
            ///     draw them without the application needing to know the amount of data that was
            ///     written to the buffers. A measurement of the amount of data written to the SO
            ///     stage buffers is maintained internally when the data is streamed out. This means
            ///     that the CPU does not need to fetch the measurement before re-binding the data
            ///     that was streamed as input data. Although this amount is tracked internally,
            ///     it is still the responsibility of applications to use input layouts to describe
            ///     the format of the data in the SO stage buffers so that the layouts are available
            ///     when the buffers are again bound to the input assembler.The following diagram
            ///     shows the DrawAuto process.Calling DrawAuto does not change the state of the
            ///     streaming-output buffers that were bound again as inputs.DrawAuto only works
            ///     when drawing with one input buffer bound as an input to the IA stage at slot
            ///     0. Applications must create the SO buffer resource with both binding flags, SharpDX.Direct3D11.BindFlags.VertexBuffer
            ///     and SharpDX.Direct3D11.BindFlags.StreamOutput.This API does not support indexing
            ///     or instancing.If an application needs to retrieve the size of the streaming-output
            ///     buffer, it can query for statistics on streaming output by using SharpDX.Direct3D11.QueryType.StreamOutputStatistics.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawAuto()
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawAuto();
            }

            /// <summary>
            /// Draw indexed, non-instanced primitives.
            /// </summary>
            /// <param name="indexCount">Number of indices to draw.</param>
            /// <param name="startIndexLocation">The location of the first index read by the GPU from the index buffer.</param>
            /// <param name="baseVertexLocation">A value added to each index before reading a vertex from the vertex buffer.</param>
            /// <remarks>
            ///     A draw API submits work to the rendering pipeline.If the sum of both indices
            ///     is negative, the result of the function call is undefined.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawIndexed(int indexCount, int startIndexLocation, int baseVertexLocation)
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
            }

            /// <summary>
            /// Draw indexed, instanced primitives.
            /// </summary>
            /// <param name="indexCountPerInstance"> Number of indices read from the index buffer for each instance.</param>
            /// <param name="instanceCount">Number of instances to draw.</param>
            /// <param name="startIndexLocation">The location of the first index read by the GPU from the index buffer.</param>
            /// <param name="baseVertexLocation">TA value added to each index before reading a vertex from the vertex buffer.</param>
            /// <param name="startInstanceLocation">A value added to each index before reading per-instance data from a vertex buffer.</param>
            /// <remarks>
            ///     A draw API submits work to the rendering pipeline.Instancing may extend performance
            ///     by reusing the same geometry to draw multiple objects in a scene. One example
            ///     of instancing could be to draw the same object with different positions and colors.
            ///     Instancing requires multiple vertex buffers: at least one for per-vertex data
            ///     and a second buffer for per-instance data.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawIndexedInstanced(int indexCountPerInstance, int instanceCount, int startIndexLocation, int baseVertexLocation, int startInstanceLocation)
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startIndexLocation);
            }

            /// <summary>
            /// Draw indexed, instanced, GPU-generated primitives.
            /// </summary>
            /// <param name="bufferForArgsRef">A reference to an SharpDX.Direct3D11.Buffer, which is a buffer containing the GPU generated primitives.</param>
            /// <param name="alignedByteOffsetForArgs">Offset in pBufferForArgs to the start of the GPU generated primitives.</param>
            /// <remarks>
            ///     When an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
            ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
            ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
            ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
            ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
            ///     in the pDesc parameter. Windows?Phone?8: This API is supported.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawIndexedInstancedIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawIndexedInstancedIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
            }

            /// <summary>
            /// Draw non-indexed, instanced primitives.
            /// </summary>
            /// <param name="vertexCountPerInstance">Number of vertices to draw.</param>
            /// <param name="instanceCount">Number of instances to draw.</param>
            /// <param name="startVertexLocation">Index of the first vertex.</param>
            /// <param name="startInstanceLocation">A value added to each index before reading per-instance data from a vertex buffer.</param>
            /// <remarks>
            ///     A draw API submits work to the rendering pipeline.Instancing may extend performance
            ///     by reusing the same geometry to draw multiple objects in a scene. One example
            ///     of instancing could be to draw the same object with different positions and colors.The
            ///     vertex data for an instanced draw call normally comes from a vertex buffer that
            ///     is bound to the pipeline. However, you could also provide the vertex data from
            ///     a shader that has instanced data identified with a system-value semantic (SV_InstanceID).
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawInstanced(int vertexCountPerInstance, int instanceCount, int startVertexLocation, int startInstanceLocation)
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawInstanced(vertexCountPerInstance, instanceCount, startVertexLocation, startInstanceLocation);
            }

            /// <summary>
            /// Draw instanced, GPU-generated primitives.
            /// </summary>
            /// <param name="bufferForArgsRef">A reference to an SharpDX.Direct3D11.Buffer, which is a buffer containing the GPU generated primitives.</param>
            /// <param name="alignedByteOffsetForArgs">Offset in pBufferForArgs to the start of the GPU generated primitives.</param>
            /// <remarks>
            ///     When an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
            ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
            ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
            ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
            ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
            ///     in the pDesc parameter.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DrawInstancedIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
            {
                ++NumberOfDrawCalls;
                deviceContext.DrawInstancedIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
            }

            #endregion DrawCall

            #region Dispatch

            /// <summary>
            /// Execute a command list from a thread group.
            /// </summary>
            /// <param name="threadGroupCountX">
            ///     The number of groups dispatched in the x direction. ThreadGroupCountX must be
            ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
            ///     (65535).
            ///  </param>
            ///  <param name="threadGroupCountY">
            ///     The number of groups dispatched in the y direction. ThreadGroupCountY must be
            ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
            ///     (65535).
            ///  </param>
            ///  <param name="threadGroupCountZ">
            ///     The number of groups dispatched in the z direction. ThreadGroupCountZ must be
            ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
            ///     (65535). In feature level 10 the value for ThreadGroupCountZ must be 1.
            ///  </param>
            ///  <remarks>
            ///     You call the Dispatch method to execute commands in a compute shader. A compute
            ///     shader can be run on many threads in parallel, within a thread group. Index a
            ///     particular thread, within a thread group using a 3D vector given by (x,y,z).In
            ///     the following illustration, assume a thread group with 50 threads where the size
            ///     of the group is given by (5,5,2). A single thread is identified from a thread
            ///     group with 50 threads in it, using the vector (4,1,1).The following illustration
            ///     shows the relationship between the parameters passed to SharpDX.Direct3D11.DeviceContext.Dispatch(System.Int32,System.Int32,System.Int32),
            ///     Dispatch(5,3,2), the values specified in the numthreads attribute, numthreads(10,8,3),
            ///     and values that will passed to the compute shader for the thread-related system
            ///     values (SV_GroupIndex,SV_DispatchThreadID,SV_GroupThreadID,SV_GroupID).
            ///  </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispatch(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
            {
                ++NumberOfDrawCalls;
                deviceContext.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
            }

            /// <summary>
            /// Execute a command list over one or more thread groups.
            /// </summary>
            /// <param name="bufferForArgsRef">
            ///     A reference to an SharpDX.Direct3D11.Buffer, which must be loaded with data that
            ///     matches the argument list for SharpDX.Direct3D11.DeviceContext.Dispatch(System.Int32,System.Int32,System.Int32).
            /// </param>
            /// <param name="alignedByteOffsetForArgs">A byte-aligned offset between the start of the buffer and the arguments.</param>
            /// <remarks>
            ///     You call the DispatchIndirect method to execute commands in a compute shader.When
            ///     an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
            ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
            ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
            ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
            ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
            ///     in the pDesc parameter.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DispatchIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
            {
                ++NumberOfDrawCalls;
                deviceContext.DispatchIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
            }

            #endregion Dispatch

            #region CommandList

            /// <summary>
            /// Create a command list and record graphics commands into it.
            /// </summary>
            /// <param name="restoreState">
            /// A flag indicating whether the immediate context state is saved (prior) and restored
            /// (after) the execution of a command list.
            /// </param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CommandList FinishCommandList(bool restoreState)
            {
                return deviceContext.FinishCommandList(restoreState);
            }

            /// <summary>
            /// Queues commands from a command list onto a device.
            /// </summary>
            /// <param name="commandListRef">The command list reference.</param>
            /// <param name="restoreContextState">
            ///     A Boolean flag that determines whether the target context state is saved prior
            ///     to and restored after the execution of a command list. Use TRUE to indicate that
            ///     the runtime needs to save and restore the state. Use SharpDX.Result.False to
            ///     indicate that no state shall be saved or restored, which causes the target context
            ///     to return to its default state after the command list executes. Applications
            ///     should typically use SharpDX.Result.False unless they will restore the state
            ///     to be nearly equivalent to the state that the runtime would restore if TRUE were
            ///     passed. When applications use SharpDX.Result.False, they can avoid unnecessary
            ///     and inefficient state transitions.
            /// </param>
            /// <remarks>
            ///     Use this method to play back a command list that was recorded by a deferred context
            ///     on any thread. A call to ExecuteCommandList of a command list from a deferred
            ///     context onto the immediate context is required for the recorded commands to be
            ///     executed on the graphics processing unit (GPU). A call to ExecuteCommandList
            ///     of a command list from a deferred context onto another deferred context can be
            ///     used to merge recorded lists. But to run the commands from the merged deferred
            ///     command list on the GPU, you need to execute them on the immediate context. This
            ///     method performs some runtime validation related to queries. Queries that are
            ///     begun in a device context cannot be manipulated indirectly by executing a command
            ///     list (that is, Begin or End was invoked against the same query by the deferred
            ///     context which generated the command list). If such a condition occurs, the ExecuteCommandList
            ///     method does not execute the command list. However, the state of the device context
            ///     is still maintained, as would be expected (SharpDX.Direct3D11.DeviceContext.ClearState
            ///     is performed, unless the application indicates to preserve the device context
            ///     state). Windows?Phone?8: This API is supported.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ExecuteCommandList(CommandList commandListRef, bool restoreContextState)
            {
                deviceContext.ExecuteCommandList(commandListRef, restoreContextState);
            }

            #endregion CommandList

            /// <summary>
            ///     Sends queued-up commands in the command buffer to the graphics processing unit
            ///     (GPU).
            /// </summary>
            /// <remarks>
            ///     Most applications don't need to call this method. If an application calls this
            ///     method when not necessary, it incurs a performance penalty. Each call to Flush
            ///     incurs a significant amount of overhead.When Microsoft Direct3D state-setting,
            ///     present, or draw commands are called by an application, those commands are queued
            ///     into an internal command buffer. Flush sends those commands to the GPU for processing.
            ///     Typically, the Direct3D runtime sends these commands to the GPU automatically
            ///     whenever the runtime determines that they need to be sent, such as when the command
            ///     buffer is full or when an application maps a resource. Flush sends the commands
            ///     manually.We recommend that you use Flush when the CPU waits for an arbitrary
            ///     amount of time (such as when you call the Sleep function).Because Flush operates
            ///     asynchronously, it can return either before or after the GPU finishes executing
            ///     the queued graphics commands. However, the graphics commands eventually always
            ///     complete. You can call the SharpDX.Direct3D11.Device.CreateQuery(SharpDX.Direct3D11.QueryDescription,SharpDX.Direct3D11.Query)
            ///     method with the SharpDX.Direct3D11.QueryType.Event value to create an event query;
            ///     you can then use that event query in a call to the SharpDX.Direct3D11.DeviceContext.GetDataInternal(SharpDX.Direct3D11.Asynchronous,System.IntPtr,System.Int32,SharpDX.Direct3D11.AsynchronousFlags)
            ///     method to determine when the GPU is finished processing the graphics commands.
            ///     Microsoft Direct3D?11 defers the destruction of objects. Therefore, an application
            ///     can't rely upon objects immediately being destroyed. By calling Flush, you destroy
            ///     any objects whose destruction was deferred. If an application requires synchronous
            ///     destruction of an object, we recommend that the application release all its references,
            ///     call SharpDX.Direct3D11.DeviceContext.ClearState, and then call Flush.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Flush()
            {
                deviceContext.Flush();
            }

            /// <summary>
            ///
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int ResetDrawCalls()
            {
                int total = NumberOfDrawCalls;
                NumberOfDrawCalls = 0;
                return total;
            }
        }
    }

}

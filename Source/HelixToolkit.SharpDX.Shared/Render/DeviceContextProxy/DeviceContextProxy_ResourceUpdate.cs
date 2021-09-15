using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using SharpDX;

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
            #region Map/Unmap resources

            /// <summary>
            ///     Maps the data contained in a subresource to a memory pointer, and denies the
            ///     GPU access to that subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="mipSlice">The mip slice.</param>
            /// <param name="arraySlice">The array slice.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="stream">The stream.</param>
            /// <returns>The locked SharpDX.DataBox</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Texture2D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
            {
                return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
            }

            /// <summary>
            ///     Maps the data contained in a subresource to a memory pointer, and denies the
            ///     GPU access to that subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="mipSlice">The mip slice.</param>
            /// <param name="arraySlice">The array slice.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="stream">The stream.</param>
            /// <returns>The locked SharpDX.DataBox   </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Texture1D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
            {
                return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
            }

            /// <summary>
            ///     Maps the data contained in a subresource to a memory pointer, and denies the
            ///     GPU access to that subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="mipSlice">The mip slice.</param>
            /// <param name="arraySlice">The array slice.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="stream">The stream.</param>
            /// <returns>The locked SharpDX.DataBox</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Texture3D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
            {
                return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
            }

            /// <summary>
            ///     Maps the data contained in a subresource to a memory pointer, and denies the
            ///     GPU access to that subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="stream">The stream.</param>
            /// <returns>The locked SharpDX.DataBox      </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Buffer resource, MapMode mode, MapFlags flags, out DataStream stream)
            {
                return deviceContext.MapSubresource(resource, mode, flags, out stream);
            }

            /// <summary>
            /// Maps the subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="mipSlice">The mip slice.</param>
            /// <param name="arraySlice">The array slice.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="mipSizeOut">Size of the mip.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Resource resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out int mipSizeOut)
            {
                return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out mipSizeOut);
            }

            /// <summary>
            /// Maps the subresource.
            /// </summary>
            /// <param name="resource">The resource.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="mode">The mode.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="stream">The stream.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Resource resource, int subresource, MapMode mode, MapFlags flags, out DataStream stream)
            {
                return deviceContext.MapSubresource(resource, subresource, mode, flags, out stream);
            }

            /// <summary>
            /// Maps the subresource.
            /// </summary>
            /// <param name="resourceRef">The resource reference.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="mapType">Type of the map.</param>
            /// <param name="mapFlags">The map flags.</param>
            /// <returns></returns>
            /// <remarks>
            ///     If you call Map on a deferred context, you can only pass SharpDX.Direct3D11.MapMode.WriteDiscard,
            ///     SharpDX.Direct3D11.MapMode.WriteNoOverwrite, or both to the MapType parameter.
            ///     Other SharpDX.Direct3D11.MapMode-typed values are not supported for a deferred
            ///     context.The Direct3D 11.1 runtime, which is available starting with Windows Developer
            ///     Preview, can map shader resource views (SRVs) of dynamic buffers with SharpDX.Direct3D11.MapMode.WriteNoOverwrite.
            ///     The Direct3D 11 and earlier runtimes limited mapping to vertex or index buffers.
            ///     If SharpDX.Direct3D11.MapFlags.DoNotWait is used and the resource is still being
            ///     used by the GPU, this method return an empty DataBox whose property SharpDX.DataBox.IsEmpty
            ///     returns true.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox MapSubresource(Resource resourceRef, int subresource, MapMode mapType, MapFlags mapFlags)
            {
                return deviceContext.MapSubresource(resourceRef, subresource, mapType, mapFlags);
            }

            /// <summary>
            /// Invalidate the reference to a resource and reenable the GPU's access to that resource.
            /// </summary>
            /// <param name="resourceRef">The resource reference.</param>
            /// <param name="subresource">The subresource.</param>
            /// <remarks>
            ///     For info about how to use Unmap, see How to: Use dynamic resources. Windows?Phone?8:
            ///     This API is supported.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnmapSubresource(Resource resourceRef, int subresource)
            {
                deviceContext.UnmapSubresource(resourceRef, subresource);
            }

            /// <summary>
            /// Copy a multisampled resource into a non-multisampled resource.
            /// </summary>
            /// <param name="source">Source resource. Must be multisampled.</param>
            /// <param name="sourceSubresource">The source subresource.</param>
            /// <param name="destination">
            ///     Destination resource. Must be a created with the SharpDX.Direct3D11.ResourceUsage.Default
            ///     flag and be single-sampled. See SharpDX.Direct3D11.Resource.
            /// </param>
            /// <param name="destinationSubresource">A zero-based index, that identifies the destination subresource. Use {{D3D11CalcSubresource}} to calculate the index.</param>
            /// <param name="format"> A SharpDX.DXGI.Format that indicates how the multisampled resource will be resolved to a single-sampled resource. See remarks.</param>
            /// <remarks>
            ///     This API is most useful when re-using the resulting render target of one render
            ///     pass as an input to a second render pass. The source and destination resources
            ///     must be the same resource type and have the same dimensions. In addition, they
            ///     must have compatible formats. There are three scenarios for this: ScenarioRequirements
            ///     Source and destination are prestructured and typedBoth the source and destination
            ///     must have identical formats and that format must be specified in the Format parameter.
            ///     One resource is prestructured and typed and the other is prestructured and typelessThe
            ///     typed resource must have a format that is compatible with the typeless resource
            ///     (i.e. the typed resource is DXGI_FORMAT_R32_FLOAT and the typeless resource is
            ///     DXGI_FORMAT_R32_TYPELESS). The format of the typed resource must be specified
            ///     in the Format parameter. Source and destination are prestructured and typelessBoth
            ///     the source and destination must have the same typeless format (i.e. both must
            ///     have DXGI_FORMAT_R32_TYPELESS), and the Format parameter must specify a format
            ///     that is compatible with the source and destination (i.e. if both are DXGI_FORMAT_R32_TYPELESS
            ///     then DXGI_FORMAT_R32_FLOAT could be specified in the Format parameter). For example,
            ///     given the DXGI_FORMAT_R16G16B16A16_TYPELESS format: The source (or dest) format
            ///     could be DXGI_FORMAT_R16G16B16A16_UNORM The dest (or source) format could be
            ///     DXGI_FORMAT_R16G16B16A16_FLOAT ?
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResolveSubresource(Resource source, int sourceSubresource, Resource destination, int destinationSubresource, global::SharpDX.DXGI.Format format)
            {
                deviceContext.ResolveSubresource(source, sourceSubresource, destination, destinationSubresource, format);
            }

            #endregion Map/Unmap resources

            #region Upload sub resources

            /// <summary>
            /// The CPU copies data from memory to a subresource created in non-mappable memory.
            /// </summary>
            /// <param name="dstResourceRef">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
            /// <param name="dstSubresource">A zero-based index, that identifies the destination subresource. See D3D11CalcSubresource for more details.</param>
            /// <param name="dstBoxRef">
            ///     A reference to a box that defines the portion of the destination subresource
            ///     to copy the resource data into. Coordinates are in bytes for buffers and in texels
            ///     for textures. If null, the data is written to the destination subresource with
            ///     no offset. The dimensions of the source must fit the destination (see SharpDX.Direct3D11.ResourceRegion).
            ///     An empty box results in a no-op. A box is empty if the top value is greater than
            ///     or equal to the bottom value, or the left value is greater than or equal to the
            ///     right value, or the front value is greater than or equal to the back value. When
            ///     the box is empty, UpdateSubresource doesn't perform an update operation.
            /// </param>
            /// <param name="srcDataRef">A reference to the source data in memory.</param>
            /// <param name="srcRowPitch">The size of one row of the source data.</param>
            /// <param name="srcDepthPitch">The size of one depth slice of source data.</param>
            /// <remarks>
            /// Remarks:
            ///     For a shader-constant buffer; set pDstBox to null. It is not possible to use
            ///     this method to partially update a shader-constant buffer.A resource cannot be
            ///     used as a destination if: the resource is created with immutable or dynamic usage.
            ///     the resource is created as a depth-stencil resource. the resource is created
            ///     with multisampling capability (see SharpDX.DXGI.SampleDescription). When UpdateSubresource
            ///     returns, the application is free to change or even free the data pointed to by
            ///     pSrcData because the method has already copied/snapped away the original contents.The
            ///     performance of UpdateSubresource depends on whether or not there is contention
            ///     for the destination resource. For example, contention for a vertex buffer resource
            ///     occurs when the application executes a Draw call and later calls UpdateSubresource
            ///     on the same vertex buffer before the Draw call is actually executed by the GPU.
            ///     When there is contention for the resource, UpdateSubresource will perform 2 copies
            ///     of the source data. First, the data is copied by the CPU to a temporary storage
            ///     space accessible by the command buffer. This copy happens before the method returns.
            ///     A second copy is then performed by the GPU to copy the source data into non-mappable
            ///     memory. This second copy happens asynchronously because it is executed by GPU
            ///     when the command buffer is flushed. When there is no resource contention, the
            ///     behavior of UpdateSubresource is dependent on which is faster (from the CPU's
            ///     perspective): copying the data to the command buffer and then having a second
            ///     copy execute when the command buffer is flushed, or having the CPU copy the data
            ///     to the final resource location. This is dependent on the architecture of the
            ///     underlying system. Note??Applies only to feature level 9_x hardware If you use
            ///     UpdateSubresource or SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion})
            ///     to copy from a staging resource to a default resource, you can corrupt the destination
            ///     contents. This occurs if you pass a null source box and if the source resource
            ///     has different dimensions from those of the destination resource or if you use
            ///     destination offsets, (x, y, and z). In this situation, always pass a source box
            ///     that is the full size of the source resource.?To better understand the source
            ///     row pitch and source depth pitch parameters, the following illustration shows
            ///     a 3D volume texture.Each block in this visual represents an element of data,
            ///     and the size of each element is dependent on the resource's format. For example,
            ///     if the resource format is SharpDX.DXGI.Format.R32G32B32A32_Float, the size of
            ///     each element would be 128 bits, or 16 bytes. This 3D volume texture has a width
            ///     of two, a height of three, and a depth of four.To calculate the source row pitch
            ///     and source depth pitch for a given resource, use the following formulas: Source
            ///     Row Pitch = [size of one element in bytes] * [number of elements in one row]
            ///     Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] In the case
            ///     of this example 3D volume texture where the size of each element is 16 bytes,
            ///     the formulas are as follows: Source Row Pitch = 16 * 2 = 32 Source Depth Pitch
            ///     = 16 * 2 * 3 = 96 The following illustration shows the resource as it is laid
            ///     out in memory.For example, the following code snippet shows how to specify a
            ///     destination region in a 2D texture. Assume the destination texture is 512x512
            ///     and the operation will copy the data pointed to by pData to [(120,100)..(200,220)]
            ///     in the destination texture. Also assume that rowPitch has been initialized with
            ///     the proper value (as explained above). front and back are set to 0 and 1 respectively,
            ///     because by having front equal to back, the box is technically empty. SharpDX.Direct3D11.ResourceRegion
            ///     destRegion; destRegion.left = 120; destRegion.right = 200; destRegion.top = 100;
            ///     destRegion.bottom = 220; destRegion.front = 0; destRegion.back = 1; pd3dDeviceContext->UpdateSubresource(
            ///     pDestTexture, 0, destRegion, pData, rowPitch, 0 ); The 1D case is similar. The
            ///     following snippet shows how to specify a destination region in a 1D texture.
            ///     Use the same assumptions as above, except that the texture is 512 in length.
            ///     SharpDX.Direct3D11.ResourceRegion destRegion; destRegion.left = 120; destRegion.right
            ///     = 200; destRegion.top = 0; destRegion.bottom = 1; destRegion.front = 0; destRegion.back
            ///     = 1; pd3dDeviceContext->UpdateSubresource( pDestTexture, 0, destRegion, pData,
            ///     rowPitch, 0 ); For info about various resource types and how UpdateSubresource
            ///     might work with each resource type, see Introduction to a Resource in Direct3D
            ///     11.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresource(Resource dstResourceRef, int dstSubresource, ResourceRegion? dstBoxRef, System.IntPtr srcDataRef, int srcRowPitch, int srcDepthPitch)
            {
                deviceContext.UpdateSubresource(dstResourceRef, dstSubresource, dstBoxRef, srcDataRef, srcRowPitch, srcDepthPitch);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="region">The region.</param>
            /// <remarks>This method is implementing the workaround for deferred context.</remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresource(DataBox source, Resource resource, int subresource, ref ResourceRegion region)
            {
                deviceContext.UpdateSubresource(source, resource, subresource, region);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="subresource">The subresource.</param>
            /// <remarks>This method is implementing the workaround for deferred context.       </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresource(DataBox source, Resource resource, int subresource = 0)
            {
                deviceContext.UpdateSubresource(source, resource, subresource);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data">The data.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="rowPitch">The row pitch.</param>
            /// <param name="depthPitch">The depth pitch.</param>
            /// <param name="region">
            /// A region that defines the portion of the destination subresource to copy the
            /// resource data into. Coordinates are in bytes for buffers and in texels for textures.
            /// </param>
            /// <remarks>This method is implementing the workaround for deferred context.     </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresource<T>(T[] data, Resource resource, int subresource = 0, int rowPitch = 0, int depthPitch = 0, ResourceRegion? region = null) where T : struct
            {
                deviceContext.UpdateSubresource(data, resource, subresource, rowPitch, depthPitch, region);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data">The data.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="rowPitch">The row pitch.</param>
            /// <param name="depthPitch">The depth pitch.</param>
            /// <param name="region">The region.</param>
            /// <remarks>This method is implementing the workaround for deferred context.        </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresource<T>(ref T data, Resource resource, int subresource = 0, int rowPitch = 0, int depthPitch = 0, ResourceRegion? region = null) where T : struct
            {
                deviceContext.UpdateSubresource(ref data, resource, subresource, rowPitch, depthPitch, region);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
            /// <remarks>This method is implementing the workaround for deferred context. </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresourceSafe(ref DataBox source, Resource resource, int srcBytesPerElement, int subresource = 0, bool isCompressedResource = false)
            {
                deviceContext.UpdateSubresourceSafe(source, resource, srcBytesPerElement, subresource, isCompressedResource);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="region">The region.</param>
            /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
            /// <remarks>This method is implementing the workaround for deferred context.    </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresourceSafe(ref DataBox source, Resource resource, int srcBytesPerElement, int subresource, ResourceRegion region, bool isCompressedResource = false)
            {
                deviceContext.UpdateSubresourceSafe(source, resource, srcBytesPerElement, subresource, region, isCompressedResource);
            }

            /// <summary>
            /// Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data">The data.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="rowPitch">The row pitch.</param>
            /// <param name="depthPitch">The depth pitch.</param>
            /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
            /// <remarks>This method is implementing the workaround for deferred context.       </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresourceSafe<T>(T[] data, Resource resource, int srcBytesPerElement, int subresource = 0, int rowPitch = 0, int depthPitch = 0,
                bool isCompressedResource = false) where T : struct
            {
                deviceContext.UpdateSubresourceSafe(data, resource, srcBytesPerElement, subresource, rowPitch, depthPitch);
            }

            /// <summary>
            ///  Copies data from the CPU to to a non-mappable subresource region.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data">The data.</param>
            /// <param name="resource">The resource.</param>
            /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
            /// <param name="subresource">The subresource.</param>
            /// <param name="rowPitch">The row pitch.</param>
            /// <param name="depthPitch">The depth pitch.</param>
            /// <param name="isCompressedResource">if set to <c>true</c> [is compressed resource].</param>
            /// <remarks>This method is implementing the workaround for deferred context.  </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpdateSubresourceSafe<T>(ref T data, Resource resource, int srcBytesPerElement, int subresource = 0, int rowPitch = 0, int depthPitch = 0,
                bool isCompressedResource = false) where T : struct
            {
                deviceContext.UpdateSubresourceSafe(ref data, resource, srcBytesPerElement, subresource, rowPitch, depthPitch, isCompressedResource);
            }

            #endregion Upload sub resources

            #region Copy Resources

            /// <summary>
            ///     Copy the entire contents of the source resource to the destination resource using
            ///     the GPU.
            /// </summary>
            /// <param name="source">A reference to the source resource (see SharpDX.Direct3D11.Resource).</param>
            /// <param name="destination">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
            /// <remarks>
            ///     This method is unusual in that it causes the GPU to perform the copy operation
            ///     (similar to a memcpy by the CPU). As a result, it has a few restrictions designed
            ///     for improving performance. For instance, the source and destination resources:
            ///     Must be different resources. Must be the same type. Must have identical dimensions
            ///     (including width, height, depth, and size as appropriate). Will only be copied.
            ///     CopyResource does not support any stretch, color key, blend, or format conversions.
            ///     Must have compatible DXGI formats, which means the formats must be identical
            ///     or at least from the same type group. For example, a DXGI_FORMAT_R32G32B32_FLOAT
            ///     texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of
            ///     these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. Might not be currently
            ///     mapped. You cannot use an {{Immutable}} resource as a destination. You can use
            ///     a {{depth-stencil}} resource as either a source or a destination. Resources created
            ///     with multisampling capability (see SharpDX.DXGI.SampleDescription) can be used
            ///     as source and destination only if both source and destination have identical
            ///     multisampled count and quality. If source and destination differ in multisampled
            ///     count and quality or if one is multisampled and the other is not multisampled
            ///     the call to ID3D11DeviceContext::CopyResource fails. The method is an asynchronous
            ///     call which may be added to the command-buffer queue. This attempts to remove
            ///     pipeline stalls that may occur when copying data. An application that only needs
            ///     to copy a portion of the data in a resource should use SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion})
            ///     instead.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void CopyResource(Resource source, Resource destination)
            {
                deviceContext.CopyResource(source, destination);
            }

            /// <summary>
            /// Copies data from a buffer holding variable length data.
            /// </summary>
            /// <param name="dstBufferRef">
            ///     Pointer to SharpDX.Direct3D11.Buffer. This can be any buffer resource that other
            ///     copy commands, such as SharpDX.Direct3D11.DeviceContext.CopyResource_(SharpDX.Direct3D11.Resource,SharpDX.Direct3D11.Resource)
            ///     or SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion}),
            ///     are able to write to.
            /// </param>
            /// <param name="dstAlignedByteOffset">
            ///     Offset from the start of pDstBuffer to write 32-bit UINT structure (vertex) count
            ///     from pSrcView.
            /// </param>
            /// <param name="srcViewRef">
            ///     Pointer to an SharpDX.Direct3D11.UnorderedAccessView of a Structured Buffer resource
            ///     created with either SharpDX.Direct3D11.UnorderedAccessViewBufferFlags.Append
            ///     or SharpDX.Direct3D11.UnorderedAccessViewBufferFlags.Counter specified when the
            ///     UAV was created. These types of resources have hidden counters tracking "how
            ///     many" records have been written.
            /// </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void CopyStructureCount(Buffer dstBufferRef, int dstAlignedByteOffset, UnorderedAccessView srcViewRef)
            {
                deviceContext.CopyStructureCount(dstBufferRef, dstAlignedByteOffset, srcViewRef);
            }

            /// <summary>
            /// Copy a region from a source resource to a destination resource.
            /// </summary>
            /// <param name="source">A reference to the source resource (see SharpDX.Direct3D11.Resource).</param>
            /// <param name="sourceSubresource">Source subresource index.</param>
            /// <param name="sourceRegion">
            ///     A reference to a 3D box (see SharpDX.Direct3D11.ResourceRegion) that defines
            ///     the source subresources that can be copied. If NULL, the entire source subresource
            ///     is copied. The box must fit within the source resource.
            /// </param>
            /// <param name="destination">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
            /// <param name="destinationSubResource">Destination subresource index.</param>
            /// <param name="dstX">The x-coordinate of the upper left corner of the destination region.</param>
            /// <param name="dstY">
            ///     The y-coordinate of the upper left corner of the destination region. For a 1D
            ///     subresource, this must be zero.
            /// </param>
            /// <param name="dstZ">
            ///     The z-coordinate of the upper left corner of the destination region. For a 1D
            ///     or 2D subresource, this must be zero.
            /// </param>
            /// <remarks>
            ///     The source box must be within the size of the source resource. The destination
            ///     offsets, (x, y, and z) allow the source box to be offset when writing into the
            ///     destination resource; however, the dimensions of the source box and the offsets
            ///     must be within the size of the resource. If the resources are buffers, all coordinates
            ///     are in bytes; if the resources are textures, all coordinates are in texels. {{D3D11CalcSubresource}}
            ///     is a helper function for calculating subresource indexes. CopySubresourceRegion
            ///     performs the copy on the GPU (similar to a memcpy by the CPU). As a consequence,
            ///     the source and destination resources: Must be different subresources (although
            ///     they can be from the same resource). Must be the same type. Must have compatible
            ///     DXGI formats (identical or from the same type group). For example, a DXGI_FORMAT_R32G32B32_FLOAT
            ///     texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of
            ///     these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. May not be currently
            ///     mapped. CopySubresourceRegion only supports copy; it does not support any stretch,
            ///     color key, blend, or format conversions. An application that needs to copy an
            ///     entire resource should use SharpDX.Direct3D11.DeviceContext.CopyResource_(SharpDX.Direct3D11.Resource,SharpDX.Direct3D11.Resource)
            ///     instead. CopySubresourceRegion is an asynchronous call which may be added to
            ///     the command-buffer queue, this attempts to remove pipeline stalls that may occur
            ///     when copying data. See performance considerations for more details. Note??If
            ///     you use CopySubresourceRegion with a depth-stencil buffer or a multisampled resource,
            ///     you must copy the whole subresource. In this situation, you must pass 0 to the
            ///     DstX, DstY, and DstZ parameters and NULL to the pSrcBox parameter. In addition,
            ///     source and destination resources, which are represented by the pSrcResource and
            ///     pDstResource parameters, should have identical sample count values. Example The
            ///     following code snippet copies a box (located at (120,100),(200,220)) from a source
            ///     texture into a region (10,20),(90,140) in a destination texture. D3D11_BOX sourceRegion;
            ///     sourceRegion.left = 120; sourceRegion.right = 200; sourceRegion.top = 100; sourceRegion.bottom
            ///     = 220; sourceRegion.front = 0; sourceRegion.back = 1; pd3dDeviceContext->CopySubresourceRegion(
            ///     pDestTexture, 0, 10, 20, 0, pSourceTexture, 0, sourceRegion ); Notice, that
            ///     for a 2D texture, front and back are set to 0 and 1 respectively.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void CopySubresourceRegion(Resource source, int sourceSubresource, ResourceRegion? sourceRegion, Resource destination,
                int destinationSubResource, int dstX = 0, int dstY = 0, int dstZ = 0)
            {
                deviceContext.CopySubresourceRegion(source, sourceSubresource, sourceRegion, destination, destinationSubResource, dstX, dstY, dstZ);
            }

            #endregion Copy Resources

            /// <summary>
            /// Generates mipmaps for the given shader resource.
            /// </summary>
            /// <param name="shaderResourceViewRef">The shader resource view reference.</param>
            /// <remarks>
            ///     You can call GenerateMips on any shader-resource view to generate the lower mipmap
            ///     levels for the shader resource. GenerateMips uses the largest mipmap level of
            ///     the view to recursively generate the lower levels of the mip and stops with the
            ///     smallest level that is specified by the view. If the base resource wasn't created
            ///     with SharpDX.Direct3D11.BindFlags.RenderTarget, SharpDX.Direct3D11.BindFlags.ShaderResource,
            ///     and SharpDX.Direct3D11.ResourceOptionFlags.GenerateMipMaps, the call to GenerateMips
            ///     has no effect.Feature levels 9.1, 9.2, and 9.3 can't support automatic generation
            ///     of mipmaps for 3D (volume) textures.Video adapters that support feature level
            ///     9.1 and higher support generating mipmaps if you use any of these formats: SharpDX.DXGI.Format.R8G8B8A8_UNorm
            ///     SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb SharpDX.DXGI.Format.B5G6R5_UNorm SharpDX.DXGI.Format.B8G8R8A8_UNorm
            ///     SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb SharpDX.DXGI.Format.B8G8R8X8_UNorm SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb
            ///     Video adapters that support feature level 9.2 and higher support generating mipmaps
            ///     if you use any of these formats in addition to any of the formats for feature
            ///     level 9.1: SharpDX.DXGI.Format.R16G16B16A16_Float SharpDX.DXGI.Format.R16G16B16A16_UNorm
            ///     SharpDX.DXGI.Format.R16G16_Float SharpDX.DXGI.Format.R16G16_UNorm SharpDX.DXGI.Format.R32_Float
            ///     Video adapters that support feature level 9.3 and higher support generating mipmaps
            ///     if you use any of these formats in addition to any of the formats for feature
            ///     levels 9.1 and 9.2: SharpDX.DXGI.Format.R32G32B32A32_Float DXGI_FORMAT_B4G4R4A4
            ///     (optional) Video adapters that support feature level 10 and higher support generating
            ///     mipmaps if you use any of these formats in addition to any of the formats for
            ///     feature levels 9.1, 9.2, and 9.3: SharpDX.DXGI.Format.R32G32B32_Float (optional)
            ///     SharpDX.DXGI.Format.R16G16B16A16_SNorm SharpDX.DXGI.Format.R32G32_Float SharpDX.DXGI.Format.R10G10B10A2_UNorm
            ///     SharpDX.DXGI.Format.R11G11B10_Float SharpDX.DXGI.Format.R8G8B8A8_SNorm SharpDX.DXGI.Format.R16G16_SNorm
            ///     SharpDX.DXGI.Format.R8G8_UNorm SharpDX.DXGI.Format.R8G8_SNorm SharpDX.DXGI.Format.R16_Float
            ///     SharpDX.DXGI.Format.R16_UNorm SharpDX.DXGI.Format.R16_SNorm SharpDX.DXGI.Format.R8_UNorm
            ///     SharpDX.DXGI.Format.R8_SNorm SharpDX.DXGI.Format.A8_UNorm SharpDX.DXGI.Format.B5G5R5A1_UNorm
            ///     (optional) For all other unsupported formats, GenerateMips will silently fail.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GenerateMips(ShaderResourceView shaderResourceViewRef)
            {
                deviceContext.GenerateMips(shaderResourceViewRef);
            }
        }
    }

}

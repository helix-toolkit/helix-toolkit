/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using System;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContextProxy = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
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
        using Core2D;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public abstract class DX11RenderBufferProxyBase : DisposeObject
        {
            /// <summary>
            /// Occurs when [on new buffer created].
            /// </summary>
            public event EventHandler<Texture2DArgs> OnNewBufferCreated;
            /// <summary>
            /// Occurs when [on device lost].
            /// </summary>
            public event EventHandler<EventArgs> DeviceLost;
            /// <summary>
            /// The color buffer
            /// </summary>
            private ShaderResourceViewProxy colorBuffer;
            public ShaderResourceViewProxy ColorBuffer
            {
                get
                {
                    return colorBuffer;
                }
            }
            /// <summary>
            /// The back buffer
            /// </summary>
            private ShaderResourceViewProxy backBuffer;
            public ShaderResourceViewProxy BackBuffer
            {
                get
                {
                    return backBuffer;
                }
            }
            /// <summary>
            /// The depth stencil buffer
            /// </summary>
            private ShaderResourceViewProxy depthStencilBuffer;
            /// <summary>
            /// The depth stencil buffer
            /// </summary>
            public ShaderResourceViewProxy DepthStencilBuffer
            {
                get
                {
                    return depthStencilBuffer;
                }
            }            
            
            /// <summary>
            /// The depth stencil buffer
            /// </summary>
            private ShaderResourceViewProxy depthStencilBufferNoMSAA;
            /// <summary>
            /// The depth stencil buffer
            /// </summary>
            public ShaderResourceViewProxy DepthStencilBufferNoMSAA
            {
                get
                {
                    return depthStencilBufferNoMSAA;
                }
            }
            /// <summary>
            /// The D2D controls
            /// </summary>
            protected D2DTargetProxy d2dTarget;
            /// <summary>
            /// Gets the d2 d controls.
            /// </summary>
            /// <value>
            /// The d2 d controls.
            /// </value>
            public D2DTargetProxy D2DTarget
            {
                get
                {
                    return d2dTarget;
                }
            }
            /// <summary>
            /// Gets or sets the width of the target.
            /// </summary>
            /// <value>
            /// The width of the target.
            /// </value>
            public int TargetWidth
            {
                private set; get;
            }
            /// <summary>
            /// Gets or sets the height of the target.
            /// </summary>
            /// <value>
            /// The height of the target.
            /// </value>
            public int TargetHeight
            {
                private set; get;
            }
            private IDeviceContextPool deviceContextPool;
            /// <summary>
            /// Gets the device context pool.
            /// </summary>
            /// <value>
            /// The device context pool.
            /// </value>
            public IDeviceContextPool DeviceContextPool => deviceContextPool;
            #region Offscreen Texture Pools
            private PingPongColorBuffers fullResPPBuffer;
            public PingPongColorBuffers FullResPPBuffer
            {
                get
                {
                    return fullResPPBuffer;
                }
            }

            private TexturePool fullResDepthStencilPool;
            public TexturePool FullResDepthStencilPool
            {
                get
                {
                    return fullResDepthStencilPool;
                }
            }

            private TexturePool fullResRenderTargetPool;
            public TexturePool FullResRenderTargetPool
            {
                get
                {
                    return fullResRenderTargetPool;
                }
            }

            private TexturePool halfResDepthStencilPool;
            public TexturePool HalfResDepthStencilPool
            {
                get
                {
                    return halfResDepthStencilPool;
                }
            }

            private TexturePool halfResRenderTargetPool;
            public TexturePool HalfResRenderTargetPool
            {
                get
                {
                    return halfResRenderTargetPool;
                }
            }

            private TexturePool quarterResDepthStencilPool;
            public TexturePool QuarterResDepthStencilPool
            {
                get
                {
                    return quarterResDepthStencilPool;
                }
            }

            private TexturePool quarterResRenderTargetPool;
            public TexturePool QuarterResRenderTargetPool
            {
                get
                {
                    return quarterResRenderTargetPool;
                }
            }
            #endregion
            /// <summary>
            /// Gets or sets a value indicating whether this is initialized.
            /// </summary>
            /// <value>
            ///   <c>true</c> if initialized; otherwise, <c>false</c>.
            /// </value>
            public bool Initialized { private set; get; } = false;
            /// <summary>
            /// Gets or sets the texture format.
            /// </summary>
            /// <value>
            /// The format.
            /// </value>
            public Format Format { set; get; } = Format.B8G8R8A8_UNorm;
#if MSAA
            /// <summary>
            /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
            /// </summary>
            public MSAALevel MSAA
            {
                private set; get;
            } = MSAALevel.Disable;
#endif
            /// <summary>
            /// The vertical synchronize internal. Only valid under swapchain rendering mode. Default = 1
            /// <para>0: disable; 1: Sync with frame; More detail: <see cref="SwapChain.Present(int, PresentFlags)"/></para>
            /// </summary>
            public int VSyncInterval = 1;
            /// <summary>
            /// The currently used Direct3D Device
            /// </summary>
            public Device Device
            {
                get
                {
                    return DeviceResources.Device;
                }
            }
            /// <summary>
            /// Gets the device2 d.
            /// </summary>
            /// <value>
            /// The device2 d.
            /// </value>
            public global::SharpDX.Direct2D1.Device Device2D
            {
                get
                {
                    return DeviceResources.Device2D;
                }
            }
            /// <summary>
            /// Gets the device context2 d.
            /// </summary>
            /// <value>
            /// The device context2 d.
            /// </value>
            public global::SharpDX.Direct2D1.DeviceContext DeviceContext2D
            {
                get
                {
                    return DeviceResources.DeviceContext2D;
                }
            }
            /// <summary>
            /// Gets or sets the device resources.
            /// </summary>
            /// <value>
            /// The device resources.
            /// </value>
            protected IDeviceResources DeviceResources
            {
                private set; get;
            }
            /// <summary>
            /// Gets or sets a value indicating whether [use depth stencil buffer].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [use depth stencil buffer]; otherwise, <c>false</c>.
            /// </value>
            public bool UseDepthStencilBuffer { private set; get; } = true;

            /// <summary>
            /// Gets or sets the sample description.
            /// </summary>
            /// <value>
            /// The sample description.
            /// </value>
            public SampleDescription ColorBufferSampleDesc
            {
                private set; get;
            }
            /// <summary>
            /// Whether render target/depth stencil buffer are MSAA buffers.
            /// </summary>
            public bool HasMSAA => ColorBufferSampleDesc.Count > 1;

            /// <summary>
            /// Initializes a new instance of the <see cref="DX11RenderBufferProxyBase"/> class.
            /// </summary>
            /// <param name="deviceResource">The device resources.</param>
            /// <param name="useDepthStencilBuffer"></param>
            public DX11RenderBufferProxyBase(IDeviceResources deviceResource, bool useDepthStencilBuffer = true)
            {
                this.DeviceResources = deviceResource;
                deviceContextPool = new DeviceContextPool(Device);
                this.UseDepthStencilBuffer = useDepthStencilBuffer;
            }

            private void CreateNonMSAADepthStencilBuffer(int width, int height)
            {
                if (HasMSAA)
                {
                    var depthFormat = Format.D32_Float_S8X24_UInt;
                    var depthdesc = new Texture2DDescription
                    {
                        BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                        Format = DepthStencilFormatHelper.ComputeTextureFormat(depthFormat, out _),
                        Width = width,
                        Height = height,
                        MipLevels = 1,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                        OptionFlags = ResourceOptionFlags.None,
                        CpuAccessFlags = CpuAccessFlags.None,
                        ArraySize = 1,
                    };
                    depthStencilBufferNoMSAA = new ShaderResourceViewProxy(Device, depthdesc);
                    depthStencilBufferNoMSAA.CreateDepthStencilView(new DepthStencilViewDescription()
                    {
                        Format = DepthStencilFormatHelper.ComputeDSVFormat(depthFormat),
                        Dimension = DepthStencilViewDimension.Texture2D
                    });
                    depthStencilBufferNoMSAA.CreateTextureView(new ShaderResourceViewDescription() 
                    { 
                        Format = DepthStencilFormatHelper.ComputeSRVFormat(depthFormat),
                        Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                        Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = 1 }
                    });
                }
                else
                {
                    depthStencilBufferNoMSAA = depthStencilBuffer;
                }
            }

            private ShaderResourceViewProxy CreateRenderTarget(int width, int height, MSAALevel msaa)
            {
#if MSAA
                MSAA = msaa;
#endif
                TargetWidth = width;
                TargetHeight = height;
                DisposeBuffers();
                ColorBufferSampleDesc = GetMSAASampleDescription();
                OnCreateRenderTargetAndDepthBuffers(width, height, UseDepthStencilBuffer, out colorBuffer, out depthStencilBuffer);
                CreateNonMSAADepthStencilBuffer(width, height);
                backBuffer = OnCreateBackBuffer(width, height);
                backBuffer.CreateRenderTargetView();
                #region Initialize Texture Pool
                InitializeTexturePools(width, height);
                #endregion
                Initialized = true;
                OnNewBufferCreated?.Invoke(this, new Texture2DArgs(backBuffer));
                return backBuffer;
            }

            private void InitializeTexturePools(int width, int height)
            {
                fullResPPBuffer = new PingPongColorBuffers(Format, width, height, this.DeviceResources);
                fullResDepthStencilPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = width,
                    Height = height,
                    ArraySize = 1,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });

                fullResRenderTargetPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = width,
                    Height = height,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    ArraySize = 1,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });

                halfResDepthStencilPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = Math.Max(2, width / 2),
                    Height = Math.Max(2, height / 2),
                    ArraySize = 1,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });

                halfResRenderTargetPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = Math.Max(2, width / 2),
                    Height = Math.Max(2, height / 2),
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    ArraySize = 1,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });

                quarterResDepthStencilPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = Math.Max(2, width / 4),
                    Height = Math.Max(2, height / 4),
                    ArraySize = 1,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });

                quarterResRenderTargetPool = new TexturePool(this.DeviceResources, new Texture2DDescription()
                {
                    Width = Math.Max(2, width / 4),
                    Height = Math.Max(2, height / 4),
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Usage = ResourceUsage.Default,
                    ArraySize = 1,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });
            }

            private void DisposeTexturePools()
            {
                RemoveAndDispose(ref fullResPPBuffer);
                RemoveAndDispose(ref fullResDepthStencilPool);
                RemoveAndDispose(ref fullResRenderTargetPool);
                RemoveAndDispose(ref halfResDepthStencilPool);
                RemoveAndDispose(ref halfResRenderTargetPool);
                RemoveAndDispose(ref quarterResDepthStencilPool);
                RemoveAndDispose(ref quarterResRenderTargetPool);
            }

            /// <summary>
            /// Disposes the buffers.
            /// </summary>
            protected virtual void DisposeBuffers()
            {
                DeviceContext2D.Target = null;
                DisposeTexturePools();
                RemoveAndDispose(ref d2dTarget);
                RemoveAndDispose(ref colorBuffer);               
                RemoveAndDispose(ref depthStencilBuffer);
                RemoveAndDispose(ref depthStencilBufferNoMSAA);
                RemoveAndDispose(ref backBuffer);
            }

            protected abstract ShaderResourceViewProxy OnCreateBackBuffer(int width, int height);

            protected virtual SampleDescription GetMSAASampleDescription()
            {
                var sampleCount = 1;
                var sampleQuality = 0;
#if MSAA
                if (MSAA != MSAALevel.Disable)
                {
                    do
                    {
                        var newSampleCount = sampleCount * 2;
                        var newSampleQuality = Device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, newSampleCount) - 1;

                        if (newSampleQuality < 0)
                            break;

                        sampleCount = newSampleCount;
                        sampleQuality = newSampleQuality;
                        if (sampleCount == (int)MSAA)
                        {
                            break;
                        }
                    } while (sampleCount < 32);
                }
#endif
                return new SampleDescription(sampleCount, sampleQuality);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="createDepthStencilBuffer"></param>
            /// <param name="colorBuffer"></param>
            /// <param name="depthStencilBuffer"></param>
            /// <returns></returns>
            protected virtual void OnCreateRenderTargetAndDepthBuffers(int width, int height, bool createDepthStencilBuffer,
                out ShaderResourceViewProxy colorBuffer, out ShaderResourceViewProxy depthStencilBuffer)
            {
                var sampleDesc = ColorBufferSampleDesc;
                var optionFlags = ResourceOptionFlags.None;

                var colordesc = new Texture2DDescription
                {
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Format = Format,
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    SampleDescription = sampleDesc,
                    Usage = ResourceUsage.Default,
                    OptionFlags = optionFlags,
                    CpuAccessFlags = CpuAccessFlags.None,
                    ArraySize = 1
                };

                colorBuffer = new ShaderResourceViewProxy(Device, colordesc);
                colorBuffer.CreateRenderTargetView();
                colorBuffer.CreateTextureView();
                if (createDepthStencilBuffer)
                {
                    var depthdesc = new Texture2DDescription
                    {
                        BindFlags = BindFlags.DepthStencil,
                        Format = DepthStencilFormatHelper.ComputeTextureFormat(Format.D32_Float_S8X24_UInt, out var canUseAsShaderResource),
                        Width = width,
                        Height = height,
                        MipLevels = 1,
                        SampleDescription = sampleDesc,
                        Usage = ResourceUsage.Default,
                        OptionFlags = ResourceOptionFlags.None,
                        CpuAccessFlags = CpuAccessFlags.None,
                        ArraySize = 1,
                    };
                    canUseAsShaderResource &= !HasMSAA;
                    if (canUseAsShaderResource)
                    {
                        depthdesc.BindFlags |= BindFlags.ShaderResource;
                    }
                    depthStencilBuffer = new ShaderResourceViewProxy(Device, depthdesc);
                    depthStencilBuffer.CreateDepthStencilView(
                        new DepthStencilViewDescription() 
                        {
                            Format = DepthStencilFormatHelper.ComputeDSVFormat(depthdesc.Format),
                            Dimension = HasMSAA ? DepthStencilViewDimension.Texture2DMultisampled : DepthStencilViewDimension.Texture2D
                        });
                    if (canUseAsShaderResource)
                    {
                        depthStencilBuffer.CreateTextureView(new ShaderResourceViewDescription()
                        {
                            Format = DepthStencilFormatHelper.ComputeSRVFormat(depthdesc.Format), 
                            Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                            Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = depthdesc.MipLevels }
                        });
                    }
                }
                else
                {
                    depthStencilBuffer = null;
                }
            }

            /// <summary>
            /// Sets the default render-targets
            /// </summary>
            public void SetDefaultRenderTargets(DeviceContextProxy context, bool isColorBuffer = true)
            {
                context.SetRenderTargets(isColorBuffer ? depthStencilBuffer : null, new RenderTargetView[] { isColorBuffer ? colorBuffer : backBuffer });
                //context.OutputMerger.SetTargets(depthStencilBuffer, new RenderTargetView[] { isColorBuffer ? colorBuffer : backBuffer });
                context.SetViewport(0, 0, TargetWidth, TargetHeight, 0.0f, 1.0f);
                context.SetScissorRectangle(0, 0, TargetWidth, TargetHeight);
            }

            /// <summary>
            /// Clears the render target binding.
            /// </summary>
            /// <param name="context">The context.</param>
            public void ClearRenderTargetBinding(DeviceContextProxy context)
            {
                context.ClearRenderTagetBindings();
            }
            /// <summary>
            /// Clears the render target.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="color">The color.</param>
            public void ClearRenderTarget(DeviceContextProxy context, Color4 color)
            {
                ClearRenderTarget(context, color, true, true);
            }
            /// <summary>
            /// Clears the buffers with the clear-color
            /// </summary>
            /// <param name="context"></param>
            /// <param name="color"></param>
            /// <param name="clearBackBuffer"></param>
            /// <param name="clearDepthStencilBuffer"></param>
            public void ClearRenderTarget(DeviceContextProxy context, Color4 color, bool clearBackBuffer, bool clearDepthStencilBuffer)
            {
                if (clearBackBuffer)
                {
                    context.ClearRenderTargetView(colorBuffer, color);
                }

                if (clearDepthStencilBuffer)
                {
                    context.ClearDepthStencilView(depthStencilBuffer, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
                }
            }
            /// <summary>
            /// Initializes.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="msaa">The msaa.</param>
            /// <returns></returns>
            public ShaderResourceViewProxy Initialize(int width, int height, MSAALevel msaa)
            {
#if MSAA
                return CreateRenderTarget(width, height, msaa);
#else
                return CreateRenderTarget(width, height, MSAALevel.Disable);
#endif
            }
            /// <summary>
            /// Resize render target and depthbuffer resolution
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public virtual ShaderResourceViewProxy Resize(int width, int height)
            {
#if MSAA
                return CreateRenderTarget(width, height, MSAA);
#else
                return CreateRenderTarget(width, height, MSAALevel.Disable);
#endif
            }
            /// <summary>
            /// Begins the draw.
            /// </summary>
            /// <returns></returns>
            public virtual bool BeginDraw()
            {
                return Initialized;
            }
            /// <summary>
            /// Ends the draw.
            /// </summary>
            /// <returns></returns>
            public virtual bool EndDraw()
            {
                return true;
            }
            /// <summary>
            /// Presents this drawing..
            /// </summary>
            /// <returns></returns>
            public virtual bool Present()
            {
                return true;
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected override void OnDispose(bool disposeManagedResources)
            {
                OnNewBufferCreated = null;
                DeviceLost = null;
                DisposeBuffers();
                DisposeTexturePools();
                RemoveAndDispose(ref deviceContextPool);
                Initialized = false;
                base.OnDispose(disposeManagedResources);
            }

            #region ERROR HANDLING        
            /// <summary>
            /// Raises the on device lost.
            /// </summary>
            protected void RaiseOnDeviceLost()
            {
                DeviceLost?.Invoke(this, EventArgs.Empty);
            }
            #endregion
        }
    }
}

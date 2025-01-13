using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core2D;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.ComponentModel;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

#if false
#elif WINUI
#elif WPF
using HelixToolkit.Wpf.SharpDX.Utilities;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Use to contain shared models for multiple viewports. 
/// <para>Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak</para>
/// </summary>
public class ModelContainer3DX : HelixItemsControl, IModelContainer
{
    /// <summary>
    /// The EffectsManager property. Suggest to bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak
    /// </summary>
    public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
        "EffectsManager", typeof(IEffectsManager), typeof(ModelContainer3DX), new PropertyMetadata(null,
            (s, e) => ((ModelContainer3DX)s).EffectsManagerPropertyChanged()));

    /// <summary>
    /// Gets or sets the <see cref="EffectsManagerProperty"/>.
    /// <para>The EffectsManager property. Suggest bind effects manager in viewmodel. Assign effect manager from code behind may cause memory leak.</para>
    /// </summary>
    public IEffectsManager? EffectsManager
    {
        get
        {
            return (IEffectsManager)GetValue(EffectsManagerProperty);
        }
        set
        {
            SetValue(EffectsManagerProperty, value);
        }
    }

    public IRenderTechnique? RenderTechnique
    {
        set; get;
    }

    private readonly HashSet<IViewport3DX> viewports = new();
    private readonly HashSet<IRenderHost> attachedRenderHosts = new();
#pragma warning disable 0067
    /// <summary>
    /// Fired whenever an exception occurred on this object.
    /// </summary>
    public event EventHandler<RelayExceptionEventArgs>? ExceptionOccurred;
    /// <summary>
    /// Occurs when [on new render target texture].
    /// </summary>
    public event EventHandler<Texture2DArgs>? OnNewRenderTargetTexture;
    /// <summary>
    /// Occurs when [start render loop].
    /// </summary>
    public event EventHandler<EventArgs>? StartRenderLoop;
    /// <summary>
    /// Occurs when [stop render loop].
    /// </summary>
    public event EventHandler<EventArgs>? StopRenderLoop;
    /// <summary>
    /// Occurs when each render frame finished rendering.
    /// </summary>
    public event EventHandler? Rendered;

    public event EventHandler? SceneGraphUpdated;
#pragma warning restore 0067
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    /// <value>
    /// The unique identifier.
    /// </value>
    public Guid GUID { get; } = Guid.NewGuid();
    /// <summary>
    /// Gets or sets a value indicating whether this instance is rendering.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
    /// </value>
    public bool IsRendering { set; get; } = true;

    private int d3dCounter = 0;

    private IRenderHost? currentRenderHost = null;
    /// <summary>
    /// </summary>
    public IRenderHost? CurrentRenderHost
    {
        set
        {
            if (currentRenderHost != value)
            {
                currentRenderHost = value;
                currentRenderHost?.SetDefaultRenderTargets(false);
            }
        }
        get
        {
            return currentRenderHost;
        }
    }

    public IRenderer? Renderer
    {
        get
        {
            return CurrentRenderHost?.Renderer;
        }
    }

    public DeviceContextProxy? ImmediateDeviceContext
    {
        get => currentRenderHost?.ImmediateDeviceContext;
    }

    public new float ActualWidth
    {
        get => 0;
    }
    public new float ActualHeight
    {
        get => 0;
    }

    public float DpiScale
    {
        set; get;
    } = 1;

    /// <summary>
    /// Gets the current frame renderables for rendering.
    /// </summary>
    /// <value>
    /// The per frame renderable.
    /// </value>
    public FastList<KeyValuePair<int, SceneNode>> PerFrameFlattenedScene
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameFlattenedScene : Constants.EmptyRenderablePair;
        }
    }
    /// <summary>
    /// Gets the current frame Lights for rendering.
    /// </summary>
    /// <value>
    /// The per frame renderable.
    /// </value>
    public IEnumerable<LightNode?> PerFrameLights
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameLights : Enumerable.Empty<LightNode?>();
        }
    }
    /// <summary>
    /// Gets the per frame post effect cores.
    /// </summary>
    /// <value>
    /// The per frame post effect cores.
    /// </value>
    public FastList<SceneNode> PerFrameNodesWithPostEffect
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameNodesWithPostEffect : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Gets the per frame general render cores.
    /// </summary>
    /// <value>
    /// The per frame general render cores.
    /// </value>
    public FastList<SceneNode> PerFrameOpaqueNodes
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameOpaqueNodes : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Gets the per frame opaque nodes in frustum.
    /// </summary>
    /// <value>
    /// The per frame opaque nodes in frustum.
    /// </value>
    public FastList<SceneNode> PerFrameOpaqueNodesInFrustum
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameOpaqueNodesInFrustum : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Gets the per frame transparent nodes. , <see cref="RenderType.Transparent"/>
    /// </summary>
    /// <value>
    /// The per frame transparent nodes.
    /// </value>
    public FastList<SceneNode> PerFrameTransparentNodes
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameTransparentNodes : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Gets the per frame transparent nodes in frustum.
    /// </summary>
    /// <value>
    /// The per frame transparent nodes in frustum.
    /// </value>
    public FastList<SceneNode> PerFrameTransparentNodesInFrustum
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameTransparentNodesInFrustum : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Gets the per frame particle nodes. <see cref="RenderType.Particle" />
    /// </summary>
    /// <value>
    /// The per frame particle nodes.
    /// </value>
    public FastList<SceneNode> PerFrameParticleNodes
    {
        get
        {
            return CurrentRenderHost != null ? CurrentRenderHost.PerFrameParticleNodes : Constants.EmptyRenderable;
        }
    }
    /// <summary>
    /// Handles the change of the effects manager.
    /// </summary>
    private void EffectsManagerPropertyChanged()
    {
        foreach (var viewport in viewports)
        {
            viewport.EffectsManager = this.EffectsManager;
        }
        if (EffectsManager is not null)
        {
            EffectsManagerChanged?.Invoke(this, EffectsManager);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="viewport"></param>
    public void AttachViewport3DX(IViewport3DX viewport)
    {
        if (viewports.Add(viewport))
        {
            viewport.EffectsManager = this.EffectsManager;
        }
    }
    /// <summary>
    /// </summary>
    /// <param name="viewport"></param>
    public void DettachViewport3DX(IViewport3DX viewport)
    {
        viewports.Remove(viewport);
    }
    /// <summary>
    /// Invalidates the render.
    /// </summary>
    public void InvalidateRender()
    {
        foreach (var v in attachedRenderHosts)
        {
            v.InvalidateRender();
        }
    }

    /// <summary>
    /// Invalidates the scene graph.
    /// </summary>
    public void InvalidateSceneGraph()
    {
        foreach (var v in attachedRenderHosts)
        {
            v.InvalidateSceneGraph();
        }
    }
    /// <summary>
    /// Invalidates the per frame renderables.
    /// </summary>
    public void InvalidatePerFrameRenderables()
    {
        foreach (var v in attachedRenderHosts)
        {
            v.InvalidatePerFrameRenderables();
        }
    }

    public void Invalidate(InvalidateTypes type)
    {
        foreach (var v in attachedRenderHosts)
        {
            v.Invalidate(type);
        }
    }
    /// <summary>
    /// Sets the default render targets.
    /// </summary>
    /// <param name="clear">if set to <c>true</c> [clear].</param>
    public void SetDefaultRenderTargets(bool clear = true)
    {
        CurrentRenderHost?.SetDefaultRenderTargets(clear);
    }
    /// <summary>
    /// </summary>
    public IEnumerable<SceneNode?> Renderables
    {
        get
        {
            foreach (Element3D item in Items)
            {
                yield return item.SceneNode;
            }
        }
    }
    /// <summary>
    /// Gets the device.
    /// </summary>
    /// <value>
    /// The device.
    /// </value>
    public Device? Device
    {
        get
        {
            return this.EffectsManager?.Device;
        }
    }

    /// <summary>
    /// Gets the device2d.
    /// </summary>
    /// <value>
    /// The device2d.
    /// </value>
    public global::SharpDX.Direct2D1.Device? Device2D
    {
        get
        {
            return this.EffectsManager?.Device2D;
        }
    }

    /// <summary>
    /// Gets or sets the color of the clear.
    /// </summary>
    /// <value>
    /// The color of the clear.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(Color4Converter))]
#else
#error Unknown framework
#endif
    public Color4 ClearColor
    {
        get
        {
            return currentRenderHost != null ? currentRenderHost.ClearColor : Color.White;
        }
        set
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is shadow map enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is shadow map enabled; otherwise, <c>false</c>.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public bool IsShadowMapEnabled
    {
        get
        {
            return currentRenderHost != null && currentRenderHost.IsShadowMapEnabled;
        }
        set
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Gets or sets the msaa.
    /// </summary>
    /// <value>
    /// The msaa.
    /// </value>
    public MSAALevel MSAA
    {
        set; get;
    }

    public FeatureLevel FeatureLevel
    {
        get
        {
            return currentRenderHost != null ? currentRenderHost.FeatureLevel : FeatureLevel.Level_11_0;
        }
    }
    /// <summary>
    /// Gets or sets the viewport.
    /// </summary>
    /// <value>
    /// The viewport.
    /// </value>
    public IViewport3DX? Viewport
    {
        set; get;
    }
    /// <summary>
    /// Gets the render context.
    /// </summary>
    /// <value>
    /// The render context.
    /// </value>
    public RenderContext? RenderContext
    {
        get
        {
            return CurrentRenderHost?.RenderContext;
        }
    }
    /// <summary>
    /// Indicates if DPFCanvas busy on rendering.
    /// </summary>
    public bool IsBusy
    {
        get
        {
            return CurrentRenderHost?.IsBusy ?? false;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable render frustum].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableRenderFrustum
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the maximum FPS.
    /// </summary>
    /// <value>
    /// The maximum FPS.
    /// </value>
    public uint MaxFPS
    {
        set; get;
    }
    /// <summary>
    /// Gets a value indicating whether this instance is deferred lighting.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is deferred lighting; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeferredLighting
    {
        get
        {
            return CurrentRenderHost != null && CurrentRenderHost.IsDeferredLighting;
        }
    }
    /// <summary>
    /// Gets or sets the shared model container.
    /// </summary>
    /// <value>
    /// The shared model container.
    /// </value>
    public IModelContainer? SharedModelContainer
    {
        set
        {
        }
        get
        {
            return this;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable sharing model mode].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable sharing model mode]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableSharingModelMode
    {
        set
        {
        }
        get
        {
            return true;
        }
    }
    /// <summary>
    /// Gets the color buffer view.
    /// </summary>
    /// <value>
    /// The color buffer view.
    /// </value>
    public RenderTargetView? RenderTargetBufferView
    {
        get
        {
            return CurrentRenderHost?.RenderTargetBufferView;
        }
    }
    /// <summary>
    /// Gets the depth stencil buffer view.
    /// </summary>
    /// <value>
    /// The depth stencil buffer view.
    /// </value>
    public DepthStencilView? DepthStencilBufferView
    {
        get
        {
            return CurrentRenderHost?.DepthStencilBufferView;
        }
    }
    /// <summary>
    /// Gets the d2d target.
    /// </summary>
    /// <value>
    /// The d2d target.
    /// </value>
    public D2DTargetProxy? D2DTarget
    {
        get
        {
            return CurrentRenderHost?.D2DTarget;
        }
    }
    /// <summary>
    /// Gets the render statistics.
    /// </summary>
    /// <value>
    /// The render statistics.
    /// </value>
    public IRenderStatistics? RenderStatistics
    {
        get
        {
            return CurrentRenderHost?.RenderStatistics;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show statistics].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show statistics]; otherwise, <c>false</c>.
    /// </value>
    public RenderDetail ShowRenderDetail
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the render configuration.
    /// </summary>
    /// <value>
    /// The render configuration.
    /// </value>
    public DX11RenderHostConfiguration RenderConfiguration
    {
        set; get;
    } = new();

    public DX11RenderBufferProxyBase? RenderBuffer
    {
        get
        {
            return CurrentRenderHost?.RenderBuffer;
        }
    }

    public event EventHandler<IEffectsManager>? EffectsManagerChanged;

    public bool EnableParallelProcessing { set; get; }

    public ModelContainer3DX()
    {
        this.IsHitTestVisible = false;
#if !NETFX_CORE && !WINUI
        Visibility = System.Windows.Visibility.Collapsed;
#endif
    }

    /// <summary>
    /// Attaches the specified host.
    /// </summary>
    /// <param name="host">The host.</param>
    public void Attach(IRenderHost host)
    {
        if (host != null && attachedRenderHosts.Add(host))
        {
            if (Interlocked.Increment(ref d3dCounter) == 1 && host.EffectsManager != null)
            {
                foreach (var renderable in Renderables)
                {
                    if (renderable is null)
                    {
                        continue;
                    }

                    renderable.Invalidated += RenderableInvalidated;
                    renderable.Attach(EffectsManager);
                }
            }
        }
    }

    private void RenderableInvalidated(object? sender, InvalidateTypes e)
    {
        Invalidate(e);
    }

    /// <summary>
    /// Detaches this instance.
    /// </summary>
    /// <param name="host"></param>
    /// <exception cref="IndexOutOfRangeException">D3DCounter is negative.</exception>
    public void Detach(IRenderHost host)
    {
        if (host != null && attachedRenderHosts.Remove(host))
        {
            if (Interlocked.Decrement(ref d3dCounter) == 0)
            {
                foreach (var renderable in Renderables)
                {
                    if (renderable is null)
                    {
                        continue;
                    }

                    renderable.Detach();
                    renderable.Invalidated -= RenderableInvalidated;
                }
            }
            else if (d3dCounter < 0)
            {
                throw new IndexOutOfRangeException("D3DCounter is negative.");
            }
        }
    }

    private void Detach()
    {
        foreach (var renderable in Renderables)
        {
            if (renderable is null)
            {
                continue;
            }

            renderable.Detach();
            renderable.Invalidated -= RenderableInvalidated;
        }
    }
    /// <summary>
    /// Starts the d3 d.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public void StartD3D(int width, int height)
    {

    }
    /// <summary>
    /// Ends the d3 d.
    /// </summary>
    public void EndD3D()
    {

    }

    public void StartRendering()
    {

    }

    public void StopRendering()
    {

    }
    /// <summary>
    /// Updates the and render.
    /// </summary>
    public bool UpdateAndRender()
    {
        return false;
    }
    /// <summary>
    /// Resizes
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public void Resize(int width, int height)
    {

    }

    /// <summary>
    /// Clears the render target.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
    /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
    public void ClearRenderTarget(DeviceContextProxy context, bool clearBackBuffer, bool clearDepthStencilBuffer)
    {
        CurrentRenderHost?.ClearRenderTarget(context, clearBackBuffer, clearDepthStencilBuffer);
    }
    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                attachedRenderHosts.Clear();
                Detach();
                foreach (var item in Items)
                {
                    if (item is IDisposable d)
                    {
                        d.Dispose();
                    }
                }
                // TODO: dispose managed state (managed objects).
                viewports.Clear();
                currentRenderHost = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ModelContainer3DX() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}

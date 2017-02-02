// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   This class handles device creation for Direct2D, Direct3D, DirectWrite
//   and WIC.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using Windows.UI.Core;

namespace HelixToolkit.UWP.CommonDX
{
    /// <summary>
    /// This class handles device creation for Direct2D, Direct3D, DirectWrite
    /// and WIC.
    /// </summary>
    /// <remarks>
    /// SharpDX CommonDX is inspired from the DirectXBase C++ class from Win8
    /// Metro samples, but the design is slightly improved in order to reuse 
    /// components more easily. 
    /// <see cref="DeviceManager"/> is responsible for device creation.
    /// <see cref="TargetBase"/> is responsible for rendering, render target 
    /// creation.
    /// Initialization and Rendering is event driven based, allowing a better
    /// reuse of different components.
    /// </remarks>
    public class DeviceManager : DisposeCollector
    {
        // Declare Direct2D Objects
        protected global::SharpDX.Direct2D1.Factory1           d2dFactory;
        protected global::SharpDX.Direct2D1.Device             d2dDevice;
        protected global::SharpDX.Direct2D1.DeviceContext      d2dContext;

        // Declare DirectWrite & Windows Imaging Component Objects
        protected global::SharpDX.DirectWrite.Factory          dwriteFactory;
        protected global::SharpDX.WIC.ImagingFactory2          wicFactory;

        // Direct3D Objects
        protected global::SharpDX.Direct3D11.Device1           d3dDevice;
        protected global::SharpDX.Direct3D11.DeviceContext1    d3dContext;
        protected FeatureLevel featureLevel;
        protected float dpi;

        /// <summary>
        /// Gets the Direct3D11 device.
        /// </summary>
        public global::SharpDX.Direct3D11.Device1 DeviceDirect3D { get { return d3dDevice; } }

        /// <summary>
        /// Gets the Direct3D11 context.
        /// </summary>
        public global::SharpDX.Direct3D11.DeviceContext1 ContextDirect3D { get { return d3dContext; } }

        /// <summary>
        /// Gets the Direct2D factory.
        /// </summary>
        public global::SharpDX.Direct2D1.Factory1 FactoryDirect2D { get { return d2dFactory; } }

        /// <summary>
        /// Gets the Direct2D device.
        /// </summary>
        public global::SharpDX.Direct2D1.Device DeviceDirect2D { get { return d2dDevice; } }

        /// <summary>
        /// Gets the Direct2D context.
        /// </summary>
        public global::SharpDX.Direct2D1.DeviceContext ContextDirect2D { get { return d2dContext; } }

        /// <summary>
        /// Gets the DirectWrite factory.
        /// </summary>
        public global::SharpDX.DirectWrite.Factory FactoryDirectWrite { get { return dwriteFactory; } }

        /// <summary>
        /// Gets the WIC factory.
        /// </summary>
        public global::SharpDX.WIC.ImagingFactory2 WICFactory { get { return wicFactory; } }

        /// <summary>
        /// This event is fired when the <see cref="Dpi"/> is called,
        /// </summary>
        public event Action<DeviceManager> OnDpiChanged;

        /// <summary>
        /// This event is fired when the DeviceMamanger is initialized by the <see cref="Initialize"/> method.
        /// </summary>
        public event Action<DeviceManager> OnInitialize;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <param name="window">Window to receive the rendering</param>
        public virtual void Initialize(float dpi)
        {
            CreateDeviceIndependentResources();
            CreateDeviceResources();
            
            if (OnInitialize != null)
                OnInitialize(this);

            Dpi = dpi;
        }

        /// <summary>
        /// Creates device independent resources.
        /// </summary>
        /// <remarks>
        /// This method is called at the initialization of this instance.
        /// </remarks>
        protected virtual void CreateDeviceIndependentResources() {
#if DEBUG
            var debugLevel = global::SharpDX.Direct2D1.DebugLevel.Information;
#else
            var debugLevel = global::SharpDX.Direct2D1.DebugLevel.None;
#endif
            // Dispose previous references and set to null
            RemoveAndDispose(ref d2dFactory);
            RemoveAndDispose(ref dwriteFactory);
            RemoveAndDispose(ref wicFactory);

            // Allocate new references
            d2dFactory = Collect(new global::SharpDX.Direct2D1.Factory1(global::SharpDX.Direct2D1.FactoryType.SingleThreaded, debugLevel));
            dwriteFactory = Collect(new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Shared));
            wicFactory = Collect(new global::SharpDX.WIC.ImagingFactory2());
        }

        /// <summary>
        /// Creates device resources. 
        /// </summary>
        /// <remarks>
        /// This method is called at the initialization of this instance.
        /// </remarks>
        protected virtual void CreateDeviceResources()
        {
            // Dispose previous references and set to null
            if (d3dDevice != null)
            {
                RemoveAndDispose(ref d3dDevice);
            }

            if (d3dContext != null)
            {
                RemoveAndDispose(ref d3dContext);
            }

            if (d2dDevice != null)
            {
                RemoveAndDispose(ref d2dDevice);
            }

            if (d2dContext != null)
            {
                RemoveAndDispose(ref d2dContext);
            }

            // Allocate new references
            // Enable compatibility with Direct2D
            // Retrieve the Direct3D 11.1 device amd device context
            var creationFlags = global::SharpDX.Direct3D11.DeviceCreationFlags.VideoSupport | global::SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport;

            // Decomment this line to have Debug. Unfortunately, debug is sometimes crashing applications, so it is disable by default
            try
            {
                // Try to create it with Video Support
                // If it is not working, we just use BGRA
                // Force to FeatureLevel.Level_9_1
                using (var defaultDevice = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, creationFlags))
                    d3dDevice = defaultDevice.QueryInterface<global::SharpDX.Direct3D11.Device1>();
            } catch (Exception)
            {
                creationFlags = global::SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport;
                using (var defaultDevice = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, creationFlags))
                    d3dDevice = defaultDevice.QueryInterface<global::SharpDX.Direct3D11.Device1>();
            }
            featureLevel = d3dDevice.FeatureLevel;

            // Get Direct3D 11.1 context
            d3dContext = Collect(d3dDevice.ImmediateContext.QueryInterface<global::SharpDX.Direct3D11.DeviceContext1>());

            // Create Direct2D device
            using (var dxgiDevice = d3dDevice.QueryInterface<global::SharpDX.DXGI.Device>())
                d2dDevice = Collect(new global::SharpDX.Direct2D1.Device(d2dFactory, dxgiDevice));

            // Create Direct2D context
            d2dContext = Collect(new global::SharpDX.Direct2D1.DeviceContext(d2dDevice, global::SharpDX.Direct2D1.DeviceContextOptions.None));
        }

        /// <summary>
        /// Gets or sets the DPI.
        /// </summary>
        /// <remarks>
        /// This method will fire the event <see cref="OnDpiChanged"/>
        /// if the dpi is modified.
        /// </remarks>
        public virtual float Dpi
        {
            get
            {
                return dpi;
            }
            set
            {
                if (dpi != value)
                {
                    dpi = value;
                    d2dContext.DotsPerInch = new Size2F(dpi, dpi);

                    if (OnDpiChanged != null)
                        OnDpiChanged(this);
                }
            }
        }
    }
}
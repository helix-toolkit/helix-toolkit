// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaceNavigatorDecorator.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Input
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;

    using TDx.TDxInput;

    /// <summary>
    /// Space navigator type enumeration.
    /// </summary>
    public enum SpaceNavigatorType
    {
        /// <summary>
        /// Is a unknown device.
        /// </summary>
        UnknownDevice = 0,

        /// <summary>
        /// Is a SpaceNavigator.
        /// </summary>
        SpaceNavigator = 6,

        /// <summary>
        /// Is a SpaceExplorer.
        /// </summary>
        SpaceExplorer = 4,

        /// <summary>
        /// Is a SpaceTraveler.
        /// </summary>
        SpaceTraveler = 25,

        /// <summary>
        /// Is a SpacePilot.
        /// </summary>
        SpacePilot = 29
    }

    /// <summary>
    /// Zoom mode.
    /// </summary>
    public enum SpaceNavigatorZoomMode
    {
        /// <summary>
        /// In and out.
        /// </summary>
        InOut,

        /// <summary>
        /// Up and down.
        /// </summary>
        UpDown
    }

    /// <summary>
    /// A decorator for the space navigator.
    /// </summary>
    public class SpaceNavigatorDecorator : Decorator
    {
        /// <summary>
        /// The camera control property.
        /// </summary>
        public static readonly DependencyProperty CameraControlProperty = DependencyProperty.Register(
            "CameraController", typeof(CameraController), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(null));

        /// <summary>
        /// The connection changed event.
        /// </summary>
        public static readonly RoutedEvent ConnectionChangedEvent = EventManager.RegisterRoutedEvent(
            "ConnectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpaceNavigatorDecorator));

        /// <summary>
        /// The is connected property.
        /// </summary>
        public static readonly DependencyProperty IsConnectedProperty = DependencyProperty.Register(
            "IsConnected", typeof(bool), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(false));

        /// <summary>
        /// The is pan enabled property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(false));

        /// <summary>
        /// The my property property.
        /// </summary>
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
            "Type",
            typeof(SpaceNavigatorType),
            typeof(SpaceNavigatorDecorator),
            new UIPropertyMetadata(SpaceNavigatorType.UnknownDevice));

        /// <summary>
        /// The navigator name property.
        /// </summary>
        public static readonly DependencyProperty NavigatorNameProperty = DependencyProperty.Register(
            "NavigatorName", typeof(string), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(null));

        /// <summary>
        /// The sensivity property.
        /// </summary>
        public static readonly DependencyProperty SensivityProperty = DependencyProperty.Register(
            "Sensitivity", typeof(double), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The zoom mode property.
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty = DependencyProperty.Register(
            "ZoomMode",
            typeof(SpaceNavigatorZoomMode),
            typeof(SpaceNavigatorDecorator),
            new UIPropertyMetadata(SpaceNavigatorZoomMode.UpDown));

        /// <summary>
        /// The zoom sensivity property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensivityProperty = DependencyProperty.Register(
            "ZoomSensitivity", typeof(double), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The _input.
        /// </summary>
        private Device _input;

        /// <summary>
        /// The _sensor.
        /// </summary>
        private Sensor _sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref = "SpaceNavigatorDecorator" /> class.
        /// </summary>
        public SpaceNavigatorDecorator()
        {
            this.Connect();

            /* todo: try to start driver if not available
             Thread.Sleep(1000);
                        if (!IsConnected)
                        {
                            Disconnect();
                            Thread.Sleep(1000);
                            StartDriver();
                            Connect();
                        }*/
        }

        /// <summary>
        /// Event when a property has been changed
        /// </summary>
        public event RoutedEventHandler ConnectionChanged
        {
            add
            {
                this.AddHandler(ConnectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(ConnectionChangedEvent, value);
            }
        }

        /// <summary>
        /// Gets or sets the camera controller.
        /// </summary>
        /// <value>The camera controller.</value>
        public CameraController CameraController
        {
            get
            {
                return (CameraController)this.GetValue(CameraControlProperty);
            }

            set
            {
                this.SetValue(CameraControlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return (bool)this.GetValue(IsConnectedProperty);
            }

            set
            {
                this.SetValue(IsConnectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }

            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the navigator.
        /// </summary>
        /// <value>The name of the navigator.</value>
        public string NavigatorName
        {
            get
            {
                return (string)this.GetValue(NavigatorNameProperty);
            }

            set
            {
                this.SetValue(NavigatorNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity.
        /// </summary>
        /// <value>The sensitivity.</value>
        public double Sensitivity
        {
            get
            {
                return (double)this.GetValue(SensivityProperty);
            }

            set
            {
                this.SetValue(SensivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public SpaceNavigatorType Type
        {
            get
            {
                return (SpaceNavigatorType)this.GetValue(MyPropertyProperty);
            }

            set
            {
                this.SetValue(MyPropertyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom mode.
        /// </summary>
        /// <value>The zoom mode.</value>
        public SpaceNavigatorZoomMode ZoomMode
        {
            get
            {
                return (SpaceNavigatorZoomMode)this.GetValue(ZoomModeProperty);
            }

            set
            {
                this.SetValue(ZoomModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom sensitivity.
        /// </summary>
        /// <value>The zoom sensitivity.</value>
        public double ZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(SensivityProperty);
            }

            set
            {
                this.SetValue(SensivityProperty, value);
            }
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>The controller.</value>
        private CameraController Controller
        {
            get
            {
                // if CameraController is set, use it
                if (this.CameraController != null)
                {
                    return this.CameraController;
                }

                // otherwise use the Child of the Decorator
                var view = this.Child as HelixViewport3D;
                return view == null ? null : view.CameraController;
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            if (this._input != null)
            {
                this._input.Disconnect();
            }

            this._input = null;
            this.IsConnected = false;
        }

        /// <summary>
        /// The raise connection changed.
        /// </summary>
        protected virtual void RaiseConnectionChanged()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(ConnectionChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// The connect.
        /// </summary>
        private void Connect()
        {
            try
            {
                this._input = new Device();
                this._sensor = this._input.Sensor;
                this._input.DeviceChange += this.input_DeviceChange;
                this._sensor.SensorInput += this.Sensor_SensorInput;
                this._input.Connect();
            }
            catch (COMException e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        // todo...
        /*
        private void StartDriver()
        {
            string exe = @"C:\Program Files\3Dconnexion\3Dconnexion 3DxSoftware\3DxWare\3dxsrv.exe";
            if (!File.Exists(exe))
                return;
            var p = Process.Start(exe, "-searchWarnDlg");
            Thread.Sleep(2000);
        }

        private void StopDriver()
        {
            string exe = @"C:\Program Files\3Dconnexion\3Dconnexion 3DxSoftware\3DxWare\3dxsrv.exe";
            if (!File.Exists(exe))
                return;
            var p = Process.Start(exe, "-shutdown");
        }
        */

        /// <summary>
        /// The sensor_ sensor input.
        /// </summary>
        private void Sensor_SensorInput()
        {
            if (this.Controller == null)
            {
                return;
            }

            this.Controller.AddRotateForce(
                this.Sensitivity * this._sensor.Rotation.Y, this.Sensitivity * this._sensor.Rotation.X);

            if (this.ZoomMode == SpaceNavigatorZoomMode.InOut)
            {
                this.Controller.AddZoomForce(this.ZoomSensitivity * 0.001 * this._input.Sensor.Translation.Z);
            }

            if (this.ZoomMode == SpaceNavigatorZoomMode.UpDown)
            {
                this.Controller.AddZoomForce(this.ZoomSensitivity * 0.001 * this._sensor.Translation.Y);
                if (this.IsPanEnabled)
                {
                    this.Controller.AddPanForce(
                        this.Sensitivity * 0.03 * this._sensor.Translation.X,
                        this.Sensitivity * 0.03 * this._sensor.Translation.Z);
                }
            }
        }

        /// <summary>
        /// The input_ device change.
        /// </summary>
        /// <param name="reserved">
        /// The reserved.
        /// </param>
        private void input_DeviceChange(int reserved)
        {
            this.IsConnected = true;
            this.Type = (SpaceNavigatorType)this._input.Type;
            this.NavigatorName = this.Type.ToString();
            this.RaiseConnectionChanged();
        }

    }
}
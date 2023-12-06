using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using TDx.TDxInput;
using DependencyPropertyGenerator;

namespace HelixToolkit.Wpf.TDxInput;

/// <summary>
/// A decorator for the space navigator.
/// </summary>
[DependencyProperty<CameraController>("CameraController")]
[DependencyProperty<bool>("IsConnected", DefaultValue = false)]
[DependencyProperty<bool>("IsPanEnabled", DefaultValue = false)]
[DependencyProperty<SpaceNavigatorType>("Type", DefaultValue = SpaceNavigatorType.UnknownDevice)]
[DependencyProperty<string>("NavigatorName")]
[DependencyProperty<double>("Sensitivity", DefaultValue = 1.0)]
[DependencyProperty<SpaceNavigatorZoomMode>("ZoomMode", DefaultValue = SpaceNavigatorZoomMode.UpDown)]
[DependencyProperty<double>("ZoomSensitivity", DefaultValue = 1.0)]
[RoutedEvent("ConnectionChanged", RoutedEventStrategy.Bubble)]
public partial class SpaceNavigatorDecorator : Decorator
{
    /// <summary>
    /// The _input.
    /// </summary>
    private Device? _input;

    /// <summary>
    /// The _sensor.
    /// </summary>
    private Sensor? _sensor;

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
    /// Gets the controller.
    /// </summary>
    /// <value>The controller.</value>
    private CameraController? Controller
    {
        get
        {
            // if CameraController is set, use it
            if (this.CameraController is not null)
            {
                return this.CameraController;
            }

            // otherwise use the Child of the Decorator
            return this.Child is not HelixViewport3D view ? null : view.CameraController;
        }
    }

    /// <summary>
    /// Disconnects this instance.
    /// </summary>
    public void Disconnect()
    {
        this._input?.Disconnect();

        this._input = null;
        this.IsConnected = false;
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
            this._input.DeviceChange += this.Input_DeviceChange;
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
            this.Sensitivity * this._sensor!.Rotation.Y, this.Sensitivity * this._sensor!.Rotation.X);

        if (this.ZoomMode == SpaceNavigatorZoomMode.InOut)
        {
            this.Controller.AddZoomForce(this.ZoomSensitivity * 0.001 * this._input!.Sensor.Translation.Z);
        }

        if (this.ZoomMode == SpaceNavigatorZoomMode.UpDown)
        {
            this.Controller.AddZoomForce(this.ZoomSensitivity * 0.001 * this._sensor!.Translation.Y);
            if (this.IsPanEnabled)
            {
                this.Controller.AddPanForce(
                    this.Sensitivity * 0.03 * this._sensor!.Translation.X,
                    this.Sensitivity * 0.03 * this._sensor!.Translation.Z);
            }
        }
    }

    /// <summary>
    /// The input_ device change.
    /// </summary>
    /// <param name="reserved">
    /// The reserved.
    /// </param>
    private void Input_DeviceChange(int reserved)
    {
        this.IsConnected = true;
        this.Type = (SpaceNavigatorType)this._input!.Type;
        this.NavigatorName = this.Type.ToString();
        this.OnConnectionChanged();
    }
}

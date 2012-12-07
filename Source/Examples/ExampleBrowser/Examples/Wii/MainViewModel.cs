// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using WiimoteLib;

namespace WiiDemo
{
    using System.Drawing;

    using Brush = System.Windows.Media.Brush;
    using Brushes = System.Windows.Media.Brushes;
    using Point = System.Windows.Point;

    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly Dispatcher dispatcher;
        private readonly Stopwatch timer = new Stopwatch();
        private readonly HelixViewport3D view;
        private readonly Wiimote wm;
        private bool buttonA;
        private bool buttonB;
        private bool buttonOne;
        private bool buttonTwo;
        private bool buttonUp;

        private const double damping = 0.05;

        private bool enableModelPositioning = true;
        private bool enableMouseSimulation;
        private double heave;
        private double heel;
        private System.Windows.Media.Brush hullBrush;
        private bool led1;
        private bool led2;

        private double length;
        private bool rumble;
        private double trim;
        private double vspeed;
        private const double vspeedDamping = 0.95;

        public Action UpAction { get; set; }

        public MainViewModel(Dispatcher dispatcher, HelixViewport3D view)
        {
            this.dispatcher = dispatcher;
            this.view = view;

            Heel = 0;
            Trim = 0;
            Heave = 0;
            Length = 200;
            HullBrush = Brushes.Red;

            try
            {
                wm = new Wiimote();
                wm.WiimoteChanged += WiimoteChanged;
            }
            catch
            {
                wm = null;
            }
        }

        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                RaisePropertyChanged("Length");
            }
        }

        public Brush HullBrush
        {
            get { return hullBrush; }
            set
            {
                hullBrush = value;
                RaisePropertyChanged("HullBrush");
            }
        }

        public double Heave
        {
            get { return heave; }
            set
            {
                heave = value;
                RaisePropertyChanged("Heave");
            }
        }

        public double Heel
        {
            get { return heel; }
            set
            {
                heel = value;
                RaisePropertyChanged("Heel");
                ValidateTrimHeel();
            }
        }

        public bool Led1
        {
            get { return led1; }
            set
            {
                if (value != led1)
                {
                    led1 = value;
                    wm.SetLEDs(led1, led2, false, false);
                }
            }
        }

        public bool Led2
        {
            get { return led2; }
            set
            {
                if (value != led2)
                {
                    led2 = value;
                    wm.SetLEDs(led1, led2, false, false);
                }
            }
        }

        protected bool Rumble
        {
            get { return rumble; }
            set
            {
                if (value != rumble)
                {
                    rumble = value;
                    if (wm != null)
                        wm.SetRumble(rumble);
                }
            }
        }

        public double Trim
        {
            get { return trim; }
            set
            {
                trim = value;
                RaisePropertyChanged("Trim");
                ValidateTrimHeel();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void OnLoaded()
        {
            if (wm != null)
            {
                try
                {
                    wm.Connect();
                    wm.SetReportType(InputReport.IRAccel, true);
                    wm.SetLEDs(false, false, false, false);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
        }

        public void OnClosing()
        {
            if (wm != null)
            {
                try
                {
                    wm.Disconnect();
                }
                catch
                {
                }
            }
        }

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            // Toggle states
            if (e.WiimoteState.ButtonState.One && !buttonOne)
            {
                enableModelPositioning = !enableModelPositioning;
                if (!enableModelPositioning)
                {
                    Heel = 0;
                    Trim = 0;
                    Heave = 0;
                }
            }

            if (e.WiimoteState.ButtonState.Two && !buttonTwo)
                enableMouseSimulation = !enableMouseSimulation;

            if (e.WiimoteState.ButtonState.Up && !buttonUp)
            {
                if (UpAction != null)
                    UpAction();
            }

            if (enableModelPositioning)
            {
                if (e.WiimoteState.ButtonState.Home)
                {
                    // changing UI properties must be invoked on the UI thread
                    dispatcher.BeginInvoke(new Action(() => view.ZoomExtents()));
                }

                //dispatcher.BeginInvoke(new Action(() => HullBrush = new SolidColorBrush(colors[r.Next(colors.Length)])));

                if (e.WiimoteState.ButtonState.Plus)
                {
                    dispatcher.BeginInvoke(new Action(() => view.CameraController.Zoom(-0.01)));
                }
                if (e.WiimoteState.ButtonState.Minus)
                {
                    dispatcher.BeginInvoke(new Action(() => view.CameraController.Zoom(0.01)));
                }

                double deltaTime = timer.ElapsedMilliseconds*0.001;
                timer.Restart();

                double newHeel = -90 + 180.0*(e.WiimoteState.AccelState.RawValues.X - 94)/(143 - 94);
                double newTrim = -90 + 180.0*(e.WiimoteState.AccelState.RawValues.Y - 94)/(143 - 94);
                Heel = Heel + (newHeel - Heel)*damping;
                Trim = Trim + (newTrim - Trim)*damping;

                double verticalAcceleration = 100.0*(e.WiimoteState.AccelState.RawValues.Z - 143);
                vspeed += verticalAcceleration*deltaTime;
                vspeed *= vspeedDamping;
                Heave = (Heave + vspeed*deltaTime)*0.95;

                // Console.WriteLine(e.WiimoteState.AccelState.Values.Z + " -> " + verticalAcceleration + " => " + vspeed);
            }
            if (enableMouseSimulation)
            {
                Point? pos = GetPosition(e.WiimoteState);
                if (pos.HasValue)
                {
                    MouseHelper.SetCursorPos((int) pos.Value.X, (int) pos.Value.Y);
                    if (e.WiimoteState.ButtonState.A && !buttonA)
                        MouseHelper.SimulateMouseEvent(MouseButton.Left, MouseButtonState.Pressed);
                    else if (!e.WiimoteState.ButtonState.A && buttonA)
                        MouseHelper.SimulateMouseEvent(MouseButton.Left, MouseButtonState.Released);
                    if (e.WiimoteState.ButtonState.B && !buttonB)
                        MouseHelper.SimulateMouseEvent(MouseButton.Right, MouseButtonState.Pressed);
                    else if (!e.WiimoteState.ButtonState.B && buttonB)
                        MouseHelper.SimulateMouseEvent(MouseButton.Right, MouseButtonState.Released);

                    if (e.WiimoteState.ButtonState.A && e.WiimoteState.ButtonState.B && !middleDown)
                    {
                        MouseHelper.SimulateMouseEvent(MouseButton.Left, MouseButtonState.Released);
                        MouseHelper.SimulateMouseEvent(MouseButton.Right, MouseButtonState.Released);
                        MouseHelper.SimulateMouseEvent(MouseButton.Middle, MouseButtonState.Pressed);
                        middleDown = true;
                    }
                    else if (!e.WiimoteState.ButtonState.A && !e.WiimoteState.ButtonState.B && middleDown)
                    {
                        MouseHelper.SimulateMouseEvent(MouseButton.Middle, MouseButtonState.Released);
                        middleDown = false;
                    }
                }
            }

            buttonUp = e.WiimoteState.ButtonState.Up;

            buttonOne = e.WiimoteState.ButtonState.One;
            buttonTwo = e.WiimoteState.ButtonState.Two;
            buttonA = e.WiimoteState.ButtonState.A;
            buttonB = e.WiimoteState.ButtonState.B;
        }

        private bool middleDown;
        public static Point? GetPosition(WiimoteState wiimoteState)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            PointF relativePosition;
            //if (wiimoteState.IRState.IRSensors[0].Found && wiimoteState.IRState.IRSensors[1].Found)
            //{
            //    relativePosition = wiimoteState.IRState.Midpoint;
            //}
            //else
            if (wiimoteState.IRState.IRSensors[0].Found)
            {
                relativePosition = wiimoteState.IRState.IRSensors[0].Position;
            }
            else
            {
                return null;
            }

            return new Point(screenWidth*(1.0 - relativePosition.X), screenHeight*relativePosition.Y);
        }

        private void ValidateTrimHeel()
        {
            if (wm != null)
            {
                Rumble = Math.Abs(Heel) > 60 || Math.Abs(Trim) > 60;
                Led1 = Math.Abs(Heel) > 30;
                Led2 = Math.Abs(Trim) > 30;
            }
        }
    }
}
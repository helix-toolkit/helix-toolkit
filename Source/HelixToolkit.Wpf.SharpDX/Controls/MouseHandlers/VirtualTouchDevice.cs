// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualTouchDevice.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A virtual <see cref="TouchDevice"/> enabling Windows.Forms controls to generate Touch/Manipulation-Events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// A virtual <see cref="TouchDevice"/> enabling Windows.Forms controls to generate Touch/Manipulation-Events.
    /// </summary>
    public class VirtualTouchDevice : TouchDevice
    {
        private static readonly Dictionary<int, VirtualTouchDevice> Devices = new Dictionary<int, VirtualTouchDevice>();

        private Point lastPosition;

        private TouchAction lastAction;

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterTouchWindow(IntPtr hWnd, ulong ulFlags);

        public static bool WndProc(Visual visual, ref Message m)
        {
            if (m.Msg == W32.WM_TOUCH)
            {
                var inputCount = m.WParam.ToInt32() & 0xffff;
                var inputs = new W32.TOUCHINPUT[inputCount];

                if (W32.GetTouchInputInfo(m.LParam, inputCount, inputs, W32.TOUCHINPUT_SIZE))
                {
                    for (var i = 0; i < inputCount; i++)
                    {
                        var input = inputs[i];
                        var position = new Point(input.x * 0.01, input.y * 0.01);
                        position = visual.PointFromScreen(position);

                        if (!Devices.TryGetValue(input.dwID, out var device))
                        {
                            device = new VirtualTouchDevice(input.dwID);
                            Devices.Add(input.dwID, device);
                        }

                        if (!device.IsActive && input.dwFlags.HasFlag(W32.TOUCHEVENTF.DOWN))
                        {
                            device.SetActiveSource(PresentationSource.FromVisual(visual));
                            device.lastPosition = position;
                            device.lastAction = TouchAction.Down;
                            device.Activate();
                            device.ReportDown();
                        }
                        else if (device.IsActive && input.dwFlags.HasFlag(W32.TOUCHEVENTF.UP))
                        {
                            device.lastPosition = position;
                            device.lastAction = TouchAction.Up;
                            device.ReportUp();
                            device.Deactivate();
                            Devices.Remove(input.dwID);
                        }
                        else if (device.IsActive && input.dwFlags.HasFlag(W32.TOUCHEVENTF.MOVE) &&
                                 device.lastPosition != position)
                        {
                            device.lastPosition = position;
                            device.lastAction = TouchAction.Move;
                            device.ReportMove();
                        }
                    }
                }

                W32.CloseTouchInputHandle(m.LParam);
                m.Result = new IntPtr(1);
                return true;
            }

            return false;
        }

        private VirtualTouchDevice(int id)
            : base(id)
        {
        }

        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            return new TouchPointCollection();
        }

        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            var pt = this.lastPosition;
            var relativeVisual = relativeTo as Visual;
            var rootVisual = this.ActiveSource?.RootVisual;
            if (relativeVisual != null && rootVisual != null && rootVisual.IsAncestorOf(relativeVisual))
            {
                pt = rootVisual.TransformToDescendant(relativeVisual).Transform(this.lastPosition);
            }

            var rect = new Rect(pt, new Size(1.0, 1.0));
            return new TouchPoint(this, pt, rect, this.lastAction);
        }

        protected override void OnCapture(IInputElement element, CaptureMode captureMode)
        {
            Mouse.PrimaryDevice.Capture(element, captureMode);
        }

        // ReSharper disable InconsistentNaming
        private static class W32
        {
            public const int WM_TOUCH = 0x0240;

            public static readonly int TOUCHINPUT_SIZE = Marshal.SizeOf(typeof(W32.TOUCHINPUT));

            [Flags]
            public enum TOUCHEVENTF
            {
                MOVE = 0x0001,
                DOWN = 0x0002,
                UP = 0x0004,
                INRANGE = 0x0008,
                PRIMARY = 0x0010,
                NOCOALESCE = 0x0020,
                PEN = 0x0040,
            }

            public enum TOUCHINPUTMASKF
            {
                TIMEFROMSYSTEM = 0x0001,
                EXTRAINFO = 0x0002,
                CONTACTAREA = 0x0004,
            }


            [StructLayout(LayoutKind.Sequential)]
            public struct TOUCHINPUT
            {
                public int x;
                public int y;
                public IntPtr hSource;
                public int dwID;
                public TOUCHEVENTF dwFlags;
                public int dwMask;
                public int dwTime;
                public IntPtr dwExtraInfo;
                public int cxContact;
                public int cyContact;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINTS
            {
                public short x;
                public short y;
            }

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs,
                int cbSize);

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern void CloseTouchInputHandle(IntPtr lParam);
        }
        // ReSharper enable InconsistentNaming
    }
}


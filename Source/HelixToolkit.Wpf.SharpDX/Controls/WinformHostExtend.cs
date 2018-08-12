using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX.Controls
{
    public class WinformHostExtend : WindowsFormsHost
    {
        public delegate void FormMouseMoveEventHandler(object sender, FormMouseMoveEventArgs e);
        public static readonly RoutedEvent FormMouseMoveEvent =
            EventManager.RegisterRoutedEvent("FormMouseMove", RoutingStrategy.Bubble, typeof(FormMouseMoveEventHandler), typeof(WinformHostExtend));

        public event FormMouseMoveEventHandler FormMouseMove
        {
            add { this.AddHandler(FormMouseMoveEvent, value); }
            remove { this.RemoveHandler(FormMouseMoveEvent, value); }
        }
        public delegate void FormMouseWheelEventHandler(object sender, FormMouseWheelEventArgs e);
        public static readonly RoutedEvent FormMouseWheelEvent =
            EventManager.RegisterRoutedEvent("FormMouseWheel", RoutingStrategy.Bubble, typeof(FormMouseWheelEventHandler), typeof(WinformHostExtend));

        public event FormMouseWheelEventHandler FormMouseWheel
        {
            add { this.AddHandler(FormMouseWheelEvent, value); }
            remove { this.RemoveHandler(FormMouseWheelEvent, value); }
        }
        protected UIElement ParentControl { set; get; }

        public double DPIXScale { set; get; } = 1;

        public double DPIYScale { set; get; } = 1;

        public WinformHostExtend()
        {
            ChildChanged += OnChildChanged;
        }

        private void OnChildChanged(object sender, ChildChangedEventArgs childChangedEventArgs)
        {
            var previousChild = childChangedEventArgs.PreviousChild as Control;
            if (previousChild != null)
            {
                previousChild.MouseDown -= OnMouseDown;
                previousChild.MouseWheel -= OnMouseWheel;
                previousChild.MouseMove -= OnMouseMove;
                previousChild.MouseUp -= OnMouseUp;
            }
            if (Child != null)
            {
                Child.MouseDown += OnMouseDown;
                Child.MouseWheel += OnMouseWheel;
                Child.MouseMove += OnMouseMove;
                Child.MouseUp += OnMouseUp;
            }
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RaiseEvent(new FormMouseMoveEventArgs(FormMouseMoveEvent, new Point(e.Location.X * DPIXScale, e.Location.Y * DPIYScale), e.X, e.Y, e.Delta) { Source = this });
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RaiseEvent(new FormMouseWheelEventArgs(FormMouseWheelEvent, Mouse.PrimaryDevice, Environment.TickCount, e.Delta)
            {
                Source = this,
            });
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            MouseButton? wpfButton = ConvertToWpf(mouseEventArgs.Button);
            if (!wpfButton.HasValue)
                return;
            Capture();
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, wpfButton.Value)
            {
                RoutedEvent = Mouse.PreviewMouseDownEvent,
                Source = this,
            });

            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, wpfButton.Value)
            {
                RoutedEvent = Mouse.MouseDownEvent,
                Source = this,
            });
        }
        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            MouseButton? wpfButton = ConvertToWpf(mouseEventArgs.Button);
            if (!wpfButton.HasValue)
                return;
            Capture();
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, wpfButton.Value)
            {
                RoutedEvent = Mouse.MouseUpEvent,
                Source = this,
            });
        }

        /// <summary>
        /// Has to do this, otherwise the mouse point is wrong in mouse event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Capture()
        {
            this.CaptureMouse();
            this.ReleaseMouseCapture();
        }

        private MouseButton? ConvertToWpf(MouseButtons winformButton)
        {
            switch (winformButton)
            {
                case MouseButtons.Left:
                    return MouseButton.Left;
                case MouseButtons.None:
                    return null;
                case MouseButtons.Right:
                    return MouseButton.Right;
                case MouseButtons.Middle:
                    return MouseButton.Middle;
                case MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case MouseButtons.XButton2:
                    return MouseButton.XButton2;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="System.Windows.RoutedEventArgs" />
        public sealed class FormMouseMoveEventArgs : RoutedEventArgs
        {
            //
            // Summary:
            //     Gets a signed count of the number of detents the mouse wheel has rotated, multiplied
            //     by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
            //
            // Returns:
            //     A signed count of the number of detents the mouse wheel has rotated, multiplied
            //     by the WHEEL_DELTA constant.
            public int Delta { get; private set; }
            //
            // Summary:
            //     Gets the location of the mouse during the generating mouse event.
            //
            // Returns:
            //     A System.Drawing.Point that contains the x- and y- mouse coordinates, in pixels,
            //     relative to the upper-left corner of the form.
            public Point Location { get; private set; }
            //
            // Summary:
            //     Gets the x-coordinate of the mouse during the generating mouse event.
            //
            // Returns:
            //     The x-coordinate of the mouse, in pixels.
            public int X { get; private set; }
            //
            // Summary:
            //     Gets the y-coordinate of the mouse during the generating mouse event.
            //
            // Returns:
            //     The y-coordinate of the mouse, in pixels.
            public int Y { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="FormMouseMoveEventArgs"/> class.
            /// </summary>
            /// <param name="routedEvent">The routed event.</param>
            /// <param name="p">The p.</param>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="delta">The delta.</param>
            public FormMouseMoveEventArgs(RoutedEvent routedEvent, Point p, int x, int y, int delta)
                :base(routedEvent)
            {
                Location = p;
                X = x;
                Y = y;
                Delta = delta;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="System.Windows.Input.MouseWheelEventArgs" />
        public sealed class FormMouseWheelEventArgs : RoutedEventArgs
        {
            public readonly int Delta;
            public readonly int Timestamp;
            public readonly MouseDevice Mouse;
            /// <summary>
            /// Initializes a new instance of the <see cref="FormMouseWheelEventArgs"/> class.
            /// </summary>
            /// <param name="routedEvent"></param>
            /// <param name="mouse">The mouse device associated with this event.</param>
            /// <param name="timestamp">The time when the input occurred.</param>
            /// <param name="delta">The amount the wheel has changed.</param>
            public FormMouseWheelEventArgs(RoutedEvent routedEvent, MouseDevice mouse, int timestamp, int delta):base(routedEvent)
            {
                Delta = delta;
                Timestamp = timestamp;
                Mouse = mouse;
            }

            public static implicit operator MouseWheelEventArgs(FormMouseWheelEventArgs args)
            {
                return new MouseWheelEventArgs(args.Mouse, args.Timestamp, args.Delta) { RoutedEvent = MouseWheelEvent };
            }
        }
    }
}

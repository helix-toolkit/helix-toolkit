using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX.Controls
{
    public class WinformHostExtend : WindowsFormsHost
    {
        protected UIElement ParentControl { set; get; }
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
                previousChild.MouseEnter -= OnMouseEnter;
                previousChild.MouseLeave -= OnMouseLeave;
            }
            if (Child != null)
            {
                Child.MouseDown += OnMouseDown;
                Child.MouseWheel += OnMouseWheel;
                Child.MouseMove += OnMouseMove;
                Child.MouseUp += OnMouseUp;
                Child.MouseEnter += OnMouseEnter;
                Child.MouseLeave += OnMouseLeave;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            Capture();
            RaiseEvent(new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond)
            {
                RoutedEvent = Mouse.MouseLeaveEvent,
                Source = this,
            });
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Capture();
            RaiseEvent(new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond)
            {
                RoutedEvent = Mouse.MouseEnterEvent,
                Source = this,
            });
        }

        private System.Drawing.Point prevP;
        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(prevP == e.Location) { return; }
            prevP = e.Location;
            Capture();
            RaiseEvent(new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond)
            {
                RoutedEvent = Mouse.MouseMoveEvent,
                Source = this,
            });
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond, e.Delta)
            {
                RoutedEvent = Mouse.MouseWheelEvent,
                Source = this,
            });
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            MouseButton? wpfButton = ConvertToWpf(mouseEventArgs.Button);
            if (!wpfButton.HasValue)
                return;
            Capture();
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond, wpfButton.Value)
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
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond, wpfButton.Value)
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
                    throw new ArgumentOutOfRangeException("winformButton");
            }
        }
    }
}

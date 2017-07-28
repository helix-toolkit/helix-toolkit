using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX.Controls
{
    public class WinformHostExtend : WindowsFormsHost
    {
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
            }
            if (Child != null)
            {
                Child.MouseDown += OnMouseDown;
                Child.MouseWheel += OnMouseWheel;
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, 0, e.Delta)
            {
                RoutedEvent = Mouse.MouseWheelEvent,
                Source = this,
            });
        }

        private void Child_MouseUp(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            MouseButton? wpfButton = ConvertToWpf(mouseEventArgs.Button);
            if (!wpfButton.HasValue)
                return;

            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, wpfButton.Value)
            {
                RoutedEvent = Mouse.MouseUpEvent,
                Source = this,
            });
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            MouseButton? wpfButton = ConvertToWpf(mouseEventArgs.Button);
            if (!wpfButton.HasValue)
                return;
            this.CaptureMouse();
            this.ReleaseMouseCapture();
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, wpfButton.Value)
            {
                RoutedEvent = Mouse.MouseDownEvent,
                Source = this,
            });
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

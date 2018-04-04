using SharpDX;
using System;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace HelixToolkit.UWP
{

    /// <summary>
    /// Use to determine shortcut keys/Mouse key for Rotation, Pan, etc.
    /// User must override certain functions to change default shortcut keys/buttons.
    /// </summary>
    public class InputController
    {
        public sealed class AddForceEventArgs : EventArgs
        {
            public readonly Vector2 Force;
            public readonly Vector3 Move;

            public AddForceEventArgs(Vector2 f)
            {
                Force = f;
            }

            public AddForceEventArgs(Vector3 f)
            {
                Move = f;
            }
        }
        #region Events
        /// <summary>
        /// Occurs when [on start zoom].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnStartZoom;

        /// <summary>
        /// Occurs when [on start rotate].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnStartRotate;
        /// <summary>
        /// Occurs when [on start zoom extends].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnStartZoomExtends;
        /// <summary>
        /// Occurs when [on start zoom rectangle].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnStartZoomRectangle;
        /// <summary>
        /// Occurs when [on start pan].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnStartPan;
        /// <summary>
        /// Occurs when [on reset camera].
        /// </summary>
        public event EventHandler OnResetCamera;
        /// <summary>
        /// Occurs when [on change look at].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnChangeLookAt;
        /// <summary>
        /// Occurs when [on change field of view].
        /// </summary>
        public event EventHandler<PointerRoutedEventArgs> OnChangeFieldOfView;
        /// <summary>
        /// Occurs when [on top view].
        /// </summary>
        public event EventHandler OnTopView;

        protected void RaiseOnTopView() { OnTopView?.Invoke(Viewport, EventArgs.Empty); }
        /// <summary>
        /// Occurs when [on bottom view].
        /// </summary>
        public event EventHandler OnBottomView;

        protected void RaiseOnBottomView() { OnBottomView?.Invoke(Viewport, EventArgs.Empty); }
        /// <summary>
        /// Occurs when [on left view].
        /// </summary>
        public event EventHandler OnLeftView;

        protected void RaiseOnLeftView() { OnLeftView?.Invoke(Viewport, EventArgs.Empty); }
        /// <summary>
        /// Occurs when [on right view].
        /// </summary>
        public event EventHandler OnRightView;

        protected void RaiseOnRightView() { OnRightView?.Invoke(Viewport, EventArgs.Empty); }
        /// <summary>
        /// Occurs when [on front view].
        /// </summary>
        public event EventHandler OnFrontView;

        protected void RaiseOnFrontView() { OnFrontView?.Invoke(Viewport, EventArgs.Empty); }
        /// <summary>
        /// Occurs when [on back view].
        /// </summary>
        public event EventHandler OnBackView;

        protected void RaiseOnBackView() { OnBackView?.Invoke(Viewport, EventArgs.Empty); }

        /// <summary>
        /// Occurs when [on add rotation force].
        /// </summary>
        public event EventHandler<AddForceEventArgs> OnAddRotationForce;

        protected void RaiseOnAddRotationForce(AddForceEventArgs args)
        {
            OnAddRotationForce?.Invoke(Viewport, args);
        }
        /// <summary>
        /// Occurs when [on add pan force].
        /// </summary>
        public event EventHandler<AddForceEventArgs> OnAddPanForce;

        protected void RaiseOnAddPanForce(AddForceEventArgs args)
        {
            OnAddPanForce?.Invoke(Viewport, args);
        }
        /// <summary>
        /// Occurs when [on add zoom force].
        /// </summary>
        public event EventHandler<AddForceEventArgs> OnAddZoomForce;

        protected void RaiseOnAddZoomForce(AddForceEventArgs args)
        {
            OnAddZoomForce?.Invoke(Viewport, args);
        }
        /// <summary>
        /// Occurs when [on add move force].
        /// </summary>
        public event EventHandler<AddForceEventArgs> OnAddMoveForce;

        protected void RaiseOnAddMoveForce(AddForceEventArgs args)
        {
            OnAddMoveForce?.Invoke(Viewport, args);
        }
        /// <summary>
        /// Occurs when [on restore camera settings].
        /// </summary>
        public event EventHandler OnRestoreCameraSettings;

        protected void RaiseOnRestoreCameraSettings()
        {
            OnRestoreCameraSettings?.Invoke(Viewport, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// Gets or sets the viewport.
        /// </summary>
        /// <value>
        /// The viewport.
        /// </value>
        public Viewport3DX Viewport { set; get; }

        #region Mouse Buttons        
        /// <summary>
        /// Raises the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        public virtual void OnPointerPressed(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(Viewport).Properties;
            if (IsStartRotate(p))
            {
                OnStartRotate?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsStartPan(p))
            {
                OnStartPan?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsStartZoom(p))
            {
                OnStartZoom?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsStartZoomExtends(p))
            {
                OnStartZoomExtends?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsStartZoomRectangle(p))
            {
                OnStartZoomRectangle?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsResetCamera(p))
            {
                OnResetCamera?.Invoke(Viewport, EventArgs.Empty);
                e.Handled = true;
            }
            else if (IsChangeFieldOfView(p))
            {
                OnChangeFieldOfView?.Invoke(Viewport, e);
                e.Handled = true;
            }
            else if (IsChangeLookAt(p))
            {
                OnChangeLookAt?.Invoke(Viewport, e);
                e.Handled = true;
            }
        }

        protected virtual bool IsStartRotate(PointerPointProperties properties)
        {
            return properties.IsRightButtonPressed;
        }

        protected virtual bool IsStartPan(PointerPointProperties properties)
        {
            return properties.IsLeftButtonPressed;
        }

        protected virtual bool IsStartZoomExtends(PointerPointProperties properties)
        {
            return false;
        }

        protected virtual bool IsStartZoom(PointerPointProperties properties)
        {
            return properties.MouseWheelDelta != 0;
        }

        protected virtual bool IsStartZoomRectangle(PointerPointProperties properties)
        {
            return false;
        }

        protected virtual bool IsResetCamera(PointerPointProperties properties)
        {
            return false;
        }

        protected virtual bool IsChangeLookAt(PointerPointProperties properties)
        {
            return false;
        }

        protected virtual bool IsChangeFieldOfView(PointerPointProperties properties)
        {
            return false;
        }
        #endregion

        #region Keyboards
        public virtual void OnKeyPressed(KeyRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                OnKeyboardStartRotate(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardStartPan(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardStartZoom(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardChangeView(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardMoveCamera(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardStartZoomExtends(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardStartZoomRectangle(e);
            }
            else { return; }
            if (!e.Handled)
            {
                OnKeyboardResetCamera(e);
            }
            else { return; }
        }

        protected virtual void OnKeyboardStartRotate(KeyRoutedEventArgs e)
        {
            var f = IsCtrlKeyPressed() ? 0.25f : 1;
            if (!IsShiftKeyPressed())
            {
                switch (e.Key)
                {
                    case VirtualKey.Left:
                        OnAddRotationForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(-1 * f * (float)Viewport.LeftRightRotationSensitivity, 0)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Right:
                        OnAddRotationForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(1 * f * (float)Viewport.LeftRightRotationSensitivity, 0)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Up:
                        OnAddRotationForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(0, -1 * f * (float)Viewport.UpDownRotationSensitivity)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Down:
                        OnAddRotationForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(0, 1 * f * (float)Viewport.UpDownRotationSensitivity)));
                        e.Handled = true;
                        break;
                }
            }
        }

        protected virtual void OnKeyboardStartPan(KeyRoutedEventArgs e)
        {
            var f = IsCtrlKeyPressed() ? 0.25f : 1;
            if (IsShiftKeyPressed())
            {
                switch (e.Key)
                {
                    case VirtualKey.Left:
                        OnAddPanForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(-1 * f * (float)Viewport.LeftRightPanSensitivity, 0)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Right:
                        OnAddPanForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(1 * f * (float)Viewport.LeftRightPanSensitivity, 0)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Up:
                        OnAddPanForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(0, -1 * f * (float)Viewport.UpDownPanSensitivity)));
                        e.Handled = true;
                        break;
                    case VirtualKey.Down:
                        OnAddPanForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(0, 1 * f * (float)Viewport.UpDownPanSensitivity)));
                        e.Handled = true;
                        break;
                }
            }
        }

        protected virtual void OnKeyboardStartZoom(KeyRoutedEventArgs e)
        {
            var f = IsCtrlKeyPressed() ? 0.25f : 1;
            switch (e.Key)
            {
                case VirtualKey.PageUp:
                    OnAddZoomForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(-0.1f * f * (float)Viewport.PageUpDownZoomSensitivity, 0)));
                    e.Handled = true;
                    break;
                case VirtualKey.PageDown:
                    OnAddZoomForce?.Invoke(Viewport, new AddForceEventArgs(new Vector2(0.1f * f * (float)Viewport.PageUpDownZoomSensitivity, 0)));
                    e.Handled = true;
                    break;
                case VirtualKey.Back:
                    OnRestoreCameraSettings?.Invoke(Viewport, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
        }

        protected virtual void OnKeyboardMoveCamera(KeyRoutedEventArgs e)
        {
            var f = IsCtrlKeyPressed() ? 0.25f : 1;
            switch (e.Key)
            {
                case VirtualKey.W:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(0, 0, 0.1f * f * (float)Viewport.MoveSensitivity)));
                    break;
                case VirtualKey.A:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(0.1f * f * (float)Viewport.LeftRightPanSensitivity, 0, 0)));
                    break;
                case VirtualKey.S:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(0, 0, -0.1f * f * (float)Viewport.MoveSensitivity)));
                    break;
                case VirtualKey.D:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(-0.1f * f * (float)Viewport.LeftRightPanSensitivity, 0, 0)));
                    break;
                case VirtualKey.Z:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(0, -0.1f * f * (float)Viewport.LeftRightPanSensitivity, 0)));
                    break;
                case VirtualKey.Q:
                    OnAddMoveForce?.Invoke(Viewport, new AddForceEventArgs(new Vector3(0, 0.1f * f * (float)Viewport.LeftRightPanSensitivity, 0)));
                    break;
            }
        }

        protected virtual void OnKeyboardStartZoomExtends(KeyRoutedEventArgs e)
        {
            
        }

        protected virtual void OnKeyboardStartZoomRectangle(KeyRoutedEventArgs e)
        {

        }

        protected virtual void OnKeyboardResetCamera(KeyRoutedEventArgs e)
        {
        }

        protected virtual void OnKeyboardChangeView(KeyRoutedEventArgs e)
        {
            if (IsCtrlKeyPressed())
            {
                switch (e.Key)
                {
                    case VirtualKey.PageUp:
                    case VirtualKey.F:
                        OnFrontView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case VirtualKey.PageDown:
                    case VirtualKey.B:
                        OnBackView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case VirtualKey.Left:
                    case VirtualKey.L:
                        OnLeftView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case VirtualKey.Right:
                    case VirtualKey.R:
                        OnRightView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case VirtualKey.Up:
                    case VirtualKey.U:
                        OnTopView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case VirtualKey.Down:
                    case VirtualKey.D:
                        OnBottomView?.Invoke(Viewport, EventArgs.Empty);
                        e.Handled = true;
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCtrlKeyPressed()
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsShiftKeyPressed()
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        #endregion
    }
}

using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Wii;

public static class MouseHelper
{
    // todo: use SendInput
    // http://msdn.microsoft.com/en-us/library/ms646310(v=vs.85).aspx

    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAsAttribute(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(int x, int y);

    private const int INPUT_MOUSE = 0;
    private const int INPUT_KEYBOARD = 1;
    private const int INPUT_HARDWARE = 2;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_UNICODE = 0x0004;
    private const uint KEYEVENTF_SCANCODE = 0x0008;
    private const uint XBUTTON1 = 0x0001;
    private const uint XBUTTON2 = 0x0002;
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const uint MOUSEEVENTF_XDOWN = 0x0080;
    private const uint MOUSEEVENTF_XUP = 0x0100;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;
    private const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

    [DllImport("user32.dll", EntryPoint = "mouse_event")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

    public static void SimulateMouseEvent(MouseButton button, MouseButtonState state)
    {
        uint x = 0;
        uint y = 0;
        switch (button)
        {
            case MouseButton.Left:
                if (state == MouseButtonState.Pressed)
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                else
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                break;
            case MouseButton.Middle:
                if (state == MouseButtonState.Pressed)
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, x, y, 0, 0);
                else
                    mouse_event(MOUSEEVENTF_MIDDLEUP, x, y, 0, 0);
                break;
            case MouseButton.Right:
                if (state == MouseButtonState.Pressed)
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                else
                    mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
                break;
        }
    }
}

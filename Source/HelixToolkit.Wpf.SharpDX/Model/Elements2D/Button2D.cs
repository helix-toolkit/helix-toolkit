using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class Button2D : Clickable2D
    {
        public Button2D()
        {
            Background = SystemColors.ControlBrush;
            Foreground = SystemColors.ControlTextBrush;
            CornerRadius = 4;
            StrokeThickness = 0.5;
            Stroke = SystemColors.ActiveBorderBrush;
        }

        protected override void OnMouseOverChanged(bool newValue, bool oldValue)
        {
            if (newValue)
            {
                Background = SystemColors.HighlightBrush;
            }
            else
            {
                Background = SystemColors.ControlBrush;
            }
        }
    }
}
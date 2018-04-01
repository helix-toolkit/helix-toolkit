using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public abstract class Clickable2D : Border2D
    {
        public static long DoubleClickThreshold = 300;

        #region Dependency Properties
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(Clickable2D), new PropertyMetadata(null));
        public ICommand Command
        {
            set
            {
                SetValue(CommandProperty, value);
            }
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
        }

        #endregion

        #region Events
        public static readonly RoutedEvent Clicked2DEvent =
            EventManager.RegisterRoutedEvent("Clicked2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Clickable2D));

        public event Mouse2DRoutedEventHandler Clicked2D
        {
            add { AddHandler(Clicked2DEvent, value); }
            remove { RemoveHandler(Clicked2DEvent, value); }
        }

        public static readonly RoutedEvent DoubleClicked2DEvent =
            EventManager.RegisterRoutedEvent("DoubleClicked2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Clickable2D));

        public event Mouse2DRoutedEventHandler DoubleClicked2D
        {
            add { AddHandler(DoubleClicked2DEvent, value); }
            remove { RemoveHandler(DoubleClicked2DEvent, value); }
        }
        #endregion

        private long lastClickedTime = 0;

        public Clickable2D()
        {
            MouseDown2D += Clickable2D_MouseDown2D;
            MouseEnter2D += Clickable2D_MouseEnter2D;
            MouseLeave2D += Clickable2D_MouseLeave2D;
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new ClickableNode2D();
        }

        private void Clickable2D_MouseLeave2D(object sender, Mouse2DEventArgs e)
        {

        }

        private void Clickable2D_MouseEnter2D(object sender, Mouse2DEventArgs e)
        {

        }

        private void Clickable2D_MouseDown2D(object sender, Mouse2DEventArgs e)
        {
            if(e.InputArgs is TouchEventArgs || (e.InputArgs is MouseEventArgs && (e.InputArgs as MouseEventArgs).LeftButton == MouseButtonState.Pressed))
            {
                long time = e.InputArgs.Timestamp;
                if (time - lastClickedTime < DoubleClickThreshold)
                {
                    RaiseEvent(new Mouse2DEventArgs(DoubleClicked2DEvent, this));
#if DEBUG
                    Debug.WriteLine("DoubleClicked2DEvent");
#endif
                }
                else
                {
                    RaiseEvent(new Mouse2DEventArgs(Clicked2DEvent, this));
#if DEBUG
                    Debug.WriteLine("Clicked2DEvent");
#endif
                    Command?.Execute(e);
                }
                lastClickedTime = time;
            }
        }
    }
}

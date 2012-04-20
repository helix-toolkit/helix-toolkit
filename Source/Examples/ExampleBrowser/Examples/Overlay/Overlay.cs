namespace OverlayDemo
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// The overlay.
    /// </summary>
    public class Overlay : DependencyObject
    {
        #region Constants and Fields

        /// <summary>
        /// The position 3 d property.
        /// </summary>
        public static readonly DependencyProperty Position3DProperty = DependencyProperty.RegisterAttached(
            "Position3D", typeof(Point3D), typeof(Overlay));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get position 3 d.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// </returns>
        public static Point3D GetPosition3D(DependencyObject obj)
        {
            return (Point3D)obj.GetValue(Position3DProperty);
        }

        /// <summary>
        /// The set position 3 d.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetPosition3D(DependencyObject obj, Point3D value)
        {
            obj.SetValue(Position3DProperty, value);
        }

        #endregion

        // Using a DependencyProperty as the backing store for Position3D.  This enables animation, styling, binding, etc...
    }
}
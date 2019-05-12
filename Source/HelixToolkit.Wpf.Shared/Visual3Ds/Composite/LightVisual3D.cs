// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a model for the specified light.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a model for the specified light.
    /// </summary>
    public class LightVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Light"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LightProperty = DependencyProperty.Register(
            "Light", typeof(Light), typeof(LightVisual3D), new UIPropertyMetadata(null, LightChanged));

        /// <summary>
        /// Gets or sets the light.
        /// </summary>
        /// <value>The light.</value>
        public Light Light
        {
            get
            {
                return (Light)this.GetValue(LightProperty);
            }

            set
            {
                this.SetValue(LightProperty, value);
            }
        }

        /// <summary>
        /// The light changed.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected static void LightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((LightVisual3D)obj).OnLightChanged();
        }

        /// <summary>
        /// Called when the light changed.
        /// </summary>
        protected virtual void OnLightChanged()
        {
            this.Children.Clear();
            if (this.Light == null)
            {
                return;
            }

            var dl = this.Light as DirectionalLight;
            if (dl != null)
            {
                var arrow = new ArrowVisual3D();
                double distance = 10;
                double length = 5;
                arrow.BeginEdit();
                arrow.Point1 = new Point3D() + dl.Direction * distance;
                arrow.Point2 = arrow.Point1 - dl.Direction * length;
                arrow.Diameter = 0.1 * length;
                arrow.Fill = new SolidColorBrush(dl.Color);
                arrow.EndEdit();
                this.Children.Add(arrow);
            }

            var sl = this.Light as SpotLight;
            if (sl != null)
            {
                var sphere = new SphereVisual3D();
                sphere.BeginEdit();
                sphere.Center = sl.Position;
                sphere.Fill = new SolidColorBrush(sl.Color);
                sphere.EndEdit();
                this.Children.Add(sphere);

                var arrow = new ArrowVisual3D();
                arrow.BeginEdit();
                arrow.Point1 = sl.Position;
                arrow.Point2 = sl.Position + sl.Direction;
                arrow.Diameter = 0.1;
                arrow.EndEdit();
                this.Children.Add(arrow);
            }

            var pl = this.Light as PointLight;
            if (pl != null)
            {
                var sphere = new SphereVisual3D();
                sphere.BeginEdit();
                sphere.Center = pl.Position;
                sphere.Fill = new SolidColorBrush(pl.Color);
                sphere.EndEdit();
                this.Children.Add(sphere);
            }

            var al = this.Light as AmbientLight;
        }

    }
}
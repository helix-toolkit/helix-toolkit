// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightSetup.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An abstract base class for light models.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// An abstract base class for light models.
    /// </summary>
    public abstract class LightSetup : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="ShowLights"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLightsProperty = DependencyProperty.Register(
            "ShowLights", typeof(bool), typeof(LightSetup), new UIPropertyMetadata(false, ShowLightsChanged));

        /// <summary>
        /// The light group.
        /// </summary>
        private readonly Model3DGroup lightGroup = new Model3DGroup();

        /// <summary>
        /// The lights visual.
        /// </summary>
        private readonly ModelVisual3D lightsVisual = new ModelVisual3D();

        /// <summary>
        /// Initializes a new instance of the <see cref = "LightSetup" /> class.
        /// </summary>
        protected LightSetup()
        {
            this.Content = this.lightGroup;
            this.Children.Add(this.lightsVisual);
            this.OnSetupChanged();
            this.OnShowLightsChanged();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show light visuals.
        /// </summary>
        public bool ShowLights
        {
            get
            {
                return (bool)this.GetValue(ShowLightsProperty);
            }

            set
            {
                this.SetValue(ShowLightsProperty, value);
            }
        }

        /// <summary>
        /// The setup changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void SetupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LightSetup)d).OnSetupChanged();
        }

        /// <summary>
        /// Adds the lights to the element.
        /// </summary>
        /// <param name="lightGroup">
        /// The light group.
        /// </param>
        protected abstract void AddLights(Model3DGroup lightGroup);

        /// <summary>
        /// Handles changes to the light setup.
        /// </summary>
        protected void OnSetupChanged()
        {
            this.lightGroup.Children.Clear();
            this.AddLights(this.lightGroup);
        }

        /// <summary>
        /// Called when show lights is changed.
        /// </summary>
        protected void OnShowLightsChanged()
        {
            this.lightsVisual.Children.Clear();
            if (this.ShowLights)
            {
                foreach (var light in this.lightGroup.Children)
                {
                    var pl = light as PointLight;
                    if (pl != null)
                    {
                        var sphere = new SphereVisual3D();
                        sphere.BeginEdit();
                        sphere.Center = pl.Position;
                        sphere.Radius = 1.0;
                        sphere.Fill = new SolidColorBrush(pl.Color);
                        sphere.EndEdit();
                        this.lightsVisual.Children.Add(sphere);
                    }

                    var dl = light as DirectionalLight;
                    if (dl != null)
                    {
                        var dir = dl.Direction;
                        dir.Normalize();

                        var target = new Point3D(0, 0, 0);
                        var source = target - (dir * 20);
                        var p2 = source + (dir * 10);

                        var sphere = new SphereVisual3D();
                        sphere.BeginEdit();
                        sphere.Center = source;
                        sphere.Radius = 1.0;
                        sphere.Fill = new SolidColorBrush(dl.Color);
                        sphere.EndEdit();
                        this.lightsVisual.Children.Add(sphere);

                        var arrow = new ArrowVisual3D();
                        arrow.BeginEdit();
                        arrow.Point1 = source;
                        arrow.Point2 = p2;
                        arrow.Diameter = 0.5;
                        arrow.Fill = new SolidColorBrush(dl.Color);
                        arrow.EndEdit();
                        this.lightsVisual.Children.Add(arrow);
                    }

                    var al = light as AmbientLight;
                    if (al != null)
                    {
                        var pos = new Point3D(0, 0, 20);
                        this.lightsVisual.Children.Add(
                            new CubeVisual3D { Center = pos, SideLength = 1.0, Fill = new SolidColorBrush(al.Color) });
                    }
                }
            }
        }

        /// <summary>
        /// The show lights changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ShowLightsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LightSetup)d).OnShowLightsChanged();
        }

    }
}
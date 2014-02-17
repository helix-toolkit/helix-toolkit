namespace HelixToolkit.Win8
{
    using HelixToolkit.Win8.CommonDX;

    using Windows.UI.Xaml;

    /// <summary>
    /// Base class for 3D elements.
    /// </summary>
    public abstract class Element3D : FrameworkElement
    {
        /// <summary>
        /// Initializes the element.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        public virtual void Initialize(DeviceManager deviceManager)
        {
        }

        /// <summary>
        /// Renders the element.
        /// </summary>
        /// <param name="render">The render.</param>
        public virtual void Render(TargetBase render)
        {
        }
    }
}

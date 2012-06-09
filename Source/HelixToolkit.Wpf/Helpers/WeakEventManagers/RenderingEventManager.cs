namespace HelixToolkit.Wpf
{
    using System.Windows.Media;

    /// <summary>
    /// Represents a weak event manager for the CompositionTarget.Rendering event.
    /// </summary>
    public class RenderingEventManager : WeakEventManagerBase<RenderingEventManager>
    {
        /// <summary>
        /// Start listening to the CompositionTarget.Rendering event.
        /// </summary>
        protected override void StartListening()
        {
            CompositionTarget.Rendering += base.Handler;
        }

        /// <summary>
        /// Stop listening to the CompositionTarget.Rendering event.
        /// </summary>
        protected override void StopListening()
        {
            CompositionTarget.Rendering -= base.Handler;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

 namespace Workitem10051
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Extensions;
    using HelixToolkit.Wpf.SharpDX.Utilities;

    public class MainViewModel : BaseViewModel
    {
        private Exception renderException;

        private string viewportMessage;

        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10051)";
            this.SubTitle = "LineGeometryModel3D now works with OrthographicCamera and Intel HD 3000.";
            this.PropertyChanged += this.OnPropertyChanged;

            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Mesh];
        }

        /// <summary>
        /// Gets or sets the render exception.
        /// </summary>
        public Exception RenderException
        {
            get
            {
                return this.renderException;
            }

            set
            {
                if (this.renderException != value)
                {
                    this.renderException = value;
                    this.OnPropertyChanged("RenderException");
                }
            }
        }

        /// <summary>
        /// Gets or sets the viewport message.
        /// </summary>
        public string ViewportMessage
        {
            get
            {
                return this.viewportMessage;
            }

            set
            {
                if (this.viewportMessage != value)
                {
                    this.viewportMessage = value;
                    this.OnPropertyChanged("ViewportMessage");
                }
            }
        }

        /// <summary>
        /// Handles exceptions at the rendering subsystem.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        public void HandleRenderException(object sender, RelayExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.ToString(), "RenderException");
            }
        }

        /// <summary>
        /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("RenderException".Equals(e.PropertyName))
            {
                this.ViewportMessage = this.RenderException != null ? this.RenderException.ToString() : null;
            }
        }
    }
}

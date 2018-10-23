using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX.Cameras;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Render;
using HelixToolkit.Wpf.SharpDX.Utilities;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;

namespace HelixToolkit.Wpf.SharpDX
{
    [DefaultProperty("Children")]
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(ContentPresenter))]
    public class ScreenDuplicationViewport3DX : ItemsControl, IViewport3DX
    {
        /// <summary>
        /// The EffectsManager property.
        /// </summary>
        public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
            "EffectsManager", typeof(IEffectsManager), typeof(ScreenDuplicationViewport3DX), new PropertyMetadata(
                null,
                (s, e) => ((ScreenDuplicationViewport3DX)s).EffectsManagerPropertyChanged()));

        /// <summary>
        /// Gets or sets the <see cref="IEffectsManager"/>.
        /// </summary>
        public IEffectsManager EffectsManager
        {
            get { return (IEffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
        }
        /// <summary>
        /// The Render Technique property
        /// </summary>
        public static readonly DependencyProperty RenderTechniqueProperty = DependencyProperty.Register(
            "RenderTechnique", typeof(IRenderTechnique), typeof(ScreenDuplicationViewport3DX), new PropertyMetadata(null,
                (s, e) => ((ScreenDuplicationViewport3DX)s).RenderTechniquePropertyChanged()));

        /// <summary>
        /// Gets or sets value for the shading model shading is used
        /// </summary>
        /// <value>
        /// <c>true</c> if deferred shading is enabled; otherwise, <c>false</c>.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            get { return (IRenderTechnique)this.GetValue(RenderTechniqueProperty); }
            set { this.SetValue(RenderTechniqueProperty, value); }
        }

        /// <summary>
        /// The render exception property.
        /// </summary>
        public static DependencyProperty RenderExceptionProperty = DependencyProperty.Register(
            "RenderException", typeof(Exception), typeof(ScreenDuplicationViewport3DX), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="System.Exception"/> that occured at rendering subsystem.
        /// </summary>
        public Exception RenderException
        {
            get { return (Exception)this.GetValue(RenderExceptionProperty); }
            set { this.SetValue(RenderExceptionProperty, value); }
        }

        /// <summary>
        /// The message text property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(
            "MessageText", typeof(string), typeof(ScreenDuplicationViewport3DX), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        /// <value>
        /// The message text.
        /// </value>
        public string MessageText
        {
            get
            {
                return (string)this.GetValue(MessageTextProperty);
            }

            set
            {
                this.SetValue(MessageTextProperty, value);
            }
        }

        /// <summary>
        /// Background Color property.this.RenderHost
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color), typeof(ScreenDuplicationViewport3DX),
            new PropertyMetadata(Colors.White, (s, e) =>
            {
                if (((ScreenDuplicationViewport3DX)s).renderHostInternal != null)
                {
                    ((ScreenDuplicationViewport3DX)s).renderHostInternal.ClearColor = ((Color)e.NewValue).ToColor4();
                }
            }));

        /// <summary>
        /// Background Color
        /// </summary>
        public Color BackgroundColor
        {
            get { return (Color)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }


        public CameraCore CameraCore { get; } = new PerspectiveCameraCore();

        public global::SharpDX.Matrix WorldMatrix { get; } = global::SharpDX.Matrix.Identity;

        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                if (renderHostInternal != null)
                {
                    foreach (Element3D item in Items)
                    {
                        yield return item.SceneNode;
                    }
                }
            }
        }

        public IEnumerable<SceneNode2D> D2DRenderables
        {
            get { return Enumerable.Empty<SceneNode2D>(); }
        }

        public IRenderHost RenderHost { private set { renderHostInternal = value; } get { return renderHostInternal; } }

        public bool IsShadowMappingEnabled { get { return false; } }

        public global::SharpDX.Rectangle ViewportRectangle { get { return new global::SharpDX.Rectangle(); } }

        private IRenderHost renderHostInternal;

        private bool IsAttached = false;

        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }

        /// <summary>
        /// Fired whenever an exception occurred at rendering subsystem.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> RenderExceptionOccurred = delegate { };

        static ScreenDuplicationViewport3DX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ScreenDuplicationViewport3DX), new FrameworkPropertyMetadata(typeof(ScreenDuplicationViewport3DX)));
        }
        /// <summary>
        /// Handles the change of the effects manager.
        /// </summary>
        private void EffectsManagerPropertyChanged()
        {
            if (this.renderHostInternal != null)
            {
                this.renderHostInternal.EffectsManager = this.EffectsManager;
            }
        }

        /// <summary>
        /// Handles the change of the render technique        
        /// </summary>
        private void RenderTechniquePropertyChanged()
        {
            if (this.renderHostInternal != null)
            {
                // remove the scene
                this.renderHostInternal.Viewport = null;

                // if new rendertechnique set, attach the scene
                if (this.RenderTechnique != null)
                {
                    this.renderHostInternal.Viewport = this;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (IsInDesignMode)
            { return; }
            if (this.renderHostInternal != null)
            {
                this.renderHostInternal.ExceptionOccurred -= this.HandleRenderException;
            }
            var hostPresenter = this.GetTemplateChild("PART_Canvas") as ContentPresenter;
            hostPresenter.Content = new DPFSurfaceSwapChain((surface) => { return new ScreenCloneRenderHost(surface); });
            renderHostInternal = (hostPresenter.Content as IRenderCanvas).RenderHost;
            this.renderHostInternal.ExceptionOccurred += this.HandleRenderException;
            this.renderHostInternal.Viewport = this;
            this.renderHostInternal.EffectsManager = this.EffectsManager;
            this.renderHostInternal.ClearColor = this.BackgroundColor.ToColor4();
        }

        /// <summary>
        /// Handles a rendering exception.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleRenderException(object sender, RelayExceptionEventArgs e)
        {
            var bindingExpression = this.GetBindingExpression(RenderExceptionProperty);
            if (bindingExpression != null)
            {
                // If RenderExceptionProperty is bound, we assume the exception will be handled.
                this.RenderException = e.Exception;
                e.Handled = true;
            }

            // Fire RenderExceptionOccurred event
            this.RenderExceptionOccurred(sender, e);

            // If the Exception is still unhandled...
            if (!e.Handled)
            {
                // ... prevent a MessageBox.Show().
                this.MessageText = e.Exception.ToString();
                e.Handled = true;
            }
        }

        public void Attach(IRenderHost host)
        {
            if (!IsAttached)
            {
                foreach (var e in this.Renderables)
                {
                    e.Attach(host);
                }
                IsAttached = true;
            }
        }

        public void Detach()
        {
            if (IsAttached)
            {
                IsAttached = false;
                foreach (var e in this.Renderables)
                {
                    e.Detach();
                }
            }
        }

        public void InvalidateRender()
        {
            renderHostInternal?.InvalidateRender();
        }

        public void InvalidateSceneGraph()
        {
            renderHostInternal?.InvalidateSceneGraph();
        }

        public void Update(TimeSpan timeStamp)
        {
            
        }
    }
}

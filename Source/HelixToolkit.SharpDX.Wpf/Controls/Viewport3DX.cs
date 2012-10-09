
namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    using global::SharpDX;


    public class Element3DCollection : List<Element3D> { }

    [DefaultEvent("OnChildrenChanged"), DefaultProperty("Children")]
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(DPFCanvas))]
    public class Viewport3DX : ItemsControl, IRenderable
    {
        public Camera Camera
        {
            get { return (Camera)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public FpsCounter FpsCounter
        {
            get { return (FpsCounter)GetValue(FpsCounterProperty); }
            private set { SetValue(FpsCounterProperty, value); }
        }

        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register("Camera", typeof(Camera), typeof(Viewport3DX));

        public static readonly DependencyProperty FpsCounterProperty =
            DependencyProperty.Register("FpsCounter", typeof(FpsCounter), typeof(Viewport3DX), new PropertyMetadata(new FpsCounter()));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Bindable(true)]
        public Element3DCollection Children
        {
            get { return (Element3DCollection)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(Element3DCollection), typeof(Viewport3DX));

        static Viewport3DX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Viewport3DX), new FrameworkPropertyMetadata(typeof(Viewport3DX)));
        }

        public Viewport3DX()
        {
            this.Children = new Element3DCollection();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Canvas = this.GetTemplateChild("PART_Canvas") as DPFCanvas;
            this.Canvas.Renderable = this;
        }

        protected DPFCanvas Canvas { get; private set; }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            //var py = ((ProjectionCamera)Camera).Position;
            //py.Y = py.Y + 0.1f;
            //((ProjectionCamera)Camera).Position = py;// new Vector3(0, 0, 100);
        }

        void IRenderable.Attach(IRenderHost host)
        {
            foreach (Element3D e in this.Items)
            {
                e.Attach(host);
            }

            /// --- start stop watch
            s_stopWatch.Start();

        }

        void IRenderable.Detach()
        {
            foreach (Element3D e in this.Items)
            {
                e.Detach();
            }
        }

        void IRenderable.Update(TimeSpan timeSpan)
        {
            foreach (Element3D e in this.Items)
            {
                e.Update(timeSpan);
            }
        }

        private static System.Diagnostics.Stopwatch s_stopWatch = new System.Diagnostics.Stopwatch();

        void IRenderable.Render()
        {
            this.FpsCounter.AddFrame(s_stopWatch.Elapsed);

            var context = new RenderContext(this.Camera, this.Canvas, Matrix.Identity);
            foreach (Element3D e in this.Items)
            {
                e.Render(context);
            }
        }
    }
}

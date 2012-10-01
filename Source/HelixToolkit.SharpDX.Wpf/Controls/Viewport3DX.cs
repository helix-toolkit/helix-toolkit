
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
    [ContentProperty("Children")]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(DPFCanvas))]
    public class Viewport3DX : Control, IRenderable
    {
        public Camera Camera
        {
            get { return (Camera)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register("Camera", typeof(Camera), typeof(Viewport3DX));

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
            ((ProjectionCamera)Camera).Position = new Vector3(0, 0, 100);
        }

        void IRenderable.Attach(IRenderHost host)
        {
            foreach (IRenderable e in this.Children)
            {
                e.Attach(host);
            }
        }

        void IRenderable.Detach()
        {
            foreach (IRenderable e in this.Children)
            {
                e.Detach();
            }
        }

        void IRenderable.Update(TimeSpan timeSpan)
        {
            foreach (IRenderable e in this.Children)
            {
                e.Update(timeSpan);
            }
        }

        void IRenderable.Render()
        {
            foreach (IRenderable e in this.Children)
            {
                e.Render();
            }
        }
    }
}

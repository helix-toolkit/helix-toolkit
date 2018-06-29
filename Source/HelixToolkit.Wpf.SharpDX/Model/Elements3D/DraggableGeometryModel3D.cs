// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DraggableGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Example class how to implement mouse dragging for objects.
//   Probably it should be moved to a "Dragging Demo."
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using System.Numerics;

    /// <summary>
    /// Example class how to implement mouse dragging for objects.
    /// Probably it should be moved to a "Dragging Demo."
    /// </summary>
    public class DraggableGeometryModel3D : MeshGeometryModel3D, ISelectable
    {
        protected bool isCaptured;
        protected Viewport3DX viewport;
        protected Camera camera;
        protected Vector3 lastHitPos;

        public static readonly DependencyProperty DragXProperty =
            DependencyProperty.Register("DragX", typeof(bool), typeof(DraggableGeometryModel3D), new PropertyMetadata(true));

        public static readonly DependencyProperty DragYProperty =
            DependencyProperty.Register("DragY", typeof(bool), typeof(DraggableGeometryModel3D), new PropertyMetadata(true));

        public static readonly DependencyProperty DragZProperty =
            DependencyProperty.Register("DragZ", typeof(bool), typeof(DraggableGeometryModel3D), new PropertyMetadata(true));


        public bool DragX
        {
            get { return (bool)this.GetValue(DragXProperty); }
            set { this.SetValue(DragXProperty, value); }
        }

        public bool DragY
        {
            get { return (bool)this.GetValue(DragYProperty); }
            set { this.SetValue(DragYProperty, value); }
        }

        public bool DragZ
        {
            get { return (bool)this.GetValue(DragZProperty); }
            set { this.SetValue(DragZProperty, value); }
        }

        public Vector3 LastHitPosition
        {
            get { return this.lastHitPos; }
        }

        protected override void OnMouse3DDown(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DDown(sender, e);

            if (!(e is Mouse3DEventArgs args)) return;
            if (args.Viewport == null) return;

            this.isCaptured = true;
            this.viewport = args.Viewport;
            this.camera = args.Viewport.Camera;
            this.lastHitPos = args.HitTestResult.PointHit;
        }

        protected override void OnMouse3DUp(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DUp(sender, e);
            if (this.isCaptured)
            {
                this.isCaptured = false;
                this.camera = null;
                this.viewport = null;
            }
        }

        protected override void OnMouse3DMove(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DMove(sender, e);
            if (this.isCaptured)
            {
                var args = e as Mouse3DEventArgs;

                // move dragmodel                         
                var normal = this.camera.CameraInternal.LookDirection;

                // hit position                        
                var newHit = this.viewport.UnProjectOnPlane(args.Position, lastHitPos, normal);
                if (newHit.HasValue)
                {
                    var offset = (newHit.Value - lastHitPos);
                    this.lastHitPos = newHit.Value;
                    if (Transform == null)
                    {
                        Transform = new System.Windows.Media.Media3D.TranslateTransform3D(offset.ToVector3D());
                    }
                    else
                    {
                        this.Transform = new System.Windows.Media.Media3D.MatrixTransform3D(Transform.AppendTransform(new System.Windows.Media.Media3D.TranslateTransform3D(offset.ToVector3D())).Value);
                    }
                }
            }
        }
    }
}
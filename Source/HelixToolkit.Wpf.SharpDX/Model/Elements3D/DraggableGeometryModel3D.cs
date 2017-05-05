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
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Example class how to implement mouse dragging for objects.
    /// Probably it should be moved to a "Dragging Demo."
    /// </summary>
    public class DraggableGeometryModel3D : MeshGeometryModel3D, ISelectable
    {
        protected bool isCaptured;
        protected Viewport3DX viewport;
        protected Camera camera;
        protected Point3D lastHitPos;
        private MatrixTransform3D dragTransform;

        public static readonly DependencyProperty DragXProperty =
            DependencyProperty.Register("DragX", typeof(bool), typeof(DraggableGeometryModel3D), new AffectsRenderPropertyMetadata(true));

        public static readonly DependencyProperty DragYProperty =
            DependencyProperty.Register("DragY", typeof(bool), typeof(DraggableGeometryModel3D), new AffectsRenderPropertyMetadata(true));

        public static readonly DependencyProperty DragZProperty =
            DependencyProperty.Register("DragZ", typeof(bool), typeof(DraggableGeometryModel3D), new AffectsRenderPropertyMetadata(true));


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

        public MatrixTransform3D DragTransform
        {
            get { return this.dragTransform; }
        }

        public Point3D LastHitPosition
        {
            get { return this.lastHitPos; }
        }

        public DraggableGeometryModel3D()
            : base()
        {
            this.dragTransform = new MatrixTransform3D(this.Transform.Value);
        }

        public override void OnMouse3DDown(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DDown(sender, e);

            var args = e as Mouse3DEventArgs;
            if (args == null) return;
            if (args.Viewport == null) return;

            this.isCaptured = true;
            this.viewport = args.Viewport;
            this.camera = args.Viewport.Camera;
            this.lastHitPos = args.HitTestResult.PointHit;
        }

        public override void OnMouse3DUp(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DUp(sender, e);
            if (this.isCaptured)
            {
                this.isCaptured = false;
                this.camera = null;
                this.viewport = null;
            }
        }

        public override void OnMouse3DMove(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DMove(sender, e);
            if (this.isCaptured)
            {
                var args = e as Mouse3DEventArgs;

                // move dragmodel                         
                var normal = this.camera.LookDirection;

                // hit position                        
                var newHit = this.viewport.UnProjectOnPlane(args.Position, lastHitPos, normal);
                if (newHit.HasValue)
                {
                    var offset = (newHit.Value - lastHitPos);
                    var trafo = this.Transform.Value;

                    if (this.DragX)
                        trafo.OffsetX += offset.X;

                    if (this.DragY)
                        trafo.OffsetY += offset.Y;

                    if (this.DragZ)
                        trafo.OffsetZ += offset.Z;

                    this.dragTransform.Matrix = trafo;

                    this.lastHitPos = newHit.Value;
                    this.Transform = this.dragTransform;
                }
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyLineGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Workitem10048
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Color = SharpDX.Color;
    using HitTestResult = HelixToolkit.Wpf.SharpDX.HitTestResult;

    public class MyLineGeometryModel3D : LineGeometryModel3D
    {
        private Color? initialColor = null;

        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            if (initialColor == null)
            {
                initialColor = this.Color;
            }

            var result = base.HitTest(rayWS, ref hits);
            var pressedMouseButtons = Viewport3DX.GetPressedMouseButtons();

            if (pressedMouseButtons == 0 || pressedMouseButtons.HasFlag(MouseButton.Left))
            {
                this.Color = result ? Color.Red : this.initialColor.Value;
            }

            return result;
        }

        // alternative way, 3.36 times slower
        protected bool MyHitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            LineGeometry3D lineGeometry3D;
            Viewport3DX viewport;

            if (this.Visibility == Visibility.Collapsed ||
                this.IsHitTestVisible == false ||
                (viewport = FindVisualAncestor<Viewport3DX>(this.renderHost as DependencyObject)) == null ||
                (lineGeometry3D = this.Geometry as LineGeometry3D) == null)
            {
                return false;
            }

            var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
            var lastDist = double.MaxValue;
            var index = 0;
            foreach (var line in lineGeometry3D.Lines)
            {
                var t0 = Vector3.TransformCoordinate(line.P0, this.ModelMatrix);
                var t1 = Vector3.TransformCoordinate(line.P1, this.ModelMatrix);
                Vector3 sp, tp;
                float sc, tc;
                var distance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out sp, out tp, out sc, out tc);
                var svpm = viewport.GetScreenViewProjectionMatrix();
                Vector4 sp4;
                Vector4 tp4;
                Vector3.Transform(ref sp, ref svpm, out sp4);
                Vector3.Transform(ref tp, ref svpm, out tp4);
                var sp3 = sp4.ToVector3();
                var tp3 = tp4.ToVector3();
                var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                var dist = tv2.Length();
                if (dist < lastDist && dist <= this.HitTestThickness)
                {
                    lastDist = dist;
                    result.PointHit = sp.ToPoint3D();
                    result.NormalAtHit = (sp - tp).ToVector3D(); // not normalized to get length
                    result.Distance = distance;
                    result.ModelHit = this;
                    result.IsValid = true;
                    result.Tag = index; // ToDo: LineHitTag with additional info
                }

                index++;
            }

            if (result.IsValid)
            {
                hits.Add(result);
            }

            return result.IsValid;
        }
    }
}

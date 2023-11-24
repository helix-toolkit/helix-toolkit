using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.Generic;
using System.Windows.Input;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using HitTestResult = HelixToolkit.SharpDX.HitTestResult;

namespace Workitem10048;

public class MyLineGeometryModel3D : LineGeometryModel3D
{
    private Color? initialColor = null;

    public override bool HitTest(HitTestContext? context, ref List<HitTestResult> hits)
    {
        if (initialColor == null)
        {
            initialColor = this.Color;
        }

        var result = base.HitTest(context, ref hits); // this.HitTest2D(rayWS, ref hits);
        var pressedMouseButtons = Viewport3DX.GetPressedMouseButtons();

        if (pressedMouseButtons == 0 || pressedMouseButtons.HasFlag(MouseButton.Left))
        {
            this.Color = result ? Colors.Red : this.initialColor.Value;
        }

        return result;
    }


    //// alternative way, 3.36 times faster, but wrong PointHit
    //protected bool HitTest2D(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
    //{
    //    LineGeometry3D lineGeometry3D;

    //    if (this.Visibility == Visibility.Collapsed ||
    //        this.IsHitTestVisible == false ||
    //        context == null ||
    //        (lineGeometry3D = this.Geometry as LineGeometry3D) == null)
    //    {
    //        return false;
    //    }

    //    // revert unprojection; probably better: overloaded HitTest() for LineGeometryModel3D?
    //    var svpm = context.ScreenViewProjectionMatrix;
    //    var smvpm = this.ModelMatrix * svpm;
    //    var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
    //    Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
    //    var clickPoint = clickPoint4.ToVector3();

    //    var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
    //    var maxDist = this.HitTestThickness;
    //    var lastDist = double.MaxValue;
    //    var index = 0;

    //    foreach (var line in lineGeometry3D.Lines)
    //    {
    //        var p0 = Vector3.TransformCoordinate(line.P0, smvpm);
    //        var p1 = Vector3.TransformCoordinate(line.P1, smvpm);
    //        Vector3 hitPoint;
    //        float t;

    //        var dist = LineBuilder.GetPointToLineDistance2D(ref clickPoint, ref p0, ref p1, out hitPoint, out t);
    //        if (dist < lastDist && dist <= maxDist)
    //        {
    //            lastDist = dist;
    //            Vector4 res;
    //            var lp0 = line.P0;
    //            var modelMatrix = ModelMatrix;
    //            Vector3.Transform(ref lp0, ref modelMatrix, out res);
    //            lp0 = res.ToVector3();

    //            var lp1 = line.P1;
    //            Vector3.Transform(ref lp1, ref modelMatrix, out res);
    //            lp1 = res.ToVector3();

    //            var lv = lp1 - lp0;
    //            var hitPointWS = lp0 + lv * t; // wrong, because t refers to screen space
    //            result.Distance = (rayWS.Position - hitPointWS).Length();
    //            result.PointHit = hitPointWS;
    //            result.ModelHit = this;
    //            result.IsValid = true;
    //            result.Tag = index; // ToDo: LineHitTag with additional info
    //        }

    //        index++;
    //    }

    //    if (result.IsValid)
    //    {
    //        hits.Add(result);
    //    }

    //    return result.IsValid;
    //}
}

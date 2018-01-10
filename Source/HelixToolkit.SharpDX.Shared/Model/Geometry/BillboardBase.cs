/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using SharpDX;
using System.IO;
using System.Linq;
#if NETFX_CORE

#else
using System.Windows.Media.Imaging;
#endif


#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using System.Diagnostics;

    public abstract class BillboardBase : Geometry3D, IBillboardText
    {
        public float Height
        {
            protected set;
            get;
        }

        public abstract BillboardType Type
        {
            get;
        }

        public virtual Stream Texture
        {
            protected set;
            get;
        }

        public float Width
        {
            protected set;
            get;
        }

        public IList<BillboardVertex> BillboardVertices { set; get; } = new List<BillboardVertex>();

        public BillboardBase()
        {
        }

        public virtual void DrawTexture()
        {
            UpdateBounds();
        }

        public virtual bool HitTest(IRenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, IRenderable originalSource, bool fixedSize)
        {
            var h = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;

            if (context == null || Width == 0 || Height == 0 || Positions == null || Positions.Count == 0)
            {
                return false;
            }

            BoundingBox b = new BoundingBox();
            var left = -Width / 2;
            var right = -left;
            var top = -Height / 2;
            var bottom = -top;
            var projectionMatrix = context.ProjectionMatrix;
            var viewMatrix = context.ViewMatrix;
            var visualToScreen = context.ScreenViewProjectionMatrix;
            foreach(var center in BillboardVertices.Select(x=>x.Position))
            {
                if (fixedSize)
                {               
                    var screenPoint = Vector4.Transform(center, visualToScreen);
                    var spw = screenPoint.W;
                    var spx = screenPoint.X;
                    var spy = screenPoint.Y;
                    var spz = screenPoint.Z / spw / projectionMatrix.M33;

                    var matrix = MatrixExtensions.InverseViewMatrix(ref viewMatrix);
                    var width = (float)context.ActualWidth;
                    var height = (float)context.ActualHeight;
                    Vector3 v = new Vector3();

                    var x = spx + left * spw;
                    var y = spy + bottom * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 bl;
                    Vector3.TransformCoordinate(ref v, ref matrix, out bl);


                    x = spx + right * spw;
                    y = spy + bottom * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 br;
                    Vector3.TransformCoordinate(ref v, ref matrix, out br);

                    x = spx + right * spw;
                    y = spy + top * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 tr;
                    Vector3.TransformCoordinate(ref v, ref matrix, out tr);

                    x = spx + left * spw;
                    y = spy + top * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 tl;
                    Vector3.TransformCoordinate(ref v, ref matrix, out tl);

                    b = BoundingBox.FromPoints(new Vector3[] { tl, tr, bl, br });
                }
                else
                {
                    var vcenter = Vector4.Transform(center, viewMatrix);
                    var vcX = vcenter.X;
                    var vcY = vcenter.Y;

                    var bl = new Vector4(vcX + left, vcY + bottom, vcenter.Z, vcenter.W);
                    var br = new Vector4(vcX + right, vcY + bottom, vcenter.Z, vcenter.W);
                    var tr = new Vector4(vcX + right, vcY + top, vcenter.Z, vcenter.W);
                    var tl = new Vector4(vcX + left, vcY + top, vcenter.Z, vcenter.W);
                    var invViewMatrix = MatrixExtensions.InverseViewMatrix(ref viewMatrix);

                    bl = Vector4.Transform(bl, invViewMatrix);
                    bl /= bl.W;
                    br = Vector4.Transform(br, invViewMatrix);
                    br /= br.W;
                    tr = Vector4.Transform(tr, invViewMatrix);
                    tr /= tr.W;
                    tl = Vector4.Transform(tl, invViewMatrix);
                    tl /= tl.W;
                    b = BoundingBox.FromPoints(new Vector3[] { tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3() });
                }
                // this all happens now in world space now:
                //Debug.WriteLine(string.Format("RayPosition:{0}; Direction:{1};", rayWS.Position, rayWS.Direction));
                if (rayWS.Intersects(ref b))
                {

                    float distance;
                    if (Collision.RayIntersectsBox(ref rayWS, ref b, out distance))
                    {
                        h = true;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.PointHit = rayWS.Position + (rayWS.Direction * distance);
                        result.Distance = distance;
                        Debug.WriteLine(string.Format("Hit; HitPoint:{0}; Bound={1}; Distance={2}", result.PointHit, b, distance));
                        break;
                    }
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }
    }
}

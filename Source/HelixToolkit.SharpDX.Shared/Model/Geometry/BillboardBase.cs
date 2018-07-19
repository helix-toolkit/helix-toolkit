/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE

#else
#endif


#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;
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

        public Stream Texture
        {
            protected set;
            get;
        }

        public float Width
        {
            protected set;
            get;
        }

        public bool IsInitialized
        {
            protected set
            {
                isInitialized = value;
                if (!isInitialized)
                {
                    //Notify to rebuild the texture or billboard vertices
                    RaisePropertyChanged(VertexBuffer);
                }
            }
            get
            {
                return isInitialized;
            }
        }

        private bool isInitialized = false;

        public IList<BillboardVertex> BillboardVertices { get; } = new List<BillboardVertex>();

        public BillboardBase()
        {
        }

        /// <summary>
        /// Draws the texture and fill the billboardverties. Called during initialize vertex buffer.
        /// </summary>
        /// <param name="deviceResources">The device resources.</param>
        public void DrawTexture(IDeviceResources deviceResources)
        {
            if (!isInitialized)
            {
                Debug.WriteLine($"Billboard DrawTexture");
                BillboardVertices.Clear();
                OnDrawTexture(deviceResources);
                RaisePropertyChanged(VertexBuffer);
                UpdateBounds();
            }
            isInitialized = true;           
        }

        protected virtual void OnDrawTexture(IDeviceResources deviceResources)
        {

        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardBase billboard)
            {
                billboard.Texture = Texture;
                IsInitialized = false;
            }
        }
        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="fixedSize">if set to <c>true</c> [fixed size].</param>
        /// <returns></returns>
        public virtual bool HitTest(RenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, 
            object originalSource, bool fixedSize)
        {
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };

            if (context == null || Width == 0 || Height == 0 || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
            {
                return false;
            }
            var scale = modelMatrix.ScaleVector();
            var left = -(Width * scale.X) / 2;
            var right = -left;
            var top = -(Height * scale.Y) / 2;
            var bottom = -top;
            var projectionMatrix = context.ProjectionMatrix;
            var viewMatrix = context.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var visualToScreen = context.ScreenViewProjectionMatrix;
            var rayDir = Vector3.Normalize(rayWS.Direction);
            foreach(var center in BillboardVertices.Select(x=>x.Position))
            {
                var c = Vector3Helper.TransformCoordinate(center.ToVector3(), modelMatrix);
                var dir = Vector3.Normalize(c - rayWS.Position);
                if(Vector3.Dot(dir, rayDir) < 0)
                {
                    continue;
                }

                var b = GetHitTestBound(c, 
                    left, right, top, bottom, ref projectionMatrix, ref viewMatrix, ref viewMatrixInv, ref visualToScreen,
                    fixedSize, (float)context.ActualWidth, (float)context.ActualHeight);
                if (rayWS.Intersects(ref b))
                {
                    if (Collision.RayIntersectsBox(ref rayWS, ref b, out float distance))
                    {
                        h = true;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.PointHit = rayWS.Position + (rayWS.Direction * distance);
                        result.Distance = distance;
                        result.Geometry = this;
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

        protected virtual BoundingBox GetHitTestBound(Vector3 center, float left, float right, float top, float bottom, 
            ref Matrix projectionMatrix, ref Matrix viewMatrix, ref Matrix viewMatrixInv, ref Matrix visualToScreen,
            bool fixedSize, float viewportWidth, float viewportHeight)
        {
            if (fixedSize)
            {
                var screenPoint = Vector4.Transform(center, visualToScreen);
                var spw = screenPoint.W;
                var spx = screenPoint.X;
                var spy = screenPoint.Y;
                var spz = screenPoint.Z / spw / projectionMatrix.M33;

                Vector3 v = new Vector3();

                var x = spx + left * spw;
                var y = spy + bottom * spw;
                v.X = (2 * x / viewportWidth / spw - 1) / projectionMatrix.M11;
                v.Y = -(2 * y / viewportHeight / spw - 1) / projectionMatrix.M22;
                v.Z = spz;

                Vector3 bl;
                Vector3Helper.TransformCoordinate(ref v, ref viewMatrixInv, out bl);


                x = spx + right * spw;
                y = spy + bottom * spw;
                v.X = (2 * x / viewportWidth / spw - 1) / projectionMatrix.M11;
                v.Y = -(2 * y / viewportHeight / spw - 1) / projectionMatrix.M22;
                v.Z = spz;

                Vector3 br;
                Vector3Helper.TransformCoordinate(ref v, ref viewMatrixInv, out br);

                x = spx + right * spw;
                y = spy + top * spw;
                v.X = (2 * x / viewportWidth / spw - 1) / projectionMatrix.M11;
                v.Y = -(2 * y / viewportHeight / spw - 1) / projectionMatrix.M22;
                v.Z = spz;

                Vector3 tr;
                Vector3Helper.TransformCoordinate(ref v, ref viewMatrixInv, out tr);

                x = spx + left * spw;
                y = spy + top * spw;
                v.X = (2 * x / viewportWidth / spw - 1) / projectionMatrix.M11;
                v.Y = -(2 * y / viewportHeight / spw - 1) / projectionMatrix.M22;
                v.Z = spz;

                Vector3 tl;
                Vector3Helper.TransformCoordinate(ref v, ref viewMatrixInv, out tl);
                return BoundingBox.FromPoints(new Vector3[] { tl, tr, bl, br });
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

                bl = Vector4.Transform(bl, viewMatrixInv);
                bl /= bl.W;
                br = Vector4.Transform(br, viewMatrixInv);
                br /= br.W;
                tr = Vector4.Transform(tr, viewMatrixInv);
                tr /= tr.W;
                tl = Vector4.Transform(tl, viewMatrixInv);
                tl /= tl.W;
                return BoundingBox.FromPoints(new Vector3[] { tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3() });
            }
        }
    }
}

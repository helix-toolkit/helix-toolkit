/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
#if NETFX_CORE

#else
using System.Windows.Media.Imaging;
#endif


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
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

        public TextureModel Texture
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

        public IList<BillboardVertex> BillboardVertices { get; } = new FastList<BillboardVertex>();

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
                OnUpdateTextureAndBillboardVertices(deviceResources);
                UpdateBounds();
            }
            isInitialized = true;
        }

        protected abstract void OnUpdateTextureAndBillboardVertices(IDeviceResources deviceResources);

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if (target is BillboardBase billboard)
            {
                billboard.Texture = Texture;
                billboard.IsInitialized = false;
            }
        }

        public void Invalidate()
        {
            IsInitialized = false;
        }

        #region HitTest
        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="fixedSize">if set to <c>true</c> [fixed size].</param>
        /// <returns></returns>
        public abstract bool HitTest(HitTestContext context, Matrix modelMatrix, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize);


        /// <summary>
        /// Hits the size of the test fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected bool HitTestFixedSize(HitTestContext context, ref Matrix modelMatrix, ref List<HitTestResult> hits,
            object originalSource, int count)
        {
            if (BillboardVertices == null || BillboardVertices.Count == 0)
            {
                return false;
            }
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };
            var visualToScreen = context.RenderMatrices.ScreenViewProjectionMatrix;
            var screenPoint = context.HitPointSP * context.RenderMatrices.DpiScale;
            if (screenPoint.X < 0 || screenPoint.Y < 0)
            {
                return false;
            }

            for (var i = 0; i < count; ++i)
            {
                var vert = BillboardVertices[i];
                var pos = vert.Position.ToVector3();
                var c = Vector3.TransformCoordinate(pos, modelMatrix);
                var dir = c - context.RayWS.Position;
                if (Vector3.Dot(dir, context.RayWS.Direction) < 0)
                {
                    continue;
                }
                var quad = GetScreenQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref visualToScreen, context.RenderMatrices.DpiScale);
                if (quad.IsPointInQuad2D(ref screenPoint))
                {
                    var v = c - context.RayWS.Position;
                    var dist = Vector3.Dot(context.RayWS.Direction, v);
                    if (dist > result.Distance)
                    {
                        continue;
                    }
                    h = true;

                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = context.RayWS.Position + context.RayWS.Direction * dist;
                    result.Distance = dist;
                    result.Geometry = this;
                    AssignResultAdditional(result, i);
                    Debug.WriteLine(string.Format("Hit; HitPoint:{0}; Text={1}", result.PointHit, result.TextInfo == null ? Type.ToString() : result.TextInfo.Text));
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }

        protected virtual void AssignResultAdditional(BillboardHitResult result, int index)
        {
            result.TextInfoIndex = index;
            result.Type = Type;
        }

        protected override void OnClearAllGeometryData()
        {
            base.OnClearAllGeometryData();
            BillboardVertices?.Clear();
            (BillboardVertices as FastList<BillboardVertex>)?.TrimExcess();
        }

        /// <summary>
        /// Hits the size of the test non fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected bool HitTestNonFixedSize(HitTestContext context, ref Matrix modelMatrix, ref List<HitTestResult> hits,
            object originalSource, int count)
        {
            if (BillboardVertices == null || BillboardVertices.Count == 0)
            {
                return false;
            }
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };
            var viewMatrix = context.RenderMatrices.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var rayWS = context.RayWS;
            for (var i = 0; i < count; ++i)
            {
                var vert = BillboardVertices[i];
                var pos = vert.Position.ToVector3();
                var c = Vector3.TransformCoordinate(pos, modelMatrix);
                var dir = c - rayWS.Position;
                if (Vector3.Dot(dir, rayWS.Direction) < 0)
                {
                    continue;
                }
                var quad = GetHitTestQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref viewMatrix, ref viewMatrixInv);
                if (Collision.RayIntersectsTriangle(ref rayWS, ref quad.TL, ref quad.TR, ref quad.BR, out Vector3 hitPoint)
                    || Collision.RayIntersectsTriangle(ref rayWS, ref quad.TL, ref quad.BR, ref quad.BL, out hitPoint))
                {
                    var dist = (rayWS.Position - hitPoint).Length();
                    if (dist > result.Distance)
                    {
                        continue;
                    }
                    h = true;
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = hitPoint;
                    result.Distance = dist;
                    result.Geometry = this;
                    AssignResultAdditional(result, i);
                    Debug.WriteLine(string.Format("Hit; HitPoint:{0}; Text={1}", result.PointHit, result.TextInfo == null ? Type.ToString() : result.TextInfo.Text));
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }

        protected static void GetQuadOffset(float width, float height,
            BillboardHorizontalAlignment horizontalAlignment, BillboardVerticalAlignment verticalAlignment,
            out Vector2 topLeft, out Vector2 bottomRight)
        {
            float top = 0;
            float bottom = 0;
            float left = 0;
            float right = 0;
            switch (horizontalAlignment)
            {
                case BillboardHorizontalAlignment.Center:
                    left = -width / 2;
                    right = width / 2;
                    break;
                case BillboardHorizontalAlignment.Left:
                    left = -width;
                    right = 0;
                    break;
                case BillboardHorizontalAlignment.Right:
                    left = 0;
                    right = width;
                    break;
            }
            switch (verticalAlignment)
            {
                case BillboardVerticalAlignment.Center:
                    top = height / 2;
                    bottom = -height / 2;
                    break;
                case BillboardVerticalAlignment.Top:
                    top = height;
                    bottom = 0;
                    break;
                case BillboardVerticalAlignment.Bottom:
                    top = 0;
                    bottom = -height;
                    break;
            }
            topLeft = new Vector2(left, top);
            bottomRight = new Vector2(right, bottom);
        }

        private struct Quad
        {
            public Vector3 TL;
            public Vector3 TR;
            public Vector3 BL;
            public Vector3 BR;

            public Quad(ref Vector3 tl, ref Vector3 tr, ref Vector3 bl, ref Vector3 br)
            {
                TL = tl;
                TR = tr;
                BL = bl;
                BR = br;
            }

            public Quad(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
            {
                TL = tl;
                TR = tr;
                BL = bl;
                BR = br;
            }
        }

        private struct Quad2D
        {
            public Vector2 TL;
            public Vector2 TR;
            public Vector2 BL;
            public Vector2 BR;

            public Quad2D(ref Vector2 tl, ref Vector2 tr, ref Vector2 bl, ref Vector2 br)
            {
                TL = tl;
                TR = tr;
                BL = bl;
                BR = br;
            }

            public Quad2D(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br)
            {
                TL = tl;
                TR = tr;
                BL = bl;
                BR = br;
            }


            public bool IsPointInQuad2D(Vector2 point)
            {
                return IsPointInQuad2D(ref point);
            }


            public bool IsPointInQuad2D(ref Vector2 point)
            {
                //var v1 = point - TL;
                //var t1 = BL - TL;
                //if(Vector2.Dot(v1, t1) < 0)
                //{
                //    return false;
                //}
                //var v2 = point - BL;
                //var t2 = BR - BL;
                //if (Vector2.Dot(v2, t2) < 0)
                //{
                //    return false;
                //}

                //var v3 = point - BR;
                //var t3 = TR - BR;
                //if (Vector2.Dot(v3, t3) < 0)
                //{
                //    return false;
                //}

                //var v4 = point - TR;
                //var t4 = TL - TR;
                //if (Vector2.Dot(v4, t4) < 0)
                //{
                //    return false;
                //}
                //return true;
                return Vector2.Dot(point - TL, BL - TL) >= 0 && Vector2.Dot(point - BL, BR - BL) >= 0
                    && Vector2.Dot(point - BR, TR - BR) >= 0 && Vector2.Dot(point - TR, TL - TR) >= 0;
            }
        }

        private static Quad GetHitTestQuad(ref Vector3 center, ref Vector2 TL, ref Vector2 TR, ref Vector2 BL, ref Vector2 BR,
            ref Matrix viewMatrix, ref Matrix viewMatrixInv)
        {
            var vcenter = Vector3.TransformCoordinate(center, viewMatrix);
            var vcX = vcenter.X;
            var vcY = vcenter.Y;

            var bl = new Vector3(vcX + BL.X, vcY + BL.Y, vcenter.Z);
            var br = new Vector3(vcX + BR.X, vcY + BR.Y, vcenter.Z);
            var tr = new Vector3(vcX + TR.X, vcY + TR.Y, vcenter.Z);
            var tl = new Vector3(vcX + TL.X, vcY + TL.Y, vcenter.Z);

            bl = Vector3.TransformCoordinate(bl, viewMatrixInv);
            br = Vector3.TransformCoordinate(br, viewMatrixInv);
            tr = Vector3.TransformCoordinate(tr, viewMatrixInv);
            tl = Vector3.TransformCoordinate(tl, viewMatrixInv);
            return new Quad(ref tl, ref tr, ref bl, ref br);
        }

        private static Quad2D GetScreenQuad(ref Vector3 center, ref Vector2 TL, ref Vector2 TR, ref Vector2 BL, ref Vector2 BR,
            ref Matrix screenViewProjection, float scale)
        {
            var vcenter = Vector3.TransformCoordinate(center, screenViewProjection);
            var p = new Vector2(vcenter.X, vcenter.Y);
            var tl = p + new Vector2(TL.X, -TL.Y) * scale;
            var tr = p + new Vector2(TR.X, -TR.Y) * scale;
            var bl = p + new Vector2(BL.X, -BL.Y) * scale;
            var br = p + new Vector2(BR.X, -BR.Y) * scale;
            return new Quad2D(ref tl, ref tr, ref bl, ref br);
        }
        #endregion
    }
}

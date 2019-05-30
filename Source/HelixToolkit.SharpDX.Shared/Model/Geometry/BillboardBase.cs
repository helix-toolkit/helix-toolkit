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
            if(target is BillboardBase billboard)
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
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="fixedSize">if set to <c>true</c> [fixed size].</param>
        /// <returns></returns>
        public abstract bool HitTest(RenderContext context, Matrix modelMatrix,
            ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize);


        /// <summary>
        /// Hits the size of the test fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected bool HitTestFixedSize(RenderContext context, ref Matrix modelMatrix,
            ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, int count)
        {
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };
            var visualToScreen = context.ScreenViewProjectionMatrix;
            var screenPoint3D = Vector3.TransformCoordinate(rayWS.Position, visualToScreen);
            var screenPoint = new Vector2(screenPoint3D.X, screenPoint3D.Y);
            var scale3D = modelMatrix.ScaleVector;
            var scale = new Vector2(scale3D.X, scale3D.Y);
            for (int i = 0; i < count; ++i)
            {
                var vert = BillboardVertices[i];
                var pos = vert.Position.ToVector3();
                var c = Vector3.TransformCoordinate(pos, modelMatrix);
                var dir = c - rayWS.Position;
                if (Vector3.Dot(dir, rayWS.Direction) < 0)
                {
                    continue;
                }
                var quad = GetScreenQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref visualToScreen, ref scale);
                if (quad.IsPointInQuad2D(ref screenPoint))
                {
                    var v = c - rayWS.Position;
                    var dist = Vector3.Dot(rayWS.Direction, v);
                    if (dist > result.Distance)
                    {
                        continue;
                    }
                    h = true;

                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = rayWS.Position + rayWS.Direction * dist;
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
        /// <summary>
        /// Hits the size of the test non fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected bool HitTestNonFixedSize(RenderContext context, ref Matrix modelMatrix,
            ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, int count)
        {
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };
            var viewMatrix = context.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var scale3D = modelMatrix.ScaleVector;
            var scale = new Vector2(scale3D.X, scale3D.Y);
            for (int i = 0; i < count; ++i)
            {
                var vert = BillboardVertices[i];
                var pos = vert.Position.ToVector3();
                var c = Vector3.TransformCoordinate(pos, modelMatrix);
                var dir = c - rayWS.Position;
                if (Vector3.Dot(dir, rayWS.Direction) < 0)
                {
                    continue;
                }
                var quad = GetHitTestQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref viewMatrix, ref viewMatrixInv, ref scale);
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
            ref Matrix viewMatrix, ref Matrix viewMatrixInv, ref Vector2 scale)
        {
            var vcenter = Vector3.TransformCoordinate(center, viewMatrix);
            var vcX = vcenter.X;
            var vcY = vcenter.Y;

            var bl = new Vector3(vcX + BL.X * scale.X, vcY + BL.Y * scale.X, vcenter.Z);
            var br = new Vector3(vcX + BR.X * scale.X, vcY + BR.Y * scale.Y, vcenter.Z);
            var tr = new Vector3(vcX + TR.X * scale.X, vcY + TR.Y * scale.Y, vcenter.Z);
            var tl = new Vector3(vcX + TL.X * scale.X, vcY + TL.Y * scale.Y, vcenter.Z);

            bl = Vector3.TransformCoordinate(bl, viewMatrixInv);
            br = Vector3.TransformCoordinate(br, viewMatrixInv);
            tr = Vector3.TransformCoordinate(tr, viewMatrixInv);
            tl = Vector3.TransformCoordinate(tl, viewMatrixInv);
            return new Quad(ref tl, ref tr, ref bl, ref br);
        }

        private static Quad2D GetScreenQuad(ref Vector3 center, ref Vector2 TL, ref Vector2 TR, ref Vector2 BL, ref Vector2 BR,
            ref Matrix screenViewProjection, ref Vector2 scale)
        {
            var vcenter = Vector3.TransformCoordinate(center, screenViewProjection);
            Vector2 p = new Vector2(vcenter.X, vcenter.Y);
            var tl = p + TL * scale;
            var tr = p + TR * scale;
            var bl = p + BL * scale;
            var br = p + BR * scale;
            return new Quad2D(ref tl, ref tr, ref bl, ref br);
        }
        #endregion
    }
}

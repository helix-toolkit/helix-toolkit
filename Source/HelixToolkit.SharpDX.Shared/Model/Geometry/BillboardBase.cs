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
        public abstract bool HitTest(RenderContext context, Matrix modelMatrix,
            ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize);


        protected struct Quad
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

        protected struct Quad2D
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

        protected Quad GetHitTestQuad(ref Vector3 center, ref Vector2 TL, ref Vector2 TR, ref Vector2 BL, ref Vector2 BR,
            ref Matrix viewMatrix, ref Matrix viewMatrixInv)
        {
            var vcenter = Vector3.Transform(center, viewMatrix);
            var vcX = vcenter.X;
            var vcY = vcenter.Y;

            var bl = new Vector4(vcX + BL.X, vcY + BL.Y, vcenter.Z, vcenter.W);
            var br = new Vector4(vcX + BR.X, vcY + BR.Y, vcenter.Z, vcenter.W);
            var tr = new Vector4(vcX + TR.X, vcY + TR.Y, vcenter.Z, vcenter.W);
            var tl = new Vector4(vcX + TL.X, vcY + TL.Y, vcenter.Z, vcenter.W);

            bl = Vector4.Transform(bl, viewMatrixInv);
            bl /= bl.W;
            br = Vector4.Transform(br, viewMatrixInv);
            br /= br.W;
            tr = Vector4.Transform(tr, viewMatrixInv);
            tr /= tr.W;
            tl = Vector4.Transform(tl, viewMatrixInv);
            tl /= tl.W;
            return new Quad(tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3());
        }

        protected Quad2D GetScreenQuad(ref Vector3 center, ref Vector2 TL, ref Vector2 TR, ref Vector2 BL, ref Vector2 BR,
            ref Matrix screenViewProjection)
        {
            var vcenter = Vector3.TransformCoordinate(center, screenViewProjection);
            Vector2 p = new Vector2(vcenter.X, vcenter.Y);
            var tl = p + TL;
            var tr = p + TR;
            var bl = p + BL;
            var br = p + BR;
            return new Quad2D(ref tl, ref tr, ref bl, ref br);
        }
    }
}

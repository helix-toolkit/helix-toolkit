/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.IO;
using SharpDX.Toolkit.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// 
    /// </summary>
    public class BillboardSingleImage3D : BillboardBase
    {
        /// <summary>
        /// Billboard type, <see cref="BillboardType"/>
        /// </summary>
        public override BillboardType Type
        {
            get
            {
                return BillboardType.SingleImage;
            }
        }

        private Vector3 center = Vector3.Zero;
        /// <summary>
        /// Billboard center location
        /// </summary>
        public Vector3 Center
        {
            set
            {
                if(Set(ref center, value))
                {
                    IsInitialized = false;
                }
            }
            get { return center; }
        }

        private Color4 maskColor = Color.Transparent;
        /// <summary>
        /// If color in image is equal to the mask color, the color will set to transparent in image.
        /// Default color is Transparent, which did not mask any color.
        /// </summary>
        public Color4 MaskColor
        {
            set
            {
                if(Set(ref maskColor, value))
                {
                    IsInitialized = false;
                }
            }
            get { return maskColor; }
        }

        private float angle = 0;
        /// <summary>
        /// Gets or sets the rotation angle in radians.
        /// </summary>
        /// <value>
        /// The angle in radians.
        /// </value>
        public float Angle
        {
            set
            {
                if(Set(ref angle, value))
                {
                    IsInitialized = false;
                }
            }
            get { return angle; }
        }
#if !NETFX_CORE        
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="bitmapSource">The bitmap source.</param>
        public BillboardSingleImage3D(BitmapSource bitmapSource)
        {
            this.Texture = bitmapSource.ToMemoryStream();
            Width = bitmapSource.PixelWidth;
            Height = bitmapSource.PixelHeight;
        }
#endif        
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        public BillboardSingleImage3D(Stream imageStream)
        {
            this.Texture = imageStream;
            using (Image image = Image.Load(imageStream))
            {
                Width = image.Description.Width;
                Height = image.Description.Height;
            }
            Texture.Position = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BillboardSingleImage3D(Stream imageStream, float width, float height)
        {
            this.Texture = imageStream;
            Width = width;
            Height = height;
            Texture.Position = 0;
        }

        /// <summary>
        /// Updates the bounds.
        /// </summary>
        public override void UpdateBounds()
        {
            BoundingSphere = new BoundingSphere(Center, (float)Math.Sqrt(Width * Width + Height * Height) / 2);
            Bound = BoundingBox.FromSphere(BoundingSphere);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardSingleImage3D billboard)
            {
                billboard.Center = Center;
                billboard.MaskColor = MaskColor;
            }
        }
        /// <summary>
        /// Called when [draw texture].
        /// </summary>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnDrawTexture(IDeviceResources deviceResources)
        {
            var w = Width;
            var h = Height;
            // CCW from bottom left 
            var tl = new Vector2(-w / 2, h / 2);
            var br = new Vector2(w / 2, -h / 2);

            var uv_tl = new Vector2(0, 0);
            var uv_br = new Vector2(1, 1);
            var transform = Angle != 0 ? Matrix3x2.Rotation(Angle) : Matrix3x2.Identity;
            var tr = new Vector2(br.X, tl.Y);
            var bl = new Vector2(tl.X, br.Y);
            BillboardVertices.Add(new BillboardVertex()
            {
                Position = Center.ToVector4(),
                Foreground = Color.White,
                Background = MaskColor,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = Matrix3x2.TransformPoint(transform, tl),
                OffBR = Matrix3x2.TransformPoint(transform, br),
                OffBL = Matrix3x2.TransformPoint(transform, bl),
                OffTR = Matrix3x2.TransformPoint(transform, tr)
            });
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
        public override bool HitTest(RenderContext context, Matrix modelMatrix,
            ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize)
        {
            if (!IsInitialized || context == null || Width == 0 || Height == 0 || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
            {
                return false;
            }

            return fixedSize ? HitTestFixedSize(context, ref modelMatrix, ref rayWS, ref hits, originalSource, BillboardVertices.Count)
                : HitTestNonFixedSize(context, ref modelMatrix, ref rayWS, ref hits, originalSource, BillboardVertices.Count);
        }

        /// <summary>
        /// Hits the size of the test fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardVertices"/>.</param>
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
                var quad = GetScreenQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref visualToScreen);
                if (quad.IsPointInQuad2D(ref screenPoint))
                {
                    h = true;
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = c;
                    result.Distance = (rayWS.Position - c).Length();
                    result.Geometry = this;
                    Debug.WriteLine(string.Format("Hit; HitPoint:{0}; Text={1}", result.PointHit, result.TextInfo.Text));
                    break;
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }
        /// <summary>
        /// Hits the size of the test non fixed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelMatrix">The model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="originalSource">The original source.</param>
        /// <param name="count">The count of vertices in <see cref="BillboardVertices"/>.</param>
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
                var quad = GetHitTestQuad(ref c, ref vert.OffTL, ref vert.OffTR, ref vert.OffBL, ref vert.OffBR, ref viewMatrix, ref viewMatrixInv);
                if (Collision.RayIntersectsTriangle(ref rayWS, ref quad.TL, ref quad.TR, ref quad.BR, out Vector3 hitPoint)
                    || Collision.RayIntersectsTriangle(ref rayWS, ref quad.TL, ref quad.BR, ref quad.BL, out hitPoint))
                {
                    h = true;
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = hitPoint;
                    result.Distance = (rayWS.Position - hitPoint).Length();
                    result.Geometry = this;
                    Debug.WriteLine(string.Format("Hit; HitPoint:{0};", result.PointHit));
                    break;
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

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
#if CORE
using SharpDX.DirectWrite;
using FontWeight = SharpDX.DirectWrite.FontWeight;
using FontWeights = SharpDX.DirectWrite.FontWeight;
using Thickness = HelixToolkit.UWP.Model.Scene2D.Thickness;
#else
#if NETFX_CORE
    using Windows.UI.Xaml;
    using Media = Windows.UI.Xaml.Media;
    using Windows.UI.Text;
#else
    using System.Windows;
    using Media = System.Windows.Media;
#endif
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
using Core;
    using Extensions;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class BillboardSingleText3D : BillboardBase
    {
        private readonly bool predefinedSize = false;
        /// <summary>
        /// Billboard type, <see cref="BillboardType"/>
        /// </summary>
        public override BillboardType Type
        {
            get
            {
                return BillboardType.SingleText;
            }
        }

        private TextInfo mTextInfo = new TextInfo("", new Vector3());
        /// <summary>
        /// Gets or sets the text information.
        /// </summary>
        /// <value>
        /// The text information.
        /// </value>
        public TextInfo TextInfo
        {
            get { return mTextInfo; }
            set
            {
                if(Set(ref mTextInfo, value))
                {
                    IsInitialized = false;
                }
            }
        }

        private Color4 mFontColor = Color.Black;
        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        /// <value>
        /// The color of the font.
        /// </value>
        public Color4 FontColor
        {
            set
            {
                if(Set(ref mFontColor, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mFontColor; }
        }

        private Color4 mBackgroundColor = Color.Transparent;
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public Color4 BackgroundColor
        {
            set
            {
                if(Set(ref mBackgroundColor, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mBackgroundColor; }
        }

        private int mFontSize = 12;
        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize
        {
            set
            {
                if(Set(ref mFontSize, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mFontSize; }
        }

        private string mFontFamily = "Arial";
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily
        {
            set
            {
                if(Set(ref mFontFamily, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mFontFamily;
            }
        }

        private FontWeight mFontWeight = FontWeights.Normal;
        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight FontWeight
        {
            set
            {
                if(Set(ref mFontWeight, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mFontWeight;
            }
        }
#if NETFX_CORE
        private FontStyle mFontStyle = FontStyle.Normal;
#else
        private FontStyle mFontStyle = FontStyles.Normal;
#endif        
        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle FontStyle
        {
            set
            {
                if(Set(ref mFontStyle, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mFontStyle;
            }
        }

        private Thickness mPadding = new Thickness(0);
        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        public Thickness Padding
        {
            set
            {
                if(Set(ref mPadding, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mPadding;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleText3D"/> class.
        /// </summary>
        public BillboardSingleText3D()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleText3D"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BillboardSingleText3D(float width, float height)
        {
            TextInfo = new TextInfo();
            Width = width;
            Height = height;
            predefinedSize = true;
        }
        /// <summary>
        /// Updates the bounds.
        /// </summary>
        public override void UpdateBounds()
        {
            BoundingSphere = new BoundingSphere(TextInfo.Origin, (float)Math.Sqrt(Width * Width + Height * Height) / 2);
            Bound = BoundingBox.FromSphere(BoundingSphere);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardSingleText3D billboard)
            {
                billboard.BackgroundColor = BackgroundColor;
                billboard.FontColor = FontColor;
                billboard.FontFamily = FontFamily;
                billboard.FontSize = FontSize;
                billboard.FontStyle = FontStyle;
                billboard.FontWeight = FontWeight;
                billboard.TextInfo = TextInfo;
                billboard.Padding = Padding;
            }
        }
        /// <summary>
        /// Called when [draw texture].
        /// </summary>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnDrawTexture(IDeviceResources deviceResources)
        {
            if (!string.IsNullOrEmpty(TextInfo.Text))
            {
                var w = Width;
                var h = Height;
#if CORE
                Texture = TextInfo.Text.ToBitmapStream(FontSize, Color.White, Color.Black, FontFamily, FontWeight, FontStyle,
                    new Vector4((float)Padding.Left, (float)Padding.Top, (float)Padding.Right, (float)Padding.Bottom), ref w, ref h, predefinedSize, deviceResources);
#else
                Texture = TextInfo.Text.ToBitmapStream(FontSize, Color.White, Color.Black, FontFamily, FontWeight.ToDXFontWeight(), FontStyle.ToDXFontStyle(),
                    new Vector4((float)Padding.Left, (float)Padding.Top, (float)Padding.Right, (float)Padding.Bottom), ref w, ref h, predefinedSize, deviceResources);
#endif
                if (!predefinedSize)
                {
                    Width = w;
                    Height = h;
                }
                DrawCharacter(TextInfo.Text, TextInfo.Origin, Width, Height, TextInfo);
            }
            else
            {
                Texture = null;
                if (!predefinedSize)
                {
                    Width = 0;
                    Height = 0;
                }
            }
            TextInfo.UpdateTextInfo(Width, Height);
        }

        private void DrawCharacter(string text, Vector3 origin, float w, float h, TextInfo info)
        {
            // CCW from bottom left 
            var tl = new Vector2(-w / 2, h / 2);
            var br = new Vector2(w / 2, -h / 2);

            var uv_tl = new Vector2(0, 0);
            var uv_br = new Vector2(1, 1);
            var transform = info.Angle != 0 ? Matrix3x2.Rotation(info.Angle) : Matrix3x2.Identity;
            var offTL = tl * info.Scale;
            var offBR = br * info.Scale;
            var offTR = new Vector2(offBR.X, offTL.Y);
            var offBL = new Vector2(offTL.X, offBR.Y);
            BillboardVertices.Add(new BillboardVertex()
            {
                Position = info.Origin.ToVector4(),
                Foreground = FontColor,
                Background = BackgroundColor,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = Matrix3x2.TransformPoint(transform, offTL),
                OffBL = Matrix3x2.TransformPoint(transform, offBL),
                OffBR = Matrix3x2.TransformPoint(transform, offBR),
                OffTR = Matrix3x2.TransformPoint(transform, offTR)
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
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected virtual bool HitTestFixedSize(RenderContext context, ref Matrix modelMatrix,
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
                    h = true;
                    var v = c - rayWS.Position;
                    var dist = Vector3.Dot(rayWS.Direction, v);
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = c;
                    result.Distance = dist;
                    result.PointHit = rayWS.Position + rayWS.Direction * dist;
                    result.Geometry = this;
                    result.TextInfo = TextInfo;
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
        /// <param name="count">The count of vertices in <see cref="BillboardBase.BillboardVertices"/>.</param>
        /// <returns></returns>
        protected virtual bool HitTestNonFixedSize(RenderContext context, ref Matrix modelMatrix,
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
                    h = true;
                    result.ModelHit = originalSource;
                    result.IsValid = true;
                    result.PointHit = hitPoint;
                    result.Distance = (rayWS.Position - hitPoint).Length();
                    result.Geometry = this;
                    result.TextInfo = TextInfo;
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
    }
}

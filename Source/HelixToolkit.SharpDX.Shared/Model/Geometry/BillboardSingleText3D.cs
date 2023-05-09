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
using Thickness = HelixToolkit.SharpDX.Core.Model.Scene2D.Thickness;
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
    using Core;
#if !CORE
    using Extensions;
#endif
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

        private TextInfo mTextInfo = new TextInfo(string.Empty, new Vector3());
        /// <summary>
        /// Gets or sets the text information.
        /// </summary>
        /// <value>
        /// The text information.
        /// </value>
        public TextInfo TextInfo
        {
            get
            {
                return mTextInfo;
            }
            set
            {
                if (Set(ref mTextInfo, value))
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
                if (Set(ref mFontColor, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mFontColor;
            }
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
                if (Set(ref mBackgroundColor, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mBackgroundColor;
            }
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
                if (Set(ref mFontSize, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mFontSize;
            }
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
                if (Set(ref mFontFamily, value))
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
                if (Set(ref mFontWeight, value))
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
                if (Set(ref mFontStyle, value))
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
                if (Set(ref mPadding, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return mPadding;
            }
        }

        private BillboardHorizontalAlignment horizontalAlignment = BillboardHorizontalAlignment.Center;
        /// <summary>
        /// Sets or gets the horizontal alignment. Default = <see cref="BillboardHorizontalAlignment.Center"/>
        /// <para>
        /// For example, when sets horizontal and vertical alignment to top/left,
        /// billboard's bottom/right point will be anchored at the billboard origin.
        /// </para>
        /// </summary>
        /// <value>
        /// The horizontal alignment.
        /// </value>
        public BillboardHorizontalAlignment HorizontalAlignment
        {
            set
            {
                if (Set(ref horizontalAlignment, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return horizontalAlignment;
            }
        }

        private BillboardVerticalAlignment verticalAlignment = BillboardVerticalAlignment.Center;
        /// <summary>
        /// Sets or gets the vertical alignment. Default = <see cref="BillboardVerticalAlignment.Center"/>
        /// <para>
        /// For example, when sets horizontal and vertical alignment to top/left,
        /// billboard's bottom/right point will be anchored at the billboard origin.
        /// </para>
        /// </summary>
        /// <value>
        /// The vertical alignment.
        /// </value>
        public BillboardVerticalAlignment VerticalAlignment
        {
            set
            {
                if (Set(ref verticalAlignment, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return verticalAlignment;
            }
        }

        private Vector2 offset = Vector2.Zero;
        /// <summary>
        /// Additional offset for billboard display location.
        /// Behavior depends on whether billboard is fixed sized or not.
        /// When billboard is fixed sized, the offset is screen spaced.
        /// </summary>
        public Vector2 Offset
        {
            set
            {
                if(Set(ref offset, value))
                {
                    IsInitialized = false;
                }
            }
            get { return offset; }
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
            if (TextInfo == null)
            {
                BoundingSphere = new BoundingSphere();
                Bound = new BoundingBox();
            }
            else
            {
                BoundingSphere = new BoundingSphere(TextInfo.Origin, (float)Math.Sqrt(Width * Width + Height * Height) / 2);
                Bound = BoundingBox.FromSphere(BoundingSphere);
            }
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if (target is BillboardSingleText3D billboard)
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
        protected override void OnUpdateTextureAndBillboardVertices(IDeviceResources deviceResources)
        {
            if (TextInfo != null && !string.IsNullOrEmpty(TextInfo.Text))
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
            TextInfo?.UpdateTextInfo(Width, Height);
        }

        private void DrawCharacter(string text, Vector3 origin, float w, float h, TextInfo info)
        {
            GetQuadOffset(w, h, HorizontalAlignment, VerticalAlignment, out var tl, out var br);

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
                OffTL = Matrix3x2.TransformPoint(transform, offTL) + Offset,
                OffBL = Matrix3x2.TransformPoint(transform, offBL) + Offset,
                OffBR = Matrix3x2.TransformPoint(transform, offBR) + Offset,
                OffTR = Matrix3x2.TransformPoint(transform, offTR) + Offset
            });
        }


        public override bool HitTest(HitTestContext context, Matrix modelMatrix, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize)
        {
            var rayWS = context.RayWS;
            if (!IsInitialized || context == null || Width == 0 || Height == 0
                || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
            {
                return false;
            }

            return fixedSize ? HitTestFixedSize(context, ref modelMatrix, ref hits, originalSource, BillboardVertices.Count)
                : HitTestNonFixedSize(context, ref modelMatrix, ref hits, originalSource, BillboardVertices.Count);
        }

        protected override void AssignResultAdditional(BillboardHitResult result, int index)
        {
            base.AssignResultAdditional(result, index);
            result.TextInfo = this.TextInfo;
            result.TextInfoIndex = index;
        }
    }
}

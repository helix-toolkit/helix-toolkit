/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Runtime.CompilerServices;

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
    namespace Model.Scene2D
    {
        /// <summary>
        /// 
        /// </summary>
        public partial class SceneNode2D
        {
            #region layout management

            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this instance is measure dirty.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is measure dirty; otherwise, <c>false</c>.
            /// </value>
            public bool IsMeasureDirty { protected set; get; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether this instance is arrange dirty.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is arrange dirty; otherwise, <c>false</c>.
            /// </value>
            public bool IsArrangeDirty { protected set; get; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether this instance is transform dirty.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is transform dirty; otherwise, <c>false</c>.
            /// </value>
            public bool IsTransformDirty { private set; get; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether this instance is visual dirty.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is visual dirty; otherwise, <c>false</c>.
            /// </value>
            public bool IsVisualDirty { set; get; } = true;

            private Thickness margin = new Thickness();

            public Thickness Margin
            {
                set
                {
                    if (Set(ref margin, value))
                    {
                        MarginWidthHeight = new Vector2((value.Left + value.Right), (value.Top + value.Bottom));
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return margin;
                }
            }

            protected Vector2 MarginWidthHeight { private set; get; }

            private float width = float.PositiveInfinity;

            public float Width
            {
                set
                {
                    if (Set(ref width, value))
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return width;
                }
            }

            private float height = float.PositiveInfinity;

            public float Height
            {
                set
                {
                    if (Set(ref height, value))
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return height;
                }
            }

            private float minimumWidth = 0;

            public float MinimumWidth
            {
                set
                {
                    if (Set(ref minimumWidth, value) && value > width)
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return minimumWidth;
                }
            }

            private float minimumHeight = 0;

            public float MinimumHeight
            {
                set
                {
                    if (Set(ref minimumHeight, value) && value > height)
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return minimumHeight;
                }
            }

            private float maximumWidth = float.PositiveInfinity;

            public float MaximumWidth
            {
                set
                {
                    if (Set(ref maximumWidth, value) && value < width)
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return maximumWidth;
                }
            }

            private float maximumHeight = float.PositiveInfinity;

            public float MaximumHeight
            {
                set
                {
                    if (Set(ref maximumHeight, value) && value < height)
                    {
                        InvalidateMeasure();
                    }
                }
                get
                {
                    return maximumHeight;
                }
            }

            private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch;

            public HorizontalAlignment HorizontalAlignment
            {
                set
                {
                    if (Set(ref horizontalAlignment, value))
                    {
                        InvalidateArrange();
                    }
                }
                get
                {
                    return horizontalAlignment;
                }
            }

            private VerticalAlignment verticalAlignment = VerticalAlignment.Stretch;

            public VerticalAlignment VerticalAlignment
            {
                set
                {
                    if (Set(ref verticalAlignment, value))
                    {
                        InvalidateArrange();
                    }
                }
                get
                {
                    return verticalAlignment;
                }
            }

            private Vector2 layoutOffset = Vector2.Zero;

            public Vector2 LayoutOffsets
            {
                private set
                {
                    if (Set(ref layoutOffset, value))
                    {
                        InvalidateTransform();
                    }
                }
                get { return layoutOffset; }
            }

            private Vector2 renderSize = Vector2.Zero;

            /// <summary>
            /// Gets the render size. Same as the <see cref="LayoutBound"/> size
            /// </summary>
            /// <value>
            /// The size of the render.
            /// </value>
            public Vector2 RenderSize
            {
                get { return renderSize; }
                private set
                {
                    if (Set(ref renderSize, value))
                    {
                        InvalidateTransform();
                    }
                }
            }

            private Vector2 renderTransformOrigin = new Vector2(0.5f, 0.5f);

            public Vector2 RenderTransformOrigin
            {
                set
                {
                    if (Set(ref renderTransformOrigin, value))
                    {
                        InvalidateRender();
                    }
                }
                get { return renderTransformOrigin; }
            }

            /// <summary>
            /// Gets the size of the desired size after measure.
            /// </summary>
            /// <value>
            /// The size of the desired.
            /// </value>
            public Vector2 DesiredSize { get; private set; }

            /// <summary>
            /// Gets the size of the unclipped desired size after measure.
            /// </summary>
            /// <value>
            /// The size of the unclipped desired.
            /// </value>
            public Vector2 UnclippedDesiredSize { get; private set; } = new Vector2(-1, -1);

            private Vector2 Size { get { return new Vector2(width, height); } }

            public bool ClipEnabled { private set; get; } = false;

            public bool ClipToBound { set; get; } = false;

            /// <summary>
            /// Gets or sets the layout clip bound. This bound includes the margin.
            /// </summary>
            /// <value>
            /// The layout clip bound.
            /// </value>
            public RectangleF LayoutClipBound
            {
                private set
                {
                    RenderCore.LayoutClippingBound = value;
                }
                get { return RenderCore.LayoutClippingBound; }
            }

            /// <summary>
            /// Gets the size of the actual layout bound without margin.
            /// </summary>
            public RectangleF LayoutBound
            {
                private set
                {
                    RenderCore.LayoutBound = value;
                }
                get { return RenderCore.LayoutBound; }
            }

            private Size2F? previousMeasureSize;
            private RectangleF? previousArrange;

            #endregion Properties

            public void InvalidateMeasure()
            {
                IsArrangeDirty = true;
                IsMeasureDirty = true;
                TraverseUp(this, (p) =>
                {
                    if (p.IsArrangeDirty && p.IsMeasureDirty)
                    {
                        return false;
                    }
                    p.IsArrangeDirty = true;
                    p.IsMeasureDirty = true;
                    return true;
                });
                if (IsAttached)
                {
                    InvalidateRender();
                }
            }

            public void InvalidateArrange()
            {
                IsArrangeDirty = true;
                TraverseUp(this, (p) =>
                {
                    if (p.IsArrangeDirty)
                    {
                        return false;
                    }
                    p.IsArrangeDirty = true;
                    return true;
                });
                if (IsAttached)
                {
                    InvalidateRender();
                }
            }

            public void InvalidateVisual()
            {
                IsVisualDirty = true;
                TraverseUp(this, (p) =>
                {
                    if (p.IsVisualDirty)
                    {
                        return false;
                    }
                    p.IsVisualDirty = true;
                    return true;
                });
                if (IsAttached)
                {
                    InvalidateRender();
                }
            }

            public void InvalidateTransform()
            {
                IsTransformDirty = true;
                TraverseUp(this, (e) =>
                {
                    if (e.IsTransformDirty)
                    { return false; }
                    e.IsTransformDirty = true;
                    return true;
                });
                if (IsAttached)
                {
                    InvalidateRender();
                }
            }

            public void InvalidateAll()
            {
                IsTransformDirty = true;
                IsMeasureDirty = true;
                IsArrangeDirty = true;
                IsVisualDirty = true;
                TraverseUp(this, (p) =>
                {
                    if (p.IsTransformDirty && p.IsMeasureDirty && p.IsArrangeDirty && p.IsVisualDirty)
                    {
                        return false;
                    }
                    p.IsTransformDirty = true;
                    p.IsMeasureDirty = true;
                    p.IsArrangeDirty = true;
                    p.IsVisualDirty = true;
                    return true;
                });
                if (IsAttached)
                {
                    InvalidateRender();
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected static void TraverseUp(SceneNode2D core, Func<SceneNode2D, bool> action)
            {
                var ancestor = core.Parent as SceneNode2D;
                while (ancestor != null)
                {
                    if (!action(ancestor))
                    { break; }
                    ancestor = ancestor.Parent as SceneNode2D;
                }
            }

            public void Measure(Size2F size)
            {
                if (!IsAttached || Visibility == Visibility.Collapsed || (!IsMeasureDirty && previousMeasureSize == size))
                {
                    return;
                }
                previousMeasureSize = size;
                var availableSize = size.ToVector2();
                var availableSizeWithoutMargin = availableSize - MarginWidthHeight;
                Vector2 maxSize = Vector2.Zero, minSize = Vector2.Zero;
                CalculateMinMax(ref minSize, ref maxSize);

                availableSizeWithoutMargin.X = Math.Max(minSize.X, Math.Min(availableSizeWithoutMargin.X, maxSize.X));
                availableSizeWithoutMargin.Y = Math.Max(minSize.Y, Math.Min(availableSizeWithoutMargin.Y, maxSize.Y));

                var desiredSize = MeasureOverride(availableSizeWithoutMargin.ToSize2F()).ToVector2();

                var unclippedDesiredSize = desiredSize;

                bool clipped = false;
                if (desiredSize.X > maxSize.X)
                {
                    desiredSize.X = maxSize.X;
                    clipped = true;
                }

                if (desiredSize.Y > maxSize.Y)
                {
                    desiredSize.Y = maxSize.Y;
                    clipped = true;
                }

                var clippedDesiredSize = desiredSize + MarginWidthHeight;

                if (clippedDesiredSize.X > availableSize.X)
                {
                    clippedDesiredSize.X = availableSize.X;
                    clipped = true;
                }

                if (clippedDesiredSize.Y > availableSize.Y)
                {
                    clippedDesiredSize.Y = availableSize.Y;
                    clipped = true;
                }

                if (clipped || clippedDesiredSize.X < 0 || clippedDesiredSize.Y < 0)
                {
                    UnclippedDesiredSize = unclippedDesiredSize;
                }
                else
                {
                    UnclippedDesiredSize = new Vector2(-1, -1);
                }
                if (DesiredSize != clippedDesiredSize)
                {
                    DesiredSize = clippedDesiredSize;
                    for (int i = 0; i < ItemsInternal.Count; ++i)
                    {
                        ItemsInternal[i].InvalidateMeasure();
                    }
                }
                else
                {
                    IsMeasureDirty = false;
                }
            }

            public void Arrange(RectangleF rect)
            {
                if (!IsAttached || Visibility == Visibility.Collapsed)
                {
                    return;
                }
                if (IsMeasureDirty)
                {
                    Measure(previousMeasureSize ?? rect.Size);
                }
                bool ancestorDirty = false;
                TraverseUp(this, (parent) =>
                {
                    if (parent.IsArrangeDirty)
                    {
                        ancestorDirty = true;
                        return false;
                    }
                    else { return true; }
                });

                var rectWidthHeight = new Vector2(rect.Width, rect.Height);

                if ((!IsArrangeDirty && !ancestorDirty && previousArrange == rect) || rectWidthHeight.IsZero)
                    return;
                previousArrange = rect;
                var arrangeSize = rectWidthHeight;

                ClipEnabled = false;
                var desiredSize = DesiredSize;

                if (float.IsNaN(DesiredSize.X) || float.IsNaN(DesiredSize.Y))
                {
                    if (UnclippedDesiredSize.X == -1 || UnclippedDesiredSize.Y == -1)
                    {
                        desiredSize = arrangeSize - MarginWidthHeight;
                    }
                    else
                    {
                        desiredSize = UnclippedDesiredSize - MarginWidthHeight;
                    }
                }

                if (arrangeSize.X < desiredSize.X)
                {
                    ClipEnabled = true;
                    arrangeSize.X = desiredSize.X;
                }

                if (arrangeSize.Y < desiredSize.Y)
                {
                    ClipEnabled = true;
                    arrangeSize.Y = desiredSize.Y;
                }

                if (HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    arrangeSize.X = desiredSize.X;
                }

                if (VerticalAlignment != VerticalAlignment.Stretch)
                {
                    arrangeSize.Y = desiredSize.Y;
                }

                Vector2 minSize = Vector2.Zero, maxSize = Vector2.Zero;

                CalculateMinMax(ref minSize, ref maxSize);

                float calcedMaxWidth = Math.Max(desiredSize.X, maxSize.X);
                if (calcedMaxWidth < arrangeSize.X)
                {
                    ClipEnabled = true;
                    arrangeSize.X = calcedMaxWidth;
                }

                float calcedMaxHeight = Math.Max(desiredSize.Y, maxSize.Y);
                if (calcedMaxHeight < arrangeSize.Y)
                {
                    ClipEnabled = true;
                    arrangeSize.Y = calcedMaxHeight;
                }

                var oldRenderSize = RenderSize;
                var arrangeResultSize = ArrangeOverride(new RectangleF(Margin.Left, Margin.Top, arrangeSize.X - MarginWidthHeight.X, arrangeSize.Y - MarginWidthHeight.Y)).ToVector2();

                bool arrangeSizeChanged = arrangeResultSize != oldRenderSize;
                if (arrangeSizeChanged)
                {
                    InvalidateAll();
                }

                RenderSize = arrangeResultSize;

                var clippedArrangeResultSize = new Vector2(Math.Min(arrangeResultSize.X, maxSize.X), Math.Min(arrangeResultSize.Y, maxSize.Y));
                if (!ClipEnabled)
                {
                    ClipEnabled = clippedArrangeResultSize.X < arrangeResultSize.X || clippedArrangeResultSize.Y < arrangeResultSize.Y;
                }

                var clientSize = new Vector2(Math.Max(0, rectWidthHeight.X - MarginWidthHeight.X), Math.Max(0, rectWidthHeight.Y - MarginWidthHeight.Y));

                if (!ClipEnabled)
                {
                    ClipEnabled = clientSize.X < clippedArrangeResultSize.X || clientSize.Y < clippedArrangeResultSize.Y;
                }

                var layoutOffset = Vector2.Zero;

                var tempHorizontalAlign = HorizontalAlignment;
                var tempVerticalAlign = VerticalAlignment;

                if (tempHorizontalAlign == HorizontalAlignment.Stretch && clippedArrangeResultSize.X >= clientSize.X)
                {
                    tempHorizontalAlign = HorizontalAlignment.Left;
                }

                if (tempVerticalAlign == VerticalAlignment.Stretch && clippedArrangeResultSize.Y >= clientSize.Y)
                {
                    tempVerticalAlign = VerticalAlignment.Top;
                }

                if ((tempHorizontalAlign == HorizontalAlignment.Center || tempHorizontalAlign == HorizontalAlignment.Stretch) && clientSize.X >= clippedArrangeResultSize.X)
                {
                    layoutOffset.X = (clientSize.X - clippedArrangeResultSize.X) / 2.0f;
                }
                else if (tempHorizontalAlign == HorizontalAlignment.Right && clientSize.X >= clippedArrangeResultSize.X)
                {
                    layoutOffset.X = clientSize.X - clippedArrangeResultSize.X;
                }
                else
                {
                    layoutOffset.X = 0;
                }

                if ((tempVerticalAlign == VerticalAlignment.Center || tempVerticalAlign == VerticalAlignment.Stretch) && clientSize.Y >= clippedArrangeResultSize.Y)
                {
                    layoutOffset.Y = (clientSize.Y - clippedArrangeResultSize.Y) / 2.0f;
                }
                else if (tempVerticalAlign == VerticalAlignment.Bottom && clientSize.Y >= clippedArrangeResultSize.Y)
                {
                    layoutOffset.Y = clientSize.Y - clippedArrangeResultSize.Y;
                }
                else
                {
                    layoutOffset.Y = 0;
                }

                layoutOffset += new Vector2(rect.Left, rect.Top);

                if (ClipEnabled || ClipToBound)
                {
                    LayoutClipBound = new RectangleF(0, 0, clientSize.X, clientSize.Y);
                }

                LayoutOffsets = layoutOffset;
                UpdateLayoutInternal();
                IsArrangeDirty = false;
            }

            private void CalculateMinMax(ref Vector2 minSize, ref Vector2 maxSize)
            {
                maxSize.Y = MaximumHeight;
                minSize.Y = MinimumHeight;

                var dimensionLength = Height;

                float height = dimensionLength;

                maxSize.Y = Math.Max(Math.Min(height, maxSize.Y), minSize.Y);

                height = (float.IsInfinity(dimensionLength) ? 0 : dimensionLength);

                minSize.Y = Math.Max(Math.Min(maxSize.Y, height), minSize.Y);

                maxSize.X = MaximumWidth;
                minSize.X = MinimumWidth;

                dimensionLength = Width;

                float width = dimensionLength;

                maxSize.X = Math.Max(Math.Min(width, maxSize.X), minSize.X);

                width = (float.IsInfinity(dimensionLength) ? 0 : dimensionLength);

                minSize.X = Math.Max(Math.Min(maxSize.X, width), minSize.X);
            }

            private void UpdateLayoutInternal()
            {
                LayoutBound = new RectangleF((float)Margin.Left, (float)Margin.Top, RenderSize.X, RenderSize.Y);
                LayoutClipBound = new RectangleF(0, 0, RenderSize.X + MarginWidthHeight.X, RenderSize.Y + MarginWidthHeight.Y);
                LayoutTranslate = Matrix3x2.Translation((float)Math.Round(LayoutOffsets.X), (float)Math.Round(LayoutOffsets.Y));
            }

            protected virtual RectangleF ArrangeOverride(RectangleF finalSize)
            {
                for (int i = 0; i < ItemsInternal.Count; ++i)
                {
                    ItemsInternal[i].Arrange(finalSize);
                }
                return finalSize;
            }

            protected virtual Size2F MeasureOverride(Size2F availableSize)
            {
                for (int i = 0; i < ItemsInternal.Count; ++i)
                {
                    ItemsInternal[i].Measure(availableSize);
                }
                return availableSize;
            }

            #endregion layout management
        }
    }


}
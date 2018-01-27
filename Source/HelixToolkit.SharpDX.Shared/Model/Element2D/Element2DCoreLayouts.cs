/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
Reference from https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/
*/
using SharpDX;
using System;
using System.Runtime.CompilerServices;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
#if NETFX_CORE
    public abstract partial class Element2DCore
#else
    public abstract partial class Element2DCore
#endif
    {
        #region layout management
        public bool IsMeasureDirty { protected set; get; } = true;

        public bool IsArrangeDirty { protected set; get; } = true;

        internal bool IsTransformDirty { set; get; } = true;

        internal bool IsVisualDirty { set; get; } = true;

        private Thickness marginInternal = new Thickness();
        internal Thickness MarginInternal
        {
            set
            {
                if (Set(ref marginInternal, value))
                {
                    MarginWidthHeight = new Vector2((float)(value.Left + value.Right), (float)(value.Top + value.Bottom));
                    InvalidateMeasure();
                }
            }
            get
            {
                return marginInternal;
            }
        }

        private Vector2 MarginWidthHeight { set; get; }

        private float widthInternal = float.PositiveInfinity;
        internal float WidthInternal
        {
            set
            {
                if (Set(ref widthInternal, value))
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return widthInternal;
            }
        }


        private float heightInternal = float.PositiveInfinity;
        internal float HeightInternal
        {
            set
            {
                if (Set(ref heightInternal, value))
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return heightInternal;
            }
        }

        private float minimumWidthInternal = 0;
        internal float MinimumWidthInternal
        {
            set
            {
                if (Set(ref minimumWidthInternal, value) && value > widthInternal)
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return minimumWidthInternal;
            }
        }


        private float minimumHeightInternal = 0;
        internal float MinimumHeightInternal
        {
            set
            {
                if (Set(ref minimumHeightInternal, value) && value > heightInternal)
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return minimumHeightInternal;
            }
        }

        private float maximumWidthInternal = float.PositiveInfinity;
        internal float MaximumWidthInternal
        {
            set
            {
                if (Set(ref maximumWidthInternal, value) && value < widthInternal)
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return maximumWidthInternal;
            }
        }


        private float maximumHeightInternal = float.PositiveInfinity;
        internal float MaximumHeightInternal
        {
            set
            {
                if (Set(ref maximumHeightInternal, value) && value < heightInternal)
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return maximumHeightInternal;
            }
        }


        private HorizontalAlignment horizontalAlignmentInternal = HorizontalAlignment.Stretch;
        internal HorizontalAlignment HorizontalAlignmentInternal
        {
            set
            {
                if (Set(ref horizontalAlignmentInternal, value))
                {
                    InvalidateArrange();
                }
            }
            get
            {
                return horizontalAlignmentInternal;
            }
        }


        private VerticalAlignment verticalAlignmentInternal = VerticalAlignment.Stretch;
        internal VerticalAlignment VerticalAlignmentInternal
        {
            set
            {
                if (Set(ref verticalAlignmentInternal, value))
                {
                    InvalidateArrange();
                }
            }
            get
            {
                return verticalAlignmentInternal;
            }
        }
        private Vector2 layoutOffset = Vector2.Zero;
        public Vector2 LayoutOffsets
        {
            private set
            {
                if(Set(ref layoutOffset, value))
                {
                    InvalidateRender();
                }
            }
            get { return layoutOffset; }
        }

        private Size2F renderSize = Size2F.Zero;
        public Size2F RenderSize
        {
            get { return renderSize; }
            private set
            {
                if (Set(ref renderSize, value))
                {
                    InvalidateRender();
                }
            }
        }

        private Vector2 renderTransformOriginInternal = new Vector2(0.5f, 0.5f);
        internal Vector2 RenderTransformOriginInternal
        {
            set
            {
                if(Set(ref renderTransformOriginInternal, value))
                {
                    InvalidateRender();
                }
            }
            get { return renderTransformOriginInternal; }
        }

        public Vector2 DesiredSize { get; private set; }
        public Vector2 UnclippedDesiredSize { get; private set; } = new Vector2(-1, -1);

        public Vector2 Size { get { return new Vector2(widthInternal, heightInternal); } }

        public bool ClipEnabled { private set; get; } = false;

        public bool ClipToBound { set; get; } = false;

        public RectangleF ClipBound
        {
            private set
            {
                RenderCore.LayoutClippingBound = value;
            }
            get { return RenderCore.LayoutClippingBound; }
        }

        public RectangleF LayoutBound
        {
            private set
            {
                RenderCore.LayoutBound = value;
                LayoutBoundWithTransform = value;
            }
            get { return RenderCore.LayoutBound; }
        }

        public void InvalidateMeasure()
        {
            IsArrangeDirty = true;
            IsMeasureDirty = true;
            TraverseUp(this, (p) =>
            {
                if(p.IsArrangeDirty && p.IsMeasureDirty)
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
        protected static void TraverseUp(Element2DCore core, Func<Element2DCore, bool> action)
        {
            var ancestor = core.Parent as Element2DCore;
            while (ancestor != null)
            {
                if (!action(ancestor))
                { break; }
                ancestor = ancestor.Parent as Element2DCore;
            }
        }

        private Size2F? previousMeasureSize;
        private RectangleF? previousArrange;

        public void Measure(Size2F size)
        {
            if (!IsAttached || VisibilityInternal == Visibility.Collapsed || (!IsMeasureDirty && previousMeasureSize != size))
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
                foreach (var item in Items)
                {
                    item.InvalidateMeasure();
                }
            }
            else
            {
                IsMeasureDirty = false;
            }
        }

        public void Arrange(RectangleF rect)
        {
            if (!IsAttached || VisibilityInternal == Visibility.Collapsed)
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

            if (HorizontalAlignmentInternal != HorizontalAlignment.Stretch)
            {
                arrangeSize.X = desiredSize.X;
            }

            if (VerticalAlignmentInternal != VerticalAlignment.Stretch)
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

            var oldRenderSize = RenderSize.ToVector2();
            var arrangeResultSize = ArrangeOverride(arrangeSize.ToRectangleF()).ToVector2();

            bool arrangeSizeChanged = arrangeResultSize != oldRenderSize;
            if (arrangeSizeChanged)
            {
                InvalidateAll();
            }

            RenderSize = arrangeResultSize.ToSize2F();

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

            var tempHorizontalAlign = HorizontalAlignmentInternal;
            var tempVerticalAlign = VerticalAlignmentInternal;

            if (tempHorizontalAlign == HorizontalAlignment.Stretch && clippedArrangeResultSize.X > clientSize.X)
            {
                tempHorizontalAlign = HorizontalAlignment.Left;
            }

            if (tempVerticalAlign == VerticalAlignment.Stretch && clippedArrangeResultSize.Y > clientSize.Y)
            {
                tempVerticalAlign = VerticalAlignment.Top;
            }

            if (tempHorizontalAlign == HorizontalAlignment.Center || tempHorizontalAlign == HorizontalAlignment.Stretch)
            {
                layoutOffset.X = (clientSize.X - clippedArrangeResultSize.X) / 2.0f;
            }
            else if (tempHorizontalAlign == HorizontalAlignment.Right)
            {
                layoutOffset.X = clientSize.X - clippedArrangeResultSize.X;// - (float)MarginInternal.Right;
            }
            else
            {
                layoutOffset.X = 0;// (float)MarginInternal.Left;
            }

            if (tempVerticalAlign == VerticalAlignment.Center || tempVerticalAlign == VerticalAlignment.Stretch)
            {
                layoutOffset.Y = (clientSize.Y - clippedArrangeResultSize.Y) / 2.0f;
            }
            else if (tempVerticalAlign == VerticalAlignment.Bottom)
            {
                layoutOffset.Y = clientSize.Y - clippedArrangeResultSize.Y;// - (float)MarginInternal.Bottom;
            }
            else
            {
                layoutOffset.Y = 0;// (float)MarginInternal.Top;
            }

            layoutOffset += new Vector2(rect.Left, rect.Top);

            if (ClipEnabled || ClipToBound)
            {
                ClipBound = new RectangleF(0, 0, clientSize.X, clientSize.Y);
            }

            LayoutOffsets = layoutOffset;
            UpdateLayoutInternal();
            IsArrangeDirty = false;
        }

        private void CalculateMinMax(ref Vector2 minSize, ref Vector2 maxSize)
        {
            maxSize.Y = MaximumHeightInternal;
            minSize.Y = MinimumHeightInternal;

            var dimensionLength = HeightInternal;

            float height = dimensionLength;

            maxSize.Y = Math.Max(Math.Min(height, maxSize.Y), minSize.Y);

            height = (float.IsInfinity(dimensionLength) ? 0 : dimensionLength);

            minSize.Y = Math.Max(Math.Min(maxSize.Y, height), minSize.Y);

            maxSize.X = MaximumWidthInternal;
            minSize.X = MinimumWidthInternal;

            dimensionLength = WidthInternal;

            float width = dimensionLength;

            maxSize.X = Math.Max(Math.Min(width, maxSize.X), minSize.X);

            width = (float.IsInfinity(dimensionLength) ? 0 : dimensionLength);

            minSize.X = Math.Max(Math.Min(maxSize.X, width), minSize.X);
        }

        private void UpdateLayoutInternal()
        {
            LayoutBound = new RectangleF((float)MarginInternal.Left, (float)MarginInternal.Top, RenderSize.Width - MarginWidthHeight.X, RenderSize.Height - MarginWidthHeight.Y);
            ClipBound = new RectangleF(0, 0, RenderSize.Width, RenderSize.Height);
            LayoutTranslate = Matrix3x2.Translation(LayoutOffsets.X, LayoutOffsets.Y);
        }

        protected virtual RectangleF ArrangeOverride(RectangleF finalSize)
        {
            foreach(var item in Items)
            {
                item.Arrange(finalSize);
            }
            return finalSize;
        }

        protected virtual Size2F MeasureOverride(Size2F availableSize)
        {
            //if (float.IsInfinity(WidthInternal))
            //{
            //    WidthInternal = availableSize.Width;
            //}
            //if(float.IsInfinity(HeightInternal))
            //{
            //    HeightInternal = availableSize.Height;
            //}
            foreach(var item in Items)
            {
                //item.InvalidateMeasure();
                item.Measure(availableSize);
            }
            return availableSize;
        }
        #endregion
    }
}

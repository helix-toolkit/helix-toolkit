/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;

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
        internal bool IsMeasureValid { set; get; } = false;
        internal bool IsArrangeValid { set; get; } = false;

        private Vector2 marginInternal = Vector2.Zero;
        internal Vector2 MarginInternal
        {
            set
            {
                if (Set(ref marginInternal, value))
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return marginInternal;
            }
        }


        private Vector2 positionInternal = Vector2.Zero;
        internal Vector2 PositionInternal
        {
            set
            {
                if (Set(ref positionInternal, value))
                {
                    InvalidateMeasure();
                }
            }
            get
            {
                return positionInternal;
            }
        }


        private float widthInternal;
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


        private float heightInternal;
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

        private float maximumWidthInternal = float.MaxValue;
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


        private float maximumHeightInternal = float.MaxValue;
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


        private HorizontalAlignment horizontalAlignmentInternal = HorizontalAlignment.Center;
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


        private VerticalAlignment verticalAlignmentInternal = VerticalAlignment.Center;
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

        internal Vector2 PositionOffsets { set; get; } = Vector2.Zero;

        private Vector2 renderSize = Vector2.Zero;
        public Vector2 RenderSize
        {
            get { return renderSize; }
            private set
            {
                if (Set(ref renderSize, value))
                {
                    UpdateLayoutInternal();
                }
            }
        }

        public Vector2 DesiredSize { get; private set; }
        public Vector2 DesiredSizeWithMargins { get; private set; }

        public Vector2 Size { get { return new Vector2(widthInternal, heightInternal); } }

        private Vector2 absolutePosition;
        public Vector2 AbsolutePosition
        {
            get { return absolutePosition; }
            protected set
            {
                if (Set(ref absolutePosition, value))
                {
                    UpdateLayoutInternal();
                }
            }
        }

        protected void InvalidateMeasure()
        {
            ForceMeasure();
            PropagateMeasureInvalidationToChildren();
        }


        protected void InvalidateArrange()
        {
            ForceArrange();
            PropagateArrangeInvalidationToChildren();
        }

        internal void ForceMeasure()
        {
            IsMeasureValid = false;
            if (Parent is Element2DCore p)
            {
                p.ForceMeasure();
                Measure(p.RenderSize + p.MarginInternal);
            }
            else if(Parent is FrameworkElement pf)
            {
                Measure(new Vector2((float)pf.ActualWidth, (float)pf.ActualHeight));
            }
            InvalidateArrange();
        }

        internal void ForceArrange()
        {
            IsArrangeValid = false;
            if (Parent is Element2DCore p)
            {
                p.ForceArrange();
                Arrange(p.DesiredSizeWithMargins);
            }
            else if(Parent is FrameworkElement pf)
            {
                Arrange(new Vector2((float)pf.ActualWidth, (float)pf.ActualHeight));
            }
        }

        private void PropagateMeasureInvalidationToChildren()
        {
            foreach (var child in Items)
            {
                if (child is Element2DCore c)
                {
                    c.IsMeasureValid = false;
                    c.PropagateMeasureInvalidationToChildren();
                }
            }
        }

        private void PropagateArrangeInvalidationToChildren()
        {
            foreach (var child in Items)
            {
                if (child is Element2DCore c)
                {
                    c.IsArrangeValid = false;
                    c.PropagateArrangeInvalidationToChildren();
                }
            }
        }

        public void Measure(Vector2 availableSize)
        {
            if (IsMeasureValid || !IsAttached)
                return;

            IsMeasureValid = true;
            var desiredSize = new Vector2(widthInternal, heightInternal);

            if (float.IsNaN(desiredSize.X) || float.IsNaN(desiredSize.Y))
            {
                var availableSizeWithoutMargins = availableSize - MarginInternal;
                availableSizeWithoutMargins = new Vector2(
                    Math.Max(MinimumWidthInternal, Math.Min(MaximumWidthInternal, !float.IsNaN(desiredSize.X) ? desiredSize.X : availableSizeWithoutMargins.X)),
                    Math.Max(MinimumHeightInternal, Math.Min(MaximumHeightInternal, !float.IsNaN(desiredSize.Y) ? desiredSize.Y : availableSizeWithoutMargins.Y)));

                var childrenDesiredSize = MeasureOverride(availableSizeWithoutMargins);
                if (float.IsNaN(desiredSize.X))
                    desiredSize.X = childrenDesiredSize.X;
                if (float.IsNaN(desiredSize.Y))
                    desiredSize.Y = childrenDesiredSize.Y;
            }

            desiredSize = new Vector2(
                    Math.Max(MinimumWidthInternal, Math.Min(MaximumWidthInternal, desiredSize.X)),
                    Math.Max(MinimumHeightInternal, Math.Min(MaximumHeightInternal, desiredSize.Y)));

            DesiredSize = desiredSize;
            DesiredSizeWithMargins = desiredSize + MarginInternal;
        }

        public void Arrange(Vector2 rect)
        {
            if (IsArrangeValid || !IsAttached)
                return;
            IsArrangeValid = true;

            var oldAbsolutePosition = absolutePosition;
            PositionOffsets += MarginInternal;
            var newAbsolutePosition = PositionInternal + PositionOffsets;
            if (Parent is Element2DCore p)
                newAbsolutePosition += p.AbsolutePosition;

            if (!newAbsolutePosition.Equals(oldAbsolutePosition))
                AbsolutePosition = newAbsolutePosition;

            var elementSize = Size;
            var finalSizeWithoutMargins = rect - MarginInternal;
            if (float.IsNaN(elementSize.X) && HorizontalAlignmentInternal == HorizontalAlignment.Stretch)
                elementSize.X = finalSizeWithoutMargins.X;
            if (float.IsNaN(elementSize.Y) && VerticalAlignmentInternal == VerticalAlignment.Stretch)
                elementSize.Y = finalSizeWithoutMargins.Y;

            if (float.IsNaN(elementSize.X))
                elementSize.X = Math.Min(DesiredSize.X, finalSizeWithoutMargins.X);
            if (float.IsNaN(elementSize.Y))
                elementSize.Y = Math.Min(DesiredSize.Y, finalSizeWithoutMargins.Y);

            // trunk the element size between the maximum and minimum width/height of the UIElement
            elementSize = new Vector2(
                Math.Max(MinimumWidthInternal, Math.Min(MaximumWidthInternal, elementSize.X)),
                Math.Max(MinimumHeightInternal, Math.Min(MaximumHeightInternal, elementSize.Y)));

            elementSize = ArrangeOverride(elementSize);
            RenderSize = elementSize;

            PositionOffsets = CalculatePosition(rect, elementSize);
            if (PositionOffsets != Vector2.Zero)
            {
                AbsolutePosition = PositionOffsets;
                foreach (var element in Items)
                {
                    if (element is Element2DCore e)
                    {
                        e.PositionOffsets += PositionOffsets;
                    }
                }
            }
        }
        private void UpdateLayoutInternal()
        {
            Bound = new RectangleF(AbsolutePosition.X, AbsolutePosition.Y, RenderSize.X, RenderSize.Y);
            LayoutTranslate = Matrix3x2.Translation(AbsolutePosition.X, AbsolutePosition.Y);
        }

        protected virtual Vector2 ArrangeOverride(Vector2 availableSizeWithoutMargins)
        {
            return availableSizeWithoutMargins;
        }

        protected virtual Vector2 MeasureOverride(Vector2 availableSizeWithoutMargins)
        {
            return Vector2.Zero;
        }

        private Vector2 CalculatePosition(Vector2 availableSpace, Vector2 usedSpace)
        {
            Vector2 offsets = Vector2.Zero;
            switch (VerticalAlignmentInternal)
            {
                case VerticalAlignment.Bottom:
                    offsets.Y += availableSpace.Y - usedSpace.Y;

                    break;

                case VerticalAlignment.Center:
                    offsets.Y += (availableSpace.Y - usedSpace.Y) / 2;
                    break;
            }

            switch (HorizontalAlignmentInternal)
            {
                case HorizontalAlignment.Center:
                    offsets.X += (availableSpace.X - usedSpace.X) / 2;
                    break;

                case HorizontalAlignment.Right:
                    offsets.X += availableSpace.X - usedSpace.X;
                    break;
            }
            return offsets;
        }

        public void Layout(Vector2 availableSize)
        {
            Measure(availableSize);
            Arrange(DesiredSizeWithMargins);
        }
        #endregion
    }
}

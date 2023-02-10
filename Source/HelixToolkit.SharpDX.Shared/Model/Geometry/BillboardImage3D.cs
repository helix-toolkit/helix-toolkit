/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    public class BillboardImage3D : BillboardBase
    {
        public override BillboardType Type => BillboardType.Image;

        private Color4 maskColor = Color.Transparent;
        /// <summary>
        /// If color in image is equal to the mask color, the color will set to transparent in image.
        /// Default color is Transparent, which did not mask any color.
        /// </summary>
        public Color4 MaskColor
        {
            set
            {
                if (Set(ref maskColor, value))
                {
                    IsInitialized = false;
                }
            }
            get
            {
                return maskColor;
            }
        }
        private ObservableCollection<ImageInfo> imageInfos = new ObservableCollection<ImageInfo>();
        public ObservableCollection<ImageInfo> ImageInfos
        {
            set
            {
                var old = imageInfos;
                if (Set(ref imageInfos, value))
                {
                    old.CollectionChanged -= CollectionChanged;
                    IsInitialized = false;
                    if (value != null)
                    {
                        value.CollectionChanged += CollectionChanged;
                    }
                }
            }
            get
            {
                return imageInfos;
            }
        }

        public BillboardImage3D(Stream imageStream)
        {
            Texture = imageStream;
            imageInfos.CollectionChanged += CollectionChanged;
        }

        public BillboardImage3D(TextureModel texture)
        {
            Texture = texture;
            imageInfos.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsInitialized = false;
        }

        protected override void OnUpdateTextureAndBillboardVertices(IDeviceResources deviceResources)
        {
            foreach (var img in ImageInfos)
            {
                img.UpdateImage();
                DrawImageVertex(img);
            }
        }

        private void DrawImageVertex(ImageInfo info)
        {
            GetQuadOffset(info.Width, info.Height, info.HorizontalAlignment, info.VerticalAlignment, out var tl, out var br);

            var transform = info.Angle != 0 ? Matrix3x2.Rotation(info.Angle) : Matrix3x2.Identity;
            var offTL = tl * info.Scale;
            var offBR = br * info.Scale;
            var offTR = new Vector2(offBR.X, offTL.Y);
            var offBL = new Vector2(offTL.X, offBR.Y);
            BillboardVertices.Add(new BillboardVertex()
            {
                Position = info.Position.ToVector4(),
                Foreground = Color.White,
                Background = maskColor,
                TexTL = info.UV_TopLeft,
                TexBR = info.UV_BottomRight,
                OffTL = Matrix3x2.TransformPoint(transform, offTL) + info.Offset,
                OffBL = Matrix3x2.TransformPoint(transform, offBL) + info.Offset,
                OffBR = Matrix3x2.TransformPoint(transform, offBR) + info.Offset,
                OffTR = Matrix3x2.TransformPoint(transform, offTR) + info.Offset
            });
        }

        public override bool HitTest(HitTestContext context, Matrix modelMatrix, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize)
        {
            var rayWS = context.RayWS;
            if (!IsInitialized || context == null || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
            {
                return false;
            }

            return fixedSize ? HitTestFixedSize(context, ref modelMatrix, ref hits, originalSource, imageInfos.Count)
                : HitTestNonFixedSize(context, ref modelMatrix, ref hits, originalSource, imageInfos.Count);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if (target is BillboardImage3D t)
            {
                t.ImageInfos = new ObservableCollection<ImageInfo>(ImageInfos);
                t.IsInitialized = false;
            }
        }

        public override void UpdateBounds()
        {
            if (ImageInfos.Count == 0)
            {
                Bound = new BoundingBox();
                BoundingSphere = new BoundingSphere();
            }
            else
            {
                var sphere = ImageInfos[0].BoundSphere;
                var bound = BoundingBox.FromSphere(sphere);
                foreach (var info in ImageInfos)
                {
                    sphere = BoundingSphere.Merge(sphere, info.BoundSphere);
                    bound = BoundingBox.Merge(bound, BoundingBox.FromSphere(info.BoundSphere));
                }
                BoundingSphere = sphere;
                Bound = bound;
            }
        }
    }

    public class ImageInfo
    {
        public Vector2 UV_TopLeft
        {
            set; get;
        }
        public Vector2 UV_BottomRight
        {
            set; get;
        }
        public Vector3 Position
        {
            set; get;
        }
        public float Width { set; get; } = 1;
        public float Height { set; get; } = 1;
        public float Angle { set; get; } = 0;
        public float Scale { set; get; } = 1;

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
            set; get;
        } = BillboardHorizontalAlignment.Center;

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
            set; get;
        } = BillboardVerticalAlignment.Center;

        /// <summary>
        /// Additional offset for billboard display location.
        /// Behavior depends on whether billboard is fixed sized or not.
        /// When billboard is fixed sized, the offset is screen spaced.
        /// </summary>
        public Vector2 Offset
        {
            set; get;
        } = Vector2.Zero;

        public virtual void UpdateImage()
        {
            BoundSphere = new BoundingSphere(Position, Math.Max(Width * Scale, Height * Scale) / 2);
        }

        public BoundingSphere BoundSphere
        {
            get; private set;
        }
    }
}

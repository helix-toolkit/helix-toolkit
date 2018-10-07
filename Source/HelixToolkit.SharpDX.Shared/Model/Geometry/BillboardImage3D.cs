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
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
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
            get { return maskColor; }
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
            get { return imageInfos; }
        }

        public BillboardImage3D(Stream imageStream)
        {
            Texture = imageStream;
            imageInfos.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsInitialized = false;
        }

        protected override void OnUpdateTextureAndBillboardVertices(IDeviceResources deviceResources)
        {
            foreach(var img in ImageInfos)
            {
                img.UpdateImage();
                DrawImageVertex(img);
            }
        }

        private void DrawImageVertex(ImageInfo info)
        {
            // CCW from bottom left 
            var tl = new Vector2(-info.Width / 2, info.Height / 2);
            var br = new Vector2(info.Width / 2, -info.Height / 2);

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
                OffTL = Matrix3x2.TransformPoint(transform, offTL),
                OffBL = Matrix3x2.TransformPoint(transform, offBL),
                OffBR = Matrix3x2.TransformPoint(transform, offBR),
                OffTR = Matrix3x2.TransformPoint(transform, offTR)
            });
        }

        public override bool HitTest(RenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits,
            object originalSource, bool fixedSize)
        {
            if (!IsInitialized || context == null || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
            {
                return false;
            }

            return fixedSize ? HitTestFixedSize(context, ref modelMatrix, ref rayWS, ref hits, originalSource, imageInfos.Count)
                : HitTestNonFixedSize(context, ref modelMatrix, ref rayWS, ref hits, originalSource, imageInfos.Count);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardImage3D t)
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
        public Vector2 UV_TopLeft;
        public Vector2 UV_BottomRight;
        public Vector3 Position;
        public float Width = 1;
        public float Height = 1;
        public float Angle = 0;
        public float Scale = 1;
        public virtual void UpdateImage()
        {
            BoundSphere = new BoundingSphere(Position, Math.Max(Width * Scale, Height * Scale) / 2);
        }

        public BoundingSphere BoundSphere { get; private set; }
    }
}

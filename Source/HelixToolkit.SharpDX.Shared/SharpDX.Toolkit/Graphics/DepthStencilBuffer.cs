/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A DepthStencilBuffer front end to <see cref="SharpDX.Direct3D11.Texture2D"/>.
    /// </summary>
    /// <remarks>
    /// This class instantiates a <see cref="Texture2D"/> with the binding flags <see cref="BindFlags.DepthStencil"/>.
    /// </remarks>
    public class DepthStencilBuffer : Texture2DBase
    {
        internal readonly DXGI.Format DefaultViewFormat;
        private TextureView[] depthStencilViews;
        private TextureView[] readOnlyViews;

        /// <summary>
        /// Gets the <see cref="Graphics.DepthFormat"/> of this depth stencil buffer.
        /// </summary>
        public readonly DepthFormat DepthFormat;

        /// <summary>
        /// Gets a boolean value indicating if this buffer is supporting stencil.
        /// </summary>
        public readonly bool HasStencil;

        /// <summary>
        /// Gets a boolean value indicating if this buffer is supporting read-only view.
        /// </summary>
        public readonly bool HasReadOnlyView;

        /// <summary>
        /// Gets the selector for a <see cref="DepthStencilView"/>
        /// </summary>
        public readonly DepthStencilViewSelector DepthStencilView;

        /// <summary>
        /// Gets a a read-only <see cref="DepthStencilView"/>.
        /// </summary>
        /// <remarks>
        /// This value can be null if not supported by hardware (minimum features level is 11.0)
        /// </remarks>
        public DepthStencilView ReadOnlyView
        {
            get
            {
                return readOnlyViews != null ? readOnlyViews[0] : null;
            }
        }

        internal DepthStencilBuffer(Direct3D11.Device device, Texture2DDescription description2D, DepthFormat depthFormat)
            : base(device, description2D)
        {
            DepthFormat = depthFormat;
            DefaultViewFormat = ComputeViewFormat(DepthFormat, out HasStencil);
            Initialize(Resource);
            HasReadOnlyView = InitializeViewsDelayed();
            DepthStencilView = new DepthStencilViewSelector(this);
        }

        internal DepthStencilBuffer(Direct3D11.Device device, Direct3D11.Texture2D texture, DepthFormat depthFormat)
            : base(device, texture)
        {
            DepthFormat = depthFormat;
            DefaultViewFormat = ComputeViewFormat(DepthFormat, out HasStencil);
            Initialize(Resource);
            HasReadOnlyView = InitializeViewsDelayed();
            DepthStencilView = new DepthStencilViewSelector(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeViews()
        {
            // Override this, because we need the DepthFormat setup in order to initialize this class
            // This is caused by a bad design of the constructors/initialize sequence. 
            // TODO: Fix this problem
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool InitializeViewsDelayed()
        {
            bool hasReadOnlyView = false;

            // Perform default initialization
            base.InitializeViews();

            if ((Description.BindFlags & BindFlags.DepthStencil) == 0)
                return hasReadOnlyView;

            int viewCount = GetViewCount();
            depthStencilViews = new TextureView[viewCount];
            GetDepthStencilView(ViewType.Full, 0, 0, false);

            // ReadOnly for feature level Direct3D11
            if (((Direct3D11.Device)GraphicsDevice).FeatureLevel >= FeatureLevel.Level_11_0)
            {
                hasReadOnlyView = true;
                readOnlyViews = new TextureView[viewCount];
                GetDepthStencilView(ViewType.Full, 0, 0, true);
            }

            return hasReadOnlyView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Format GetDefaultViewFormat()
        {
            return DefaultViewFormat;
        }

        /// <summary>
        /// DepthStencilView casting operator.
        /// </summary>
        /// <param name="buffer">Source for the.</param>
        public static implicit operator DepthStencilView(DepthStencilBuffer buffer)
        {
            return buffer == null ? null : buffer.depthStencilViews != null ? buffer.depthStencilViews[0] : null;
        }

        internal override TextureView GetRenderTargetView(ViewType viewType, int arrayOrDepthSlice, int mipIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a specific <see cref="DepthStencilView" /> from this texture.
        /// </summary>
        /// <param name="viewType">Type of the view slice.</param>
        /// <param name="arrayOrDepthSlice">The texture array slice index.</param>
        /// <param name="mipIndex">The mip map slice index.</param>
        /// <param name="readOnlyView">Indicates if the view is read-only.</param>
        /// <returns>A <see cref="DepthStencilView" /></returns>
        internal virtual TextureView GetDepthStencilView(ViewType viewType, int arrayOrDepthSlice, int mipIndex, bool readOnlyView)
        {
            if ((this.Description.BindFlags & BindFlags.DepthStencil) == 0)
                return null;

            if (viewType == ViewType.MipBand)
                throw new NotSupportedException("ViewSlice.MipBand is not supported for depth stencils");

            if (readOnlyView && !HasReadOnlyView)
                return null;

            var views = readOnlyView ? readOnlyViews : depthStencilViews;

            int arrayCount;
            int mipCount;
            GetViewSliceBounds(viewType, ref arrayOrDepthSlice, ref mipIndex, out arrayCount, out mipCount);

            var dsvIndex = GetViewIndex(viewType, arrayOrDepthSlice, mipIndex);

            lock (views)
            {
                var dsv = views[dsvIndex];

                // Creates the shader resource view
                if (dsv == null)
                {
                    // Create the depth stencil view
                    var dsvDescription = new DepthStencilViewDescription() { Format = (Format)DepthFormat };

                    if (this.Description.ArraySize > 1)
                    {
                        dsvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? DepthStencilViewDimension.Texture2DMultisampledArray : DepthStencilViewDimension.Texture2DArray;
                        if (this.Description.SampleDescription.Count > 1)
                        {
                            dsvDescription.Texture2DMSArray.ArraySize = arrayCount;
                            dsvDescription.Texture2DMSArray.FirstArraySlice = arrayOrDepthSlice;
                        }
                        else
                        {
                            dsvDescription.Texture2DArray.ArraySize = arrayCount;
                            dsvDescription.Texture2DArray.FirstArraySlice = arrayOrDepthSlice;
                            dsvDescription.Texture2DArray.MipSlice = mipIndex;
                        }
                    }
                    else
                    {
                        dsvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? DepthStencilViewDimension.Texture2DMultisampled : DepthStencilViewDimension.Texture2D;
                        if (this.Description.SampleDescription.Count <= 1)
                            dsvDescription.Texture2D.MipSlice = mipIndex;
                    }

                    if (readOnlyView)
                    {
                        dsvDescription.Flags = DepthStencilViewFlags.ReadOnlyDepth;
                        if (HasStencil)
                            dsvDescription.Flags |= DepthStencilViewFlags.ReadOnlyStencil;
                    }

                    dsv = new TextureView(this, new DepthStencilView(GraphicsDevice, Resource, dsvDescription));
                    views[dsvIndex] = ToDispose(dsv);
                }

                return dsv;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Texture Clone()
        {
            return new DepthStencilBuffer(GraphicsDevice, this.Description, DepthFormat);
        }

        private static DXGI.Format ComputeViewFormat(DepthFormat format, out bool hasStencil)
        {
            DXGI.Format viewFormat;
            hasStencil = false;

            // Determine TypeLess Format and ShaderResourceView Format
            switch (format)
            {
                case DepthFormat.Depth16:
                    viewFormat = SharpDX.DXGI.Format.R16_Float;
                    break;
                case DepthFormat.Depth32:
                    viewFormat = SharpDX.DXGI.Format.R32_Float;
                    break;
                case DepthFormat.Depth24Stencil8:
                    viewFormat = SharpDX.DXGI.Format.R24_UNorm_X8_Typeless;
                    hasStencil = true;
                    break;
                case DepthFormat.Depth32Stencil8X24:
                    viewFormat = SharpDX.DXGI.Format.R32_Float_X8X24_Typeless;
                    hasStencil = true;
                    break;
                default:
                    viewFormat = DXGI.Format.Unknown;
                    break;
            }

            return viewFormat;
        }


        private static DepthFormat ComputeViewFormat(DXGI.Format format)
        {
            switch (format)
            {
                case SharpDX.DXGI.Format.D16_UNorm:
                case DXGI.Format.R16_Float:
                case DXGI.Format.R16_Typeless:
                    return DepthFormat.Depth16;

                case SharpDX.DXGI.Format.D32_Float:
                case DXGI.Format.R32_Float:
                case DXGI.Format.R32_Typeless:
                    return DepthFormat.Depth32;

                case SharpDX.DXGI.Format.D24_UNorm_S8_UInt:
                case SharpDX.DXGI.Format.R24_UNorm_X8_Typeless:
                    return DepthFormat.Depth24Stencil8;

                case SharpDX.DXGI.Format.D32_Float_S8X24_UInt:
                case SharpDX.DXGI.Format.R32_Float_X8X24_Typeless:
                    return DepthFormat.Depth32Stencil8X24;
            }
            throw new InvalidOperationException(string.Format("Unsupported DXGI.FORMAT [{0}] for depth buffer", format));
        }
    }
}
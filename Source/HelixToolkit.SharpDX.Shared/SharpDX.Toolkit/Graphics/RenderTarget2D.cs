/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A RenderTarget2D front end to <see cref="SharpDX.Direct3D11.Texture2D"/>.
    /// </summary>
    /// <remarks>
    /// This class instantiates a <see cref="Texture2D"/> with the binding flags <see cref="BindFlags.RenderTarget"/>.
    /// This class is also castable to <see cref="Direct3D11.RenderTargetView"/>.
    /// </remarks>
    public class RenderTarget2D : Texture2DBase
    {
        private bool pureRenderTarget;
        private RenderTargetView customRenderTargetView;

        internal RenderTarget2D(Direct3D11.Device device, Texture2DDescription description2D)
            : base(device, description2D)
        {
            Initialize(Resource);
        }

        internal RenderTarget2D(Direct3D11.Device device, Direct3D11.Texture2D texture, RenderTargetView renderTargetView = null, bool pureRenderTarget = false)
            : base(device, texture)
        {
            this.pureRenderTarget = pureRenderTarget;
            this.customRenderTargetView = renderTargetView;
            Initialize(Resource);
        }

        /// <summary>
        /// RenderTargetView casting operator.
        /// </summary>
        /// <param name="from">Source for the.</param>
        public static implicit operator RenderTargetView(RenderTarget2D from)
        {
            return from == null ? null : from.renderTargetViews != null ? from.renderTargetViews[0] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeViews()
        {
            if ((this.Description.BindFlags & BindFlags.RenderTarget) != 0)
            {
                this.renderTargetViews = new TextureView[GetViewCount()];
            }

            if (pureRenderTarget)
            {
                renderTargetViews[0] = new TextureView(this, customRenderTargetView);
            }
            else
            {
                // Perform default initialization
                base.InitializeViews();

                if ((this.Description.BindFlags & BindFlags.RenderTarget) != 0)
                {
                    GetRenderTargetView(ViewType.Full, 0, 0);
                }
            }
        }

        internal override TextureView GetRenderTargetView(ViewType viewType, int arrayOrDepthSlice, int mipIndex)
        {
            if ((this.Description.BindFlags & BindFlags.RenderTarget) == 0)
                return null;

            if (viewType == ViewType.MipBand)
                throw new NotSupportedException("ViewSlice.MipBand is not supported for render targets");

            int arrayCount;
            int mipCount;
            GetViewSliceBounds(viewType, ref arrayOrDepthSlice, ref mipIndex, out arrayCount, out mipCount);

            var rtvIndex = GetViewIndex(viewType, arrayOrDepthSlice, mipIndex);

            lock (this.renderTargetViews)
            {
                var rtv = this.renderTargetViews[rtvIndex];

                // Creates the shader resource view
                if (rtv == null)
                {
                    // Create the render target view
                    var rtvDescription = new RenderTargetViewDescription() { Format = this.Description.Format };

                    if (this.Description.ArraySize > 1)
                    {
                        rtvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? RenderTargetViewDimension.Texture2DMultisampledArray : RenderTargetViewDimension.Texture2DArray;
                        if (this.Description.SampleDescription.Count > 1)
                        {
                            rtvDescription.Texture2DMSArray.ArraySize = arrayCount;
                            rtvDescription.Texture2DMSArray.FirstArraySlice = arrayOrDepthSlice;
                        }
                        else
                        {
                            rtvDescription.Texture2DArray.ArraySize = arrayCount;
                            rtvDescription.Texture2DArray.FirstArraySlice = arrayOrDepthSlice;
                            rtvDescription.Texture2DArray.MipSlice = mipIndex;
                        }
                    }
                    else
                    {
                        rtvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? RenderTargetViewDimension.Texture2DMultisampled : RenderTargetViewDimension.Texture2D;
                        if (this.Description.SampleDescription.Count <= 1)
                            rtvDescription.Texture2D.MipSlice = mipIndex;
                    }

                    rtv = new TextureView(this, new RenderTargetView(GraphicsDevice, Resource, rtvDescription));
                    this.renderTargetViews[rtvIndex] = ToDispose(rtv);
                }

                return rtv;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Texture Clone()
        {
            return new RenderTarget2D(GraphicsDevice, this.Description);
        }

        /// <summary>
        /// <see cref="SharpDX.Direct3D11.Texture2D"/> casting operator.
        /// </summary>
        /// <param name="from">From the Texture1D.</param>
        public static implicit operator SharpDX.Direct3D11.Texture2D(RenderTarget2D from)
        {
            // Don't bother with multithreading here
            return from == null ? null : from.Resource;
        }

        /// <summary>
        /// Implicit casting operator to <see cref="Direct3D11.Resource"/>
        /// </summary>
        /// <param name="from">The GraphicsResource to convert from.</param>
        public static implicit operator SharpDX.Direct3D11.Resource(RenderTarget2D from)
        {
            return from == null ? null : (SharpDX.Direct3D11.Resource)from.Resource;
        }
    }
}
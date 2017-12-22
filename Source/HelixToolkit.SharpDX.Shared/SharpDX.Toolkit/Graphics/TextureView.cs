/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A texture view is a view on a specified mipmap set of a <see cref="Texture"/>, a RenderTarget or a <see cref="DepthStencilBuffer"/>. 
    /// An instance of this class is castable to <see cref="ShaderResourceView"/>, <see cref="RenderTargetView"/> or <see cref="DepthStencilView"/> depending on the underlying native view.
    /// </summary>
    public sealed class TextureView : GraphicsResource
    {
        internal TextureView(Texture texture, DeviceChild view) : base(texture.GraphicsDevice)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            Texture = texture;
            Description = texture.Description;

            Initialize(view);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        protected override void Initialize(DeviceChild view)
        {
            // The initialize method will override the view.Tag, so we are setting it back
            base.Initialize(view);

            var shaderResourceView = view as ShaderResourceView;
            int mipLevel = 0;
            bool isMultisampled = Texture.Description.SampleDescription.Count > 1;

            if (shaderResourceView != null)
            {
                mipLevel = isMultisampled ? 0 : shaderResourceView.Description.Texture1D.MostDetailedMip;
            }
            else
            {
                var renderTargetView = view as RenderTargetView;
                if (renderTargetView != null)
                {
                    IsRenderView = true;
                    mipLevel = isMultisampled ? 0 : renderTargetView.Description.Texture1D.MipSlice;
                }
                else
                {
                    var depthStencilView = view as DepthStencilView;
                    if (depthStencilView != null)
                    {
                        IsDepthStencilView = true;
                        mipLevel = isMultisampled ? 0 : depthStencilView.Description.Texture1D.MipSlice;
                    }
                    else
                    {
                        throw new ArgumentException("Expecting argument to be a ShaderResourceView, RenderTargetView or DepthStencilView", "view");
                    }
                }
            }
            Size = new Size2(Math.Max(1, Texture.Width >> mipLevel), Math.Max(1, Texture.Height >> mipLevel));

            TexelSize = new Size2F
            {
                Width = 1.0f / Size.Width,
                Height = 1.0f / Size.Height
            };
        }

        /// <summary>
        /// Gets the width of this view (taking account the miplevel of the view).
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return Size.Width;
            }
        }

        /// <summary>
        /// Gets the height of this view (taking account the miplevel of the view).
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return Size.Height;
            }
        }

        /// <summary>
        /// Gets the format of this view.
        /// </summary>
        /// <value>The format.</value>
        public SharpDX.DXGI.Format Format
        {
            get
            {
                return Texture.Format;
            }
        }

        /// <summary>
        /// The description.
        /// </summary>
        public readonly TextureDescription Description;

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public DeviceChild View
        {
            get
            {
                return Resource;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is render view.
        /// </summary>
        /// <value><c>true</c> if this instance is render view; otherwise, <c>false</c>.</value>
        public bool IsRenderView { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a depth stencil view.
        /// </summary>
        /// <value><c>true</c> if this instance is a depth stencil view; otherwise, <c>false</c>.</value>
        public bool IsDepthStencilView { get; private set; }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>The texture.</value>
        public Texture Texture { get; private set; }

        /// <summary>
        /// The size of the view in pixel (taking account the miplevel of the view).
        /// </summary>
        public Size2 Size { get; private set; }

        /// <summary>
        /// The size of a texel in the view (1 texel = (1.0f / width of the view, 1.0f / height of the view)
        /// </summary>
        public Size2F TexelSize { get; private set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TextureView"/> to <see cref="ShaderResourceView"/>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ShaderResourceView(TextureView view)
        {
            return view == null ? null : view.View as ShaderResourceView;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TextureView"/> to <see cref="RenderTargetView"/>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RenderTargetView(TextureView view)
        {
            return view == null ? null : view.View as RenderTargetView;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TextureView"/> to <see cref="DepthStencilView"/>.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator DepthStencilView(TextureView view)
        {
            return view == null ? null : view.View as DepthStencilView;
        }
    }
}
// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Base class for all <see cref="GraphicsResource"/>.
    /// </summary>
    public abstract class GraphicsResource : Component
    {
        /// <summary>
        /// Device used to create this instance.
        /// </summary>
        public Device GraphicsDevice { get; internal set; }

        /// <summary>
        /// The attached Direct3D11 resource to this instance.
        /// </summary>
        internal DeviceChild Resource;

        internal GraphicsResource()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        protected GraphicsResource(Device graphicsDevice) : this(graphicsDevice, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="name"></param>
        protected GraphicsResource(Device graphicsDevice, string name) : base(name)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            GraphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Initializes the specified device local.
        /// </summary>
        /// <param name="resource">The resource.</param>
        protected virtual void Initialize(DeviceChild resource)
        {
            Resource = ToDispose(resource);
            if (resource != null)
            {
                resource.Tag = this;
            }
        }

        /// <summary>
        /// Implicit casting operator to <see cref="Direct3D11.Resource"/>
        /// </summary>
        /// <param name="from">The GraphicsResource to convert from.</param>
        public static implicit operator Resource(GraphicsResource from)
        {
            return from == null ? null : (Resource)from.Resource;
        }

        /// <summary>
        /// Gets the CPU access flags from the <see cref="ResourceUsage"/>.
        /// </summary>
        /// <param name="usage">The usage.</param>
        /// <returns>The CPU access flags</returns>
        protected static CpuAccessFlags GetCpuAccessFlagsFromUsage(ResourceUsage usage)
        {
            switch (usage)
            {
                case ResourceUsage.Dynamic:
                    return CpuAccessFlags.Write;
                case ResourceUsage.Staging:
                    return CpuAccessFlags.Read | CpuAccessFlags.Write;
            }
            return CpuAccessFlags.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
            if (disposeManagedResources)
                Resource = null;
        }

        /// <summary>
        /// Called when name changed for this component.
        /// </summary>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Name")
            {
                if ((GraphicsDevice.CreationFlags & DeviceCreationFlags.Debug) != 0 && this.Resource != null)
                    this.Resource.DebugName = Name;
            }
        }
    }
}
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
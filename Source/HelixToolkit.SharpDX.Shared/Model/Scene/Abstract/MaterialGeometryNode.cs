/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    public abstract class MaterialGeometryNode : GeometryNode
    {
        private bool isTransparent = false;

        private MaterialCore material;

        /// <summary>
        /// Specifiy if model material is transparent.
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return isTransparent; }
            set
            {
                if (Set(ref isTransparent, value))
                {
                    if (RenderType == RenderType.Opaque || RenderType == RenderType.Transparent)
                    {
                        RenderType = value ? RenderType.Transparent : RenderType.Opaque;
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public MaterialCore Material
        {
            get { return material; }
            set
            {
                if (Set(ref material, value))
                {
                    if (RenderHost != null)
                    {
                        if (IsAttached)
                        {
                            AttachMaterial();
                            InvalidateRender();
                        }
                        else
                        {
                            var host = RenderHost;
                            Detach();
                            Attach(host);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void AttachMaterial()
        {
            if(RenderCore is IMaterialRenderParams core)
            {
                core.Material = this.Material;
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }
            // --- material
            this.AttachMaterial();
            return true;
        }
    }
}
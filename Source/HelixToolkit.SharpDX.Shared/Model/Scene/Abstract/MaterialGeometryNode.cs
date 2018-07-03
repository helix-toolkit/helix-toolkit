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
                    (RenderCore as IMaterialRenderParams).Material = material;
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
        /// Render environment map on this mesh if has environment map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderEnvironmentMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderEnvironmentMap; }
            set { (RenderCore as IMaterialRenderParams).RenderEnvironmentMap = value; }
        }

        /// <summary>
        /// Render shadow on this mesh if has shadow map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderShadowMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderShadowMap; }
            set { (RenderCore as IMaterialRenderParams).RenderShadowMap = value; }
        }
        /// <summary>
        ///
        /// </summary>
        protected virtual void AttachMaterial()
        {
            var core = RenderCore as IMaterialRenderParams;
            core.Material = this.Material;
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
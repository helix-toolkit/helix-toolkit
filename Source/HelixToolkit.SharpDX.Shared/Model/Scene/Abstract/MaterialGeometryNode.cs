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
                    if (RenderCore.RenderType == RenderType.Opaque || RenderCore.RenderType == RenderType.Transparent)
                    {
                        RenderCore.RenderType = value ? RenderType.Transparent : RenderType.Opaque;
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
        ///
        /// </summary>
        public bool RenderDiffuseAlphaMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderDiffuseAlphaMap; }
            set { (RenderCore as IMaterialRenderParams).RenderDiffuseAlphaMap = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RenderDiffuseMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderDiffuseMap; }
            set { (RenderCore as IMaterialRenderParams).RenderDiffuseMap = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RenderDisplacementMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderDisplacementMap; }
            set { (RenderCore as IMaterialRenderParams).RenderDisplacementMap = value; }
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
        ///
        /// </summary>
        public bool RenderNormalMap
        {
            get { return (RenderCore as IMaterialRenderParams).RenderNormalMap; }
            set { (RenderCore as IMaterialRenderParams).RenderNormalMap = value; }
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
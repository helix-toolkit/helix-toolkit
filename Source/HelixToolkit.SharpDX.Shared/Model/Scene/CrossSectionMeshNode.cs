/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class CrossSectionMeshNode : MeshNode
    {
        public Color4 CrossSectionColor
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).SectionColor = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).SectionColor;
            }
        }

        public bool EnablePlane1
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane1Enabled = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).Plane1Enabled;
            }
        }

        public bool EnablePlane2
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane2Enabled = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).Plane2Enabled;
            }
        }
        public bool EnablePlane3
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane3Enabled = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).Plane3Enabled;
            }
        }
        public bool EnablePlane4
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane4Enabled = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).Plane4Enabled;
            }
        }

        public Plane Plane1
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane1Params = PlaneToVector(ref value);
            }
            get
            {
                return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane1Params);
            }
        }

        public Plane Plane2
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane2Params = PlaneToVector(ref value);
            }
            get
            {
                return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane2Params);
            }
        }

        public Plane Plane3
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane3Params = PlaneToVector(ref value);
            }
            get
            {
                return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane3Params);
            }
        }

        public Plane Plane4
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).Plane4Params = PlaneToVector(ref value);
            }
            get
            {
                return VectorToPlane((RenderCore as ICrossSectionRenderParams).Plane4Params);
            }
        }

        /// <summary>
        /// The PlaneToVector
        /// </summary>
        /// <param name="p">The <see cref="Plane"/></param>
        /// <returns>The <see cref="Vector4"/></returns>
        private static Vector4 PlaneToVector(ref Plane p)
        {
            return new Vector4(p.Normal, p.D);
        }

        private static Plane VectorToPlane(Vector4 v)
        {
            return new Plane(v.ToXYZ(), v.W);
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.CrossSection];
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new CrossSectionMeshRenderCore();
        }
    }
}

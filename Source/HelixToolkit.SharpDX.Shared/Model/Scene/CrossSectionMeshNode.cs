/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using System.Numerics;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class CrossSectionMeshNode : MeshNode
    {
        /// <summary>
        /// Gets or sets the cutting operation.
        /// </summary>
        /// <value>
        /// The cutting operation.
        /// </value>
        public CuttingOperation CuttingOperation
        {
            set
            {
                (RenderCore as ICrossSectionRenderParams).CuttingOperation = value;
            }
            get
            {
                return (RenderCore as ICrossSectionRenderParams).CuttingOperation;
            }
        }
        /// <summary>
        /// Gets or sets the color of the cross section.
        /// </summary>
        /// <value>
        /// The color of the cross section.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether [enable plane1].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable plane1]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether [enable plane2].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable plane2]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether [enable plane3].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable plane3]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether [enable plane4].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable plane4]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets the plane1.
        /// </summary>
        /// <value>
        /// The plane1.
        /// </value>
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
        /// <summary>
        /// Gets or sets the plane2.
        /// </summary>
        /// <value>
        /// The plane2.
        /// </value>
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
        /// <summary>
        /// Gets or sets the plane3.
        /// </summary>
        /// <value>
        /// The plane3.
        /// </value>
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
        /// <summary>
        /// Gets or sets the plane4.
        /// </summary>
        /// <value>
        /// The plane4.
        /// </value>
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
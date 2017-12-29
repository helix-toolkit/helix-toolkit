// <copyright file="CrossSectionMeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>


namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using global::SharpDX;
    using System.Windows;
    using Media = System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="CrossSectionMeshGeometryModel3D" />
    /// </summary>
    public class CrossSectionMeshGeometryModel3D : MeshGeometryModel3D
    {
        /// <summary>
        /// The PlaneToVector
        /// </summary>
        /// <param name="p">The <see cref="Plane"/></param>
        /// <returns>The <see cref="Vector4"/></returns>
        private static Vector4 PlaneToVector(Plane p)
        {
            return new Vector4(p.Normal, p.D);
        }
        #region Dependency Properties
        /// <summary>
        /// Defines the CrossSectionColorProperty
        /// </summary>
        public static DependencyProperty CrossSectionColorProperty = DependencyProperty.Register("CrossSectionColor", typeof(Media.Color), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(Media.Colors.Firebrick,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.SectionColor = ((Media.Color)e.NewValue).ToColor4();
           }));

        /// <summary>
        /// Gets or sets the CrossSectionColor
        /// </summary>
        public Media.Color CrossSectionColor
        {
            set
            {
                SetValue(CrossSectionColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(CrossSectionColorProperty);
            }
        }
        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane1Property = DependencyProperty.Register("EnablePlane1", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane1Enabled = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane1
        {
            set
            {
                SetValue(EnablePlane1Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane1Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane2Property = DependencyProperty.Register("EnablePlane2", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane2Enabled = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane2
        {
            set
            {
                SetValue(EnablePlane2Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane2Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane3Property = DependencyProperty.Register("EnablePlane3", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane3Enabled = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane3
        {
            set
            {
                SetValue(EnablePlane3Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane3Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane4Property = DependencyProperty.Register("EnablePlane4", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane4Enabled = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane4
        {
            set
            {
                SetValue(EnablePlane4Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane4Property);
            }
        }

        /// <summary>
        /// Defines the Plane1Property
        /// </summary>
        public static DependencyProperty Plane1Property = DependencyProperty.Register("Plane1", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane1Params = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane1
        /// </summary>
        public Plane Plane1
        {
            set
            {
                SetValue(Plane1Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane1Property);
            }
        }

        /// <summary>
        /// Defines the Plane2Property
        /// </summary>
        public static DependencyProperty Plane2Property = DependencyProperty.Register("Plane2", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane2Params = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane2
        /// </summary>
        public Plane Plane2
        {
            set
            {
                SetValue(Plane2Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane2Property);
            }
        }

        /// <summary>
        /// Defines the Plane3Property
        /// </summary>
        public static DependencyProperty Plane3Property = DependencyProperty.Register("Plane3", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane3Params = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane3
        /// </summary>
        public Plane Plane3
        {
            set
            {
                SetValue(Plane3Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane3Property);
            }
        }

        /// <summary>
        /// Defines the Plane4Property
        /// </summary>
        public static DependencyProperty Plane4Property = DependencyProperty.Register("Plane4", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).crossSectionCore.Plane4Params = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane4
        /// </summary>
        public Plane Plane4
        {
            set
            {
                SetValue(Plane4Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane4Property);
            }
        }
        #endregion

        #region Private Variables
        private ICrossSectionRenderParams crossSectionCore
        { get { return (ICrossSectionRenderParams)RenderCore; } }

        #endregion
        /// <summary>
        /// The SetRenderTechnique
        /// </summary>
        /// <param name="host">The <see cref="IRenderHost"/></param>
        /// <returns>The <see cref="RenderTechnique"/></returns>
        protected override IRenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.CrossSection];
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new CrossSectionMeshRenderCore();
        }
    }
}

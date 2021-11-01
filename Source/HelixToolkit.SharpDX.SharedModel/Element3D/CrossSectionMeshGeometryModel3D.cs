// <copyright file="CrossSectionMeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
// </copyright>


#if NETFX_CORE
using  Windows.UI.Xaml;
using Media = Windows.UI;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Media = Windows.UI;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Media = System.Windows.Media;
// using MediaColors = System.Windows.Media.Colors;

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    using global::SharpDX;


    /// <summary>
    /// Defines the <see cref="CrossSectionMeshGeometryModel3D" />
    /// </summary>
    public class CrossSectionMeshGeometryModel3D : MeshGeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Gets or sets the cutting operation.
        /// </summary>
        /// <value>
        /// The cutting operation.
        /// </value>
        public CuttingOperation CuttingOperation
        {
            get
            {
                return (CuttingOperation)GetValue(CuttingOperationProperty);
            }
            set
            {
                SetValue(CuttingOperationProperty, value);
            }
        }
        /// <summary>
        /// The cutting operation property
        /// </summary>
        public static readonly DependencyProperty CuttingOperationProperty =
            DependencyProperty.Register("CuttingOperation", typeof(CuttingOperation), typeof(CrossSectionMeshGeometryModel3D),
                new PropertyMetadata(CuttingOperation.Intersect, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CrossSectionMeshNode).CuttingOperation = (CuttingOperation)e.NewValue;
                }));


        /// <summary>
        /// Defines the CrossSectionColorProperty
        /// </summary>
        public static DependencyProperty CrossSectionColorProperty = DependencyProperty.Register("CrossSectionColor", typeof(Media.Color), typeof(CrossSectionMeshGeometryModel3D),
#if WINUI
                new PropertyMetadata(Microsoft.UI.Colors.Firebrick,
#else
                new PropertyMetadata(Media.Colors.Firebrick,
#endif          
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).CrossSectionColor = ((Media.Color)e.NewValue).ToColor4();
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
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane1 = (bool)e.NewValue;
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
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane2 = (bool)e.NewValue;
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
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane3 = (bool)e.NewValue;
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
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane4 = (bool)e.NewValue;
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
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane5Property = DependencyProperty.Register("EnablePlane5", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane5 = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane5
        {
            set
            {
                SetValue(EnablePlane5Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane5Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane6Property = DependencyProperty.Register("EnablePlane6", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane6 = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane6
        {
            set
            {
                SetValue(EnablePlane6Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane6Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane7Property = DependencyProperty.Register("EnablePlane7", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane7 = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane7
        {
            set
            {
                SetValue(EnablePlane7Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane7Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane8Property = DependencyProperty.Register("EnablePlane8", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(false,
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).EnablePlane8 = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane8
        {
            set
            {
                SetValue(EnablePlane8Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane8Property);
            }
        }

        /// <summary>
        /// Defines the Plane1Property
        /// </summary>
        public static DependencyProperty Plane1Property = DependencyProperty.Register("Plane1", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane1 = (Plane)e.NewValue;
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
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane2 = (Plane)e.NewValue;
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
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane3 = (Plane)e.NewValue;
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
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane4 = (Plane)e.NewValue;
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

        /// <summary>
        /// Defines the Plane5Property
        /// </summary>
        public static DependencyProperty Plane5Property = DependencyProperty.Register("Plane5", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane5 = (Plane)e.NewValue;
           }));

        /// <summary>
        /// Gets or sets the Plane5
        /// </summary>
        public Plane Plane5
        {
            set
            {
                SetValue(Plane5Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane5Property);
            }
        }

        /// <summary>
        /// Defines the Plane6Property
        /// </summary>
        public static DependencyProperty Plane6Property = DependencyProperty.Register("Plane6", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane6 = (Plane)e.NewValue;
           }));

        /// <summary>
        /// Gets or sets the Plane6
        /// </summary>
        public Plane Plane6
        {
            set
            {
                SetValue(Plane6Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane6Property);
            }
        }

        /// <summary>
        /// Defines the Plane7Property
        /// </summary>
        public static DependencyProperty Plane7Property = DependencyProperty.Register("Plane7", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane7 = (Plane)e.NewValue;
           }));

        /// <summary>
        /// Gets or sets the Plane7
        /// </summary>
        public Plane Plane7
        {
            set
            {
                SetValue(Plane7Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane7Property);
            }
        }

        /// <summary>
        /// Defines the Plane8Property
        /// </summary>
        public static DependencyProperty Plane8Property = DependencyProperty.Register("Plane8", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new PropertyMetadata(new Plane(),
           (d, e) =>
           {
               ((d as Element3DCore).SceneNode as CrossSectionMeshNode).Plane8 = (Plane)e.NewValue;
           }));

        /// <summary>
        /// Gets or sets the Plane8
        /// </summary>
        public Plane Plane8
        {
            set
            {
                SetValue(Plane8Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane8Property);
            }
        }
        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new CrossSectionMeshNode();
        }
    }
}

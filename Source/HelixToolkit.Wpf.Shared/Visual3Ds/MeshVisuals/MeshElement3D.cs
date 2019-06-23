// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a base class for elements that contain one GeometryModel3D and front and back Materials.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a base class for elements that contain one <see cref="GeometryModel3D"/> and front and back <see cref="Material"/>s.
    /// </summary>
    /// <remarks>
    /// Derived classes should override the Tessellate method to generate the geometry.
    /// </remarks>
    public abstract class MeshElement3D : ModelVisual3D, IEditableObject
    {
        /// <summary>
        /// Identifies the <see cref="BackMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackMaterialProperty = DependencyProperty.Register(
            "BackMaterial", typeof(Material), typeof(MeshElement3D), new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.LightBlue), MaterialChanged));

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(MeshElement3D), new UIPropertyMetadata(null, FillChanged));

        /// <summary>
        /// Identifies the <see cref="Material"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
            "Material",
            typeof(Material),
            typeof(MeshElement3D),
            new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.Blue), MaterialChanged));

        /// <summary>
        ///   The visibility property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(
            "Visible",
            typeof(bool),
            typeof(MeshElement3D),
            new UIPropertyMetadata(true, VisibleChanged));

        /// <summary>
        /// A flag that is set when the element is in editing mode (<see cref="IEditableObject"/>, <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> and <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/>).
        /// </summary>
        private bool isEditing;

        /// <summary>
        /// A flag that is set when the geometry is changed.
        /// </summary>
        private bool isGeometryChanged;

        /// <summary>
        /// A flag that is set when the material is changed.
        /// </summary>
        private bool isMaterialChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MeshElement3D" /> class.
        /// </summary>
        protected MeshElement3D()
        {
            this.Content = new GeometryModel3D();
            this.UpdateModel();
        }

        /// <summary>
        /// Gets or sets the back material.
        /// </summary>
        /// <value>The back material.</value>
        public Material BackMaterial
        {
            get
            {
                return (Material)this.GetValue(BackMaterialProperty);
            }

            set
            {
                this.SetValue(BackMaterialProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fill brush. This brush will be used for both the Material and BackMaterial.
        /// </summary>
        /// <value>The fill brush.</value>
        public Brush Fill
        {
            get
            {
                return (Brush)this.GetValue(FillProperty);
            }

            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material
        {
            get
            {
                return (Material)this.GetValue(MaterialProperty);
            }

            set
            {
                this.SetValue(MaterialProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MeshElement3D"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the element is visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            get
            {
                return (bool)this.GetValue(VisibleProperty);
            }

            set
            {
                this.SetValue(VisibleProperty, value);
            }
        }

        /// <summary>
        ///   Gets the geometry model.
        /// </summary>
        /// <value>The geometry model.</value>
        public GeometryModel3D Model
        {
            get
            {
                return this.Content as GeometryModel3D;
            }
        }

        /// <summary>
        /// Begins an edit on the object.
        /// </summary>
        public void BeginEdit()
        {
            this.isEditing = true;
            this.isGeometryChanged = false;
            this.isMaterialChanged = false;
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            this.isEditing = false;
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            this.isEditing = false;
            if (this.isGeometryChanged)
            {
                this.OnGeometryChanged();
            }

            if (this.isMaterialChanged)
            {
                this.OnMaterialChanged();
            }
        }

        /// <summary>
        /// Forces an update of the geometry and materials.
        /// </summary>
        public void UpdateModel()
        {
            this.OnGeometryChanged();
            this.OnMaterialChanged();
        }

        /// <summary>
        /// The visible flag changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D)d).OnGeometryChanged();
        }        
        
        /// <summary>
        /// The geometry was changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D)d).OnGeometryChanged();
        }

        /// <summary>
        /// The Material or BackMaterial property was changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D)d).OnMaterialChanged();
        }

        /// <summary>
        /// The Fill property was changed.
        /// </summary>
        protected virtual void OnFillChanged()
        {
            this.Material = MaterialHelper.CreateMaterial(this.Fill);
            this.BackMaterial = this.Material;
        }

        /// <summary>
        /// Handles changes in geometry or visible state.
        /// </summary>
        protected virtual void OnGeometryChanged()
        {
            if (!this.isEditing)
            {
                this.Model.Geometry = this.Visible ? this.Tessellate() : null;
            }
            else
            {
                // flag the geometry as changed, the geometry will be updated when the <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/> is called.
                this.isGeometryChanged = true;
            }
        }

        /// <summary>
        /// Handles changes in material/back material.
        /// </summary>
        protected virtual void OnMaterialChanged()
        {
            if (!this.isEditing)
            {
                this.Model.Material = this.Material;
                this.Model.BackMaterial = this.BackMaterial;
            }
            else
            {
                this.isMaterialChanged = true;
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry.
        /// </returns>
        protected abstract MeshGeometry3D Tessellate();

        /// <summary>
        /// Called when Fill is changed.
        /// </summary>
        /// <param name="d">
        /// The mesh element.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D)d).OnFillChanged();
        }
    }
}
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using global::SharpDX;
using System.Runtime.Serialization;
using System.Collections.Generic;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{

    using Core;
    using Model;


#if !NETFX_CORE && !NET5_0
    [Serializable]
#endif
    [DataContract]
    public abstract class Geometry3D : ObservableObject, IGUID
    {
        public const string VertexBuffer = "VertexBuffer";
        public const string TriangleBuffer = "TriangleBuffer";
        [DataMember]
        public Guid GUID { set; get; } = Guid.NewGuid();

        private IntCollection indices = null;

        /// <summary>
        /// Indices, can be triangle list, line list, etc.
        /// </summary>
        [DataMember]
        public IntCollection Indices
        {
            get
            {
                return indices;
            }
            set
            {
                if (Set(ref indices, value))
                {
                    ClearOctree();
                }
            }
        }

        private Vector3Collection position = null;

        /// <summary>
        /// Vertex Positions
        /// </summary>
        [DataMember]
        public Vector3Collection Positions
        {
            get
            {
                return position;
            }
            set
            {
                if(position == value) { return; }
                position = value;
                ClearOctree();
                RaisePropertyChanged();
                UpdateBounds();             
            }
        }

#if !NETFX_CORE && !NET5_0
        [NonSerialized]
#endif
        private BoundingBox bound;
        /// <summary>
        /// Geometry AABB
        /// </summary>       
        [IgnoreDataMember]
        public BoundingBox Bound
        {
            set
            {
                Set(ref bound, value);
            }
            get
            {
                return bound;
            }
        }

#if !NETFX_CORE && !NET5_0
        [NonSerialized]
#endif
        private BoundingSphere boundingSphere;
        /// <summary>
        /// Geometry Bounding Sphere
        /// </summary>
        [IgnoreDataMember]
        public BoundingSphere BoundingSphere
        {
            set
            {
                Set(ref boundingSphere, value);
            }
            get
            {
                return boundingSphere;
            }
        }

        private Color4Collection colors = null;
        /// <summary>
        /// Vertex Color
        /// </summary>
        [DataMember]
        public Color4Collection Colors
        {
            get
            {
                return colors;
            }
            set
            {
                Set(ref colors, value);
            }
        }

        /// <summary>
        /// TO use Octree during hit test to improve hit performance, please call UpdateOctree after model created.
        /// </summary>
        public IOctreeBasic Octree { private set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [octree dirty], needs update.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [octree dirty]; otherwise, <c>false</c>.
        /// </value>
        public bool OctreeDirty { get; private set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dynamic. Must be set before passing to GeometryModel3D.
        /// <para>When set to true, the internal vertex/index buffer will be created using dynamic buffer.</para> 
        /// <para>Default is false, which is using immutable.</para>
        /// <para>Dynamic buffer is useful if user streaming similar sizes of Vertices/Indices into this geometry, this will avoid unnecessary buffer creation and reuse the existing dynamic buffer if the max size less than the size of existing buffer.</para>
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dynamic; otherwise, <c>false</c>.
        /// </value>
        public bool IsDynamic { set; get; } = false;
        /// <summary>
        /// The pre defined vertex count. Only used when <see cref="IsDynamic"/> = true.
        /// <para>The pre define vertex count allows user to initialize a dynamic buffer with a minimum pre-define size.</para>
        /// <para>Example: If the vertex count increments from 0 to around 3000 during vertex array streaming, 
        /// pre-define a size of 3000 for this geometry allows the dynamic buffer to be reused and avoid recreating dynamic buffer 3000 times.</para>
        /// </summary>
        public int PreDefinedVertexCount { set; get; } = 0;
        /// <summary>
        /// The pre defined index count. Used when <see cref="IsDynamic"/> = true.
        ///  <para>The pre define index count allows user to initialize a dynamic buffer with a minimum pre-define size.</para>
        /// <para>Example: If the index count increments from 0 to around 3000 during index array streaming, 
        /// pre-define a size of 3000 for this geometry allows the dynamic buffer to be reused and avoid recreating dynamic buffer 3000 times.</para>
        /// </summary>
        public int PreDefinedIndexCount { set; get; } = 0;
        /// <summary>
        /// Gets a value indicating whether the geometry data are transient. Call <see cref="SetAsTransient"/> to set this flag to true.
        /// <para>
        /// When this is true, geometry3D data will be cleared once being loaded into GPU.
        /// </para>
        /// <para>
        /// This geometry3D can only be used by one Model3D in one Viewport.
        /// Must not be shared.
        /// Hit test is disabled as well.
        /// </para>
        /// <para>
        /// Useful when loading a large geometry for view only and free up memory after geometry data being uploaded to GPU.
        /// </para>
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        public bool IsTransient
        {
            private set; get;
        } = false;
        /// <summary>
        /// The disable update bound, only used in <see cref="AssignTo(Geometry3D)"/>
        /// </summary>
        protected bool DisableUpdateBound = false;

        private readonly object octreeLock = new object();
        /// <summary>
        /// Gets or sets the octree parameter.
        /// </summary>
        /// <value>
        /// The octree parameter.
        /// </value>
        public OctreeBuildParameter OctreeParameter { private set; get; } = new OctreeBuildParameter();
        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry3D"/> class.
        /// </summary>
        public Geometry3D()
        {
            OctreeParameter.PropertyChanged += OctreeParameter_PropertyChanged;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry3D"/> class.
        /// </summary>
        /// <param name="isDynamic">if set to <c>true</c> [is dynamic].</param>
        public Geometry3D(bool isDynamic) 
            : this()
        {
            IsDynamic = isDynamic;
        }

        private void OctreeParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OctreeDirty = true;
        }

        /// <summary>
        /// Call to manually update vertex buffer. Use with <see cref="ObservableObject.DisablePropertyChangedEvent"/>
        /// <para>This is useful if user want to reuse existing <see cref="Positions"/> list and update vertex value inside the list.</para>
        /// <para>Note: For performance purpose, this will not cause bounding box update. 
        /// User must manually call <see cref="UpdateBounds"/> to refresh geometry bounding box.</para>
        /// </summary>
        public void UpdateVertices()
        {
            RaisePropertyChanged(VertexBuffer);
        }
        /// <summary>
        /// Call to manually update triangle buffer.
        /// <para>This is useful if user want to reuse existing <see cref="Indices"/> object and update index value inside the list.</para>
        /// </summary>
        public void UpdateTriangles()
        {
            RaisePropertyChanged(TriangleBuffer);
        }

        /// <summary>
        /// Call to manually update vertex color buffer.
        /// <para>This is useful if user want to reuse existing <see cref="Colors"/> object and update color value inside the list.</para>
        /// <para>Make sure the <see cref="Colors"/> count equal to the <see cref="Positions"/> count</para>
        /// </summary>
        public void UpdateColors()
        {
            RaisePropertyChanged(nameof(Colors));
        }

        /// <summary>
        /// Create Octree for current model.
        /// </summary>
        public void UpdateOctree(bool force = false)
        {
            if (!IsTransient && CanCreateOctree())
            {
                if (OctreeDirty || force)
                {
                    lock (octreeLock)
                    {
                        if (OctreeDirty || force)
                        {
                            this.Octree = CreateOctree(this.OctreeParameter);              
                            this.Octree?.BuildTree();                                
                            OctreeDirty = false;   
                        }                 
                    }
                    RaisePropertyChanged(nameof(Octree));
                }
            }
            else
            {
                this.Octree = null;
                OctreeDirty = true;
            }
        }
        
        protected virtual bool CanCreateOctree()
        {
            return Positions != null && Indices != null && Positions.Count > 0 && Indices.Count > 0;
        }


        /// <summary>
        /// Override to create different octree in subclasses.
        /// </summary>
        /// <returns></returns>
        protected virtual IOctreeBasic CreateOctree(OctreeBuildParameter parameter)
        {
            return null;
        }


        /// <summary>
        /// Set octree to null
        /// </summary>
        public void ClearOctree()
        {
            Octree = null;
            OctreeDirty = true;
        }
        /// <summary>
        /// Manuals the set octree.
        /// </summary>
        /// <param name="octree">The octree.</param>
        public void ManualSetOctree(IOctreeBasic octree)
        {
            Octree = octree;
            OctreeDirty = false;
        }

        /// <summary>
        /// Assigns internal properties to another geometry3D. This does not assign <see cref="IsDynamic"/>/<see cref="PreDefinedIndexCount"/>/<see cref="PreDefinedVertexCount"/>
        /// <para>
        /// Following properties are assigned:
        /// <see cref="Positions"/>, <see cref="Indices"/>, <see cref="Colors"/>, <see cref="Bound"/>, <see cref="BoundingSphere"/>, <see cref="Octree"/>, <see cref="OctreeParameter"/>
        /// </para>
        /// <para>Override <see cref="OnAssignTo(Geometry3D)"/> to assign custom properties in child class</para>
        /// </summary>
        /// <param name="target">The target.</param>
        public void AssignTo(Geometry3D target)
        {
            target.DisableUpdateBound = true;
            target.Positions = this.Positions;
            target.ClearOctree();
            target.DisableUpdateBound = false;
            target.Indices = this.Indices;
            target.Colors = this.Colors;
            target.Bound = this.Bound;
            target.BoundingSphere = this.BoundingSphere;
            target.OctreeParameter.MinimumOctantSize = OctreeParameter.MinimumOctantSize;
            target.OctreeParameter.MinObjectSizeToSplit = OctreeParameter.MinObjectSizeToSplit;
            target.OctreeParameter.Cubify = OctreeParameter.Cubify;
            target.OctreeParameter.EnableParallelBuild = OctreeParameter.EnableParallelBuild;
            if (Octree != null)
            {
                target.ManualSetOctree(Octree);
            }
            OnAssignTo(target);
        }

        protected virtual void OnAssignTo(Geometry3D target)
        {

        }
        /// <summary>
        /// Manually call this function to update AABB and Bounding Sphere
        /// </summary>
        public virtual void UpdateBounds()
        {
            if (DisableUpdateBound)
            {
                return;
            }
            else if (position == null || position.Count == 0)
            {
                Bound = new BoundingBox();
                BoundingSphere = new BoundingSphere();
            }
            else
            {
                Bound = BoundingBoxExtensions.FromPoints(Positions);
                BoundingSphere = BoundingSphereExtensions.FromPoints(Positions);
            }
            if(Bound.Maximum.IsUndefined() || Bound.Minimum.IsUndefined() || BoundingSphere.Center.IsUndefined()
                || float.IsInfinity(Bound.Center.X) || float.IsInfinity(Bound.Center.Y) || float.IsInfinity(Bound.Center.Z))
            {
                throw new Exception("Position vertex contains invalid value(Example: Float.NaN, Float.Infinity).");
            }
        }

        public struct Triangle
        {
            public Vector3 P0, P1, P2;
        }

        public struct Line
        {
            public Vector3 P0, P1;
        }

        public struct Point
        {
            public Vector3 P0;
        }

        /// <summary>
        /// Sets this geometry as transient.
        /// <para>
        /// Once this is called, this geometry will be marked as <see cref="IsTransient"/> = true.
        /// </para>
        /// <para>
        /// This function must be called before geometry is attached to a model for rendering. 
        /// Or before the model is attached to a viewport for rendering.
        /// </para>
        /// <para>
        /// A transient geometry is being used to save memory. All geometry data will be cleared once being uploaded into GPU.
        /// Should not be shared with multiple models.
        /// </para>
        /// A transient geometry does not support hit test.
        /// </summary>
        public void SetAsTransient()
        {
            IsTransient = true;
            ClearOctree();
        }
        /// <summary>
        /// Clears all geometry data.
        /// </summary>
        public void ClearAllGeometryData()
        {
            Positions?.Clear();
            Positions?.TrimExcess();
            Indices?.Clear();
            Indices?.TrimExcess();
            Colors?.Clear();
            Colors?.TrimExcess();
            OnClearAllGeometryData();
        }

        protected virtual void OnClearAllGeometryData()
        {
        
        }
    }
}

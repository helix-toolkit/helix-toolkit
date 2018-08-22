/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System;
using System.Numerics;
using System.Runtime.Serialization;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using Model;


#if !NETFX_CORE
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
                UpdateBounds();
                RaisePropertyChanged();
            }
        }

#if !NETFX_CORE
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

#if !NETFX_CORE
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
        public int PreDefinedVertexCount = 0;
        /// <summary>
        /// The pre defined index count. Used when <see cref="IsDynamic"/> = true.
        ///  <para>The pre define index count allows user to initialize a dynamic buffer with a minimum pre-define size.</para>
        /// <para>Example: If the index count increments from 0 to around 3000 during index array streaming, 
        /// pre-define a size of 3000 for this geometry allows the dynamic buffer to be reused and avoid recreating dynamic buffer 3000 times.</para>
        /// </summary>
        public int PreDefinedIndexCount = 0;
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

        public Geometry3D()
        {
            OctreeParameter.PropertyChanged += OctreeParameter_PropertyChanged;
        }

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
        /// </summary>
        public void UpdateVertices()
        {
            RaisePropertyChanged(VertexBuffer);
        }
        /// <summary>
        /// Call to manually update triangle buffer. Use with <see cref="ObservableObject.DisablePropertyChangedEvent"/>
        /// </summary>
        public void UpdateTriangles()
        {
            RaisePropertyChanged(TriangleBuffer);
        }

        /// <summary>
        /// Create Octree for current model.
        /// </summary>
        public void UpdateOctree(bool force = false)
        {
            if (CanCreateOctree())
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
            target.ManualSetOctree(Octree);
            target.OctreeParameter.MinimumOctantSize = OctreeParameter.MinimumOctantSize;
            target.OctreeParameter.MinObjectSizeToSplit = OctreeParameter.MinObjectSizeToSplit;
            target.OctreeParameter.Cubify = OctreeParameter.Cubify;
            target.OctreeParameter.EnableParallelBuild = OctreeParameter.EnableParallelBuild;
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
    }
}

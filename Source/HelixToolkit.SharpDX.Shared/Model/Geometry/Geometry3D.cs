// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;
    using global::SharpDX;
    using Core;
    using Model;

#if !NETFX_CORE
    [Serializable]
#endif
    public abstract class Geometry3D : ObservableObject, IGUID
    {
        public const string VertexBuffer = "VertexBuffer";
        public const string TriangleBuffer = "TriangleBuffer";

        private readonly Guid guid = Guid.NewGuid();
        public Guid GUID { get { return guid; } }

        private IntCollection indices = null;
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
                    Octree = null;
                }
            }
        }

        private Vector3Collection position = null;
        public Vector3Collection Positions
        {
            get
            {
                return position;
            }
            set
            {
                if (Set(ref position, value))
                {                 
                    Octree = null;
                    UpdateBounds();
                }
            }
        }

        private BoundingBox bound;
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

        private BoundingSphere boundingSphere;
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
        public Color4Collection Colors
        {
            get
            {
                return colors;
            }
            set
            {
                Set<Color4Collection>(ref colors, value);
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
        /// TO use Octree during hit test to improve hit performance, please call UpdateOctree after model created.
        /// </summary>
        public IOctree<GeometryModel3D> Octree { private set; get; }

        public OctreeBuildParameter OctreeParameter { private set; get; } = new OctreeBuildParameter();
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
        public void UpdateOctree(float minSize = 1f, bool autoDeleteIfEmpty = true)
        {
            if (CanCreateOctree())
            {
                OctreeParameter.MinimumOctantSize = minSize;
                OctreeParameter.AutoDeleteIfEmpty = autoDeleteIfEmpty;
                this.Octree = CreateOctree(this.OctreeParameter);
                this.Octree?.BuildTree();
            }
            else
            {
                this.Octree = null;
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
        protected virtual IOctree<GeometryModel3D> CreateOctree(OctreeBuildParameter parameter)
        {
            return null;
        }


        /// <summary>
        /// Set octree to null
        /// </summary>
        public void ClearOctree()
        {
            Octree = null;
        }

        public void UpdateBounds()
        {
            if (position == null || position.Count == 0)
            {
                Bound = new BoundingBox();
                BoundingSphere = new BoundingSphere();
            }
            else
            {
                Bound = BoundingBoxExtensions.FromPoints(Positions);
                BoundingSphere = BoundingSphereExtensions.FromPoints(Positions);
            }
            if(Bound.Maximum.IsUndefined() || Bound.Minimum.IsUndefined() || BoundingSphere.Center.IsUndefined())
            {
                throw new Exception("Position vertex contains invalid value(Example: Float.NaN).");
            }
        }
    }
}

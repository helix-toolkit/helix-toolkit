// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using HelixToolkit.SharpDX.Shared.Model;
    using System.Diagnostics;

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
#if !NETFX_CORE
                    Octree = null;
#endif
                }
            }
        }

        //private int[] indicesArray = new int[0];
        ///// <summary>
        ///// Used to avoid excessive array copy
        ///// </summary>
        //public int[] IndicesArray
        //{
        //    get
        //    {
        //        if (indicesArray == null)
        //        {
        //            indicesArray = Indices != null ? Indices.ToArray() : new int[0];
        //        }
        //        return indicesArray;
        //    }
        //}

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
#if !NETFX_CORE
                    Octree = null;
                    UpdateBounds();
#endif
                }
            }
        }

        //private Vector3[] positionArray = new Vector3[0];
        ///// <summary>
        ///// Used to avoid excessive array copy
        ///// </summary>
        //public Vector3[] PositionArray
        //{
        //    get
        //    {
        //        if (positionArray == null)
        //        {
        //            positionArray = Positions != null ? Positions.ToArray() : new Vector3[0];
        //        }
        //        return positionArray;
        //    }
        //}

#if !NETFX_CORE
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
#endif
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

#if !NETFX_CORE
        /// <summary>
        /// TO use Octree during hit test to improve hit performance, please call UpdateOctree after model created.
        /// </summary>
        public IOctree Octree { private set; get; }

        public OctreeBuildParameter OctreeParameter { private set; get; } = new OctreeBuildParameter();
#endif

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

#if !NETFX_CORE
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
        protected virtual IOctree CreateOctree(OctreeBuildParameter parameter)
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
        }
#endif
    }
}

/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    using Model.Scene;
    using System;
    using BoundingSphere = global::SharpDX.BoundingSphere;

    public class GeometryBoundManager : DisposeObject, IBoundable
    {
        #region Properties
        private Geometry3D geometry = null;
        /// <summary>
        /// 
        /// </summary>
        public Geometry3D Geometry
        {
            set
            {
                var old = geometry;
                if(Set(ref geometry, value))
                {
                    if (geometry != null && geometry.Bound.Maximum == Vector3.Zero && geometry.Bound.Minimum == Vector3.Zero)
                    {
                        geometry.UpdateBounds();
                    }
                    if (old != null)
                    {
                        old.PropertyChanged -= OnGeometryPropertyChangedPrivate;
                    }
                    if (geometry != null)
                    {
                        geometry.PropertyChanged += OnGeometryPropertyChangedPrivate;
                    }
                    UpdateBounds();
                }
            }
            get { return geometry; }
        }

        private IList<Matrix> instances = null;
        public IList<Matrix> Instances
        {
            set
            {
                if(Set(ref instances, value))
                {
                    UpdateBounds();
                }
            }
            get
            {
                return instances;
            }
        }

        public bool HasInstances { get { return instances != null && instances.Count > 0; } }

        public bool GeometryValid { private set; get; } = false;
        #region Bounds
        public static readonly BoundingBox DefaultBound = new BoundingBox();
        public static readonly BoundingSphere DefaultBoundSphere = new BoundingSphere();
        /// <summary>
        /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
        /// </summary>
        /// <value>
        /// The original bound.
        /// </value>
        public BoundingBox OriginalBounds
        {
            get { return Geometry == null ? DefaultBound : geometry.Bound; }
        }
        /// <summary>
        /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
        /// </summary>
        /// <value>
        /// The original bound sphere.
        /// </value>
        public BoundingSphere OriginalBoundsSphere
        {
            get { return Geometry == null ? DefaultBoundSphere : geometry.BoundingSphere; }
        }

        private BoundingBox bounds = DefaultBound;
        /// <summary>
        /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public BoundingBox Bounds
        {
            get { return bounds; }
            protected set
            {
                if (bounds != value)
                {
                    var old = bounds;
                    bounds = value;
                    RaiseOnBoundChanged(value, old);
                }
            }
        }

        private BoundingBox boundsWithTransform = DefaultBound;
        /// <summary>
        /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public BoundingBox BoundsWithTransform
        {
            get { return boundsWithTransform; }
            private set
            {
                if (boundsWithTransform != value)
                {
                    var old = boundsWithTransform;
                    boundsWithTransform = value;
                    RaiseOnTransformBoundChanged(value, old);
                }
            }
        }

        private BoundingSphere boundsSphere = DefaultBoundSphere;
        /// <summary>
        /// Gets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public BoundingSphere BoundsSphere
        {
            protected set
            {
                if (boundsSphere != value)
                {
                    var old = boundsSphere;
                    boundsSphere = value;
                    RaiseOnBoundSphereChanged(value, old);
                }
            }
            get
            {
                return boundsSphere;
            }
        }

        private BoundingSphere boundsSphereWithTransform = DefaultBoundSphere;
        /// <summary>
        /// Gets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public BoundingSphere BoundsSphereWithTransform
        {
            private set
            {
                if (boundsSphereWithTransform != value)
                {
                    var old = boundsSphereWithTransform;
                    boundsSphereWithTransform = value;
                    RaiseOnTransformBoundSphereChanged(value, old);
                }
            }
            get
            {
                return boundsSphereWithTransform;
            }
        }

        public bool HasBound { set; get; } = true;
        #endregion
        #endregion
        #region Events and Delegates
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;

        public event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;

        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnBoundSphereChanged;

        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnTransformBoundSphereChanged;

        private void RaiseOnTransformBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnTransformBoundChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingBox>(ref newBound, ref oldBound));
        }

        private void RaiseOnBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnBoundChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingBox>(ref newBound, ref oldBound));
        }


        private void RaiseOnTransformBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnTransformBoundSphereChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingSphere>(ref newBoundSphere, ref oldBoundSphere));
        }


        private void RaiseOnBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnBoundSphereChanged?.Invoke(elementCore, new BoundChangeArgs<BoundingSphere>(ref newBoundSphere, ref oldBoundSphere));
        }
        #endregion
        public delegate bool OnCheckGeometryDelegate(Geometry3D geometry);
        public OnCheckGeometryDelegate OnCheckGeometry; 

        private GeometryNode elementCore;

        public GeometryBoundManager(GeometryNode core)
        {
            this.elementCore = core;
            core.OnTransformChanged += OnTransformChanged;
        }

        private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Geometry3D.Positions)))
            {
                UpdateBounds();
            }
            else if (e.PropertyName.Equals(nameof(Geometry3D.Bound)))
            {
                Bounds = Geometry.Bound;
                BoundsWithTransform = Bounds.Transform(elementCore.ModelMatrix);
            }
            else if (e.PropertyName.Equals(nameof(Geometry3D.BoundingSphere)))
            {
                BoundsSphere = Geometry.BoundingSphere;
                BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(elementCore.ModelMatrix);
            }
            if (GeometryValid)
            {
                OnGeometryPropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// <para>Check geometry validity.</para>
        /// Return false if (this.geometryInternal == null || this.geometryInternal.Positions == null || this.geometryInternal.Positions.Count == 0 || this.geometryInternal.Indices == null || this.geometryInternal.Indices.Count == 0)
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual bool CheckGeometry()
        {
            return !(this.Geometry == null || this.Geometry.Positions == null || this.Geometry.Positions.Count == 0);
        }

        protected virtual void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void OnTransformChanged(object sender, TransformArgs e)
        {
            BoundsWithTransform = Bounds.Transform(e);
            BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(e);
        }

        protected void UpdateBounds()
        {
            GeometryValid = OnCheckGeometry != null ? OnCheckGeometry.Invoke(this.geometry) : CheckGeometry();
            if (!GeometryValid)
            {
                Bounds = DefaultBound;
                BoundsSphere = DefaultBoundSphere;
            }
            else
            {
                if (!HasInstances)
                {
                    Bounds = Geometry.Bound;
                    BoundsSphere = Geometry.BoundingSphere;
                    BoundsWithTransform = Bounds.Transform(elementCore.ModelMatrix);
                    BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(elementCore.ModelMatrix);
                }
                else
                {
                    var bound = Geometry.Bound.Transform(Instances[0]);
                    var boundSphere = Geometry.BoundingSphere.TransformBoundingSphere(Instances[0]);
                    if(Instances.Count > 50)
                    {
                        Parallel.Invoke(() => 
                        {
                            foreach (var instance in Instances)
                            {
                                var b = Geometry.Bound.Transform(instance);
                                BoundingBox.Merge(ref bound, ref b, out bound);
                            }
                        },
                        ()=> 
                        {
                            foreach (var instance in Instances)
                            {
                                var bs = Geometry.BoundingSphere.TransformBoundingSphere(instance);
                                BoundingSphere.Merge(ref boundSphere, ref bs, out boundSphere);
                            }
                        });
                    }
                    else
                    {
                        foreach (var instance in Instances)
                        {
                            var b = Geometry.Bound.Transform(instance);
                            BoundingBox.Merge(ref bound, ref b, out bound);
                            var bs = Geometry.BoundingSphere.TransformBoundingSphere(instance);
                            BoundingSphere.Merge(ref boundSphere, ref bs, out boundSphere);
                        }
                    }
                    Bounds = bound;
                    BoundsSphere = boundSphere;
                    BoundsWithTransform = Bounds.Transform(elementCore.ModelMatrix);
                    BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(elementCore.ModelMatrix);
                }
            }
        }

        public override void DisposeAndClear()
        {
            Geometry = null;
            base.DisposeAndClear();
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            elementCore.OnTransformChanged -= OnTransformChanged;
            OnBoundChanged = null;
            OnTransformBoundChanged = null;
            OnBoundSphereChanged = null;
            OnTransformBoundSphereChanged = null;
            base.OnDispose(disposeManagedResources);
        }
    }
}

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
    using Model;
    using BoundingSphere = global::SharpDX.BoundingSphere;

    public class GeometryBoundManager : ObservableObject, IBoundable
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
        public static readonly BoundingBox MaxBound = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MaxValue));
        public static readonly BoundingSphere MaxBoundSphere = new BoundingSphere(Vector3.Zero, float.MaxValue);

        private BoundingBox bounds = MaxBound;
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

        private BoundingBox boundsWithTransform = MaxBound;
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

        private BoundingSphere boundsSphere = MaxBoundSphere;
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

        private BoundingSphere boundsSphereWithTransform = MaxBoundSphere;
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
        #endregion
        #endregion
        #region Events and Delegates
        public delegate void BoundChangedEventHandler(object sender, ref BoundingBox newBound, ref BoundingBox oldBound);

        public event BoundChangedEventHandler OnBoundChanged;

        public event BoundChangedEventHandler OnTransformBoundChanged;

        public delegate void BoundSphereChangedEventHandler(object sender, ref BoundingSphere newBound, ref BoundingSphere oldBound);

        public event BoundSphereChangedEventHandler OnBoundSphereChanged;

        public event BoundSphereChangedEventHandler OnTransformBoundSphereChanged;

        private void RaiseOnTransformBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnTransformBoundChanged?.Invoke(this, ref newBound, ref oldBound);
        }

        private void RaiseOnBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnBoundChanged?.Invoke(this, ref newBound, ref oldBound);
        }


        private void RaiseOnTransformBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnTransformBoundSphereChanged?.Invoke(this, ref newBoundSphere, ref oldBoundSphere);
        }


        private void RaiseOnBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnBoundSphereChanged?.Invoke(this, ref newBoundSphere, ref oldBoundSphere);
        }
        #endregion

        public GeometryBoundManager(Element3DCore core)
        {
            core.OnTransformChanged += OnTransformChanged;
        }

        private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Geometry3D.Positions)))
            {
                UpdateBounds();
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
            return !(this.Geometry == null || this.Geometry.Positions == null || this.Geometry.Positions.Count == 0
                || this.Geometry.Indices == null || this.Geometry.Indices.Count == 0);
        }

        protected virtual void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void OnTransformChanged(object sender, Matrix e)
        {
            BoundsWithTransform = Bounds.Transform(e);
            BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(e);
        }

        protected void UpdateBounds()
        {
            GeometryValid = CheckGeometry();
            if (!GeometryValid)
            {
                Bounds = MaxBound;
                BoundsSphere = MaxBoundSphere;
            }
            else
            {
                if (!HasInstances)
                {
                    Bounds = Geometry.Bound;
                    BoundsSphere = Geometry.BoundingSphere;
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
                }
            }
        }
    }
}

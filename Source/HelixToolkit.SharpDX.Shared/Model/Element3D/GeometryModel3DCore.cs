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
    using BoundingSphere = global::SharpDX.BoundingSphere;

    public abstract class GeometryModel3DCore : Element3DCore, IBoundable
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

        private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
        {
            GeometryValid = CheckGeometry();
            if (this.IsAttached)
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

        protected override void OnTransformChanged(ref Matrix totalTransform)
        {
            base.OnTransformChanged(ref totalTransform);
            BoundsWithTransform = Bounds.Transform(totalTransform);
            BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(totalTransform);
        }

        protected void UpdateBounds()
        {
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

        protected override bool DetermineVisibility(IRenderContext context)
        {
            if (base.DetermineVisibility(context) && GeometryValid)
            {
                return !context.EnableBoundingFrustum || (context.BoundingFrustum.Intersects(ref boundsWithTransform) && context.BoundingFrustum.Intersects(ref boundsSphereWithTransform));
            }
            else { return false; }
        }

        public override bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits, IRenderable originalSource)
        {
            if (CanHit(context) && GeometryValid)
            {
                if (HasInstances)
                {
                    bool hit = false;
                    int idx = 0;
                    foreach (var m in instances)
                    {
                        var b = this.Bounds;
                        if (OnHitTest(context, this.TotalModelMatrix * m, ref ray, ref hits, originalSource))
                        {
                            hit = true;
                            var lastHit = hits[hits.Count - 1];
                            lastHit.Tag = idx;
                            hits[hits.Count - 1] = lastHit;
                        }
                        ++idx;
                    }

                    return hit;
                }
                else
                {
                    return OnHitTest(context, this.TotalModelMatrix, ref ray, ref hits, originalSource);
                }
            }
            else
            {
                return false;
            }
        }
    }
}

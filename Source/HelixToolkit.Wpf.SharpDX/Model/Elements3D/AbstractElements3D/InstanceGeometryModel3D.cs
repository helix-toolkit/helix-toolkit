using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public abstract class InstanceGeometryModel3D : GeometryModel3D
    {
        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(InstanceGeometryModel3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, InstancesChanged));

        /// <summary>
        /// 
        /// </summary>
        private static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstanceGeometryModel3D)d;
            model.instanceInternal = e.NewValue == null ? null : e.NewValue as IList<Matrix>;
            model.InstancesChanged();
        }

        protected bool isInstanceChanged = true;
        protected bool hasInstances = false;
        public bool HasInstancing { get { return hasInstances; } }
        protected IList<Matrix> instanceInternal;
        protected EffectScalarVariable bHasInstances;
        private readonly DynamicBufferProxy<Matrix> instanceBuffer = new DynamicBufferProxy<Matrix>(Matrix.SizeInBytes, BindFlags.VertexBuffer);
        public DynamicBufferProxy<Matrix> InstanceBuffer { get { return instanceBuffer; } }

        protected BoundingBox instancesBound;
        public BoundingBox InstancesBound
        {
            protected set
            {
                instancesBound = value;
            }
            get
            {
                return instancesBound;
            }
        }

        protected virtual void InstancesChanged()
        {
            this.hasInstances = (this.instanceInternal != null) && (this.instanceInternal.Any());
            UpdateInstancesBounds();
            isInstanceChanged = true;
        }

        protected virtual void UpdateInstancesBounds()
        {
            if (!hasInstances)
            {
                InstancesBound = this.BoundsWithTransform;
            }
            else
            {
                var bound = BoundingBox.FromPoints(this.BoundsWithTransform.GetCorners().Select(x => Vector3.TransformCoordinate(x, instanceInternal[0])).ToArray());
                foreach (var instance in instanceInternal)
                {
                    var b = BoundingBox.FromPoints(this.BoundsWithTransform.GetCorners().Select(x => Vector3.TransformCoordinate(x, instance)).ToArray());
                    BoundingBox.Merge(ref bound, ref b, out bound);
                }
                InstancesBound = bound;
            }
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && geometryInternal != null && geometryInternal.Positions != null && geometryInternal.Positions.Count > 0;
        }
        /// <summary>
        /// 
        /// </summary>        
        public override bool HitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (CanHitTest(context))
            {
                if ((this.instanceInternal != null) && (this.instanceInternal.Any()))
                {
                    bool hit = false;
                    int idx = 0;
                    foreach (var modelMatrix in instanceInternal)
                    {
                        var b = this.Bounds;
                        this.PushMatrix(modelMatrix);
                        if (OnHitTest(context, rayWS, ref hits))
                        {
                            hit = true;
                            var lastHit = hits[hits.Count - 1];
                            lastHit.Tag = idx;
                            hits[hits.Count - 1] = lastHit;
                        }
                        this.PopMatrix();
                        ++idx;
                    }

                    return hit;
                }
                else
                {
                    return OnHitTest(context, rayWS, ref hits);
                }
            }
            else
            {
                return false;
            }
        }

        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return !hasInstances && base.CheckBoundingFrustum(ref boundingFrustum) || boundingFrustum.Intersects(ref instancesBound);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            // --- init instances buffer            
            bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            InstancesChanged();
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref this.bHasInstances);
            instanceBuffer.Dispose();
            base.OnDetach();
        }
    }
}

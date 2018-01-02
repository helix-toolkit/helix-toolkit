using System.Linq;
using global::SharpDX;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
    {
        #region DependencyProperties
        /// <summary>
        /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
        /// </summary>
        public static readonly DependencyProperty InstanceIdentifiersProperty = DependencyProperty.Register("InstanceIdentifiers", typeof(IList<System.Guid>),
            typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManager), typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as InstancingMeshGeometryModel3D;
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
            }));

        /// <summary>
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), 
                new AffectsRenderPropertyMetadata(null, InstancesParamChanged));

        /// <summary>
        /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
        /// </summary>        
        public IList<System.Guid> InstanceIdentifiers
        {
            set
            {
                SetValue(InstanceIdentifiersProperty, value);
            }
            get
            {
                return (IList<System.Guid>)GetValue(InstanceIdentifiersProperty);
            }
        }

        public IOctreeManager OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManager)GetValue(OctreeManagerProperty);
            }
        }

        /// <summary>
        /// List of instance parameters. 
        /// </summary>
        public IList<InstanceParameter> InstanceParamArray
        {
            get { return (IList<InstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }

        #endregion
        protected IElementsBufferModel<InstanceParameter> instanceParamBuffer = new InstanceParamsBufferModel<InstanceParameter>(InstanceParameter.SizeInBytes);

        private static void InstancesParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingMeshGeometryModel3D)d;
            model.instanceParamBuffer.Elements = e.NewValue as IList<InstanceParameter>;
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.InstancingBlinn];
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new InstancingMeshRenderCore() { ParameterBuffer = this.instanceParamBuffer };
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                instanceParamBuffer.Initialize();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            instanceParamBuffer.Dispose();
            base.OnDetach();
        }

        protected override void OnRender(IRenderContext context)
        {
            if (InstanceBuffer.Changed)
            {
                BuildOctree();
            }
            base.OnRender(context);
        }

        protected override void UpdateInstancesBounds()
        {
            OctreeManager?.Clear();
            base.UpdateInstancesBounds();
        }

        private void BuildOctree()
        {
            if (isHitTestVisibleInternal && InstanceBuffer.HasElements)
            {
                OctreeManager?.RebuildTree(new Element3D[] { this });
            }
            else
            {
                OctreeManager?.Clear();
            }
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context);
        }

        public override bool HitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (CanHitTest(context) && OctreeManager!=null && OctreeManager.Octree != null)
            {
                var boundHits = new List<HitTestResult>();             
                isHit = OctreeManager.Octree.HitTest(context, this, ModelMatrix, rayWS, ref boundHits);
                if (isHit)
                {
                    Matrix instanceMatrix;
                    foreach (var hit in boundHits)
                    {
                        int instanceIdx = (int)hit.Tag;
                        instanceMatrix = InstanceBuffer.Elements[instanceIdx];
                        this.PushMatrix(instanceMatrix);
                        var h = OnHitTest(context, rayWS, ref hits);
                        isHit |= h;
                        this.PopMatrix();
                        if (h && hits.Count > 0)
                        {
                            var result = hits.Last();
                            object tag = null;
                            if (InstanceIdentifiers != null && InstanceIdentifiers.Count == InstanceBuffer.Elements.Count)
                            {
                                tag = InstanceIdentifiers[instanceIdx];
                            }
                            else
                            {
                                tag = instanceIdx;
                            }
                            result.Tag = tag;
                            hits[hits.Count - 1] = result;
                        }
                    }
                }
            }
            else
            {
                base.HitTest(context, rayWS, ref hits);
            }
            return isHit;
        }
    }
}

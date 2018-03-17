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
            typeof(IOctreeManagerWrapper), typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (s, e) =>
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
                d.octreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

        /// <summary>
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), 
                new PropertyMetadata(null, InstancesParamChanged));

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

        public IOctreeManagerWrapper OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManagerWrapper)GetValue(OctreeManagerProperty);
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

        private bool isInstanceChanged = false;
        protected IOctreeManager octreeManager { private set; get; }

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

        protected override RenderCore OnCreateRenderCore()
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
            instanceParamBuffer.DisposeAndClear();
            base.OnDetach();
        }

        public override void UpdateNotRender()
        {
            base.UpdateNotRender();
            if (isInstanceChanged)
            {
                BuildOctree();
                isInstanceChanged = false;
            }
        }

        protected override void InstancesChanged()
        {
            base.InstancesChanged();
            octreeManager?.Clear();
            isInstanceChanged = true;
        }

        private void BuildOctree()
        {
            if (IsRenderable && InstanceBuffer.HasElements)
            {
                octreeManager?.RebuildTree(Enumerable.Repeat<IRenderable>(this, 1));
            }
            else
            {
                octreeManager?.Clear();
            }
        }

        public override bool HitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (CanHitTest(context))
            {               
                if (octreeManager != null && octreeManager.Octree != null)
                {
                    var boundHits = new List<HitTestResult>();             
                    isHit = octreeManager.Octree.HitTest(context, this, TotalModelMatrix, rayWS, ref boundHits);
                    if (isHit)
                    {
                        isHit = false;
                        Matrix instanceMatrix;
                        foreach (var hit in boundHits)
                        {
                            int instanceIdx = (int)hit.Tag;
                            instanceMatrix = InstanceBuffer.Elements[instanceIdx];
                            var h = base.OnHitTest(context, TotalModelMatrix * instanceMatrix, ref rayWS, ref hits);
                            isHit |= h;
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
                    isHit = base.HitTest(context, rayWS, ref hits);
                }
                
            }
            return isHit;
        }
    }
}

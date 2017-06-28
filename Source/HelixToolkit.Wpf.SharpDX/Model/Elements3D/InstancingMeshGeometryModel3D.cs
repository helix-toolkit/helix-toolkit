using System.Linq;
using global::SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Diagnostics;
using HelixToolkit.Wpf.SharpDX.Utilities;

namespace HelixToolkit.Wpf.SharpDX
{
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
        protected readonly DynamicBufferProxy<InstanceParameter> instanceParamBuffer = new DynamicBufferProxy<InstanceParameter>(InstanceParameter.SizeInBytes, BindFlags.VertexBuffer);
        private EffectScalarVariable hasInstanceParamVar;

        protected bool instanceParamArrayChanged { private set; get; } = true;
        protected bool hasInstanceParams { private set; get; } = false;

        public bool HasInstanceParams { get { return hasInstanceParams; } }

        private static void InstancesParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingMeshGeometryModel3D)d;
            model.InstancesParamChanged();
        }

        protected void InstancesParamChanged()
        {
            hasInstanceParams = (InstanceParamArray != null && InstanceParamArray.Any());
            instanceParamArrayChanged = true;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.InstancingBlinn];
        }

        protected override void OnRender(RenderContext renderContext)
        {
            this.bHasInstances.Set(this.hasInstances);
            this.hasInstanceParamVar.Set(this.hasInstanceParams);
            // --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));
            if (this.hasInstances)
            {
                // --- update instance buffer
                if (this.isInstanceChanged)
                {
                    BuildOctree();
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.instanceInternal);
                    this.isInstanceChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.InstanceBuffer.Buffer, this.InstanceBuffer.StructureSize, 0));
                if (this.hasInstanceParams)
                {
                    if (instanceParamArrayChanged)
                    {
                        instanceParamBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.InstanceParamArray);
                        this.instanceParamArrayChanged = false;
                    }
                    renderContext.DeviceContext.InputAssembler.SetVertexBuffers(2, new VertexBufferBinding(this.instanceParamBuffer.Buffer, this.instanceParamBuffer.StructureSize, 0));
                }

                OnInstancedDrawCall(renderContext);
            }
            this.bHasInstances.Set(false);
            this.hasInstanceParamVar.Set(false);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            InstancesParamChanged();
            hasInstanceParamVar = effect.GetVariableByName("bHasInstanceParams").AsScalar();
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            instanceParamBuffer.Dispose();
            Disposer.RemoveAndDispose(ref hasInstanceParamVar);
        }

        protected override void UpdateInstancesBounds()
        {
            OctreeManager?.Clear();
            base.UpdateInstancesBounds();
        }

        private void BuildOctree()
        {
            if (isHitTestVisibleInternal && hasInstances)
            {
                OctreeManager?.RebuildTree(new Element3D[] { this });
            }
            else
            {
                OctreeManager?.Clear();
            }
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && OctreeManager != null && OctreeManager.Octree != null;
        }

        public override bool HitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (CanHitTest(context))
            {
                var boundHits = new List<HitTestResult>();
                
                isHit = OctreeManager.Octree.HitTest(context, this, ModelMatrix, rayWS, ref boundHits);
                if (isHit)
                {
                    Matrix instanceMatrix;
                    foreach (var hit in boundHits)
                    {
                        int instanceIdx = (int)hit.Tag;
                        instanceMatrix = instanceInternal[instanceIdx];
                        this.PushMatrix(instanceMatrix);
                        var h = OnHitTest(context, rayWS, ref hits);
                        isHit |= h;
                        this.PopMatrix();
                        if (h && hits.Count > 0)
                        {
                            var result = hits.Last();
                            object tag = null;
                            if (InstanceIdentifiers != null && InstanceIdentifiers.Count == instanceInternal.Count)
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
            return isHit;
        }
    }
}

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
        protected readonly DynamicBufferProxy<InstanceParameter> instanceParamBuffer = new DynamicBufferProxy<InstanceParameter>(InstanceParameter.SizeInBytes, BindFlags.VertexBuffer);
        protected bool instanceParamArrayChanged = true;
        protected bool hasInstanceParams = false;
        private EffectScalarVariable hasInstanceParamVar;
        public bool HasInstanceParams { get { return hasInstanceParams; } }
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
        /// <summary>
        /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
        /// </summary>
        public static readonly DependencyProperty InstanceIdentifiersProperty = DependencyProperty.Register("InstanceIdentifiers", typeof(IList<System.Guid>),
            typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null));
        /// <summary>
        /// List of instance parameters. 
        /// </summary>
        public IList<InstanceParameter> InstanceParamArray
        {
            get { return (IList<InstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }
        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManager),
            typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (s, e) =>
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
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), 
                new AffectsRenderPropertyMetadata(null, InstancesParamChanged));


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

        //protected override bool CanRender(RenderContext context)
        //{
        //    return base.CanRender(context);
        //}

        //protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        //{
        //    if(hasAdvInstancing || hasInstances)
        //    {
        //        return boundingFrustum.Intersects(ref instancesBound);
        //    }
        //    return false;
        //}

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
            this.effectMaterial.AttachMaterial();
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
                // --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
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
            if (IsHitTestVisibleInternal && hasInstances)
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

//        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
//        {
//            var boundHits = new List<HitTestResult>();
//            bool isHit = false;
//            isHit = OctreeManager.Octree.HitTest(context, this, ModelMatrix, rayWS, ref boundHits);
//            if (isHit)
//            {
//                var g = this.geometryInternal as MeshGeometry3D;
//                isHit = false;
//                Matrix instanceMatrix;
//                if (g.Octree != null)
//                {
//                    foreach (var hit in boundHits)
//                    {
//                        int instanceIdx = (int)hit.Tag;
//                        instanceMatrix = instanceInternal[instanceIdx];
//                        this.PushMatrix(instanceMatrix);
//                        var h = g.Octree.HitTest(context, this, ModelMatrix, rayWS, ref hits);
//                        isHit |= h;
//                        this.PopMatrix();
//                        if (h && hits.Count > 0)
//                        {
//                            var result = hits[0];
//                            object tag = null;
//                            if (InstanceIdentifiers != null && InstanceIdentifiers.Count == instanceInternal.Count)
//                            {
//                                tag = InstanceIdentifiers[instanceIdx];
//                            }
//                            else
//                            {
//                                tag = instanceIdx;
//                            }
//                            hits[0] = new HitTestResult()
//                            {
//                                Distance = result.Distance,
//                                IsValid = result.IsValid,
//                                ModelHit = result.ModelHit,
//                                NormalAtHit = result.NormalAtHit,
//                                PointHit = result.PointHit,
//                                TriangleIndices = result.TriangleIndices,
//                                Tag = tag
//                            };
//                        }
//                    }
//                }
//                else
//                {
//                    var result = new HitTestResult();
//                    result.Distance = double.MaxValue;
//                    foreach (var hit in boundHits)
//                    {
//                        int instanceIdx = (int)hit.Tag;
//                        instanceMatrix = instanceInternal[instanceIdx];
//                        this.PushMatrix(instanceMatrix);

//                        var m = this.modelMatrix;

//                        // put bounds to world space

//                        int index = 0;
//                        foreach (var t in g.Triangles)
//                        {
//                            float d;
//                            var p0 = Vector3.TransformCoordinate(t.P0, m);
//                            var p1 = Vector3.TransformCoordinate(t.P1, m);
//                            var p2 = Vector3.TransformCoordinate(t.P2, m);
//                            if (Collision.RayIntersectsTriangle(ref rayWS, ref p0, ref p1, ref p2, out d))
//                            {
//                                if (d > 0 && d < result.Distance) // If d is NaN, the condition is false.
//                                {
//                                    result.IsValid = true;
//                                    result.ModelHit = this;
//                                    // transform hit-info to world space now:
//                                    result.PointHit = (rayWS.Position + (rayWS.Direction * d)).ToPoint3D();
//                                    result.Distance = d;
//                                    object tag = null;
//                                    if (InstanceIdentifiers != null && InstanceIdentifiers.Count == instanceInternal.Count)
//                                    {
//                                        tag = InstanceIdentifiers[instanceIdx];
//                                    }
//                                    else
//                                    {
//                                        tag = instanceIdx;
//                                    }
//                                    result.Tag = tag;
//                                    var n = Vector3.Cross(p1 - p0, p2 - p0);
//                                    n.Normalize();
//                                    // transform hit-info to world space now:
//                                    result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
//                                    result.TriangleIndices = new System.Tuple<int, int, int>(g.Indices[index], g.Indices[index + 1], g.Indices[index + 2]);
//                                    isHit = true;
//                                }
//                            }
//                            index += 3;
//                        }
//                        this.PopMatrix();
//                    }
//                    if (isHit)
//                    {
//                        hits.Add(result);
//                    }
//                }
//#if DEBUG
//                if (isHit)
//                    Debug.WriteLine("Hit: " + hits[0].Tag + "; HitPoint: " + hits[0].PointHit);
//#endif
//            }
//            return isHit;
//        }
    }
}

using System.Linq;
using global::SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Diagnostics;

namespace HelixToolkit.Wpf.SharpDX
{
    public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
    {
        protected Buffer instanceParamBuffer = null;
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
            DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), new UIPropertyMetadata(null, InstancesParamChanged));


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
            /// --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);
            this.effectMaterial.AttachMaterial();
            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));
            if (this.hasInstances)
            {
                /// --- update instance buffer
                if (this.isInstanceChanged)
                {
                    BuildOctree();
                    if (instanceBuffer == null || instanceBuffer.Description.SizeInBytes < Matrix.SizeInBytes * this.Instances.Count)
                    {
                        Disposer.RemoveAndDispose(ref instanceBuffer);
                        this.instanceBuffer = Buffer.Create(this.Device, this.Instances.ToArray(), new BufferDescription(Matrix.SizeInBytes * this.Instances.Count, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    }
                    else
                    {
                        DataStream stream;
                        Device.ImmediateContext.MapSubresource(this.instanceBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                        stream.Position = 0;
                        stream.WriteRange(this.Instances.ToArray(), 0, this.Instances.Count);
                        Device.ImmediateContext.UnmapSubresource(this.instanceBuffer, 0);
                        stream.Dispose();
                    }
                    this.isInstanceChanged = false;
                }
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0));
                if (this.hasInstanceParams)
                {
                    if (instanceParamArrayChanged)
                    {
                        if (instanceParamBuffer == null || this.instanceParamBuffer.Description.SizeInBytes < InstanceParameter.SizeInBytes * this.InstanceParamArray.Count)
                        {
                            Disposer.RemoveAndDispose(ref instanceParamBuffer);
                            this.instanceParamBuffer = Buffer.Create(this.Device, this.InstanceParamArray.ToArray(),
                                new BufferDescription(InstanceParameter.SizeInBytes * this.InstanceParamArray.Count, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                                CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                        }
                        else
                        {
                            DataStream stream;
                            Device.ImmediateContext.MapSubresource(this.instanceParamBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                            stream.Position = 0;
                            stream.WriteRange(this.InstanceParamArray.ToArray(), 0, this.InstanceParamArray.Count);
                            Device.ImmediateContext.UnmapSubresource(this.instanceParamBuffer, 0);
                            stream.Dispose();
                        }
                        this.instanceParamArrayChanged = false;
                    }
                    this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(2, new VertexBufferBinding(this.instanceParamBuffer, InstanceParameter.SizeInBytes, 0));
                }
                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
                /// --- draw
                this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.Instances.Count, 0, 0, 0);
            }
            this.bHasInstances.Set(false);
            this.hasInstanceParamVar.Set(false);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            instanceParamArrayChanged = true;
            hasInstanceParamVar = effect.GetVariableByName("bHasInstanceParams").AsScalar();
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            Disposer.RemoveAndDispose(ref instanceParamBuffer);
            Disposer.RemoveAndDispose(ref hasInstanceParamVar);
        }

        protected override void UpdateInstancesBounds()
        {
            OctreeManager?.Clear();
            base.UpdateInstancesBounds();
        }

        private void BuildOctree()
        {
            if (IsHitTestVisible && hasInstances)
            {
                OctreeManager?.RebuildTree(new Element3D[] { this });
            }
            else
            {
                OctreeManager?.Clear();
            }
        }

        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            if (!IsHitTestVisible || OctreeManager == null || OctreeManager.Octree == null)
            {
                return false;
            }
            else
            {
                var boundHits = new List<HitTestResult>();
                bool isHit = false;
                isHit = OctreeManager.Octree.HitTest(this, ModelMatrix, rayWS, ref boundHits);
                if (isHit)
                {
                    var g = this.Geometry as MeshGeometry3D;
                    isHit = false;
                    Matrix instanceMatrix;
                    if (g.Octree != null)
                    {
                        foreach (var hit in boundHits)
                        {
                            int instanceIdx = (int)hit.Tag;
                            instanceMatrix = Instances[instanceIdx];
                            this.PushMatrix(instanceMatrix);
                            var h = g.Octree.HitTest(this, ModelMatrix, rayWS, ref hits);
                            isHit |= h;
                            this.PopMatrix();
                            if (h && hits.Count > 0)
                            {
                                var result = hits[0];
                                object tag = null;
                                if (InstanceIdentifiers != null && InstanceIdentifiers.Count == Instances.Count)
                                {
                                    tag = InstanceIdentifiers[instanceIdx];
                                }
                                else
                                {
                                    tag = instanceIdx;
                                }
                                hits[0] = new HitTestResult()
                                {
                                    Distance = result.Distance,
                                    IsValid = result.IsValid,
                                    ModelHit = result.ModelHit,
                                    NormalAtHit = result.NormalAtHit,
                                    PointHit = result.PointHit,
                                    TriangleIndices = result.TriangleIndices,
                                    Tag = tag
                                };
                            }
                        }
                    }
                    else
                    {
                        var result = new HitTestResult();
                        result.Distance = double.MaxValue;
                        foreach (var hit in boundHits)
                        {
                            int instanceIdx = (int)hit.Tag;
                            instanceMatrix = Instances[instanceIdx];
                            this.PushMatrix(instanceMatrix);

                            var m = this.modelMatrix;

                            // put bounds to world space

                            int index = 0;
                            foreach (var t in g.Triangles)
                            {
                                float d;
                                var p0 = Vector3.TransformCoordinate(t.P0, m);
                                var p1 = Vector3.TransformCoordinate(t.P1, m);
                                var p2 = Vector3.TransformCoordinate(t.P2, m);
                                if (Collision.RayIntersectsTriangle(ref rayWS, ref p0, ref p1, ref p2, out d))
                                {
                                    if (d > 0 && d < result.Distance) // If d is NaN, the condition is false.
                                    {
                                        result.IsValid = true;
                                        result.ModelHit = this;
                                        // transform hit-info to world space now:
                                        result.PointHit = (rayWS.Position + (rayWS.Direction * d)).ToPoint3D();
                                        result.Distance = d;
                                        object tag = null;
                                        if (InstanceIdentifiers != null && InstanceIdentifiers.Count == Instances.Count)
                                        {
                                            tag = InstanceIdentifiers[instanceIdx];
                                        }
                                        else
                                        {
                                            tag = instanceIdx;
                                        }
                                        result.Tag = tag;
                                        var n = Vector3.Cross(p1 - p0, p2 - p0);
                                        n.Normalize();
                                        // transform hit-info to world space now:
                                        result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
                                        result.TriangleIndices = new System.Tuple<int, int, int>(g.Indices[index], g.Indices[index + 1], g.Indices[index + 2]);
                                        isHit = true;
                                    }
                                }
                                index += 3;
                            }
                            this.PopMatrix();
                        }
                        if (isHit)
                        {
                            hits.Add(result);
                        }
                    }
                }
#if DEBUG
                if (isHit)
                    Debug.WriteLine("Hit: " + hits[0].Tag + "; HitPoint: " + hits[0].PointHit);
#endif
                return isHit;
            }
        }
    }
}

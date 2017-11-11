// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using global::SharpDX;

    using global::SharpDX.Direct3D;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;
    using Utilities;

    public class MeshGeometryModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, RasterStateChanged));
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D), 
            new AffectsRenderPropertyMetadata(CullMode.None, RasterStateChanged));
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(MeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, RasterStateChanged));
        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(MeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(false, (d,e)=> { (d as MeshGeometryModel3D).invertNormal = (bool)e.NewValue; }));

        public bool FrontCounterClockwise
        {
            set
            {
                SetValue(FrontCounterClockwiseProperty, value);
            }
            get
            {
                return (bool)GetValue(FrontCounterClockwiseProperty);
            }
        }


        public CullMode CullMode
        {
            set
            {
                SetValue(CullModeProperty, value);
            }
            get
            {
                return (CullMode)GetValue(CullModeProperty);
            }
        }


        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }
        /// <summary>
        /// Invert the surface normal during rendering
        /// </summary>
        public bool InvertNormal
        {
            set
            {
                SetValue(InvertNormalProperty, value);
            }
            get
            {
                return (bool)GetValue(InvertNormalProperty);
            }
        }
        #endregion
        [ThreadStatic]
        private static DefaultVertex[] vertexArrayBuffer = null;
        private readonly ImmutableBufferProxy<DefaultVertex> vertexBuffer = new ImmutableBufferProxy<DefaultVertex>(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer);
        private readonly ImmutableBufferProxy<int> indexBuffer = new ImmutableBufferProxy<int>(sizeof(int), BindFlags.IndexBuffer);

        protected bool invertNormal { private set; get; } = false;

        protected EffectScalarVariable bInvertNormalVar;
        /// <summary>
        /// For subclass override
        /// </summary>
        public override IBufferProxy VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }
        /// <summary>
        /// For subclass override
        /// </summary>
        public override IBufferProxy IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
        }

        protected override RasterizerState CreateRasterState()
        {
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = +0,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCounterClockwise,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow? false : IsScissorEnabled,
            };
            return new RasterizerState(this.Device, rasterStateDesc);
        }

        protected override void OnCreateGeometryBuffers()
        {
            CreateVertexBuffer(CreateDefaultVertexArray);
            indexBuffer.CreateBufferFromDataArray(this.Device, geometryInternal.Indices);
        }

        protected override void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnGeometryPropertyChanged(sender, e);

            if (e.PropertyName.Equals(nameof(MeshGeometry3D.TextureCoordinates))
                || e.PropertyName.Equals(nameof(MeshGeometry3D.Positions))
                || e.PropertyName.Equals(nameof(MeshGeometry3D.Colors))
                || e.PropertyName.Equals(Geometry3D.VertexBuffer))
            {
                OnUpdateVertexBuffer(CreateDefaultVertexArray);
            }
            else if (e.PropertyName.Equals(nameof(MeshGeometry3D.Indices)) || e.PropertyName.Equals(Geometry3D.TriangleBuffer))
            {
                indexBuffer.CreateBufferFromDataArray(this.Device, this.geometryInternal.Indices);
                InvalidateRender();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }

            // --- scale texcoords
            var texScale = TextureCoodScale;

            // --- get geometry
            var geometry = this.geometryInternal as MeshGeometry3D;

            // -- set geometry if given
            if (geometry != null)
            {
                //throw new HelixToolkitException("Geometry not found!");                

                // --- init vertex buffer
                OnCreateGeometryBuffers();
            }
            else
            {
                throw new System.Exception("Geometry must not be null");
            }
            bInvertNormalVar = effect.GetVariableByName("bInvertNormal").AsScalar();
            // --- flush
            //this.Device.ImmediateContext.Flush();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnUpdateVertexBuffer(Func<DefaultVertex[]> updateFunction)
        {
            CreateVertexBuffer(updateFunction);
            InvalidateRender();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateVertexBuffer(Func<DefaultVertex[]> updateFunction)
        {
            // --- get geometry
            var geometry = this.geometryInternal as MeshGeometry3D;

            // -- set geometry if given
            if (geometry != null && geometry.Positions != null)
            {
                var data = updateFunction();
                vertexBuffer.CreateBufferFromDataArray(this.Device, data, geometry.Positions.Count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref bInvertNormalVar);
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            base.OnDetach();
        }
        
        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && this.effectMaterial != null;
        }
        
        protected override void OnRender(RenderContext renderContext)
        {
            // --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.EffectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            // --- set material params      
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);
            this.bInvertNormalVar.Set(invertNormal);
            this.bHasInstances.Set(this.hasInstances);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.RasterState;
            if (this.hasInstances)
            {
                // --- update instance buffer
                if (this.isInstanceChanged)
                {
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.instanceInternal);
                    this.isInstanceChanged = false;
                }

                // --- INSTANCING: need to set 2 buffers            
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new[]
                {
                    new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0),
                    new VertexBufferBinding(this.InstanceBuffer.Buffer, this.InstanceBuffer.StructureSize, 0),
                });
                OnInstancedDrawCall(renderContext);
                this.bHasInstances.Set(false);
            }
            else
            {
                // --- bind buffer                
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));

                OnDrawCall(renderContext);
            }
            this.bInvertNormalVar.Set(false);
        }

        /// <summary>
        /// Just before calling DrawIndexedInstanced. All buffers are attached. Override to use for multipass drawing
        /// </summary>
        /// <param name="renderContext"></param>
        protected virtual void OnInstancedDrawCall(RenderContext renderContext)
        {
            // --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);
            // --- draw
            renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
        }

        /// <summary>
        /// Just before calling DrawIndexed. All buffers are attached. Override to use for multipass drawing
        /// </summary>
        /// <param name="renderContext"></param>
        protected virtual void OnDrawCall(RenderContext renderContext)
        {
            // --- render the geometry
            // 
            var pass = this.effectTechnique.GetPassByIndex(0);
            pass.Apply(renderContext.DeviceContext);
            // --- draw
            renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);
        }

        protected override bool CheckGeometry()
        {
            return base.CheckGeometry() && geometryInternal is MeshGeometry3D;
        }

        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            var g = this.geometryInternal as MeshGeometry3D;
            bool isHit = false;

            if (g.Octree != null)
            {
                isHit = g.Octree.HitTest(context, this, ModelMatrix, rayWS, ref hits);
            }
            else
            {
                var result = new HitTestResult();
                result.Distance = double.MaxValue;
                if (g != null)
                {
                    var modelInvert = this.modelMatrix.Inverted();
                    if(modelInvert == Matrix.Zero)//Check if model matrix can be inverted.
                    {
                        return false;
                    }
                    //transform ray into model coordinates
                    var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInvert), Vector3.TransformNormal(rayWS.Direction, modelInvert));

                    var b = Bounds;
                    //Do hit test in local space
                    if (rayModel.Intersects(ref b))
                    {
                        int index = 0;
                        foreach (var t in g.Triangles)
                        {
                            float d;
                            var v0 = t.P0;
                            var v1 = t.P1;
                            var v2 = t.P2;
                            if (Collision.RayIntersectsTriangle(ref rayModel, ref v0, ref v1, ref v2, out d))
                            {
                                if (d > 0 && d < result.Distance) // If d is NaN, the condition is false.
                                {
                                    result.IsValid = true;
                                    result.ModelHit = this;
                                    // transform hit-info to world space now:
                                    var pointWorld = Vector3.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                                    result.PointHit = pointWorld.ToPoint3D();
                                    result.Distance = (rayWS.Position - pointWorld).Length();
                                    var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                                    var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                                    var p2 = Vector3.TransformCoordinate(v2, modelMatrix);
                                    var n = Vector3.Cross(p1 - p0, p2 - p0);
                                    n.Normalize();
                                    // transform hit-info to world space now:
                                    result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
                                    result.TriangleIndices = new System.Tuple<int, int, int>(g.Indices[index], g.Indices[index + 1], g.Indices[index + 2]);
                                    result.Tag = index / 3;
                                    isHit = true;
                                }
                            }
                            index += 3;
                        }
                    }
                }
                if (isHit)
                {
                    hits.Add(result);
                }
            }
            return isHit;
        }

        /// <summary>
        /// Creates a <see cref="T:DefaultVertex[]"/>.
        /// </summary>
        private DefaultVertex[] CreateDefaultVertexArray()
        {
            var geometry = this.geometryInternal as MeshGeometry3D;
            var positions = geometry.Positions.GetEnumerator();
            var vertexCount = geometry.Positions.Count;

            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.GetEnumerator() : Enumerable.Repeat(Vector2.Zero, vertexCount).GetEnumerator();
            var texScale = this.TextureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var tangents = geometry.Tangents != null ? geometry.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new DefaultVertex[vertexCount];
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }
            for (var i = 0; i < vertexCount; i++)
            {
                positions.MoveNext();
                colors.MoveNext();
                textureCoordinates.MoveNext();
                normals.MoveNext();
                tangents.MoveNext();
                bitangents.MoveNext();
                array[i].Position = new Vector4(positions.Current, 1f);
                array[i].Color = colors.Current;
                array[i].TexCoord = textureCoordinates.Current * texScale;
                array[i].Normal = normals.Current;
                array[i].Tangent = tangents.Current;
                array[i].BiTangent = bitangents.Current;
            }

            return array;
        }
    }
}

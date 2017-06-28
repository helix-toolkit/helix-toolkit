using System.Windows;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Diagnostics;
using HelixToolkit.Wpf.SharpDX.Utilities;
using System;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardTextModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(BillboardTextModel3D),
            new AffectsRenderPropertyMetadata(true,
                (d,e)=> 
                {
                    (d as BillboardTextModel3D).fixedSize = (bool)e.NewValue;
                }));

        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public bool FixedSize
        {
            set
            {
                SetValue(FixedSizeProperty, value);
            }
            get
            {
                return (bool)GetValue(FixedSizeProperty);
            }
        }
        #endregion
        #region Private Class Data Members
        private readonly ImmutableBufferProxy<BillboardVertex> vertexBuffer = new ImmutableBufferProxy<BillboardVertex>(BillboardVertex.SizeInBytes, BindFlags.VertexBuffer);
        private EffectVectorVariable vViewport;
        private EffectScalarVariable bHasBillboardTexture;
        private ShaderResourceView billboardTextureView;
        private ShaderResourceView billboardAlphaTextureView;
        private EffectShaderResourceVariable billboardTextureVariable;
        private EffectShaderResourceVariable billboardAlphaTextureVariable;
        private EffectScalarVariable bHasBillboardAlphaTexture;
        private BillboardType billboardType;
        private BillboardVertex[] vertexArrayBuffer;
        private EffectScalarVariable bFixedSizeVariable;
        #endregion

        protected bool fixedSize = true;


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
                return null;
            }
        }

        #region Overridable Methods
        /// <summary>
        /// Initial implementation of hittest for billboard. Needs further improvement.
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            var g = this.geometryInternal as IBillboardText;
            var h = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;

            if (context == null || g.Width == 0 || g.Height == 0)
            {
                return false;
            }

            if (g != null)
            {
                BoundingBox b = new BoundingBox();
                var left = -g.Width / 2;
                var right = -left;
                var top = -g.Height / 2;
                var bottom = -top;
                if (FixedSize)
                {
                    var viewportMatrix = context.ViewportMatrix;
                    var projectionMatrix = context.ProjectionMatrix; 
                    var viewMatrix = context.ViewMatrix;
                    var visualToScreen = viewMatrix * projectionMatrix * viewportMatrix;

                    var center = new Vector4(g.Positions[0], 1);
                    var screenPoint = Vector4.Transform(center, visualToScreen);
                    var spw = screenPoint.W;
                    var spx = screenPoint.X;
                    var spy = screenPoint.Y;
                    var spz = screenPoint.Z / spw / projectionMatrix.M33;

                    var matrix = CameraExtensions.InverseViewMatrix(ref viewMatrix);
                    var width = (float)context.ActualWidth;
                    var height = (float)context.ActualHeight;
                    Vector3 v = new Vector3();

                    var x = spx + left * spw;
                    var y = spy + bottom * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 bl;
                    Vector3.TransformCoordinate(ref v, ref matrix, out bl);


                    x = spx + right * spw;
                    y = spy + bottom * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 br;
                    Vector3.TransformCoordinate(ref v, ref matrix, out br);

                    x = spx + right * spw;
                    y = spy + top * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 tr;
                    Vector3.TransformCoordinate(ref v, ref matrix, out tr);

                    x = spx + left * spw;
                    y = spy + top * spw;
                    v.X = (2 * x / width / spw - 1) / projectionMatrix.M11;
                    v.Y = -(2 * y / height / spw - 1) / projectionMatrix.M22;
                    v.Z = spz;

                    Vector3 tl;
                    Vector3.TransformCoordinate(ref v, ref matrix, out tl);

                    b = BoundingBox.FromPoints(new Vector3[] { tl, tr, bl, br });

                    /*
                    var visualToScreen = viewport.GetViewProjectionMatrix() * viewport.GetViewportMatrix();

                    var screenToVisual = visualToScreen.Inverted();

                    var center = new Vector4(g.Positions[0], 1);
                    var screenPoint = Vector4.Transform(center, visualToScreen);
                    var spw = screenPoint.W;
                    var spx = screenPoint.X;
                    var spy = screenPoint.Y;
                    var spz = screenPoint.Z;

                    //Debug.WriteLine(spw);
                    // Debug.WriteLine(string.Format("Z={0}; W={1}", spz, spw));
                    var bl = new Vector4(spx + left * spw, spy + bottom * spw, spz, spw);
                    bl = Vector4.Transform(bl, screenToVisual);
                    bl /= bl.W;

                    var br = new Vector4(spx + right * spw, spy + bottom * spw, spz, spw);
                    br = Vector4.Transform(br, screenToVisual);
                    br /= br.W;

                    var tr = new Vector4(spx + right * spw, spy + top * spw, spz, spw);
                    tr = Vector4.Transform(tr, screenToVisual);
                    tr /= tr.W;

                    var tl = new Vector4(spx + left * spw, spy + top * spw, spz, spw);
                    tl = Vector4.Transform(tl, screenToVisual);
                    tl /= tl.W;

                    b = BoundingBox.FromPoints(new Vector3[] { tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3() }); 
                    */
                }
                else
                {
                    var center = new Vector4(g.Positions[0], 1);
                    var viewMatrix = context.ViewMatrix;
                    
                    var vcenter = Vector4.Transform(center, viewMatrix);
                    var vcX = vcenter.X;
                    var vcY = vcenter.Y;

                    var bl = new Vector4(vcX + left, vcY + bottom, vcenter.Z, vcenter.W);
                    var br = new Vector4(vcX + right, vcY + bottom, vcenter.Z, vcenter.W);
                    var tr = new Vector4(vcX + right, vcY + top, vcenter.Z, vcenter.W);
                    var tl = new Vector4(vcX + left, vcY + top, vcenter.Z, vcenter.W);
                    var invViewMatrix = CameraExtensions.InverseViewMatrix(ref viewMatrix);

                    bl = Vector4.Transform(bl, invViewMatrix);
                    bl /= bl.W;
                    br = Vector4.Transform(br, invViewMatrix);
                    br /= br.W;
                    tr = Vector4.Transform(tr, invViewMatrix);
                    tr /= tr.W;
                    tl = Vector4.Transform(tl, invViewMatrix);
                    tl /= tl.W;
                    b = BoundingBox.FromPoints(new Vector3[] { tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3() });
                }

                // this all happens now in world space now:
                //Debug.WriteLine(string.Format("RayPosition:{0}; Direction:{1};", rayWS.Position, rayWS.Direction));
                if (rayWS.Intersects(ref b))
                {

                    float distance;
                    if (Collision.RayIntersectsBox(ref rayWS, ref b, out distance))
                    {
                        h = true;
                        result.ModelHit = this;
                        result.IsValid = true;
                        result.PointHit = (rayWS.Position + (rayWS.Direction * distance)).ToPoint3D();
                        result.Distance = distance;
                        Debug.WriteLine(string.Format("Hit; HitPoint:{0}; Bound={1}; Distance={2}", result.PointHit, b, distance));
                    }
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText];
        }

        protected override bool CheckGeometry()
        {
            return geometryInternal is IBillboardText;
        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref this.rasterState);
            if (!IsAttached) { return; }
            // --- set up rasterizer states
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = +0,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = false,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
            try
            {
                this.rasterState = new RasterizerState(this.Device, rasterStateDesc);
            }
            catch (System.Exception)
            {
            }
        }

        protected override void OnCreateGeometryBuffers()
        {
            vertexBuffer.CreateBufferFromDataArray(this.Device, CreateBillboardVertexArray());
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }

            // --- get variables
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);
            // --- transformations
            effectTransforms = new EffectTransformVariables(effect);

            // --- shader variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();
            bFixedSizeVariable = effect.GetVariableByName("bBillboardFixedSize").AsScalar();
            // --- get geometry
            var geometry = geometryInternal as IBillboardText;
            if (geometry == null)
            {
                throw new System.Exception("Geometry must implement IBillboardText");
            }
            OnCreateGeometryBuffers();
            // --- material 
            // this.AttachMaterial();
            this.bHasBillboardTexture = effect.GetVariableByName("bHasTexture").AsScalar();
            this.billboardTextureVariable = effect.GetVariableByName("billboardTexture").AsShaderResource();
            if (geometry.Texture != null)
            {
                var textureBytes = geometry.Texture.ToByteArray();
                billboardTextureView = TextureLoader.FromMemoryAsShaderResourceView(Device, textureBytes);
            }

            this.billboardAlphaTextureVariable = effect.GetVariableByName("billboardAlphaTexture").AsShaderResource();
            this.bHasBillboardAlphaTexture = effect.GetVariableByName("bHasAlphaTexture").AsScalar();
            if (geometry.AlphaTexture != null)
            {
                billboardAlphaTextureView = global::SharpDX.Toolkit.Graphics.Texture.Load(Device, geometry.AlphaTexture);
            }
            // --- set rasterstate
            OnRasterStateChanged();

            // --- flush
            //Device.ImmediateContext.Flush();
            return true;
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref vViewport);
            Disposer.RemoveAndDispose(ref billboardTextureVariable);
            Disposer.RemoveAndDispose(ref billboardTextureView);
            Disposer.RemoveAndDispose(ref billboardAlphaTextureVariable);
            Disposer.RemoveAndDispose(ref billboardAlphaTextureView);
            Disposer.RemoveAndDispose(ref bHasBillboardAlphaTexture);
            Disposer.RemoveAndDispose(ref bHasBillboardTexture);
            Disposer.RemoveAndDispose(ref bFixedSizeVariable);
            vertexBuffer.Dispose();
            base.OnDetach();
        }

        protected override void OnRender(RenderContext renderContext)
        {
            // --- check to render the model
            var geometry = geometryInternal as IBillboardText;
            if (geometry == null)
            {
                throw new System.Exception("Geometry must implement IBillboardText");
            }

            // --- set constant paramerers             
            var worldMatrix = modelMatrix * renderContext.worldMatrix;
            effectTransforms.mWorld.SetMatrix(ref worldMatrix);
            bFixedSizeVariable?.Set(fixedSize);
            // --- check shadowmaps
            //this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            //this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = vertexLayout;
            switch (billboardType)
            {
                case BillboardType.MultipleText:
                    renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    break;
                default:
                    renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                    break;
            }

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = rasterState;

            // --- bind buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, 0));
            // --- render the geometry
            this.bHasBillboardTexture.Set(geometry.Texture != null);
            if (geometry.Texture != null)
            {
                billboardTextureVariable.SetResource(billboardTextureView);
            }

            this.bHasBillboardAlphaTexture.Set(geometry.AlphaTexture != null);
            if (geometry.AlphaTexture != null)
            {
                billboardAlphaTextureVariable.SetResource(billboardAlphaTextureView);
            }

            var vertexCount = geometryInternal.Positions.Count;
            switch (billboardType)
            {
                case BillboardType.MultipleText:
                    // Use foreground shader to draw text
                    effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);

                    // --- draw text, foreground vertex is beginning from 0.
                    renderContext.DeviceContext.Draw(vertexCount, 0);
                    break;
                case BillboardType.SingleText:
                    if (vertexCount == 8)
                    {
                        var half = vertexCount / 2;
                        // Use background shader to draw background first
                        effectTechnique.GetPassByIndex(1).Apply(renderContext.DeviceContext);
                        // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                        renderContext.DeviceContext.Draw(half, half);

                        // Use foreground shader to draw text
                        effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);

                        // --- draw text, foreground vertex is beginning from 0.
                        renderContext.DeviceContext.Draw(half, 0);
                    }
                    break;
                case BillboardType.SingleImage:
                    // Use foreground shader to draw text
                    effectTechnique.GetPassByIndex(2).Apply(renderContext.DeviceContext);

                    // --- draw text, foreground vertex is beginning from 0.
                    renderContext.DeviceContext.Draw(vertexCount, 0);
                    break;
            }
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray()
        {
            var billboardGeometry = Geometry as IBillboardText;

            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            billboardType = billboardGeometry.Type;
            billboardGeometry.DrawTexture();

            var position = billboardGeometry.Positions;
            var vertexCount = billboardGeometry.Positions.Count;
            if (!ReuseVertexArrayBuffer || vertexArrayBuffer == null || vertexArrayBuffer.Length < vertexCount)
                vertexArrayBuffer = new BillboardVertex[vertexCount];

            var allOffsets = billboardGeometry.TextureOffsets;

            for (var i = 0; i < vertexCount; i++)
            {
                var tc = billboardGeometry.TextureCoordinates[i];
                vertexArrayBuffer[i].Position = new Vector4(position[i], 1.0f);
                vertexArrayBuffer[i].Color = billboardGeometry.Colors[i];
                vertexArrayBuffer[i].TexCoord = new Vector4(tc.X, tc.Y, allOffsets[i].X, allOffsets[i].Y);
            }

            return vertexArrayBuffer;
        }

        #endregion
    }
}

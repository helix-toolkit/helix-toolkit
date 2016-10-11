using System.Windows;
﻿using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Diagnostics;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardTextModel3D : MeshGeometryModel3D
    {
        #region Private Class Data Members

        private EffectVectorVariable vViewport;
        private ShaderResourceView billboardTextureView;
        private EffectShaderResourceVariable billboardTextureVariable;

        #endregion

        #region Overridable Methods
        /// <summary>
        /// Initial implementation of hittest for billboard. Needs further improvement.
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {            
            if (this.Visibility == Visibility.Collapsed)
            {
                return false;
            }
            if (this.IsHitTestVisible == false)
            {
                return false;
            }

            var g = this.Geometry as IBillboardText;
            var h = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            Viewport3DX viewport;

            if ((viewport = FindVisualAncestor<Viewport3DX>(this.renderHost as DependencyObject)) == null || g.Width == 0 || g.Height == 0)
            {
                return false;
            }

            if (g != null)
            {
                var visualToScreen = viewport.GetViewportMatrix();
                float heightScale = 1;
                if (viewport.Camera is PerspectiveCamera)
                {
                    var viewProjection = viewport.GetViewProjectionMatrix();
                    visualToScreen.M44 = viewProjection.M44;
                }
                else if (viewport.Camera is OrthographicCamera)
                {
                    var screenView = viewport.GetScreenViewProjectionMatrix();
                    screenView.Invert();
                    visualToScreen.M44 = screenView.M43;
                    heightScale = 0.5f; //Not sure why the height needs to be half size. Need further investigation.
                }
                else
                {
                    return false;
                }

                var screenToVisual = visualToScreen.Inverted();

                var center = new Vector4(g.Positions[0], 1);
                var screenPoint = Vector4.Transform(center, visualToScreen);
                var spw = screenPoint.W;
                var spx = screenPoint.X;
                var spy = screenPoint.Y;
                var spz = screenPoint.Z;
                var left = -g.Width / 2;
                var right = g.Width / 2;
                var top = -g.Height / 2 * heightScale;
                var bottom = g.Height / 2 * heightScale;
                Debug.WriteLine(spw);
                Debug.WriteLine(string.Format("Z={0}; W={1}", spz, spw));
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

                var b = BoundingBox.FromPoints(new Vector3[] { tl.ToVector3(), tr.ToVector3(), bl.ToVector3(), br.ToVector3() });

                // this all happens now in world space now:
                Debug.WriteLine(string.Format("RayPosition:{0}; Direction:{1};", rayWS.Position, rayWS.Direction));
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

        public override void Attach(IRenderHost host)
        {
            // --- attach
            renderTechnique = host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText];
            effect = host.EffectsManager.GetEffect(renderTechnique);
            renderHost = host;

            // --- get variables
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            // --- transformations
            effectTransforms = new EffectTransformVariables(effect);

            // --- shader variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();

            // --- get geometry
            var geometry = Geometry as IBillboardText;
            if (geometry == null)
            {
                return;
            }
            // -- set geometry if given
            vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer,
                VertexSizeInBytes, CreateBillboardVertexArray());
            // --- material 
            // this.AttachMaterial();
            billboardTextureVariable = effect.GetVariableByName("billboardTexture").AsShaderResource();

            var textureBytes = geometry.Texture.ToByteArray();
            billboardTextureView = ShaderResourceView.FromMemory(Device, textureBytes);

            /// --- set rasterstate
            OnRasterStateChanged();

            /// --- flush
            Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref vViewport);
            Disposer.RemoveAndDispose(ref billboardTextureVariable);
            Disposer.RemoveAndDispose(ref billboardTextureView);
            base.Detach();
        }

        public override void Render(RenderContext renderContext)
        {
            /// --- check to render the model
            {
                if (!IsRendering)
                    return;

                if (Geometry == null)
                    return;

                if (Visibility != System.Windows.Visibility.Visible)
                    return;

                if (renderContext.IsShadowPass)
                    if (!IsThrowingShadow)
                        return;
            }

            if (renderContext.Camera is ProjectionCamera)
            {
                var c = renderContext.Camera as ProjectionCamera;
                var width = ((float)renderContext.Canvas.ActualWidth);
                var height = ((float)renderContext.Canvas.ActualHeight);
                var viewport = new Vector4(width, height, 0, 0);
                vViewport.Set(ref viewport);
            }

            /// --- set constant paramerers             
            var worldMatrix = modelMatrix * renderContext.worldMatrix;
            effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            //this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            //this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            /// --- set context
            Device.ImmediateContext.InputAssembler.InputLayout = vertexLayout;
            Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            /// --- set rasterstate            
            Device.ImmediateContext.Rasterizer.State = rasterState;

            /// --- bind buffer                
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, BillboardVertex.SizeInBytes, 0));
            /// --- render the geometry 
            billboardTextureVariable.SetResource(billboardTextureView);
            effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);

            /// --- draw
            Device.ImmediateContext.Draw(Geometry.Positions.Count, 0);
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray()
        {
            var billboardGeometry = Geometry as IBillboardText;

            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.

            billboardGeometry.DrawText();

            var position = billboardGeometry.Positions.Array;
            var vertexCount = billboardGeometry.Positions.Count;
            var result = new List<BillboardVertex>();

            var allOffsets = billboardGeometry.TextInfoOffsets;

            for (var i = 0; i < vertexCount; i++)
            {
                var tc = billboardGeometry.TextureCoordinates[i];
                var vtx = new BillboardVertex
                {
                    Position = new Vector4(position[i], 1.0f),
                    Color = billboardGeometry.Colors[i],
                    TexCoord = new Vector4(tc.X, tc.Y, allOffsets[i].X, allOffsets[i].Y)
                };

                result.Add(vtx);
            }

            return result.ToArray();
        }

        #endregion
    }
}

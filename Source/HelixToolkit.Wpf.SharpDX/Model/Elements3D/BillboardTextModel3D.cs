using System.Windows;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Diagnostics;
using HelixToolkit.Wpf.SharpDX.Utilities;
using System;
using HelixToolkit.Wpf.SharpDX.Core;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardTextModel3D : InstanceGeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(BillboardTextModel3D),
            new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    ((d as BillboardTextModel3D).RenderCore as IBillboardRenderParams).FixedSize = (bool)e.NewValue;
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
        [ThreadStatic]
        private static BillboardVertex[] vertexArrayBuffer;
        #endregion

        #region Overridable Methods

        protected override IRenderCore OnCreateRenderCore()
        {
            return new BillboardRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as IBillboardRenderParams).FixedSize = FixedSize;
        }

        protected override IGeometryBufferModel OnCreateBufferModel()
        {
            var buffer = new BillboardBufferModel<BillboardVertex>(BillboardVertex.SizeInBytes);
            buffer.OnBuildVertexArray = CreateBillboardVertexArray;
            return buffer;
        }

        /// <summary>
        /// Initial implementation of hittest for billboard. Needs further improvement.
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
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

                    var center = g.BillboardVertices[0].Position;
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
                    var center = g.BillboardVertices[0].Position;
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

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardText];
        }

        protected override bool CheckGeometry()
        {
            return geometryInternal is IBillboardText;
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
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
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray(IBillboardText billboardGeometry)
        {
            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            billboardGeometry.DrawTexture();

            //var position = billboardGeometry.Positions;
            var vertexCount = billboardGeometry.BillboardVertices.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new BillboardVertex[vertexCount];
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }

            for (var i = 0; i < vertexCount; i++)
            {
                array[i] = billboardGeometry.BillboardVertices[i];
                //var tc = billboardGeometry.TextureCoordinates[i];
                //array[i].Position = new Vector4(position[i], 1.0f);
                //array[i].Foreground = billboardGeometry.Colors[i];
                //array[i].Background = billboardGeometry.BackgroundColors[i];
                //array[i].TexCoord = new Vector4(tc.X, tc.Y, allOffsets[i].X, allOffsets[i].Y);
            }

            return array;
        }

        #endregion
    }
}

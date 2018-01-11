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
    public class BillboardTextModel3D : GeometryModel3D
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
                    ((d as Element3D).RenderCore as IBillboardRenderParams).FixedSize = (bool)e.NewValue;
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

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardText];
        }

        protected override bool CheckBoundingFrustum(BoundingFrustum viewFrustum)
        {
            return true;
        }

        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return geometry is IBillboardText;
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = false,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (Geometry as BillboardBase).HitTest(context, totalModelMatrix, ref ray, ref hits, this, FixedSize);
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

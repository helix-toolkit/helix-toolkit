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
    using Core;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MeshGeometryModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(true, RasterStateChanged));
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D), 
            new PropertyMetadata(CullMode.None, RasterStateChanged));

        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d,e)=> { ((d as GeometryModel3D).RenderCore as MeshRenderCore).InvertNormal = (bool)e.NewValue; }));

        public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as GeometryModel3D).RenderCore as PatchMeshRenderCore).EnableTessellation = (bool)e.NewValue; }));

        public static readonly DependencyProperty MaxTessellationFactorProperty =
            DependencyProperty.Register("MaxTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                if (((GeometryModel3D)d).RenderCore is IPatchRenderParams)
                {
                    (((GeometryModel3D)d).RenderCore as IPatchRenderParams).MaxTessellationFactor = (float)(double)e.NewValue;
                }
            }));

        public static readonly DependencyProperty MinTessellationFactorProperty =
            DependencyProperty.Register("MinTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(2.0, (d, e) =>
            {
                if (((GeometryModel3D)d).RenderCore is IPatchRenderParams)
                    (((GeometryModel3D)d).RenderCore as IPatchRenderParams).MinTessellationFactor = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MaxTessellationDistanceProperty =
            DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(50.0, (d, e) =>
            {
                if (((GeometryModel3D)d).RenderCore is IPatchRenderParams)
                    (((GeometryModel3D)d).RenderCore as IPatchRenderParams).MaxTessellationDistance = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MinTessellationDistanceProperty =
            DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                if (((GeometryModel3D)d).RenderCore is IPatchRenderParams)
                    (((GeometryModel3D)d).RenderCore as IPatchRenderParams).MinTessellationDistance = (float)(double)e.NewValue;
            }));


        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(MeshTopologyEnum), typeof(MeshGeometryModel3D), new PropertyMetadata(
                MeshTopologyEnum.PNTriangles, (d, e) =>
                {
                    if (((GeometryModel3D)d).RenderCore is IPatchRenderParams)
                        (((GeometryModel3D)d).RenderCore as IPatchRenderParams).MeshType = (MeshTopologyEnum)e.NewValue;
                }));

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

        public bool EnableTessellation
        {
            set
            {
                SetValue(EnableTessellationProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableTessellationProperty);
            }
        }

        public double MaxTessellationFactor
        {
            get { return (double)GetValue(MaxTessellationFactorProperty); }
            set { SetValue(MaxTessellationFactorProperty, value); }
        }

        public double MinTessellationFactor
        {
            get { return (double)GetValue(MinTessellationFactorProperty); }
            set { SetValue(MinTessellationFactorProperty, value); }
        }

        public double MaxTessellationDistance
        {
            get { return (double)GetValue(MaxTessellationDistanceProperty); }
            set { SetValue(MaxTessellationDistanceProperty, value); }
        }

        public double MinTessellationDistance
        {
            get { return (double)GetValue(MinTessellationDistanceProperty); }
            set { SetValue(MinTessellationDistanceProperty, value); }
        }

        public MeshTopologyEnum MeshTopology
        {
            set { SetValue(MeshTopologyProperty, value); }
            get { return (MeshTopologyEnum)GetValue(MeshTopologyProperty); }
        }
        #endregion
        [ThreadStatic]
        private static DefaultVertex[] vertexArrayBuffer = null;

        protected override IRenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            var c = core as IInvertNormal;
            c.InvertNormal = this.InvertNormal;
            var tessCore = core as IPatchRenderParams;
            if (tessCore != null)
            {
                tessCore.MaxTessellationFactor = (float)this.MaxTessellationFactor;
                tessCore.MinTessellationFactor = (float)this.MinTessellationFactor;
                tessCore.MaxTessellationDistance = (float)this.MaxTessellationDistance;
                tessCore.MinTessellationDistance = (float)this.MinTessellationDistance;
                tessCore.MeshType = this.MeshTopology;
                tessCore.EnableTessellation = this.EnableTessellation;
            }
            base.AssignDefaultValuesToCore(core);            
        }

        protected override IGeometryBufferModel OnCreateBufferModel()
        {
            var buffer = new MeshGeometryBufferModel<DefaultVertex>(DefaultVertex.SizeInBytes);
            buffer.OnBuildVertexArray = CreateDefaultVertexArray;
            return buffer;
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCounterClockwise,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow? false : IsScissorEnabled,
            };
        }

        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return base.OnCheckGeometry(geometry) && geometry is MeshGeometry3D;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context) && MeshTopology == MeshTopologyEnum.PNTriangles;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
        {
            return (Geometry as MeshGeometry3D).HitTest(context, totalModelMatrix, ref rayWS, ref hits, this);
        }

        /// <summary>
        /// Creates a <see cref="T:DefaultVertex[]"/>.
        /// </summary>
        private DefaultVertex[] CreateDefaultVertexArray(MeshGeometry3D geometry)
        {
            //var geometry = this.geometryInternal as MeshGeometry3D;
            var positions = geometry.Positions.GetEnumerator();
            var vertexCount = geometry.Positions.Count;

            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.GetEnumerator() : Enumerable.Repeat(Vector2.Zero, vertexCount).GetEnumerator();
            var texScale = textureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var tangents = geometry.Tangents != null ? geometry.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var array = reuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new DefaultVertex[vertexCount];
            if (reuseVertexArrayBuffer)
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
            colors.Dispose();
            textureCoordinates.Dispose();
            normals.Dispose();
            tangents.Dispose();
            bitangents.Dispose();
            positions.Dispose();
            return array;
        }
    }
}

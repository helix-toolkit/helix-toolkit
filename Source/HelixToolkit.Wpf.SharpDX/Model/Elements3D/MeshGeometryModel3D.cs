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

    using global::SharpDX.Direct3D11;
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;
    using Utilities;
    using Core;

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
            new AffectsRenderPropertyMetadata(false, (d,e)=> { ((d as GeometryModel3D).RenderCore as MeshRenderCore).InvertNormal = (bool)e.NewValue; }));

        public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(MeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(false, (d, e) => { ((d as GeometryModel3D).RenderCore as PatchMeshRenderCore).EnableTessellation = (bool)e.NewValue; }));

        public static readonly DependencyProperty MaxTessellationFactorProperty =
            DependencyProperty.Register("MaxTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new AffectsRenderPropertyMetadata(1.0, (d, e) =>
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MaxTessellationFactor = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MinTessellationFactorProperty =
            DependencyProperty.Register("MinTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new AffectsRenderPropertyMetadata(2.0, (d, e) =>
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MinTessellationFactor = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MaxTessellationDistanceProperty =
            DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new AffectsRenderPropertyMetadata(50.0, (d, e) =>
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MaxTessellationDistance = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MinTessellationDistanceProperty =
            DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new AffectsRenderPropertyMetadata(1.0, (d, e) =>
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MinTessellationDistance = (float)(double)e.NewValue;
            }));


        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(MeshTopologyEnum), typeof(MeshGeometryModel3D), new AffectsRenderPropertyMetadata(
                MeshTopologyEnum.PNTriangles, (d, e) =>
                {
                    (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MeshType = (MeshTopologyEnum)e.NewValue;
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
            var c = core as PatchMeshRenderCore;
            c.InvertNormal = this.InvertNormal;
            c.MaxTessellationFactor = (float)this.MaxTessellationFactor;
            c.MinTessellationFactor = (float)this.MinTessellationFactor;
            c.MaxTessellationDistance = (float)this.MaxTessellationDistance;
            c.MinTessellationDistance = (float)this.MinTessellationDistance;
            c.MeshType = this.MeshTopology;
            c.EnableTessellation = this.EnableTessellation;
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
                SlopeScaledDepthBias = +0,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCounterClockwise,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow? false : IsScissorEnabled,
            };
        }

        protected override bool CheckGeometry()
        {
            return base.CheckGeometry() && geometryInternal is MeshGeometry3D;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            if (MeshTopology != MeshTopologyEnum.PNTriangles)
            {
                return false;
            }
            return base.CanHitTest(context);
        }

        protected override bool OnHitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
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
        private DefaultVertex[] CreateDefaultVertexArray(MeshGeometry3D geometry)
        {
            //var geometry = this.geometryInternal as MeshGeometry3D;
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

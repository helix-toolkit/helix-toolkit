// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatchGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;
    using Utilities;
    using System;

    public static class TessellationTechniques
    {
#if TESSELLATION

        /// <summary>
        /// 
        /// </summary>
        private static readonly string[] shading = new[]
        {
            "Solid",
            "Wires",
            "Positions",
            "Normals",
            "TexCoords",
            "Tangents",
            "Colors",
        };

        /// <summary>
        /// Passes available for this Model3D
        /// </summary>
        public static IEnumerable<string> Shading { get { return shading; } }

#endif
    }

    public class PatchGeometryModel3D : MaterialGeometryModel3D
    {
#if TESSELLATION
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShadingProperty =
            DependencyProperty.Register("Shading", typeof(string), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata("Solid", ShadingChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TessellationFactorProperty =
            DependencyProperty.Register("TessellationFactor", typeof(double), typeof(PatchGeometryModel3D),
                new AffectsRenderPropertyMetadata(1.0, TessellationFactorChanged));
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(PatchGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, RasterStateChanged));

        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(PatchGeometryModel3D),
            new AffectsRenderPropertyMetadata(CullMode.None, RasterStateChanged));

        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(PatchGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, RasterStateChanged));



        /// <summary>
        /// 
        /// </summary>
        public string Shading
        {
            get { return (string)GetValue(ShadingProperty); }
            set { SetValue(ShadingProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double TessellationFactor
        {
            get { return (double)GetValue(TessellationFactorProperty); }
            set { SetValue(TessellationFactorProperty, value); }
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
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        protected static void ShadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (PatchGeometryModel3D)d;
            if (obj.IsAttached)
            {
                var shadingPass = e.NewValue as string;
                if (TessellationTechniques.Shading.Contains(shadingPass))
                {
                    // --- change the pass
                    obj.shaderPass = obj.effectTechnique.GetPassByName(shadingPass);
                    if (shadingPass.Equals("Wires"))
                    {
                        obj.FillMode = FillMode.Wireframe;
                    }
                    else
                    {
                        obj.FillMode = FillMode.Solid;
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        protected static void TessellationFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (PatchGeometryModel3D)d;
            if (obj.IsAttached)
            {
                obj.vTessellationVariables.Set(new Vector4((float)obj.TessellationFactor, 0, 0, 0));
            }
        }

        private readonly ImmutableBufferProxy<DefaultVertex> vertexBuffer = new ImmutableBufferProxy<DefaultVertex>(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer);
        private readonly ImmutableBufferProxy<int> indexBuffer = new ImmutableBufferProxy<int>(sizeof(int), BindFlags.IndexBuffer);
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
        private DefaultVertex[] vertexArrayBuffer = null;
        /// <summary>
        /// 
        /// </summary>
        public PatchGeometryModel3D()
        {
            // System.Console.WriteLine();

        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref this.rasterState);
            if (!IsAttached) { return; }
            // --- set up rasterizer states
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = -5,
                DepthBiasClamp = -10,
                SlopeScaledDepthBias = +0,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCounterClockwise,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
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
            // --- init vertex buffer
            vertexBuffer.CreateBufferFromDataArray(Device, CreateDefaultVertexArray());
            // --- init index buffer
            indexBuffer.CreateBufferFromDataArray(Device, geometryInternal.Indices.Array);
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
            // --- get variables
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            // --- get the pass
            shaderPass = effectTechnique.GetPassByName(Shading);

            // --- model transformation
            effectTransforms = new EffectTransformVariables(effect);

            // --- material 
            AttachMaterial();

            // -- get geometry
            var geometry = geometryInternal as MeshGeometry3D;

            // -- get geometry
            if (geometry != null)
            {
                OnCreateGeometryBuffers();
            }
            else
            {
                throw new System.Exception("Geometry must not be null");
            }


            // --- init instances buffer            
            //this.hasInstances = this.instanceInternal != null;            
            //this.bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            //if (this.hasInstances)
            //{                
            //    this.instanceBuffer = Buffer.Create(this.device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));                            
            //}

            // --- init tessellation vars
            vTessellationVariables = effect.GetVariableByName("vTessellation").AsVector();
            vTessellationVariables.Set(new Vector4((float)TessellationFactor, 0, 0, 0));
            // --- flush
            //Device.ImmediateContext.Flush();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetach()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            Disposer.RemoveAndDispose(ref vTessellationVariables);
            Disposer.RemoveAndDispose(ref shaderPass);
            base.OnDetach();
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnRender(RenderContext renderContext)
        {
            // --- set model transform paramerers        
            var worldMatrix = modelMatrix * renderContext.worldMatrix;
            effectTransforms.mWorld.SetMatrix(ref worldMatrix);
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);

            // --- set primitive type
            if (renderTechnique == renderHost.RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles])
            {
                renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith3ControlPoints;
            }
            else if (renderTechnique == renderHost.RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads])
            {
                renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith4ControlPoints;
            }
            else
            {
                throw new System.Exception("Technique not supported by PatchGeometryModel3D");
            }

            // --- set vertex layout
            renderContext.DeviceContext.InputAssembler.InputLayout = vertexLayout;

            // --- set index buffer
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set vertex buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, 0));

            renderContext.DeviceContext.Rasterizer.State = this.rasterState;
            // --- apply chosen pass
            shaderPass.Apply(renderContext.DeviceContext);

            // --- render the geometry
            renderContext.DeviceContext.DrawIndexed(geometryInternal.Indices.Count, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            // disable hittesting for patchgeometry for now
            // need to be implemented
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            Detach();
        }


        private EffectVectorVariable vTessellationVariables;
        private EffectPass shaderPass;

        /// <summary>
        /// Creates a <see cref="T:DefaultVertex[]"/>.
        /// </summary>
        private DefaultVertex[] CreateDefaultVertexArray()
        {
            var geometry = (MeshGeometry3D)this.geometryInternal;
            var positions = geometry.Positions.GetEnumerator();
            var vertexCount = geometry.Positions.Count;

            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            var textureCoordinates = geometry.TextureCoordinates != null ? geometry.TextureCoordinates.GetEnumerator() : Enumerable.Repeat(Vector2.Zero, vertexCount).GetEnumerator();
            var texScale = this.TextureCoodScale;
            var normals = geometry.Normals != null ? geometry.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var tangents = geometry.Tangents != null ? geometry.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            if (!ReuseVertexArrayBuffer || vertexArrayBuffer == null || vertexArrayBuffer.Length < vertexCount)
                vertexArrayBuffer = new DefaultVertex[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                positions.MoveNext();
                colors.MoveNext();
                textureCoordinates.MoveNext();
                normals.MoveNext();
                tangents.MoveNext();
                bitangents.MoveNext();
                vertexArrayBuffer[i].Position = new Vector4(positions.Current, 1f);
                vertexArrayBuffer[i].Color = colors.Current;
                vertexArrayBuffer[i].TexCoord = textureCoordinates.Current * texScale;
                vertexArrayBuffer[i].Normal = normals.Current;
                vertexArrayBuffer[i].Tangent = tangents.Current;
                vertexArrayBuffer[i].BiTangent = bitangents.Current;
            }

            return vertexArrayBuffer;
        }
#endif
    }
}
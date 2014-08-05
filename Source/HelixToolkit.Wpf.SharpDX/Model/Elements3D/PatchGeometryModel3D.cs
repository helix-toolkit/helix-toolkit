namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;    

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

        /// <summary>
        /// 
        /// </summary>
        private static readonly RenderTechnique[] technique = new[]
        {
            Techniques.RenderPNTriangs,
            Techniques.RenderPNQuads,
        };

        /// <summary>
        /// Techniqes available for this Model3D
        /// </summary>
        public static IEnumerable<RenderTechnique> RenderTechniques { get { return technique; } }

#endif
    }

    public class PatchGeometryModel3D : MaterialGeometryModel3D
    {
#if TESSELLATION
        /// <summary>
        /// 
        /// </summary>
        public string Shading
        {
            get { return (string)this.GetValue(ShadingProperty); }
            set { this.SetValue(ShadingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShadingProperty =
            DependencyProperty.Register("Shading", typeof(string), typeof(PatchGeometryModel3D), new UIPropertyMetadata("Solid", ShadingChanged));

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
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double TessellationFactor
        {
            get { return (double)this.GetValue(TessellationFactorProperty); }
            set { this.SetValue(TessellationFactorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TessellationFactorProperty =
            DependencyProperty.Register("TessellationFactor", typeof(double), typeof(PatchGeometryModel3D), new UIPropertyMetadata(1.0, TessellationFactorChanged));

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

        /// <summary>
        /// 
        /// </summary>
        public PatchGeometryModel3D()
        {
           // System.Console.WriteLine();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        public override void Attach(IRenderHost host)
        {
            /// --- attach
            this.renderTechnique = host.RenderTechnique;
            base.Attach(host);

            // --- get variables
            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            // --- get the pass
            this.shaderPass = this.effectTechnique.GetPassByName(this.Shading);

            /// --- model transformation
            this.effectTransforms = new EffectTransformVariables(this.effect);

            /// --- material 
            this.AttachMaterial();

            // -- get geometry
            var geometry = this.Geometry as MeshGeometry3D;

            // -- get geometry
            if (geometry != null)
            {
                //throw new HelixToolkitException("Geometry not found!");

                /// --- init vertex buffer
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, DefaultVertex.SizeInBytes, geometry.Positions.Select((x, ii) => new DefaultVertex()
                {
                    Position = new Vector4(x, 1f),
                    Color = geometry.Colors != null ? geometry.Colors[ii] : new Color4(1f, 0f, 0f, 1f),
                    TexCoord = geometry.TextureCoordinates != null ? geometry.TextureCoordinates[ii] : new Vector2(),
                    Normal = geometry.Normals != null ? geometry.Normals[ii] : new Vector3(),
                    Tangent = geometry.Tangents != null ? geometry.BiTangents[ii] : new Vector3(),
                    BiTangent = geometry.BiTangents != null ? geometry.BiTangents[ii] : new Vector3(),
                }).ToArray());

                /// --- init index buffer
                this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), this.Geometry.Indices.Array);
            }
            else
            {
                throw new System.Exception("Geometry must not be null");
            }


            ///// --- init instances buffer            
            //this.hasInstances = this.Instances != null;            
            //this.bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            //if (this.hasInstances)
            //{                
            //    this.instanceBuffer = Buffer.Create(this.device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));                            
            //}

            /// --- init tessellation vars
            this.vTessellationVariables = effect.GetVariableByName("vTessellation").AsVector();
            this.vTessellationVariables.Set(new Vector4((float)this.TessellationFactor, 0, 0, 0));

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vTessellationVariables);
            Disposer.RemoveAndDispose(ref this.shaderPass);
            base.Detach();
        }

        /// <summary>
        /// 
        /// </summary>        
        public override void Update(System.TimeSpan timeSpan)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render(RenderContext renderContext)
        {
            /// --- check if to render the model
            {
                if (!this.IsRendering)
                    return;

                if (this.Geometry == null)
                    return;

                if (this.Visibility != System.Windows.Visibility.Visible)
                    return;

                if (renderContext.IsShadowPass)
                    if (!this.IsThrowingShadow)
                        return;
            }

            /// --- set model transform paramerers                         
            this.effectTransforms.mWorld.SetMatrix(ref this.modelMatrix);

            /// --- set material props
            if (phongMaterial != null)
            {
                /// --- set material lighting-params      
                this.effectMaterial.vMaterialDiffuseVariable.Set(phongMaterial.DiffuseColor);
                this.effectMaterial.vMaterialAmbientVariable.Set(phongMaterial.AmbientColor);
                this.effectMaterial.vMaterialEmissiveVariable.Set(phongMaterial.EmissiveColor);
                this.effectMaterial.vMaterialSpecularVariable.Set(phongMaterial.SpecularColor);
                this.effectMaterial.vMaterialReflectVariable.Set(phongMaterial.ReflectiveColor);
                this.effectMaterial.sMaterialShininessVariable.Set(phongMaterial.SpecularShininess);

                /// --- set samplers boolean flags
                this.effectMaterial.bHasDiffuseMapVariable.Set(phongMaterial.DiffuseMap != null && this.RenderDiffuseMap);
                this.effectMaterial.bHasNormalMapVariable.Set(phongMaterial.NormalMap != null && this.RenderNormalMap);
                this.effectMaterial.bHasDisplacementMapVariable.Set(phongMaterial.DisplacementMap != null && this.RenderDisplacementMap);

                /// --- set samplers
                if (phongMaterial.DiffuseMap != null)
                {
                    this.effectMaterial.texDiffuseMapVariable.SetResource(this.texDiffuseMapView);
                }
                if (phongMaterial.NormalMap != null)
                {
                    this.effectMaterial.texNormalMapVariable.SetResource(this.texNormalMapView);
                }
                if (phongMaterial.DisplacementMap != null)
                {
                    this.effectMaterial.texDisplacementMapVariable.SetResource(this.texDisplacementMapView);
                }
            }

            /// --- set primitive type
            if (this.renderTechnique == Techniques.RenderPNTriangs)
            {
                this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith3ControlPoints;
            }
            else if (this.renderTechnique == Techniques.RenderPNQuads)
            {
                this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith4ControlPoints;
            }
            else
            {
                throw new System.Exception("Technique not supported by PatchGeometryModel3D");
            }

            /// --- set vertex layout
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;

            /// --- set index buffer
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);

            /// --- set vertex buffer                
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, DefaultVertex.SizeInBytes, 0));

            /// --- apply chosen pass
            this.shaderPass.Apply(Device.ImmediateContext);

            /// --- render the geometry
            this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
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
            this.Detach();
        }


        private EffectVectorVariable vTessellationVariables;
        private EffectPass shaderPass;
#endif
    } 
}
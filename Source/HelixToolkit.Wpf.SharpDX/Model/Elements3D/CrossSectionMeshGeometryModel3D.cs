// <copyright file="CrossSectionMeshGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>


namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using System.Windows;
    using Media = System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="CrossSectionMeshGeometryModel3D" />
    /// </summary>
    public class CrossSectionMeshGeometryModel3D : MeshGeometryModel3D
    {
        /// <summary>
        /// The PlaneToVector
        /// </summary>
        /// <param name="p">The <see cref="Plane"/></param>
        /// <returns>The <see cref="Vector4"/></returns>
        private static Vector4 PlaneToVector(Plane p)
        {
            return new Vector4(p.Normal, p.D);
        }
        #region Dependency Properties
        /// <summary>
        /// Defines the CrossSectionColorProperty
        /// </summary>
        public static DependencyProperty CrossSectionColorProperty = DependencyProperty.Register("CrossSectionColor", typeof(Media.Color), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(Media.Colors.Firebrick,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).sectionColor = ((Media.Color)e.NewValue).ToColor4();
           }));

        /// <summary>
        /// Gets or sets the CrossSectionColor
        /// </summary>
        public Media.Color CrossSectionColor
        {
            set
            {
                SetValue(CrossSectionColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(CrossSectionColorProperty);
            }
        }
        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane1Property = DependencyProperty.Register("EnablePlane1", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeEnabled.X = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane1
        {
            set
            {
                SetValue(EnablePlane1Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane1Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane2Property = DependencyProperty.Register("EnablePlane2", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeEnabled.Y = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane2
        {
            set
            {
                SetValue(EnablePlane2Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane2Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane3Property = DependencyProperty.Register("EnablePlane3", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeEnabled.Z = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane3
        {
            set
            {
                SetValue(EnablePlane3Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane3Property);
            }
        }

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public static DependencyProperty EnablePlane4Property = DependencyProperty.Register("EnablePlane4", typeof(bool), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false,
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeEnabled.W = (bool)e.NewValue;
           }));

        /// <summary>
        /// Enable CrossSection Plane
        /// </summary>
        public bool EnablePlane4
        {
            set
            {
                SetValue(EnablePlane4Property, value);
            }
            get
            {
                return (bool)GetValue(EnablePlane4Property);
            }
        }

        /// <summary>
        /// Defines the Plane1Property
        /// </summary>
        public static DependencyProperty Plane1Property = DependencyProperty.Register("Plane1", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeParams.Row1 = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane1
        /// </summary>
        public Plane Plane1
        {
            set
            {
                SetValue(Plane1Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane1Property);
            }
        }

        /// <summary>
        /// Defines the Plane2Property
        /// </summary>
        public static DependencyProperty Plane2Property = DependencyProperty.Register("Plane2", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeParams.Row2 = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane2
        /// </summary>
        public Plane Plane2
        {
            set
            {
                SetValue(Plane2Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane2Property);
            }
        }

        /// <summary>
        /// Defines the Plane3Property
        /// </summary>
        public static DependencyProperty Plane3Property = DependencyProperty.Register("Plane3", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeParams.Row3 = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane3
        /// </summary>
        public Plane Plane3
        {
            set
            {
                SetValue(Plane3Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane3Property);
            }
        }

        /// <summary>
        /// Defines the Plane4Property
        /// </summary>
        public static DependencyProperty Plane4Property = DependencyProperty.Register("Plane4", typeof(Plane), typeof(CrossSectionMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(new Plane(),
           (d, e) =>
           {
               (d as CrossSectionMeshGeometryModel3D).planeParams.Row4 = PlaneToVector((Plane)e.NewValue);
           }));

        /// <summary>
        /// Gets or sets the Plane4
        /// </summary>
        public Plane Plane4
        {
            set
            {
                SetValue(Plane4Property, value);
            }
            get
            {
                return (Plane)GetValue(Plane4Property);
            }
        }
        #endregion

        #region Private Variables
        /// <summary>
        /// Defines the planeParamsVar
        /// </summary>
        private EffectMatrixVariable planeParamsVar;

        /// <summary>
        /// Defines the planeEnabledVar
        /// </summary>
        private EffectVectorVariable planeEnabledVar;

        /// <summary>
        /// Defines the crossSectionColorVar
        /// </summary>
        private EffectVectorVariable crossSectionColorVar;

        /// <summary>
        /// Defines the sectionFillTextureVar
        /// </summary>
        private EffectShaderResourceVariable sectionFillTextureVar;

        /// <summary>
        /// Defines the fillStencilRasterState
        /// </summary>
        private RasterizerState fillStencilRasterState;

        /// <summary>
        /// Defines the fillCrossSectionState
        /// </summary>
        private DepthStencilState fillCrossSectionState;

        /// <summary>
        /// Defines the fillStencilState
        /// </summary>
        private DepthStencilState fillStencilState;

        /// <summary>
        /// Defines the sectionColor
        /// </summary>
        private Color4 sectionColor = Color.Firebrick;

        /// <summary>
        /// Defines the planeEnabled
        /// </summary>
        private Bool4 planeEnabled = new Bool4(false, false, false, false);

        /// <summary>
        /// Defines the planeParams
        /// </summary>
        private Matrix planeParams = new Matrix();

        #endregion
        /// <summary>
        /// The SetRenderTechnique
        /// </summary>
        /// <param name="host">The <see cref="IRenderHost"/></param>
        /// <returns>The <see cref="RenderTechnique"/></returns>
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.CrossSection];
        }

        /// <summary>
        /// The OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            planeParamsVar = effect.GetVariableByName("CrossPlaneParams").AsMatrix();
            planeEnabledVar = effect.GetVariableByName("EnableCrossPlane").AsVector();
            crossSectionColorVar = effect.GetVariableByName("CrossSectionColor").AsVector();
            sectionFillTextureVar = effect.GetVariableByName("SectionFillTexture").AsShaderResource();

            CreateStates();
        }

        /// <summary>
        /// The CreateStates
        /// </summary>
        private void CreateStates()
        {
            this.fillStencilRasterState = new RasterizerState(this.Device,
                new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Front,
                    DepthBias = DepthBias,
                    DepthBiasClamp = -1000,
                    SlopeScaledDepthBias = +0,
                    IsDepthClipEnabled = IsDepthClipEnabled,
                    IsFrontCounterClockwise = FrontCounterClockwise,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false
                });

            fillStencilState = new DepthStencilState(this.Device,
                new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,
                    IsStencilEnabled = true,
                    DepthWriteMask = DepthWriteMask.Zero,
                    DepthComparison = Comparison.Less,
                    StencilWriteMask = 0xFF,
                    StencilReadMask = 0,
                    BackFace = new DepthStencilOperationDescription()
                    {
                        PassOperation = StencilOperation.Replace,
                        Comparison = Comparison.Always,
                        DepthFailOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep
                    },
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Never,
                        DepthFailOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep
                    }
                });

            fillCrossSectionState = new DepthStencilState(this.Device,
                new DepthStencilStateDescription()
                {
                    IsDepthEnabled = false,
                    IsStencilEnabled = true,
                    DepthWriteMask = DepthWriteMask.Zero,
                    DepthComparison = Comparison.Less,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Equal
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        Comparison = Comparison.Never,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Keep
                    },
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0
                });
        }

        /// <summary>
        /// The OnRender
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/></param>
        protected override void OnRender(RenderContext renderContext)
        {
            planeParamsVar.SetMatrix(planeParams);
            planeEnabledVar.Set(planeEnabled);
            base.OnRender(renderContext);
        }


        /*
        /// <summary>
        /// The OnDrawCall
        /// </summary>
        /// <param name="renderContext">The <see cref="RenderContext"/></param>
        protected override void OnDrawCall(RenderContext renderContext)
        {
            base.OnDrawCall(renderContext);
            //Draw backface into stencil buffer
            renderContext.DeviceContext.Rasterizer.State = fillStencilRasterState;

            renderContext.DeviceContext.ClearDepthStencilView(RenderHost.DepthStencilBufferView, DepthStencilClearFlags.Stencil, 0, 0);
            var pass = this.effectTechnique.GetPassByIndex(1);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(RenderHost.DepthStencilBufferView, new RenderTargetView[0]);//Remove render target
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(fillStencilState, 1); //Draw backface onto stencil buffer, set value to 1
            renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);

            //Draw full screen quad to fill cross section
            crossSectionColorVar.Set(sectionColor);
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            renderContext.DeviceContext.Rasterizer.State = RasterState;

            pass = this.effectTechnique.GetPassByIndex(2);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(RenderHost.DepthStencilBufferView, RenderHost.ColorBufferView);//Rebind render target
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(fillCrossSectionState, 1); //Only pass stencil buffer test if value is 1
            renderContext.DeviceContext.Draw(4, 0);
        }
        */
        /// <summary>
        /// The OnDetach
        /// </summary>
        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref planeParamsVar);
            Disposer.RemoveAndDispose(ref planeEnabledVar);
            Disposer.RemoveAndDispose(ref fillStencilRasterState);
            Disposer.RemoveAndDispose(ref fillStencilState);
            Disposer.RemoveAndDispose(ref fillCrossSectionState);
            Disposer.RemoveAndDispose(ref crossSectionColorVar);
            Disposer.RemoveAndDispose(ref sectionFillTextureVar);
            base.OnDetach();
        }
    }
}

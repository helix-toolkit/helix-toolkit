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
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.MaterialGeometryModel3D" />
    public class MeshGeometryModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties        
        /// <summary>
        /// The front counter clockwise property
        /// </summary>
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(true, RasterStateChanged));
        /// <summary>
        /// The cull mode property
        /// </summary>
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D), 
            new PropertyMetadata(CullMode.None, RasterStateChanged));
        /// <summary>
        /// The invert normal property
        /// </summary>
        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d,e)=> { ((d as GeometryModel3D).RenderCore as MeshRenderCore).InvertNormal = (bool)e.NewValue; }));
        /// <summary>
        /// The enable tessellation property
        /// </summary>
        public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as GeometryModel3D).RenderCore as PatchMeshRenderCore).EnableTessellation = (bool)e.NewValue; }));
        /// <summary>
        /// The maximum tessellation factor property
        /// </summary>
        public static readonly DependencyProperty MaxTessellationFactorProperty =
            DependencyProperty.Register("MaxTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                if ((d as IRenderable).RenderCore is IPatchRenderParams r)
                {
                    r.MaxTessellationFactor = (float)(double)e.NewValue;
                }
            }));
        /// <summary>
        /// The minimum tessellation factor property
        /// </summary>
        public static readonly DependencyProperty MinTessellationFactorProperty =
            DependencyProperty.Register("MinTessellationFactor", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(2.0, (d, e) =>
            {
                if ((d as IRenderable).RenderCore is IPatchRenderParams r)
                    r.MinTessellationFactor = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// The maximum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MaxTessellationDistanceProperty =
            DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(50.0, (d, e) =>
            {
                if ((d as IRenderable).RenderCore is IPatchRenderParams r)
                    r.MaxTessellationDistance = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// The minimum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MinTessellationDistanceProperty =
            DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                if ((d as IRenderable).RenderCore is IPatchRenderParams r)
                    r.MinTessellationDistance = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// The mesh topology property
        /// </summary>
        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(MeshTopologyEnum), typeof(MeshGeometryModel3D), new PropertyMetadata(
                MeshTopologyEnum.PNTriangles, (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IPatchRenderParams r)
                        r.MeshType = (MeshTopologyEnum)e.NewValue;
                }));

        /// <summary>
        /// The render wireframe property
        /// </summary>
        public static readonly DependencyProperty RenderWireframeProperty =
            DependencyProperty.Register("RenderWireframe", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d,e)=>
            {
                if ((d as IRenderable).RenderCore is IMeshRenderParams r)
                    r.RenderWireframe = (bool)e.NewValue;
            }));

        /// <summary>
        /// The wireframe color property
        /// </summary>
        public static readonly DependencyProperty WireframeColorProperty =
            DependencyProperty.Register("WireframeColor", typeof(System.Windows.Media.Color), typeof(MeshGeometryModel3D), new PropertyMetadata(System.Windows.Media.Colors.SkyBlue, (d,e)=> 
            {
                if ((d as IRenderable).RenderCore is IMeshRenderParams r)
                    r.WireframeColor = ((System.Windows.Media.Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// Gets or sets a value indicating whether [render overlapping wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get { return (bool)GetValue(RenderWireframeProperty); }
            set { SetValue(RenderWireframeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public System.Windows.Media.Color WireframeColor
        {
            get { return (System.Windows.Media.Color)GetValue(WireframeColorProperty); }
            set { SetValue(WireframeColorProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [front counter clockwise].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front counter clockwise]; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether [enable tessellation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable tessellation]; otherwise, <c>false</c>.
        /// </value>
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
        /// <summary>
        /// Gets or sets the maximum tessellation factor.
        /// </summary>
        /// <value>
        /// The maximum tessellation factor.
        /// </value>
        public double MaxTessellationFactor
        {
            get { return (double)GetValue(MaxTessellationFactorProperty); }
            set { SetValue(MaxTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation factor.
        /// </summary>
        /// <value>
        /// The minimum tessellation factor.
        /// </value>
        public double MinTessellationFactor
        {
            get { return (double)GetValue(MinTessellationFactorProperty); }
            set { SetValue(MinTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation distance.
        /// </summary>
        /// <value>
        /// The maximum tessellation distance.
        /// </value>
        public double MaxTessellationDistance
        {
            get { return (double)GetValue(MaxTessellationDistanceProperty); }
            set { SetValue(MaxTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation distance.
        /// </summary>
        /// <value>
        /// The minimum tessellation distance.
        /// </value>
        public double MinTessellationDistance
        {
            get { return (double)GetValue(MinTessellationDistanceProperty); }
            set { SetValue(MinTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the mesh topology.
        /// </summary>
        /// <value>
        /// The mesh topology.
        /// </value>
        public MeshTopologyEnum MeshTopology
        {
            set { SetValue(MeshTopologyProperty, value); }
            get { return (MeshTopologyEnum)GetValue(MeshTopologyProperty); }
        }
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            var c = core as IMeshRenderParams;
            c.InvertNormal = this.InvertNormal;
            c.WireframeColor = this.WireframeColor.ToColor4();
            c.RenderWireframe = this.RenderWireframe;
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
        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <param name="modelGuid"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected override IGeometryBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            var buffer = EffectsManager.GeometryBufferManager.Register<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
            return buffer;
        }
        /// <summary>
        /// Called when [unregister buffer model].
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        /// <param name="geometry">The geometry.</param>
        protected override void OnUnregisterBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            EffectsManager.GeometryBufferManager.Unregister<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
        }

        /// <summary>
        /// Create raster state description.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return base.OnCheckGeometry(geometry) && geometry is MeshGeometry3D;
        }
        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context) && MeshTopology == MeshTopologyEnum.PNTriangles;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
        {
            return (Geometry as MeshGeometry3D).HitTest(context, totalModelMatrix, ref rayWS, ref hits, this);
        }
    }
}

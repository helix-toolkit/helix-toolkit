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
        /// <para>If <see cref="OnCreateRasterState" /> is set, then <see cref="OnCreateRasterState" /> instead of <see cref="CreateRasterState" /> will be called.</para>
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

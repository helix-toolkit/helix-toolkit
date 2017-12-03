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
    using System.Windows;
    using Core;
    using global::SharpDX.Direct3D;

    public class PatchGeometryModel3D : MeshGeometryModel3D
    {
#if TESSELLATION
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShadingProperty =
            DependencyProperty.Register("Shading", typeof(string), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata(TessellationTechniques.Shading.Solid.ToString(), (d,e)=> 
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).TessellationTechniqueName = (string)e.NewValue;        
            }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TessellationFactorProperty =
            DependencyProperty.Register("TessellationFactor", typeof(double), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata(1.0, (d,e)=> 
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).TessellationFactor = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(TessellationTechniques.MeshTopology), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata(
                TessellationTechniques.MeshTopology.Triangle, (d, e) => 
                {
                    var model = d as PatchGeometryModel3D;
                    if (model.IsAttached)
                    {
                        var host = model.renderHost;
                        model.Detach();
                        model.Attach(host);
                    }
                }));

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

        public TessellationTechniques.MeshTopology MeshTopology
        {
            set { SetValue(MeshTopologyProperty, value); }
            get { return (TessellationTechniques.MeshTopology)GetValue(MeshTopologyProperty); }
        }
        #endregion

        protected override IRenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            (core as PatchMeshRenderCore).TessellationFactor = (float)TessellationFactor;
            (core as PatchMeshRenderCore).TessellationTechniqueName = this.Shading;
            base.AssignDefaultValuesToCore(core);
        }

        protected override IRenderTechnique SetRenderTechnique(IRenderHost host)
        {
            switch (MeshTopology)
            {
                case TessellationTechniques.MeshTopology.Triangle:
                    return host.RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNTriangles];
                case TessellationTechniques.MeshTopology.Quads:
                    return host.RenderTechniquesManager.RenderTechniques[TessellationRenderTechniqueNames.PNQuads];
                default:
                    return null;
            }          
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            if(BufferModel.Topology != PrimitiveTopology.PatchListWith3ControlPoints)
            {
                return false;
            }
            return base.CanHitTest(context);
        }
#endif
    }
}
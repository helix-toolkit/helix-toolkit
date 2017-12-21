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

    public class PatchGeometryModel3D : MeshGeometryModel3D
    {
#if TESSELLATION
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TessellationFactorProperty =
            DependencyProperty.Register("TessellationFactor", typeof(double), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata(1.0, (d,e)=> 
            {
                (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).TessellationFactor = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty MeshTopologyProperty =
            DependencyProperty.Register("MeshTopology", typeof(MeshTopologyEnum), typeof(PatchGeometryModel3D), new AffectsRenderPropertyMetadata(
                MeshTopologyEnum.PNTriangles, (d, e) => 
                {
                    (((GeometryModel3D)d).RenderCore as PatchMeshRenderCore).MeshType = (MeshTopologyEnum)e.NewValue;
                }));

        /// <summary>
        /// 
        /// </summary>
        public double TessellationFactor
        {
            get { return (double)GetValue(TessellationFactorProperty); }
            set { SetValue(TessellationFactorProperty, value); }
        }

        public MeshTopologyEnum MeshTopology
        {
            set { SetValue(MeshTopologyProperty, value); }
            get { return (MeshTopologyEnum)GetValue(MeshTopologyProperty); }
        }
        #endregion

        protected override IRenderCore OnCreateRenderCore()
        {
            return new PatchMeshRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            (core as PatchMeshRenderCore).TessellationFactor = (float)TessellationFactor;
            (core as PatchMeshRenderCore).MeshType = this.MeshTopology;
            base.AssignDefaultValuesToCore(core);
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            if(MeshTopology != MeshTopologyEnum.PNTriangles)
            {
                return false;
            }
            return base.CanHitTest(context);
        }
#endif
    }
}
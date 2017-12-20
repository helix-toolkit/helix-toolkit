using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class OutLineMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty EnableOutlineProperty = DependencyProperty.Register("EnableOutline", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, (d, e) =>
            {
                ((d as OutLineMeshGeometryModel3D).RenderCore as MeshOutlineRenderCore).OutlineEnabled = (bool)e.NewValue;
            }));

        public bool EnableOutline
        {
            set
            {
                SetValue(EnableOutlineProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableOutlineProperty);
            }
        }

        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Media.Color), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.White,
            (d, e) =>
            {
                ((d as OutLineMeshGeometryModel3D).RenderCore as MeshOutlineRenderCore).Color = ((Media.Color)e.NewValue).ToColor4();
            }));

        public Media.Color OutlineColor
        {
            set
            {
                SetValue(OutlineColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(OutlineColorProperty);
            }
        }

        public static DependencyProperty IsDrawGeometryProperty = DependencyProperty.Register("IsDrawGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, (d, e) => {
                ((d as OutLineMeshGeometryModel3D).RenderCore as MeshOutlineRenderCore).DrawMesh = (bool)e.NewValue;
            }));

        public bool IsDrawGeometry
        {
            set
            {
                SetValue(IsDrawGeometryProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDrawGeometryProperty);
            }
        }


        public static DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(float), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(1.5f, (d, e) => {
                ((d as OutLineMeshGeometryModel3D).RenderCore as MeshOutlineRenderCore).OutlineFadingFactor = (float)e.NewValue;
            }));

        public float OutlineFadingFactor
        {
            set
            {
                SetValue(OutlineFadingFactorProperty, value);
            }
            get
            {
                return (float)GetValue(OutlineFadingFactorProperty);
            }
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new MeshOutlineRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            var c = core as MeshOutlineRenderCore;
            c.Color = this.OutlineColor.ToColor4();
            c.OutlineEnabled = this.EnableOutline;
            c.OutlineFadingFactor = this.OutlineFadingFactor;
            c.DrawMesh = this.IsDrawGeometry;
            base.AssignDefaultValuesToCore(core);
        }
    }
}

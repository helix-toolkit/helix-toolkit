using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using System.Windows;
using Media = System.Windows.Media;
/*
namespace HelixToolkit.Wpf.SharpDX
{
    public class OutLineMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty EnableOutlineProperty = DependencyProperty.Register("EnableOutline", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IMeshOutlineParams).OutlineEnabled = (bool)e.NewValue;
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
            new PropertyMetadata(Media.Colors.White,
            (d, e) =>
            {
                ((d as IRenderable).RenderCore as IMeshOutlineParams).Color = ((Media.Color)e.NewValue).ToColor4();
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
            new PropertyMetadata(true, (d, e) => {
                ((d as IRenderable).RenderCore as IMeshOutlineParams).DrawMesh = (bool)e.NewValue;
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


        public static DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(1.5, (d, e) => {
                ((d as IRenderable).RenderCore as IMeshOutlineParams).OutlineFadingFactor = (float)(double)e.NewValue;
            }));

        public double OutlineFadingFactor
        {
            set
            {
                SetValue(OutlineFadingFactorProperty, value);
            }
            get
            {
                return (double)GetValue(OutlineFadingFactorProperty);
            }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshOutlineRenderCore();
        }

        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            var c = core as IMeshOutlineParams;
            c.Color = this.OutlineColor.ToColor4();
            c.OutlineEnabled = this.EnableOutline;
            c.OutlineFadingFactor = (float)this.OutlineFadingFactor;
            c.DrawMesh = this.IsDrawGeometry;
            base.AssignDefaultValuesToCore(core);
        }
    }
}
*/
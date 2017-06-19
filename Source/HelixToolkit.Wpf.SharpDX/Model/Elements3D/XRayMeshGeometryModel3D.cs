using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Windows;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
{
    public class XRayMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty EnableXRayProperty = DependencyProperty.Register("EnableXRay", typeof(bool), typeof(XRayMeshGeometryModel3D),
           new AffectsRenderPropertyMetadata(false, (d, e) =>
           {
               (d as XRayMeshGeometryModel3D).enableXRay = (bool)e.NewValue;
           }));

        public bool EnableXRay
        {
            set
            {
                SetValue(EnableXRayProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableXRayProperty);
            }
        }

        public static DependencyProperty XRayColorProperty = DependencyProperty.Register("XRayColor", typeof(Media.Color), typeof(XRayMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.White,
            (d, e) =>
            {
                (d as XRayMeshGeometryModel3D).xRayColor = ((Media.Color)e.NewValue).ToColor4();
            }));

        public Media.Color XRayColor
        {
            set
            {
                SetValue(XRayColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(XRayColorProperty);
            }
        }


        protected bool enableXRay { private set; get; }
        protected Color4 xRayColor { private set; get; }

        protected EffectVectorVariable xRayColorVar;

        public XRayMeshGeometryModel3D()
        {
            xRayColor = XRayColor.ToColor4();
        }

        protected override void OnBeforeDrawCall(RenderContext renderContext)
        {
            if (enableXRay)
            {
                xRayColorVar.Set(xRayColor);
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);
            }
            base.OnBeforeDrawCall(renderContext);
        }

        protected override void OnBeforeInstancedDrawCall(RenderContext renderContext)
        {
            if (enableXRay)
            {
                xRayColorVar.Set(xRayColor);
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
            }
            base.OnBeforeInstancedDrawCall(renderContext);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            xRayColorVar = effect.GetVariableByName("XRayObjectColor").AsVector();
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref xRayColorVar);
            base.OnDetach();
        }
    }
}

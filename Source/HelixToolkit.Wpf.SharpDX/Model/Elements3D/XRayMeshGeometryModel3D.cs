using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Windows;
using Media = System.Windows.Media;
using System;

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

        public static DependencyProperty BlendStateDescriptionProperty = DependencyProperty.Register("BlendStateDescription", typeof(BlendStateDescription), typeof(XRayMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(new BlendStateDescription(), 
                (d, e) => {
                    (d as XRayMeshGeometryModel3D).isBlendChanged = true;
                }));

        public BlendStateDescription BlendStateDescription
        {
            set
            {
                SetValue(BlendStateDescriptionProperty, value);
            }
            get
            {
                return (BlendStateDescription)GetValue(BlendStateDescriptionProperty);
            }
        }

        public static DependencyProperty DepthStencilStateDescriptionProperty = DependencyProperty.Register("DepthStencilStateDescription", typeof(DepthStencilStateDescription), typeof(XRayMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(new DepthStencilStateDescription(),
        (d, e) => {
            (d as XRayMeshGeometryModel3D).isDepthStencilStateChanged = true;
        }));

        public DepthStencilStateDescription DepthStencilStateDescription
        {
            set
            {
                SetValue(DepthStencilStateDescriptionProperty, value);
            }
            get
            {
                return (DepthStencilStateDescription)GetValue(DepthStencilStateDescriptionProperty);
            }
        }

        protected bool enableXRay { private set; get; }
        protected Color4 xRayColor { private set; get; }
        protected bool isBlendChanged { private set; get; } = true;
        protected bool isDepthStencilStateChanged { private set; get; } = true;

        protected EffectVectorVariable xRayColorVar;
        protected BlendState blendState;
        protected DepthStencilState depthStencilState;

        public XRayMeshGeometryModel3D()
        {
            xRayColor = XRayColor.ToColor4();
            var blendDesc = new BlendStateDescription();
            blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                BlendOperation = BlendOperation.Add,
                AlphaBlendOperation = BlendOperation.Add,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            this.BlendStateDescription = blendDesc;

            var depthStencilDesc = new DepthStencilStateDescription()
            {
                 IsDepthEnabled=true, DepthComparison = Comparison.Greater, DepthWriteMask = DepthWriteMask.Zero
            };
            DepthStencilStateDescription = depthStencilDesc;
        }

        private void OnBlendStateChanged()
        {
            if (isBlendChanged)
            {
                Disposer.RemoveAndDispose(ref blendState);
                blendState = new BlendState(this.Device, this.BlendStateDescription);
                isBlendChanged = false;
            }
        }

        private void OnDepthStencilChanged()
        {
            if (isDepthStencilStateChanged)
            {
                Disposer.RemoveAndDispose(ref depthStencilState);
                depthStencilState = new DepthStencilState(this.Device, this.DepthStencilStateDescription);
                isDepthStencilStateChanged = false;
            }
        }

        protected override void OnBeforeDrawCall(RenderContext renderContext)
        {
            if (enableXRay)
            {
                OnBlendStateChanged();
                OnDepthStencilChanged();
                xRayColorVar.Set(xRayColor);
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                renderContext.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
                renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState, 0);
                // --- draw
                renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);
            }
            base.OnBeforeDrawCall(renderContext);
        }

        protected override void OnBeforeInstancedDrawCall(RenderContext renderContext)
        {
            if (enableXRay)
            {
                OnBlendStateChanged();
                OnDepthStencilChanged();
                xRayColorVar.Set(xRayColor);
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                renderContext.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
                renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState, 0);
                // --- draw
                renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
            }
            base.OnBeforeInstancedDrawCall(renderContext);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            xRayColorVar = effect.GetVariableByName("XRayObjectColor").AsVector();
            isBlendChanged = true;
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref xRayColorVar);
            Disposer.RemoveAndDispose(ref blendState);
            base.OnDetach();
        }
    }
}

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
                (d as OutLineMeshGeometryModel3D).enableOutline = (bool)e.NewValue;
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
                (d as OutLineMeshGeometryModel3D).outlineColor = ((Media.Color)e.NewValue).ToColor4();
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

        public static DependencyProperty BlendStateDescriptionProperty = DependencyProperty.Register("BlendStateDescription", typeof(BlendStateDescription), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(new BlendStateDescription(),
                (d, e) =>
                {
                    (d as OutLineMeshGeometryModel3D).isBlendChanged = true;
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

        public static DependencyProperty DepthStencilStateDescriptionProperty = DependencyProperty.Register("DepthStencilStateDescription", typeof(DepthStencilStateDescription), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(new DepthStencilStateDescription(),
            (d, e) =>
            {
                (d as OutLineMeshGeometryModel3D).isDepthStencilStateChanged = true;
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

        public static DependencyProperty IsDrawBeforeGeometryProperty = DependencyProperty.Register("IsDrawBeforeGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(false, (d, e) => {
                (d as OutLineMeshGeometryModel3D).isDrawBeforeGeometry = (bool)e.NewValue;
            }));

        public bool IsDrawBeforeGeometry
        {
            set
            {
                SetValue(IsDrawBeforeGeometryProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDrawBeforeGeometryProperty);
            }
        }

        public static DependencyProperty IsDrawGeometryProperty = DependencyProperty.Register("IsDrawGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new AffectsRenderPropertyMetadata(true, (d, e) => {
                (d as OutLineMeshGeometryModel3D).isDrawGeometry = (bool)e.NewValue;
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
                (d as OutLineMeshGeometryModel3D).outlineFadingFactor = (float)e.NewValue;
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

        protected bool enableOutline { private set; get; } = true;
        protected Color4 outlineColor { private set; get; } = Color.White;
        protected bool isBlendChanged { private set; get; } = true;
        protected bool isDepthStencilStateChanged { private set; get; } = true;

        protected bool isDrawBeforeGeometry { private set; get; } = false;
        protected bool isDrawGeometry { private set; get; } = true;

        protected float outlineFadingFactor { private set; get; } = 1.5f;

        protected EffectVectorVariable xOutlineColorVar;
        protected BlendState blendState;
        protected DepthStencilState depthStencilState;
        protected EffectScalarVariable xOutlineFadingFactor;
        public OutLineMeshGeometryModel3D()
        {
            outlineColor = OutlineColor.ToColor4();
            var blendDesc = new BlendStateDescription();
            blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                BlendOperation = BlendOperation.Add,
                AlphaBlendOperation = BlendOperation.Add,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            this.BlendStateDescription = blendDesc;

            var depthStencilDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.Zero
            };
            DepthStencilStateDescription = depthStencilDesc;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnBlendStateChanged()
        {
            if (isBlendChanged)
            {
                Disposer.RemoveAndDispose(ref blendState);
                blendState = new BlendState(this.Device, this.BlendStateDescription);
                isBlendChanged = false;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnDepthStencilChanged()
        {
            if (isDepthStencilStateChanged)
            {
                Disposer.RemoveAndDispose(ref depthStencilState);
                depthStencilState = new DepthStencilState(this.Device, this.DepthStencilStateDescription);
                isDepthStencilStateChanged = false;
            }
        }

        protected override void OnDrawCall(RenderContext renderContext)
        {
            if (isDrawBeforeGeometry)
            {
                OnOutLineDrawCall(renderContext);
            }
            if (isDrawGeometry)
            {
                base.OnDrawCall(renderContext);
            }
            if (!isDrawBeforeGeometry)
            {
                OnOutLineDrawCall(renderContext);
            }
        }

        protected override void OnInstancedDrawCall(RenderContext renderContext)
        {
            if (isDrawBeforeGeometry)
            {
                OnOutLineInstancedDrawCall(renderContext);
            }
            if (isDrawGeometry)
            { base.OnInstancedDrawCall(renderContext); }
            if (!isDrawBeforeGeometry)
            {
                OnOutLineInstancedDrawCall(renderContext);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnOutLineDrawCall(RenderContext renderContext)
        {
            if (enableOutline)
            {
                SetParameters();
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                renderContext.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
                renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState, 0);
                // --- draw
                renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnOutLineInstancedDrawCall(RenderContext renderContext)
        {
            if (enableOutline)
            {
                SetParameters();
                // --- render the xray
                // 
                var pass1 = this.effectTechnique.GetPassByIndex(2);
                pass1.Apply(renderContext.DeviceContext);
                renderContext.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
                renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState, 0);
                // --- draw
                renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetParameters()
        {
            OnBlendStateChanged();
            OnDepthStencilChanged();
            xOutlineColorVar.Set(outlineColor);
            xOutlineFadingFactor.Set(outlineFadingFactor);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            xOutlineColorVar = effect.GetVariableByName("XRayObjectColor").AsVector();
            xOutlineFadingFactor = effect.GetVariableByName("XRayBorderFadingFactor").AsScalar();
            isBlendChanged = true;
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref xOutlineColorVar);
            Disposer.RemoveAndDispose(ref blendState);
            Disposer.RemoveAndDispose(ref depthStencilState);
            Disposer.RemoveAndDispose(ref xOutlineFadingFactor);
            base.OnDetach();
        }
    }
}

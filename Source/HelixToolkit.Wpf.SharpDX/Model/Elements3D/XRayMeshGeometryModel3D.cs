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
            new PropertyMetadata(Media.Colors.White,
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

        protected override void OnRender(RenderContext renderContext)
        {
            // --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            // --- set material params      
            this.effectMaterial.AttachMaterial();

            this.bHasInstances.Set(this.hasInstances);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;
            if (this.hasInstances)
            {
                // --- update instance buffer
                if (this.isInstanceChanged)
                {
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.instanceInternal);
                    this.isInstanceChanged = false;
                }

                // --- INSTANCING: need to set 2 buffers            
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new[]
                {
                    new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0),
                    new VertexBufferBinding(this.InstanceBuffer.Buffer, this.InstanceBuffer.StructureSize, 0),
                });

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

                // --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexedInstanced(this.geometryInternal.Indices.Count, this.instanceInternal.Count, 0, 0, 0);
                this.bHasInstances.Set(false);
            }
            else
            {
                // --- bind buffer                
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));

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

                // --- render the geometry
                // 
                var pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(renderContext.DeviceContext);
                // --- draw
                renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);

            }
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

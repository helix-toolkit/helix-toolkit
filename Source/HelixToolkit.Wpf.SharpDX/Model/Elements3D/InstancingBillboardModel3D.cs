using System.Windows;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using HelixToolkit.Wpf.SharpDX.Utilities;
using System;
using HelixToolkit.Wpf.SharpDX.Core;

namespace HelixToolkit.Wpf.SharpDX
{
    public class InstancingBillboardModel3D : MaterialGeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<BillboardInstanceParameter>), typeof(InstancingBillboardModel3D), 
                new AffectsRenderPropertyMetadata(null, InstancesParamChanged));

        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(InstancingBillboardModel3D),
            new AffectsRenderPropertyMetadata(true));

        /// <summary>
        /// List of instance parameters. 
        /// </summary>
        public IList<BillboardInstanceParameter> InstanceParamArray
        {
            get { return (IList<BillboardInstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }

        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public bool FixedSize
        {
            set
            {
                SetValue(FixedSizeProperty, value);
            }
            get
            {
                return (bool)GetValue(FixedSizeProperty);
            }
        }
        #endregion

        #region Static Methods
        private static void InstancesParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingBillboardModel3D)d;
            model.InstancesParamChanged();
        }
        #endregion
        #region Private Class Data Members
        [ThreadStatic]
        private static BillboardVertex[] vertexArrayBuffer;
        private readonly ImmutableBufferProxy<BillboardVertex> vertexBuffer = new ImmutableBufferProxy<BillboardVertex>(BillboardVertex.SizeInBytes, BindFlags.VertexBuffer);
        private EffectVectorVariable vViewport;
        private EffectScalarVariable bHasBillboardTexture;
        private ShaderResourceView billboardTextureView;
        private ShaderResourceView billboardAlphaTextureView;
        private EffectShaderResourceVariable billboardTextureVariable;
        private EffectShaderResourceVariable billboardAlphaTextureVariable;
        private EffectScalarVariable bHasBillboardAlphaTexture;
        private BillboardType billboardType;

        protected readonly DynamicBufferProxy<BillboardInstanceParameter> instanceParamBuffer = new DynamicBufferProxy<BillboardInstanceParameter>(BillboardInstanceParameter.SizeInBytes, BindFlags.VertexBuffer);
        protected bool instanceParamArrayChanged = true;
        protected bool hasInstanceParams = false;
        private EffectScalarVariable hasInstanceParamVar;

        private EffectScalarVariable bFixedSizeVariable;
        public bool HasInstanceParams { get { return hasInstanceParams; } }
        #endregion


        protected void InstancesParamChanged()
        {
            hasInstanceParams = (InstanceParamArray != null && InstanceParamArray.Any());
            instanceParamArrayChanged = true;
        }

        #region Overridable Methods
        protected override GeometryBufferModel OnCreateBufferModel()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override bool CanHitTest(IRenderMatrices context)
        {
            //Implementation pending.
            return false;
        }

        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            return false;
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardInstancing];
        }

        protected override bool CheckGeometry()
        {
            return geometryInternal is IBillboardText;
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = +0,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = false,
                //IsAntialiasedLineEnabled = true,                    
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
        }

        protected void OnCreateGeometryBuffers()
        {
            if (geometryInternal != null && geometryInternal.Positions != null)
            {
                vertexBuffer.CreateBufferFromDataArray(this.Device, CreateBillboardVertexArray(), geometryInternal.Positions.Count);
            }
            Disposer.RemoveAndDispose(ref billboardTextureView);
            Disposer.RemoveAndDispose(ref billboardAlphaTextureView);
            var geometry = geometryInternal as IBillboardText;
            if (geometry != null)
            {
                if (geometry.Texture != null)
                {
                    var textureBytes = geometry.Texture.ToByteArray();
                    billboardTextureView = TextureLoader.FromMemoryAsShaderResourceView(Device, textureBytes);
                }
                if (geometry.AlphaTexture != null)
                {
                    billboardAlphaTextureView = global::SharpDX.Toolkit.Graphics.Texture.Load(Device, geometry.AlphaTexture);
                }
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }

            bFixedSizeVariable = effect.GetVariableByName("bBillboardFixedSize").AsScalar();
            // --- shader variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();

            // --- get geometry
            var geometry = geometryInternal as IBillboardText;
            if (geometry == null)
            {
                throw new System.Exception("Geometry must implement IBillboardText");
            }
            // -- set geometry if given
            
            // --- material 
            // this.AttachMaterial();
            this.bHasBillboardTexture = effect.GetVariableByName("bHasTexture").AsScalar();
            this.billboardTextureVariable = effect.GetVariableByName("billboardTexture").AsShaderResource();

            this.billboardAlphaTextureVariable = effect.GetVariableByName("billboardAlphaTexture").AsShaderResource();
            this.bHasBillboardAlphaTexture = effect.GetVariableByName("bHasAlphaTexture").AsScalar();

            instanceParamArrayChanged = true;
            hasInstanceParamVar = effect.GetVariableByName("bHasInstanceParams").AsScalar();
            OnCreateGeometryBuffers();
            // --- set rasterstate
            OnRasterStateChanged();

            // --- flush
            //Device.ImmediateContext.Flush();
            return true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            InstancesParamChanged();
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref vViewport);
            Disposer.RemoveAndDispose(ref billboardTextureVariable);
            Disposer.RemoveAndDispose(ref billboardTextureView);
            Disposer.RemoveAndDispose(ref billboardAlphaTextureVariable);
            Disposer.RemoveAndDispose(ref billboardAlphaTextureView);
            Disposer.RemoveAndDispose(ref bHasBillboardAlphaTexture);
            Disposer.RemoveAndDispose(ref bHasBillboardTexture);

            instanceParamBuffer.Dispose();
            Disposer.RemoveAndDispose(ref hasInstanceParamVar);
            Disposer.RemoveAndDispose(ref bFixedSizeVariable);
            base.OnDetach();
        }

        protected override void OnRender(RenderContext renderContext)
        {
            /*
            // --- check to render the model
            var geometry = geometryInternal as IBillboardText;
            if (geometry == null)
            {
                throw new System.Exception("Geometry must implement IBillboardText");
            }
            this.bHasInstances.Set(this.hasInstances);
            this.hasInstanceParamVar.Set(this.hasInstanceParams);
            // --- set constant paramerers             
            var worldMatrix = modelMatrix * renderContext.worldMatrix;
            EffectTransforms.World.SetMatrix(ref worldMatrix);
            this.bFixedSizeVariable.Set(FixedSize);
            // --- check shadowmaps
            //this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            //this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = RasterState;

            // --- bind buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, 0));
            // --- render the geometry
            this.bHasBillboardTexture.Set(geometry.Texture != null);
            if (geometry.Texture != null)
            {
                billboardTextureVariable.SetResource(billboardTextureView);
            }

            this.bHasBillboardAlphaTexture.Set(geometry.AlphaTexture != null);
            if (geometry.AlphaTexture != null)
            {
                billboardAlphaTextureVariable.SetResource(billboardAlphaTextureView);
            }
            var vertexCount = geometryInternal.Positions.Count;
            if (this.hasInstances)
            {
                if (this.isInstanceChanged)
                {
                    InstanceBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.instanceInternal);
                    this.isInstanceChanged = false;
                }
                renderContext.DeviceContext.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this.InstanceBuffer.Buffer, this.InstanceBuffer.StructureSize, 0));
                if (this.hasInstanceParams)
                {
                    if (instanceParamArrayChanged)
                    {
                        instanceParamBuffer.UploadDataToBuffer(renderContext.DeviceContext, this.InstanceParamArray);
                        this.instanceParamArrayChanged = false;
                    }
                    renderContext.DeviceContext.InputAssembler.SetVertexBuffers(2, new VertexBufferBinding(this.instanceParamBuffer.Buffer, this.instanceParamBuffer.StructureSize, 0));
                }

                switch (billboardType)
                {
                    case BillboardType.SingleImage:
                        // Use foreground shader to draw text
                        effectTechnique.GetPassByIndex(2).Apply(renderContext.DeviceContext);

                        // --- draw text, foreground vertex is beginning from 0.
                        renderContext.DeviceContext.DrawInstanced(vertexCount, this.instanceInternal.Count, 0, 0);
                        break;
                    case BillboardType.SingleText:
                        if (vertexCount == 8)
                        {
                            var half = vertexCount / 2;
                            // Use background shader to draw background first
                            effectTechnique.GetPassByIndex(1).Apply(renderContext.DeviceContext);
                            // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                            renderContext.DeviceContext.DrawInstanced(half, this.instanceInternal.Count, half, 0);

                            // Use foreground shader to draw text
                            effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);

                            // --- draw text, foreground vertex is beginning from 0.
                            renderContext.DeviceContext.DrawInstanced(half, this.instanceInternal.Count, 0, 0);
                        }
                        break;
                }
            }
            this.bHasInstances.Set(false);
            this.hasInstanceParamVar.Set(false);
            */
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray()
        {
            var billboardGeometry = geometryInternal as IBillboardText;

            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            billboardType = billboardGeometry.Type;
            billboardGeometry.DrawTexture();

            var position = billboardGeometry.Positions;
            var vertexCount = billboardGeometry.Positions.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new BillboardVertex[vertexCount];
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }
            var allOffsets = billboardGeometry.TextureOffsets;
            for (var i = 0; i < vertexCount; i++)
            {
                var tc = billboardGeometry.TextureCoordinates[i];
                array[i].Position = new Vector4(position[i], 1.0f);
                array[i].Color = billboardGeometry.Colors[i];
                array[i].TexCoord = new Vector4(tc.X, tc.Y, allOffsets[i].X, allOffsets[i].Y);
            }

            return array;
        }

        #endregion
    }
}

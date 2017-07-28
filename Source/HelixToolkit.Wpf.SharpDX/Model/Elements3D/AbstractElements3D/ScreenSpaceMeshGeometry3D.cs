// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Linq;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Base class for screen space rendering, such as Coordinate System or ViewBox
    /// </summary>
    public abstract class ScreenSpaceMeshGeometry3D : MeshGeometryModel3D
    {        
        /// <summary>
        /// <see cref="RelativeScreenLocationX"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(float), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(-0.8f,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).projectionMatrix.M41 = (float)e.NewValue;
                }));
        /// <summary>
        /// <see cref="RelativeScreenLocationY"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(float), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(-0.8f,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).projectionMatrix.M42 = (float)e.NewValue;
                }));
        /// <summary>
        /// <see cref="SizeScale"/>
        /// </summary>
        public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(float), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(1f,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).OnCreateProjectionMatrix((float)e.NewValue);
                }));

        /// <summary>
        /// Relative Location X on screen. Range from -1~1
        /// </summary>
        public float RelativeScreenLocationX
        {
            set
            {
                SetValue(RelativeScreenLocationXProperty, value);
            }
            get
            {
                return (float)GetValue(RelativeScreenLocationXProperty);
            }
        }

        /// <summary>
        /// Relative Location Y on screen. Range from -1~1
        /// </summary>
        public float RelativeScreenLocationY
        {
            set
            {
                SetValue(RelativeScreenLocationYProperty, value);
            }
            get
            {
                return (float)GetValue(RelativeScreenLocationYProperty);
            }
        }

        /// <summary>
        /// Size scaling
        /// </summary>
        public float SizeScale
        {
            set
            {
                SetValue(SizeScaleProperty, value);
            }
            get
            {
                return (float)GetValue(SizeScaleProperty);
            }
        }

        protected EffectMatrixVariable viewMatrixVar;
        protected EffectMatrixVariable projectionMatrixVar;
        protected Matrix projectionMatrix;
        protected DepthStencilState depthStencil;
        protected float screenRatio = 1f;
        protected override bool CanHitTest(IRenderMatrices context)
        {
            return false;
        }

        protected virtual void OnCreateProjectionMatrix(float scale)
        {
            projectionMatrix = Matrix.OrthoRH(140 * screenRatio / scale, 140 / scale, 0.1f, 200);
            projectionMatrix.M41 = RelativeScreenLocationX;
            projectionMatrix.M42 = RelativeScreenLocationY;
        }
        
        protected void UpdateProjectionMatrix(double width, double height)
        {
            var ratio = (float)(width / height);
            if (screenRatio != ratio)
            {
                screenRatio = ratio;
                OnCreateProjectionMatrix(SizeScale);
            }
        }        

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                viewMatrixVar = effect.GetVariableByName("mView").AsMatrix();
                projectionMatrixVar = effect.GetVariableByName("mProjection").AsMatrix();
                Disposer.RemoveAndDispose(ref depthStencil);
                depthStencil = CreateDepthStencilState(this.Device);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual DepthStencilState CreateDepthStencilState(global::SharpDX.Direct3D11.Device device)
        {
            return new DepthStencilState(device, new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false });
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref viewMatrixVar);
            Disposer.RemoveAndDispose(ref projectionMatrixVar);
            Disposer.RemoveAndDispose(ref depthStencil);
            base.OnDetach();
        }
        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return true;
        }
        protected override void OnRender(RenderContext renderContext)
        {
            UpdateProjectionMatrix(renderContext.ActualWidth, renderContext.ActualHeight);
            // --- set constant paramerers             
            var worldMatrix = renderContext.worldMatrix;
            worldMatrix.Row4 = new Vector4(0, 0, 0, 1);
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);
            this.viewMatrixVar.SetMatrix(CreateViewMatrix(renderContext));
            this.projectionMatrixVar.SetMatrix(projectionMatrix);
            this.effectMaterial.bHasShadowMapVariable.Set(false);

            // --- set material params      
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);

            this.bHasInstances.Set(false);
            int depthStateRef;
            var depthStateBack = renderContext.DeviceContext.OutputMerger.GetDepthStencilState(out depthStateRef);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;

            // --- bind buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));

            var pass = this.effectTechnique.GetPassByIndex(0);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencil);
            // --- draw
            renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);            

            this.viewMatrixVar.SetMatrix(renderContext.ViewMatrix);
            this.projectionMatrixVar.SetMatrix(renderContext.ProjectionMatrix);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStateBack);
        }

        protected Matrix CreateViewMatrix(RenderContext renderContext)
        {
            return global::SharpDX.Matrix.LookAtRH(
                -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20,
                Vector3.Zero,
                renderContext.Camera.UpDirection.ToVector3());
        }
    }
}

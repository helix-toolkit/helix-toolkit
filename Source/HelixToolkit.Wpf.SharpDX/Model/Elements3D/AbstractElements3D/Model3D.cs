// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Model3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class for a scene model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;
    using global::SharpDX.Direct3D11;

    /// <summary>
    /// Provides a base class for a scene model
    /// </summary>
    public abstract class Model3D : Element3D, ITransformable
    {
        /// <summary>
        /// This is a hack model matrix. It is always pushed but
        /// never poped. It can be used to get the total model matrix
        /// in functions different than render or hittext, e.g., OnMouse3DMove.
        /// This is a temporar solution, until we have more time to think how to make it better.
        /// </summary>
        protected Matrix totalModelMatrix = Matrix.Identity;

        protected Matrix modelMatrix = Matrix.Identity;

        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        public void PushMatrix(Matrix matrix)
        {
            matrixStack.Push(this.modelMatrix);
            this.modelMatrix = this.modelMatrix * matrix;
            this.totalModelMatrix = this.modelMatrix;
        }

        public void PopMatrix()
        {
            this.modelMatrix = matrixStack.Pop();
        }

        public Matrix ModelMatrix
        {
            get { return this.modelMatrix; }
        }

        public Matrix TotalModelMatrix
        {
            get { return this.totalModelMatrix; }
        }

        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Model3D), new FrameworkPropertyMetadata(Transform3D.Identity, FrameworkPropertyMetadataOptions.AffectsRender, TransformPropertyChanged));

        protected static void TransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Model3D)d).OnTransformChanged(e);
        }

        protected virtual void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Transform != null)
            {
                var trafo = this.Transform.Value;                
                this.modelMatrix = trafo.ToMatrix();
            }
        }

        public class EffectTransformVariables : System.IDisposable
        {
            public EffectTransformVariables(Effect effect)
            {
                // openGL: uniform variables            
                mWorld = effect.GetVariableByName("mWorld").AsMatrix();
            }
            public EffectMatrixVariable mWorld;            
            public void Dispose()
            {
                mWorld.Dispose();
            }
        }        
    }
}
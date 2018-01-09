/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    public class Element3DCore : DisposeObject, ITreeNode, IDisposable, IHitable
    {        
        /// <summary>
        /// 
        /// </summary>
        public Guid GUID { get { return Guid.NewGuid(); } }

        private Matrix totalMatrixTransform = Matrix.Identity;
        /// <summary>
        /// 
        /// </summary>
        public Matrix TotalModelMatrix
        {
            private set
            {
                if(Set(ref totalMatrixTransform, value))
                {
                    OnTransformChanged(ref value);
                }
            }
            get
            {
                return totalMatrixTransform;
            }
        } 

        protected bool needMatrixUpdate { private set; get; } = true;

        private Matrix modelMatrix = Matrix.Identity;
        /// <summary>
        /// 
        /// </summary>
        public Matrix ModelMatrix
        {
            set
            {
                if(Set(ref modelMatrix, value))
                {
                    needMatrixUpdate = true;
                    InvalidateRender();
                }
            }
            get { return modelMatrix; }
        } 


        private Matrix parentMatrix = Matrix.Identity;
        public Matrix ParentMatrix
        {
            set
            {
                if(Set(ref parentMatrix, value))
                {
                    needMatrixUpdate = true;
                }
            }
            get
            {
                return parentMatrix;
            }
        }

        private bool visible = true;
        public bool Visible
        {
            set
            {
                if(Set(ref visible, value))
                { InvalidateRender(); }
            }
            get { return visible; }
        }

        public bool IsVisible { private set; get; } = true;
        /// <summary>
        /// If this has been attached onto renderhost. 
        /// </summary>
        public bool IsAttached
        {
            get { return renderHost != null; }
        }

        private IRenderHost renderHost;
        /// <summary>
        /// 
        /// </summary>
        public IRenderHost RenderHost
        {
            get { return renderHost; }
        }

        public virtual IEnumerable<ITreeNode> Items
        {
            get
            {
                return System.Linq.Enumerable.Empty<ITreeNode>();
            }
        }

        public bool IsHitTestVisible { set; get; } = true;
        #region Handling Transforms
        protected virtual void OnTransformChanged(ref Matrix totalTransform) { }
        #endregion
        /// <summary>
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            renderHost = host;
            InvalidateRender();
        }

        /// <summary>
        /// Detaches the element from the host. Override <see cref="OnDetach"/>
        /// </summary>
        public void Detach()
        {
            OnDetach();
        }

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected virtual void OnDetach()
        {
            renderHost = null;
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            renderHost?.InvalidateRender();
        }

        /// <summary>
        /// Updates the element by the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time since last update.</param>
        public virtual void Update(IRenderContext context)
        {
            if (needMatrixUpdate)
            {
                TotalModelMatrix = modelMatrix * parentMatrix;
                needMatrixUpdate = false;
            }
            IsVisible = DetermineVisibility(context);
        }
        /// <summary>
        /// Determine actual visibility for this model. Such as determine by view frustum, etc.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetermineVisibility(IRenderContext context)
        {
            return Visible;
        }

        public virtual bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits, IRenderable originalSource)
        {
            if (CanHit(context))
            {
                return OnHitTest(context, TotalModelMatrix, ref ray, ref hits, originalSource);
            }
            else
            {
                return false;
            }
        }

        protected virtual bool CanHit(IRenderContext context)
        {
            return IsHitTestVisible && IsVisible;
        }

        protected virtual bool OnHitTest(IRenderContext context, Matrix modelMatrix, ref Ray ray, ref List<HitTestResult> hits, IRenderable originalSource)
        { return false; }
    }
}

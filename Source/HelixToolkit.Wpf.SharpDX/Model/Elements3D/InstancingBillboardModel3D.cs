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
    public class InstancingBillboardModel3D : BillboardTextModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<BillboardInstanceParameter>), typeof(InstancingBillboardModel3D), 
                new AffectsRenderPropertyMetadata(null, InstancesParamChanged));

        /// <summary>
        /// List of instance parameters. 
        /// </summary>
        public IList<BillboardInstanceParameter> InstanceParamArray
        {
            get { return (IList<BillboardInstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }
        #endregion

        private static void InstancesParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (InstancingBillboardModel3D)d;
            model.instanceParamBuffer.Elements = e.NewValue as IList<BillboardInstanceParameter>;
        }

        protected IElementsBufferModel<BillboardInstanceParameter> instanceParamBuffer = new InstanceParamsBufferModel<BillboardInstanceParameter>(BillboardInstanceParameter.SizeInBytes);
        #region Overridable Methods

        protected override IRenderCore OnCreateRenderCore()
        {
            return new InstancingBillboardRenderCore() { ParameterBuffer = this.instanceParamBuffer };
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

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }
            instanceParamBuffer.Initialize(effect);
            return true;
        }

        protected override void OnDetach()
        {
            instanceParamBuffer.Dispose();
            base.OnDetach();
        }

        #endregion
    }
}

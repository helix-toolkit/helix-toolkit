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
                new PropertyMetadata(null, InstancesParamChanged));

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

        /// <summary>
        /// The instance parameter buffer
        /// </summary>
        protected IElementsBufferModel<BillboardInstanceParameter> instanceParamBuffer = new InstanceParamsBufferModel<BillboardInstanceParameter>(BillboardInstanceParameter.SizeInBytes);
        #region Overridable Methods

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new InstancingBillboardRenderCore() { ParameterBuffer = this.instanceParamBuffer };
        }


        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
        {
            //Implementation pending.
            return false;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="Element3DCore.OnSetRenderTechnique" /> is set, then <see cref="Element3DCore.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BillboardInstancing];
        }
        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return true if attached
        /// </returns>
        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }
            instanceParamBuffer.Initialize();
            return true;
        }
        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected override void OnDetach()
        {
            instanceParamBuffer.DisposeAndClear();
            base.OnDetach();
        }

        #endregion
    }
}

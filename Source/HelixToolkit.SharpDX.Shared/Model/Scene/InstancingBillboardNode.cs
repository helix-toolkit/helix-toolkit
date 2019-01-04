/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using System.Diagnostics;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;

        /// <summary>
        /// 
        /// </summary>
        public class InstancingBillboardNode : BillboardNode
        {
            /// <summary>
            /// Gets or sets the instance parameter array.
            /// </summary>
            /// <value>
            /// The instance parameter array.
            /// </value>
            public IList<BillboardInstanceParameter> InstanceParamArray
            {
                set { instanceParamBuffer.Elements = value; }
                get { return instanceParamBuffer.Elements; }
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
            protected override RenderCore OnCreateRenderCore()
            {
                return new InstancingBillboardRenderCore() { ParameterBuffer = this.instanceParamBuffer };
            }

            /// <summary>
            /// Override this function to set render technique during Attach Host.
            /// <para>If <see cref="SceneNode.OnSetRenderTechnique" /> is set, then <see cref="SceneNode.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
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

}

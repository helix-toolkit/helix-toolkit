/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Cameras;
    using Core;

    /// <summary>
    /// 
    /// </summary>
    public class ShadowMapNode : SceneNode
    {
        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public Size2 Resolution
        {
            get { return new Size2((RenderCore as ShadowMapCore).Width, (RenderCore as ShadowMapCore).Height); }
            set { (RenderCore as ShadowMapCore).Width = value.Width; (RenderCore as ShadowMapCore).Height = value.Height; }
        }

        /// <summary>
        ///
        /// </summary>
        public float Bias
        {
            get { return (RenderCore as ShadowMapCore).Bias; }
            set { (RenderCore as ShadowMapCore).Bias = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public float Intensity
        {
            get { return (RenderCore as ShadowMapCore).Intensity; }
            set { (RenderCore as ShadowMapCore).Intensity = value; }
        }

        /// <summary>
        /// Distance of the directional light from origin
        /// </summary>
        public ProjectionCameraCore LightCamera
        {
            set; get;
        } = null;

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            var core = new ShadowMapCore();
            core.OnUpdateLightSource += Core_OnUpdateLightSource;
            return core;
        }

        private ShadowMapCore shadowCore;

        private readonly OrthographicCameraCore orthoCamera = new OrthographicCameraCore() { NearPlaneDistance = 1, FarPlaneDistance = 500 };
        private readonly PerspectiveCameraCore persCamera = new PerspectiveCameraCore() { NearPlaneDistance = 1, FarPlaneDistance = 500 };

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            var c = core as ShadowMapCore;
            //c.FactorPCF = (float)FactorPCF;
            c.Intensity = (float)Intensity;
            c.Bias = (float)Bias;
            c.Width = (int)(Resolution.Width);
            c.Height = (int)(Resolution.Height);
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
            base.OnAttach(host);
            shadowCore = RenderCore as ShadowMapCore;
            return true;
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && RenderHost.IsShadowMapEnabled && !context.IsShadowPass;
        }

        private void Core_OnUpdateLightSource(object sender, ShadowMapCore.UpdateLightSourceEventArgs e)
        {
            CameraCore camera = LightCamera == null ? null : LightCamera;
            if (LightCamera == null)
            {
                var lights = e.Context.RenderHost.PerFrameLights.Take(Constants.MaxLights);
                foreach (var light in lights)
                {
                    if (light.LightType == LightType.Directional)
                    {
                        var dlight = light as DirectionalLightCore;
                        var dir = Vector4.Transform(dlight.Direction.ToVector4(0), dlight.ModelMatrix).Normalized();
                        var pos = -100 * dir;
                        orthoCamera.LookDirection = new Vector3(dir.X, dir.Y, dir.Z);
                        orthoCamera.Position = new Vector3(pos.X, pos.Y, pos.Z);
                        orthoCamera.UpDirection = Vector3.UnitZ;
                        orthoCamera.Width = 50;
                        camera = orthoCamera;
                        break;
                    }
                    else if (light.LightType == LightType.Spot)
                    {
                        var splight = light as SpotLightCore;
                        persCamera.Position = (splight.Position + splight.ModelMatrix.Row4.ToVector3());
                        var look = Vector4.Transform(splight.Direction.ToVector4(0), splight.ModelMatrix);
                        persCamera.LookDirection = new Vector3(look.X, look.Y, look.Z);
                        persCamera.FarPlaneDistance = (float)splight.Range;
                        persCamera.FieldOfView = (float)splight.OuterAngle;
                        persCamera.UpDirection = Vector3.UnitZ;
                        camera = persCamera;
                        break;
                    }
                }
            }
            if (camera == null)
            {
                shadowCore.FoundLightSource = false;
            }
            else
            {
                shadowCore.FoundLightSource = true;
                shadowCore.LightViewProjectMatrix = camera.CreateViewMatrix() * camera.CreateProjectionMatrix(shadowCore.Width / shadowCore.Height);
            }
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
        /// <exception cref="System.NotImplementedException"></exception>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
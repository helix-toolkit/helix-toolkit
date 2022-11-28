/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;
using System.Linq;
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
                get
                {
                    return new Size2((RenderCore as ShadowMapCore).Width, (RenderCore as ShadowMapCore).Height);
                }
                set
                {
                    (RenderCore as ShadowMapCore).Width = value.Width;
                    (RenderCore as ShadowMapCore).Height = value.Height;
                }
            }

            /// <summary>
            ///
            /// </summary>
            public float Bias
            {
                get
                {
                    return (RenderCore as ShadowMapCore).Bias;
                }
                set
                {
                    (RenderCore as ShadowMapCore).Bias = value;
                }
            }

            /// <summary>
            ///
            /// </summary>
            public float Intensity
            {
                get
                {
                    return (RenderCore as ShadowMapCore).Intensity;
                }
                set
                {
                    (RenderCore as ShadowMapCore).Intensity = value;
                }
            }

            private float distance = 200;
            public float Distance
            {
                set
                {
                    SetAffectsRender(ref distance, value);
                }
                get
                {
                    return distance;
                }
            }

            private float orthoWidth = 100;
            public float OrthoWidth
            {
                set
                {
                    SetAffectsRender(ref orthoWidth, value);
                }
                get
                {
                    return orthoWidth;
                }
            }

            private float farField = 500;
            /// <summary>
            /// Gets or sets the far field.
            /// </summary>
            /// <value>
            /// The far field.
            /// </value>
            public float FarField
            {
                set
                {
                    if (SetAffectsRender(ref farField, value))
                    {
                        orthoCamera.FarPlaneDistance = value;
                        persCamera.FarPlaneDistance = value;
                    }
                }
                get
                {
                    return farField;
                }
            }

            private float nearField = 500;
            /// <summary>
            /// Gets or sets the near field.
            /// </summary>
            /// <value>
            /// The far field.
            /// </value>
            public float NearField
            {
                set
                {
                    if (SetAffectsRender(ref nearField, value))
                    {
                        orthoCamera.NearPlaneDistance = value;
                        persCamera.NearPlaneDistance = value;
                    }
                }
                get
                {
                    return nearField;
                }
            }

            private ProjectionCameraCore lightCamera = null;
            /// <summary>
            /// Distance of the directional light from origin
            /// </summary>
            public ProjectionCameraCore LightCamera
            {
                set
                {
                    if (lightCamera != null)
                    {
                        lightCamera.PropertyChanged -= LightCamera_PropertyChanged;
                    }
                    SetAffectsRender(ref lightCamera, value);
                    if (lightCamera != null)
                    {
                        lightCamera.PropertyChanged += LightCamera_PropertyChanged;
                    }
                }
                get => lightCamera;
            }

            /// <summary>
            /// Gets or sets a value indicating whether shadow map should automatically cover complete scene. Only effective with directional light.
            /// <para>Limitation: Currently unable to properly cover BoneSkinned model animation.</para>
            /// </summary>
            /// <value>
            ///   <c>true</c> if [automatic cover complete scene]; otherwise, <c>false</c>.
            /// </value>
            public bool AutoCoverCompleteScene
            {
                set; get;
            } = false;
            /// <summary>
            /// Gets or sets a value indicating whether the scene is dynamic. Only effective if <see cref="AutoCoverCompleteScene"/> is true.
            /// <para>Setting to true will force shadow map to update the shadow camera for each frame. May impact the performance.</para>
            /// </summary>
            /// <value>
            ///   <c>true</c> if scene is dynamic; otherwise, <c>false</c>.
            /// </value>
            public bool IsSceneDynamic
            {
                set; get;
            } = false;
            /// <summary>
            /// Gets or sets the shadow cast scene scale. Only effective if <see cref="AutoCoverCompleteScene"/> is true.
            /// <para>
            /// This is used if the mesh render shadow is much bigger than the meshes casting shadow.
            /// The shadow cast camera has to cover the shadow rendering region, otherwise the shadow maybe cut off.
            /// Increase the value to increase the shadow cast camera rendering region.
            /// </para>
            /// </summary>
            /// <value>
            /// Region scale for shadow cast.
            /// </value>
            public float CastSceneScale
            {
                set; get;
            } = 2f;
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
            private bool sceneChanged = false;
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

            private void LightCamera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                InvalidateRender();
            }

            /// <summary>
            /// To override Attach routine, please override this.
            /// </summary>
            /// <param name="effectsManager"></param>
            /// <returns>
            /// Return true if attached
            /// </returns>
            protected override bool OnAttach(IEffectsManager effectsManager)
            {
                base.OnAttach(effectsManager);
                shadowCore = RenderCore as ShadowMapCore;
                this.Invalidated += Host_SceneGraphUpdated;
                sceneChanged = true;
                return true;
            }

            protected override void OnDetach()
            {
                this.Invalidated -= Host_SceneGraphUpdated;
                base.OnDetach();
            }

            private void Host_SceneGraphUpdated(object sender, InvalidateTypes type)
            {
                if (type == InvalidateTypes.SceneGraph)
                {
                    sceneChanged = true;
                }
            }

            /// <summary>
            /// <para>Determine if this can be rendered.</para>
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            protected override bool CanRender(RenderContext context)
            {
                (RenderCore as ShadowMapCore).NeedRender = base.CanRender(context) && context.RenderHost.IsShadowMapEnabled;
                return true;
            }

            private BoundingBox FindSceneBound(FastList<SceneNode> nodes) 
            {
                var box = new BoundingBox();
                if (nodes.Count > 0)
                {
                    foreach (var node in nodes.Where(x => x is IThrowingShadow k && k.IsThrowingShadow))
                    {
                        if (node.BoundsWithTransform.Minimum == node.BoundsWithTransform.Maximum)
                        {
                            continue;
                        }
                        if (box.Minimum == box.Maximum)
                        {
                            box = node.BoundsWithTransform;
                        }
                        else
                        {
                            box = BoundingBox.Merge(box, node.BoundsWithTransform);
                        }
                    }
                }
                return box;
            }

            private unsafe bool CreateCameraFromBound(ref BoundingBox box, ref Vector3 lookDir)
            {
                if (box.Maximum == box.Minimum)
                {
                    return false;
                }
                var center = box.Center;
                var dist = 0.0f;
                var points = stackalloc Vector3[8];
                points[0] = box.Minimum;
                points[1] = box.Maximum;
                points[2] = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z);
                points[3] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z);
                points[4] = new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z);
                points[5] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z);
                points[6] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z);
                points[7] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z);
                var plane = new Plane(center, lookDir);
                var farthest = Vector3.Zero;
                var farestDist = 0f;

                for (var i = 0; i < 8; ++i)
                {
                    Vector3.Dot(ref plane.Normal, ref points[i], out var dot);
                    dot += plane.D;
                    if (dot > 0)
                    {
                        continue;
                    }
                    var t = dot - plane.D;
                    var v = points[i] - (t * plane.Normal);
                    var vDist = v.Length();
                    if (vDist > farestDist)
                    {
                        farthest = points[i];
                        farestDist = vDist;
                    }
                }

                dist = farestDist * CastSceneScale + 0.1f;
                var pos = center + (-lookDir * dist);
                orthoCamera.Position = pos;
                orthoCamera.LookDirection = center - pos;
                orthoCamera.Width = dist * 2;
                orthoCamera.NearPlaneDistance = 0.1f;
                orthoCamera.FarPlaneDistance = dist * 2;
                orthoCamera.UpDirection = lookDir.FindAnyPerpendicular();
                return true;
            }

            private void SetOrthoCameraParameters(ref Vector3 lookDir)
            {
                orthoCamera.LookDirection = lookDir * distance;
                orthoCamera.Position = -lookDir * distance;
                orthoCamera.UpDirection = Vector3.UnitZ;
                orthoCamera.Width = orthoWidth;
            }

            private void Core_OnUpdateLightSource(object sender, ShadowMapCore.UpdateLightSourceEventArgs e)
            {
                CameraCore camera = LightCamera ?? null;
                if (LightCamera == null)
                {
                    var lights = e.Context.RenderHost.PerFrameLights.Take(Constants.MaxLights);
                    foreach (var light in lights)
                    {
                        if (light.LightType == LightType.Directional)
                        {
                            var dlight = light.RenderCore as DirectionalLightCore;
                            var dir = Vector3.TransformNormal(dlight.Direction, dlight.ModelMatrix).Normalized();
                            if (AutoCoverCompleteScene)
                            {
                                if (sceneChanged || e.Context.UpdateSceneGraphRequested || IsSceneDynamic)
                                {
                                    sceneChanged = false;
                                    var boundingBox = FindSceneBound(e.Context.RenderHost.PerFrameOpaqueNodes);
                                    if (!CreateCameraFromBound(ref boundingBox, ref dir))
                                    {
                                        SetOrthoCameraParameters(ref dir);
                                    }
                                }
                            }
                            else
                            {
                                SetOrthoCameraParameters(ref dir);
                            }
                            camera = orthoCamera;
                            break;
                        }
                        else if (light.LightType == LightType.Spot)
                        {
                            var splight = light.RenderCore as SpotLightCore;
                            persCamera.Position = (splight.Position + splight.ModelMatrix.Row4.ToVector3());
                            var look = Vector3.TransformNormal(splight.Direction, splight.ModelMatrix);
                            persCamera.LookDirection = look;
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
                    shadowCore.LightView = camera.CreateViewMatrix();
                    shadowCore.LightProjection = camera.CreateProjectionMatrix(shadowCore.Width / shadowCore.Height);
                }
            }

            protected override bool CanHitTest(HitTestContext context)
            {
                return false;
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }
}
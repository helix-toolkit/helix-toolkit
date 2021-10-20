/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
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
    namespace Core
    {
        using Render;
        using Shaders;
        using Utilities;
        using Components;
        public class CrossSectionMeshRenderCore : MeshRenderCore, ICrossSectionRenderParams
        {
            #region Shader Variables
            private ShaderPass drawBackfacePass;
            private ShaderPass drawScreenQuadPass;
            /// <summary>
            /// Used to draw back faced triangles onto stencil buffer
            /// </summary>
            private RasterizerStateProxy backfaceRasterState;

            private readonly ConstantBufferComponent clipParamCB;

            private bool needsAssignVariables = true;
            #endregion
            #region Properties
            private CuttingOperation cuttingOperation = CuttingOperation.Intersect;
            /// <summary>
            /// Gets or sets the cutting operation.
            /// </summary>
            /// <value>
            /// The cutting operation.
            /// </value>
            public CuttingOperation CuttingOperation
            {
                set
                {
                    if (SetAffectsRender(ref cuttingOperation, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CuttingOperationStr, (int)value);
                    }
                }
                get
                {
                    return cuttingOperation;
                }
            }

            private Color4 sectionColor = Color.Green;
            /// <summary>
            /// Defines the sectionColor
            /// </summary>       
            public Color4 SectionColor
            {
                set
                {
                    if (SetAffectsRender(ref sectionColor, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossSectionColorStr, value);
                    }
                }
                get
                {
                    return sectionColor;
                }
            }

            private Bool4 planeEnabled;
            public Bool4 PlaneEnabled
            {
                set
                {
                    if (SetAffectsRender(ref planeEnabled, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.EnableCrossPlaneStr, value);
                    }
                }
                get
                {
                    return planeEnabled;
                }
            }

            private Bool4 plane5To8Enabled;
            public Bool4 Plane5To8Enabled
            {
                set
                {
                    if (SetAffectsRender(ref plane5To8Enabled, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.EnableCrossPlane5To8Str, value);
                    }
                }
                get
                {
                    return plane5To8Enabled;
                }
            }

            private Vector4 plane1Params;
            /// <summary>
            /// Defines the plane 1(Normal + d)
            /// </summary>
            public Vector4 Plane1Params
            {
                set
                {
                    if (SetAffectsRender(ref plane1Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane1ParamsStr, value);
                    }
                }
                get
                {
                    return plane1Params;
                }
            }

            private Vector4 plane2Params;
            /// <summary>
            /// Defines the plane 2(Normal + d)
            /// </summary>
            public Vector4 Plane2Params
            {
                set
                {
                    if (SetAffectsRender(ref plane2Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane2ParamsStr, value);
                    }
                }
                get
                {
                    return plane2Params;
                }
            }

            private Vector4 plane3Params;
            /// <summary>
            /// Defines the plane 3(Normal + d)
            /// </summary>
            public Vector4 Plane3Params
            {
                set
                {
                    if (SetAffectsRender(ref plane3Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane3ParamsStr, value);
                    }
                }
                get
                {
                    return plane3Params;
                }
            }

            private Vector4 plane4Params;
            /// <summary>
            /// Defines the plane 4(Normal + d)
            /// </summary>
            public Vector4 Plane4Params
            {
                set
                {
                    if (SetAffectsRender(ref plane4Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane4ParamsStr, value);
                    }
                }
                get
                {
                    return plane4Params;
                }
            }

            private Vector4 plane5Params;
            /// <summary>
            /// Defines the plane 5(Normal + d)
            /// </summary>
            public Vector4 Plane5Params
            {
                set
                {
                    if (SetAffectsRender(ref plane5Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane5ParamsStr, value);
                    }
                }
                get
                {
                    return plane5Params;
                }
            }

            private Vector4 plane6Params;
            /// <summary>
            /// Defines the plane 6(Normal + d)
            /// </summary>
            public Vector4 Plane6Params
            {
                set
                {
                    if (SetAffectsRender(ref plane6Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane6ParamsStr, value);
                    }
                }
                get
                {
                    return plane6Params;
                }
            }

            private Vector4 plane7Params;
            /// <summary>
            /// Defines the plane 7(Normal + d)
            /// </summary>
            public Vector4 Plane7Params
            {
                set
                {
                    if (SetAffectsRender(ref plane7Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane7ParamsStr, value);
                    }
                }
                get
                {
                    return plane7Params;
                }
            }

            private Vector4 plane8Params;
            /// <summary>
            /// Defines the plane 8(Normal + d)
            /// </summary>
            public Vector4 Plane8Params
            {
                set
                {
                    if (SetAffectsRender(ref plane8Params, value))
                    {
                        clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane8ParamsStr, value);
                    }
                }
                get
                {
                    return plane8Params;
                }
            }
            #endregion

            public CrossSectionMeshRenderCore()
            {
                clipParamCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.ClipParamsCB, ClipPlaneStruct.SizeInBytes)));
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                if (base.OnAttach(technique))
                {
                    needsAssignVariables = true;
                    drawBackfacePass = technique[DefaultPassNames.Backface];
                    drawScreenQuadPass = technique[DefaultPassNames.ScreenQuad];
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref backfaceRasterState);
                base.OnDetach();
            }

            protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
            {
                if (!base.CreateRasterState(description, force))
                {
                    return false;
                }
                #region Create states              
                var newRasterState = EffectTechnique.EffectsManager.StateManager.Register(new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Front,
                    DepthBias = description.DepthBias,
                    DepthBiasClamp = description.DepthBiasClamp,
                    SlopeScaledDepthBias = description.SlopeScaledDepthBias,
                    IsDepthClipEnabled = description.IsDepthClipEnabled,
                    IsFrontCounterClockwise = description.IsFrontCounterClockwise,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false
                });
                RemoveAndDispose(ref backfaceRasterState);
                backfaceRasterState = newRasterState;
                #endregion
                return true;
            }

            protected override void OnRender(RenderContext renderContext, DeviceContextProxy deviceContext)
            {
                if (needsAssignVariables)
                {
                    lock (clipParamCB)
                    {
                        if (needsAssignVariables)
                        {
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CuttingOperationStr, (int)cuttingOperation);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossSectionColorStr, sectionColor);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.EnableCrossPlaneStr, planeEnabled);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.EnableCrossPlane5To8Str, plane5To8Enabled);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane1ParamsStr, plane1Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane2ParamsStr, plane2Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane3ParamsStr, plane3Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane4ParamsStr, plane4Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane5ParamsStr, plane5Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane6ParamsStr, plane6Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane7ParamsStr, plane7Params);
                            clipParamCB.WriteValueByName(ClipPlaneStruct.CrossPlane8ParamsStr, plane8Params);
                            needsAssignVariables = false;
                        }
                    }
                }
                clipParamCB.Upload(deviceContext);
                base.OnRender(renderContext, deviceContext);
                // Draw backface into stencil buffer
                var dsView = renderContext.RenderHost.DepthStencilBufferView;
                deviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
                deviceContext.SetDepthStencil(dsView);//Remove render target
                deviceContext.SetRasterState(backfaceRasterState);
                drawBackfacePass.BindShader(deviceContext);
                drawBackfacePass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);

                //Draw full screen quad to fill cross section            
                deviceContext.SetRasterState(RasterState);
                drawScreenQuadPass.BindShader(deviceContext);
                drawScreenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                renderContext.RenderHost.SetDefaultRenderTargets(false);//Rebind render target
                deviceContext.Draw(4, 0);
            }
        }
    }
}

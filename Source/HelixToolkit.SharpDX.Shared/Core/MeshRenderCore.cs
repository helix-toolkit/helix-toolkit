/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D11;
using global::SharpDX;

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
        using Shaders;
        using Render;
        using Utilities;
        using Model;
    

        public class MeshRenderCore : GeometryRenderCore, IMeshRenderParams, IDynamicReflectable
        {
            #region Variables
            /// <summary>
            /// Gets the raster state wireframe.
            /// </summary>
            /// <value>
            /// The raster state wireframe.
            /// </value>
            protected RasterizerStateProxy RasterStateWireframe { get { return rasterStateWireframe; } }
            private RasterizerStateProxy rasterStateWireframe = null;

            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            public bool InvertNormal
            {
                set
                {
                    SetAffectsRender(ref modelStruct.InvertNormal, (value ? 1 : 0));
                }
                get
                {
                    return modelStruct.InvertNormal == 1 ? true : false;
                }
            }
            private bool renderWireframe = false;
            /// <summary>
            /// Gets or sets a value indicating whether [render wireframe].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
            /// </value>
            public bool RenderWireframe
            {
                set
                {
                    SetAffectsRender(ref renderWireframe, value);
                }
                get
                {
                    return renderWireframe;
                }
            }

            /// <summary>
            /// Gets or sets the color of the wireframe.
            /// </summary>
            /// <value>
            /// The color of the wireframe.
            /// </value>
            public Color4 WireframeColor
            {
                set
                {
                    SetAffectsRender(ref modelStruct.WireframeColor, value);
                }
                get { return modelStruct.WireframeColor; }
            }



            /// <summary>
            /// Gets or sets the dynamic reflector.
            /// </summary>
            /// <value>
            /// The dynamic reflector.
            /// </value>
            public IDynamicReflector DynamicReflector
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="MeshRenderCore"/> is batched.
            /// </summary>
            /// <value>
            ///   <c>true</c> if batched; otherwise, <c>false</c>.
            /// </value>
            public bool Batched
            {
                set; get;
            } = false;

            private MaterialVariable materialVariables = EmptyMaterialVariable.EmptyVariable;
            /// <summary>
            /// Used to wrap all material resources
            /// </summary>
            public MaterialVariable MaterialVariables
            {
                set
                {
                    if (SetAffectsCanRenderFlag(ref materialVariables, value))
                    {
                        materialVariables = materialVariables ?? EmptyMaterialVariable.EmptyVariable;
                    }
                }
                get
                {
                    return materialVariables;
                }
            }
            #endregion

            protected ModelStruct modelStruct = new ModelStruct() { World = Matrix.Identity };

            protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
            {
                if (base.CreateRasterState(description, force))
                {
                    RemoveAndDispose(ref rasterStateWireframe);
                    var wireframeDesc = description;
                    wireframeDesc.FillMode = FillMode.Wireframe;
                    wireframeDesc.DepthBias = -100;
                    wireframeDesc.SlopeScaledDepthBias = -2f;
                    wireframeDesc.DepthBiasClamp = -0.00008f;
                    rasterStateWireframe = Collect(EffectTechnique.EffectsManager.StateManager.Register(wireframeDesc));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override void OnDetach()
            {
                rasterStateWireframe = null;
                base.OnDetach();
            }

            protected override bool OnUpdateCanRenderFlag()
            {
                return base.OnUpdateCanRenderFlag() && materialVariables != EmptyMaterialVariable.EmptyVariable;
            }

            protected virtual void OnUpdatePerModelStruct(RenderContext context)
            {
                modelStruct.World = ModelMatrix;
                modelStruct.HasInstances = InstanceBuffer.HasElements ? 1 : 0;
                modelStruct.Batched = Batched ? 1 : 0;
            }

            protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
            {
                ShaderPass pass = MaterialVariables.GetPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                OnUpdatePerModelStruct(context);
                if(!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, ModelStruct.SizeInBytes))
                {
                    return;
                }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, DefaultStateBinding);
                if (!materialVariables.BindMaterialResources(context, deviceContext, pass))
                {
                    return;
                }

                DynamicReflector?.BindCubeMap(deviceContext);
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
                DynamicReflector?.UnBindCubeMap(deviceContext);

                if (RenderWireframe)
                {
                    pass = materialVariables.GetWireframePass(RenderType, context);
                    if (pass.IsNULL)
                    {
                        return;
                    }
                    pass.BindShader(deviceContext, false);
                    pass.BindStates(deviceContext, DefaultStateBinding);
                    deviceContext.SetRasterState(RasterStateWireframe);
                    materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
                }
            }

            protected override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (!materialVariables.UpdateMaterialStruct(deviceContext, ref modelStruct, ModelStruct.SizeInBytes))
                {
                    return;
                }
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
            }

            protected override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
            {
                var pass = materialVariables.GetShadowPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                var v = new SimpleMeshStruct()
                {
                    World = ModelMatrix,
                    HasInstances = InstanceBuffer.HasElements ? 1 : 0
                };
                if (!materialVariables.UpdateNonMaterialStruct(deviceContext, ref v, SimpleMeshStruct.SizeInBytes))
                {
                    return;
                }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, ShadowStateBinding);
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
            }

            protected override void OnRenderDepth(RenderContext context, DeviceContextProxy deviceContext, ShaderPass customPass)
            {
                var pass = customPass ?? materialVariables.GetDepthPass(RenderType, context);
                if (pass.IsNULL)
                { return; }
                var v = new SimpleMeshStruct()
                {
                    World = ModelMatrix,
                    HasInstances = InstanceBuffer.HasElements ? 1 : 0
                };
                if (!materialVariables.UpdateNonMaterialStruct(deviceContext, ref v, SimpleMeshStruct.SizeInBytes))
                {
                    return;
                }
                pass.BindShader(deviceContext);
                pass.BindStates(deviceContext, ShadowStateBinding);
                materialVariables.Draw(deviceContext, GeometryBuffer, InstanceBuffer.ElementCount);
            }
        }
    }

}

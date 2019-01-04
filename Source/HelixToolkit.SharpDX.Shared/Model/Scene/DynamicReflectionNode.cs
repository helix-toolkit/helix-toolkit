/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

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
        using Render;

        public class DynamicReflectionNode : GroupNode, IDynamicReflector
        {
            /// <summary>
            /// Gets or sets a value indicating whether [enable reflector].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable reflector]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableReflector
            {
                set
                {
                    (RenderCore as IDynamicReflector).EnableReflector = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).EnableReflector;
                }
            }
            /// <summary>
            /// Gets or sets the center.
            /// </summary>
            /// <value>
            /// The center.
            /// </value>
            public Vector3 Center
            {
                set
                {
                    (RenderCore as IDynamicReflector).Center = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).Center;
                }
            }

            /// <summary>
            /// Gets or sets the size of the face.
            /// </summary>
            /// <value>
            /// The size of the face.
            /// </value>
            public int FaceSize
            {
                set
                {
                    (RenderCore as IDynamicReflector).FaceSize = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).FaceSize;
                }
            }
            /// <summary>
            /// Gets or sets the near field.
            /// </summary>
            /// <value>
            /// The near field.
            /// </value>
            public float NearField
            {
                set
                {
                    (RenderCore as IDynamicReflector).NearField = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).NearField;
                }
            }
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
                    (RenderCore as IDynamicReflector).FarField = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).FarField;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether this coordinate system is left handed.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this coordinate system is left handed; otherwise, <c>false</c>.
            /// </value>
            public bool IsLeftHanded
            {
                set
                {
                    (RenderCore as IDynamicReflector).IsLeftHanded = value;
                }
                get
                {
                    return (RenderCore as IDynamicReflector).IsLeftHanded;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this scene is dynamic scene.
            /// If true, reflection map will be updated in each frame. Otherwise it will only be updated if scene graph or visibility changed.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is dynamic scene; otherwise, <c>false</c>.
            /// </value>
            public bool IsDynamicScene
            {
                set { (RenderCore as IDynamicReflector).IsDynamicScene = value; }
                get { return (RenderCore as IDynamicReflector).IsDynamicScene; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicReflectionNode"/> class.
            /// </summary>
            public DynamicReflectionNode()
            {
                this.ChildNodeAdded += DynamicReflectionNode_OnAddChildNode;
                this.ChildNodeRemoved += DynamicReflectionNode_OnRemoveChildNode;
                this.Cleared += DynamicReflectionNode_OnClear;
            }

            private void DynamicReflectionNode_OnClear(object sender, OnChildNodeChangedArgs e)
            {
                (RenderCore as DynamicCubeMapCore).IgnoredGuid.Clear();
            }

            private void DynamicReflectionNode_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
            {
                (RenderCore as DynamicCubeMapCore).IgnoredGuid.Remove(e.Node.RenderCore.GUID);
                if (e.Node is IDynamicReflectable dyn)
                {
                    dyn.DynamicReflector = null;
                }
            }

            private void DynamicReflectionNode_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
            {
                (RenderCore as DynamicCubeMapCore).IgnoredGuid.Add(e.Node.RenderCore.GUID);
                if(e.Node is IDynamicReflectable dyn)
                {
                    dyn.DynamicReflector = this;
                }
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return new DynamicCubeMapCore();
            }

            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    RenderCore.Attach(this.EffectTechnique);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void UpdateNotRender(RenderContext context)
            {
                base.UpdateNotRender(context);
                if(Octree != null)
                {
                    Center = Octree.Bound.Center();
                }
                else
                {
                    BoundingBox box = new BoundingBox();
                    int i = 0;
                    for(; i < ItemsInternal.Count; ++i)
                    {
                        if(ItemsInternal[i] is IDynamicReflectable)
                        {
                            box = ItemsInternal[i].BoundsWithTransform;
                            break;
                        }
                    }
                    for (; i < ItemsInternal.Count; ++i)
                    {
                        if (ItemsInternal[i] is IDynamicReflectable)
                        {
                            box = BoundingBox.Merge(box, ItemsInternal[i].BoundsWithTransform);
                        }
                    }
                    Center = box.Center();
                }
            }

            protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
            {
                return host.EffectsManager[DefaultRenderTechniqueNames.Skybox];
            }

            /// <summary>
            /// Binds the cube map.
            /// </summary>
            /// <param name="deviceContext">The device context.</param>
            public void BindCubeMap(DeviceContextProxy deviceContext)
            {
                (RenderCore as IDynamicReflector).BindCubeMap(deviceContext);
            }
            /// <summary>
            /// Uns the bind cube map.
            /// </summary>
            /// <param name="deviceContext">The device context.</param>
            public void UnBindCubeMap(DeviceContextProxy deviceContext)
            {
                (RenderCore as IDynamicReflector).UnBindCubeMap(deviceContext);
            }

            protected override bool CanRender(RenderContext context)
            {
                return base.CanRender(context);
            }
        }
    }

}

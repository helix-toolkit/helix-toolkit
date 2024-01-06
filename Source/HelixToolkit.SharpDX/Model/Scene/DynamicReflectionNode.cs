using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Render;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

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
            if (RenderCore is IDynamicReflector core)
            {
                core.EnableReflector = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.EnableReflector;
            }

            return false;
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
            if (RenderCore is IDynamicReflector core)
            {
                core.Center = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.Center;
            }

            return Vector3.Zero;
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
            if (RenderCore is IDynamicReflector core)
            {
                core.FaceSize = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.FaceSize;
            }

            return 0;
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
            if (RenderCore is IDynamicReflector core)
            {
                core.NearField = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.NearField;
            }

            return 0;
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
            if (RenderCore is IDynamicReflector core)
            {
                core.FarField = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.FarField;
            }

            return 1.0f;
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
            if (RenderCore is IDynamicReflector core)
            {
                core.IsLeftHanded = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.IsLeftHanded;
            }

            return false;
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
        set
        {
            if (RenderCore is IDynamicReflector core)
            {
                core.IsDynamicScene = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflector core)
            {
                return core.IsDynamicScene;
            }

            return false;
        }
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

    private void DynamicReflectionNode_OnClear(object? sender, OnChildNodeChangedArgs e)
    {
        (RenderCore as DynamicCubeMapCore)?.IgnoredGuid.Clear();
    }

    private void DynamicReflectionNode_OnRemoveChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        if (e.Node is not null)
        {
            (RenderCore as DynamicCubeMapCore)?.IgnoredGuid.Remove(e.Node.RenderCore.GUID);
        }

        if (e.Node is IDynamicReflectable dyn)
        {
            dyn.DynamicReflector = null;
        }
    }

    private void DynamicReflectionNode_OnAddChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        if (e.Node is not null)
        {
            (RenderCore as DynamicCubeMapCore)?.IgnoredGuid.Add(e.Node.RenderCore.GUID);
        }

        if (e.Node is IDynamicReflectable dyn)
        {
            dyn.DynamicReflector = this;
        }
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new DynamicCubeMapCore();
    }

    protected override bool OnAttach(IEffectsManager effectsManager)
    {
        if (base.OnAttach(effectsManager))
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
        if (Octree != null)
        {
            Center = Octree.Bound.Center;
        }
        else
        {
            var box = new BoundingBox();
            var i = 0;
            for (; i < ItemsInternal.Count; ++i)
            {
                if (ItemsInternal[i] is IDynamicReflectable)
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
            Center = box.Center;
        }
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.Skybox];
    }

    /// <summary>
    /// Binds the cube map.
    /// </summary>
    /// <param name="deviceContext">The device context.</param>
    public void BindCubeMap(DeviceContextProxy deviceContext)
    {
        (RenderCore as IDynamicReflector)?.BindCubeMap(deviceContext);
    }
    /// <summary>
    /// Uns the bind cube map.
    /// </summary>
    /// <param name="deviceContext">The device context.</param>
    public void UnBindCubeMap(DeviceContextProxy deviceContext)
    {
        (RenderCore as IDynamicReflector)?.UnBindCubeMap(deviceContext);
    }

    protected override bool CanRender(RenderContext context)
    {
        return base.CanRender(context);
    }
}

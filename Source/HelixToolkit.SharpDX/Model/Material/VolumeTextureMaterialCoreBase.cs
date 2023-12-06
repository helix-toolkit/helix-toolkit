using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Abstract class for VolumeTextureMaterial
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class VolumeTextureMaterialCoreBase<T> : MaterialCore, IVolumeTextureMaterial
{
    private T? volumeTexture;
    public T? VolumeTexture
    {
        set
        {
            Set(ref volumeTexture, value);
        }
        get
        {
            return volumeTexture;
        }
    }

    private global::SharpDX.Direct3D11.SamplerStateDescription sampler = DefaultSamplers.VolumeSampler;
    public global::SharpDX.Direct3D11.SamplerStateDescription Sampler
    {
        set
        {
            Set(ref sampler, value);
        }
        get
        {
            return sampler;
        }
    }

    private double sampleDistance = 1.0;
    /// <summary>
    /// Gets or sets the step size, controls the quality.
    /// </summary>
    /// <value>
    /// The size of the step.
    /// </value>
    public double SampleDistance
    {
        set
        {
            Set(ref sampleDistance, value);
        }
        get
        {
            return sampleDistance;
        }
    }

    private int maxIterations = 512;
    /// <summary>
    /// Gets or sets the iteration. Usually set to VolumeDepth.
    /// </summary>
    /// <value>
    /// The iteration.
    /// </value>
    public int MaxIterations
    {
        set
        {
            Set(ref maxIterations, value);
        }
        get
        {
            return maxIterations;
        }
    }

    private int iterationOffset = 0;
    /// <summary>
    /// Gets or sets the iteration offset. This can be used to achieve cross section
    /// </summary>
    /// <value>
    /// The iteration offset.
    /// </value>
    public int IterationOffset
    {
        set
        {
            Set(ref iterationOffset, value);
        }
        get
        {
            return iterationOffset;
        }
    }

    private double isoValue = 0;
    /// <summary>
    /// Gets or sets the iso value. Only data with isovalue > sepecified iso value will be displayed
    /// Value must be normalized to 0~1. Default = 1, show all data.
    /// </summary>
    /// <value>
    /// The iso value.
    /// </value>
    public double IsoValue
    {
        set
        {
            Set(ref isoValue, value);
        }
        get
        {
            return isoValue;
        }
    }

    private Color4 color = new(1, 1, 1, 1);
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color4 Color
    {
        set
        {
            Set(ref color, value);
        }
        get
        {
            return color;
        }
    }

    private Color4[]? transferMap;
    public Color4[]? TransferMap
    {
        set
        {
            Set(ref transferMap, value);
        }
        get
        {
            return transferMap;
        }
    }

    private bool enablePlaneAlignment = true;
    public bool EnablePlaneAlignment
    {
        set
        {
            Set(ref enablePlaneAlignment, value);
        }
        get
        {
            return enablePlaneAlignment;
        }
    }
    protected virtual string DefaultPassName { get; } = DefaultPassNames.Default;

    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new VolumeMaterialVariable<T>(manager, technique, this, DefaultPassName)
        {
            OnCreateTexture = (material, effectsManager) => { return OnCreateTexture(effectsManager); }
        };
    }

    protected abstract ShaderResourceViewProxy? OnCreateTexture(IEffectsManager? manager);
}

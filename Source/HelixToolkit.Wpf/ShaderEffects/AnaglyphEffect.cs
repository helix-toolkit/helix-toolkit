using System.Reflection;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides an anaglyph blending effect.
/// </summary>
/// <remarks>
/// Usage:
/// 1. Add the effect to the LEFT EYE UIElement.
/// 2. Set RightInput to a VisualBrush of the RIGHT EYE UIElement.
/// See the AnaglyphView3D for an example.
/// </remarks>
public sealed class AnaglyphEffect : ShaderEffect
{
    // Brush-valued properties turn into sampler-property in the shader.
    // This helper sets "ImplicitInput" as the default, meaning the default
    // sampler is whatever the rendering of the element it's being applied to is.
    /// <summary>
    /// Identifies the <see cref="LeftInput"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftInputProperty = RegisterPixelShaderSamplerProperty(
        "LeftInput", typeof(AnaglyphEffect), 0);

    /// <summary>
    /// Identifies the <see cref="Method"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MethodProperty = DependencyProperty.Register(
        "Method",
        typeof(AnaglyphMethod),
        typeof(AnaglyphEffect),
        new UIPropertyMetadata(AnaglyphMethod.Gray, AnaglyphMethodChanged));

    // Brush-valued properties turn into sampler-property in the shader.
    // This helper sets "ImplicitInput" as the default, meaning the default
    // sampler is whatever the rendering of the element it's being applied to is.
    /// <summary>
    /// Identifies the <see cref="RightInput"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RightInputProperty = RegisterPixelShaderSamplerProperty(
        "RightInput", typeof(AnaglyphEffect), 1);

    /// <summary>
    /// The effect file.
    /// </summary>
    private const string EffectFile = "ShaderEffects/AnaglyphEffect.ps";

    /// <summary>
    /// This property is mapped to the offset variable within the pixel shader.
    /// </summary>
    private static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
        "Offset",
        typeof(float),
        typeof(AnaglyphEffect),
        new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(1)));

    /// <summary>
    /// The shader.
    /// </summary>
    private static readonly PixelShader Shader = new PixelShader();

    /// <summary>
    /// This property is mapped to the method variable within the pixel shader.
    /// </summary>
    private static readonly DependencyProperty ShaderMethodProperty = DependencyProperty.Register(
        "ShaderMethod",
        typeof(float),
        typeof(AnaglyphEffect),
        new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(0)));

    /// <summary>
    /// Initializes static members of the <see cref="AnaglyphEffect"/> class.
    /// </summary>
    static AnaglyphEffect()
    {
        Assembly a = typeof(AnaglyphEffect).Assembly;
        string assemblyShortName = a.ToString().Split(',')[0];
        string uriString = "pack://application:,,,/" + assemblyShortName + ";component/" + EffectFile;
        Shader.UriSource = new Uri(uriString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "AnaglyphEffect" /> class.
    /// </summary>
    public AnaglyphEffect()
    {
        this.PixelShader = Shader;

        // Update each DependencyProperty that's registered with a shader register.  This
        // is needed to ensure the shader gets sent the proper default value.
        this.UpdateShaderValue(MethodProperty);
        this.UpdateShaderValue(LeftInputProperty);
        this.UpdateShaderValue(RightInputProperty);
    }

    /// <summary>
    /// Gets or sets the left input brush.
    /// </summary>
    /// <value>The left input.</value>
    public Brush LeftInput
    {
        get
        {
            return (Brush)this.GetValue(LeftInputProperty);
        }

        set
        {
            this.SetValue(LeftInputProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the anaglyph method.
    /// </summary>
    /// <value>The method.</value>
    public AnaglyphMethod Method
    {
        get
        {
            return (AnaglyphMethod)this.GetValue(MethodProperty);
        }

        set
        {
            this.SetValue(MethodProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the horizontal offset.
    /// </summary>
    public float Offset
    {
        get
        {
            return (float)this.GetValue(OffsetProperty);
        }

        set
        {
            this.SetValue(OffsetProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the right input brush.
    /// </summary>
    /// <value>The right input.</value>
    public Brush RightInput
    {
        get
        {
            return (Brush)this.GetValue(RightInputProperty);
        }

        set
        {
            this.SetValue(RightInputProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the shader method.
    /// </summary>
    private float ShaderMethod
    {
        set
        {
            this.SetValue(ShaderMethodProperty, value);
        }
    }

    /// <summary>
    /// The anaglyph method changed.
    /// </summary>
    /// <param name="d">
    /// The d.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void AnaglyphMethodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ef = (AnaglyphEffect)d;
        ef.ShaderMethod = (int)ef.Method;
    }
}

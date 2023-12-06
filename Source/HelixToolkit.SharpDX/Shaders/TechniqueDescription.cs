using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
[DataContract]
public sealed class TechniqueDescription
{
    /// <summary>
    /// Technique Name
    /// </summary>
    [DataMember(Name = @"Name")]
    public string Name
    {
        set; get;
    } = string.Empty;

    /// <summary>
    /// Input Layout
    /// </summary>
    [DataMember(Name = @"InputLayoutDescription")]
    public InputLayoutDescription? InputLayoutDescription
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the pass descriptions.
    /// </summary>
    /// <value>
    /// The pass descriptions.
    /// </value>
    [DataMember(Name = @"PassDescriptions")]
    public IList<ShaderPassDescription>? PassDescriptions
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether this technique is null technique.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
    /// </value>
    public bool IsNull { set; get; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
    /// </summary>
    public TechniqueDescription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public TechniqueDescription(string name)
    {
        Name = name;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="inputLayout">The input layout.</param>
    public TechniqueDescription(string name, InputLayoutDescription inputLayout)
        : this(name)
    {
        InputLayoutDescription = inputLayout;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="inputLayout">The input layout.</param>
    /// <param name="shaderPasses">The shader passes.</param>
    public TechniqueDescription(string name, InputLayoutDescription inputLayout, IList<ShaderPassDescription> shaderPasses)
        : this(name, inputLayout)
    {
        PassDescriptions = shaderPasses;
    }
}

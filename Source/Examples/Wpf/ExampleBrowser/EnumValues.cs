using System.Windows.Markup;
using System;
using CommunityToolkit.Diagnostics;

namespace ExampleBrowser;

/// <summary>
/// Provides the enumeration values from the specified type.
/// </summary>
public sealed class EnumValues : MarkupExtension
{
    /// <summary>
    /// The type.
    /// </summary>
    private readonly Type type;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValues"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="System.InvalidOperationException">The type is not an enumeration type.</exception>
    public EnumValues(Type type)
    {
        this.type = type;

        if (!type.IsEnum)
        {
            ThrowHelper.ThrowInvalidOperationException("The type is not an Enum.");
        }
    }

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(this.type);
    }
}

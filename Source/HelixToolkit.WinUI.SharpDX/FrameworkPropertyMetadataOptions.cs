namespace HelixToolkit.WinUI.SharpDX;

public enum FrameworkPropertyMetadataOptions
{
    //     No options are specified; the dependency property uses the default behavior of
    //     the WPF property system.
    None = 0,

    //     The measure pass of layout compositions is affected by value changes to this
    //     dependency property.
    AffectsMeasure = 1,

    //     The arrange pass of layout composition is affected by value changes to this dependency
    //     property.
    AffectsArrange = 2,

    //     The measure pass on the parent element is affected by value changes to this dependency
    //     property.
    AffectsParentMeasure = 4,

    //     The arrange pass on the parent element is affected by value changes to this dependency
    //     property.
    AffectsParentArrange = 8,

    //     Some aspect of rendering or layout composition (other than measure or arrange)
    //     is affected by value changes to this dependency property.
    AffectsRender = 16,

    //     The values of this dependency property are inherited by child elements.
    Inherits = 32,

    //     The values of this dependency property span separated trees for purposes of property
    //     value inheritance.
    OverridesInheritanceBehavior = 64,

    //     Data binding to this dependency property is not allowed.
    NotDataBindable = 128,

    //     The System.Windows.Data.BindingMode for data bindings on this dependency property
    //     defaults to System.Windows.Data.BindingMode.TwoWay.
    BindsTwoWayByDefault = 256,

    //     The values of this dependency property should be saved or restored by journaling
    //     processes, or when navigating by Uniform resource identifiers (URIs).
    Journal = 1024,

    //     The subproperties on the value of this dependency property do not affect any
    //     aspect of rendering.
    SubPropertiesDoNotAffectRender = 2048
}

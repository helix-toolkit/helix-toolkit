using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System;

namespace DataTemplate;

public class BindingConverter : ExpressionConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(MarkupExtension))
            return true;
        else
            return false;
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(MarkupExtension))
        {
            if (value is not BindingExpression bindingExpression)
                throw new ArgumentNullException(nameof(value));

            return bindingExpression.ParentBinding;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}

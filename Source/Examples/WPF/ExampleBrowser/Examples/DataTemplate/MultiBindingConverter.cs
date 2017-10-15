namespace DataTemplateDemo
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class MultiBindingConverter : ExpressionConverter
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
                var bindingExpression = value as MultiBindingExpression;
                if (bindingExpression == null)
                    throw new ArgumentNullException(nameof(bindingExpression));

                return bindingExpression.ParentMultiBinding;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

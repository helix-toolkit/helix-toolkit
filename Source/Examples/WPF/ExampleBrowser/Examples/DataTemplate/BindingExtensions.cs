namespace DataTemplateDemo
{
    using System.Windows.Data;

    public static class BindingExtensions
    {
        /// <summary>
        /// Clones <paramref name="binding"/>.
        /// </summary>
        /// <param name="binding">The binding to clone.</param>
        /// <returns>A clone of <paramref name="binding"/>.</returns>
        /// <remarks>
        /// ElementName, RelativeSource, Source and ValidationRules are not being copied.
        /// </remarks>
        public static Binding Clone(this Binding binding)
        {
            return new Binding
            {
                AsyncState = binding.AsyncState,
                BindingGroupName = binding.BindingGroupName,
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Converter = binding.Converter,
                ConverterCulture = binding.ConverterCulture,
                ConverterParameter = binding.ConverterParameter,
                Delay = binding.Delay,
                FallbackValue = binding.FallbackValue,
                IsAsync = binding.IsAsync,
                Mode = binding.Mode,
                NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                NotifyOnValidationError = binding.NotifyOnValidationError,
                Path = binding.Path,
                StringFormat = binding.StringFormat,
                TargetNullValue = binding.TargetNullValue,
                UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                UpdateSourceTrigger = binding.UpdateSourceTrigger,
                ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                ValidatesOnExceptions = binding.ValidatesOnExceptions,
                ValidatesOnNotifyDataErrors = binding.ValidatesOnNotifyDataErrors,
                XPath = binding.XPath
            };
        }
        /// <summary>
        /// Clones <paramref name="multiBinding"/>.
        /// </summary>
        /// <param name="multiBinding">The binding to clone.</param>
        /// <returns>A clone of <paramref name="multiBinding"/>.</returns>
        /// <remarks>
        /// Bindings and ValidationRules are not being copied.
        /// </remarks>
        public static MultiBinding Clone(this MultiBinding multiBinding)
        {
            return new MultiBinding
            {
                BindingGroupName = multiBinding.BindingGroupName,
                Converter = multiBinding.Converter,
                ConverterCulture = multiBinding.ConverterCulture,
                ConverterParameter = multiBinding.ConverterParameter,
                Delay = multiBinding.Delay,
                FallbackValue = multiBinding.FallbackValue,
                Mode = multiBinding.Mode,
                NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated,
                NotifyOnValidationError = multiBinding.NotifyOnValidationError,
                StringFormat = multiBinding.StringFormat,
                TargetNullValue = multiBinding.TargetNullValue,
                UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter,
                UpdateSourceTrigger = multiBinding.UpdateSourceTrigger,
                ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors,
                ValidatesOnExceptions = multiBinding.ValidatesOnExceptions,
                ValidatesOnNotifyDataErrors = multiBinding.ValidatesOnNotifyDataErrors
            };
        }
    }
}

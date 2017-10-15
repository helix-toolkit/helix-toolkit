using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HelixToolkit.Wpf.SharpDX
{
    public interface IAffectsRender
    {
        bool AffectsRender { get; }
    }
    public class AffectsRenderPropertyMetadata : PropertyMetadata, IAffectsRender
    {
        public AffectsRenderPropertyMetadata()
        {
        }

        public AffectsRenderPropertyMetadata(PropertyChangedCallback propertyChangedCallback) : base(propertyChangedCallback)
        {
        }

        public AffectsRenderPropertyMetadata(object defaultValue) : base(defaultValue)
        {
        }

        public AffectsRenderPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) : base(defaultValue, propertyChangedCallback)
        {
        }

        public AffectsRenderPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        public bool AffectsRender { set; get; } = true;       
    }

    public class AffectsRenderFrameworkPropertyMetadata : FrameworkPropertyMetadata, IAffectsRender
    {
        public AffectsRenderFrameworkPropertyMetadata()
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback) 
            : base(FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags = FrameworkPropertyMetadataOptions.AffectsRender) 
            : base(defaultValue, flags)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) 
            : base(propertyChangedCallback, coerceValueCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) 
            : base(defaultValue, flags, propertyChangedCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) 
            : base(defaultValue, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback, coerceValueCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(defaultValue, flags, propertyChangedCallback, coerceValueCallback)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, bool isAnimationProhibited)
            : base(defaultValue, flags, propertyChangedCallback, coerceValueCallback, isAnimationProhibited)
        {
        }

        public AffectsRenderFrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, bool isAnimationProhibited, UpdateSourceTrigger defaultUpdateSourceTrigger) 
            : base(defaultValue, flags, propertyChangedCallback, coerceValueCallback, isAnimationProhibited, defaultUpdateSourceTrigger)
        {
        }
    }
}

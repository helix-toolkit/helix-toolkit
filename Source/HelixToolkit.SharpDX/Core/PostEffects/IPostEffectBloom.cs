using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public interface IPostEffectBloom : IPostEffect
{
    Color4 ThresholdColor
    {
        set; get;
    }
    float BloomExtractIntensity
    {
        set; get;
    }
    float BloomPassIntensity
    {
        set; get;
    }

    float BloomCombineSaturation
    {
        set; get;
    }

    float BloomCombineIntensity
    {
        set; get;
    }
    int NumberOfBlurPass
    {
        set; get;
    }
}

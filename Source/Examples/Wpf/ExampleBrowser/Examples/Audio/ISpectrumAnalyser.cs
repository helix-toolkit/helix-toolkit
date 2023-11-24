using NAudio.Dsp;

namespace Audio;

public interface ISpectrumAnalyser
{
    void Update(Complex[] result);
}

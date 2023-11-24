using NAudio.Dsp;
using System.Diagnostics;
using System;

namespace Audio;

internal sealed class SampleAggregator
{
    // volume
    public event EventHandler<MaxSampleEventArgs>? MaximumCalculated;
    private float maxValue;
    private float minValue;
    public int NotificationCount { get; set; }
    int count;

    // FFT
    public event EventHandler<FftEventArgs>? FftCalculated;
    public bool PerformFFT { get; set; }
    private readonly Complex[] fftBuffer;
    private readonly FftEventArgs fftArgs;
    private int fftPos;
    private readonly int m;

    public SampleAggregator(int m)
    {
        this.m = m;
        int n = (int)Math.Pow(2, m);
        fftBuffer = new Complex[n];
        fftArgs = new FftEventArgs(fftBuffer);
    }

    public void Reset()
    {
        count = 0;
        maxValue = minValue = 0;
    }

    public void Add(float value)
    {
        if (PerformFFT && FftCalculated != null)
        {
            fftBuffer[fftPos].X = value;
            fftBuffer[fftPos].Y = 0;
            fftPos++;
            if (fftPos >= fftBuffer.Length)
            {
                fftPos = 0;
                // 1024 = 2^10
                FastFourierTransform.FFT(true, m, fftBuffer);
                FftCalculated(this, fftArgs);
            }
        }

        maxValue = Math.Max(maxValue, value);
        minValue = Math.Min(minValue, value);
        count++;
        if (count >= NotificationCount && NotificationCount > 0)
        {
            MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue));
            Reset();
        }
    }
}

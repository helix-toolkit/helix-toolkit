using System.Diagnostics;
using System;
using NAudio.Dsp;

namespace Audio;

internal sealed class FftEventArgs : EventArgs
{
    [DebuggerStepThrough]
    public FftEventArgs(Complex[] result)
    {
        this.Result = result;
    }
    public Complex[] Result { get; private set; }
}

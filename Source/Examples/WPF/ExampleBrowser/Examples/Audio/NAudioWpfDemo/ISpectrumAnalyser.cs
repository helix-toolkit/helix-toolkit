// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpectrumAnalyser.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NAudio.Dsp;

namespace NAudioWpfDemo
{
    public interface ISpectrumAnalyser
    {
        void Update(Complex[] result);
    }
}
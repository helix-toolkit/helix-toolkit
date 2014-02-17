// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpectrumAnalyser.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWaveFormRenderer.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAudioWpfDemo
{
    public interface IWaveFormRenderer
    {
        void AddValue(float maxValue, float minValue);
    }
}
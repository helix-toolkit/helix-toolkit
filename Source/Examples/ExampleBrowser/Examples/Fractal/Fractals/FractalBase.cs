// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FractalBase.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Media.Media3D;

namespace FractalDemo
{
    public abstract class FractalBase : Observable
    {
        public int Level { get; set; }
        public int TriangleCount { get; protected set; }
        public abstract GeometryModel3D Generate();

      
    }
}
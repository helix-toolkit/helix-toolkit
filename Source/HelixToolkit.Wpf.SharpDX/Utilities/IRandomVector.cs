using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX.Randoms
{
    public interface IRandomSeed
    {
        uint Seed { get; }
    }
    public interface IRandomVector : IRandomSeed
    {
        Vector3 RandomVector3 { get; }
    }
}

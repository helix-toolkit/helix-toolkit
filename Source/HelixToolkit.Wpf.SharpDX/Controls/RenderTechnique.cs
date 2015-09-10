using SharpDX.Direct3D11;
using System;

namespace HelixToolkit.Wpf.SharpDX
{
    public sealed class RenderTechnique : IComparable
    {
        public RenderTechnique(string name)
        {
            this.Name = name;
        }

        public RenderTechnique(string name, Effect effect, InputLayout layout)
        {
            this.Name = name;
            this.Device = effect.Device;
            this.Effect = effect;
            this.EffectTechnique = effect.GetTechniqueByName(this.Name);
            this.InputLayout = layout;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Name.Equals(obj.ToString());
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj.ToString());
        }

        public static bool operator ==(RenderTechnique a, RenderTechnique b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name.Equals(b.Name);
        }

        public static bool operator !=(RenderTechnique a, RenderTechnique b)
        {
            return !(a == b);
        }

        public string Name { get; private set; }

        public Effect Effect { get; private set; }

        public EffectTechnique EffectTechnique { get; private set; }

        public Device Device { get; private set; }

        public InputLayout InputLayout { get; private set; }
    }
}

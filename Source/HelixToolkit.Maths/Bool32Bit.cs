namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a bool value with size of 32 bits (4 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Bool32Bit : IEquatable<Bool32Bit>, IFormattable
    {
        internal uint v;
        public bool V
        { 
            readonly get => v != 0;
            set => v = value ? 1u : 0;
        }

        public Bool32Bit(bool value)
        {
            v = value ? 1u : 0;
        }

        public readonly bool Equals(Bool32Bit other)
        {
            return v == other.v;
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return v.ToString(format, formatProvider);
        }

        public static implicit operator Bool32Bit(bool value)
        {
            return new Bool32Bit(value);
        }

        public static implicit operator bool(Bool32Bit value)
        {
            return value.v != 0;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Bool32Bit bit && Equals(bit);
        }

        public static bool operator ==(Bool32Bit left, Bool32Bit right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bool32Bit left, Bool32Bit right)
        {
            return !(left == right);
        }

        public override readonly int GetHashCode()
        {
            return v.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a bool value with size of 32 bits (4 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Bool32Bit : IEquatable<Bool32Bit>, IFormattable
    {
        public uint Value;
        public Bool32Bit(bool value)
        {
            Value = value ? 1u : 0;
        }

        public bool Equals(Bool32Bit other)
        {
            return Value == other.Value;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(format, formatProvider);
        }

        public static implicit operator Bool32Bit(bool value)
        {
            return new Bool32Bit(value);
        }

        public static implicit operator bool(Bool32Bit value)
        {
            return value.Value != 0;
        }

        public override bool Equals(object obj)
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

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public struct Thickness : IEquatable<Thickness>
{
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    public Thickness(float size)
    {
        Left = size;
        Right = size;
        Top = size;
        Bottom = size;
    }

    public Thickness(float left, float right, float top, float bottom)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }

    public bool Equals(Thickness other)
    {
        return this.Left == other.Left && this.Right == other.Right && this.Top == other.Top && this.Bottom == other.Bottom;
    }

    public override bool Equals(object? obj)
    {
        return obj is Thickness thickness && Equals(thickness);
    }

    public override int GetHashCode()
    {
#if NET6_0_OR_GREATER
        return HashCode.Combine(Left, Right, Top, Bottom);
#else
        int hashCode = 551583723;
        hashCode = hashCode * -1521134295 + Left.GetHashCode();
        hashCode = hashCode * -1521134295 + Right.GetHashCode();
        hashCode = hashCode * -1521134295 + Top.GetHashCode();
        hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
        return hashCode;
#endif
    }

    public static bool operator ==(Thickness left, Thickness right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Thickness left, Thickness right)
    {
        return !(left == right);
    }

    public static implicit operator Vector4(Thickness t)
    {
        return new Vector4(t.Left, t.Top, t.Right, t.Bottom);
    }
}

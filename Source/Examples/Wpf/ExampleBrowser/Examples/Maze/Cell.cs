using System;

namespace Maze;

/// <summary>
/// Represents a cell.
/// </summary>
public struct Cell : IEquatable<Cell>
{
    private int i;

    /// <summary>
    /// Gets or sets the I.
    /// </summary>
    /// <value>The I.</value>
    public int I
    {
        get
        {
            return this.i;
        }
        set
        {
            this.i = value;
        }
    }

    private int j;

    /// <summary>
    /// Gets or sets the J.
    /// </summary>
    /// <value>The J.</value>
    public int J
    {
        get
        {
            return this.j;
        }
        set
        {
            this.j = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cell"/> struct.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <param name="j">The j.</param>
    public Cell(int i, int j)
    {
        this.i = i; this.j = j;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cell cell && Equals(cell);
    }

    public bool Equals(Cell other)
    {
        return i == other.i &&
               j == other.j;
    }

    public override int GetHashCode()
    {
#if NET6_0_OR_GREATER
        return HashCode.Combine(i, j);
#else
        unchecked
        {
            return (this.i * 397) ^ this.j;
        }
#endif
    }

    public static bool operator ==(Cell left, Cell right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Cell left, Cell right)
    {
        return !(left == right);
    }
}

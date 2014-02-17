// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cell.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MazeDemo
{
    /// <summary>
    /// Represents a cell.
    /// </summary>
    public struct Cell
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != typeof(Cell))
            {
                return false;
            }
            return this.Equals((Cell)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Cell"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Cell"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Cell"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Cell other)
        {
            return other.i == this.i && other.j == this.j;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.i * 397) ^ this.j;
            }
        }
    }
}
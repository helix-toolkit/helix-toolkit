// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMazeGenerator.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MazeDemo
{
    /// <summary>
    /// Provides functionality to generate a maze.
    /// </summary>
    public interface IMazeGenerator
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        int Height { get; set; }

        /// <summary>
        /// Generates the maze.
        /// </summary>
        /// <returns>A maze.</returns>
        bool[,] Generate();
    }
}
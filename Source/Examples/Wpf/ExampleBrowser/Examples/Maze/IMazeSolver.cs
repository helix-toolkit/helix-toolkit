using System.Collections.Generic;

namespace Maze;

/// <summary>
/// Provides functionality to solve a maze.
/// </summary>
public interface IMazeSolver
{
    /// <summary>
    /// Solves the specified maze.
    /// </summary>
    /// <param name="maze">The maze.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>The solution.</returns>
    IEnumerable<Cell>? Solve(bool[,] maze, Cell start, Cell end);
}

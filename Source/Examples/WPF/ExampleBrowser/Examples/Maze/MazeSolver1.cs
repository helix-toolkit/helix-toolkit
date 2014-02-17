// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MazeSolver1.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MazeDemo
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a simple DFS solution algorithm.
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Maze_solving_algorithm.
    /// </remarks>
    public class MazeSolver1 : IMazeSolver
    {
        /// <summary>
        /// Solves the specified maze.
        /// </summary>
        /// <param name="maze">The maze.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>The solution.</returns>
        public IEnumerable<Cell> Solve(bool[,] maze, Cell start, Cell end)
        {
            var stack = new Stack<Cell>();
            stack.Push(start);

            var visited = new bool[maze.GetLength(0), maze.GetLength(1)];
            var previousCell = new Cell[maze.GetLength(0), maze.GetLength(1)];

            Func<Cell, bool> hasRightWall = cell => maze[cell.I, cell.J + 1];
            Func<Cell, bool> hasLeftWall = cell => maze[cell.I, cell.J - 1];
            Func<Cell, bool> hasTopWall = cell => maze[cell.I - 1, cell.J];
            Func<Cell, bool> hasBottomWall = cell => maze[cell.I + 1, cell.J];

            while (stack.Count > 0)
            {
                var temp = stack.Pop();

                if (temp.Equals(end))
                {
                    var foundPath = new List<Cell>();
                    while (true)
                    {
                        foundPath.Add(temp);
                        if (temp.Equals(start))
                        {
                            break;
                        }

                        temp = previousCell[temp.I, temp.J];
                    }

                    foundPath.Reverse();
                    return foundPath;
                }

                visited[temp.I, temp.J] = true;

                if (!hasLeftWall(temp) && !visited[temp.I, temp.J - 2])
                {
                    stack.Push(new Cell(temp.I, temp.J - 2));
                    previousCell[temp.I, temp.J - 2] = temp;
                }

                if (!hasRightWall(temp) && !visited[temp.I, temp.J + 2])
                {
                    stack.Push(new Cell(temp.I, temp.J + 2));
                    previousCell[temp.I, temp.J + 2] = temp;
                }

                if (!hasTopWall(temp) && !visited[temp.I - 2, temp.J])
                {
                    stack.Push(new Cell(temp.I - 2, temp.J));
                    previousCell[temp.I - 2, temp.J] = temp;
                }

                if (!hasBottomWall(temp) && !visited[temp.I + 2, temp.J])
                {
                    stack.Push(new Cell(temp.I + 2, temp.J));
                    previousCell[temp.I + 2, temp.J] = temp;
                }
            }

            // no solution found
            return null;
        }
    }
}
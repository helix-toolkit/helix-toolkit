using System.Collections.Generic;
using System;

namespace Maze;

/// <summary>
/// A simple DFS maze generator.
/// </summary>
/// <remarks>
/// Converted from python code example at http://en.wikipedia.org/wiki/Maze_generation_algorithm.
/// </remarks>
public sealed class MazeGenerator2 : IMazeGenerator
{
    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the seed.
    /// </summary>
    /// <value>The seed.</value>
    public int Seed { get; set; }

    /// <summary>
    /// Generates the maze.
    /// </summary>
    /// <returns>A maze.</returns>
    public bool[,] Generate()
    {
        int width = this.Width;
        int height = this.Height;

        if (width % 2 == 0)
        {
            throw new InvalidOperationException("Width must be an odd number");
        }

        if (height % 2 == 0)
        {
            throw new InvalidOperationException("Height must be an odd number");
        }

        bool[,] maze = new bool[height, width];

        // Initialize maze (all walls intact)
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                maze[i, j] = i % 2 == 0 || j % 2 == 0;
            }
        }

        var stack = new Stack<Cell>();
        var current = new Cell(1, 1);
        stack.Push(current);
        bool isVisited(int i, int j) => maze[i - 1, j] && maze[i + 1, j] && maze[i, j - 1] && maze[i, j + 1];
        var random = new Random(this.Seed);

        while (stack.Count > 0)
        {
            var neighbours = new List<Cell>();
            int i = current.I;
            int j = current.J;

            // Find unvisited neighbours
            if (i > 1 && isVisited(i - 2, j))
            {
                neighbours.Add(new Cell(i - 2, j));
            }

            if (j > 1 && isVisited(i, j - 2))
            {
                neighbours.Add(new Cell(i, j - 2));
            }

            if (i + 2 < height && isVisited(i + 2, j))
            {
                neighbours.Add(new Cell(i + 2, j));
            }

            if (j + 2 < width && isVisited(i, j + 2))
            {
                neighbours.Add(new Cell(i, j + 2));
            }

            if (neighbours.Count > 0)
            {
                var next = neighbours[random.Next(neighbours.Count)];

                // break the wall between current and next cell
                int ii = (next.I + current.I) / 2;
                int jj = (next.J + current.J) / 2;
                maze[ii, jj] = false;

                stack.Push(current);
                current = next;
            }
            else
            {
                current = stack.Pop();
            }
        }

        return maze;
    }
}

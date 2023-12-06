using System.Collections.Generic;
using System;

namespace Maze;

/// <summary>
/// A simple maze generator (this maze may not be 'simply connected').
/// </summary>
/// <remarks>
/// Converted from python code example at http://en.wikipedia.org/wiki/Maze_generation_algorithm.
/// </remarks>
public sealed class MazeGenerator1 : IMazeGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MazeGenerator1"/> class.
    /// </summary>
    public MazeGenerator1()
    {
        this.Width = 31;
        this.Height = 21;
        this.Density = 0.75;
        this.Complexity = 0.75;
    }

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
    /// Gets or sets the complexity.
    /// </summary>
    /// <value>The complexity.</value>
    public double Complexity { get; set; }

    /// <summary>
    /// Gets or sets the density.
    /// </summary>
    /// <value>The density.</value>
    public double Density { get; set; }

    /// <summary>
    /// Generates a simple maze by the specified properties.
    /// </summary>
    /// <returns>A maze.</returns>
    public bool[,] Generate()
    {
        int width = this.Width;
        int height = this.Height;
        double complexity = this.Complexity;
        double density = this.Density;

        if (width % 2 == 0)
        {
            throw new InvalidOperationException("Width must be an odd number");
        }

        if (height % 2 == 0)
        {
            throw new InvalidOperationException("Height must be an odd number");
        }

        // Adjust complexity and density relative to maze size
        int complexityLevel = (int)(complexity * (5 * (height + width)));
        int densityLevel = (int)(density * ((height / 2) * (width / 2)));

        // Build actual maze
        bool[,] maze = new bool[height, width];

        // Fill borders
        for (int j = 0; j < width; j++)
        {
            maze[0, j] = maze[height - 1, j] = true;
        }

        for (int i = 0; i < height; i++)
        {
            maze[i, 0] = maze[i, width - 1] = true;
        }

        // Make isles
        var random = new Random(this.Seed);
        for (int i = 0; i < densityLevel; i++)
        {
            int x = random.Next(width / 2 + 1) * 2;
            int y = random.Next(height / 2 + 1) * 2;
            maze[y, x] = true;
            for (int j = 0; j < complexityLevel; j++)
            {
                var neighbours = new List<int[]>();
                if (x > 1)
                {
                    neighbours.Add(new[] { y, x - 2 });
                }

                if (x < width - 2)
                {
                    neighbours.Add(new[] { y, x + 2 });
                }

                if (y > 1)
                {
                    neighbours.Add(new[] { y - 2, x });
                }

                if (y < height - 2)
                {
                    neighbours.Add(new[] { y + 2, x });
                }

                if (neighbours.Count > 0)
                {
                    var yx = neighbours[random.Next(neighbours.Count)];
                    int y2 = yx[0];
                    int x2 = yx[1];
                    if (!maze[y2, x2])
                    {
                        maze[y2, x2] = true;
                        maze[y2 + (y - y2) / 2, x2 + (x - x2) / 2] = true;
                        x = x2;
                        y = y2;
                    }
                }
            }
        }

        return maze;
    }
}

using System.IO;
using System;

namespace Export;

/// <summary>
/// Finds files in the Program files folders.
/// </summary>
public static class ProgramFilesHelper
{
    /// <summary>
    /// Finds the program file.
    /// </summary>
    /// <param name="company">The company.</param>
    /// <param name="program">The program.</param>
    /// <returns>
    /// The program file.
    /// </returns>
    public static string? FindProgramFile(string company, string program)
    {
        string? result;
        var programFiles6432 = Environment.GetEnvironmentVariable("ProgramW6432");
        if (programFiles6432 != null)
        {
            result = FindRecursive(Path.Combine(programFiles6432, company), program);
            if (result != null)
            {
                return result;
            }
        }

        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        result = FindRecursive(Path.Combine(programFiles, company), program);
        if (result != null)
        {
            return result;
        }

        programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        result = FindRecursive(Path.Combine(programFiles, company), program);
        return result;
    }

    /// <summary>
    /// Finds a file recursively.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="file">The file to search for.</param>
    /// <returns>
    /// The path to the file.
    /// </returns>
    private static string? FindRecursive(string folder, string file)
    {
        if (!Directory.Exists(folder))
        {
            return null;
        }

        var path = Path.Combine(folder, file);
        if (File.Exists(path))
        {
            return path;
        }

        foreach (var subdir in Directory.GetDirectories(folder))
        {
            var result = FindRecursive(subdir, file);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}

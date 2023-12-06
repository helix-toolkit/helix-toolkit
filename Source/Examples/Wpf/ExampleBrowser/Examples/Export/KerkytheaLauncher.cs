using System.Diagnostics;
using System.IO;

namespace Export;

/// <summary>
/// Creates the arguments to start Kerkythea.
/// </summary>
public class KerkytheaLauncher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KerkytheaLauncher"/> class.
    /// </summary>
    /// <param name="executable">
    /// Path to the Kerkythea executable file.
    /// </param>
    public KerkytheaLauncher(string? executable = null)
    {
        executable ??= ProgramFilesHelper.FindProgramFile("Kerkythea Rendering System", "Kerkythea.exe");

        this.Executable = executable;
    }

    /// <summary>
    /// Gets or sets the path to the Kerkythea executable.
    /// </summary>
    /// <value>The executable.</value>
    public string? Executable { get; set; }

    /// <summary>
    /// Gets or sets the input file (.xml).
    /// </summary>
    /// <value>The input file.</value>
    public string? InputFile { get; set; }

    /// <summary>
    /// Gets or sets the output file (.png).
    /// </summary>
    /// <value>The output file.</value>
    public string? OutputFile { get; set; }

    /// <summary>
    /// Starts Kerkythea.
    /// </summary>
    /// <returns>The process.</returns>
    public Process? Start()
    {
        if (string.IsNullOrEmpty(this.InputFile) || string.IsNullOrEmpty(this.Executable))
        {
            return null;
        }

        string fullPath = Path.GetFullPath(this.InputFile);
        string input = Path.GetFileName(this.InputFile);
        string? output = Path.GetFileName(this.OutputFile);
        if (string.IsNullOrEmpty(output))
        {
            output = Path.ChangeExtension(input, ".png");
        }

        var psi = new ProcessStartInfo(this.Executable)
        {
            WorkingDirectory = Path.GetDirectoryName(fullPath),
            Arguments = input + " -o " + output
        };

        return Process.Start(psi);
    }

}

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media.Media3D;

namespace Export;

/// <summary>
/// Creates the arguments to start Octane render.
/// </summary>
public class OctaneLauncher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OctaneLauncher"/> class.
    /// </summary>
    /// <param name="octaneExecutable">
    /// The octane executable.
    /// </param>
    public OctaneLauncher(string? octaneExecutable = null)
    {
        octaneExecutable ??= ProgramFilesHelper.FindProgramFile("Refractive Software", "Octane.exe");

        this.OctaneExecutable = octaneExecutable;
        this.MaxSamples = 16000;
        this.FilmWidth = 1000;
        this.FilmHeight = 600;
        this.Aperture = 1.0;
        this.FocalDepth = 1.0;
        this.FieldOfView = 45;
        this.IsNewProject = true;
        this.Link = true;
    }

    /// <summary>
    /// Gets or sets the aperture.
    /// </summary>
    /// <value>The aperture.</value>
    public double Aperture { get; set; }

    /// <summary>
    /// Gets or sets the cam motion pos.
    /// </summary>
    /// <value>The cam motion pos.</value>
    public Point3D CamMotionPos { get; set; }

    /// <summary>
    /// Gets or sets the cam motion target.
    /// </summary>
    /// <value>The cam motion target.</value>
    public Point3D CamMotionTarget { get; set; }

    /// <summary>
    /// Gets or sets the cam motion up.
    /// </summary>
    /// <value>The cam motion up.</value>
    public Vector3D CamMotionUp { get; set; }

    /// <summary>
    /// Gets or sets the camera position.
    /// </summary>
    public Point3D CamPos { get; set; }

    /// <summary>
    /// Gets or sets the camera target position.
    /// </summary>
    public Point3D CamTarget { get; set; }

    /// <summary>
    /// Gets or sets the camera up direction.
    /// </summary>
    public Vector3D CamUp { get; set; }

    /// <summary>
    /// Gets or sets the daylight sun direction.
    /// </summary>
    public Vector3D DaylightSunDir { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to exit after MaxSamples is reached.
    /// </summary>
    public bool Exit { get; set; }

    /// <summary>
    /// Gets or sets the field of view (degrees).
    /// </summary>
    /// <value>The field of view.</value>
    public double FieldOfView { get; set; }

    /// <summary>
    /// Gets or sets the height of the film.
    /// </summary>
    /// <value>The height of the film.</value>
    public int FilmHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the film.
    /// </summary>
    /// <value>The width of the film.</value>
    public int FilmWidth { get; set; }

    /// <summary>
    /// Gets or sets the focal depth.
    /// </summary>
    /// <value>The focal depth.</value>
    public double FocalDepth { get; set; }

    /// <summary>
    /// Gets or sets the imager exposure.
    /// </summary>
    /// <value>The imager exposure.</value>
    public double ImagerExposure { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is new project.
    /// </summary>
    public bool IsNewProject { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to link the mesh after startup.
    /// </summary>
    /// <value><c>true</c> if link; otherwise, <c>false</c>.</value>
    public bool Link { get; set; }

    /// <summary>
    /// Gets or sets the max samples.
    /// </summary>
    /// <value>The max samples.</value>
    public int MaxSamples { get; set; }

    /// <summary>
    /// Gets or sets the mesh file (.obj).
    /// </summary>
    /// <value>The mesh file.</value>
    public string? MeshFile { get; set; }

    /// <summary>
    /// Gets or sets the mesh node.
    /// </summary>
    /// <value>The mesh node.</value>
    public string? MeshNode { get; set; }

    /// <summary>
    /// Gets or sets the path to the Octane executable.
    /// </summary>
    /// <value>The octane executable.</value>
    public string? OctaneExecutable { get; set; }

    /// <summary>
    /// Gets or sets the output file (.png).
    /// </summary>
    /// <value>The output file.</value>
    public string? OutputFile { get; set; }

    /// <summary>
    /// Gets or sets the project file (.ocs).
    /// </summary>
    /// <value>The project file.</value>
    public string? ProjectFile { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use quiet mode.
    /// </summary>
    public bool Quiet { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to relink the mesh.
    /// </summary>
    public bool Relink { get; set; }

    /// <summary>
    /// Sets the camera.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <param name="changeUpDirectionFromZtoY">switch Y and Z if set to <c>true</c>.</param>
    public void SetCamera(ProjectionCamera camera, bool changeUpDirectionFromZtoY = true)
    {
        this.CamUp = new Vector3D(0, 1, 0);
        this.CamTarget = camera.Position + camera.LookDirection;
        this.CamPos = camera.Position;
        if (changeUpDirectionFromZtoY)
        {
            this.CamTarget = this.SwitchYZ(this.CamTarget);
            this.CamPos = this.SwitchYZ(this.CamPos);
        }

        if (camera is PerspectiveCamera pc)
        {
            this.FieldOfView = pc.FieldOfView;
        }

        this.CamMotionUp = this.CamUp;
        this.CamMotionTarget = this.CamTarget;
        this.CamMotionPos = this.CamPos;
    }

    /// <summary>
    /// Starts Octane.
    /// </summary>
    /// <returns>The process.</returns>
    public Process? Start()
    {
        var arguments = new StringBuilder();

        arguments.Append("-m " + this.MeshNode);

        if (this.Relink)
        {
            arguments.Append(" -r " + this.MeshFile); // Name of OBJ mesh file to relink rendered meshnoded with
        }

        if (this.Link)
        {
            arguments.Append(" -l " + this.MeshFile); // Name of OBJ mesh file to link after startup
        }

        if (this.IsNewProject && !string.IsNullOrEmpty(this.ProjectFile))
        {
            arguments.Append(" -n " + this.ProjectFile);

            // Create a new OCS project file from given command line arguments
        }

        // arguments.Append(" --imager-exposure " + ImagerExposure); // Imager Exposure Amount
        // arguments.Append(" --daylight-sundir-z " + DaylightSunDir.Z); // Daylight Sun Direction Vector Z Component
        // arguments.Append(" --daylight-sundir-y " + DaylightSunDir.Y); // Daylight Sun Direction Vector Y Component
        // arguments.Append(" --daylight-sundir-x " + DaylightSunDir.X); // Daylight Sun Direction Vector X Component
        // arguments.Append(" --cam-aperture " + Aperture); // Camera Aperture Radius
        // arguments.Append(" --cam-focaldepth " + FocalDepth); // Camera Focal Depth

        // arguments.Append(" --cam-fov " + FieldOfView); // Camera FOV (degrees)
        arguments.Append(" --cam-motion-up-z " + this.CamMotionUp.Z); // Camera Up Motion 2nd Vector Z Component
        arguments.Append(" --cam-motion-up-y " + this.CamMotionUp.Y); // Camera Up Motion 2nd Vector Y Component
        arguments.Append(" --cam-motion-up-x " + this.CamMotionUp.X); // Camera Up Motion 2nd Vector X Component
        arguments.Append(" --cam-motion-target-z " + this.CamMotionTarget.Z);

        // Camera Target Motion 2nd Position Z Component
        arguments.Append(" --cam-motion-target-y " + this.CamMotionTarget.Y);

        // Camera Target Motion 2nd Position Y Component
        arguments.Append(" --cam-motion-target-x " + this.CamMotionTarget.X);

        // Camera Target Motion 2nd Position X Component
        arguments.Append(" --cam-motion-pos-z " + this.CamMotionPos.Z); // Camera Motion 2nd Position Z Component
        arguments.Append(" --cam-motion-pos-y " + this.CamMotionPos.Y); // Camera Motion 2nd Position Y Component
        arguments.Append(" --cam-motion-pos-x " + this.CamMotionPos.X); // Camera Motion 2nd Position X Component

        arguments.Append(" --cam-up-z " + this.CamUp.Z); // camera Up Vector Z Component
        arguments.Append(" --cam-up-y " + this.CamUp.Y); // camera Up Vector Y Component
        arguments.Append(" --cam-up-x " + this.CamUp.X); // Camera Up Vector X Component
        arguments.Append(" --cam-target-z " + this.CamTarget.Z); // Camera Target Position Z Component
        arguments.Append(" --cam-target-y " + this.CamTarget.Y); // Camera Target Position Y Component
        arguments.Append(" --cam-target-x " + this.CamTarget.X); // Camera Target Position X Component
        arguments.Append(" --cam-pos-z " + this.CamPos.Z); // Camera Position Z Component
        arguments.Append(" --cam-pos-y " + this.CamPos.Y); // Camera Position Y Component
        arguments.Append(" --cam-pos-x " + this.CamPos.X); // Camera Position X Component

        if (this.Quiet)
        {
            arguments.Append(" -q"); // Start Application without splash and minimized window
        }

        // arguments.Append(" -g "+gpu); // (accepted multiple times)); add GPU device to use for rendering (0 = first)
        arguments.Append(" -s " + this.MaxSamples); // Maximum number of samples per pixel (maxsamples)
        if (!string.IsNullOrEmpty(this.OutputFile))
        {
            arguments.Append(" -o " + this.OutputFile); // Output PNG imagefile when maxsamples is reached
        }

        // arguments.Append(" --film-height " + FilmHeight); // Film height
        // arguments.Append(" --film-width " + FilmWidth); // Film width
        if (this.Exit)
        {
            arguments.Append(" -e"); // Close the application when rendering is done
        }

        // arguments.Append(" --"); // Ignores the rest of the labeled arguments following this flag.
        // arguments.Append(" --version"); // Displays version information and exits.
        // arguments.Append(" -h"); // Displays usage information and exits.
        if (!this.IsNewProject && !string.IsNullOrEmpty(this.ProjectFile))
        {
            arguments.Append(" " + this.ProjectFile); // .OCS Project scene file
        }

        var psi = new ProcessStartInfo
        {
            WorkingDirectory = Directory.GetCurrentDirectory(),
            FileName = this.OctaneExecutable,
            Arguments = arguments.ToString().Replace(',', '.')
        };
        return Process.Start(psi);
    }

    /// <summary>
    /// Switches the YZ coordinates of the specified point.
    /// </summary>
    /// <param name="p">The point.</param>
    /// <returns>The transformed point.</returns>
    private Point3D SwitchYZ(Point3D p)
    {
        return new Point3D(p.X, p.Z, -p.Y);
    }
}

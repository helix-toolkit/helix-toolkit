using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides a command that raises an event returning the hit models at the mouse location when the mouse button is clicked.
/// </summary>
public sealed class PointSelectionCommand : SelectionCommand
{
    /// <summary>
    /// The position
    /// </summary>
    private Point position;

    /// <summary>
    /// Initializes a new instance of the <see cref="PointSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
    public PointSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler)
        : base(viewport, modelsSelectedEventHandler, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
    public PointSelectionCommand(Viewport3D viewport, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
        : base(viewport, null, visualsSelectedEventHandler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointSelectionCommand" /> class.
    /// </summary>
    /// <param name="viewport">The viewport.</param>
    /// <param name="modelsSelectedEventHandler">The selection event handler.</param>
    /// <param name="visualsSelectedEventHandler">The selection event handler.</param>
    public PointSelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> modelsSelectedEventHandler, EventHandler<VisualsSelectedEventArgs> visualsSelectedEventHandler)
        : base(viewport, modelsSelectedEventHandler, visualsSelectedEventHandler)
    {
    }

    /// <summary>
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
    protected override void Started(ManipulationEventArgs e)
    {
        base.Started(e);
        this.position = e.CurrentPosition;

        var res = this.Viewport.FindHits(this.position);
        var selectedModels = res?.Select(hit => hit.Model).ToList() ?? new();
        var selectedVisuals = res?.Select(hit => hit.Visual).ToList() ?? new();
        this.OnModelsSelected(new ModelsSelectedByPointEventArgs(selectedModels, this.position));
        this.OnVisualsSelected(new VisualsSelectedByPointEventArgs(selectedVisuals, this.position));
    }

    /// <summary>
    /// The customized complete operation when the manipulation is completed.
    /// </summary>
    /// <param name="e">
    /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
    /// </param>
    protected override void Completed(ManipulationEventArgs e)
    {
        // do not raise event here
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>A cursor.</returns>
    protected override Cursor GetCursor()
    {
        return Cursors.Arrow;
    }
}

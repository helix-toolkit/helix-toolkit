// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

 namespace Workitem10048
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Extensions;

    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            // titles
            this.Title = "Simple Demo (Workitem 10048 and 10052)";
            this.SubTitle = "Select lines with left mouse button.\nRotate or zoom around a point on a line if the cursor is above one.";

            if (this.RenderTechniquesManager != null)
            {
                // default render technique
                this.RenderTechnique = RenderTechniquesManager.RenderTechniques.Get(DefaultRenderTechniqueNames.Blinn);
            }
        }
    }
}
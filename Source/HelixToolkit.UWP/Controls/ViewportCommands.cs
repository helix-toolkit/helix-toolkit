// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportCommands.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    public static class ViewportCommands
    {
        public enum Id
        {
            Zoom,
            ZoomExtents,
            ZoomRectangle,
            Pan,
            Rotate,
            SetTarget,
            Reset,
            ChangeFieldOfView,
            BackView,
            FrontView,
            TopView,
            BottomView,
            LeftView,
            RightView,
        }

        public static ViewportCommand Zoom { get; } = ViewportCommand.Get(Id.Zoom);
        public static ViewportCommand ZoomExtents { get; } = ViewportCommand.Get(Id.ZoomExtents);
        public static ViewportCommand ZoomRectangle { get; } = ViewportCommand.Get(Id.ZoomRectangle);
        public static ViewportCommand Pan { get; } = ViewportCommand.Get(Id.Pan);
        public static ViewportCommand Rotate { get; } = ViewportCommand.Get(Id.Rotate);
        public static ViewportCommand SetTarget { get; } = ViewportCommand.Get(Id.SetTarget);
        public static ViewportCommand Reset { get; } = ViewportCommand.Get(Id.Reset);
        public static ViewportCommand ChangeFieldOfView { get; } = ViewportCommand.Get(Id.ChangeFieldOfView);
        public static ViewportCommand BackView { get; } = ViewportCommand.Get(Id.BackView);
        public static ViewportCommand FrontView { get; } = ViewportCommand.Get(Id.FrontView);
        public static ViewportCommand TopView { get; } = ViewportCommand.Get(Id.TopView);
        public static ViewportCommand BottomView { get; } = ViewportCommand.Get(Id.BottomView);
        public static ViewportCommand LeftView { get; } = ViewportCommand.Get(Id.LeftView);
        public static ViewportCommand RightView { get; } = ViewportCommand.Get(Id.RightView);
    }
}
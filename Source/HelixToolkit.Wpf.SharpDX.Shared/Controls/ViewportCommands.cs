// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportCommands.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX
{
    public static class ViewportCommands
    {
        public static RoutedCommand Zoom
        {
            get
            {
                return zoom;
            }
        }
        public static RoutedCommand ZoomExtents
        {
            get
            {
                return zoomExtents;
            }
        }
        public static RoutedCommand ZoomRectangle
        {
            get
            {
                return zoomRectangle;
            }
        }
        public static RoutedCommand Pan
        {
            get
            {
                return pan;
            }
        }
        public static RoutedCommand Rotate
        {
            get
            {
                return rotate;
            }
        }
        public static RoutedCommand SetTarget
        {
            get
            {
                return setTarget;
            }
        }
        public static RoutedCommand Reset
        {
            get
            {
                return reset;
            }
        }
        public static RoutedCommand ChangeFieldOfView
        {
            get
            {
                return changeFieldOfView;
            }
        }
        public static RoutedCommand BackView
        {
            get
            {
                return backView;
            }
        }
        public static RoutedCommand FrontView
        {
            get
            {
                return frontView;
            }
        }
        public static RoutedCommand TopView
        {
            get
            {
                return topView;
            }
        }
        public static RoutedCommand BottomView
        {
            get
            {
                return bottomView;
            }
        }
        public static RoutedCommand LeftView
        {
            get
            {
                return leftView;
            }
        }
        public static RoutedCommand RightView
        {
            get
            {
                return rightView;
            }
        }

        private static RoutedCommand zoom = new RoutedCommand();
        private static RoutedCommand zoomExtents = new RoutedCommand();
        private static RoutedCommand zoomRectangle = new RoutedCommand();
        private static RoutedCommand pan = new RoutedCommand();
        private static RoutedCommand rotate = new RoutedCommand();
        private static RoutedCommand setTarget = new RoutedCommand();
        private static RoutedCommand reset = new RoutedCommand();
        private static RoutedCommand changeFieldOfView = new RoutedCommand();
        private static RoutedCommand backView = new RoutedCommand();
        private static RoutedCommand frontView = new RoutedCommand();
        private static RoutedCommand topView = new RoutedCommand();
        private static RoutedCommand bottomView = new RoutedCommand();
        private static RoutedCommand leftView = new RoutedCommand();
        private static RoutedCommand rightView = new RoutedCommand();
    }
}
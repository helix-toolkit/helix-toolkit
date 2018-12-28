using System;
using System.Collections.Generic;
using SharpDX;
using System.Runtime.CompilerServices;
using System.Linq;

namespace HelixToolkit.SharpDX.Core
{
    using Controls;
    using Model.Scene;
    using Cameras;

    public static class ViewportExtensions
    {
        /// <summary>
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="delta">The delta.</param>
        public static void ZoomByChangingFieldOfView(this CameraController controller, float delta)
        {
            var viewport = controller.Viewport;
            if(viewport.CameraCore is PerspectiveCameraCore pcamera)
            {
                float fov = pcamera.FieldOfView;
                float d = pcamera.LookDirection.Length();
                float r = d * (float)Math.Tan(0.5f * fov / 180 * Math.PI);

                fov *= 1f + (delta * 0.5f);
                if (fov < controller.MinimumFieldOfView)
                {
                    fov = controller.MinimumFieldOfView;
                }

                if (fov > controller.MaximumFieldOfView)
                {
                    fov = controller.MaximumFieldOfView;
                }

                pcamera.FieldOfView = fov;
                float d2 = r / (float)Math.Tan(0.5f * fov / 180 * Math.PI);
                var newLookDirection = pcamera.LookDirection;
                newLookDirection.Normalize();
                newLookDirection *= (float)d2;
                var target = pcamera.Position + pcamera.LookDirection;
                pcamera.Position = target - newLookDirection;
                pcamera.LookDirection = newLookDirection;
            }
        }

        /// <summary>
        /// Zooms the viewport to the specified rectangle.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void ZoomToRectangle(this ViewportCore viewport, RectangleF rectangle)
        {
            viewport.CameraCore.ZoomToRectangle(viewport, rectangle);
        }
    }
}

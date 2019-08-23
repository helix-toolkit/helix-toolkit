// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OctreeHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using SharpDX;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    public static class OctreeHelper
    {
        public static LineGeometry3D CreateOctreeLineModel(this IDynamicOctree tree)
        {
            var builder = new LineBuilder();
            CreateOctreeLineModel(tree, builder);
            return builder.ToLineGeometry3D();
        }

        public static void CreateOctreeLineModel(this IDynamicOctree tree, LineBuilder builder)
        {
            if (tree == null) return;
            var box = tree.Bound;
            Vector3[] verts = new Vector3[8];
            verts[0] = box.Minimum;
            verts[1] = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z); //Z
            verts[2] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z); //Y
            verts[3] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z); //X

            verts[7] = box.Maximum;
            verts[4] = new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z); //Z
            verts[5] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z); //Y
            verts[6] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z); //X
            builder.AddLine(verts[0], verts[1]);
            builder.AddLine(verts[0], verts[2]);
            builder.AddLine(verts[0], verts[3]);
            builder.AddLine(verts[7], verts[4]);
            builder.AddLine(verts[7], verts[5]);
            builder.AddLine(verts[7], verts[6]);

            builder.AddLine(verts[1], verts[6]);
            builder.AddLine(verts[1], verts[5]);
            builder.AddLine(verts[4], verts[2]);
            builder.AddLine(verts[4], verts[3]);
            builder.AddLine(verts[2], verts[6]);
            builder.AddLine(verts[3], verts[5]);

            if (tree.HasChildren)
            {
                foreach (IDynamicOctree child in tree.ChildNodes)
                {
                    if (child != null)
                    {
                        CreateOctreeLineModel(child, builder);
                    }
                }
            }
        }

        public static LineGeometry3D CreatePathLines(this IList<BoundingBox> path)
        {
            Vector3[] verts = new Vector3[8];
            var builder = new LineBuilder();
            foreach (var box in path)
            {
                verts[0] = box.Minimum;
                verts[1] = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z); //Z
                verts[2] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z); //Y
                verts[3] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z); //X

                verts[7] = box.Maximum;
                verts[4] = new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z); //Z
                verts[5] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z); //Y
                verts[6] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z); //X
                builder.AddLine(verts[0], verts[1]);
                builder.AddLine(verts[0], verts[2]);
                builder.AddLine(verts[0], verts[3]);
                builder.AddLine(verts[7], verts[4]);
                builder.AddLine(verts[7], verts[5]);
                builder.AddLine(verts[7], verts[6]);

                builder.AddLine(verts[1], verts[6]);
                builder.AddLine(verts[1], verts[5]);
                builder.AddLine(verts[4], verts[2]);
                builder.AddLine(verts[4], verts[3]);
                builder.AddLine(verts[2], verts[6]);
                builder.AddLine(verts[3], verts[5]);
            }
            return builder.ToLineGeometry3D();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit.Wpf
{
    public interface IHitResult
    {
        public Model3D? Model { get; }
        public Visual3D? Visual { get; }
    }

    /// <summary>
    /// Represents a rectangle hit result.
    /// </summary>
    public sealed class RectangleHitResult : IHitResult
    {
        /// <summary>
        /// Gets the hit model.
        /// </summary>
        public Model3D? Model { get; }

        /// <summary>
        /// Gets the hit visual.
        /// </summary>
        public Visual3D? Visual { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleHitResult" /> class.
        /// </summary>
        /// <param name="model">The hit model.</param>
        /// <param name="visual">The hit visual.</param>
        public RectangleHitResult(Model3D? model, Visual3D? visual)
        {
            this.Model = model;
            this.Visual = visual;
        }
    }

    /// <summary>
    /// A hit result.
    /// </summary>
    public sealed class PointHitResult : IHitResult
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public Model3D? Model
        {
            get
            {
                return this.RayHit.ModelHit;
            }
        }

        /// <summary>
        /// Gets the visual.
        /// </summary>
        /// <value>The visual.</value>
        public Visual3D? Visual
        {
            get
            {
                return this.RayHit.VisualHit;
            }
        }
        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance { get; }

        /// <summary>
        /// Gets the mesh.
        /// </summary>
        /// <value>The mesh.</value>
        public MeshGeometry3D Mesh
        {
            get
            {
                return this.RayHit.MeshHit;
            }
        }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position { get; }

        /// <summary>
        /// Gets the ray hit.
        /// </summary>
        /// <value>The ray hit.</value>
        public RayMeshGeometry3DHitTestResult RayHit { get; }

        public PointHitResult(RayMeshGeometry3DHitTestResult rayHit, double distance, Point3D position, Vector3D normal)
        {
            this.RayHit = rayHit;
            this.Distance = distance;
            this.Position = position;
            this.Normal = normal;
        }
    }
}

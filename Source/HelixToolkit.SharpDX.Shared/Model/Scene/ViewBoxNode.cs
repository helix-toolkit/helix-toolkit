/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct3D11;
using HelixToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using Shaders;

    /// <summary>
    /// 
    /// </summary>
    public class ViewBoxNode : ScreenSpacedNode
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public sealed class ViewBoxClickedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the look direction.
            /// </summary>
            /// <value>The look direction.</value>
            public Vector3 LookDirection { get; private set; }

            /// <summary>
            /// Gets up direction.
            /// </summary>
            /// <value>Up direction.</value>
            public Vector3 UpDirection { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ViewBoxClickedEventArgs"/> class.
            /// </summary>
            /// <param name="lookDir">The look dir.</param>
            /// <param name="upDir">Up dir.</param>
            public ViewBoxClickedEventArgs(Vector3 lookDir, Vector3 upDir)
            {
                LookDirection = lookDir;
                UpDirection = upDir;
            }
        }
        /// <summary>
        /// Occurs when [on view box clicked].
        /// </summary>
        public event EventHandler<ViewBoxClickedEventArgs> OnViewBoxClicked;

        private Stream viewboxTexture;
        /// <summary>
        /// Gets or sets the view box texture.
        /// </summary>
        /// <value>
        /// The view box texture.
        /// </value>
        public Stream ViewBoxTexture
        {
            set
            {
                if (Set(ref viewboxTexture, value))
                {
                    UpdateTexture(value);
                }
            }
            get { return viewboxTexture; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable edge click].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable edge click]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableEdgeClick
        {
            set
            {
                CornerModel.Visible = EdgeModel.Visible = value;
            }
            get
            {
                return CornerModel.Visible;
            }
        }

        private Vector3 upDirection = new Vector3(0, 1, 0);
        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public Vector3 UpDirection
        {
            set
            {
                if (Set(ref upDirection, value))
                {
                    UpdateModel(value);
                }
            }
            get
            {
                return upDirection;
            }
        }
        #endregion

        #region Fields
        private const float size = 5;

        private static readonly Vector3[] xAligned = { new Vector3(0, -1, -1), new Vector3(0, 1, -1), new Vector3(0, -1, 1), new Vector3(0, 1, 1) }; //x
        private static readonly Vector3[] yAligned = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };//y
        private static readonly Vector3[] zAligned = { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0) };//z

        private static readonly Vector3[] cornerPoints =   {
                new Vector3(-1,-1,-1 ), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1),
                new Vector3(-1,-1,1 ),new Vector3(1,-1,1 ),new Vector3(1,1,1 ),new Vector3(-1,1,1 )};

        private static readonly Matrix[] cornerInstances;
        private static readonly Matrix[] edgeInstances;
        private static readonly Geometry3D cornerGeometry;
        private static readonly Geometry3D edgeGeometry;

        private readonly MeshNode ViewBoxMeshModel;
        private readonly InstancingMeshNode EdgeModel;
        private readonly InstancingMeshNode CornerModel;

        private bool isRightHanded = true;
        #endregion

        static ViewBoxNode()
        {
            var builder = new MeshBuilder(true, false);
            float cornerSize = size / 5;
            builder.AddBox(Vector3.Zero, cornerSize, cornerSize, cornerSize);
            cornerGeometry = builder.ToMesh();

            builder = new MeshBuilder(true, false);
            float halfSize = size / 2;
            float edgeSize = halfSize * 1.5f;
            builder.AddBox(Vector3.Zero, cornerSize, edgeSize, cornerSize);
            edgeGeometry = builder.ToMesh();

            cornerInstances = new Matrix[cornerPoints.Length];
            for (int i = 0; i < cornerPoints.Length; ++i)
            {
                cornerInstances[i] = Matrix.CreateTranslation(cornerPoints[i] * size / 2 * 0.95f);
            }
            int count = xAligned.Length;
            edgeInstances = new Matrix[count * 3];

            for (int i = 0; i < count; ++i)
            {
                edgeInstances[i] = Matrix.CreateRotationZ((float)Math.PI / 2) * Matrix.CreateTranslation(xAligned[i] * halfSize * 0.95f);
            }
            for (int i = count; i < count * 2; ++i)
            {
                edgeInstances[i] = Matrix.CreateTranslation(yAligned[i % count] * halfSize * 0.95f);
            }
            for (int i = count * 2; i < count * 3; ++i)
            {
                edgeInstances[i] = Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(zAligned[i % count] * halfSize * 0.95f);
            }
        }

        public ViewBoxNode()
        {
            RelativeScreenLocationX = 0.8f;
            ViewBoxMeshModel = new MeshNode() { EnableViewFrustumCheck = false, CullMode = CullMode.Back };
            ViewBoxMeshModel.RenderType = RenderType.ScreenSpaced;
            var sampler = DefaultSamplers.LinearSamplerWrapAni1;
            sampler.BorderColor = Color.Gray;
            sampler.AddressU = sampler.AddressV = sampler.AddressW = TextureAddressMode.Border;
            this.AddChildNode(ViewBoxMeshModel);
            ViewBoxMeshModel.Material = new ViewCubeMaterialCore()
            {
                DiffuseColor = Color.White,
                DiffuseMapSampler = sampler
            };

            CornerModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new DiffuseMaterialCore() { DiffuseColor = Color.Yellow },
                Geometry = cornerGeometry,
                Instances = cornerInstances,
                Visible = false
            };
            CornerModel.RenderType = RenderType.ScreenSpaced;
            this.AddChildNode(CornerModel);

            EdgeModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new DiffuseMaterialCore() { DiffuseColor = Color.Silver },
                Geometry = edgeGeometry,
                Instances = edgeInstances,
                Visible = false
            };
            EdgeModel.RenderType = RenderType.ScreenSpaced;
            this.AddChildNode(EdgeModel);
            UpdateModel(UpDirection);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                var material = (ViewBoxMeshModel.Material as ViewCubeMaterialCore);
                if (material.DiffuseMap == null)
                {
                    material.DiffuseMap = ViewBoxTexture ?? BitmapExtensions.CreateViewBoxTexture(host.EffectsManager,
                        "F", "B", "L", "R", "U", "D", Color.Red, Color.Red, Color.Blue, Color.Blue, Color.Green, Color.Green,
                        Color.White, Color.White, Color.White, Color.White, Color.White, Color.White);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnCoordinateSystemChanged(bool e)
        {
            if (isRightHanded != e)
            {
                isRightHanded = e;
                UpdateModel(UpDirection);
            }
        }

        private void UpdateTexture(Stream texture)
        {
            (ViewBoxMeshModel.Material as PhongMaterialCore).DiffuseMap = texture;
        }

        protected void UpdateModel(Vector3 up)
        {
            var left = new Vector3(up.Y, up.Z, up.X);
            var front = Vector3.Cross(left, up);
            if (!isRightHanded)
            {
                front *= -1;
                left *= -1;
            }
            var builder = new MeshBuilder(true, true, false);
            builder.AddCubeFace(new Vector3(0, 0, 0), front, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -front, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -left, up, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), up, left, size, size, size);
            builder.AddCubeFace(new Vector3(0, 0, 0), -up, -left, size, size, size);

            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);

            var pts = new List<Vector3>();

            var center = up * -size / 2 * 1.1f;
            int phi = 24;
            for (int i = 0; i < phi; i++)
            {
                double angle = 0 + (360 * i / (phi - 1));
                double angleRad = angle / 180 * Math.PI;
                var dir = (left * (float)Math.Cos(angleRad)) + (front * (float)Math.Sin(angleRad));
                pts.Add(center + (dir * (size - 0.75f)));
                pts.Add(center + (dir * (size + 1.1f)));
            }
            builder = new MeshBuilder(false, false, false);
            builder.AddTriangleStrip(pts);
            var pie = builder.ToMesh();
            int count = pie.Indices.Count;
            for (int i = 0; i < count;)
            {
                var v1 = pie.Indices[i++];
                var v2 = pie.Indices[i++];
                var v3 = pie.Indices[i++];
                pie.Indices.Add(v1);
                pie.Indices.Add(v3);
                pie.Indices.Add(v2);
            }
            var newMesh = MeshGeometry3D.Merge(new MeshGeometry3D[] { pie, mesh });

            if (!isRightHanded)
            {
                for (int i = 0; i < newMesh.Positions.Count; ++i)
                {
                    var p = newMesh.Positions[i];
                    p.Z *= -1;
                    newMesh.Positions[i] = p;
                }
            }

            newMesh.TextureCoordinates = new Vector2Collection(Enumerable.Repeat(new Vector2(-1, -1), pie.Positions.Count));
            newMesh.Colors = new Color4Collection(Enumerable.Repeat(new Color4(1f, 1f, 1f, 1f), pie.Positions.Count));
            newMesh.TextureCoordinates.AddRange(mesh.TextureCoordinates);
            newMesh.Colors.AddRange(Enumerable.Repeat(new Color4(1, 1, 1, 1), mesh.Positions.Count));
            newMesh.Normals = newMesh.CalculateNormals();
            ViewBoxMeshModel.Geometry = newMesh;
        }

        private static void CreateTextureCoordinates(MeshGeometry3D mesh)
        {
            int faces = 6;
            int segment = 4;
            float inc = 1f / faces;

            for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
            {
                mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i / segment), mesh.TextureCoordinates[i].Y);
            }
        }

        protected override bool CanHitTest(RenderContext context)
        {
            return context != null;
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            var p = Vector4.Transform(new Vector4(ray.Position, 1), context.ScreenViewProjectionMatrix);
            if (Math.Abs(p.W) > 1e-7)
            {
                p /= p.W;
            }
            else
            {
                return false;
            }
            var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
            float viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale;
            var px = p.X - (float)(context.ActualWidth / 2 * (1 + screenSpaceCore.RelativeScreenLocationX) - viewportSize / 2);
            var py = p.Y - (float)(context.ActualHeight / 2 * (1 - screenSpaceCore.RelativeScreenLocationY) - viewportSize / 2);
            if (px < 0 || py < 0 || px > viewportSize || py > viewportSize)
            {
                return false;
            }
            var viewMatrix = screenSpaceCore.GlobalTransform.View;
            Vector3 v = new Vector3();

            var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
            var aspectRatio = screenSpaceCore.ScreenRatio;
            var projMatrix = screenSpaceCore.GlobalTransform.Projection;
            Vector3 zn;
            v.X = (2 * px / viewportSize - 1) / projMatrix.M11;
            v.Y = -(2 * py / viewportSize - 1) / projMatrix.M22;
            v.Z = 1 / projMatrix.M33;
            Vector3Helper.TransformCoordinate(ref v, ref matrix, out Vector3 zf);
            if (screenSpaceCore.IsPerspective)
            {
                zn = screenSpaceCore.GlobalTransform.EyePos;
            }
            else
            {
                v.Z = 0;
                Vector3Helper.TransformCoordinate(ref v, ref matrix, out zn);
            }

            Vector3 r = Vector3.Normalize(zf - zn);

            ray = new Ray(zn, r);
            List<HitTestResult> viewBoxHit = new List<HitTestResult>();

            if (base.OnHitTest(context, totalModelMatrix, ref ray, ref viewBoxHit))
            {
                hits?.Clear();
                hits = viewBoxHit;
                Debug.WriteLine("View box hit.");
                var hit = viewBoxHit[0];
                Vector3 normal = Vector3.Zero;
                int inv = isRightHanded ? 1 : -1;
                if (hit.ModelHit == ViewBoxMeshModel)
                {
                    normal = -hit.NormalAtHit * inv;
                    //Fix the normal if returned normal is reversed
                    if(Vector3.Dot(normal, context.Camera.LookDirection) < 0)
                    {
                        normal *= -1;
                    }
                }
                else if (hit.Tag is int index)
                {
                    if (hit.ModelHit == EdgeModel && index < edgeInstances.Length)
                    {
                        Matrix transform = edgeInstances[index];
                        normal = -transform.Translation;
                    }
                    else if (hit.ModelHit == CornerModel && index < cornerInstances.Length)
                    {
                        Matrix transform = cornerInstances[index];
                        normal = -transform.Translation;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                normal = Vector3.Normalize(normal);
                if (Vector3.Cross(normal, UpDirection).LengthSquared() < 1e-5)
                {
                    var vecLeft = new Vector3(-normal.Y, -normal.Z, -normal.X);
                    OnViewBoxClicked?.Invoke(this, new ViewBoxClickedEventArgs(normal, vecLeft));
                }
                else
                {
                    OnViewBoxClicked?.Invoke(this, new ViewBoxClickedEventArgs(normal, UpDirection));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
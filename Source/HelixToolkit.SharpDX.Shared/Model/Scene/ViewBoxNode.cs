/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct3D11;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Shaders;
    using Core;
    #region Properties
    /// <summary>
    /// 
    /// </summary>
    public class ViewBoxNode : ScreenSpacedNode
    {
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
                cornerInstances[i] = Matrix.Translation(cornerPoints[i] * size / 2 * 0.95f);
            }
            int count = xAligned.Length;
            edgeInstances = new Matrix[count * 3];

            for (int i = 0; i < count; ++i)
            {
                edgeInstances[i] = Matrix.RotationZ((float)Math.PI / 2) * Matrix.Translation(xAligned[i] * halfSize * 0.95f);
            }
            for (int i = count; i < count * 2; ++i)
            {
                edgeInstances[i] = Matrix.Translation(yAligned[i % count] * halfSize * 0.95f);
            }
            for (int i = count * 2; i < count * 3; ++i)
            {
                edgeInstances[i] = Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(zAligned[i % count] * halfSize * 0.95f);
            }
        }

        public ViewBoxNode()
        {
            RelativeScreenLocationX = 0.8f;
            ViewBoxMeshModel = new MeshNode() { EnableViewFrustumCheck = false };
            ViewBoxMeshModel.RenderCore.RenderType = RenderType.ScreenSpaced;
            var sampler = DefaultSamplers.LinearSamplerWrapAni1;
            sampler.BorderColor = Color.Gray;
            sampler.AddressU = sampler.AddressV = sampler.AddressW = TextureAddressMode.Border;

            ViewBoxMeshModel.CullMode = CullMode.Back;
            ViewBoxMeshModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.ViewCube]; };
            this.AddChildNode(ViewBoxMeshModel);
            ViewBoxMeshModel.Material = new PhongMaterialCore()
            {
                DiffuseColor = Color.White,
                DiffuseMapSampler = sampler
            };

            CornerModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new PhongMaterialCore() { DiffuseColor = Color.Yellow },
                Geometry = cornerGeometry,
                Instances = cornerInstances,
                Visible = false
            };
            CornerModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Diffuse]; };
            CornerModel.RenderCore.RenderType = RenderType.ScreenSpaced;
            this.AddChildNode(CornerModel);

            EdgeModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new PhongMaterialCore() { DiffuseColor = Color.Silver },
                Geometry = edgeGeometry,
                Instances = edgeInstances,
                Visible = false
            };
            EdgeModel.OnSetRenderTechnique = (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Diffuse]; };
            EdgeModel.RenderCore.RenderType = RenderType.ScreenSpaced;
            this.AddChildNode(EdgeModel);
            UpdateModel(UpDirection);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                var material = (ViewBoxMeshModel.Material as PhongMaterialCore);
                if (material.DiffuseMap == null)
                {
                    material.DiffuseMap = ViewBoxTexture == null ? BitmapExtensions.CreateViewBoxTexture(host.EffectsManager,
                        "F", "B", "L", "R", "U", "D", Color.Red, Color.Red, Color.Blue, Color.Blue, Color.Green, Color.Green,
                        Color.White, Color.White, Color.White, Color.White, Color.White, Color.White) : ViewBoxTexture;
                }
                return true;
            }
            else
            {
                return false;
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
            for (int i = 0; i < count; i += 3)
            {
                pie.Indices.Add(pie.Indices[i + 2]);
                pie.Indices.Add(pie.Indices[i + 1]);
                pie.Indices.Add(pie.Indices[i]);
            }

            var newMesh = MeshGeometry3D.Merge(new MeshGeometry3D[] { pie, mesh });

            newMesh.TextureCoordinates = new Core.Vector2Collection(Enumerable.Repeat(new Vector2(-1, -1), pie.Positions.Count));
            newMesh.Colors = new Core.Color4Collection(Enumerable.Repeat(new Color4(1f, 1f, 1f, 1f), pie.Positions.Count));
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

        protected override bool CanHitTest(IRenderContext context)
        {
            return context != null;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
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
            Vector3 zn, zf;
            v.X = (2 * px / viewportSize - 1) / projMatrix.M11;
            v.Y = -(2 * py / viewportSize - 1) / projMatrix.M22;
            v.Z = 1 / projMatrix.M33;
            Vector3.TransformCoordinate(ref v, ref matrix, out zf);
            if (screenSpaceCore.IsPerspective)
            {
                zn = screenSpaceCore.GlobalTransform.EyePos;
            }
            else
            {
                v.Z = 0;
                Vector3.TransformCoordinate(ref v, ref matrix, out zn);
            }

            Vector3 r = zf - zn;
            r.Normalize();

            ray = new Ray(zn, r);
            List<HitTestResult> viewBoxHit = new List<HitTestResult>();

            if (base.OnHitTest(context, totalModelMatrix, ref ray, ref viewBoxHit))
            {
                hits?.Clear();
                hits = viewBoxHit;
                Debug.WriteLine("View box hit.");
                var hit = viewBoxHit[0];
                Vector3 normal = Vector3.Zero;
                if (hit.ModelHit == ViewBoxMeshModel)
                {
                    normal = -hit.NormalAtHit;
                }
                else if (hit.Tag is int)
                {
                    int index = (int)hit.Tag;
                    if (hit.ModelHit == EdgeModel && index < edgeInstances.Length)
                    {
                        Matrix transform = edgeInstances[index];
                        normal = -transform.TranslationVector;
                    }
                    else if (hit.ModelHit == CornerModel && index < cornerInstances.Length)
                    {
                        Matrix transform = cornerInstances[index];
                        normal = -transform.TranslationVector;
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
                normal.Normalize();
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
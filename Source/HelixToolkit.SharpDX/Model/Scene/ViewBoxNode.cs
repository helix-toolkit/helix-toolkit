using HelixToolkit.Geometry;
using HelixToolkit.SharpDX.Shaders;
using Microsoft.Extensions.Logging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class ViewBoxNode : ScreenSpacedNode
{
    static readonly ILogger logger = Logger.LogManager.Create<ViewBoxNode>();
    #region Properties
    private TextureModel? viewboxTexture;
    /// <summary>
    /// Gets or sets the view box texture.
    /// </summary>
    /// <value>
    /// The view box texture.
    /// </value>
    public TextureModel? ViewBoxTexture
    {
        set
        {
            if (Set(ref viewboxTexture, value))
            {
                UpdateTexture(value);
            }
        }
        get
        {
            return viewboxTexture;
        }
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

    private Vector3 upDirection = new(0, 1, 0);
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
    private List<HitTestResult> hitsInternal = new();
    #endregion

    static ViewBoxNode()
    {
        var builder = new MeshBuilder(true, false);
        var cornerSize = size / 5;
        builder.AddBox(Vector3.Zero, cornerSize, cornerSize, cornerSize);
        cornerGeometry = builder.ToMeshGeometry3D();

        builder = new MeshBuilder(true, false);
        var halfSize = size / 2;
        var edgeSize = halfSize * 1.5f;
        builder.AddBox(Vector3.Zero, cornerSize, edgeSize, cornerSize);
        edgeGeometry = builder.ToMeshGeometry3D();

        cornerInstances = new Matrix[cornerPoints.Length];
        for (var i = 0; i < cornerPoints.Length; ++i)
        {
            cornerInstances[i] = Matrix.CreateTranslation(cornerPoints[i] * size / 2 * 0.95f);
        }
        var count = xAligned.Length;
        edgeInstances = new Matrix[count * 3];

        for (var i = 0; i < count; ++i)
        {
            edgeInstances[i] = Matrix.CreateRotationZ((float)Math.PI / 2) * Matrix.CreateTranslation(xAligned[i] * halfSize * 0.95f);
        }
        for (var i = count; i < count * 2; ++i)
        {
            edgeInstances[i] = Matrix.CreateTranslation(yAligned[i % count] * halfSize * 0.95f);
        }
        for (var i = count * 2; i < count * 3; ++i)
        {
            edgeInstances[i] = Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(zAligned[i % count] * halfSize * 0.95f);
        }
    }

    public ViewBoxNode()
    {
        CameraType = ScreenSpacedCameraType.Perspective;
        RelativeScreenLocationX = 0.8f;
        ViewBoxMeshModel = new MeshNode() { EnableViewFrustumCheck = false, CullMode = CullMode.Back };
        var sampler = DefaultSamplers.LinearSamplerWrapAni1;
        sampler.BorderColor = Color.Gray.ToColor4().ToStruct<Color4, RawColor4>();
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
        this.AddChildNode(CornerModel);

        EdgeModel = new InstancingMeshNode()
        {
            EnableViewFrustumCheck = false,
            Material = new DiffuseMaterialCore() { DiffuseColor = Color.Silver },
            Geometry = edgeGeometry,
            Instances = edgeInstances,
            Visible = false
        };
        this.AddChildNode(EdgeModel);
        UpdateModel(UpDirection);
    }

    protected override bool OnAttach(IEffectsManager effectsManager)
    {
        if (base.OnAttach(effectsManager))
        {
            var material = ViewBoxMeshModel.Material as ViewCubeMaterialCore;
            if (material is not null && material.DiffuseMap == null)
            {
                if (ViewBoxTexture is not null)
                {
                    material.DiffuseMap = ViewBoxTexture;
                }
                else
                {
                    var model = BitmapExtensions.CreateViewBoxTexture(
                        effectsManager,
                        "F", "B", "L", "R", "U", "D",
                        Color.Red, Color.Red, Color.Blue, Color.Blue, Color.Green, Color.Green,
                        Color.White, Color.White, Color.White, Color.White, Color.White, Color.White);

                    material.DiffuseMap = model is null ? null : new TextureModel(model, true);
                }
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

    private void UpdateTexture(TextureModel? texture)
    {
        if (ViewBoxMeshModel.Material is ViewCubeMaterialCore material)
            material.DiffuseMap = texture;
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

        var mesh = builder.ToMeshGeometry3D();
        CreateTextureCoordinates(mesh);

        var pts = new List<Vector3>();

        var center = up * -size / 2 * 1.1f;
        var phi = 24;
        for (var i = 0; i < phi; i++)
        {
            double angle = 0 + (360 * i / (phi - 1));
            var angleRad = angle / 180 * Math.PI;
            var dir = (left * (float)Math.Cos(angleRad)) + (front * (float)Math.Sin(angleRad));
            pts.Add(center + (dir * (size - 0.75f)));
            pts.Add(center + (dir * (size + 1.1f)));
        }
        builder = new MeshBuilder(false, false, false);
        builder.AddTriangleStrip(pts.ToList());
        var pie = builder.ToMeshGeometry3D();
        var count = pie.Indices?.Count ?? 0;
        for (var i = 0; i < count;)
        {
            var v1 = pie.Indices![i++];
            var v2 = pie.Indices![i++];
            var v3 = pie.Indices![i++];
            pie.Indices!.Add(v1);
            pie.Indices!.Add(v3);
            pie.Indices!.Add(v2);
        }
        var newMesh = MeshGeometry3D.Merge(new MeshGeometry3D[] { pie, mesh });

        if (!isRightHanded && newMesh.Positions is not null)
        {
            for (var i = 0; i < newMesh.Positions.Count; ++i)
            {
                var p = newMesh.Positions[i];
                p.Z *= -1;
                newMesh.Positions[i] = p;
            }
        }

        newMesh.TextureCoordinates = pie.Positions is null ? null : new Vector2Collection(Enumerable.Repeat(new Vector2(-1, -1), pie.Positions.Count));
        newMesh.Colors = pie.Positions is null ? null : new Color4Collection(Enumerable.Repeat(new Color4(1f, 1f, 1f, 1f), pie.Positions.Count));
        if (mesh.TextureCoordinates is not null)
        {
            newMesh.TextureCoordinates?.AddRange(mesh.TextureCoordinates);
        }
        if (mesh.Positions is not null)
        {
            newMesh.Colors?.AddRange(Enumerable.Repeat(new Color4(1, 1, 1, 1), mesh.Positions.Count));
        }
        newMesh.UpdateNormals();
        ViewBoxMeshModel.Geometry = newMesh;
    }

    private static void CreateTextureCoordinates(MeshGeometry3D mesh)
    {
        var faces = 6;
        var segment = 4;
        var inc = 1f / faces;

        if (mesh.TextureCoordinates is null)
        {
            return;
        }

        for (var i = 0; i < mesh.TextureCoordinates.Count; ++i)
        {
            mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i / segment), mesh.TextureCoordinates[i].Y);
        }
    }

    protected override bool CanHitTest(HitTestContext? context)
    {
        return context != null;
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        if (context is null)
        {
            return false;
        }

        if (base.OnHitTest(context, totalModelMatrix, ref hitsInternal))
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("View box hit.");
            }
            var hit = hitsInternal.OrderBy(x => x.Distance).FirstOrDefault();
            if (hit == null)
            { return false; }
            Vector3 normal = Vector3.Zero;
            int inv = isRightHanded ? 1 : -1;
            if (hit.ModelHit == ViewBoxMeshModel)
            {
                normal = -hit.NormalAtHit * inv;
                //Fix the normal if returned normal is reversed
                if (context.RenderMatrices is not null && Vector3.Dot(normal, context.RenderMatrices.CameraParams.LookAtDir) < 0)
                {
                    normal *= -1;
                }
            }
            else if (hit.Tag is int index)
            {
                if (hit.ModelHit == EdgeModel && index < edgeInstances.Length)
                {
                    var transform = edgeInstances[index];
                    normal = -transform.Translation;
                }
                else if (hit.ModelHit == CornerModel && index < cornerInstances.Length)
                {
                    var transform = cornerInstances[index];
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
            hit.NormalAtHit = normal;
            hit.ModelHit = this;
            hit.Tag = this.Tag;
            hits.Add(hit);
            hitsInternal.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }
}

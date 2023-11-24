using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Windows.Media.Media3D;

namespace Cloth;

public sealed partial class Flag : ObservableObject
{
    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public double FPS { get; private set; }

    private readonly VerletIntegrator integrator;
    public Vector3D Gravity = new(0, 0, -9);

    public Vector3D Wind
    {
        get
        {
            double dir = WindDirection / 180 * Math.PI;
            return new Vector3D(Math.Cos(dir) * WindSpeed, Math.Sin(dir) * WindSpeed, 0);
        }
    }

    public double Damping { get; set; }

    private readonly int m = 48;
    private readonly int n = 32;
    private readonly double normalforcecoeff = 10.0;
    private readonly double relax = 1.5;
    private readonly double tangentforcecoeff = 0.1;

    public Flag(string image)
    {
        Mesh = new MeshGeometry3D();
        Material = MaterialHelper.CreateImageMaterial(image);
        Damping = 0.98;
        integrator = new VerletIntegrator() { Iterations = 4, Damping = this.Damping };
        WindSpeed = 6;
        WindDirection = 180;
        PoleHeight = 12;
        Height = 3;
        Length = 4;
        Mass = 0.8;
    }

    public MeshGeometry3D Mesh { get; set; }
    public Material? Material { get; set; }

    public double Height { get; set; }
    public double Length { get; set; }
    public double PoleHeight { get; set; }

    private double Mass { get; set; }


    public void Init()
    {
        CreateInitialMesh();

        integrator.Init(Mesh);

        CreateConstraints();
        // integrator.AddSphereConstraint(new Point3D(-3,0.3,10),0.4);
        integrator.SetInverseMass(1 / Mass);
        for (int i = 0; i < n; i++)
            integrator.FixPosition(i * m);
    }

    private void CreateConstraints()
    {
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                int ij = i * m + j;
                if (j + 1 < m)
                    integrator.AddConstraint(ij, ij + 1, relax);
                if (i + 1 < n)
                    integrator.AddConstraint(ij, ij + m, relax);
            }
    }

    private void CreateInitialMesh()
    {
        var pts = new System.Numerics.Vector3[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                pts[i, j] = new System.Numerics.Vector3((float)(-Length * j / (m - 1)), 0, (float)(PoleHeight - Height * i / (n - 1)));
            }

        var mb = new MeshBuilder(false, true);
        mb.AddRectangularMesh(pts, null, false, false);
        Mesh = mb.ToMesh().ToMeshGeometry3D();
    }

    private Vector3D[] CalculateNormals()
    {
        var pts = integrator.Positions;
        var normals = new Vector3D[n * m];
        for (int i = 0; i < n; i++)
        {
            int i1 = i + 1;
            if (i1 == n) i1--;
            int i0 = i1 - 1;
            for (int j = 0; j < m; j++)
            {
                int j1 = j + 1;
                if (j1 == m) j1--;
                int j0 = j1 - 1;
                var normal = Vector3D.CrossProduct(Point3D.Subtract(pts[i1 * m + j0], pts[i0 * m + j0]),
                                                   Point3D.Subtract(pts[i0 * m + j1], pts[i0 * m + j0]));
                normal.Normalize();
                normals[i * m + j] = normal;
            }
        }
        return normals;
    }

    private void AccumulateForces()
    {
        integrator.Damping = this.Damping;
        var wind = Wind;
        var gravity = Gravity;
        double mass = Mass;
        var normals = CalculateNormals();
        for (int i = 0; i < normals.Length; i++)
        {
            var n = normals[i];
            var F = n * Vector3D.DotProduct(n, wind) * normalforcecoeff + wind * tangentforcecoeff;
            F += gravity * mass;
            integrator.SetForce(i, F);
        }
    }

    public void Update(double dt)
    {
        UpdateFps(dt);

        AccumulateForces();
        integrator.TimeStep(dt);
    }

    private double timeFrames = 0;
    private int nFrames = 0;
    private void UpdateFps(double dt)
    {
        timeFrames += dt;
        nFrames++;
        if (timeFrames > 1)
        {
            FPS = nFrames / timeFrames;
            OnPropertyChanged(nameof(FPS));
            timeFrames = 0;
            nFrames = 0;
        }
    }

    public void Transfer()
    {
        integrator.TransferPositions(Mesh);
    }
}

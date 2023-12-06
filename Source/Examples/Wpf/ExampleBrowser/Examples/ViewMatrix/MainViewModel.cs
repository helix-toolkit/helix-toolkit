using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;

namespace ViewMatrix;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProjectionTransform))]
    private Matrix3D projectionMatrix;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ViewTransform))]
    private Matrix3D viewMatrix;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ViewportTransform))]
    private Matrix3D viewportMatrix;

    public MainViewModel()
    {
        var gm = new MeshBuilder();
        gm.AddBox(new Point3D(0, 0, 0.5).ToVector(), 1, 1, 1);
        gm.AddCylinder(new Point3D(5, 0, 0).ToVector(), new Point3D(5, 0, 5).ToVector(), 1, 36);
        this.Model = new GeometryModel3D(gm.ToMesh().ToMeshGeometry3D(true), Materials.Blue);
        this.Model.Freeze();
    }

    public Model3D Model { get; set; }

    public Transform3D ProjectionTransform
    {
        get
        {
            var m = Matrix3D.Identity;
            m.Append(this.ProjectionMatrix);
            m.Append(this.ViewMatrix);
            return new MatrixTransform3D(m);
        }
    }

    public Transform3D ViewTransform
    {
        get
        {
            var m = Matrix3D.Identity;
            m.Append(this.ProjectionMatrix);
            return new MatrixTransform3D(m);
        }
    }

    public Transform3D ViewportTransform
    {
        get
        {
            var m = Matrix3D.Identity;
            m.Append(this.ViewportMatrix);
            m.Append(this.ProjectionMatrix);
            m.Append(this.ViewMatrix);
            return new MatrixTransform3D(m);
        }
    }
}

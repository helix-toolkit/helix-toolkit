using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using HelixToolkit.Wpf.SharpDX.Core;
using System.Windows.Input;

namespace GroupElementTester
{
    public class MainViewModel : BaseViewModel
    {
        public LineGeometry3D AxisModel { get; private set; }
        public BillboardText3D AxisLabel { private set; get; }

        public MeshGeometry3D SphereModel { private set; get; }

        public MeshGeometry3D BoxModel { private set; get; }

        public PhongMaterial RedMaterial { get { return PhongMaterials.Red; } }

        public PhongMaterial BlueMaterial { get { return PhongMaterials.Blue; } }
        public PhongMaterial GreenMaterial { get { return PhongMaterials.Green; } }
        public Transform3D GroupModel3DTransform { get; } = new Media3D.TranslateTransform3D(5, 0, 0);

        public Transform3D Transform1 { get; } = new Media3D.TranslateTransform3D(0, 0, 0);

        public Transform3D Transform2 { get; } = new Media3D.TranslateTransform3D(-2, 0, 0);

        public Transform3D Transform3 { get; } = new Media3D.TranslateTransform3D(-4, 0, 0);

        public Transform3D Transform4 { get; } = new Media3D.TranslateTransform3D(-6, 0, 0);

        public ObservableElement3DCollection GroupModelSource { get; } = new ObservableElement3DCollection();

        public ICommand AddGroupModelCommand { get; private set; }

        public ICommand RemoveGroupModelCommand { private set; get; }

        public MainViewModel()
        {
            //    RenderTechniquesManager = new DefaultRenderTechniquesManager();           
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            // ----------------------------------------------
            // titles
            this.Title = "GroupElement Test";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(10, 2, 10), LookDirection = new Vector3D(-10, -2, -10), UpDirection = new Vector3D(0, 1, 0) };

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));
            AxisModel = lineBuilder.ToLineGeometry3D();
            AxisModel.Colors = new Color4Collection(AxisModel.Positions.Count);
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());

            AxisLabel = new BillboardText3D();
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(11, 0, 0), Text = "X", Foreground = Colors.Red.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 11, 0), Text = "Y", Foreground = Colors.Green.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 0, 11), Text = "Z", Foreground = Colors.Blue.ToColor4() });

            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5);
            SphereModel = meshBuilder.ToMesh();
            meshBuilder = new MeshBuilder(true);
            meshBuilder.AddBox(Vector3.Zero, 0.5, 0.5, 0.5);
            BoxModel = meshBuilder.ToMesh();
            AddGroupModelCommand = new RelayCommand(AddGroupModel);
            RemoveGroupModelCommand = new RelayCommand(RemoveGroupModel);
        }

        private void AddGroupModel(object o)
        {
            var model = new MeshGeometryModel3D();
            model.Geometry = SphereModel;
            model.Material = BlueMaterial;
            model.Transform = new Media3D.TranslateTransform3D((GroupModelSource.Count + 1) * 2, 0, 0);
            GroupModelSource.Add(model);
        }

        private void RemoveGroupModel(object o)
        {
            if (GroupModelSource.Count > 0)
            { GroupModelSource.RemoveAt(GroupModelSource.Count - 1); }
        }
    }
}

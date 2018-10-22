using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using HelixToolkit.Wpf.SharpDX.Model;

namespace VolumeRendering
{
    public class MainViewModel : BaseViewModel
    {
        private Material volumeMaterial;
        public Material VolumeMaterial
        {
            set
            {
                SetValue(ref volumeMaterial, value);
            }
            get { return volumeMaterial; }
        }


        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() { Position = new Point3D(0, 0, -5), LookDirection = new Vector3D(0, 0, 5), UpDirection = new Vector3D(0, 1, 0) };
            VolumeMaterial = LoadTeapot();
        }

        private Material LoadTeapot()
        {
            var m = new VolumeTextureRawDataMaterial();
            m.Texture = VolumeTextureRawDataMaterialCore.LoadRAWFile("teapot256x256x178.raw", 256, 256, 178);
            m.StepSize = 1.0 / 178;
            m.Iterations = 178;
            return m;
        }
    }
}

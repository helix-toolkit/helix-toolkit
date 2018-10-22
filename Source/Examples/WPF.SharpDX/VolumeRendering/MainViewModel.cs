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
using HelixToolkit.Wpf.SharpDX.Utilities;

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

        private Media3D.Transform3D transform;
        public Media3D.Transform3D Transform
        {
            set
            {
                SetValue(ref transform, value);
            }
            get { return transform; }
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
            //var m = new VolumeTextureDiffuseMaterial();
            //var data = VolumeTextureRawDataMaterialCore.LoadRAWFile("teapot256x256x178.raw", 256, 256, 178);
            //m.Texture = ProcessData(data.VolumeTextures, data.Width, data.Height, data.Depth);
            m.Color = new Color4(1, 1, 1, 0.1f);
            m.GradientMap = GetGradients(Colors.Red.ToColor4(), Colors.Blue.ToColor4(), 128).ToArray();
            return m;
        }

        private Material LoadSkull()
        {
            var m = new VolumeTextureDiffuseMaterial();
            var data = VolumeTextureRawDataMaterialCore.LoadRAWFile("male128x256x256.raw", 128, 256, 256);
            m.Texture = ProcessData(data.VolumeTextures, data.Width, data.Height, data.Depth);
            m.Color = new Color4(1, 1, 1, 1f);
            m.GradientMap = GetGradients(new Color4(1, 0, 0, 0.1f), new Color4(1, 1f, 1, 1f), 128).ToArray();
            return m;
        }

        private VolumeTextureGradientParams ProcessData(byte[] data, int width, int height, int depth)
        {
            float[] fdata = new float[data.Length];
            for(int i=0; i < data.Length; ++i)
            {
                fdata[i] = (float) data[i] / byte.MaxValue;
            }
            var gradients = VolumeDataHelper.GenerateGradients(fdata, width, height, depth, 1);
            VolumeDataHelper.FilterNxNxN(gradients, width, height, depth, 3);
            return new VolumeTextureGradientParams(gradients, width, height, depth);
        }

        public static IEnumerable<Color4> GetGradients(Color4 start, Color4 mid, Color4 end, int steps)
        {
            return GetGradients(start, mid, steps / 2).Concat(GetGradients(mid, end, steps / 2));
        }

        public static IEnumerable<Color4> GetGradients(Color4 start, Color4 end, int steps)
        {
            float stepA = ((end.Alpha - start.Alpha) / (steps - 1));
            float stepR = ((end.Red - start.Red) / (steps - 1));
            float stepG = ((end.Green - start.Green) / (steps - 1));
            float stepB = ((end.Blue - start.Blue) / (steps - 1));

            for (int i = 0; i < steps; i++)
            {
                yield return new Color4((start.Red + (stepR * i)),
                                            (start.Green + (stepG * i)),
                                            (start.Blue + (stepB * i)),
                                            (start.Alpha + (stepA * i)));
            }
        }
    }
}

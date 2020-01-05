using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Color4 = SharpDX.Color4;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System.Linq;


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

        public Geometry3D MeshModel { get; }

        public Material MeshMaterial { get; }

        public LineGeometry3D AxisModel { get; }

        public Material AxisModelMaterial { get; }

        private bool isLoading = false;
        public bool IsLoading
        {
            private set
            {
                SetValue(ref isLoading, value);
            }
            get { return isLoading; }
        }
        
        public ICommand LoadTeapotCommand { get; }
        public ICommand LoadSkullCommand { get; }
        public ICommand LoadCloudCommand { get; }
        public ICommand LoadBeetleCommand { get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() { Position = new Point3D(0, 0, -5), LookDirection = new Vector3D(0, 0, 5), UpDirection = new Vector3D(0, 1, 0) };
            LoadTeapotCommand = new RelayCommand((o) => { Load(0); });
            LoadSkullCommand = new RelayCommand((o) => { Load(1); });
            LoadCloudCommand = new RelayCommand((o) => { Load(2); });
            LoadBeetleCommand = new RelayCommand((o) => { Load(3); });
            var builder = new MeshBuilder();
            //builder.AddBox(new Vector3(0, 0, 0), 2, 2, 0.001);
            builder.AddSphere(Vector3.Zero, 0.1);
            builder.AddBox(Vector3.UnitX, 0.2, 0.2, 0.2);
            MeshModel = builder.ToMesh();
            MeshMaterial = PhongMaterials.Yellow;

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(Vector3.Zero, Vector3.UnitX * 1.5f);
            lineBuilder.AddLine(Vector3.Zero, Vector3.UnitY * 1.5f);
            lineBuilder.AddLine(Vector3.Zero, Vector3.UnitZ * 1.5f);
            AxisModel = lineBuilder.ToLineGeometry3D();
            AxisModel.Colors = new Color4Collection(AxisModel.Positions.Count);
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());
            AxisModelMaterial = new LineArrowHeadMaterial() { Color = Colors.White, ArrowSize = 0.05 };
            //Load(0);
        }

        private void Load(int idx)
        {
            if (IsLoading) { return; }
            IsLoading = true;
            Task.Run<Tuple<Material, Media3D.Transform3D>>(() => 
            {
                switch (idx)
                {
                    case 0:
                        return LoadTeapot();
                    case 1:
                        return LoadSkull();
                    case 2:
                        return LoadNoise();
                    case 3:
                        return LoadBeetle();
                }
                return null;
            }).ContinueWith((result)=> 
            {
                VolumeMaterial = result.Result.Item1.Clone() as Material;
                Transform = result.Result.Item2;
                IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Tuple<Material, Media3D.Transform3D> LoadTeapot()
        {
            //var m = new VolumeTextureRawDataMaterial();
            //m.Texture = VolumeTextureRawDataMaterialCore.LoadRAWFile("teapot256x256x178.raw", 256, 256, 178);
            var m = new VolumeTextureDiffuseMaterial();
            var data = VolumeTextureRawDataMaterialCore.LoadRAWFile("teapot256x256x178.raw", 256, 256, 178);
            m.Texture = ProcessData(data.VolumeTextures, data.Width, data.Height, data.Depth, out var transferMap);
            m.Color = new Color4(1, 1, 1, 0.4f);
            m.TransferMap = transferMap;
            m.Freeze();
            var scale = Matrix.Scaling(2, 2, 178 / 256f * 2);
            var rotate = Matrix.RotationAxis(new Vector3(1, 0, 0), (float)Math.PI);
            var t = new Media3D.MatrixTransform3D((scale * rotate).ToMatrix3D());
            t.Freeze();
            return new Tuple<Material, Media3D.Transform3D>(m, t);
        }

        private Tuple<Material, Media3D.Transform3D> LoadSkull()
        {
            var m = new VolumeTextureDiffuseMaterial();
            var data = VolumeTextureRawDataMaterialCore.LoadRAWFile("male128x256x256.raw", 128, 256, 256);
            m.Texture = ProcessData(data.VolumeTextures, data.Width, data.Height, data.Depth, out var transferMap);
            m.Color = new Color4(0.6f, 0.6f, 0.6f, 1f);
            m.TransferMap = transferMap;
            m.Freeze();
            var rotate = Matrix.RotationAxis(new Vector3(1, 0, 0), (float)Math.PI);
            var transform = new Media3D.MatrixTransform3D(rotate.ToMatrix3D());
            transform.Freeze();
            return new Tuple<Material, Media3D.Transform3D>(m, transform);
        }

        private Tuple<Material, Media3D.Transform3D> LoadNoise()
        {
            var m = new VolumeTextureDDS3DMaterial();
            m.Texture = LoadFileToMemory("NoiseVolume.dds");
            m.Color = new Color4(1, 1, 1, 0.01f);
            m.Freeze();
            var transform = new Media3D.ScaleTransform3D(1, 1, 1);
            transform.Freeze();
            return new Tuple<Material, Media3D.Transform3D>(m, transform);
        }

        private Tuple<Material, Media3D.Transform3D> LoadBeetle()
        {
            var data = ReadDat("stagbeetle208x208x123.dat", out var width, out var height, out var depth);
            var m = new VolumeTextureDiffuseMaterial();
            ushort max = data.Max();
            uint[] histogram = new uint[max+1];

            float[] fdata = new float[data.Length];
            
            for (int i = 0; i < data.Length; ++i)
            {
                fdata[i] = (float)data[i] / max;
                histogram[data[i]]++;
            }
            var transferMap = GetTransferFunction(histogram, data.Length, 1, 0.0001f);
            var gradients = VolumeDataHelper.GenerateGradients(fdata, width, height, depth, 1);
            VolumeDataHelper.FilterNxNxN(gradients, width, height, depth, 3);
            m.Texture = new VolumeTextureGradientParams(gradients, width, height, depth);           
            m.Color = new Color4(0, 1, 0, 0.4f);
            var transform = new Media3D.ScaleTransform3D(1, 1, 1);
            transform.Freeze();
            m.Freeze();
            return new Tuple<Material, Media3D.Transform3D>(m, transform);
        }

        private VolumeTextureGradientParams ProcessData(byte[] data, int width, int height, int depth, out Color4[] transferMap)
        {
            uint[] histogram = new uint[256];

            float[] fdata = new float[data.Length];
            for (int i=0; i < data.Length; ++i)
            {
                fdata[i] = (float) data[i] / byte.MaxValue;
                histogram[data[i]]++;
            }
            transferMap = GetTransferFunction(histogram, data.Length);
            var gradients = VolumeDataHelper.GenerateGradients(fdata, width, height, depth, 1);
            VolumeDataHelper.FilterNxNxN(gradients, width, height, depth, 3);      
            return new VolumeTextureGradientParams(gradients, width, height, depth);
        }

        private float[] normalize(byte[] data)
        {
            float[] fdata = new float[data.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                fdata[i] = (float)data[i] / byte.MaxValue;
            }
            return fdata;
        }

        private static readonly Color4[] ColorCandidates = new Color4[]
        {
            Colors.LightPink.ToColor4(),
            Colors.DarkGray.ToColor4(),
            Colors.Yellow.ToColor4(),
            Colors.Red.ToColor4(),
            Colors.Green.ToColor4(),            
        };
        /// <summary>
        /// Gets the transfer function. Please create your own color transfer map.
        /// </summary>
        /// <param name="histogram">The histogram.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public static Color4[] GetTransferFunction(uint[] histogram, int total, 
            float maxPercent = 0.003f, float minPercent = 0.0001f)
        {
            float[] percentage = new float[histogram.Length];
            for(int i=0; i < histogram.Length; ++i)
            {
                percentage[i] = (float)histogram[i] / total;
                if(percentage[i] > maxPercent || percentage[i] < minPercent)
                {
                    percentage[i] = 0;
                }
            }
            Color4[] ret = new Color4[histogram.Length];
            int counter = 0;
            bool isZero = true;
            for(int i=0; i < percentage.Length; ++i)
            {
                if(percentage[i] > 0)
                {
                    ret[i] = ColorCandidates[counter];
                    isZero = false;
                }

                if (!isZero && percentage[i] == 0)
                {
                    counter = (counter + 1) % ColorCandidates.Length;
                    isZero = true;
                }
            }
            return ret;
        }

        private static ushort[] ReadDat(string file, out int width, out int height, out int depth)
        {
            using (var f = File.OpenRead(file))
            {
                using (var stream = new BinaryReader(f))
                {
                    width = stream.ReadUInt16();
                    height = stream.ReadUInt16();
                    depth = stream.ReadUInt16();
                    return stream.ReadUInt16(width * height * depth);
                }
            }
        }
    }
}

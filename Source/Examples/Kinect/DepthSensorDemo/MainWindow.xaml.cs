using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DepthSensorDemo
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using Microsoft.Kinect;

    // http://www.microsoft.com/en-us/kinectforwindows/
    // http://www.kinectforwindows.org
    // http://en.wikipedia.org/wiki/Kinect
    // http://www.kinecthacks.com/
    // http://openkinect.org
    // http://www.i-programmer.info/programming/hardware/2623-getting-started-with-microsoft-kinect-sdk.html
    // http://campar.in.tum.de/twiki/pub/Chair/TeachingSs11Kinect/2011-DSensors_LabCourse_Kinect.pdf
    // http://www.isprs.org/proceedings/XXXVIII/5-W12/Papers/ls2011_submission_40.pdf

    // http://www.youtube.com/watch?v=7QrnwoO1-8A
    // http://www.youtube.com/watch?v=Xq91aMwYezU
    // http://www.youtube.com/watch?v=ttMHme2EI9I&feature=relmfu
    // http://www.youtube.com/watch?v=5-w7UXCAUJE

    // The depth stream format is hard-coded to 640x480, reduce this if this makes it too slow

    // Keys:
    //   Space: pause color and depth stream
    //   Enter: pause depth stream only
    //   E: export mesh to an .obj file


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.WindowLoaded;
            this.Closed += this.WindowClosed;
            this.KeyDown += this.MainWindow_KeyDown;
            this.DataContext = this;
            this.Model = new GeometryModel3D();
            this.IsColorStreamUpdating = true;
            this.IsDepthStreamUpdating = true;
            CompositionTarget.Rendering += this.CompositionTarget_Rendering;
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                this.IsColorStreamUpdating = !this.IsColorStreamUpdating;
                this.IsDepthStreamUpdating = !this.IsDepthStreamUpdating;
            }

            if (e.Key == Key.C)
            {
                this.IsColorStreamUpdating = !this.IsColorStreamUpdating;
            }

            if (e.Key == Key.D)
            {
                this.IsDepthStreamUpdating = !this.IsDepthStreamUpdating;
            }

            if (e.Key == Key.E)
            {
                this.ExportModel();
            }
        }

        private void ExportModel(string fileName = "model.obj")
        {
            using (var exporter = new ObjExporter(fileName))
            {
                exporter.Export(this.Model);
            }

            Process.Start("explorer.exe", "/select,\"" + fileName + "\"");
        }

        public bool IsColorStreamUpdating { get; set; }

        public bool IsDepthStreamUpdating { get; set; }

        public GeometryModel3D Model { get; set; }

        private HashSet<KinectSensor> sensors = new HashSet<KinectSensor>();

        private bool updateColorData;

        private bool updateDepthData;

        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        private ColorImageFormat lastColorImageFormat = ColorImageFormat.Undefined;

        private byte[] colorPixelData;

        private WriteableBitmap outputImage;

        private DepthImageFormat lastDepthImageFormat;

        private short[] depthPixelData;

        private int[] rawDepth;

        private Point3D[] depthFramePoints;

        private Point[] textureCoordinates;


        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.updateColorData = true;
            this.updateDepthData = true;
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            this.KinectStart();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            this.KinectStop();
        }

        private void KinectStart()
        {
            // listen to any status change for Kinects.
            KinectSensor.KinectSensors.StatusChanged += this.KinectsStatusChanged;

            // show status for each sensor that is found now.
            foreach (var kinect in KinectSensor.KinectSensors)
            {
                this.ShowStatus(kinect, kinect.Status);
            }
        }

        private void KinectStop()
        {
            foreach (var sensor in this.sensors)
            {
                this.StopSensor(sensor);
            }

            this.sensors.Clear();
        }

        private void KinectsStatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.ShowStatus(e.Sensor, e.Status);
        }

        private void ShowStatus(KinectSensor sensor, KinectStatus status)
        {
            Debug.WriteLine(sensor.DeviceConnectionId + ": " + status);
            switch (status)
            {
                case KinectStatus.Disconnected:
                case KinectStatus.NotPowered:
                    if (this.sensors.Contains(sensor))
                    {
                        this.sensors.Remove(sensor);
                        this.StopSensor(sensor);
                    }

                    break;
                default:
                    if (!this.sensors.Contains(sensor))
                    {
                        this.sensors.Add(sensor);
                        this.StartSensor(sensor);
                    }

                    break;
            }
        }

        private void StartSensor(KinectSensor sensor)
        {
            sensor.ColorFrameReady += this.OnColorFrameReady;
            sensor.DepthFrameReady += this.OnDepthFrameReady;

            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            // sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

            sensor.Start();
        }

        private void StopSensor(KinectSensor sensor)
        {
            sensor.ColorFrameReady -= this.OnColorFrameReady;
            sensor.DepthFrameReady -= this.OnDepthFrameReady;
            sensor.Stop();
        }

        private void OnColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (!this.updateColorData || !this.IsColorStreamUpdating)
            {
                return;
            }

            this.updateColorData = false;

            using (var imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    // Detect if the format has changed.
                    bool haveNewFormat = this.lastColorImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.colorPixelData = new byte[imageFrame.PixelDataLength];
                    }

                    imageFrame.CopyPixelDataTo(this.colorPixelData);

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (haveNewFormat)
                    {
                        this.outputImage = new WriteableBitmap(
                            imageFrame.Width,
                            imageFrame.Height,
                            96,  // DpiX
                            96,  // DpiY
                            PixelFormats.Bgr32,
                            null);
                    }

                    this.outputImage.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                        this.colorPixelData,
                        imageFrame.Width * Bgr32BytesPerPixel,
                        0);

                    this.lastColorImageFormat = imageFrame.Format;

                    // update the material of the mesh
                    var material = new DiffuseMaterial(new ImageBrush(this.outputImage));
                    this.Model.Material = this.Model.BackMaterial = material;
                }
            }
        }

        void OnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (!this.updateDepthData || !this.IsDepthStreamUpdating)
            {
                return;
            }

            this.updateDepthData = false;

            using (var imageFrame = e.OpenDepthImageFrame())
            {
                if (imageFrame != null)
                {
                    bool haveNewFormat = this.lastDepthImageFormat != imageFrame.Format;

                    if (haveNewFormat)
                    {
                        this.depthPixelData = new short[imageFrame.PixelDataLength];
                        this.rawDepth = new int[imageFrame.PixelDataLength];
                        this.depthFramePoints = new Point3D[imageFrame.PixelDataLength];
                        this.textureCoordinates = new Point[imageFrame.PixelDataLength];
                        this.lastDepthImageFormat = imageFrame.Format;

                        // create the texture coordinates
                        // todo: correct the registration with the color image
                        int height = imageFrame.Height;
                        int width = imageFrame.Width;
                        for (int iy = 0; iy < height; iy++)
                            for (int ix = 0; ix < width; ix++)
                                this.textureCoordinates[iy * width + ix] = new Point((double)ix / (width - 1), (double)iy / (height - 1));
                    }

                    imageFrame.CopyPixelDataTo(this.depthPixelData);

                    this.ConvertDepthFrame(this.depthPixelData, ((KinectSensor)sender).DepthStream);

                    this.Model.Geometry = this.CreateMesh(imageFrame.Width, imageFrame.Height);
                }
            }
        }

        /// <summary>
        /// Creates a mesh from the depthFramePoints.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depthDifferenceTolerance">The depth difference tolerance.</param>
        /// <returns>A mesh geometry.</returns>
        private MeshGeometry3D CreateMesh(int width, int height, double depthDifferenceTolerance = 200)
        {
            var triangleIndices = new List<int>();
            for (int iy = 0; iy + 1 < height; iy++)
            {
                for (int ix = 0; ix + 1 < width; ix++)
                {
                    int i0 = (iy * width) + ix;
                    int i1 = (iy * width) + ix + 1;
                    int i2 = ((iy + 1) * width) + ix + 1;
                    int i3 = ((iy + 1) * width) + ix;

                    var d0 = this.rawDepth[i0];
                    var d1 = this.rawDepth[i1];
                    var d2 = this.rawDepth[i2];
                    var d3 = this.rawDepth[i3];

                    var dmax0 = Math.Max(Math.Max(d0, d1), d2);
                    var dmin0 = Math.Min(Math.Min(d0, d1), d2);
                    var dmax1 = Math.Max(d0, Math.Max(d2, d3));
                    var dmin1 = Math.Min(d0, Math.Min(d2, d3));

                    if (dmax0 - dmin0 < depthDifferenceTolerance && dmin0 != -1)
                    {
                        triangleIndices.Add(i0);
                        triangleIndices.Add(i1);
                        triangleIndices.Add(i2);
                    }

                    if (dmax1 - dmin1 < depthDifferenceTolerance && dmin1 != -1)
                    {
                        triangleIndices.Add(i0);
                        triangleIndices.Add(i2);
                        triangleIndices.Add(i3);
                    }
                }
            }

            return new MeshGeometry3D()
                {
                    Positions = new Point3DCollection(this.depthFramePoints),
                    TextureCoordinates = new PointCollection(this.textureCoordinates),
                    TriangleIndices = new Int32Collection(triangleIndices)
                };
        }

        double ConvertRawDepthToMeters(int rawDepth)
        {
            // http://nicolas.burrus.name/index.php/Research/KinectCalibration
            // http://www.ros.org/wiki/kinect_node
            if (rawDepth < 2047)
            {
                return 1.0 / (rawDepth * -0.0030711016 + 3.3309495161);
            }
            return 0;
        }

        /// <summary>
        /// Converts a 16-bit grayscale depth frame which includes player indexes into a 3D point array.
        /// </summary>
        /// <param name="depthFrame">The depth frame.</param>
        /// <param name="depthStream">The depth stream.</param>
        private void ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream)
        {
            int tooNearDepth = depthStream.TooNearDepth;
            int tooFarDepth = depthStream.TooFarDepth;
            int unknownDepth = depthStream.UnknownDepth;

            int width = depthStream.FrameWidth;
            int height = depthStream.FrameHeight;

            int cx = width / 2;
            int cy = height / 2;

            double fxinv = 1.0 / 476;
            double fyinv = 1.0 / 476;

            double scale = 0.001;

            Parallel.For(
                0,
                height,
                iy =>
                {
                    for (int ix = 0; ix < width; ix++)
                    {
                        int i = (iy * width) + ix;
                        this.rawDepth[i] = depthFrame[(iy * width) + ix] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                        if (rawDepth[i] == unknownDepth || rawDepth[i] < tooNearDepth || rawDepth[i] > tooFarDepth)
                        {
                            this.rawDepth[i] = -1;
                            this.depthFramePoints[i] = new Point3D();
                        }
                        else
                        {
                            double zz = this.rawDepth[i] * scale;
                            double x = (cx - ix) * zz * fxinv;
                            double y = zz;
                            double z = (cy - iy) * zz * fyinv;
                            this.depthFramePoints[i] = new Point3D(x, y, z);
                        }
                    }
                });
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

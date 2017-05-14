using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ParticleSystemDemo
{
    public class MainViewModel : BaseViewModel
    {
        public Stream ParticleTexture { set; get; }

        private Vector3D acceleration = new Vector3D(0, 1, 0);
        public Vector3D Acceleration
        {
            set
            {
                SetValue(ref acceleration, value);
            }
            get
            {
                return acceleration;
            }
        }

        private int accelerationX = 0;
        public int AccelerationX
        {
            set
            {
                if(SetValue(ref accelerationX, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationX;
            }
        }

        private Size particlesize = new Size(0.1, 0.1);
        public Size ParticleSize
        {
            set
            {
                SetValue(ref particlesize, value);
            }
            get
            {
                return particlesize;
            }
        }

        private int sizeSlider = 10;
        public int SizeSlider
        {
            set
            {
                if(SetValue(ref sizeSlider, value))
                {
                    ParticleSize = new Size(((double)value)/100, ((double)value)/100);
                }
            }
            get
            {
                return sizeSlider;
            }
        }

        private int accelerationY = 100;
        public int AccelerationY
        {
            set
            {
                if (SetValue(ref accelerationY, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationY;
            }
        }

        private int accelerationZ = 0;
        public int AccelerationZ
        {
            set
            {
                if (SetValue(ref accelerationZ, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationZ;
            }
        }

        const int DefaultBoundScale = 10;
        public LineGeometry3D BoundingLines { private set; get; }

        public ScaleTransform3D BoundingLineTransform { private set; get; } = new ScaleTransform3D(DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);

        private Rect3D particleBounds = new Rect3D(0, 0, 0, DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);
        public Rect3D ParticleBounds
        {
            set
            {
                SetValue(ref particleBounds, value);
            }
            get
            {
                return particleBounds;
            }
        }

        private int boundScale = DefaultBoundScale;
        public int BoundScale
        {
            set
            {
                if(SetValue(ref boundScale, value))
                {
                    ParticleBounds = new Rect3D(0, 0, 0, value, value, value);
                    BoundingLineTransform.ScaleX = BoundingLineTransform.ScaleY = BoundingLineTransform.ScaleZ = value;
                }
            }
            get
            {
                return boundScale;
            }
        }

        public MainViewModel()
        {
            ParticleTexture = new FileStream(new System.Uri(@"Snowflake.png", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open);

            var lineBuilder = new LineBuilder();
            lineBuilder.AddBox(new SharpDX.Vector3(), 1, 1, 1);
            BoundingLines = lineBuilder.ToLineGeometry3D();
        }

        private void UpdateAcceleration()
        {
            Acceleration = new Vector3D((double)AccelerationX/100, (double)AccelerationY /100, (double)AccelerationZ /100);
        }
    }
}

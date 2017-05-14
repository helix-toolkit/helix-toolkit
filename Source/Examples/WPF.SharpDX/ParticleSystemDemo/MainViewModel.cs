using DemoCore;
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


        public MainViewModel()
        {
            ParticleTexture = new FileStream(new System.Uri(@"Snowflake.png", System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open);
        }

        private void UpdateAcceleration()
        {
            Acceleration = new Vector3D((double)AccelerationX/100, (double)AccelerationY /100, (double)AccelerationZ /100);
        }
    }
}

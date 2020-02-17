using GalaSoft.MvvmLight;
using HelixToolkit.UWP;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Media = Windows.UI;
using Vector3 = SharpDX.Vector3;

namespace SimpleDemoW10
{
    public class ParticleViewModel : ObservableObject
    {
        private double emitterRadius = 0.5;
        public double EmitterRadius
        {
            set
            {
                Set(ref emitterRadius, value);
            }
            get
            {
                return emitterRadius;
            }
        }

        private Vector3 emitterLocation = new Vector3(6, -4, 6);
        public Vector3 EmitterLocation
        {
            set
            {
                Set(ref emitterLocation, value);
            }
            get
            {
                return emitterLocation;
            }
        }

        private Vector3 consumerLocation = new Vector3(6, 8, 6);
        public Vector3 ConsumerLocation
        {
            set
            {
                Set(ref consumerLocation, value);
            }
            get
            {
                return consumerLocation;
            }
        }

        private double consumerRadius = 2;
        public double ConsumerRadius
        {
            set
            {
                Set(ref consumerRadius, value);
            }
            get
            {
                return consumerRadius;
            }
        }

        private TextureModel particleTexture;
        public TextureModel ParticleTexture
        {
            set
            {
                Set(ref particleTexture, value);
            }
            get
            {
                return particleTexture;
            }
        }

        private Vector3 acceleration = new Vector3(0, 1, 0);
        public Vector3 Acceleration
        {
            set
            {
                Set(ref acceleration, value);
            }
            get
            {
                return acceleration;
            }
        }

        private Size particlesize = new Size(0.5, 0.5);
        public Size ParticleSize
        {
            set
            {
                Set(ref particlesize, value);
            }
            get
            {
                return particlesize;
            }
        }

        const int DefaultBoundScale = 10;

        private BoundingBox particleBounds = new BoundingBox(new Vector3(-DefaultBoundScale, -DefaultBoundScale, -DefaultBoundScale), new Vector3(DefaultBoundScale, DefaultBoundScale, DefaultBoundScale));
        public BoundingBox ParticleBounds
        {
            set
            {
                Set(ref particleBounds, value);
            }
            get
            {
                return particleBounds;
            }
        }

        private Media.Color blendColor = Media.Colors.White;
        public Media.Color BlendColor
        {
            set
            {
                Set(ref blendColor, value);
            }
            get
            {
                return blendColor;
            }
        }

        private int numTextureRows = 4;
        public int NumTextureRows
        {
            set
            {
                Set(ref numTextureRows, value);
            }
            get
            {
                return numTextureRows;
            }
        }

        private int numTextureColumns = 4;
        public int NumTextureColumns
        {
            set
            {
                Set(ref numTextureColumns, value);
            }
            get
            {
                return numTextureColumns;
            }
        }

        private BlendOption sourceBlendOption = BlendOption.One;
        public BlendOption SourceBlendOption
        {
            set
            {
                Set(ref sourceBlendOption, value);
            }
            get
            {
                return sourceBlendOption;
            }
        }

        private BlendOption sourceAlphaBlendOption = BlendOption.One;
        public BlendOption SourceAlphaBlendOption
        {
            set
            {
                Set(ref sourceAlphaBlendOption, value);
            }
            get
            {
                return sourceAlphaBlendOption;
            }
        }

        private BlendOption destBlendOption = BlendOption.One;
        public BlendOption DestBlendOption
        {
            set
            {
                Set(ref destBlendOption, value);
            }
            get
            {
                return destBlendOption;
            }
        }

        private BlendOption destAlphaBlendOption = BlendOption.Zero;
        public BlendOption DestAlphaBlendOption
        {
            set
            {
                Set(ref destAlphaBlendOption, value);
            }
            get
            {
                return destAlphaBlendOption;
            }
        }

        public IList<Matrix> Instances { private set; get; }

        public ParticleViewModel()
        {
            ParticleTexture = LoadTexture("FXT_Sparks_01_Atlas_d.png");
        }

        private Stream LoadTexture(string file)
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            var bytecode = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\" + file);
            return new MemoryStream(bytecode);
        }
    }
}

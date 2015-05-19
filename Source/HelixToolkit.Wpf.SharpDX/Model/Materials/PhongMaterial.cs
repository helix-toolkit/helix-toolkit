// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhongMaterial.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Implments a phong-material with its all properties
//   Includes Diffuse, Normal, Displacement, Specular, etc. maps
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;
    using System.Windows.Media.Imaging;
    using System;    


    /// <summary>
    /// Implments a phong-material with its all properties
    /// Includes Diffuse, Normal, Displacement, Specular, etc. maps
    /// </summary>
    [Serializable]
    public partial class PhongMaterial : Material
    {
        private Color4 _ambientColor = Color.Gray;
        public Color4 _diffuseColor = Color.Gray;
        public Color4 _emissiveColor = Color.Black;
        public Color4 _reflectiveColor = new Color4(0.1f, 0.1f, 0.1f, 1f);
        public Color4 _specularColor = Color.Black;
        public float _specularShininess = 30f;
        public BitmapSource _diffuseMap;
        public BitmapSource _normalMap;
        public BitmapSource _displacementMap;

        /// <summary>
        /// Gets or sets a color that represents how the material reflects System.Windows.Media.Media3D.AmbientLight.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
        public Color4 AmbientColor
        {
            get { return _ambientColor; }
            set
            {
                _ambientColor = value;
                OnPropertyChanged("AmbientColor");
            }
        }

        /// <summary>
        /// Gets or sets the diffuse color for the material.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
        public Color4 DiffuseColor
        {
            get { return _diffuseColor; }
            set
            {
                _diffuseColor = value;
                OnPropertyChanged("DiffuseColor");
            }
        }

        /// <summary>
        /// Gets or sets the emissive color for the material.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
        public Color4 EmissiveColor
        {
            get { return _emissiveColor; }
            set
            {
                _emissiveColor = value;
                OnPropertyChanged("EmissiveColor");
            }
        }

        /// <summary>
        /// A fake parameter for reflectivity of the environment map
        /// </summary>
        public Color4 ReflectiveColor
        {
            get { return _reflectiveColor; }
            set
            {
                _reflectiveColor = value;
                OnPropertyChanged("ReflectiveColor");
            }
        }

        /// <summary>
        /// Gets or sets the specular color for the material.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
        public Color4 SpecularColor
        {
            get { return _specularColor; }
            set
            {
                _specularColor = value;
                OnPropertyChanged("SpecularColor");
            }
        }

        /// <summary>
        /// The power of specular reflections. 
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
        public float SpecularShininess
        {
            get { return _specularShininess; }
            set
            {
                _specularShininess = value;
                OnPropertyChanged("SpecularShininess");
            }
        }

        /// <summary>
        /// System.Windows.Media.Brush to be applied as a System.Windows.Media.Media3D.Material
        /// to a 3-D model.
        /// </summary>
        public BitmapSource DiffuseMap
        {
            get { return _diffuseMap; }
            set
            {
                _diffuseMap = value;
                OnPropertyChanged("DiffuseMap");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BitmapSource NormalMap
        {
            get { return _normalMap; }
            set
            {
                _normalMap = value;
                OnPropertyChanged("NormalMap");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BitmapSource DisplacementMap
        {
            get { return _displacementMap; }
            set
            {
                _displacementMap = value;
                OnPropertyChanged("DisplacementMap");
            }
        }

        public PhongMaterial Clone()
        {
            return new PhongMaterial()
            {
                AmbientColor = this.AmbientColor,
                DiffuseColor = this.DiffuseColor,
                DisplacementMap = this.DisplacementMap,
                EmissiveColor = this.EmissiveColor,
                Name = this.Name,
                NormalMap = this.NormalMap,
                ReflectiveColor = this.ReflectiveColor,
                SpecularColor = this.SpecularColor,
                SpecularShininess = this.SpecularShininess,
                DiffuseMap = this.DiffuseMap,
            };
        }


    }
}
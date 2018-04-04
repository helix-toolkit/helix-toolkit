/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using global::SharpDX;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    public class PhongMaterialCollection : ObservableCollection<PhongMaterial>
    {
        public PhongMaterialCollection()
        {
            Add(PhongMaterials.Black);
            Add(PhongMaterials.BlackPlastic);
            Add(PhongMaterials.BlackRubber);
            Add(PhongMaterials.Blue);
            Add(PhongMaterials.Brass);
            Add(PhongMaterials.Bronze);
            Add(PhongMaterials.Chrome);
            Add(PhongMaterials.Copper);
            Add(PhongMaterials.DefaultVRML);
            Add(PhongMaterials.Emerald);
            Add(PhongMaterials.Glass);
            Add(PhongMaterials.Gold);
            Add(PhongMaterials.Green);
            Add(PhongMaterials.Indigo);
            Add(PhongMaterials.Jade);
            Add(PhongMaterials.LightGray);
            Add(PhongMaterials.MediumGray);
            Add(PhongMaterials.Obsidian);
            Add(PhongMaterials.Orange);
            Add(PhongMaterials.Pearl);
            Add(PhongMaterials.Pewter);
            Add(PhongMaterials.PolishedBronze);
            Add(PhongMaterials.PolishedCopper);
            Add(PhongMaterials.PolishedGold);
            Add(PhongMaterials.PolishedSilver);
            Add(PhongMaterials.Red);
            Add(PhongMaterials.Ruby);
            Add(PhongMaterials.Silver);
            Add(PhongMaterials.Turquoise);
            Add(PhongMaterials.Violet);
            Add(PhongMaterials.White);
            Add(PhongMaterials.Yellow);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class PhongMaterials
    {
        public static PhongMaterialCollection Materials { get; private set; }

        public static PhongMaterial GetMaterial(string materialName)
        {
            var mat = Materials.FirstOrDefault(x => x.Name == materialName);
            return mat != null ? mat : PhongMaterials.DefaultVRML;
        }



        static PhongMaterials()
        {
            Materials = new PhongMaterialCollection();
        }

        public static Color4 ToColor(double r, double g, double b, double a = 1.0)
        {
            return FromScRgb((float)a, (float)r, (float)g, (float)b);
        }

        ///<summary>
        /// FromScRgb
        ///</summary>
        public static Color FromScRgb(float a, float r, float g, float b)
        {
            Color c1 = new Color();
            if (a < 0.0f)
            {
                a = 0.0f;
            }
            else if (a > 1.0f)
            {
                a = 1.0f;
            }

            c1.A = (byte)((a * 255.0f) + 0.5f);
            c1.R = ScRgbTosRgb(r);
            c1.G = ScRgbTosRgb(g);
            c1.B = ScRgbTosRgb(b);
            return c1;
        }

        ///<summary>
        /// private helper function to set context values from a color value with a set context and ScRgb values
        ///</summary>
        ///
        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (val <= 0.0031308)
            {
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            }
            else if (val < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow((double)val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }

        // factory
        [ThreadStatic]
        public static readonly PhongMaterial Red = new PhongMaterial
        {
            Name = "Red",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = Color.Red,
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Blue = new PhongMaterial
        {
            Name = "Blue",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = Color.Blue,
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Green = new PhongMaterial
        {
            Name = "Green",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = Color.Green,
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Orange = new PhongMaterial
        {
            Name = "Orange",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.992157, 0.513726, 0.0, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial BlanchedAlmond = new PhongMaterial
        {
            Name = "BlanchedAlmond",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = Color.BlanchedAlmond,
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Bisque = new PhongMaterial
        {
            Name = "Bisque",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = Color.Bisque,
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Yellow = new PhongMaterial
        {
            Name = "Yellow",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(1.0, 0.964706, 0.0, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Indigo = new PhongMaterial
        {
            Name = "Indigo",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.0980392, 0.0, 0.458824, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Violet = new PhongMaterial
        {
            Name = "Violet",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.635294, 0.0, 1.0, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial White = new PhongMaterial
        {
            Name = "White",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.992157, 0.992157, 0.992157, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial PureWhite = new PhongMaterial
        {
            Name = "PureWhite",
            AmbientColor = ToColor(1, 1, 1, 1.0),
            DiffuseColor = ToColor(1, 1, 1, 1.0),
            SpecularColor = ToColor(0.0, 0.0, 0.0, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 1000000f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Black = new PhongMaterial
        {
            Name = "Black",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Gray = new PhongMaterial
        {
            Name = "Gray",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.254902, 0.254902, 0.254902, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial MediumGray = new PhongMaterial
        {
            Name = "MediumGray",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.454902, 0.454902, 0.454902, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial LightGray = new PhongMaterial
        {
            Name = "LightGray",
            AmbientColor = ToColor(0.1, 0.1, 0.1, 1.0),
            DiffuseColor = ToColor(0.682353, 0.682353, 0.682353, 1.0),
            SpecularColor = ToColor(0.0225, 0.0225, 0.0225, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 12.8f,
        }.Clone();

        // Materials from: http://globe3d.sourceforge.net/g3d_html/gl-materials__ads.htm
        [ThreadStatic]
        public static readonly PhongMaterial Glass = new PhongMaterial
        {
            Name = "Glass",
            AmbientColor = ToColor(0.0, 0.0, 0.0, 1.0),
            DiffuseColor = ToColor(0.588235, 0.670588, 0.729412, 1.0),
            SpecularColor = ToColor(0.9, 0.9, 0.9, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 96.0f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Brass = new PhongMaterial
        {
            Name = "Brass",
            AmbientColor = ToColor(0.329412, 0.223529, 0.027451, 1.0),
            DiffuseColor = ToColor(0.780392, 0.568627, 0.113725, 1.0),
            SpecularColor = ToColor(0.992157, 0.941176, 0.807843, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 27.8974f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Bronze = new PhongMaterial
        {
            Name = "Bronze",
            AmbientColor = ToColor(0.2125, 0.1275, 0.054, 1.0),
            DiffuseColor = ToColor(0.714, 0.4284, 0.18144, 1.0),
            SpecularColor = ToColor(0.393548, 0.271906, 0.166721, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 25.6f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial PolishedBronze = new PhongMaterial
        {
            Name = "PolishedBronze",
            AmbientColor = ToColor(0.25, 0.148, 0.06475, 1.0),
            DiffuseColor = ToColor(0.4, 0.2368, 0.1036, 1.0),
            SpecularColor = ToColor(0.774597, 0.458561, 0.200621, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 76.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Chrome = new PhongMaterial
        {
            Name = "Chrome",
            AmbientColor = ToColor(0.25f, 0.25f, 0.25f, 1.0f),
            DiffuseColor = ToColor(0.4f, 0.4f, 0.4f, 1.0f),
            SpecularColor = ToColor(0.774597f, 0.774597f, 0.774597f, 1.0f),
            EmissiveColor = ToColor(0f, 0f, 0f, 0f),
            SpecularShininess = 76.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Copper = new PhongMaterial
        {
            Name = "Copper",
            AmbientColor = ToColor(0.19125, 0.0735, 0.0225, 1.0),
            DiffuseColor = ToColor(0.7038, 0.27048, 0.0828, 1.0),
            SpecularColor = ToColor(0.256777, 0.137622, 0.086014, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial PolishedCopper = new PhongMaterial
        {
            Name = "PolishedCopper",
            AmbientColor = ToColor(0.2295, 0.08825, 0.0275, 1.0),
            DiffuseColor = ToColor(0.5508, 0.2118, 0.066, 1.0),
            SpecularColor = ToColor(0.580594, 0.223257, 0.0695701, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 51.2f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Gold = new PhongMaterial
        {
            Name = "Gold",
            AmbientColor = ToColor(0.24725, 0.1995, 0.0745, 1.0),
            DiffuseColor = ToColor(0.75164, 0.60648, 0.22648, 1.0),
            SpecularColor = ToColor(0.628281, 0.555802, 0.366065, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 51.2f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial PolishedGold = new PhongMaterial
        {
            Name = "PolishedGold",
            AmbientColor = ToColor(0.24725, 0.2245, 0.0645, 1.0),
            DiffuseColor = ToColor(0.34615, 0.3143, 0.0903, 1.0),
            SpecularColor = ToColor(0.797357, 0.723991, 0.208006, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 83.2f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Pewter = new PhongMaterial
        {
            Name = "Pewter",
            AmbientColor = ToColor(0.105882, 0.058824, 0.113725, 1.0),
            DiffuseColor = ToColor(0.427451, 0.470588, 0.541176, 1.0),
            SpecularColor = ToColor(0.333333, 0.333333, 0.521569, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 9.84615f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Silver = new PhongMaterial
        {
            Name = "Silver",
            AmbientColor = ToColor(0.19225, 0.19225, 0.19225, 1.0),
            DiffuseColor = ToColor(0.50754, 0.50754, 0.50754, 1.0),
            SpecularColor = ToColor(0.508273, 0.508273, 0.508273, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 51.2f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial PolishedSilver = new PhongMaterial
        {
            Name = "PolishedSilver",
            AmbientColor = ToColor(0.23125, 0.23125, 0.23125, 1.0),
            DiffuseColor = ToColor(0.2775, 0.2775, 0.2775, 1.0),
            SpecularColor = ToColor(0.773911, 0.773911, 0.773911, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 89.6f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Emerald = new PhongMaterial
        {
            Name = "Emerald",
            AmbientColor = ToColor(0.0215, 0.1745, 0.0215, 0.55),
            DiffuseColor = ToColor(0.07568, 0.61424, 0.07568, 0.55),
            SpecularColor = ToColor(0.633, 0.727811, 0.633, 0.55),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 76.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Jade = new PhongMaterial
        {
            Name = "Jade",
            AmbientColor = ToColor(0.135, 0.2225, 0.1575, 0.95),
            DiffuseColor = ToColor(0.54, 0.89, 0.63, 0.95),
            SpecularColor = ToColor(0.316228, 0.316228, 0.316228, 0.95),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Obsidian = new PhongMaterial
        {
            Name = "Obsidian",
            AmbientColor = ToColor(0.05375, 0.05, 0.06625, 0.82),
            DiffuseColor = ToColor(0.18275, 0.17, 0.22525, 0.82),
            SpecularColor = ToColor(0.332741, 0.328634, 0.346435, 0.82),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 38.4f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Pearl = new PhongMaterial
        {
            Name = "Pearl",
            AmbientColor = ToColor(0.25, 0.20725, 0.20725, 0.922),
            DiffuseColor = ToColor(1.0, 0.829, 0.829, 0.922),
            SpecularColor = ToColor(0.296648, 0.296648, 0.296648, 0.922),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 11.264f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Ruby = new PhongMaterial
        {
            Name = "Ruby",
            AmbientColor = ToColor(0.1745, 0.01175, 0.01175, 0.55),
            DiffuseColor = ToColor(0.61424, 0.04136, 0.04136, 0.55),
            SpecularColor = ToColor(0.727811, 0.626959, 0.626959, 0.55),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 76.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial Turquoise = new PhongMaterial
        {
            Name = "Turquoise",
            AmbientColor = ToColor(0.1, 0.18725, 0.1745, 0.8),
            DiffuseColor = ToColor(0.396, 0.74151, 0.69102, 0.8),
            SpecularColor = ToColor(0.297254, 0.30829, 0.306678, 0.8),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 12.8f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial BlackPlastic = new PhongMaterial
        {
            Name = "BlackPlastic",
            AmbientColor = ToColor(0.0, 0.0, 0.0, 1.0),
            DiffuseColor = ToColor(0.01, 0.01, 0.01, 1.0),
            SpecularColor = ToColor(0.50, 0.50, 0.50, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 32f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial BlackRubber = new PhongMaterial
        {
            Name = "BlackRubber",
            AmbientColor = ToColor(0.02, 0.02, 0.02, 1.0),
            DiffuseColor = ToColor(0.01, 0.01, 0.01, 1.0),
            SpecularColor = ToColor(0.4, 0.4, 0.4, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 0.0),
            SpecularShininess = 10f,
        }.Clone();
        [ThreadStatic]
        public static readonly PhongMaterial DefaultVRML = new PhongMaterial
        {
            Name = "DefaultVRML",
            AmbientColor = ToColor(0.2, 0.2, 0.2, 1.0),
            DiffuseColor = ToColor(0.8, 0.8, 0.8, 1.0),
            SpecularColor = ToColor(0.0, 0.0, 0.0, 1.0),
            EmissiveColor = ToColor(0.0, 0.0, 0.0, 1.0),
            SpecularShininess = 25.6f,
        }.Clone();
    }
}

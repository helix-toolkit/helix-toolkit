﻿using SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public static class DiffuseMaterials
{
    public static DiffuseMaterialCollection Materials
    {
        get; private set;
    }

    public static DiffuseMaterial GetMaterial(string materialName)
    {
        var mat = Materials.FirstOrDefault(x => x.Name == materialName);
        return mat ?? DiffuseMaterials.DefaultVRML;
    }

    static DiffuseMaterials()
    {
        Materials = new DiffuseMaterialCollection();
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
        var c1 = new Color();
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
    public static DiffuseMaterial Red
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Red",
                DiffuseColor = Color.Red,
            };
        }
    }

    public static DiffuseMaterial Blue
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Blue",
                DiffuseColor = Color.Blue,
            };
        }
    }
    public static DiffuseMaterial LightBlue
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "LightBlue",
                DiffuseColor = Color.LightBlue,
            };
        }
    }
    public static DiffuseMaterial SkyBlue
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "SkyBlue",
                DiffuseColor = Color.SkyBlue,
            };
        }
    }
    public static DiffuseMaterial Green
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Green",
                DiffuseColor = Color.Green,
            };
        }
    }
    public static DiffuseMaterial LightGreen
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "LightGreen",
                DiffuseColor = Color.LightGreen,
            };
        }
    }
    public static DiffuseMaterial Orange
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Orange",
                DiffuseColor = ToColor(0.992157, 0.513726, 0.0, 1.0),
            };
        }
    }

    public static DiffuseMaterial BlanchedAlmond
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "BlanchedAlmond",
                DiffuseColor = Color.BlanchedAlmond,
            };
        }
    }

    public static DiffuseMaterial Bisque
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Bisque",
                DiffuseColor = Color.Bisque,
            };
        }
    }

    public static DiffuseMaterial Yellow
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Yellow",
                DiffuseColor = ToColor(1.0, 0.964706, 0.0, 1.0),
            };
        }
    }

    public static DiffuseMaterial Indigo
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Indigo",
                DiffuseColor = ToColor(0.0980392, 0.0, 0.458824, 1.0),
            };
        }
    }

    public static DiffuseMaterial Violet
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Violet",
                DiffuseColor = ToColor(0.635294, 0.0, 1.0, 1.0),
            };
        }
    }

    public static DiffuseMaterial White
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "White",
                DiffuseColor = ToColor(0.992157, 0.992157, 0.992157, 1.0),
            };
        }
    }

    public static DiffuseMaterial PureWhite
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "PureWhite",
                DiffuseColor = ToColor(1, 1, 1, 1.0),
            };
        }
    }

    public static DiffuseMaterial Black
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Black",
                DiffuseColor = ToColor(0.0, 0.0, 0.0, 1.0),
            };
        }
    }

    public static DiffuseMaterial Gray
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Gray",
                DiffuseColor = ToColor(0.254902, 0.254902, 0.254902, 1.0),
            };
        }
    }

    public static DiffuseMaterial MediumGray
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "MediumGray",
                DiffuseColor = ToColor(0.454902, 0.454902, 0.454902, 1.0),
            };
        }
    }

    public static DiffuseMaterial LightGray
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "LightGray",
                DiffuseColor = ToColor(0.682353, 0.682353, 0.682353, 1.0),
            };
        }
    }

    // Materials from: http://globe3d.sourceforge.net/g3d_html/gl-materials__ads.htm
    public static DiffuseMaterial Glass
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Glass",
                DiffuseColor = ToColor(0.588235, 0.670588, 0.729412, 1.0),
            };
        }
    }

    public static DiffuseMaterial Brass
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Brass",
                DiffuseColor = ToColor(0.780392, 0.568627, 0.113725, 1.0),
            };
        }
    }

    public static DiffuseMaterial Bronze
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Bronze",
                DiffuseColor = ToColor(0.714, 0.4284, 0.18144, 1.0),
            };
        }
    }

    public static DiffuseMaterial PolishedBronze
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "PolishedBronze",
                DiffuseColor = ToColor(0.4, 0.2368, 0.1036, 1.0),
            };
        }
    }

    public static DiffuseMaterial Chrome
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Chrome",
                DiffuseColor = ToColor(0.4f, 0.4f, 0.4f, 1.0f),
            };
        }
    }

    public static DiffuseMaterial Copper
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Copper",
                DiffuseColor = ToColor(0.7038, 0.27048, 0.0828, 1.0),
            };
        }
    }

    public static DiffuseMaterial PolishedCopper
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "PolishedCopper",
                DiffuseColor = ToColor(0.5508, 0.2118, 0.066, 1.0),
            };
        }
    }

    public static DiffuseMaterial Gold
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Gold",
                DiffuseColor = ToColor(0.75164, 0.60648, 0.22648, 1.0),
            };
        }
    }

    public static DiffuseMaterial PolishedGold
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "PolishedGold",
                DiffuseColor = ToColor(0.34615, 0.3143, 0.0903, 1.0),
            };
        }
    }


    public static DiffuseMaterial Pewter
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Pewter",
                DiffuseColor = ToColor(0.427451, 0.470588, 0.541176, 1.0),
            };
        }
    }

    public static DiffuseMaterial Silver
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Silver",
                DiffuseColor = ToColor(0.50754, 0.50754, 0.50754, 1.0),
            };
        }
    }

    public static DiffuseMaterial PolishedSilver
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "PolishedSilver",
                DiffuseColor = ToColor(0.2775, 0.2775, 0.2775, 1.0),
            };
        }
    }

    public static DiffuseMaterial Emerald
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Emerald",
                DiffuseColor = ToColor(0.07568, 0.61424, 0.07568, 0.55),
            };
        }
    }

    public static DiffuseMaterial Jade
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Jade",
                DiffuseColor = ToColor(0.54, 0.89, 0.63, 0.95),
            };
        }
    }

    public static DiffuseMaterial Obsidian
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Obsidian",
                DiffuseColor = ToColor(0.18275, 0.17, 0.22525, 0.82),
            };
        }
    }

    public static DiffuseMaterial Pearl
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Pearl",
                DiffuseColor = ToColor(1.0, 0.829, 0.829, 0.922),
            };
        }
    }

    public static DiffuseMaterial Ruby
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Ruby",
                DiffuseColor = ToColor(0.61424, 0.04136, 0.04136, 0.55),
            };
        }
    }

    public static DiffuseMaterial Turquoise
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "Turquoise",
                DiffuseColor = ToColor(0.396, 0.74151, 0.69102, 0.8),
            };
        }
    }

    public static DiffuseMaterial BlackPlastic
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "BlackPlastic",
                DiffuseColor = ToColor(0.01, 0.01, 0.01, 1.0),
            };
        }
    }

    public static DiffuseMaterial BlackRubber
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "BlackRubber",
                DiffuseColor = ToColor(0.01, 0.01, 0.01, 1.0),
            };
        }
    }

    public static DiffuseMaterial DefaultVRML
    {
        get
        {
            return new DiffuseMaterial
            {
                Name = "DefaultVRML",
                DiffuseColor = ToColor(0.8, 0.8, 0.8, 1.0),
            };
        }
    }
}

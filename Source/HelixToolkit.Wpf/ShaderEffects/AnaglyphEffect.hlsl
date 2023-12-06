// remember to save .fx file using Codepage 1252

// http://www.3dtv.at/knowhow/AnaglyphComparison_en.aspx
// http://stereo.jpn.org/eng/stphmkr/help/stereo_13.htm
// http://www.swell3d.com/2008/07/wimmers-optimized-anaglyph-on.html

float method : register(C0);
float offset : register(C1);

// todo: custom matrices
// float3x3 M1 : register(C1);
// float3x3 M2 : register(C2);

sampler2D input1 : register(S0); // left image input
sampler2D input2 : register(S1); // right image input
 
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c1, c2;

    float2 uv2;
    uv2[0] = uv[0] - offset;
    uv2[1] = uv[1];

    c1 = tex2D(input1, uv2.xy);
    c2 = tex2D(input2, uv.xy);

    float r, g, b;

    if (method == 0)
    { // True
        r = 0.299 * c1.r + 0.587 * c1.g + 0.114 * c1.b;
        g = 0;
        b = 0.299 * c2.r + 0.587 * c2.g + 0.114 * c2.b;
    }
    else if (method == 1)
    { // Gray
        r = 0.299 * c1.r + 0.587 * c1.g + 0.114 * c1.b;
        g = 0.299 * c2.r + 0.587 * c2.g + 0.114 * c2.b;
        b = 0.299 * c2.r + 0.587 * c2.g + 0.114 * c2.b;
    }
    else if (method == 2)
    { // Color
        r = c1.r;
        g = c2.g;
        b = c2.b;
    }
    else if (method == 3)
    { // Half color
        r = 0.299 * c1.r + 0.587 * c1.g + 0.114 * c1.b;
        g = c2.g;
        b = c2.b;
    }
    else if (method == 4)
    { // Optimized
        r = 0.7 * c1.g + 0.3 * c1.b;
        g = c2.g;
        b = c2.b;

		// Gamma 1.5 on the red channel
        r = pow(abs(r), 1.0 / 1.5);
    }
    else if (method == 5)
    { // Dubois
        r = 0.456 * c1.r + 0.500 * c1.g + 0.176 * c1.b - 0.043 * c2.r - 0.088 * c2.g - 0.002 * c2.b;
        g = -0.040 * c1.r - 0.038 * c1.g - 0.016 * c1.b + 0.378 * c2.r + 0.734 * c2.g - 0.018 * c2.b;
        b = -0.015 * c1.r - 0.021 * c1.g - 0.005 * c1.b - 0.072 * c2.r - 0.113 * c2.g + 1.226 * c2.b;
    }

    c2.r = r;
    c2.g = g;
    c2.b = b;
    c2.a = max(c2.a, c1.a);
    
    return c2;
}

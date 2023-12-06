// remember to save .fx file using Codepage 1252

float method : register(C0);
float offset : register(C1);

sampler2D input1 : register(S0); // left image input
sampler2D input2 : register(S1); // right image input
 
float4 main(float2 uv : TEXCOORD, float2 vpos : VPOS) : COLOR
{
    float4 c1, c2;

    float2 uv2;
    uv2[0] = uv[0] - offset;
    uv2[1] = uv[1];

    c1 = tex2D(input1, uv2.xy);
    c2 = tex2D(input2, uv.xy);
	
    int y = (int) vpos[1];

    if (y % 2 == method)
    {
		// even line
        c2.r = c1.r;
        c2.g = c1.g;
        c2.b = c1.b;
        c2.a = c1.a;
    }
    
    return c2;
}

#ifndef GSBILLBOARD_HLSL
#define GSBILLBOARD_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------
[maxvertexcount(4)]
void main(point GSInputBT input[1], inout TriangleStream<PSInputBT> SpriteStream)
{
    float4 ndcPosition0 = input[0].p;
    float4 ndcPosition1 = input[0].p;
    float4 ndcPosition2 = input[0].p;
    float4 ndcPosition3 = input[0].p;

	// Transform to clip space
    if (!pbParams.x)// if not fixed size billboard
    {
        ndcPosition0.xy += input[0].offTR;
        ndcPosition1.xy += input[0].offBR;
        ndcPosition2.xy += input[0].offTL;
        ndcPosition3.xy += input[0].offBL;
    }

    ndcPosition0 = mul(ndcPosition0, mProjection);
    ndcPosition1 = mul(ndcPosition1, mProjection);
    ndcPosition2 = mul(ndcPosition2, mProjection);
    ndcPosition3 = mul(ndcPosition3, mProjection);

    float4 ndcTranslated0 = ndcPosition0 / ndcPosition0.w;
    float4 ndcTranslated1 = ndcPosition1 / ndcPosition1.w;
    float4 ndcTranslated2 = ndcPosition2 / ndcPosition2.w;
    float4 ndcTranslated3 = ndcPosition3 / ndcPosition3.w;

    if (pbParams.x)// if fixed sized billboard
    {
		// Translate offset into normalized device coordinates.
        ndcTranslated0.xy += windowToNdc(input[0].offTR);
        ndcTranslated1.xy += windowToNdc(input[0].offBR);
        ndcTranslated2.xy += windowToNdc(input[0].offTL);
        ndcTranslated3.xy += windowToNdc(input[0].offBL);
    }

    float3 vEye = vEyePos - input[0].p.xyz;
    float4 eye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction

    PSInputBT output = (PSInputBT) 0;
    output.p = float4(ndcTranslated0.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = float2(input[0].t3.x, input[0].t0.y);
    output.vEye = eye;
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated1.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t3;
    output.vEye = eye;
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated2.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t0;
    output.vEye = eye;
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated3.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = float2(input[0].t0.x, input[0].t3.y);    
    output.vEye = eye;
    SpriteStream.Append(output);

    SpriteStream.RestartStrip();
}

#endif
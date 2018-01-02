#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------
[maxvertexcount(4)]
void main(point VSInputBT input[1], inout TriangleStream<PSInputBT> SpriteStream)
{
    float4 ndcPosition0 = input[0].p;
    float4 ndcPosition1 = input[0].p;
    float4 ndcPosition2 = input[0].p;
    float4 ndcPosition3 = input[0].p;

	// Transform to clip space
    if (!bParams.x)// if not fixed size billboard
    {
        ndcPosition0.xy += input[0].p0;
        ndcPosition1.xy += input[0].p1;
        ndcPosition2.xy += input[0].p2;
        ndcPosition3.xy += input[0].p3;
    }

    ndcPosition0 = mul(ndcPosition0, mProjection);
    ndcPosition1 = mul(ndcPosition1, mProjection);
    ndcPosition2 = mul(ndcPosition2, mProjection);
    ndcPosition3 = mul(ndcPosition3, mProjection);

    float4 ndcTranslated0 = ndcPosition0 / ndcPosition0.w;
    float4 ndcTranslated1 = ndcPosition1 / ndcPosition1.w;
    float4 ndcTranslated2 = ndcPosition2 / ndcPosition2.w;
    float4 ndcTranslated3 = ndcPosition3 / ndcPosition3.w;

    if (bParams.x)// if fixed sized billboard
    {
		// Translate offset into normalized device coordinates.
        ndcTranslated0.xy += windowToNdc(input[0].p0);
        ndcTranslated1.xy += windowToNdc(input[0].p1);
        ndcTranslated2.xy += windowToNdc(input[0].p2);
        ndcTranslated3.xy += windowToNdc(input[0].p3);
    }

    PSInputBT output = (PSInputBT) 0;
    output.p = float4(ndcTranslated0.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t0;	
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated1.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t1;
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated2.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t2;
    SpriteStream.Append(output);

    output.p = float4(ndcTranslated3.xyz, 1.0);
    output.background = input[0].background;
    output.foreground = input[0].foreground;
    output.t = input[0].t3;
    SpriteStream.Append(output);

    SpriteStream.RestartStrip();
}
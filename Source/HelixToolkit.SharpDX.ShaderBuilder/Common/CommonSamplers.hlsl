#ifndef COMMONSAMPLERS_HLSL
#define COMMONSAMPLERS_HLSL

//--------------------------------------------------------------------------------------
//  STATES DEFININITIONS 
//--------------------------------------------------------------------------------------
SamplerState LinearSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
    MaxAnisotropy = 16;
};
SamplerState NormalSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerState PointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerComparisonState CmpSampler
{
   // sampler state
    Filter = COMPARISON_MIN_MAG_MIP_LINEAR;
    AddressU = MIRROR;
    AddressV = MIRROR;
   // sampler comparison state
    ComparisonFunc = LESS_EQUAL;
};
#endif
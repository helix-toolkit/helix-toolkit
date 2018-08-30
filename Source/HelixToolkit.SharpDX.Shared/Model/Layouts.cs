/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Linq;
using System.Runtime.InteropServices;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#pragma warning disable 1591
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DefaultVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;
        public const int SizeInBytes = 4 * (4 + 3 + 3 + 3);
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BatchedMeshVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;
        public Vector2 TexCoord;
        public Vector4 Color;//Diffuse, Emissive, Specular, Reflect
        public Vector4 Color2;//Ambient, sMaterialShininess, diffuseAlpha
        public const int SizeInBytes = 4 * (4 + 3 + 3 + 3 + 2 + 4 + 4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinesVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public const int SizeInBytes = 4 * (4 + 4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PointsVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public const int SizeInBytes = 4 * (4 + 4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeVertex
    {
        public Vector4 Position;
        public const int SizeInBytes = 4 * 4;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BillboardVertex
    {
        public Vector4 Position;
        public Color4 Foreground;
        public Color4 Background;
        public Vector2 TexTL;
        public Vector2 TexBR;
        public Vector2 OffTL;
        public Vector2 OffBR;
        public const int SizeInBytes = 4 * (4  * 3 + 2 * 4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BillboardInstanceParameter
    {
        public Color4 DiffuseColor;
        public Vector2 TexCoordScale;
        public Vector2 TexCoordOffset;
        public const int SizeInBytes = 4 * (4 + 2 + 2);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct InstanceParameter
    {
        public Color4 DiffuseColor;
        public Color4 AmbientColor;
        public Color4 EmissiveColor;
        public Vector2 TexCoordOffset;
        public const int SizeInBytes = 4 * (4 * 3 + 2);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoneIds
    {
        public int Bone1;
        public int Bone2;
        public int Bone3;
        public int Bone4;
        public Vector4 Weights;

        public const int SizeInBytes = 4 * (4 + 4);
    }
    /// <summary>
    /// 
    /// </summary>
    //[StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal static class BoneMatricesStruct
    {
        //public const int NumberOfBones = 128;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = NumberOfBones)]
        //public Matrix[] Bones;
        //public const int SizeInBytes = 4 * (4 * 4 * NumberOfBones);
        public static readonly Matrix[] DefaultBones = Enumerable.Repeat(Matrix.Identity, 1).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Particle
    {
        Vector3 position;
        float initEnergy;
        Vector3 velocity;
        float energy;
        Color4 color;
        Vector3 initAcceleration;
        float dissipRate;
        uint texRow;
        uint texColumn;
        public const int SizeInBytes = 4 * (4 * 4 + 2);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticlePerFrame
    {
        public uint NumParticles;
        public Vector3 ExtraAcceleration;

        public float TimeFactors;
        public Vector3 DomainBoundsMax;

        public Vector3 DomainBoundsMin;
        public uint CumulateAtBound;

        public Vector3 ConsumerLocation;
        public float ConsumerGravity;

        public float ConsumerRadius;
        public Vector3 RandomVector;

        public uint RandomSeed;
        public uint NumTexCol;
        public uint NumTexRow;
        public int AnimateByEnergyLevel;

        public Vector2 ParticleSize;
        public float Turbulance;
        float padding;

        public const int SizeInBytes = 4 * (4 * 7);
        public const int NumParticlesOffset = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticleInsertParameters
    {
        public Vector3 EmitterLocation;
        public float InitialEnergy;

        public float EmitterRadius;
        private Vector2 Pad;
        public float InitialVelocity;

        public Color4 ParticleBlendColor;

        public float EnergyDissipationRate; //Energy dissipation rate per second
        public Vector3 InitialAcceleration;

        public const int SizeInBytes = 4 * (4 * 4);
        public const int NumParticlesOffset = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticleCountIndirectArgs
    {
        public uint VertexCount;
        public uint InstanceCount;
        public uint StartVertexLocation;
        public uint StartInstanceLocation;
        public const int SizeInBytes = 4 * 4;
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ShadowMapParamStruct
    {
        public Vector2 ShadowMapSize;
        public int HasShadowMap;
        float paddingShadow0;
        public Vector4 ShadowMapInfo;
        public Matrix LightViewProjection;
        public const int SizeInBytes = 4 * (4 * 2 + 4*4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GlobalTransformStruct
    {
        /// <summary>
        /// The view matrix
        /// </summary>
        public Matrix View;
        /// <summary>
        /// The projection matrix
        /// </summary>
        public Matrix Projection;
        /// <summary>
        /// The view projection matrix
        /// </summary>
        public Matrix ViewProjection;   
        /// <summary>
        /// The frustum [fov,asepct-ratio,near,far]  
        /// </summary>
        public Vector4 Frustum;  
        /// <summary>
        /// The viewport [w,h,1/w,1/h]      
        /// </summary>
        public Vector4 Viewport;
        /// <summary>
        /// The eye position
        /// </summary>
        public Vector3 EyePos;
        private float padding0;
        public float OITWeightPower;
        public float OITWeightDepthSlope;
        public int OITWeightMode;
        private int padding1;
        public const int SizeInBytes = 4 * (4 * 4 * 3 + 4 * 4);
    }

    /// <summary>
    /// Used combine with <see cref="PhongMaterialStruct"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ModelStruct
    {
        public Matrix World;
        public int InvertNormal;
        public int HasInstances;
        public int HasInstanceParams;
        public int HasBones;
        public Vector4 Params;
        public Vector4 Color;
        public Int3 BoolParams;
        public int Batched;
        public int RenderOIT;
        Vector3 padding;
        public Color4 WireframeColor;
        public const int SizeInBytes = 4 * (4 * 4 + 4 + 4 * 2 + 4 + 4 * 2);

        public const string WorldStr = "mWorld";
        public const string InvertNormalStr = "bInvertNormal";
        public const string HasInstancesStr = "bHasInstances";
        public const string HasInstanceParamsStr = "bHasInstanceParams";
        public const string HasBonesStr = "bHasBones";
        public const string ParamsStr = "vParams";
        public const string ColorStr = "vColor";
        public const string BoolParamsStr = "bParams";
        public const string BatchedStr = "bBatched";
        public const string RenderOITStr = "bRenderOIT";
        public const string WireframeColorStr = "wireframeColor";
    }

    /// <summary>
    /// Used combine with <see cref="ModelStruct"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PhongMaterialStruct
    {
        //public float MinTessDistance; // Minimum distance to do tessellation
        //public float MaxTessDistance; // Maximum distance to do tessellation
        //public float MinDistTessFactor; // Tessellation factor when at minimum distance, usually MinTessFactor > MaxTessFactor
        //public float MaxDistTessFactor; // Tessellation factor when at maximum distance
        ///// <summary>
        ///// Material variables
        ///// </summary>
        //public Color4 Ambient;
        //public Color4 Diffuse;
        //public Color4 Emissive;
        //public Color4 Specular;
        //public Color4 Reflect;
        //public float Shininess;
        //public int HasDiffuseMap;
        //public int HasDiffuseAlphaMap;
        //public int HasNormalMap;
        //public int HasDisplacementMap;
        //public int HasCubeMap;
        //public int RenderShadowMap;
        //float padding;
        //public Vector4 DisplacementMapScaleMask; // Use to select which channel will be used after displacement map sampling, also scaling the value
        //public Vector4 UVTransformR1; //Make sure to convert column majo into Row major. Pass into shader
        //public Vector4 UVTransformR2; //Make sure to Convert column majo into Row major. Pass into shader
        public const int SizeInBytes = 4 * ( 4 + 4 * 5 + 4 * 2 + 4 + 4 * 2);

        public const string MinTessDistanceStr = "minTessDistance";
        public const string MaxTessDistanceStr = "maxTessDistance";
        public const string MinDistTessFactorStr = "minTessFactor";
        public const string MaxDistTessFactorStr = "maxTessFactor";
        public const string AmbientStr = "vMaterialAmbient";
        public const string DiffuseStr = "vMaterialDiffuse";
        public const string EmissiveStr = "vMaterialEmissive";
        public const string SpecularStr = "vMaterialSpecular";
        public const string ReflectStr = "vMaterialReflect";
        public const string ShininessStr = "sMaterialShininess";
        public const string HasDiffuseMapStr = "bHasDiffuseMap";
        public const string HasDiffuseAlphaMapStr = "bHasAlphaMap";
        public const string HasNormalMapStr = "bHasNormalMap";
        public const string HasDisplacementMapStr = "bHasDisplacementMap";
        public const string HasCubeMapStr = "bHasCubeMap";
        public const string RenderShadowMapStr = "bRenderShadowMap";
        public const string DisplacementMapScaleMaskStr = "displacementMapScaleMask";
        public const string UVTransformR1Str = "uvTransformR1";
        public const string UVTransformR2Str = "uvTransformR2";
    }

    /// <summary>
    /// Used combine with <see cref="PointLineMaterialStruct"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PointLineModelStruct
    {
        public Matrix World;
        public int HasInstances;
        public int HasInstanceParams;
        Vector2 padding;
        public const int SizeInBytes = 4 * (4 * 4 + 4);
    }

    /// <summary>
    /// Used combine with <see cref="PointLineModelStruct"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PointLineMaterialStruct
    {
        //public Vector4 Params;
        //public Vector4 Color;
        //public Bool4 BoolParams;

        public const int SizeInBytes = 4 * (3 * 4);

        public const string ParamsStr = "pfParams";
        public const string ColorStr = "pColor";
        public const string BoolParamsStr = "pbParams";
    }

    /// <summary>
    /// Used combine with <see cref="PointLineMaterialStruct"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticleModelStruct
    {
        public Matrix World;
        public int HasInstances;
        public int HasInstanceParams;
        public int HasTexture;
        int padding;
        public const int SizeInBytes = 4 * (4 * 4 + 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PlaneGridModelStruct
    {
        public Matrix World;
        public float GridSpacing;
        public float GridThickenss;
        public float FadingFactor;
        public float PlaneD;
        public Vector4 PlaneColor;
        public Vector4 GridColor;
        public bool HasShadowMap;
        public int Axis;
        public int Type;
        float pad;
        public const int SizeInBytes = 4 * (4 * 4 + 4 * 4);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LightStruct
    {
        public int LightType;
        Vector3 padding;
        public Vector4 LightDir;
        public Vector4 LightPos;
        public Vector4 LightAtt;
        public Vector4 LightSpot; //(outer angle , inner angle, falloff, free)
        public Color4 LightColor;
        public Matrix LightView;
        public Matrix LightProj;
        public const int SizeInBytes = 4 * (4 * 6 + 4 * 4 * 2);
    }
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ClipPlaneStruct
    {
        //public Bool4 EnableCrossPlane;
        //public Vector4 CrossSectionColors;
        //public int CuttingOperation;
        //Vector3 padding;
        // Format:
        // M00M01M02 PlaneNormal1 M03 Plane1 Distance to origin
        // M10M11M12 PlaneNormal2 M13 Plane2 Distance to origin
        // M20M21M22 PlaneNormal3 M23 Plane3 Distance to origin
        // M30M31M32 PlaneNormal4 M33 Plane4 Distance to origin        
        /// <summary>
        /// The cross plane parameters
        /// Format:
        /// <para>M00M01M02 PlaneNormal1 M03 Plane1 Distance to origin</para>
        /// <para>M10M11M12 PlaneNormal2 M13 Plane2 Distance to origin</para>
        /// <para>M20M21M22 PlaneNormal3 M23 Plane3 Distance to origin</para>
        /// <para>M30M31M32 PlaneNormal4 M33 Plane4 Distance to origin</para>
        /// </summary>
        //public Matrix CrossPlaneParams;
        public const int SizeInBytes = 4 * (4 * 3 + 4 * 4);

        public const string EnableCrossPlaneStr = "EnableCrossPlane";
        public const string CrossSectionColorStr = "CrossSectionColors";
        public const string CuttingOperationStr = "CuttingOperation";
        public const string CrossPlane1ParamsStr = "CrossPlane1Params";
        public const string CrossPlane2ParamsStr = "CrossPlane2Params";
        public const string CrossPlane3ParamsStr = "CrossPlane3Params";
        public const string CrossPlane4ParamsStr = "CrossPlane4Params";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BorderEffectStruct
    {
        public Color4 Color;
        public Matrix Param;

        public const int SizeInBytes = 4 * (4 + 4 * 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeFaceCamera
    {
        public Matrix View;
        public Matrix Projection;
        public const int SizeInBytes = 4 * 4 * 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeFaceCamerasStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public CubeFaceCamera[] Cameras;
        public const int SizeInBytes = CubeFaceCamera.SizeInBytes * 6;
    }
#if !NETFX_CORE
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ScreenDuplicationModelStruct
    {        
        public Vector4 TopRight;
        public Vector4 TopLeft;
        public Vector4 BottomRight;
        public Vector4 BottomLeft;

        public Vector2 TexTopRight;
        Vector2 pad0;
        public Vector2 TexTopLeft;
        Vector2 pad1;
        public Vector2 TexBottomRight;
        Vector2 pad2;
        public Vector2 TexBottomLeft;
        Vector2 pad3;

        public Vector4 CursorTopRight;
        public Vector4 CursorTopLeft;
        public Vector4 CursorBottomRight;
        public Vector4 CursorBottomLeft;

        public const int SizeInBytes = 4 * 4 * 12;
    }
#endif

#pragma warning restore 1591
}

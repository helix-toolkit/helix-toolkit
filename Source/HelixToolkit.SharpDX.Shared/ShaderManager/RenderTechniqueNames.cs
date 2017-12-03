using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public struct DefaultRenderTechniqueNames
    {
        public const string Blinn = "RenderBlinn";
        public const string Phong = "RenderPhong";
        public const string Diffuse = "RenderDiffuse";
        public const string Colors = "RenderColors";
        public const string Positions = "RenderPositions";
        public const string Normals = "RenderNormals";
        public const string PerturbedNormals = "RenderPerturbedNormals";
        public const string Tangents = "RenderTangents";
        public const string TexCoords = "RenderTexCoords";
        public const string Wires = "RenderWires";
        public const string Lines = "RenderLines";
        public const string Points = "RenderPoints";
        public const string CubeMap = "RenderCubeMap";
        public const string BillboardText = "RenderBillboard";
        public const string BillboardInstancing = "RenderBillboardInstancing";
        public const string InstancingBlinn = "RenderInstancingBlinn";
        public const string BoneSkinBlinn = "RenderBoneSkinBlinn";
        public const string ParticleStorm = "ParticleStorm";
        public const string CrossSection = "RenderCrossSectionBlinn";
    }

    public struct TessellationRenderTechniqueNames
    {
        public const string PNTriangles = "RenderPNTriangs";
        public const string PNQuads = "RenderPNQuads";
    }

    public struct DeferredRenderTechniqueNames
    {
        public const string Deferred = "RenderDeferred";
        public const string GBuffer = "RenderGBuffer";
        public const string DeferredLighting = "RenderDeferredLighting";
        public const string ScreenSpace = "RenderScreenSpace";
    }

    public static class TessellationTechniques
    {
        public enum Shading
        {
            Solid,
            Positions,
            Normals,
            TexCoords,
            Tangents,
            Colors
        };
        /// <summary>
        /// Passes available for this Model3D
        /// </summary>
        public static IEnumerable<string> Shadings { get { return new string[] { Shading.Solid.ToString(), Shading.Positions.ToString(), Shading.Normals.ToString(), Shading.TexCoords.ToString(), Shading.Tangents.ToString(), Shading.Colors.ToString() }; } }
        public enum MeshTopology
        {
            Triangle, Quads
        }

        public static IEnumerable<string> MeshTopologies { get { return new string[] { MeshTopology.Triangle.ToString(), MeshTopology.Quads.ToString() }; } }
    }
}

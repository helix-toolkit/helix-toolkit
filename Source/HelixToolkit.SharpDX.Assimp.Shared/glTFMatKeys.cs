/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Assimp
    {
        public static class GLTFMatKeys
        {
            /// <summary>
            ///     The ai matkey GLTF basecolor factor for PBR material
            /// </summary>
            public const string AI_MATKEY_GLTF_BASECOLOR_FACTOR = @"$mat.gltf.pbrMetallicRoughness.baseColorFactor";

            /// <summary>
            ///     The ai matkey GLTF metallic factor for PBR material
            /// </summary>
            public const string AI_MATKEY_GLTF_METALLIC_FACTOR = @"$mat.gltf.pbrMetallicRoughness.metallicFactor";

            /// <summary>
            ///     The ai matkey GLTF metallic, roughness, ambient occlusion texture
            /// </summary>
            public const string AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE = @"$tex.file";

            /// <summary>
            ///     The ai matkey GLTF roughness factor for PBR material
            /// </summary>
            public const string AI_MATKEY_GLTF_ROUGHNESS_FACTOR = @"$mat.gltf.pbrMetallicRoughness.roughnessFactor";
            /// <summary>
            /// The ai matkey GLTF pbrspecularglossiness
            /// </summary>
            public const string AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS = @"$mat.gltf.pbrSpecularGlossiness";
            /// <summary>
            /// The ai matkey GLTF pbrspecularglossiness glossiness factor
            /// </summary>
            public const string AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS_GLOSSINESS_FACTOR = @"$mat.gltf.pbrMetallicRoughness.glossinessFactor";
        }
    }
}

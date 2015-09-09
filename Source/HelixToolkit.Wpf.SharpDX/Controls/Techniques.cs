using System.Collections.Generic;

namespace HelixToolkit.Wpf.SharpDX
{
    public sealed class Techniques
    {
        static Techniques()
        {
            /// <summary>
            /// Names of techniques which are implemented by default
            /// </summary>
            RenderBlinn = new RenderTechnique("RenderBlinn");
            RenderPhong = new RenderTechnique("RenderPhong");

            RenderDiffuse = new RenderTechnique("RenderDiffuse");
            RenderColors = new RenderTechnique("RenderColors");
            RenderPositions = new RenderTechnique("RenderPositions");
            RenderNormals = new RenderTechnique("RenderNormals");
            RenderPerturbedNormals = new RenderTechnique("RenderPerturbedNormals");
            RenderTangents = new RenderTechnique("RenderTangents");
            RenderTexCoords = new RenderTechnique("RenderTexCoords");
            RenderWires = new RenderTechnique("RenderWires");

#if DEFERRED 
            RenderDeferred = new RenderTechnique("RenderDeferred");
            RenderGBuffer = new RenderTechnique("RenderGBuffer");
            RenderDeferredLighting = new RenderTechnique("RenderDeferredLighting");
            RenderScreenSpace = new RenderTechnique("RenderScreenSpace");
#endif

#if TESSELLATION 
            RenderPNTriangs = new RenderTechnique("RenderPNTriangs");
            RenderPNQuads = new RenderTechnique("RenderPNQuads");
#endif
            RenderCubeMap = new RenderTechnique("RenderCubeMap");
            RenderLines = new RenderTechnique("RenderLines");
            RenderPoints = new RenderTechnique("RenderPoints");
            RenderBillboard = new RenderTechnique("RenderBillboard");

            RenderTechniques = new List<RenderTechnique>
            {
                RenderBlinn,
                RenderPhong,

                RenderColors,
                RenderDiffuse,
                RenderPositions,
                RenderNormals,
                RenderPerturbedNormals,
                RenderTangents,
                RenderTexCoords,
                RenderWires,
#if DEFERRED
                RenderDeferred,
                RenderGBuffer,  
#endif
                
#if TESSELLATION 
                RenderPNTriangs,
                RenderPNQuads,
#endif
            };

            TechniquesSourceDict = new Dictionary<RenderTechnique, byte[]>()
            {
                {     Techniques.RenderPhong,      Properties.Resources._default},
                {     Techniques.RenderBlinn,      Properties.Resources._default},
                {     Techniques.RenderCubeMap,    Properties.Resources._default},
                {     Techniques.RenderColors,     Properties.Resources._default},
                {     Techniques.RenderDiffuse,    Properties.Resources._default},
                {     Techniques.RenderPositions,  Properties.Resources._default},
                {     Techniques.RenderNormals,    Properties.Resources._default},
                {     Techniques.RenderPerturbedNormals,    Properties.Resources._default},
                {     Techniques.RenderTangents,   Properties.Resources._default},
                {     Techniques.RenderTexCoords,  Properties.Resources._default},
                {     Techniques.RenderWires,      Properties.Resources._default},
                {     Techniques.RenderLines,      Properties.Resources._default},
                {     Techniques.RenderPoints,     Properties.Resources._default},
                {     Techniques.RenderBillboard,  Properties.Resources._default},
    #if TESSELLATION                                        
                {     Techniques.RenderPNTriangs,  Properties.Resources._default},
                {     Techniques.RenderPNQuads,    Properties.Resources._default}, 
    #endif 
    #if DEFERRED            
                {     Techniques.RenderDeferred,   Properties.Resources._deferred},
                {     Techniques.RenderGBuffer,    Properties.Resources._deferred},
                {     Techniques.RenderDeferredLighting , Properties.Resources._deferred},
    #endif
            };
        }

        internal static readonly Techniques Instance = new Techniques();

        internal static readonly Dictionary<RenderTechnique, byte[]> TechniquesSourceDict;

        private Techniques()
        {

        }

        /// <summary>
        /// Names of techniques which are implemented by default
        /// </summary>
        public static RenderTechnique RenderBlinn { get; private set; }// = new RenderTechnique("RenderBlinn");
        public static RenderTechnique RenderPhong { get; private set; }

        public static RenderTechnique RenderDiffuse { get; private set; }
        public static RenderTechnique RenderColors { get; private set; }
        public static RenderTechnique RenderPositions { get; private set; }
        public static RenderTechnique RenderNormals { get; private set; }
        public static RenderTechnique RenderPerturbedNormals { get; private set; }
        public static RenderTechnique RenderTangents { get; private set; }
        public static RenderTechnique RenderTexCoords { get; private set; }
        public static RenderTechnique RenderWires { get; private set; }
        public static RenderTechnique RenderCubeMap { get; private set; }
        public static RenderTechnique RenderLines { get; private set; }
        public static RenderTechnique RenderPoints { get; private set; }
        public static RenderTechnique RenderBillboard { get; private set; }

#if TESSELLATION
        public static RenderTechnique RenderPNTriangs { get; private set; }
        public static RenderTechnique RenderPNQuads { get; private set; }
#endif

#if DEFERRED                 
        public static RenderTechnique RenderDeferred { get; private set; }
        public static RenderTechnique RenderGBuffer { get; private set; }
        public static RenderTechnique RenderDeferredLighting { get; private set; }
        public static RenderTechnique RenderScreenSpace { get; private set; }
#endif
        public static IEnumerable<RenderTechnique> RenderTechniques { get; private set; }
    }
}

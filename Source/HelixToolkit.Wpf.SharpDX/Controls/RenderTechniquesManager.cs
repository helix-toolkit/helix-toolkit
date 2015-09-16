using System.Collections.Generic;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX
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

    public interface IRenderTechniquesManager
    {
        void AddRenderTechnique(string techniqueName, byte[] techniqueSource);
        IDictionary<string, RenderTechnique> RenderTechniques { get; }
    }

    public class DefaultRenderTechniquesManager: IRenderTechniquesManager
    {
        internal readonly Dictionary<RenderTechnique, byte[]> TechniquesSourceDict = new Dictionary<RenderTechnique, byte[]>();
        private Dictionary<string, RenderTechnique> renderTechniques = new Dictionary<string, RenderTechnique>();

        public IDictionary<string,RenderTechnique> RenderTechniques
        {
            get { return renderTechniques; }
        }

        public DefaultRenderTechniquesManager()
        {
            InitTechniques();
        }

        /// <summary>
        /// Override in a derived class to control technique registration.
        /// </summary>
        protected virtual void InitTechniques()
        {
            InitializeDefaultTechniques();
        }

        protected void InitializeDefaultTechniques()
        {
            AddDefaultTechnique(DefaultRenderTechniqueNames.Phong);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Blinn);
            AddDefaultTechnique(DefaultRenderTechniqueNames.CubeMap);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Colors);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Diffuse);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Positions);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Normals);
            AddDefaultTechnique(DefaultRenderTechniqueNames.PerturbedNormals);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Tangents);
            AddDefaultTechnique(DefaultRenderTechniqueNames.TexCoords);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Wires);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Lines);
            AddDefaultTechnique(DefaultRenderTechniqueNames.Points);
            AddDefaultTechnique(DefaultRenderTechniqueNames.BillboardText);
        }

        protected void AddDefaultTechnique(string techniqueName)
        {
            AddRenderTechnique(techniqueName, Properties.Resources._default);
        }

        #region IRenderTechniqueManager interface

        public void AddRenderTechnique(string techniqueName, byte[] techniqueSource)
        {
            var technique = new RenderTechnique(techniqueName);
            if (!TechniquesSourceDict.ContainsKey(technique))
            {
                TechniquesSourceDict.Add(technique, techniqueSource);
            }

            renderTechniques.Add(techniqueName, technique);
        }

        #endregion
    }

    public class DeferredTechniquesManager : DefaultRenderTechniquesManager
    {
        protected override void InitTechniques()
        {
            InitializeDefaultTechniques();
            InitializeDefferredTechniques();
        }

        private void InitializeDefferredTechniques()
        {
            AddDeferredTechnique(DeferredRenderTechniqueNames.Deferred);
            AddDeferredTechnique(DeferredRenderTechniqueNames.GBuffer);
            AddDeferredTechnique(DeferredRenderTechniqueNames.DeferredLighting);
        }

        private void AddDeferredTechnique(string techniqueName)
        {
            AddRenderTechnique(techniqueName, Properties.Resources._deferred);
        }
    }

    public class TessellationTechniquesManager : DefaultRenderTechniquesManager
    {
        protected override void InitTechniques()
        {
            InitializeDefaultTechniques();
            InitializeTessellationTechniques();
        }

        protected void InitializeTessellationTechniques()
        {
            AddDefaultTechnique(TessellationRenderTechniqueNames.PNTriangles);
            AddDefaultTechnique(TessellationRenderTechniqueNames.PNQuads);
        }
    }
}

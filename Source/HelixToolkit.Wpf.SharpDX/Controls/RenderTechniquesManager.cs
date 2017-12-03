using System.Collections.Generic;

namespace HelixToolkit.Wpf.SharpDX
{


    public static class ShaderResources
    {
        public static string DefaultName { get; } = "Default";
        public static byte[] Default { get; } = Properties.Resources._default;
        public static string DeferredName { get; } = "Deferred";
        public static byte[] Deferred { get; } = Properties.Resources._deferred;
        public static string TessellationName { get; } = "Tessellation";
        public static byte[] Tessellation { get; } = Properties.Resources._Tessellation;
    }

    public class DefaultRenderTechniquesManager: IRenderTechniquesManager
    {
        internal readonly Dictionary<IRenderTechnique, byte[]> TechniquesSourceDict = new Dictionary<IRenderTechnique, byte[]>();
        private Dictionary<string, IRenderTechnique> renderTechniques = new Dictionary<string, IRenderTechnique>();

        public IDictionary<string, IRenderTechnique> RenderTechniques
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
            AddDefaultTechnique(DefaultRenderTechniqueNames.BillboardInstancing);
            AddDefaultTechnique(DefaultRenderTechniqueNames.InstancingBlinn);
            AddDefaultTechnique(DefaultRenderTechniqueNames.BoneSkinBlinn);
            AddDefaultTechnique(DefaultRenderTechniqueNames.ParticleStorm);
            AddDefaultTechnique(DefaultRenderTechniqueNames.CrossSection);
        }

        protected void AddDefaultTechnique(string techniqueName)
        {
            AddRenderTechnique(techniqueName, GetShaderByteCode());
        }

        protected virtual byte[] GetShaderByteCode()
        {
            return ShaderResources.Default;
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
            AddRenderTechnique(techniqueName, ShaderResources.Deferred);
        }
    }
    public class DefaultTechniquesManager : DefaultRenderTechniquesManager { }

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

        protected override byte[] GetShaderByteCode()
        {
            return ShaderResources.Tessellation;
        }
    }
}

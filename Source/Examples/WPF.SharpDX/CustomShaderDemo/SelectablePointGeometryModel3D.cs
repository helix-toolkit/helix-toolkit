using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;

using System.Linq;
using System.Runtime.InteropServices;

namespace CustomShaderDemo
{
    using HelixToolkit.Wpf.SharpDX.Extensions;
    using HelixToolkit.Wpf.SharpDX.Utilities;

    /// <summary>
    /// Our CustomPointsVertex class has an additional Vector4
    /// to store data to be used by the shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CustomPointsVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector4 Parameters;
        public const int SizeInBytes = 4 * (4 + 4 + 4);
    }

    public class SelectablePointGeometryModel3D: PointGeometryModel3D
    {
        private readonly ImmutableBufferProxy<CustomPointsVertex> vertexBuffer = new ImmutableBufferProxy<CustomPointsVertex>(CustomPointsVertex.SizeInBytes, BindFlags.VertexBuffer);
        public override IBufferProxy VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
        }
        /// <summary>
        /// By overriding Attach, we can provide our own vertex array.
        /// </summary>
        /// <param name="host">An object whose type implements IRenderHost.</param>
        protected override bool OnAttach(IRenderHost host)
        {
            if (!base.OnAttach(host))
            {
                return false;
            }

            if (renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
                return false;

            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            effectTransforms = new EffectTransformVariables(effect);

            var geometry = Geometry as PointGeometry3D;

            if (geometry != null)
            {
                vertexBuffer.CreateBufferFromDataArray(Device, CreateVertexArray());
            }

            /// --- set up const variables
           // vViewport = effect.GetVariableByName("vViewport").AsVector();
            //this.vFrustum = effect.GetVariableByName("vFrustum").AsVector();
            vPointParams = effect.GetVariableByName("vPointParams").AsVector();

            /// --- set effect per object const vars
            var pointParams = new Vector4((float)Size.Width, (float)Size.Height, (float)Figure, (float)FigureRatio);
            vPointParams.Set(pointParams);

            /// --- create raster state
            OnRasterStateChanged();

            /// --- flush
            //  Device.ImmediateContext.Flush();
            return true;
        }

        protected override void OnDetach()
        {
            vertexBuffer.Dispose();
            base.OnDetach();
        }

        private CustomPointsVertex[] CreateVertexArray()
        {
            var positions = Geometry.Positions.ToArray();
            var vertexCount = Geometry.Positions.Count;
            var color = Color;
            var result = new CustomPointsVertex[vertexCount];
            var colors = Geometry.Colors;

            for (var i = 0; i < vertexCount; i++)
            {
                Color4 finalColor;
                if (colors != null && colors.Any())
                {

                    finalColor = color * colors[i];
                }
                else
                {
                    finalColor = color;
                }

                result[i] = new CustomPointsVertex
                {
                    Position = new Vector4(positions[i], 1f),
                    Color = finalColor,
                    Parameters = new Vector4((bool)GetValue(AttachedProperties.ShowSelectedProperty) ? 1 : 0, 0, 0, 0)
                };
            }

            return result;
        }
    }
}

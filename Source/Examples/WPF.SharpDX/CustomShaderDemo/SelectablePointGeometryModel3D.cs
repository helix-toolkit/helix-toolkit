using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D11;

using System.Linq;
using System.Runtime.InteropServices;

namespace CustomShaderDemo
{
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
        /// <summary>
        /// Because we are using a custom vertex, we need
        /// to override this method to provide the 
        /// size of our custom vertex.
        /// </summary>
        public override int VertexSizeInBytes
        {
            get
            {
                return CustomPointsVertex.SizeInBytes;
            }
        }

        /// <summary>
        /// By overriding Attach, we can provide our own vertex array.
        /// </summary>
        /// <param name="host">An object whose type implements IRenderHost.</param>
        public override void Attach(IRenderHost host)
        {
            renderTechnique = host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
            base.Attach(host);

            if (Geometry == null)
                return;

            if (renderHost.RenderTechnique == host.RenderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.Deferred] ||
                renderHost.RenderTechnique == host.RenderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.GBuffer])
                return;

            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            effectTransforms = new EffectTransformVariables(effect);

            var geometry = Geometry as PointGeometry3D;

            if (geometry != null)
            {
                /// --- set up buffers            
                vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, VertexSizeInBytes, CreateVertexArray());
            }

            /// --- set up const variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();
            //this.vFrustum = effect.GetVariableByName("vFrustum").AsVector();
            vPointParams = effect.GetVariableByName("vPointParams").AsVector();

            /// --- set effect per object const vars
            var pointParams = new Vector4((float)Size.Width, (float)Size.Height, (float)Figure, (float)FigureRatio);
            vPointParams.Set(pointParams);

            /// --- create raster state
            OnRasterStateChanged(DepthBias);

            /// --- flush
            Device.ImmediateContext.Flush();
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

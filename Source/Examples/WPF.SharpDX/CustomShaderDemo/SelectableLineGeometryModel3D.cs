using System.Linq;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

using HelixToolkit.Wpf.SharpDX;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace CustomShaderDemo
{
    using HelixToolkit.Wpf.SharpDX.Extensions;
    using HelixToolkit.Wpf.SharpDX.Utilities;

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CustomLinesVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector4 Parameters;
        public const int SizeInBytes = 4 * (4 + 4 + 4);
    }

    public class SelectableLineGeometryModel3D : LineGeometryModel3D
    {
        private readonly ImmutableBufferProxy<CustomLinesVertex> vertexBuffer = new ImmutableBufferProxy<CustomLinesVertex>(CustomLinesVertex.SizeInBytes, BindFlags.VertexBuffer);
        private readonly ImmutableBufferProxy<int> indexBuffer = new ImmutableBufferProxy<int>(sizeof(int), BindFlags.IndexBuffer);
        public override IBufferProxy VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }

        public override IBufferProxy IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
        }
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Lines];
        }
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

            var geometry = Geometry as LineGeometry3D;

            if (geometry != null)
            {        
                vertexBuffer.CreateBufferFromDataArray(Device, CreateVertexArray());
                indexBuffer.CreateBufferFromDataArray(Device, geometry.Indices);
            }
          
            hasInstances = (Instances != null) && (Instances.Any());
            bHasInstances = effect.GetVariableByName("bHasInstances").AsScalar();
            if (hasInstances)
            {
                isInstanceChanged = true;
                InstanceBuffer.CreateBufferFromDataArray(Device, Instances);
            }

            vViewport = effect.GetVariableByName("vViewport").AsVector();
            vLineParams = effect.GetVariableByName("vLineParams").AsVector();

            var lineParams = new Vector4((float)Thickness, (float)Smoothness, 0, 0);
            vLineParams.Set(lineParams);

            OnRasterStateChanged();

            //  Device.ImmediateContext.Flush();
            return true;
        }

        protected override void OnDetach()
        {
            vertexBuffer.Dispose();
            InstanceBuffer.Dispose();
            base.OnDetach();
        }

        private CustomLinesVertex[] CreateVertexArray()
        {
            var positions = Geometry.Positions.ToArray();
            var vertexCount = Geometry.Positions.Count;
            var color = Color;
            var result = new CustomLinesVertex[vertexCount];
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

                result[i] = new CustomLinesVertex
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

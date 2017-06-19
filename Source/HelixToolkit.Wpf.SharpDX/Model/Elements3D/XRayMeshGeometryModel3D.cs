using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Windows;
using Media = System.Windows.Media;
using System;

namespace HelixToolkit.Wpf.SharpDX
{
    public class XRayMeshGeometryModel3D : OutLineMeshGeometryModel3D
    {
        public XRayMeshGeometryModel3D()
        {
            IsDrawBeforeGeometry = true;
            var blendDesc = new BlendStateDescription();
            blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                BlendOperation = BlendOperation.Add,
                AlphaBlendOperation = BlendOperation.Add,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            this.BlendStateDescription = blendDesc;

            var depthStencilDesc = new DepthStencilStateDescription()
            {
                 IsDepthEnabled=true, DepthComparison = Comparison.Greater, DepthWriteMask = DepthWriteMask.Zero
            };
            DepthStencilStateDescription = depthStencilDesc;
        }
    }
}

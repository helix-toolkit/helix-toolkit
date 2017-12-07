using SharpDX;
#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    using global::SharpDX.Direct3D11;
    using SharpDX;
    using System;
    using Utilities;
    /// <summary>
    /// Default Light Model
    /// </summary>
    public class LightsBufferModel : ILightsBufferProxy<LightStruct>
    {
        public const int MaxLights = 8;
        public const int SizeInBytes = LightStruct.SizeInBytes * MaxLights + 4 * 4;
        private readonly LightStruct[] lights = new LightStruct[MaxLights];
        public Color4 AmbientLight { set; get; } = new Color4(0, 0, 0, 1);

        public int BufferSize
        {
            get { return SizeInBytes; }
        }

        public LightStruct[] Lights
        {
            get
            {
                return lights;
            }
        }

        public void UploadToBuffer(IBufferProxy buffer, DeviceContext context)
        {
            if (buffer.Buffer.Description.Usage == ResourceUsage.Dynamic && buffer.StructureSize == SizeInBytes)
            {
                DataStream stream;
                context.MapSubresource(buffer.Buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    stream.WriteRange(Lights, 0, Lights.Length);
                    stream.Write(AmbientLight);
                    context.UnmapSubresource(buffer.Buffer, 0);
                }
            }
            else
            {
                throw new ArgumentException("Buffer type or size do not match the model requirement");
            }
        }
    }
}

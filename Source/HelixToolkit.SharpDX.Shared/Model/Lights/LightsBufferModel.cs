/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.Runtime.CompilerServices;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    using global::SharpDX;
    using Render;
    using Utilities;
    /// <summary>
    /// Default Light Model
    /// </summary>
    public sealed class LightsBufferModel : ILightsBufferProxy<LightStruct>
    {
        public const int SizeInBytes = LightStruct.SizeInBytes * Constants.MaxLights + 4 * (4 * 2);

        private readonly LightStruct[] lights = new LightStruct[Constants.MaxLights];
        public Color4 AmbientLight { set; get; } = new Color4(0, 0, 0, 1);
        public int LightCount { private set; get; } = 0;

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

        public void IncrementLightCount()
        {
            ++LightCount;
        }

        public void ResetLightCount()
        {
            LightCount = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadToBuffer(IBufferProxy buffer, DeviceContextProxy context)
        {
            if (buffer.StructureSize == SizeInBytes)
            {
                context.MapSubresource(buffer.Buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    stream.WriteRange(Lights, 0, Lights.Length);
                    stream.Write(AmbientLight);
                    stream.Write(LightCount);                        
                }
                context.UnmapSubresource(buffer.Buffer, 0);
            }
            else
            {
#if DEBUG
                throw new ArgumentException("Buffer type or size do not match the model requirement");
#endif
            }
        }
    }
}

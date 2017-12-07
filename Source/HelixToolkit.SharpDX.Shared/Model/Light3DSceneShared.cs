using SharpDX;
#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    using global::SharpDX.Direct3D11;
    using ShaderManager;
    using Shaders;
    using System;
    using Utilities;
    /// <summary>
    /// Used to hold shared variables for Lights per scene
    /// </summary>
    public sealed class Light3DSceneShared : IDisposable
    {
        public LightsStruct LightModels;
        //public Vector4[] LightDirections { private set; get; }
        //public Vector4[] LightPositions { private set; get; }
        //public Vector4[] LightAtt { private set; get; }
        //public Vector4[] LightSpots { private set; get; }
        //public Color4[] LightColors { private set; get; }
        //public int[] LightTypes { private set; get; }
        //public Matrix[] LightViewMatrices { private set; get; }
        //public Matrix[] LightProjMatrices { private set; get; }

        private int lightCount = 0;
        public int LightCount
        {
            get { return lightCount; }
            set
            {
                lightCount = value;
            }
        }

        private IBufferProxy<LightsStruct> buffer;
        /// <summary>
        /// 
        /// </summary>
        public Light3DSceneShared(IConstantBufferPool pool)
        {
            LightModels.Lights = new LightStruct[LightsStruct.MaxLights];
            var cb = pool.Register(DefaultConstantBufferDescriptions.LightCB);          
            buffer = cb as IBufferProxy<LightsStruct>;
            for(int i=0; i < LightsStruct.MaxLights; ++i)
            {
                LightModels.Lights[i].LightColor = new Color4(0, 1, 0, 1);
                LightModels.Lights[i].LightType = 1;
                LightModels.Lights[i].LightDir = new Vector4(0, 1, 1, 1);
            }
            //LightDirections = new Vector4[MaxLights];
            //LightPositions = new Vector4[MaxLights];
            //LightAtt = new Vector4[MaxLights];
            //LightSpots = new Vector4[MaxLights];
            //LightColors = new Color4[MaxLights];
            //LightTypes = new int[MaxLights];
            //LightViewMatrices = new Matrix[MaxLights];
            //LightProjMatrices = new Matrix[MaxLights];
        }

        public void UploadToBuffer(DeviceContext context)
        {
            if (buffer.Buffer != null && !buffer.Buffer.IsDisposed)
            {
                buffer.UploadDataToBuffer(context, ref LightModels);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    buffer = null;
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Light3DSceneShared() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

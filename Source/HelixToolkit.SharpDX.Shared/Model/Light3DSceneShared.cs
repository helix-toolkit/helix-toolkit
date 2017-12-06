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
        public const int MaxLights = 16;
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
            LightModels.Lights = new LightStruct[MaxLights];
            var cb = pool.Get(DefaultConstantBufferDescriptions.LightCB);          
            buffer = cb as IBufferProxy<LightsStruct>;
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
            { buffer.UploadDataToBuffer(context, ref LightModels); }
        }
        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            LightCount = 0;
            LightModels.Lights = new LightStruct[MaxLights];
            LightModels.AmbientLight = new Color4();

                //LightDirections[i] = new Vector4();
                //LightPositions[i] = new Vector4();
                //LightAtt[i] = new Vector4();
                //LightSpots[i] = new Vector4();
                //LightColors[i] = new Color4();
                //LightTypes[i] = 0;
                //LightViewMatrices[i] = new Matrix();
                //LightProjMatrices[i] = new Matrix();
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

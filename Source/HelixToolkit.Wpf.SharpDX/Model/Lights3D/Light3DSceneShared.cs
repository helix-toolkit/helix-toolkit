using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Model.Lights3D
{
    /// <summary>
    /// Used to hold shared variables for Lights per scene
    /// </summary>
    public sealed class Light3DSceneShared
    {
        public const int MaxLights = 16;
        public Vector4[] LightDirections { private set; get; }
        public Vector4[] LightPositions { private set; get; }
        public Vector4[] LightAtt { private set; get; }
        public Vector4[] LightSpots { private set; get; }
        public Color4[] LightColors { private set; get; }
        public int[] LightTypes { private set; get; }
        public Matrix[] LightViewMatrices { private set; get; }
        public Matrix[] LightProjMatrices { private set; get; }

        private int lightCount = 0;
        public int LightCount
        {
            get { return lightCount; }
            set
            {
                lightCount = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Light3DSceneShared()
        {
            LightDirections = new Vector4[MaxLights];
            LightPositions = new Vector4[MaxLights];
            LightAtt = new Vector4[MaxLights];
            LightSpots = new Vector4[MaxLights];
            LightColors = new Color4[MaxLights];
            LightTypes = new int[MaxLights];
            LightViewMatrices = new Matrix[MaxLights];
            LightProjMatrices = new Matrix[MaxLights];
        }
        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            LightCount = 0;
            for (int i = 0; i < MaxLights; ++i)
            {
                LightDirections[i] = new Vector4();
                LightPositions[i] = new Vector4();
                LightAtt[i] = new Vector4();
                LightSpots[i] = new Vector4();
                LightColors[i] = new Color4();
                LightTypes[i] = 0;
                LightViewMatrices[i] = new Matrix();
                LightProjMatrices[i] = new Matrix();
            }
        }
    }
}

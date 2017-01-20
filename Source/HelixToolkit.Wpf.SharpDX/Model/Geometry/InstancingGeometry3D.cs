using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX.Model.Geometry
{
    public class InstancingGeometry3D : Geometry3D
    {
        private Matrix[] instanceMatrixArray;
        public Matrix[] InstanceMatrixArray
        {
            set
            {
                Set(ref instanceMatrixArray, value);
            }
            get
            {
                return instanceMatrixArray;
            }
        }

        private Color4[] diffuseColorArray;

        public Color4[] DiffuseColorArray
        {
            set
            {
                Set(ref diffuseColorArray, value);
            }
            get
            {
                return diffuseColorArray;
            }
        }

        private float[] textureOffsetArray;
        public float[] TextureOffsetArray
        {
            set
            {
                Set(ref textureOffsetArray, value);
            }
            get
            {
                return textureOffsetArray;
            }
        }

        public InstancingGeometry3D()
        {

        }
    }
}

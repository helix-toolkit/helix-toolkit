/*
The MIT License (MIT)
Copyright (c) 2022 Helix Toolkit contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Original code from:
SharpDX project. https://github.com/sharpdx/SharpDX
SlimMath project. http://code.google.com/p/slimmath/

Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
The MIT License (MIT)
Copyright (c) 2007-2011 SlimDX Group
The MIT License (MIT)
*/
using System.Runtime.InteropServices;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
namespace HelixToolkit.Maths
{
    /// <summary>
    /// Frustum camera parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FrustumCameraParams
    {
        /// <summary>
        /// Position of the camera.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Looking at direction of the camera.
        /// </summary>
        public Vector3 LookAtDir;

        /// <summary>
        /// Up direction.
        /// </summary>
        public Vector3 UpDir;

        /// <summary>
        /// Field of view.
        /// </summary>
        public float FOV;

        /// <summary>
        /// Z near distance.
        /// </summary>
        public float ZNear;

        /// <summary>
        /// Z far distance.
        /// </summary>
        public float ZFar;

        /// <summary>
        /// Aspect ratio.
        /// </summary>
        public float AspectRatio;
    }
}
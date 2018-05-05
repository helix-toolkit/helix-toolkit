/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InputLayoutDescription
    {
        /// <summary>
        /// The empty input layout
        /// </summary>
        public static readonly InputLayoutDescription EmptyInputLayout = new InputLayoutDescription();

        public readonly KeyValuePair<byte[], InputElement[]> Description;
        /// <summary>
        /// Initializes a new instance of the <see cref="InputLayoutDescription"/> class.
        /// </summary>
        /// <param name="byteCode">The byte code.</param>
        /// <param name="elements">The elements.</param>
        public InputLayoutDescription(byte[] byteCode, InputElement[] elements)
        {
            Description = new KeyValuePair<byte[], InputElement[]>(byteCode, elements);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InputLayoutDescription"/> class.
        /// </summary>
        public InputLayoutDescription()
        {
            Description = new KeyValuePair<byte[], InputElement[]>(null, new InputElement[0]);
        }
    }
}

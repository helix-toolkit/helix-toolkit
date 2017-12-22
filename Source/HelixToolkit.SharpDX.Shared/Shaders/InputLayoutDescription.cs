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
    public class InputLayoutDescription
    {
        public readonly Tuple<byte[], InputElement[]> Description;
        public InputLayoutDescription(byte[] byteCode, InputElement[] elements)
        {
            Description = new Tuple<byte[], InputElement[]>(byteCode, elements);
        }
    }
}

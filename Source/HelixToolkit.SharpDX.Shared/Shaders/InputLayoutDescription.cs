/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Helper;
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public sealed class InputLayoutDescription
    {
        /// <summary>
        /// The empty input layout
        /// </summary>
        public static readonly InputLayoutDescription EmptyInputLayout = new InputLayoutDescription();

        private byte[] shaderByteCode;
        /// <summary>
        /// Gets or sets the shader byte code.
        /// </summary>
        /// <value>
        /// The shader byte code.
        /// </value>
        [DataMember]
        public byte[] ShaderByteCode
        {
            set
            {
                shaderByteCode = value;
            }
            get
            {
                if(shaderByteCode == null && !string.IsNullOrEmpty(ShaderByteCodeName))
                {
                    shaderByteCode = UWPShaderBytePool.Read(ShaderByteCodeName, byteCodeReader);
                }
                return shaderByteCode;
            }
        }

        [IgnoreDataMember]
        public string ShaderByteCodeName { private set; get; }
        /// <summary>
        /// Gets or sets the input elements.
        /// </summary>
        /// <value>
        /// The input elements.
        /// </value>
        [DataMember]
        public InputElement[] InputElements { set; get; } = new InputElement[0];

        public KeyValuePair<byte[], InputElement[]> Description { get { return new KeyValuePair<byte[], InputElement[]>(ShaderByteCode, InputElements); } }

        private readonly IShaderByteCodeReader byteCodeReader;
        /// <summary>
        /// Initializes a new instance of the <see cref="InputLayoutDescription"/> class.
        /// </summary>
        /// <param name="byteCode">The byte code.</param>
        /// <param name="elements">The elements.</param>
        public InputLayoutDescription(byte[] byteCode, InputElement[] elements)
        {
            ShaderByteCode = byteCode;
            InputElements = elements;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InputLayoutDescription"/> class. Pass custom <see cref="IShaderByteCodeReader"/> to read external shader bytecodes.
        /// </summary>
        /// <param name="byteCodeName">The byte code name.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="byteCodeReader"></param>
        public InputLayoutDescription(string byteCodeName, InputElement[] elements, IShaderByteCodeReader byteCodeReader = null)
        {
            ShaderByteCodeName = byteCodeName;
            InputElements = elements;
            this.byteCodeReader = byteCodeReader ?? UWPShaderBytePool.InternalByteCodeReader;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InputLayoutDescription"/> class.
        /// </summary>
        public InputLayoutDescription()
        {
        }
    }
}

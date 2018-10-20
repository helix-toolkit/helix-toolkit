/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX.Direct3D11;
using System.Runtime.Serialization;
using System.Collections.Generic;
using SharpDX.D3DCompiler;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{   
    using Utilities;
    public struct ConstantBufferVariable
    {
        //
        // Summary:
        //     The variable name.
        public string Name;
        //
        // Summary:
        //     Offset from the start of the parent structure to the beginning of the variable.
        public int StartOffset;
        //
        // Summary:
        //     Size of the variable (in bytes).
        public int Size;
    }

    public sealed class ConstantBufferDescription
    {
        public string Name { set; get; }

        public int StructSize { set; get; }

        public int StrideSize { set; get; }

        public BindFlags BindFlags { set; get; } = BindFlags.ConstantBuffer;

        public CpuAccessFlags CpuAccessFlags { set; get; } = CpuAccessFlags.Write;

        public ResourceOptionFlags OptionFlags { set; get; } = ResourceOptionFlags.None;

        public ResourceUsage Usage { set; get; } = ResourceUsage.Dynamic;

        public ShaderStage Stage { set; get; }

        public int Slot { set; get; }

        public List<ConstantBufferVariable> Variables { get; } = new List<ConstantBufferVariable>();

        public ConstantBufferDescription(ConstantBuffer buffer)
        {
            Name = buffer.Description.Name;
            StructSize = StrideSize = buffer.Description.Size;
            Variables = new List<ConstantBufferVariable>();
            for(int i=0; i < buffer.Description.VariableCount; ++i)
            {
                var variable = buffer.GetVariable(i);
                Variables.Add(new ConstantBufferVariable() { Name = variable.Description.Name, Size = variable.Description.Size, StartOffset = variable.Description.StartOffset });
            }
        }

        public ConstantBufferDescription(string name, int structSize, int strideSize = 0)
        {
            Name = name;
            StructSize = structSize;
            StrideSize = strideSize;
        }

        public ConstantBufferProxy CreateBuffer()
        {
            return new ConstantBufferProxy(this);
        }

        public ConstantBufferMapping CreateMapping(int slot)
        {
            return new ConstantBufferMapping(slot, this);
        }

        public ConstantBufferDescription Clone()
        {
            return new ConstantBufferDescription(Name, StructSize, StrideSize)
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = this.CpuAccessFlags,
                OptionFlags = this.OptionFlags,
                Usage = this.Usage
            };
        }
    }

    [DataContract]
    public sealed class ConstantBufferMapping
    {
        [DataMember]
        public int Slot { set; get; }

        [DataMember]
        public ConstantBufferDescription Description { set; get; }

        public ConstantBufferMapping(int slot, ConstantBufferDescription description)
        {
            Slot = slot;
            Description = description;
        }

        public static ConstantBufferMapping Create(int slot, ConstantBufferDescription description)
        {
            return new ConstantBufferMapping(slot, description);
        }

        public ConstantBufferMapping Clone()
        {
            return new ConstantBufferMapping(this.Slot, this.Description.Clone());
        }
    }
}
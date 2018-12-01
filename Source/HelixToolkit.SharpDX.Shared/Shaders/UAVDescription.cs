using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Shaders
    {
        public enum UnorderedAccessViewType
        {
            AppendStructured,
            ConsumeStructured,
            RWByteAddress,
            RWStructuredWithCounter,
            RWTyped
        };
        public sealed class UAVDescription
        {
            [DataMember]
            public string Name { set; get; }
            [DataMember]
            public ShaderStage ShaderType;
            [DataMember]
            public UnorderedAccessViewType Type;

            public UAVDescription() { }

            public UAVDescription(string name, ShaderStage shaderType, UnorderedAccessViewType type)
            {
                Name = name;
                ShaderType = shaderType;
                Type = type;
            }

            public UAVMapping CreateMapping(int slot)
            {
                return new UAVMapping(slot, this);
            }

            public UAVDescription Clone()
            {
                return new UAVDescription(this.Name, this.ShaderType, this.Type);
            }
        }

        [DataContract]
        public sealed class UAVMapping
        {
            [DataMember]
            public int Slot { set; get; }
            [DataMember]
            public UAVDescription Description { set; get; }

            public UAVMapping(int slot, UAVDescription description)
            {
                Slot = slot;
                Description = description;
            }

            public UAVMapping Clone()
            {
                return new UAVMapping(this.Slot, this.Description.Clone());
            }
        }
    }

}

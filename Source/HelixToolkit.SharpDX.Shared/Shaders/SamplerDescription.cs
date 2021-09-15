using System.Runtime.Serialization;

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
        /// <summary>
        /// 
        /// </summary>
        [DataContract]
        public sealed class SamplerMapping
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            [DataMember]
            public string Name { set; get; }
            /// <summary>
            /// The shader type
            /// </summary>
            [DataMember]
            public ShaderStage ShaderType;
            /// <summary>
            /// Gets or sets the slot.
            /// </summary>
            /// <value>
            /// The slot.
            /// </value>
            [DataMember]
            public int Slot { set; get; }

            public SamplerMapping() { }

            public SamplerMapping(int slot, string name, ShaderStage type)
            {
                Slot = slot;
                Name = name;
                ShaderType = type;
            }

            public SamplerMapping Clone()
            {
                return new SamplerMapping(this.Slot, this.Name, this.ShaderType);
            }
        }
    }

}

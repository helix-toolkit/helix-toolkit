/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using global::SharpDX.Direct3D11;
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
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="MappingType"></typeparam>
        public sealed class MappingProxy<MappingType> where MappingType : class
        {
            private readonly MappingCollection<int, string, MappingType> mappingCollection = new MappingCollection<int, string, MappingType>();
            /// <summary>
            /// 
            /// </summary>
            public KeyValuePair<int, MappingType>[] Mappings { get { return mappingCollection.MappingArray; } }
            /// <summary>
            /// 
            /// </summary>
            public int Count { get { return mappingCollection.Count; } }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="slot"></param>
            /// <param name="mapping"></param>
            public void AddMapping(string name, int slot, MappingType mapping)
            {
                mappingCollection.Add(slot, name, mapping);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            public void RemoveMapping(string name)
            {
                mappingCollection.Remove(name);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="slot"></param>
            public void RemoveMapping(int slot)
            {
                mappingCollection.Remove(slot);
            }
            /// <summary>
            /// 
            /// </summary>
            public void ClearMapping()
            {
                mappingCollection.Clear();
            }
            /// <summary>
            /// Try get slot by name. If failed, return -1;
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public int TryGetBindSlot(string name)
            {
                int item;
                return mappingCollection.TryGetSlot(name, out item) ? item : -1;
            }
            /// <summary>
            /// Try to get name by register slot. If failed, return empty string;
            /// </summary>
            /// <param name="slot"></param>
            /// <returns></returns>
            public string TryGetName(int slot)
            {
                string item;
                return mappingCollection.TryGetName(slot, out item) ? item : "";
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public MappingType GetMapping(string name)
            {
                MappingType item;
                if(mappingCollection.TryGetItem(name, out item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="slot"></param>
            /// <returns></returns>
            public MappingType GetMapping(int slot)
            {
                MappingType item;
                if (mappingCollection.TryGetItem(slot, out item))
                {
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }
    }

}

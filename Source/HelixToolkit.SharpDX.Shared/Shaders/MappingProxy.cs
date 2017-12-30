/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using System.Collections;
    using global::SharpDX.Direct3D11;
    using Utilities;
    public sealed class MappingProxy<MappingType> where MappingType : class
    {
        private readonly MappingCollection<int, string, MappingType> mappingCollection = new MappingCollection<int, string, MappingType>();
        public IEnumerable<KeyValuePair<int, MappingType>> Mappings { get { return mappingCollection.DataMapping; } }

        public int Count { get { return mappingCollection.Count; } }

        public void AddMapping(string name, int slot, MappingType mapping)
        {
            mappingCollection.Add(slot, name, mapping);
        }

        public void RemoveMapping(string name)
        {
            mappingCollection.Remove(name);
        }

        public void RemoveMapping(int slot)
        {
            mappingCollection.Remove(slot);
        }

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

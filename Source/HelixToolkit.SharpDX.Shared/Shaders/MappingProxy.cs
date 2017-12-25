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
        public IEnumerable<Tuple<int, MappingType>> Mappings { get { return mappingCollection.DataMapping; } }

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
            return mappingCollection.TryGetItem(name, out item) ? item : -1;
        }
        /// <summary>
        /// Try to get name by register slot. If failed, return empty string;
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string TryGetName(int slot)
        {
            Tuple<string, MappingType> item;
            return mappingCollection.TryGetItem(slot, out item) ? item.Item1 : "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MappingType GetMapping(string name)
        {
            if (mappingCollection.HasItem(name))
            {
                return mappingCollection[name].Item2;
            }
            else
            {
                return null;
            }
        }

        public MappingType GetMapping(int slot)
        {
            if (mappingCollection.HasItem(slot))
            {
                return mappingCollection[slot].Item2;
            }
            else
            {
                return null;
            }
        }
    }
}

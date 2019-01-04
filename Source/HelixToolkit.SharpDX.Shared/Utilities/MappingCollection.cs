/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    namespace Utilities
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="INDEXTYPE"></typeparam>
        /// <typeparam name="NAMETYPE"></typeparam>
        /// <typeparam name="DATATYPE"></typeparam>
        public sealed class MappingCollection<INDEXTYPE, NAMETYPE, DATATYPE>
        {
            private readonly Dictionary<INDEXTYPE, NAMETYPE> indexNameMapping = new Dictionary<INDEXTYPE, NAMETYPE>();
            private readonly Dictionary<NAMETYPE, INDEXTYPE> nameIndexMapping = new Dictionary<NAMETYPE, INDEXTYPE>();
            private readonly Dictionary<INDEXTYPE, DATATYPE> indexDataMapping = new Dictionary<INDEXTYPE, DATATYPE>();
            /// <summary>
            /// 
            /// </summary>
            public KeyValuePair<INDEXTYPE, DATATYPE>[] MappingArray { private set; get; } = new KeyValuePair<INDEXTYPE, DATATYPE>[0];
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<DATATYPE> Datas { get { return indexDataMapping.Values; } }
            /// <summary>
            /// 
            /// </summary>
            public int Count { get { return indexNameMapping.Count; } }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="name"></param>
            /// <param name="item"></param>
            public void Add(INDEXTYPE index, NAMETYPE name,  DATATYPE item)
            {
                if (nameIndexMapping.ContainsKey(name))
                {
                    throw new ArgumentException("Cannot add duplicate name.");
                }
                if (indexNameMapping.ContainsKey(index))
                {
                    throw new ArgumentException("Cannot add duplicate index");
                }
                indexNameMapping.Add(index, name);
                nameIndexMapping.Add(name, index);
                indexDataMapping.Add(index, item);
                MappingArray = indexDataMapping.ToArray();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool Remove(INDEXTYPE index)
            {
                if (indexNameMapping.ContainsKey(index))
                {
                    nameIndexMapping.Remove(indexNameMapping[index]);
                    indexNameMapping.Remove(index);
                    indexDataMapping.Remove(index);
                    MappingArray = indexDataMapping.ToArray();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Remove(NAMETYPE name)
            {
                if (nameIndexMapping.ContainsKey(name))
                {
                    indexNameMapping.Remove(nameIndexMapping[name]);                
                    indexDataMapping.Remove(nameIndexMapping[name]);
                    nameIndexMapping.Remove(name);
                    MappingArray = indexDataMapping.ToArray();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public bool HasItem(INDEXTYPE id)
            {
                return indexNameMapping.ContainsKey(id);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public bool TryGetItem(INDEXTYPE id, out DATATYPE data)
            {
                return indexDataMapping.TryGetValue(id, out data);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool TryGetSlot(NAMETYPE name, out INDEXTYPE index)
            {
                return nameIndexMapping.TryGetValue(name, out index);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool TryGetName(INDEXTYPE id, out NAMETYPE name)
            {
                return indexNameMapping.TryGetValue(id, out name);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool HasItem(NAMETYPE name)
            {
                return nameIndexMapping.ContainsKey(name);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public bool TryGetItem(NAMETYPE name, out DATATYPE data)
            {
                INDEXTYPE idx;
                if(nameIndexMapping.TryGetValue(name, out idx) && indexDataMapping.TryGetValue(idx, out data))
                {
                    return true;
                }
                else
                {
                    data = default(DATATYPE);
                    return false;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public void Clear()
            {
                nameIndexMapping.Clear();
                indexNameMapping.Clear();
                indexDataMapping.Clear();
                MappingArray = new KeyValuePair<INDEXTYPE, DATATYPE>[0];
            }
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<INDEXTYPE> Keys
            {
                get { return indexNameMapping.Keys; }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public DATATYPE this[INDEXTYPE key]
            {
                get { return indexDataMapping[key]; }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public INDEXTYPE this[NAMETYPE name]
            {
                get { return nameIndexMapping[name]; }
            }
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="INDEXTYPE"></typeparam>
    /// <typeparam name="DATATYPE"></typeparam>
    public class MappingCollection<INDEXTYPE, NAMETYPE, DATATYPE>
    {
        private readonly Dictionary<INDEXTYPE, Tuple<NAMETYPE, DATATYPE>> indexMapping = new Dictionary<INDEXTYPE, Tuple<NAMETYPE, DATATYPE>>();
        private readonly Dictionary<NAMETYPE, INDEXTYPE> nameMapping = new Dictionary<NAMETYPE, INDEXTYPE>();

        public IEnumerable<DATATYPE> Datas { get { return indexMapping.Values.Select(x => x.Item2); } }
        public IEnumerable<Tuple<INDEXTYPE, DATATYPE>> DataMapping { get { return indexMapping.Select(x => Tuple.Create(x.Key, x.Value.Item2)); } }
        public int Count { get { return indexMapping.Count; } }

        public void Add(INDEXTYPE index, NAMETYPE name,  DATATYPE item)
        {
            if (nameMapping.ContainsKey(name))
            {
                throw new ArgumentException("Cannot add duplicate name.");
            }
            if (indexMapping.ContainsKey(index))
            {
                throw new ArgumentException("Cannot add duplicate index");
            }
            indexMapping.Add(index, Tuple.Create(name, item));
            nameMapping.Add(name, index);
        }

        public bool Remove(INDEXTYPE index)
        {
            if (indexMapping.ContainsKey(index))
            {
                nameMapping.Remove(indexMapping[index].Item1);
                indexMapping.Remove(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(NAMETYPE name)
        {
            if (nameMapping.ContainsKey(name))
            {
                indexMapping.Remove(nameMapping[name]);
                nameMapping.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasItem(INDEXTYPE id)
        {
            return indexMapping.ContainsKey(id);
        }

        public bool HasItem(NAMETYPE name)
        {
            return nameMapping.ContainsKey(name);
        }

        public void Clear()
        {
            nameMapping.Clear();
            indexMapping.Clear();
        }

        public IEnumerable<INDEXTYPE> Keys
        {
            get { return indexMapping.Keys; }
        }

        public Tuple<NAMETYPE, DATATYPE> this[INDEXTYPE key]
        {
            get { return indexMapping[key]; }
        }

        public Tuple<NAMETYPE, DATATYPE> this[NAMETYPE name]
        {
            get { return indexMapping[nameMapping[name]]; }
        }
    }
}

using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
///
/// </summary>
/// <typeparam name="INDEXTYPE"></typeparam>
/// <typeparam name="NAMETYPE"></typeparam>
/// <typeparam name="DATATYPE"></typeparam>
public sealed class MappingCollection<INDEXTYPE, NAMETYPE, DATATYPE>
    where INDEXTYPE : notnull
    where NAMETYPE : notnull
{
    private readonly Dictionary<INDEXTYPE, NAMETYPE> indexNameMapping = new();
    private readonly Dictionary<NAMETYPE, INDEXTYPE> nameIndexMapping = new();
    private readonly Dictionary<INDEXTYPE, DATATYPE> indexDataMapping = new();

    /// <summary>
    /// 
    /// </summary>
    public KeyValuePair<INDEXTYPE, DATATYPE>[] MappingArray { private set; get; } = Array.Empty<KeyValuePair<INDEXTYPE, DATATYPE>>();

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<DATATYPE> Datas
    {
        get
        {
            return indexDataMapping.Values;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Count
    {
        get
        {
            return indexNameMapping.Count;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="item"></param>
    public void Add(INDEXTYPE index, NAMETYPE name, DATATYPE item)
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
    public bool TryGetItem(INDEXTYPE id, [MaybeNullWhen(false)] out DATATYPE? data)
    {
        return indexDataMapping.TryGetValue(id, out data);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool TryGetSlot(NAMETYPE name, [MaybeNullWhen(false)] out INDEXTYPE? index)
    {
        return nameIndexMapping.TryGetValue(name, out index);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool TryGetName(INDEXTYPE id, [MaybeNullWhen(false)] out NAMETYPE? name)
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
    public bool TryGetItem(NAMETYPE name, [MaybeNullWhen(false)] out DATATYPE? data)
    {
        if (nameIndexMapping.TryGetValue(name, out INDEXTYPE? idx) && indexDataMapping.TryGetValue(idx, out data))
        {
            return true;
        }
        else
        {
            data = default;
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
        MappingArray = Array.Empty<KeyValuePair<INDEXTYPE, DATATYPE>>();
    }
    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<INDEXTYPE> Keys
    {
        get
        {
            return indexNameMapping.Keys;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public DATATYPE this[INDEXTYPE key]
    {
        get
        {
            return indexDataMapping[key];
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public INDEXTYPE this[NAMETYPE name]
    {
        get
        {
            return nameIndexMapping[name];
        }
    }
}

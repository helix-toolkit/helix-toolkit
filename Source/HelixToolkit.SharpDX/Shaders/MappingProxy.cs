using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
/// <typeparam name="MappingType"></typeparam>
public sealed class MappingProxy<MappingType> : DisposeObject where MappingType : class
{
    private readonly MappingCollection<int, string, MappingType> mappingCollection = new();
    /// <summary>
    /// 
    /// </summary>
    public KeyValuePair<int, MappingType>[] Mappings
    {
        get
        {
            return mappingCollection.MappingArray;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Count
    {
        get
        {
            return mappingCollection.Count;
        }
    }
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
        return mappingCollection.TryGetSlot(name, out int item) ? item : -1;
    }
    /// <summary>
    /// Try to get name by register slot. If failed, return empty string;
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public string TryGetName(int slot)
    {
        return mappingCollection.TryGetName(slot, out string? item) ? item! : string.Empty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MappingType? GetMapping(string name)
    {
        if (mappingCollection.TryGetItem(name, out MappingType? item))
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
    public MappingType? GetMapping(int slot)
    {
        if (mappingCollection.TryGetItem(slot, out MappingType? item))
        {
            return item;
        }
        else
        {
            return null;
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        foreach (var item in mappingCollection.Datas)
        {
            if (item is IDisposable toDispose)
            {
                toDispose.Dispose();
            }
        }
        mappingCollection.Clear();
        base.OnDispose(disposeManagedResources);
    }
}

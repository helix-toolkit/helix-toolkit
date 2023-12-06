using HelixToolkit.SharpDX.Model;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class OctreeBuildParameter : ObservableObject
{
    private float minimumOctantSize = 1f;
    /// <summary>
    /// Minimum Octant size.
    /// </summary>
    public float MinimumOctantSize
    {
        set
        {
            Set(ref minimumOctantSize, value);
        }
        get
        {
            return minimumOctantSize;
        }
    }

    private int minObjectSizeToSplit = 2;
    /// <summary>
    /// Minimum object in each octant to start splitting into smaller octant during build
    /// </summary>
    public int MinObjectSizeToSplit
    {
        set
        {
            Set(ref minObjectSizeToSplit, value);
        }
        get
        {
            return minObjectSizeToSplit;
        }
    }

    private bool autoDeleteIfEmpty = true;
    /// <summary>
    /// Delete empty octant automatically
    /// </summary>
    public bool AutoDeleteIfEmpty
    {
        set
        {
            Set(ref autoDeleteIfEmpty, value);
        }
        get
        {
            return autoDeleteIfEmpty;
        }
    }

    private bool cubify = false;
    /// <summary>
    /// Generate cube octants instead of rectangle octants
    /// </summary>
    public bool Cubify
    {
        set
        {
            Set(ref cubify, value);
        }
        get
        {
            return cubify;
        }
    }
    /// <summary>
    /// Record hit path bounding boxes for debugging or display purpose only
    /// </summary>
    public bool RecordHitPathBoundingBoxes { set; get; } = false;
    /// <summary>
    /// Use parallel tree traversal to build the octree
    /// </summary>
    public bool EnableParallelBuild { set; get; } = false;
    /// <summary>
    /// 
    /// </summary>
    public OctreeBuildParameter()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minSize"></param>
    public OctreeBuildParameter(float minSize)
    {
        MinimumOctantSize = Math.Max(0, minSize);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="autoDeleteIfEmpty"></param>
    public OctreeBuildParameter(bool autoDeleteIfEmpty)
    {
        AutoDeleteIfEmpty = autoDeleteIfEmpty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minSize"></param>
    /// <param name="autoDeleteIfEmpty"></param>
    public OctreeBuildParameter(int minSize, bool autoDeleteIfEmpty)
        : this(minSize)
    {
        AutoDeleteIfEmpty = autoDeleteIfEmpty;
    }
}

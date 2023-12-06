using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core2D;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Model.Scene2D;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public static class Constants
{
    public const int MaxLights = 8;

    /// <summary>
    /// Number of shader stages
    /// </summary>
    public const int NumShaderStages = 6;

    /// <summary>
    /// Stages that can bind textures
    /// </summary>
    public const ShaderStage CanBindTextureStages = ShaderStage.Vertex | ShaderStage.Pixel | ShaderStage.Domain | ShaderStage.Compute;

    /// <summary>
    /// 
    /// </summary>
    public const int VertexIdx = 0, HullIdx = 1, DomainIdx = 2, GeometryIdx = 3, PixelIdx = 4, ComputeIdx = 5;

    /// <summary>
    /// Convert shader stage into 0~5 stage numbers
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToIndex(this ShaderStage type)
    {
        return type switch
        {
            ShaderStage.Vertex => VertexIdx,
            ShaderStage.Hull => HullIdx,
            ShaderStage.Domain => DomainIdx,
            ShaderStage.Geometry => GeometryIdx,
            ShaderStage.Pixel => PixelIdx,
            ShaderStage.Compute => ComputeIdx,
            _ => -1,
        };
    }

    /// <summary>
    /// To the shader stage.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ShaderStage ToShaderStage(this int index)
    {
        return index switch
        {
            VertexIdx => ShaderStage.Vertex,
            DomainIdx => ShaderStage.Domain,
            HullIdx => ShaderStage.Hull,
            GeometryIdx => ShaderStage.Geometry,
            PixelIdx => ShaderStage.Pixel,
            ComputeIdx => ShaderStage.Compute,
            _ => ShaderStage.None,
        };
    }

    /// <summary>
    /// Gets the null shader.
    /// </summary>
    /// <param name="stage">The stage.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ShaderBase? GetNullShader(ShaderStage stage)
    {
        return stage switch
        {
            ShaderStage.Vertex => VertexShader.NullVertexShader,
            ShaderStage.Domain => DomainShader.NullDomainShader,
            ShaderStage.Hull => HullShader.NullHullShader,
            ShaderStage.Geometry => GeometryShader.NullGeometryShader,
            ShaderStage.Pixel => PixelShader.NullPixelShader,
            ShaderStage.Compute => ComputeShader.NullComputeShader,
            _ => null,
        };
    }

    public static readonly char[] Separators = { ';', ' ', ',' };

    public static readonly FastList<KeyValuePair<int, SceneNode>> EmptyRenderablePair = new();

    public static readonly FastList<SceneNode> EmptyRenderable = new();

    public static readonly List<RenderCore> EmptyCore = new();

    internal static readonly ObservableFastList<SceneNode> EmptyRenderableArray = new();

    internal static readonly ReadOnlyObservableFastList<SceneNode> EmptyReadOnlyRenderableArray;

    internal static readonly ObservableFastList<SceneNode2D> EmptyRenderable2D = new();

    internal static readonly ReadOnlyObservableFastList<SceneNode2D> EmptyReadOnlyRenderable2DArray;

    public static readonly IList<RenderCore2D> EmptyCore2D = Array.Empty<RenderCore2D>();

    static Constants()
    {
        EmptyReadOnlyRenderableArray = new ReadOnlyObservableFastList<SceneNode>(EmptyRenderableArray);
        EmptyReadOnlyRenderable2DArray = new ReadOnlyObservableFastList<SceneNode2D>(EmptyRenderable2D);
    }
}

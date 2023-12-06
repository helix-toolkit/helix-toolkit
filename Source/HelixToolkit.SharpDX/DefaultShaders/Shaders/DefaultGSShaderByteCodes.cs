namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultGSShaderByteCodes
{
    /// <summary>
    /// 
    /// </summary>
    public static string GSPoint
    {
        get;
    } = "gsPoint";
    /// <summary>
    /// 
    /// </summary>
    public static string GSLine
    {
        get;
    } = "gsLine";
    /// <summary>
    /// Gets the gs line arrow head.
    /// </summary>
    /// <value>
    /// The gs line arrow head.
    /// </value>
    public static string GSLineArrowHead
    {
        get;
    } = "gsLineArrowHead";
    /// <summary>
    /// Gets the gs line arrow tail.
    /// </summary>
    /// <value>
    /// The gs line arrow tail.
    /// </value>
    public static string GSLineArrowHeadTail
    {
        get;
    } = "gsLineArrowHeadTail";
    /// <summary>
    /// 
    /// </summary>
    public static string GSBillboard
    {
        get;
    } = "gsBillboard";

    /// <summary>
    /// 
    /// </summary>
    public static string GSParticle
    {
        get;
    } = "gsParticle";
    /// <summary>
    /// Gets the gs mesh normal vector.
    /// </summary>
    /// <value>
    /// The gs mesh normal vector.
    /// </value>
    public static string GSMeshNormalVector
    {
        get;
    } = "gsMeshNormalVector";

    public static string GSMeshBoneSkinnedOut
    {
        get;
    } = "gsMeshSkinnedOut";
}

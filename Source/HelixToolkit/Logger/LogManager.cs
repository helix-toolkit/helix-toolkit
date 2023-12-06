using Microsoft.Extensions.Logging;

namespace HelixToolkit.Logger;

/// <summary>
/// 
/// </summary>
public static class LogManager
{
    /// <summary>
    /// Replace factory at app start up to use custom logger.
    /// </summary>
    public static ILoggerFactory Factory { set; get; } = new DebugLoggerFactory();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILogger Create<T>()
    {
        return Factory.CreateLogger<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public static ILogger Create(string categoryName)
    {
        return Factory.CreateLogger(categoryName);
    }
}

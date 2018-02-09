using System.Diagnostics;

namespace HelixToolkit.Logger
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Logger.ILogger" />
    public sealed class DebugLogger : ILogger
    {
        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <typeparam name="MsgType">The type of the sg type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="caller">The caller.</param>
        public void Log<MsgType>(LogLevel logLevel, MsgType msg, string caller)
        {
            Debug.WriteLine($"Level: {logLevel}; Caller: {caller}; Text: {msg.ToString()};");
        }
    }
}

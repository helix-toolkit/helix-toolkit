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
        /// <param name="lineNumber"></param>
        /// <param name="className"></param>
        public void Log<MsgType>(LogLevel logLevel, MsgType msg, string className, string caller, int lineNumber)
        {
            Debug.WriteLine($"Level: {logLevel}; Class: {className}; Caller: {caller}; Line: {lineNumber}; Text: {msg.ToString()};");
        }
    }
}

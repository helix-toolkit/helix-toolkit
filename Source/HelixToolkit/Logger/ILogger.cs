namespace HelixToolkit.Logger
{
    /// <summary>
    /// Provide simple log interface for user to log helix toolkit internal logs.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <typeparam name="MsgType">The type of the sg type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="className"></param>
        /// <param name="methodName">The caller.</param>
        /// <param name="lineNumber"></param>
        void Log<MsgType>(LogLevel logLevel, MsgType msg, string className, string methodName, int lineNumber);
    }
}

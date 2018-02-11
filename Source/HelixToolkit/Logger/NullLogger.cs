namespace HelixToolkit.Logger
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Logger.ILogger" />
    public sealed class NullLogger : ILogger
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
        /// <exception cref="System.NotImplementedException"></exception>
        public void Log<MsgType>(LogLevel logLevel, MsgType msg, string className, string methodName, int lineNumber)
        {

        }
    }
}

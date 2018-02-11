using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HelixToolkit.Logger
{
    /// <summary>
    /// 
    /// </summary>
    public class LogWrapper
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWrapper"/> class.
        /// </summary>
        public LogWrapper()
        {
            logger = new NullLogger();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWrapper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public LogWrapper(ILogger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <typeparam name="MsgType">The type of the sg type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="className"></param>
        /// <param name="sourceLineNumber"></param>
        public void Log<MsgType>(LogLevel logLevel, MsgType msg, string className = "",
            [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.Log<MsgType>(logLevel, msg, className, caller, sourceLineNumber);
        }
    }
}

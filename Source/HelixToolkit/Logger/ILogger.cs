using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="caller">The caller.</param>
        void Log<MsgType>(LogLevel logLevel, MsgType msg, string caller);
    }
}

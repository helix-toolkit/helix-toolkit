using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace HelixToolkit.Logger
{
    internal class DebugLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DebugLogger(categoryName);
        }
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// A logger that writes messages in the debug output window only when a debugger is attached.
    /// </summary>
    internal sealed class DebugLogger : ILogger
    {
        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            private NullScope()
            {

            }
            public void Dispose()
            {
            }
        }
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        public DebugLogger(string name)
        {
            _name = name;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
#if DEBUG
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return Debugger.IsAttached && logLevel > LogLevel.Trace && logLevel != LogLevel.None;
#else
            return Debugger.IsAttached && logLevel > LogLevel.Debug && logLevel != LogLevel.None;
#endif
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{logLevel}: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }

            Debug.WriteLine(message, _name);
        }
    }
}

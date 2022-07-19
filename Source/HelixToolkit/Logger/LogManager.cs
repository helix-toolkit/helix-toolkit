using Microsoft.Extensions.Logging;

namespace HelixToolkit.Logger
{
    public static class LogManager
    {
        public static ILoggerFactory Factory { set; get; } = new DebugLoggerFactory();

        public static ILogger Create<T>()
        {
            return Factory.CreateLogger<T>();
        }

        public static ILogger Create(string categoryName)
        {
            return Factory.CreateLogger(categoryName);
        }
    }
}

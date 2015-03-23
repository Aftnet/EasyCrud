using System;

namespace EasyCrud.Logging
{
    public class NullLogger : ILogger
    {
        public void Log(LogSeverity severity, string message, Exception exception = null)
        {
        }
    }
}
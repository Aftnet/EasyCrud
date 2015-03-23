using System;

namespace EasyCrud.Logging
{
    public enum LogSeverity { DebugInfo, Info, Warning, Error, FatalError }

    public interface ILogger
    {
        void Log(LogSeverity severity, string message, Exception exception = null);
    }
}

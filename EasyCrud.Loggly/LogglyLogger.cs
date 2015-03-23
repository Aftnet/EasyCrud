using EasyCrud.Logging;
using log4net;
using System;
using System.Collections.Generic;

namespace EasyCrud.Loggly
{
    public abstract class LogglyLogger : ILogger
    {
        private readonly string _loggerName;
        private ILog _logger;
        private ILog Logger { get { return _logger ?? (_logger = LogManager.GetLogger(_loggerName)); } }
 
        private readonly Dictionary<LogSeverity, Tuple<Action<ILog, string>, Action<ILog, string, Exception>>> _severityDictionary = new Dictionary<LogSeverity, Tuple<Action<ILog, string>, Action<ILog, string, Exception>>>
        {
            { LogSeverity.DebugInfo, Tuple.Create<Action<ILog, string>, Action<ILog, string, Exception>>((logger,message) => logger.Debug(message), (logger,message,exception) => logger.Debug(message, exception))},
            { LogSeverity.Info, Tuple.Create<Action<ILog, string>, Action<ILog, string, Exception>>((logger,message) => logger.Info(message), (logger,message,exception) => logger.Info(message, exception))},
            { LogSeverity.Warning, Tuple.Create<Action<ILog, string>, Action<ILog, string, Exception>>((logger,message) => logger.Warn(message), (logger,message,exception) => logger.Warn(message, exception))},
            { LogSeverity.Error, Tuple.Create<Action<ILog, string>, Action<ILog, string, Exception>>((logger,message) => logger.Error(message), (logger,message,exception) => logger.Error(message, exception))},
            { LogSeverity.FatalError, Tuple.Create<Action<ILog, string>, Action<ILog, string, Exception>>((logger,message) => logger.Fatal(message), (logger,message,exception) => logger.Fatal(message, exception))},
        };

        protected LogglyLogger(string loggerName)
        {
            _loggerName = loggerName;
        }

        public void Log(LogSeverity severity, string message, Exception exception = null)
        {
            if (message == null && exception == null)
            {
                return;
            }

            var actions = _severityDictionary[severity];
            if (exception == null)
            {
                actions.Item1(Logger, message);
            }
            else
            {
                actions.Item2(Logger, message, exception);
            }
        }
    }
}

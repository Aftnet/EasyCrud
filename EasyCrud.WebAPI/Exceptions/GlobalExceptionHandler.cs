using System.Web.Http.ExceptionHandling;
using EasyCrud.Logging;

namespace EasyCrud.WebAPI.Exceptions
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        const string UnhandledExceptionErrorMsg = "Unhandled exception encountered";
        protected readonly ILogger Logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            Logger = logger;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            Logger.Log(LogSeverity.FatalError, UnhandledExceptionErrorMsg, context.Exception);
        }
    }
}

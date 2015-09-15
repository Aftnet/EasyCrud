using EasyCrud.Logging;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EasyCrud.MVC.Controllers
{
    public class ControllerBase : Controller
    {
        public const string InternalServerErrorMsg = "Internal server error";

        protected readonly ILogger Logger;

        public ControllerBase(ILogger logger)
        {
            Logger = logger;
        }

        protected void RaiseErrorMessage(HttpStatusCode statusCode, string errorMessage, LogSeverity severity, Exception exception = null)
        {
            Logger.Log(severity, errorMessage, exception);
            throw new HttpException((int)statusCode, errorMessage);
        }
    }
}

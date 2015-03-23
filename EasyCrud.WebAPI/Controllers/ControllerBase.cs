using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EasyCrud.Logging;

namespace EasyCrud.WebAPI.Controllers
{
    public class ControllerBase : ApiController
    {
        public const string InternalServerErrorMsg = "Internal server error";

        protected readonly ILogger Logger;

        public ControllerBase(ILogger logger)
        {
            Logger = logger;
        }

        protected void RaiseErrorMessage(HttpStatusCode statusCode, string errorMessage, LogSeverity severity, Exception exception = null)
        {
            var message = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage)
            };

            Logger.Log(severity, errorMessage, exception);
            throw new HttpResponseException(message);
        }
    }
}

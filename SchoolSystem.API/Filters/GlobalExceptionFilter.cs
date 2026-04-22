using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Models.Response;
using SchoolSystem.Application.Services.IServices;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SchoolSystem.API.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext?.Request == null)
                return;

            var exception = actionExecutedContext.Exception ?? new Exception("Unhandled exception with no details.");
            var errorLogsService = (IErrorLogsService)actionExecutedContext.Request
                .GetDependencyScope()
                .GetService(typeof(IErrorLogsService));

            if (errorLogsService != null)
            {
                try
                {
                    var logModel = new ErrorLogsModel
                    {
                        ErrorCode = "500",
                        TechnicalMessage = exception.ToString(),
                        StackTraces = exception.StackTrace,
                        Source = exception.Source,
                        UserMessage = "Internal server error, the IT team will fix it",
                        CreatedAt = DateTime.UtcNow
                    };

                    _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            await errorLogsService.AddErrorLogAsync(logModel);
                        }
                        catch
                        {
                        }
                    });
                }
                catch
                {
                }
            }

            var responseData = ResponseData<object>.Failure("An internal server error occurred. Please try again later.");

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                responseData);
        }
    }
}

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Infrastructure.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter {
    private readonly IWebHostEnvironment _environment;

    private readonly ILogger<HttpGlobalExceptionFilter> _logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment environment,
        ILogger<HttpGlobalExceptionFilter> logger) {
        _environment = environment;
        _logger = logger;
    }

    public void OnException(ExceptionContext context) {
        _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception, context.Exception.Message);

        context.Result = new OkObjectResult((_environment.IsDevelopment()
                ? ServiceResult.CreateExceptionResult(context.Exception,
                    context.Exception.Message)
                : ServiceResult.CreateExceptionResult("出错了。"))
            .ToServiceResultViewModel());

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        context.ExceptionHandled = true;
    }
}
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskmanAPI.Exceptions;

namespace TaskmanAPI.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case InsufficientPrivilegesException:
                context.Result = new ContentResult
                {
                    Content = context.Exception.Message,
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                context.ExceptionHandled = true;
                break;
            case EntityNotFoundException:
                context.Result = new ContentResult
                {
                    Content = context.Exception.Message,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
                context.ExceptionHandled = true;
                break;
        }
    }
}
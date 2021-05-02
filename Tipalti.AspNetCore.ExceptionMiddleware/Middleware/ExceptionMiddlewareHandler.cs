using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tipalti.AspNetCore.ExceptionMiddleware.Models;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Middleware
{
    internal class ExceptionMiddlewareHandler
    {
        private readonly ExceptionMiddlewareOptions _options;
        private readonly ILogger _log;

        internal ExceptionMiddlewareHandler(ExceptionMiddlewareOptions options, ILogger log)
        {
            _options = options;
            _log = log;
        }

        internal void UseMiddleware(IApplicationBuilder builder)
        {
            builder.UseExceptionHandler(cfg =>
            {
                cfg.Run(HandleException);
            });
        }

        private Task HandleException(HttpContext context)
        {
            IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (contextFeature == null)
            {
                _log.LogError("An exception was caught, but the IExceptionHandlerFeature was null. Passing it over to the default handler..");

                return Task.CompletedTask;
            }

            Exception exception = contextFeature.Error;
            Type exceptionType = exception.GetType();

            _log.LogInformation($"An exception was caught of type {exceptionType.Name}");

            var matchingHandler = GetMatchingHandlerForException(exceptionType);

            if (matchingHandler == null)
            {
                _log.LogInformation($"No exception handler found for type {exceptionType.Name}, passing to the default handler");

                return Task.CompletedTask;
            }

            ExceptionResult responseToWrite = matchingHandler.Handler.DynamicInvoke(exception) as ExceptionResult;

            var responseString = JsonSerializer
                .Serialize(new ExceptionMiddlewareResponse
                {
                    ErrorCode = responseToWrite.ErrorCode
                }, _options.JsonSerializerOptions);

            context.Response.StatusCode = (int)responseToWrite.StatusCode;
            context.Response.ContentType = _options.ContentType;

            return context.Response.WriteAsync(responseString);
        }

        private ExceptionHandler GetMatchingHandlerForException(Type exceptionType)
        {
            return _options.UseStrictExceptionChecking
                ? _options.Handlers.FirstOrDefault(f => f.ExceptionType == exceptionType)
                : _options.Handlers.FirstOrDefault(f => exceptionType.IsSubclassOf(f.ExceptionType));
        }
    }
}

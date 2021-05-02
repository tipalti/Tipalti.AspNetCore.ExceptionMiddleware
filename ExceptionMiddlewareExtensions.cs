using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Tipalti.AspNetCore.ExceptionMiddleware.Middleware;
using Tipalti.AspNetCore.ExceptionMiddleware.Models;

namespace Tipalti.AspNetCore.ExceptionMiddleware
{
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Configure and use the exception middleware in the pipeline.
        /// </summary>
        /// <param name="configureAction">An action to configure the middleware.</param>
        public static void UseExceptionMiddleware(this IApplicationBuilder builder, Action<ExceptionMiddlewareOptions> configureAction)
        {
            if (configureAction == null)
                throw new ArgumentNullException(nameof(configureAction), "The configuration action cannot be null!");

            var log = builder.ApplicationServices.GetRequiredService<ILogger<IExceptionMiddleware>>();

            var options = new ExceptionMiddlewareOptions();

            configureAction.Invoke(options);

            var handler = new ExceptionMiddlewareHandler(options, log);

            handler.UseMiddleware(builder);
        }

        public static ExceptionMiddlewareOptions AddHandler<T>(this ExceptionMiddlewareOptions options, Func<T, ExceptionResult> handler)
        {
            options.Handlers.Add(new ExceptionHandler
            {
                ExceptionType = typeof(T),
                Handler = handler
            });

            return options;
        }
    }
}

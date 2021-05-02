using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Models
{
    public class ExceptionMiddlewareOptions
    {
        /// <summary>
        /// The list of registered exception handlers for use in the middleware.
        /// </summary>
        public List<ExceptionHandler> Handlers { get; internal set; }

        /// <summary>
        /// Whether to use strict type checking when getting the handler to use. Defaults to true.
        /// </summary>
        /// <remarks>
        /// When true, the middleware uses <see cref="Object.Equals(object?, object?)"/> to determine the handler to use.
        /// When false, the middleware uses <see cref="Type.IsSubclassOf(Type)"/> to determine the handler to use.
        /// </remarks>
        public bool UseStrictExceptionChecking { get; set; } = true;

        /// <summary>
        /// The <see cref="System.Text.Json.JsonSerializerOptions"/> to use when serializing the handled response.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; internal set; }

        /// <summary>
        /// The content type to use when writing the handled response. Defaults to application/json.
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        internal ExceptionMiddlewareOptions()
        {
            JsonSerializerOptions = new JsonSerializerOptions();
        }
    }
}

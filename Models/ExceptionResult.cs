using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Models
{
    /// <summary>
    /// Configure the response to be written by the exception middleware.
    /// </summary>
    public class ExceptionResult
    {
        /// <summary>
        /// The status code to write to the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The error code to write to the response.
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

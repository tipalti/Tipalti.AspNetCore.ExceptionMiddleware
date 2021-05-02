using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Models
{
    /// <summary>
    /// The response used for handled exceptions.
    /// </summary>
    public class ExceptionMiddlewareResponse
    {
        /// <summary>
        /// The error code to write to the response body.
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

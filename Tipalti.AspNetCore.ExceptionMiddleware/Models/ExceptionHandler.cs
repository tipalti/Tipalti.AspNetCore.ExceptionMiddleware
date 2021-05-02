using System;
using System.Collections.Generic;
using System.Text;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Models
{
    internal class ExceptionHandler
    {
        internal Type ExceptionType { get; set; }
        internal Delegate Handler { get; set; }
    }
}

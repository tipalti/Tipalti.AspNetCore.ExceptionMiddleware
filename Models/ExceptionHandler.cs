using System;
using System.Collections.Generic;
using System.Text;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Models
{
    public class ExceptionHandler
    {
        public Type ExceptionType { get; internal set; }
        public Delegate Handler { get; internal set; }
    }
}

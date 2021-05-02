using System;
using System.Collections.Generic;
using System.Text;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Tests.Exceptions
{
    public class TestException : Exception
    {
        public TestException(string errorCode)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; }
    }
}

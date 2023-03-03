using System;
using System.Net;

namespace BlogWebApi.Contracts.Commons
{
    public class BadRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public BadRequestException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
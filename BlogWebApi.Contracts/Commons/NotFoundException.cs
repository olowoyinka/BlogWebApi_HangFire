using System;
using System.Net;

namespace BlogWebApi.Contracts.Commons
{
    public class NotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }


        public NotFoundException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
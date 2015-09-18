namespace Exceptions
{
    using System;
    using System.Net;

    public class InvalidApiRequestException : Exception
    {
        public InvalidApiRequestException(HttpStatusCode httpStatusCode, string[] errors, string httpResponsePhrase)
            : base(httpResponsePhrase + " Check `Errors` for a list of errors returned by the API.")
        {
            this.ResponseStatusCode = httpStatusCode;
            this.Errors = errors;
        }

        public string[] Errors { get; set; }

        public HttpStatusCode ResponseStatusCode { get; private set; }
    }
}
namespace Exceptions
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;

    [Serializable]
    public class SendGridResponseException : Exception
    {
        public SendGridResponseException()
        {
        }

        public SendGridResponseException(string message)
            : base(message)
        {
        }

        public SendGridResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public SendGridResponseException(string message, Exception inner, HttpResponseMessage response)
            : base(message, inner)
        {
            this.Response = response;
        }

        public SendGridResponseException(string message, HttpResponseMessage response)
            : base(message)
        {
            this.Response = response;
        }

        protected SendGridResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpResponseMessage Response { get; set; }
    }
}
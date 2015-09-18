namespace SendGrid
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    using Exceptions;

    public static class ErrorChecker
    {
        public static void CheckForErrors(HttpResponseMessage response)
        {
            CheckForErrors(response, response.Content.ReadAsStreamAsync().Result);
        }

        public static async Task CheckForErrorsAsync(HttpResponseMessage response)
        {
            CheckForErrors(response, await response.Content.ReadAsStreamAsync());
        }

        private static void CheckForErrors(HttpResponseMessage response, Stream stream)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsStartElement())
                            {
                                continue;
                            }

                            switch (reader.Name)
                            {
                                case "result":
                                    continue;
                                case "message":
                                    continue;
                                case "errors":
                                    var messages = GetErrors(reader).ToArray();
                                    throw new InvalidApiRequestException(response.StatusCode, messages, response.ReasonPhrase);
                                case "error":
                                    throw new SendGridResponseException("Unexpected element: " + reader.Name, response);
                                default:
                                    throw new SendGridResponseException("Unknown element: " + reader.Name, response);
                            }
                        }
                    }
                }
                catch (XmlException exception)
                {
                    throw new SendGridResponseException("Parsing error, see response", exception, response);
                }
            }
        }

        private static IEnumerable<string> GetErrors(XmlReader reader)
        {
            const string ErrorTag = "error";

            if (reader.ReadToDescendant(ErrorTag))
            {
                do
                {
                    yield return reader.ReadElementContentAsString();
                }
                while (reader.Name == ErrorTag);
            }
        }
    }
}
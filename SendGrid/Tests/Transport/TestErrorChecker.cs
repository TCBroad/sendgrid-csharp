namespace Transport
{
    #region Using Directives

    using System;
    using System.Net;
    using System.Net.Http;

    using Exceptions;

    using NUnit.Framework;

    using SendGrid;

    #endregion

    [TestFixture]
    public class TestErrorChecker
    {
        private const string BadUsernameOrPasswordResponseMessage = "<result><message>error</message><errors><error>Bad username / password</error></errors></result>";
        private const string MultipleErrorResponseMessage = "<result><message>error</message><errors><error>Bad username / password</error><error>Another error</error></errors></result>";
        private const string InvalidContentMessage = "This is just a string!";
        private const string InvalidContentMessage2 = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml""><head><title>some html</title></head><body><p>This is not XML</p></body></html>";

        [Test]
        [ExpectedException(typeof(InvalidApiRequestException))]
        public void WhenHttpResponseContainsBadUserErrorItIsDetectedAndAInvalidApiRequestIsThrown()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(BadUsernameOrPasswordResponseMessage)
            };

            ErrorChecker.CheckForErrors(response);
        }


        [Test]
        [ExpectedException(typeof(SendGridResponseException))]
        public void WhenHttpResponseContainsInvalidContentAndASendGridResponseExceptionIsThrown()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(InvalidContentMessage)
            };

            ErrorChecker.CheckForErrors(response);
        }

        [Test]
        [ExpectedException(typeof(SendGridResponseException))]
        public void WhenHttpResponseContainsInvalidContentAndASendGridResponseExceptionIsThrown2()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(InvalidContentMessage2)
            };

            ErrorChecker.CheckForErrors(response);
        }

        [Test]
        public void WhenHttpResponseContainsMultipleErrorsItIsDetectedAndAInvalidApiRequestIsThrown()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(MultipleErrorResponseMessage)
            };

            var exception = Assert.Catch<InvalidApiRequestException>(() => ErrorChecker.CheckForErrors(response));

            Assert.AreEqual(exception.Errors.Length, 2);
        }
    }
}
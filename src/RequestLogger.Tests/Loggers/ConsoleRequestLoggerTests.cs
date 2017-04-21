using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RequestLogger.Loggers;

namespace RequestLogger.Tests.Loggers
{
    [TestFixture]
    public class ConsoleRequestLoggerTests
    {
        [SetUp]
        public void SetUp()
        {
            _systemConsole = new Mock<ISystemConsole>();
            _requestLogger = new ConsoleRequestLogger(_systemConsole.Object);
        }

        private Mock<ISystemConsole> _systemConsole;
        private IRequestLogger _requestLogger;

        [Test]
        public void Log_Should_Write_To_SystemConsole()
        {
            var requestData = new RequestData();
            var responseData = new ResponseData();

            _requestLogger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(8));
        }

        [Test]
        public void Log_Should_Write_RequestData_To_SystemConsole()
        {
            var requestData = new RequestData
            {
                HttpMethod = "GET",
                Url = new Uri("http://localhost/test.aspx"),
                Header = new Dictionary<string, string[]>
                {
                    {"Content-Type", new[] {"application/json"}}
                }
            };
            var responseData = new ResponseData();

            _requestLogger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.HttpMethod: {0}",
                It.Is<object[]>(o => o.Contains(requestData.HttpMethod))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.Url: {0}", 
                It.Is<object[]>(o => o.Contains(requestData.Url))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.Header: {0}",
                It.Is<object[]>(o => o.Contains("Content-Type: [application/json]"))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.Content: {0}", 
                It.Is<object[]>(o => o.Contains(""))), Times.Once);
        }

        [Test]
        public void Log_Should_Write_ResponseData_To_SystemConsole()
        {
            var requestData = new RequestData();
            var responseData = new ResponseData
            {
                StatusCode = 200,
                ReasonPhrase = "OK",
                Header = new Dictionary<string, string[]>
                {
                    {"Content-Type", new[] {"application/json"}}
                }
            };

            _requestLogger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.StatusCode: {0}", 
                It.Is<object[]>(o => o.Contains(responseData.StatusCode))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.ReasonPhrase: {0}", 
                It.Is<object[]>(o => o.Contains(responseData.ReasonPhrase))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.Header: {0}", 
                It.Is<object[]>(o => o.Contains("Content-Type: [application/json]"))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.Content: {0}", 
                It.Is<object[]>(o => o.Contains(""))), Times.Once);
        }

        [Test]
        public void LogError_Should_Write_Exception_To_SystemConsole()
        {
            var requestData = new RequestData();
            var responseData = new ResponseData();
            var exception = new Exception();

            _requestLogger.LogError(requestData, responseData, exception);

            _systemConsole.Verify(x => x.WriteError(exception), Times.Once);
        }
    }
}

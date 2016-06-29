using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RequestLogger.Loggers.Wrappers;

namespace RequestLogger.Loggers.Tests
{
    [TestFixture]
    public class ConsoleLoggerTests
    {
        private Mock<ISystemConsole> _systemConsole;
        private ConsoleLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _systemConsole = new Mock<ISystemConsole>();
            _logger = new ConsoleLogger(new ConsoleLoggerConfiguration
            {
                SystemConsole = _systemConsole.Object
            });
        }

        [TearDown]
        public void TearDown()
        {
            _logger = null;
            _systemConsole = null;
        }

        [Test]
        public void Log_Should_Write_To_SystemConsole()
        {
            var requestData = new RequestData();
            var responseData = new ResponseData();

            _logger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(8));
        }

        [Test]
        public void Log_Should_Write_RequestData_To_SystemConsole()
        {
            var requestData = new RequestData
            {
                HttpMethod = "GET",
                Url = new Uri("http://localhost/test.aspx"),
                Header = new Dictionary<string, string[]>()
            };
            var responseData = new ResponseData();

            _logger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.HttpMethod: {0}",
                It.Is<object[]>(o => o.Contains(requestData.HttpMethod))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.Url: {0}", 
                It.Is<object[]>(o => o.Contains(requestData.Url))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "RequestData.Header: {0}", 
                It.Is<object[]>(o => o.Contains("{}"))), Times.Once);
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
                Header = new Dictionary<string, string[]>()
            };

            _logger.Log(requestData, responseData);

            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.StatusCode: {0}", 
                It.Is<object[]>(o => o.Contains(responseData.StatusCode))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.ReasonPhrase: {0}", 
                It.Is<object[]>(o => o.Contains(responseData.ReasonPhrase))), Times.Once);
            _systemConsole.Verify(x => x.WriteLine(
                "ResponseData.Header: {0}", 
                It.Is<object[]>(o => o.Contains("{}"))), Times.Once);
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

            _logger.LogError(requestData, responseData, exception);

            _systemConsole.Verify(x => x.WriteError(exception), Times.Once);
        }
    }
}

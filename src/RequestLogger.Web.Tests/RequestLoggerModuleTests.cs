using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Moq;
using NUnit.Framework;

namespace RequestLogger.Web.Tests
{
    [TestFixture]
    [Parallelizable]
    public class RequestLoggerModuleTests
    {
        [SetUp]
        public void SetUp()
        {
            _request = new Mock<HttpRequestBase>();
            _response = new Mock<HttpResponseBase>();
            _server = new Mock<HttpServerUtilityBase>();
            _context = new Mock<HttpContextBase>();
            _logger = new Mock<IRequestLogger>();
            _module = new RequestLoggerModule(_logger.Object);

            _context.SetupGet(x => x.Request).Returns(_request.Object);
            _context.SetupGet(x => x.Response).Returns(_response.Object);
            _context.SetupGet(x => x.Server).Returns(_server.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _module = null;
            _logger = null;
            _context = null;
            _server = null;
            _response = null;
            _request = null;
        }

        private Mock<HttpRequestBase> _request;
        private Mock<HttpResponseBase> _response;
        private Mock<HttpServerUtilityBase> _server;
        private Mock<HttpContextBase> _context;
        private Mock<IRequestLogger> _logger;
        private RequestLoggerModule _module;
        
        [Test]
        public void OnBeginRequest_Should_Set_Response_Filter()
        {
            _module.OnBeginRequest(_context.Object);

            _response.VerifySet(x => x.Filter = It.IsAny<Stream>(), Times.Once);
        }

        [Test]
        public void OnBeginRequest_With_axd_Extension_Should_Not_Set_Response_Filter()
        {
            _request.SetupGet(x => x.CurrentExecutionFilePath).Returns("test.axd");

            _module.OnBeginRequest(_context.Object);

            _response.VerifySet(x => x.Filter = It.IsAny<Stream>(), Times.Never);
        }

        [Test]
        public void OnEndRequest_Should_Log()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);

            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(It.IsAny<RequestData>(), It.IsAny<ResponseData>()), Times.Once);
        }

        [Test]
        public void OnEndRequest_Should_Log_RequestHeaders()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var headers = new NameValueCollection
            {
                {"Header 1", "My Value"},
                {"Header 2", "My Other Value"}
            };


            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(headers);
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(
                    It.Is<RequestData>(r =>
                        r.Header.ContainsKey("Header 1") &&
                        r.Header["Header 1"].Length == 1 &&
                        r.Header["Header 1"][0] == "My Value"),
                    It.IsAny<ResponseData>()),
                Times.Once);
            _logger.Verify(x => x.Log(
                    It.Is<RequestData>(r =>
                        r.Header.ContainsKey("Header 2") &&
                        r.Header["Header 2"].Length == 1 &&
                        r.Header["Header 2"][0] == "My Other Value"),
                    It.IsAny<ResponseData>()),
                Times.Once);
        }

        [Test]
        public void OnEndRequest_Should_Log_ResponseHeaders()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var headers = new NameValueCollection
            {
                {"Header 1", "My Value"},
                {"Header 2", "My Other Value"}
            };


            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(headers);
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r =>
                        r.Header.ContainsKey("Header 1") &&
                        r.Header["Header 1"].Length == 1 &&
                        r.Header["Header 1"][0] == "My Value")),
                Times.Once);
            _logger.Verify(x => x.Log(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r =>
                        r.Header.ContainsKey("Header 2") &&
                        r.Header["Header 2"].Length == 1 &&
                        r.Header["Header 2"][0] == "My Other Value")),
                Times.Once);
        }

        [Test]
        public void OnEndRequest_Should_Log_RequestContent()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            var inputStream = new MemoryStream(data);
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            
            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(
                    It.Is<RequestData>(r => r.Content.SequenceEqual(data)),
                    It.IsAny<ResponseData>()),
                Times.Once);
        }

        [Test]
        public void OnEndRequest_Should_Log_ResponseContent()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            var inputStream = new Mock<Stream>();
            var outputStream = new MemoryStream();
            var responseFilter = new ResponseFilterStream(outputStream);
            responseFilter.Write(data, 0, data.Length);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r => r.Content.SequenceEqual(data))),
                Times.Once);
        }

        [Test]
        public void OnEndRequest_With_axd_Extension_Should_Not_Log()
        {
            _request.SetupGet(x => x.CurrentExecutionFilePath).Returns("test.axd");

            _module.OnEndRequest(_context.Object);

            _logger.Verify(x => x.Log(It.IsAny<RequestData>(), It.IsAny<ResponseData>()), Times.Never);
        }

        [Test]
        public void OnError_Should_Log()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var exception = new Exception("Test Error");

            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);
            _server.Setup(x => x.GetLastError()).Returns(exception);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(It.IsAny<RequestData>(), It.IsAny<ResponseData>(), exception), Times.Once);
        }

        [Test]
        public void OnError_Should_Log_RequestHeaders()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var headers = new NameValueCollection
            {
                {"Header 1", "My Value"},
                {"Header 2", "My Other Value"}
            };


            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(headers);
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(
                    It.Is<RequestData>(r =>
                        r.Header.ContainsKey("Header 1") &&
                        r.Header["Header 1"].Length == 1 &&
                        r.Header["Header 1"][0] == "My Value"),
                    It.IsAny<ResponseData>(),
                    It.IsAny<Exception>()),
                Times.Once);
            _logger.Verify(x => x.LogError(
                    It.Is<RequestData>(r =>
                        r.Header.ContainsKey("Header 2") &&
                        r.Header["Header 2"].Length == 1 &&
                        r.Header["Header 2"][0] == "My Other Value"),
                    It.IsAny<ResponseData>(),
                    It.IsAny<Exception>()),
                Times.Once);
        }

        [Test]
        public void OnError_Should_Log_ResponseHeaders()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var headers = new NameValueCollection
            {
                {"Header 1", "My Value"},
                {"Header 2", "My Other Value"}
            };


            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(headers);
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r =>
                        r.Header.ContainsKey("Header 1") &&
                        r.Header["Header 1"].Length == 1 &&
                        r.Header["Header 1"][0] == "My Value"),
                    It.IsAny<Exception>()),
                Times.Once);
            _logger.Verify(x => x.LogError(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r =>
                        r.Header.ContainsKey("Header 2") &&
                        r.Header["Header 2"].Length == 1 &&
                        r.Header["Header 2"][0] == "My Other Value"),
                    It.IsAny<Exception>()),
                Times.Once);
        }

        [Test]
        public void OnError_Should_Log_RequestContent()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            var inputStream = new MemoryStream(data);
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(
                    It.Is<RequestData>(r => r.Content.SequenceEqual(data)),
                    It.IsAny<ResponseData>(),
                    It.IsAny<Exception>()),
                Times.Once);
        }

        [Test]
        public void OnError_Should_Log_ResponseContent()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            var inputStream = new Mock<Stream>();
            var outputStream = new MemoryStream();
            var responseFilter = new ResponseFilterStream(outputStream);
            responseFilter.Write(data, 0, data.Length);

            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(
                    It.IsAny<RequestData>(),
                    It.Is<ResponseData>(r => r.Content.SequenceEqual(data)),
                    It.IsAny<Exception>()),
                Times.Once);
        }

        [Test]
        public void OnError_With_axd_Extension_Should_Log()
        {
            var inputStream = new Mock<Stream>();
            var outputStream = new Mock<Stream>();
            var responseFilter = new ResponseFilterStream(outputStream.Object);
            var exception = new Exception("Test Error");

            inputStream.SetupGet(x => x.CanSeek).Returns(false);

            _request.SetupGet(x => x.CurrentExecutionFilePath).Returns("test.axd");
            _request.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _request.SetupGet(x => x.InputStream).Returns(inputStream.Object);
            _response.SetupGet(x => x.Headers).Returns(new NameValueCollection());
            _response.SetupGet(x => x.Filter).Returns(responseFilter);
            _server.Setup(x => x.GetLastError()).Returns(exception);

            _module.OnError(_context.Object);

            _logger.Verify(x => x.LogError(It.IsAny<RequestData>(), It.IsAny<ResponseData>(), exception), Times.Once);
        }
    }
}
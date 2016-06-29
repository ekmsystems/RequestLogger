using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Moq;
using NUnit.Framework;

namespace RequestLogger.Web.Tests
{
    [TestFixture]
    public class RequestLoggerModuleTests
    {
        private Mock<HttpRequestBase> _request;
        private Mock<HttpResponseBase> _response;
        private Mock<HttpServerUtilityBase> _server;
        private Mock<HttpContextBase> _context;
        private Mock<IRequestLogger> _logger;
        private RequestLoggerModule _module;

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

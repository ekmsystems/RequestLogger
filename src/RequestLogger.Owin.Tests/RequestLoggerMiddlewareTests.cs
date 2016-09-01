using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Moq;
using NUnit.Framework;
using Owin;

namespace RequestLogger.Owin.Tests
{
    [TestFixture]
    public class RequestLoggerMiddlewareTests
    {
        private Mock<IRequestLogger> _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<IRequestLogger>();
        }

        [TearDown]
        public void TearDown()
        {
            _logger = null;
        }

        [Test]
        public async Task Should_Log()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Use(typeof(RequestLoggerMiddleware), _logger.Object);
                app.Run(async context => await context.Response.WriteAsync("Test Server"));
            }))
            {
                await server.HttpClient.GetAsync("/");

                _logger.Verify(x => x.Log(It.IsAny<RequestData>(), It.IsAny<ResponseData>()), Times.Once);
            }
        }

        [Test]
        public async Task When_ErrorIsThrown_Should_LogError()
        {
            var ex = new SuccessException("Test Error");

            _logger
                .Setup(x => x.Log(It.IsAny<RequestData>(), It.IsAny<ResponseData>()))
                .Throws(ex);

            using (var server = TestServer.Create(app =>
            {
                app.Use(typeof(RequestLoggerMiddleware), _logger.Object);
                app.Run(async context => await context.Response.WriteAsync("Test Server"));
            }))
            {
                await server.HttpClient.GetAsync("/");

                _logger.Verify(x => x.LogError(It.IsAny<RequestData>(), It.IsAny<ResponseData>(), ex), Times.Once);
            }
        }

        [Test]
        public async Task Should_Reset_RequestBody()
        {
            const string expectedContent = "This is a request";
            var result = string.Empty;

            using (var server = TestServer.Create(app =>
            {
                app.Use(typeof(RequestLoggerMiddleware), _logger.Object);
                app.Run(async context =>
                {
                    result = new StreamReader(context.Request.Body).ReadToEnd();

                    await context.Response.WriteAsync("Test Server");
                });
            }))
            {
                await server.HttpClient.PostAsync("/", new StringContent(expectedContent));

                Assert.AreEqual(expectedContent, result);
            }
        }

        [Test]
        public async Task Should_Reset_ResponseBody()
        {
            const string expectedContent = "This is a response";

            using (var server = TestServer.Create(app =>
            {
                app.Use(typeof(RequestLoggerMiddleware), _logger.Object);
                app.Run(async context =>
                {
                    await context.Response.WriteAsync(expectedContent);
                });
            }))
            {
                var response = await server.HttpClient.GetAsync("/");
                var result = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(expectedContent, result);
            }
        }
    }
}

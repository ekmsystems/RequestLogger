using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Moq;
using NUnit.Framework;
using Owin;

namespace RequestLogger.Owin.Tests
{
    [TestFixture]
    public class RequestLoggerMiddlewareExtensionsTests
    {
        [Test]
        public async Task UseRequestLoggerMiddleware_Should_AttachMiddleware()
        {
            var requestLogger = new Mock<IRequestLogger>();

            using (var server = TestServer.Create(app =>
            {
                app.UseRequestLoggerMiddleware(requestLogger.Object);
                app.Run(async context => await context.Response.WriteAsync("Test Server"));
            }))
            {
                await server.HttpClient.GetAsync("/");

                requestLogger.Verify(x => x.Log(It.IsAny<RequestData>(), It.IsAny<ResponseData>()), Times.Once);
            }
        }
    }
}

using System;
using NUnit.Framework;
using RequestLogger.Loggers;

namespace RequestLogger.Tests.Loggers
{
    [TestFixture]
    public class NullRequestLoggerTests
    {
        [SetUp]
        public void SetUp()
        {
            _requestLogger = new NullRequestLogger();
        }

        private NullRequestLogger _requestLogger;

        [Test]
        public void Log_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => _requestLogger.Log(new RequestData(), new ResponseData()));
        }

        [Test]
        public void LogError_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => _requestLogger.LogError(new RequestData(), new ResponseData(), new Exception()));
        }
    }
}

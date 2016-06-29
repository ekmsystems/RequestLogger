using System;
using NUnit.Framework;

namespace RequestLogger.Loggers.Tests
{
    [TestFixture]
    public class NullLoggerTests
    {
        private NullLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new NullLogger();
        }

        [TearDown]
        public void TearDown()
        {
            _logger = null;
        }

        [Test]
        public void Log_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => _logger.Log(new RequestData(), new ResponseData()));
        }

        [Test]
        public void LogError_Should_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => _logger.LogError(new RequestData(), new ResponseData(), new Exception()));
        }
    }
}

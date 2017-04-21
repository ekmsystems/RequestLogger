using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;

namespace RequestLogger.Web.Tests
{
    [TestFixture]
    [Parallelizable]
    public class StreamExtensionsTests
    {
        [Test]
        public void CopyTo_Should_Copy_Stream()
        {
            var data = Encoding.UTF8.GetBytes("This is some data");
            var src = new MemoryStream(data);
            var dst = new MemoryStream();

            StreamExtensions.CopyTo(src, dst);

            Assert.AreEqual(data, dst.ToArray());
        }

        [Test]
        public void ReadStream_Should_Read_Stream()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            using (var stream = new MemoryStream(data))
            {
                var result = stream.ReadStream();

                Assert.AreEqual(data, result);
            }
        }

        [Test]
        public void ReadStream_Should_ReadFromTheStart()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            using (var stream = new MemoryStream(data))
            {
                stream.Position = 1;

                var result = stream.ReadStream();

                Assert.AreEqual(data, result);
            }
        }

        [Test]
        public void ReadStream_When_CanRead_IsFalse_ShouldReturn_EmptyArray()
        {
            var stream = new Mock<Stream>();

            stream.SetupGet(x => x.CanRead).Returns(false);

            var result = stream.Object.ReadStream();

            Assert.IsEmpty(result);
        }

        [Test]
        public void ReadStream_When_CanSeek_IsFalse_ShouldReturn_EmptyArray()
        {
            var stream = new Mock<Stream>();

            stream.SetupGet(x => x.CanSeek).Returns(false);

            var result = stream.Object.ReadStream();

            Assert.IsEmpty(result);
        }
    }
}
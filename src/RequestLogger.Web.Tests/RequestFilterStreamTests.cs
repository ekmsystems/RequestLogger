using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;

namespace RequestLogger.Web.Tests
{
    [TestFixture]
    [Parallelizable]
    public class RequestFilterStreamTests
    {
        [Test]
        public void Flush_Should_Flush_Stream()
        {
            var stream = new Mock<Stream>();
            var filterStream = new ResponseFilterStream(stream.Object);

            filterStream.Flush();

            stream.Verify(x => x.Flush(), Times.Once);
        }

        [Test]
        public void Read_Should_Read_From_Stream()
        {
            var data = Encoding.UTF8.GetBytes("Test");
            var stream = new MemoryStream(data);
            var filterStream = new ResponseFilterStream(stream);
            var buffer = new byte[2];

            var result = filterStream.Read(buffer, 0, 2);

            Assert.AreEqual(2, result);
            Assert.AreEqual(Encoding.UTF8.GetBytes("Te"), buffer);
        }

        [Test]
        public void Seek_Should_Set_Position_On_Stream()
        {
            var stream = new Mock<Stream>();
            var filterStream = new ResponseFilterStream(stream.Object);

            filterStream.Seek(3, SeekOrigin.Begin);

            stream.Verify(x => x.Seek(3, SeekOrigin.Begin), Times.Once);
        }

        [Test]
        public void Set_Position_Should_Set_Position_On_Stream()
        {
            var stream = new Mock<Stream>();

            using (new ResponseFilterStream(stream.Object) {Position = 3})
            {
            }

            stream.VerifySet(x => x.Position = 3);
        }

        [Test]
        public void SetLength_Should_Set_Length_On_Stream()
        {
            var stream = new Mock<Stream>();
            var filterStream = new ResponseFilterStream(stream.Object);

            filterStream.SetLength(2);

            stream.Verify(x => x.SetLength(2), Times.Once);
        }

        [Test]
        public void Should_Use_Passed_In_Stream_Properties()
        {
            var stream = new Mock<Stream>();

            stream.SetupGet(x => x.CanRead).Returns(true);
            stream.SetupGet(x => x.CanSeek).Returns(true);
            stream.SetupGet(x => x.CanWrite).Returns(true);
            stream.SetupGet(x => x.Length).Returns(30);
            stream.SetupGet(x => x.Position).Returns(21);

            var filterStream = new ResponseFilterStream(stream.Object);

            Assert.IsTrue(filterStream.CanRead);
            Assert.IsTrue(filterStream.CanSeek);
            Assert.IsTrue(filterStream.CanWrite);
            Assert.AreEqual(30, filterStream.Length);
            Assert.AreEqual(21, filterStream.Position);
        }

        [Test]
        public void Write_Should_Write_To_Stream()
        {
            var stream = new Mock<Stream>();
            var filterStream = new ResponseFilterStream(stream.Object);
            var buffer = Encoding.UTF8.GetBytes("Test");

            filterStream.Write(buffer, 0, buffer.Length);

            stream.Verify(x => x.Write(buffer, 0, buffer.Length), Times.Once);
        }
    }
}
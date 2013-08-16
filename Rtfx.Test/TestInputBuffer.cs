using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Rtfx.Test {
    [TestClass]
    public class TestInputBuffer {
        private InputBuffer StringBuffer(string input) {
            return new InputBuffer(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        }

        [TestMethod]
        public void TestAt() {
            using (var buffer = StringBuffer("Testing")) {
                Assert.AreEqual((byte) 'T', buffer[0]);
                Assert.AreEqual((byte) 'i', buffer[4]);
            }
        }

        [TestMethod]
        public void TestDiscard() {
            using (var buffer = StringBuffer("Testing")) {
                Assert.AreEqual((byte) 'T', buffer[0]);
                buffer.Discard(2);
                Assert.AreEqual((byte) 's', buffer[0]);
            }
        }

        [TestMethod]
        public void TestConsume() {
            using (var buffer = StringBuffer("Testing")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("Test"), buffer.Consume(4));
            }
        }

        [TestMethod]
        public void TestSpan() {
            using (var buffer = StringBuffer("Testing")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("stin"), buffer.Span(2, 4));
            }
        }

        [TestMethod]
        public void TestEof() {
            using (var buffer = StringBuffer("This is a test of the emergency broadcast system.")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This is a test"), buffer.Consume(14));
                Assert.IsFalse(buffer.Eof);
                buffer.Discard(1);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("of"), buffer.Consume(2));
                Assert.IsFalse(buffer.Eof);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes(" the emergency broadcast "), buffer.Consume(25));
                Assert.IsFalse(buffer.Eof);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("system."), buffer.Span(7));
                Assert.IsFalse(buffer.Eof);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("system."), buffer.Consume(7));
                Assert.IsTrue(buffer.Eof);
            }
        }

        [TestMethod]
        public void TestConsumeUntil() {
            using (var buffer = StringBuffer("This is a test of the emergency broadcast system.")) {
                Assert.AreEqual((byte) 'T', buffer[0]);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This is a "), buffer.ConsumeUntil(Encoding.UTF8.GetBytes("test")));
            }
        }

        [TestMethod]
        public void TestConsumeWhile() {
            using (var buffer = StringBuffer("This is a test\nof the emergency broadcast system.")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This is a test"),
                    buffer.ConsumeWhile(c => char.IsLetter((char) c) || c == ' '));
                Assert.AreEqual((byte)'\n', buffer[0]);
            }
            using (var buffer = StringBuffer("This is a test\nof the emergency broadcast system.")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This is a"),
                    buffer.ConsumeWhile(c => char.IsLetter((char) c) || c == ' ', 9));
            }
        }

        [TestMethod]
        public void TestReadShortString() {
            using (var buffer = StringBuffer("This is a test.")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("is a"), buffer.Span(5, 4));
                Assert.AreEqual((byte) 'T', buffer[0]);
                Assert.AreEqual((byte) 'h', buffer[1]);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This"), buffer.Consume(4));
                Assert.AreEqual((byte) ' ', buffer[0]);
            }
        }

        [TestMethod]
        public void TestReadMultilineString() {
            using (var buffer = StringBuffer("This is a test.\r\nThis is another test.")) {
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("is a"), buffer.Span(5, 4));
                Assert.AreEqual((byte) 'T', buffer[0]);
                Assert.AreEqual((byte) 'h', buffer[1]);
                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes("This"), buffer.Consume(4));
                Assert.AreEqual((byte) ' ', buffer[0]);

                AlsoAssert.AreEqual(Encoding.UTF8.GetBytes(" test.\r\nThis is"), buffer.Span(5, 15));
            }
        }

        [TestMethod]
        public void TestReadLongString() {
            using (var input = Resource.Stream("Rtfx.Test.TestData.TestDocument003.txt"))
            using (var buffer = new InputBuffer(input, 16)) {
                var test = "tattoo pre- cartel alcohol fluidity long-chain hydrocarbons";
                Assert.AreEqual(test, Encoding.UTF8.GetString(buffer.Consume(test.Length)));
                test = " human woman numinous carbon";
                Assert.AreEqual(test, Encoding.UTF8.GetString(buffer.Span(0, test.Length)));
                test = "মহিলার numinous কার্বন স্থান কুরিয়ার";
                Assert.AreEqual(test, Encoding.UTF8.GetString(buffer.Span(2284, Encoding.UTF8.GetByteCount(test))));
            }
        }
    }
}
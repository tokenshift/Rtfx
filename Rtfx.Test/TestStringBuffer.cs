using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.RegularExpressions;

namespace Rtfx.Test {
    [TestClass]
    public class TestStringBuffer {
        [TestMethod]
        public void TestCharAt() {
            var input = new StringReader("Testing");
            var buffer = new StringBuffer(input);

            Assert.AreEqual('T', buffer[0]);
            Assert.AreEqual('i', buffer[4]);
        }

        [TestMethod]
        public void TestDiscard() {
            var input = new StringReader("Testing");
            var buffer = new StringBuffer(input);

            Assert.AreEqual('T', buffer[0]);
            buffer.Discard(2);
            Assert.AreEqual('s', buffer[0]);
        }

        [TestMethod]
        public void TestConsume() {
            var input = new StringReader("Testing");
            var buffer = new StringBuffer(input);

            Assert.AreEqual("Test", buffer.Consume(4));
        }

        [TestMethod]
        public void TestSpan() {
            var input = new StringReader("Testing");
            var buffer = new StringBuffer(input);

            Assert.AreEqual("stin", buffer.Span(2, 4));
        }

        [TestMethod]
        public void TestEof() {
            var input = new StringReader("This is a test of the emergency broadcast system.");
            var buffer = new StringBuffer(input, 4);

            Assert.AreEqual("This is a test", buffer.Consume(14));
            Assert.IsFalse(buffer.Eof);
            buffer.Discard(1);
            Assert.AreEqual("of", buffer.Consume(2));
            Assert.IsFalse(buffer.Eof);
            Assert.AreEqual(" the emergency broadcast ", buffer.Consume(25));
            Assert.IsFalse(buffer.Eof);
            Assert.AreEqual("system.", buffer.Span(7));
            Assert.IsFalse(buffer.Eof);
            Assert.AreEqual("system.", buffer.Consume(7));
            Assert.IsTrue(buffer.Eof);
        }

        [TestMethod]
        public void TestConsumeUntil() {
            var input = new StringReader("This is a test of the emergency broadcast system.");
            var buffer = new StringBuffer(input);

            var pattern = new Regex(@"\s");
            Assert.AreEqual("This", buffer.ConsumeUntil(pattern));
            Assert.AreEqual(' ', buffer[0]);
            Assert.AreEqual(" is a ", buffer.ConsumeUntil("test"));
            Assert.AreEqual("test", buffer.ConsumeUntil(' ', '\t'));
        }

        [TestMethod]
        public void TestConsumeWhile() {
            using (var input = new StringReader("This is a test\nof the emergency broadcast system."))
            using (var buffer = new StringBuffer(input)) {
                var pattern = new Regex(@"[a-z ]", RegexOptions.IgnoreCase);
                Assert.AreEqual("This is a test", buffer.ConsumeWhile(c => pattern.IsMatch(new string(c, 1))));
                Assert.AreEqual('\n', buffer[0]);
            }
            using (var input = new StringReader("This is a test\nof the emergency broadcast system."))
            using (var buffer = new StringBuffer(input)) {
                var pattern = new Regex(@"[a-z ]", RegexOptions.IgnoreCase);
                Assert.AreEqual("This is a", buffer.ConsumeWhile(c => pattern.IsMatch(new string(c, 1)), 9));
                Assert.AreEqual(' ', buffer[0]);
            }
        }

        [TestMethod]
        public void TestReadShortString() {
            var input = new StringReader("This is a test.");
            var buffer = new StringBuffer(input);

            Assert.AreEqual("is a", buffer.Span(5, 4));
            Assert.AreEqual('T', buffer[0]);
            Assert.AreEqual('h', buffer[1]);
            Assert.AreEqual("This", buffer.Consume(4));
            Assert.AreEqual(' ', buffer[0]);
        }

        [TestMethod]
        public void TestReadMultilineString() {
            var input = new StringReader("This is a test.\r\nThis is another test.");
            var buffer = new StringBuffer(input);

            Assert.AreEqual("is a", buffer.Span(5, 4));
            Assert.AreEqual('T', buffer[0]);
            Assert.AreEqual('h', buffer[1]);
            Assert.AreEqual("This", buffer.Consume(4));
            Assert.AreEqual(' ', buffer[0]);

            Assert.AreEqual(" test.\r\nThis is", buffer.Span(5, 15));
        }

        [TestMethod]
        public void TestReadUnicode() {
            var input = new StringReader("Thiঅস্থিরতাa test.");
            var buffer = new StringBuffer(input);

            Assert.AreEqual("স্থিরতাa ", buffer.Span(4, 9));
            Assert.AreEqual('T', buffer[0]);
            Assert.AreEqual('h', buffer[1]);
            Assert.AreEqual("Thiঅ", buffer.Consume(4));
            Assert.AreEqual('স', buffer[0]);
        }

        [TestMethod]
        public void TestReadLongString() {
            var input = Resource.Reader("Rtfx.Test.TestData.TestDocument003.txt");
            var buffer = new StringBuffer(input, 16);

            var test = "tattoo pre- cartel alcohol fluidity long-chain hydrocarbons";
            Assert.AreEqual(test, buffer.Consume(test.Length));
            test = " human woman numinous carbon";
            Assert.AreEqual(test, buffer.Span(0, test.Length));
            test = "মহিলার numinous কার্বন স্থান কুরিয়ার";
            Assert.AreEqual(test, buffer.Span(2174, test.Length));
        }
    }
}
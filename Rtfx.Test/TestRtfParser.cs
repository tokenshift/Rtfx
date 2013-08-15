using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestRtfParser {
        [TestMethod]
        public void TestReadSpan() {
            using (var input = new StringBuffer(new StringReader(@"This is a test\par"))) {
                var span = RtfParser.ReadSpan(input);

                Assert.AreEqual(EventType.Span, span.Type);
                Assert.AreEqual("This is a test", span.Text);

                Assert.AreEqual('\\', input.CharAt(0));
            }
        }

        [TestMethod]
        public void TestReadControlWord() {
            using (var input = new StringBuffer(new StringReader(@"\test "))) {
                var word = RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.IsNull(word.Parameter);

                // Space should have been consumed.
                Assert.IsNull(input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test\foo"))) {
                var word = RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.IsNull(word.Parameter);

                // Slash should not have been consumed.
                Assert.AreEqual('\\', input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test12345 "))) {
                var word = RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.AreEqual(12345, word.Parameter);

                // Space should have been consumed.
                Assert.IsNull(input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test-42foo"))) {
                var word = RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.AreEqual(-42, word.Parameter);

                // Delimiter should not have been consumed.
                Assert.AreEqual('f', input.CharAt(0));
            }
        }

        [TestMethod]
        public void TestReadStarredControlWord() {
            using (var input = new StringBuffer(new StringReader(@"\*\test-42foo"))) {
                var word = RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.AreEqual(-42, word.Parameter);
                Assert.IsTrue(word.Starred);

                // Delimiter should not have been consumed.
                Assert.AreEqual('f', input.CharAt(0));
            }
        }

        [TestMethod]
        public void TestReadGroupStart() {
            using (var input = new StringBuffer(new StringReader(@"{stuff goes here}"))) {
                var token = RtfParser.ReadGroupStart(input);

                Assert.AreEqual(EventType.GroupStart, token.Type);
                Assert.IsNull(token.Text);
                Assert.IsNull(token.Parameter);

                Assert.AreEqual('s', input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"stuff goes here}"))) {
                AlsoAssert.Throws<ParseException>(() =>
                    RtfParser.ReadGroupStart(input));
            }
        }
    }
}
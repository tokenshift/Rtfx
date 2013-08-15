using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestRtfParser {
        [TestMethod]
        public void TestReadControlWord() {
            using (var input = new StringBuffer(new StringReader(@"\test "))) {
                var word = Rtfx.RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.IsNull(word.Parameter);

                // Space should have been consumed.
                Assert.IsNull(input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test\foo"))) {
                var word = Rtfx.RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.IsNull(word.Parameter);

                // Slash should not have been consumed.
                Assert.AreEqual('\\', input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test12345 "))) {
                var word = Rtfx.RtfParser.ReadControlWord(input);

                Assert.AreEqual(EventType.ControlWord, word.Type);
                Assert.AreEqual("test", word.Text);
                Assert.AreEqual(12345, word.Parameter);

                // Space should have been consumed.
                Assert.IsNull(input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"\test-42foo"))) {
                var word = Rtfx.RtfParser.ReadControlWord(input);

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
                var word = Rtfx.RtfParser.ReadControlWord(input);

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
                var token = Rtfx.RtfParser.ReadGroupStart(input);

                Assert.AreEqual(EventType.GroupStart, token.Type);
                Assert.IsNull(token.Text);
                Assert.IsNull(token.Parameter);

                Assert.AreEqual('s', input.CharAt(0));
            }

            using (var input = new StringBuffer(new StringReader(@"stuff goes here}"))) {
                AlsoAssert.Throws<ParseException>(() =>
                    Rtfx.RtfParser.ReadGroupStart(input));
            }
        }
    }
}
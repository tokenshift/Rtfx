using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestWriter {
        [TestMethod]
        public void TestWriteSpan() {
            var buffer = new MemoryStream();

            using (var writer = new Writer(buffer)) {
                writer.Span("This is a test.");
            }

            var result = Encoding.UTF8.GetString(buffer.ToArray());
            Assert.AreEqual("This is a test.", result);
        }

        [TestMethod]
        public void TestWriteUnicodeSpan() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestWriteControlSymbolsInSpan()
        {
            var buffer = new MemoryStream();

            using (var writer = new Writer(buffer)) {
                writer.Span(@"This {is} a \ test.");
            }

            var result = Encoding.UTF8.GetString(buffer.ToArray());
            Assert.AreEqual(@"This \{is\} a \\ test.", result);
        }

        [TestMethod]
        public void TestWriteGroup() {
            var buffer = new MemoryStream();

            using (var writer = new Writer(buffer)) {
                writer.GroupStart();
                writer.Control("par");
                writer.Span("This is a test.");
                writer.GroupEnd();
            }

            var result = Encoding.UTF8.GetString(buffer.ToArray());
            Assert.AreEqual(@"{\par This is a test.}", result);
        }
    }
}

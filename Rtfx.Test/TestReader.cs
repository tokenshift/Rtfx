﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestReader {
        [TestMethod]
        public void TestEmptyDocument() {
            var events = new List<ReadEvent>();
            using (var reader = Reader.Create(Resource.Stream("Rtfx.Test.TestData.TestDocument000.rtf"))) {
                events.AddRange(reader.Read());
            }

            // {\rtf1 }

            Assert.AreEqual(3, events.Count);

            Assert.AreEqual(EventType.GroupStart, events[0].Type);
            Assert.IsNull(events[0].Text);

            Assert.AreEqual(EventType.ControlWord, events[1].Type);
            Assert.AreEqual("rtf", events[1].Text);
            Assert.AreEqual(1, events[1].Parameter);

            Assert.AreEqual(EventType.GroupEnd, events[2].Type);
            Assert.IsNull(events[2].Text);
        }

        [TestMethod]
        public void TestParagraphs() {
            var events = new List<ReadEvent>();
            using (var reader = Reader.Create(Resource.Stream("Rtfx.Test.TestData.TestDocument001.rtf"))) {
                events.AddRange(reader.Read());
            }

            //{\rtf1\pard This is a test. This is only a test.\par This is a test of the emergency broadcast system.}

            Assert.AreEqual(7, events.Count);

            Assert.AreEqual(EventType.GroupStart, events[0].Type);
            Assert.IsNull(events[0].Text);

            Assert.AreEqual(EventType.ControlWord, events[1].Type);
            Assert.AreEqual("rtf", events[1].Text);
            Assert.AreEqual(1, events[1].Parameter);

            Assert.AreEqual(EventType.ControlWord, events[2].Type);
            Assert.AreEqual("pard", events[2].Text);

            Assert.AreEqual(EventType.Span, events[3].Type);
            Assert.AreEqual("This is a test. This is only a test.", events[3].Text);

            Assert.AreEqual(EventType.ControlWord, events[4].Type);
            Assert.AreEqual("par", events[4].Text);

            Assert.AreEqual(EventType.Span, events[5].Type);
            Assert.AreEqual("This is a test of the emergency broadcast system.", events[5].Text);

            Assert.AreEqual(EventType.GroupEnd, events[6].Type);
            Assert.IsNull(events[6].Text);
        }

        [TestMethod]
        public void TestEscapeCharacters() {
            var events = new List<ReadEvent>();
            using (var reader = Reader.Create(Resource.Stream("Rtfx.Test.TestData.TestDocument002.rtf"))) {
                events.AddRange(reader.Read());
            }

            //{ \rtf1\pard This is a test. This is only a test.\par This is a test of escape \\\\\\\\characters in \{\{RTF \}\} \\\\ \}| \\\{\\text \\content.\par}

            Assert.AreEqual(8, events.Count);

            Assert.AreEqual(EventType.GroupStart, events[0].Type);
            Assert.IsNull(events[0].Text);

            Assert.AreEqual(EventType.ControlWord, events[1].Type);
            Assert.AreEqual("rtf", events[1].Text);
            Assert.AreEqual(1, events[1].Parameter);

            Assert.AreEqual(EventType.ControlWord, events[2].Type);
            Assert.AreEqual("pard", events[2].Text);

            Assert.AreEqual(EventType.Span, events[3].Type);
            Assert.AreEqual("This is a test. This is only a test.", events[3].Text);

            Assert.AreEqual(EventType.ControlWord, events[4].Type);
            Assert.AreEqual("par", events[4].Text);

            Assert.AreEqual(EventType.Span, events[5].Type);
            Assert.AreEqual(@"This is a test of escape \\\\characters in {{RTF }} \\ }| \{\text \content.",
                events[5].Text);

            Assert.AreEqual(EventType.ControlWord, events[6].Type);
            Assert.AreEqual("par", events[6].Text);

            Assert.AreEqual(EventType.GroupEnd, events[7].Type);
            Assert.IsNull(events[7].Text);
        }

        [TestMethod]
        public void TestReadUnicodeSpan() {
            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"\u128799? Testing Testing")))
            using (var reader = Reader.Create(buffer)) {
                AlsoAssert.Span(reader, "\uD83D\uDF1F Testing Testing");
                Assert.IsNull(reader.Next());
            }

            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"Testing \u128799? Testing")))
            using (var reader = Reader.Create(buffer)) {
                AlsoAssert.Span(reader, "Testing \uD83D\uDF1F Testing");
                Assert.IsNull(reader.Next());
            }

            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"Testing Testing \u128799?")))
            using (var reader = Reader.Create(buffer)) {
                AlsoAssert.Span(reader, "Testing Testing \uD83D\uDF1F");
                Assert.IsNull(reader.Next());
            }
        }
    }
}
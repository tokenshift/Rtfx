using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestReaderWithWordPadDocuments {
        [TestMethod]
        public void TestEmptyDocument() {
            using (var reader = Reader.Create(Resource.Reader("Rtfx.Test.TestData.EmptyDocument.rtf"))) {
                AlsoAssert.GroupStart(reader);
                AlsoAssert.ControlWord(reader, "rtf", 1);
                AlsoAssert.ControlWord(reader, "ansi");
                AlsoAssert.ControlWord(reader, "ansicpg", 1252);
                AlsoAssert.ControlWord(reader, "deff", 0);
                AlsoAssert.ControlWord(reader, "nouicompat");
                AlsoAssert.ControlWord(reader, "deflang", 1033);
                AlsoAssert.GroupStart(reader);
                AlsoAssert.ControlWord(reader, "fonttbl");
                AlsoAssert.GroupStart(reader);
                AlsoAssert.ControlWord(reader, "f", 0);
                AlsoAssert.ControlWord(reader, "fnil");
                AlsoAssert.ControlWord(reader, "fcharset", 0);
                AlsoAssert.Span(reader, "Calibri;");
                AlsoAssert.GroupEnd(reader);
                AlsoAssert.GroupEnd(reader);
                AlsoAssert.GroupStart(reader);
                AlsoAssert.ControlWord(reader, "generator", true);
                AlsoAssert.Span(reader, "Riched20 6.2.9200");
                AlsoAssert.GroupEnd(reader);
                AlsoAssert.ControlWord(reader, "viewkind", 4);
                AlsoAssert.ControlWord(reader, "uc", 1);
                AlsoAssert.ControlWord(reader, "pard");
                AlsoAssert.ControlWord(reader, "sa", 200);
                AlsoAssert.ControlWord(reader, "sl", 276);
                AlsoAssert.ControlWord(reader, "slmult", 1);
                AlsoAssert.ControlWord(reader, "f", 0);
                AlsoAssert.ControlWord(reader, "fs", 22);
                AlsoAssert.ControlWord(reader, "lang", 9);
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.GroupEnd(reader);
                Assert.IsNull(reader.Next());
            }
        }
    }
}
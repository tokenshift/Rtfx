using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    [TestClass]
    public class TestReaderWithWordPadDocuments {
        [TestMethod]
        public void TestEmptyDocument() {
            var events = new List<ReadEvent>();
            using (var reader = Reader.Create(Resource.Reader("Rtfx.Test.TestData.EmptyDocument.rtf"))) {
                events.AddRange(reader.Read());
            }

            throw new NotImplementedException();
        }
    }
}
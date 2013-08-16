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

        [TestMethod]
        public void TestShortText() {
            using (var reader = Reader.Create(Resource.Reader("Rtfx.Test.TestData.ShortText.rtf"))) {
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
                AlsoAssert.Span(reader, "This is a test.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.GroupEnd(reader);
                Assert.IsNull(reader.Next());
            }
        }

        [TestMethod]
        public void TestSpanFormatting() {
            using (var reader = Reader.Create(Resource.Reader("Rtfx.Test.TestData.SpanFormatting.rtf"))) {
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
                AlsoAssert.Span(reader, "papier-mache otaku ");
                AlsoAssert.ControlWord(reader, "b");
                AlsoAssert.Span(reader, "plastic skyscraper ");
                AlsoAssert.ControlWord(reader, "b", 0);
                AlsoAssert.Span(reader, "sign pistol face forwards apophenia man 3D-printed long-chain hydrocarbons human monofilament. dolphin fluidity carbon numinous Shibuya artisanal cartel cardboard katana lights construct dolphin ");
                AlsoAssert.ControlWord(reader, "i");
                AlsoAssert.Span(reader, "saturation point. Tokyo wonton soup franchise savant systemic tank-traps girl silent vehicle tube -ware monofilament drone. garage ");
                AlsoAssert.ControlWord(reader, "i", 0);
                AlsoAssert.Span(reader, "spook cardboard voodoo god plastic bicycle corporation neural courier San Francisco San Francisco network apophenia. geodesic dome tattoo motion pistol drugs network neural courier alcohol papier-mache sub-orbital ");
                AlsoAssert.ControlWord(reader, "b");
                AlsoAssert.Span(reader, "soul-delay");
                AlsoAssert.ControlWord(reader, "i");
                AlsoAssert.Span(reader, ". sub-orbital drone singularity boat DIY industrial grade math- receding nodal point paranoid ");
                AlsoAssert.ControlWord(reader, "i", 0);
                AlsoAssert.Span(reader, "garage pistol papier-mache");
                AlsoAssert.ControlWord(reader, "b", 0);
                AlsoAssert.Span(reader, ". artisanal rifle neural woman corrupted pen tattoo computer wonton soup table tiger-team -space range-rover. render-farm sub-orbital wristwatch order-flow wristwatch vinyl tiger-team fluidity nodal point youtube smart- paranoid RAF.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.GroupEnd(reader);
                Assert.IsNull(reader.Next());
            }
        }
    }
}
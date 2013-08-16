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

        [TestMethod]
        public void TestMultipleParagraphs() {
            using (var reader = Reader.Create(Resource.Reader("Rtfx.Test.TestData.MultipleParagraphs.rtf"))) {
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
                AlsoAssert.Span(reader, "papier-mache otaku plastic skyscraper sign pistol face forwards apophenia man 3D-printed long-chain hydrocarbons human monofilament. dolphin fluidity carbon numinous Shibuya artisanal cartel cardboard katana lights construct dolphin saturation point. Tokyo wonton soup franchise savant systemic tank-traps girl silent vehicle tube -ware monofilament drone. garage spook cardboard voodoo god plastic bicycle corporation neural courier San Francisco San Francisco network apophenia. geodesic dome tattoo motion pistol drugs network neural courier alcohol papier-mache sub-orbital soul-delay. sub-orbital drone singularity boat DIY industrial grade math- receding nodal point paranoid garage pistol papier-mache. artisanal rifle neural woman corrupted pen tattoo computer wonton soup table tiger-team -space range-rover. render-farm sub-orbital wristwatch order-flow wristwatch vinyl tiger-team fluidity nodal point youtube smart- paranoid RAF.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.Span(reader, "shanty town engine Tokyo Tokyo network saturation point marketing range-rover futurity camera dead papier-mache post-. sub-orbital 3D-printed free-market receding neon soul-delay tiger-team systema assault jeans gang skyscraper boy. smart- gang alcohol franchise Chiba math- cartel modem man j-pop youtube face forwards boat. beef noodles pre- monofilament nodality construct systema footage monofilament table carbon monofilament carbon plastic. narrative franchise shanty town car assault industrial grade bomb stimulate car lights Chiba kanji sensory. grenade beef noodles towards engine boy film dolphin market denim chrome soul-delay skyscraper San Francisco. weathered dolphin monofilament carbon post- assassin hacker physical hotdog semiotics wonton soup grenade A.I.. table hacker man carbon rebar industrial grade lights geodesic sprawl wonton soup wonton soup modem tattoo.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.Span(reader, "futurity denim table disposable smart- savant office chrome plastic semiotics rain disposable San Francisco. sign -space sentient saturation point market motion sign market paranoid pistol rain human artisanal. shanty town nano- numinous wristwatch -ware beef noodles modem faded voodoo god soul-delay voodoo god computer shoes. corporation dome savant corporation cyber- beef noodles nodal point fluidity vinyl apophenia sentient A.I. A.I.. man sprawl rain table girl San Francisco pen corrupted rain dead range-rover free-market drone. numinous paranoid bridge katana cyber- market nano- city Tokyo artisanal paranoid RAF ablative. wonton soup rebar tower render-farm crypto- digital realism lights A.I. paranoid advert media towards. beef noodles futurity pen post- A.I. uplink bridge rebar girl futurity Kowloon dome tanto.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.Span(reader, "garage shanty town otaku wonton soup youtube city tube Kowloon towards Legba katana woman -space. dome cardboard construct sensory render-farm wonton soup realism engine corporation lights Kowloon systema smart-. DIY augmented reality denim tube grenade advert kanji film franchise fetishism augmented reality tiger-team industrial grade. spook engine fetishism kanji fetishism papier-mache BASE jump refrigerator sub-orbital realism motion military-grade shrine. systema meta- hacker pistol tattoo semiotics film decay drugs corrupted rifle cardboard nodality. apophenia 3D-printed industrial grade assassin franchise paranoid hacker engine singularity dead cardboard carbon cardboard. realism woman BASE jump plastic render-farm vinyl katana gang nodal point saturation point face forwards fetishism post-. bridge stimulate tube pen Tokyo tube alcohol cyber- marketing bicycle narrative vinyl systemic.");
                AlsoAssert.ControlWord(reader, "par");
                AlsoAssert.GroupEnd(reader);
                Assert.IsNull(reader.Next());
            }
        }
    }
}
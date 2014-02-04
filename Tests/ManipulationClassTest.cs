
namespace MinifyLibTests {
    using NUnit.Framework;
    using MinifyLib.Manipulate;
    using MinifyLib.Color;
    using System.Collections.Generic;

    [TestFixture]
    public class ManipulationClassTest {

        private Manipulation _manip;

        public void Init( string source ) {
            var comp = new ColorCompressor( new ColorConverter() );
            this._manip = new Manipulation( comp, source );
        }

        [TearDown]
        public void Cleanup() {
            this._manip = null;
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ConstructorCompExceptionTest() {
            this._manip = new Manipulation( null, "string" );
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ConstructorSourceExceptionTest() {
            this._manip = new Manipulation( new ColorCompressor( new ColorConverter() ), null );
        }

        [Test]
        public void PlaceholdersTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.SwapForPlaceholders().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );

            expected = this._manip.BaseString;
            actual = this._manip.ReplacePlaceholders().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void NormalizeSourceTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.NormalizeSource().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void CleanSelectorsTest() {
            this.Init( "@media only screen and (max-width:480px) {\n\t.container [class *= \"grid-\"] " +
                       "{\n\t\twidth: 450px;\n\t}\n}\nol, ul, p, p + p {\n\tmargin-bottom: 30px;\n}\n" +
                       ":root .hidden-chk:checked ~ label {\n\tborder: 1px solid #888;\n\tborder-bottom-color: #aaa;\n}\n" );
            string expected = "@media only screen and (max-width:480px) {\n\t.container [class*=\"grid-\"] " +
                              "{\n\t\twidth:450px;}\n}\nol,ul,p,p+p {\n\tmargin-bottom:30px;}:root " +
                              ".hidden-chk:checked~label {\n\tborder:1px solid #888;border-bottom-color:#aaa;}\n";
            string actual = this._manip.CleanSelectors().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void CleanBracesTest() {
            this.Init( "html {\n\tcolor: rgba( 50, 50, 50, .8 );\n}\n" +
                       "body {\n\tfont: normal normal normal 1rem/1.5 \"OpenSans\", sans-serif;\n}\n" +
                       "@media only screen and (max-width:480px) {\n\tbody { text-rendering: optimizeSpeed; }\n}\n" );
            string expected = "html{color: rgba(50, 50, 50, .8);}body{font: normal normal normal 1rem/1.5 \"OpenSans\", " +
                              "sans-serif;}@media only screen and (max-width:480px){body{text-rendering: optimizeSpeed;}}";
            string actual = this._manip.CleanBraces().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void CleanUnnecessaryTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.CleanUnnecessary().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void ConvertColorsTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.ConvertColors().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void CompressHexValuesTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.CompressHexValues().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void CompressColorNamesTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.CompressColorNames().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        // I should probably make this method private in the class and just
        // call it from within the method that messes them up
        [Test]
        public void FixIllFormedHslTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.FixIllFormedHsl().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void ReplaceTransparentTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.ReplaceTransparent().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }

        [Test]
        public void ReplaceFontWeightTest() {
            this.Init( "" );
            string expected = "";
            string actual = this._manip.ReplaceFontWeight().AlteredString;
            Assert.AreEqual( expected, actual, "Expected " + expected + " but was " + actual );
        }
    }
}

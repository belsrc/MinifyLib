
namespace MinifyLibTests {
    using NUnit.Framework;
    using MinifyLib.Color;
    using System.Collections.Generic;

    [TestFixture]
    public class ColorConverterClassTest {

        private ColorConverter _converter;

        public ColorConverterClassTest() {
            this._converter = new ColorConverter();
        }

        [Test]
        public void ConvertRgbToHexTest() {
            List<byte[]> rgbs = new List<byte[]>() {
                new byte[] { 78, 166, 234 },
                new byte[] { 145, 0, 145 },
                new byte[] { 235, 60, 0 }
            };

            List<string> expecteds = new List<string>() {
                "4ea6ea",
                "910091",
                "eb3c00"
            };

            for( int i = 0; i < rgbs.Count; i++ ) {
                string actual = this._converter.ConvertRgbToHex( rgbs[i][0], rgbs[i][1], rgbs[i][2] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }

        [Test]
        public void ConvertHslToHexTest() {
            List<float[]> hslas = new List<float[]>() {
                new float[] { 206F, 79F, 61F },
                new float[] { 300F, 100F, 28F },
                new float[] { 15F, 100F, 46F }
            };

            List<string> expecteds = new List<string>() {
                "4da6ea",
                "8f008f",
                "eb3b00"
            };

            for( int i = 0; i < hslas.Count; i++ ) {
                string actual = this._converter.ConvertHslToHex( hslas[i][0], hslas[i][1], hslas[i][2] );
                Assert.AreEqual( expecteds[i], actual, "Expected " + expecteds[i] + " but was " + actual );
            }
        }
    }
}
